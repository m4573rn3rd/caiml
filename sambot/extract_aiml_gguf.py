#!/usr/bin/env python3
from __future__ import annotations

import argparse
import base64
import gzip
import json
import shutil
import sys
from pathlib import Path
from typing import Any, Dict, List
import xml.etree.ElementTree as ET
from xml.sax.saxutils import escape as xml_escape


def resolve_bitnet_root(script_path: Path, provided_root: str) -> Path:
    if provided_root:
        candidate = Path(provided_root).expanduser().resolve()
        if not candidate.exists():
            raise FileNotFoundError("Provided --bitnet-root does not exist: " + str(candidate))
        return candidate

    for candidate in [script_path.parent] + list(script_path.parents):
        if (candidate / "run_inference_server.py").exists():
            return candidate

    raise FileNotFoundError("Could not locate BitNet root. Pass --bitnet-root explicitly.")


def add_gguf_python_path(bitnet_root: Path) -> None:
    gguf_path = bitnet_root / "3rdparty" / "llama.cpp" / "gguf-py"
    if not gguf_path.exists():
        raise FileNotFoundError("Missing gguf-py package path: " + str(gguf_path))
    sys.path.insert(0, str(gguf_path))


def normalize_text(value: Any, fallback: str = "") -> str:
    if value is None:
        return fallback
    text = str(value).strip()
    return text if text else fallback


def local_tag_name(tag: str) -> str:
    if "}" in tag:
        return tag.split("}", 1)[1]
    return tag


def clone_without_namespaces(node: ET.Element) -> ET.Element:
    cleaned = ET.Element(local_tag_name(node.tag))
    for key, value in node.attrib.items():
        cleaned.set(local_tag_name(key), value)

    cleaned.text = node.text
    cleaned.tail = node.tail

    for child in list(node):
        cleaned.append(clone_without_namespaces(child))

    return cleaned


def normalize_template_xml(value: Any) -> str:
    template_xml = normalize_text(value, "<template></template>")
    try:
        node = ET.fromstring(template_xml)
        cleaned = clone_without_namespaces(node)
        return ET.tostring(cleaned, encoding="unicode", method="xml").strip()
    except Exception:
        return template_xml


def read_payload_from_gguf(model_path: Path) -> Dict[str, Any]:
    import gguf  # type: ignore

    reader = gguf.GGUFReader(model_path)

    def get_value(key: str) -> Any:
        field = reader.get_field(key)
        if field is None:
            return None
        return field.contents()

    payload_format = normalize_text(get_value("sambot.aiml.payload.format"))
    if payload_format != "json+gzip+base64":
        raise ValueError("Unsupported AIML payload format: " + payload_format)

    chunk_count = int(get_value("sambot.aiml.payload.chunk_count") or 0)
    if chunk_count <= 0:
        raise ValueError("Invalid payload chunk count in GGUF model.")

    chunks: List[str] = []
    for index in range(chunk_count):
        key = "sambot.aiml.payload.chunk." + format(index, "04d")
        chunk_value = get_value(key)
        if chunk_value is None:
            raise ValueError("Missing payload chunk: " + key)
        chunks.append(str(chunk_value))

    encoded_payload = "".join(chunks)
    compressed_payload = base64.b64decode(encoded_payload.encode("ascii"))
    payload_raw = gzip.decompress(compressed_payload)
    payload = json.loads(payload_raw.decode("utf-8"))
    if not isinstance(payload, dict):
        raise ValueError("Unexpected payload shape inside GGUF model.")

    return payload


def write_category(file_handle, category: Dict[str, Any], indent: str) -> None:
    pattern = normalize_text(category.get("pattern"), "*")
    that = normalize_text(category.get("that"), "*")
    template_xml = normalize_template_xml(category.get("template_xml"))

    file_handle.write(indent + "<category>\n")
    file_handle.write(indent + "  <pattern>" + xml_escape(pattern) + "</pattern>\n")
    if that != "*":
        file_handle.write(indent + "  <that>" + xml_escape(that) + "</that>\n")

    for line in template_xml.replace("\r\n", "\n").replace("\r", "\n").split("\n"):
        file_handle.write(indent + "  " + line + "\n")

    file_handle.write(indent + "</category>\n")


def write_aiml_files(output_dir: Path, categories: List[Dict[str, Any]]) -> int:
    grouped: Dict[str, Dict[str, List[Dict[str, Any]]]] = {}
    for category in categories:
        source = normalize_text(category.get("source"), "model.aiml").replace("\\", "/")
        topic = normalize_text(category.get("topic"), "*")
        if source not in grouped:
            grouped[source] = {}
        if topic not in grouped[source]:
            grouped[source][topic] = []
        grouped[source][topic].append(category)

    files_written = 0
    for relative_source in sorted(grouped.keys()):
        target_path = (output_dir / relative_source).resolve()
        target_path.parent.mkdir(parents=True, exist_ok=True)

        with target_path.open("w", encoding="utf-8", newline="\n") as file_handle:
            file_handle.write('<?xml version="1.0" encoding="utf-8"?>\n')
            file_handle.write("<aiml>\n")

            topic_map = grouped[relative_source]
            if "*" in topic_map:
                for category in topic_map["*"]:
                    write_category(file_handle, category, "  ")

            for topic_name in sorted(topic for topic in topic_map.keys() if topic != "*"):
                file_handle.write('  <topic name="' + xml_escape(topic_name, {"\"": "&quot;"}) + '">\n')
                for category in topic_map[topic_name]:
                    write_category(file_handle, category, "    ")
                file_handle.write("  </topic>\n")

            file_handle.write("</aiml>\n")

        files_written += 1

    return files_written


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Extract AIML files from a sambot AIML GGUF knowledge pack.")
    parser.add_argument("--model", required=True, help="Path to an AIML GGUF file created by sambot.")
    parser.add_argument("--output-dir", required=True, help="Directory to write extracted AIML files.")
    parser.add_argument("--bitnet-root", default="", help="BitNet root directory.")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    script_path = Path(__file__).resolve()
    bitnet_root = resolve_bitnet_root(script_path, args.bitnet_root)
    add_gguf_python_path(bitnet_root)

    model_path = Path(args.model).expanduser().resolve()
    if not model_path.exists():
        raise FileNotFoundError("Model path does not exist: " + str(model_path))

    output_dir = Path(args.output_dir).expanduser().resolve()
    if output_dir.exists():
        shutil.rmtree(output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)

    payload = read_payload_from_gguf(model_path)
    categories = payload.get("categories")
    if not isinstance(categories, list):
        raise ValueError("Payload does not contain a valid categories list.")

    normalized_categories: List[Dict[str, Any]] = []
    for entry in categories:
        if not isinstance(entry, dict):
            continue
        normalized_categories.append(entry)

    if not normalized_categories:
        raise ValueError("No categories were found in the AIML GGUF payload.")

    files_written = write_aiml_files(output_dir, normalized_categories)
    print("Extracted AIML GGUF model: " + str(model_path))
    print("AIML files written: " + str(files_written))
    print("Categories written: " + str(len(normalized_categories)))
    print("Output directory: " + str(output_dir))
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as exc:
        print("ERROR: " + str(exc), file=sys.stderr)
        raise SystemExit(1)
