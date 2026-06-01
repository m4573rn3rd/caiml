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
from typing import Dict, List, Tuple
from urllib.parse import parse_qsl, urlencode, urlparse, urlunparse
from urllib.request import Request, urlopen

import numpy as np


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


def normalize_text(value: object) -> str:
    if value is None:
        return ""
    return str(value).strip()


def build_news_api_url(base_url: str, limit: int, offset: int, category: str) -> str:
    parsed = urlparse(base_url)
    existing_params = [
        (name, value)
        for name, value in parse_qsl(parsed.query, keep_blank_values=True)
        if name not in {"limit", "offset", "category", "include_text"}
    ]
    existing_params.append(("limit", str(limit)))
    existing_params.append(("offset", str(offset)))
    existing_params.append(("include_text", "1"))
    if category:
        existing_params.append(("category", category))

    return urlunparse(parsed._replace(query=urlencode(existing_params)))


def fetch_json(url: str, timeout_seconds: int) -> Dict:
    request = Request(
        url,
        headers={
            "Accept": "application/json",
            "User-Agent": "sambot-news-gguf/1.0",
        },
    )

    with urlopen(request, timeout=max(5, int(timeout_seconds))) as response:
        content = response.read().decode("utf-8", errors="replace")

    payload = json.loads(content)
    if not isinstance(payload, dict):
        raise ValueError("Expected JSON object from API.")
    return payload


def fetch_all_news_articles(
    api_url: str,
    page_size: int,
    timeout_seconds: int,
    category: str,
    max_pages: int,
) -> Tuple[List[Dict[str, str]], int]:
    articles: List[Dict[str, str]] = []
    total = 0
    seen_ids = set()
    offset = 0

    for _ in range(max_pages):
        page_url = build_news_api_url(api_url, page_size, offset, category)
        payload = fetch_json(page_url, timeout_seconds)
        if not bool(payload.get("ok", False)):
            raise RuntimeError("News API returned ok=false.")

        items = payload.get("items", [])
        if not isinstance(items, list):
            raise ValueError("News API returned an invalid items payload.")

        payload_total = payload.get("total", 0)
        try:
            total = max(total, int(payload_total))
        except (TypeError, ValueError):
            pass

        if not items:
            break

        for item in items:
            if not isinstance(item, dict):
                continue

            article_id = item.get("id")
            dedupe_key = str(article_id) if article_id is not None else (
                normalize_text(item.get("link")) + "|" + normalize_text(item.get("title"))
            )
            if dedupe_key in seen_ids:
                continue
            seen_ids.add(dedupe_key)

            article_text = normalize_text(item.get("article_text")) or normalize_text(item.get("summary"))
            articles.append(
                {
                    "id": str(article_id or ""),
                    "category_slug": normalize_text(item.get("category_slug")),
                    "source_name": normalize_text(item.get("source_name")),
                    "source_url": normalize_text(item.get("source_url")),
                    "external_id": normalize_text(item.get("external_id")),
                    "title": normalize_text(item.get("title")),
                    "summary": normalize_text(item.get("summary")),
                    "article_text": article_text,
                    "link": normalize_text(item.get("link")),
                    "published_at": normalize_text(item.get("published_at")),
                    "published_label": normalize_text(item.get("published_label")),
                    "created_at": normalize_text(item.get("created_at")),
                    "updated_at": normalize_text(item.get("updated_at")),
                }
            )

        offset += page_size
        if len(items) < page_size:
            break
        if total > 0 and offset >= total:
            break

    return articles, total


def chunk_text(text: str, chunk_size: int) -> List[str]:
    return [text[index : index + chunk_size] for index in range(0, len(text), chunk_size)]


def build_payload(
    api_url: str,
    category: str,
    articles: List[Dict[str, str]],
    reported_total: int,
) -> bytes:
    payload = {
        "format_version": 1,
        "created_utc": datetime.now(timezone.utc).isoformat().replace("+00:00", "Z"),
        "source_api_url": api_url,
        "category_filter": category,
        "reported_total": reported_total,
        "article_count": len(articles),
        "articles": articles,
    }
    payload_json = json.dumps(payload, ensure_ascii=False, separators=(",", ":"))
    return payload_json.encode("utf-8")


def write_gguf(
    output_path: Path,
    model_name: str,
    api_url: str,
    category: str,
    articles: List[Dict[str, str]],
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
    writer.add_description("News knowledge pack exported from United Wild news API.")
    writer.add_string("sambot.news.source_api_url", api_url)
    writer.add_string("sambot.news.category_filter", category)
    writer.add_uint32("sambot.news.article_count", len(articles))
    writer.add_string("sambot.news.payload.format", "json+gzip+base64")
    writer.add_uint64("sambot.news.payload.raw_bytes", len(payload_raw))
    writer.add_uint64("sambot.news.payload.compressed_bytes", len(compressed_payload))
    writer.add_uint32("sambot.news.payload.chunk_count", len(payload_chunks))
    writer.add_string("sambot.news.payload.sha256", payload_sha256)

    for index, chunk in enumerate(payload_chunks):
        writer.add_string("sambot.news.payload.chunk." + format(index, "04d"), chunk)

    writer.add_tokenizer_model("none")
    writer.add_tokenizer_pre("none")
    writer.add_token_list([b"<unk>"])
    writer.add_token_types([int(gguf.TokenType.UNKNOWN)])
    writer.add_token_scores([0.0])

    writer.add_tensor("sambot.news.stub", np.zeros((1,), dtype=np.float32))
    writer.write_header_to_file()
    writer.write_kv_data_to_file()
    writer.write_tensors_to_file()
    writer.close()

    return payload_sha256


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Create sambot_news.gguf from a United Wild news API endpoint.")
    parser.add_argument(
        "--news-api-url",
        default="https://unitedwild.com/api/news/articles",
        help="News API URL that returns paginated JSON article data.",
    )
    parser.add_argument(
        "--category",
        default="",
        help="Optional category filter passed to the API route.",
    )
    parser.add_argument("--output", required=True, help="Output .gguf file path.")
    parser.add_argument("--bitnet-root", default="", help="BitNet root directory.")
    parser.add_argument("--page-size", type=int, default=500, help="Articles per API page request.")
    parser.add_argument("--max-pages", type=int, default=500, help="Safety cap for pagination.")
    parser.add_argument("--timeout-seconds", type=int, default=30, help="Per-request timeout in seconds.")
    parser.add_argument("--model-name", default="sambot-news", help="Model name stored in GGUF metadata.")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    script_path = Path(__file__).resolve()
    bitnet_root = resolve_bitnet_root(script_path, args.bitnet_root)
    add_gguf_python_path(bitnet_root)

    page_size = max(1, min(int(args.page_size), 1000))
    max_pages = max(1, min(int(args.max_pages), 5000))
    timeout_seconds = max(5, min(int(args.timeout_seconds), 300))

    articles, reported_total = fetch_all_news_articles(
        api_url=str(args.news_api_url).strip(),
        page_size=page_size,
        timeout_seconds=timeout_seconds,
        category=str(args.category or "").strip().lower(),
        max_pages=max_pages,
    )

    if not articles:
        raise RuntimeError("No news articles were returned by the API.")

    output_path = Path(args.output).expanduser().resolve()
    output_path.parent.mkdir(parents=True, exist_ok=True)

    payload_raw = build_payload(
        api_url=str(args.news_api_url).strip(),
        category=str(args.category or "").strip().lower(),
        articles=articles,
        reported_total=reported_total,
    )
    payload_sha256 = write_gguf(
        output_path=output_path,
        model_name=str(args.model_name).strip() or "sambot-news",
        api_url=str(args.news_api_url).strip(),
        category=str(args.category or "").strip().lower(),
        articles=articles,
        payload_raw=payload_raw,
    )

    print("Created news GGUF: " + str(output_path))
    print("News API URL: " + str(args.news_api_url).strip())
    print("Reported total articles: " + str(reported_total))
    print("Exported articles: " + str(len(articles)))
    print("Payload SHA256: " + payload_sha256)
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as exc:
        print("ERROR: " + str(exc), file=sys.stderr)
        raise SystemExit(1)
