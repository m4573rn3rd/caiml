#!/usr/bin/env python3
from __future__ import annotations

import argparse
import base64
import gzip
import json
import sys
from pathlib import Path
from typing import Any, Dict, List


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


def sanitize_field(value: Any) -> str:
    text = normalize_text(value)
    text = text.replace("\t", " ").replace("\r", " ").replace("\n", " ")
    return " ".join(text.split())


def read_payload_from_gguf(model_path: Path) -> Dict[str, Any]:
    import gguf  # type: ignore

    reader = gguf.GGUFReader(model_path)

    def get_value(key: str) -> Any:
        field = reader.get_field(key)
        if field is None:
            return None
        return field.contents()

    payload_format = normalize_text(get_value("sambot.news.payload.format"))
    if payload_format != "json+gzip+base64":
        raise ValueError("Unsupported News payload format: " + payload_format)

    chunk_count = int(get_value("sambot.news.payload.chunk_count") or 0)
    if chunk_count <= 0:
        raise ValueError("Invalid payload chunk count in GGUF model.")

    chunks: List[str] = []
    for index in range(chunk_count):
        key = "sambot.news.payload.chunk." + format(index, "04d")
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


def write_articles_tsv(output_path: Path, articles: List[Dict[str, Any]]) -> int:
    written = 0
    output_path.parent.mkdir(parents=True, exist_ok=True)
    with output_path.open("w", encoding="utf-8", newline="\n") as handle:
        for entry in articles:
            if not isinstance(entry, dict):
                continue

            title = sanitize_field(entry.get("title"))
            summary = sanitize_field(entry.get("summary"))
            article_text = sanitize_field(entry.get("article_text"))
            published_label = sanitize_field(entry.get("published_label"))
            category_slug = sanitize_field(entry.get("category_slug"))
            link = sanitize_field(entry.get("link"))

            if not title and not summary and not article_text:
                continue

            row = "\t".join(
                [
                    title,
                    summary,
                    article_text,
                    published_label,
                    category_slug,
                    link,
                ]
            )
            handle.write(row + "\n")
            written += 1

    return written


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Extract searchable news rows from a sambot News GGUF file.")
    parser.add_argument("--model", required=True, help="Path to a News GGUF file created by sambot.")
    parser.add_argument("--output", required=True, help="Path to output TSV file.")
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

    payload = read_payload_from_gguf(model_path)
    articles = payload.get("articles")
    if not isinstance(articles, list):
        raise ValueError("Payload does not contain a valid articles list.")

    output_path = Path(args.output).expanduser().resolve()
    rows_written = write_articles_tsv(output_path, articles)
    if rows_written <= 0:
        raise ValueError("No articles were extracted from the News GGUF payload.")

    print("Extracted News GGUF model: " + str(model_path))
    print("Articles written: " + str(rows_written))
    print("Output file: " + str(output_path))
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as exc:
        print("ERROR: " + str(exc), file=sys.stderr)
        raise SystemExit(1)
