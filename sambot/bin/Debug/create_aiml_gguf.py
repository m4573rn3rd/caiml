#!/usr/bin/env python3
from __future__ import annotations

import argparse
import base64
import gzip
import hashlib
import json
import sys
from datetime import datetime, timezone
from pathlib import Path
from typing import Dict, List, Optional, Tuple
import xml.etree.ElementTree as ET

import numpy as np


def local_tag_name(tag: str) -> str:
    if "}" in tag:
        return tag.split("}", 1)[1]
    return tag


def resolve_bitnet_root(script_path: Path, provided_root: str) -> Path:
    if provided_root:
        candidate = Path(provided_root).expanduser().resolve()
        if not candidate.exists():
            raise FileNotFoundError("Provided --bitnet-root does not exist: " + str(candidate))
        return candidate

    for candidate in [script_path.parent] + list(script_path.parents):
        if (candidate / "run_inference_server.py").exists():
            return candidate

    raise FileNotFoundError(
        "Could not locate BitNet root. Pass --bitnet-root explicitly."
    )


def add_gguf_python_path(bitnet_root: Path) -> None:
    gguf_path = bitnet_root / "3rdparty" / "llama.cpp" / "gguf-py"
    if not gguf_path.exists():
        raise FileNotFoundError("Missing gguf-py package path: " + str(gguf_path))
    sys.path.insert(0, str(gguf_path))


def discover_aiml_files(aiml_dir: Path) -> List[Path]:
    files: List[Path] = []
    for file_path in aiml_dir.rglob("*"):
        if file_path.is_file() and file_path.suffix.lower() == ".aiml":
            files.append(file_path)
    files.sort(key=lambda file_path: str(file_path).lower())
    return files


def find_direct_child(node: ET.Element, name: str) -> Optional[ET.Element]:
    for child in list(node):
        if local_tag_name(child.tag).lower() == name.lower():
            return child
    return None


def read_node_text(node: Optional[ET.Element], fallback: str) -> str:
    if node is None:
        return fallback
    text = "".join(node.itertext()).strip()
    return text if text else fallback


def build_aiml_path(pattern: str, that: str, topic: str) -> str:
    normalized_pattern = pattern.strip()
    if not normalized_pattern:
        return ""

    normalized_that = that.strip() or "*"
    normalized_topic = topic.strip() or "*"
    return normalized_pattern + " <that> " + normalized_that + " <topic> " + normalized_topic


def parse_category(
    category_node: ET.Element,
    topic_name: str,
    source_file: Path,
    aiml_dir: Path,
) -> Optional[Dict[str, str]]:
    pattern_node = find_direct_child(category_node, "pattern")
    template_node = find_direct_child(category_node, "template")

    if pattern_node is None or template_node is None:
        return None

    pattern = read_node_text(pattern_node, "")
    that = read_node_text(find_direct_child(category_node, "that"), "*")
    topic = topic_name.strip() or "*"
    aiml_path = build_aiml_path(pattern, that, topic)
    if not aiml_path:
        return None

    template_xml = ET.tostring(template_node, encoding="unicode", method="xml").strip()
    relative_source = source_file.relative_to(aiml_dir).as_posix()

    return {
        "pattern": pattern.strip(),
        "that": that,
        "topic": topic,
        "path": aiml_path,
        "template_xml": template_xml,
        "source": relative_source,
    }


def parse_aiml_file(file_path: Path, aiml_dir: Path) -> List[Dict[str, str]]:
    categories: List[Dict[str, str]] = []
    xml_tree = ET.parse(file_path)
    root = xml_tree.getroot()

    for child in list(root):
        child_name = local_tag_name(child.tag).lower()
        if child_name == "category":
            category = parse_category(child, "*", file_path, aiml_dir)
            if category is not None:
                categories.append(category)
        elif child_name == "topic":
            topic_name = child.attrib.get("name", "*").strip() or "*"
            for topic_child in list(child):
                if local_tag_name(topic_child.tag).lower() != "category":
                    continue
                category = parse_category(topic_child, topic_name, file_path, aiml_dir)
                if category is not None:
                    categories.append(category)

    return categories


def collect_aiml_categories(aiml_files: List[Path], aiml_dir: Path) -> Tuple[List[Dict[str, str]], List[str]]:
    categories: List[Dict[str, str]] = []
    parse_warnings: List[str] = []
    for aiml_file in aiml_files:
        try:
            categories.extend(parse_aiml_file(aiml_file, aiml_dir))
        except Exception as exc:
            parse_warnings.append(aiml_file.name + ": " + str(exc))

    categories.sort(key=lambda item: (item["source"], item["path"], item["template_xml"]))
    return categories, parse_warnings


def chunk_text(text: str, chunk_size: int) -> List[str]:
    return [text[index : index + chunk_size] for index in range(0, len(text), chunk_size)]


def build_payload(
    aiml_dir: Path,
    aiml_files: List[Path],
    categories: List[Dict[str, str]],
    parse_warnings: List[str],
) -> bytes:
    payload = {
        "format_version": 1,
        "created_utc": datetime.now(timezone.utc).isoformat().replace("+00:00", "Z"),
        "source_aiml_dir": str(aiml_dir),
        "source_file_count": len(aiml_files),
        "category_count": len(categories),
        "parse_warning_count": len(parse_warnings),
        "parse_warnings": parse_warnings[:100],
        "categories": categories,
    }
    payload_json = json.dumps(payload, ensure_ascii=False, separators=(",", ":"))
    return payload_json.encode("utf-8")


def write_gguf(
    output_path: Path,
    model_name: str,
    aiml_dir: Path,
    file_count: int,
    categories: List[Dict[str, str]],
    parse_warnings: List[str],
    payload_raw: bytes,
) -> str:
    import gguf  # type: ignore

    compressed_payload = gzip.compress(payload_raw, compresslevel=9)
    encoded_payload = base64.b64encode(compressed_payload).decode("ascii")
    payload_chunks = chunk_text(encoded_payload, 120000)
    payload_sha256 = hashlib.sha256(payload_raw).hexdigest()

    writer = gguf.GGUFWriter(str(output_path), "llama")
    writer.add_type("adapter")
    writer.add_name(model_name)
    writer.add_author("sambot")
    writer.add_description("AIML knowledge pack exported by sambot.")
    writer.add_string("sambot.aiml.source_dir", str(aiml_dir))
    writer.add_uint32("sambot.aiml.file_count", file_count)
    writer.add_uint32("sambot.aiml.category_count", len(categories))
    writer.add_uint32("sambot.aiml.parse_warning_count", len(parse_warnings))
    writer.add_string("sambot.aiml.payload.format", "json+gzip+base64")
    writer.add_uint64("sambot.aiml.payload.raw_bytes", len(payload_raw))
    writer.add_uint64("sambot.aiml.payload.compressed_bytes", len(compressed_payload))
    writer.add_uint32("sambot.aiml.payload.chunk_count", len(payload_chunks))
    writer.add_string("sambot.aiml.payload.sha256", payload_sha256)

    for index, chunk in enumerate(payload_chunks):
        writer.add_string("sambot.aiml.payload.chunk." + format(index, "04d"), chunk)

    writer.add_tokenizer_model("none")
    writer.add_tokenizer_pre("none")
    writer.add_token_list([b"<unk>"])
    writer.add_token_types([int(gguf.TokenType.UNKNOWN)])
    writer.add_token_scores([0.0])

    writer.add_tensor("sambot.aiml.stub", np.zeros((1,), dtype=np.float32))
    writer.write_header_to_file()
    writer.write_kv_data_to_file()
    writer.write_tensors_to_file()
    writer.close()

    return payload_sha256


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Create a GGUF knowledge pack from AIML files.")
    parser.add_argument("--aiml-dir", required=True, help="Directory containing .aiml files.")
    parser.add_argument("--output", required=True, help="Output .gguf file path.")
    parser.add_argument("--bitnet-root", default="", help="BitNet root directory.")
    parser.add_argument(
        "--model-name",
        default="sambot-aiml-knowledgepack",
        help="Metadata model name for the generated GGUF file.",
    )
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    script_path = Path(__file__).resolve()
    bitnet_root = resolve_bitnet_root(script_path, args.bitnet_root)
    add_gguf_python_path(bitnet_root)

    aiml_dir = Path(args.aiml_dir).expanduser().resolve()
    if not aiml_dir.exists():
        raise FileNotFoundError("AIML directory does not exist: " + str(aiml_dir))

    aiml_files = discover_aiml_files(aiml_dir)
    if not aiml_files:
        raise FileNotFoundError("No .aiml files were found under: " + str(aiml_dir))

    categories, parse_warnings = collect_aiml_categories(aiml_files, aiml_dir)
    if not categories:
        raise RuntimeError("No AIML categories could be extracted from: " + str(aiml_dir))

    payload_raw = build_payload(aiml_dir, aiml_files, categories, parse_warnings)
    output_path = Path(args.output).expanduser().resolve()
    output_path.parent.mkdir(parents=True, exist_ok=True)

    payload_sha256 = write_gguf(
        output_path=output_path,
        model_name=args.model_name,
        aiml_dir=aiml_dir,
        file_count=len(aiml_files),
        categories=categories,
        parse_warnings=parse_warnings,
        payload_raw=payload_raw,
    )

    print("Created AIML GGUF: " + str(output_path))
    print("AIML source directory: " + str(aiml_dir))
    print("AIML files processed: " + str(len(aiml_files)))
    print("Categories exported: " + str(len(categories)))
    print("Payload SHA256: " + payload_sha256)
    if parse_warnings:
        print("Warnings: " + str(len(parse_warnings)) + " AIML files could not be parsed.")

    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as exc:
        print("ERROR: " + str(exc), file=sys.stderr)
        raise SystemExit(1)
