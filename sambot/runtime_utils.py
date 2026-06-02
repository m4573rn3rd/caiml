from __future__ import annotations

import os
import platform
from pathlib import Path


BITNET_DIR = Path(__file__).resolve().parent
BUILD_BIN_DIR = BITNET_DIR / "build" / "bin"
MODELS_DIR = BITNET_DIR / "models"
GGUF_EXTENSIONS = {".gguf", ".ggml"}
LEGACY_BITNET_MODEL_MARKERS = (
    "bitnet-b1.58",
    "bitnet_b1_58",
    "ggml-model-i2_s",
    "ggml-model-tl1",
    "ggml-model-tl2",
    "-i2_s",
    "-tl1",
    "-tl2",
)


def resolve_bitnet_path(path_value: str) -> str:
    candidate = Path(path_value or "")
    if candidate.is_absolute():
        return str(candidate)
    return str((BITNET_DIR / candidate).resolve())


def is_legacy_bitnet_model(path_value: str | Path) -> bool:
    candidate = Path(path_value)
    normalized = " ".join(part.lower() for part in candidate.parts)
    return any(marker in normalized for marker in LEGACY_BITNET_MODEL_MARKERS)


def iter_model_candidates() -> list[Path]:
    if not MODELS_DIR.exists():
        return []
    return sorted(
        candidate
        for candidate in MODELS_DIR.rglob("*")
        if candidate.is_file() and candidate.suffix.lower() in GGUF_EXTENSIONS
    )


def resolve_model_path(model_value: str = "") -> str | None:
    requested_value = str(model_value or "").strip()
    candidates = iter_model_candidates()

    if requested_value:
        requested_path = Path(resolve_bitnet_path(requested_value))
        if requested_path.is_file():
            return str(requested_path.resolve())

        requested_name = Path(requested_value).name.lower()
        requested_relative = requested_value.replace("\\", "/").strip("/").lower()
        for candidate in candidates:
            candidate_name = candidate.name.lower()
            candidate_relative = (
                candidate.relative_to(MODELS_DIR).as_posix().lower()
                if MODELS_DIR in candidate.parents
                else candidate.as_posix().lower()
            )
            if candidate_name == requested_name or candidate_relative == requested_relative:
                return str(candidate.resolve())

    for candidate in candidates:
        if not is_legacy_bitnet_model(candidate):
            return str(candidate.resolve())
    return None


def resolve_llama_binary(binary_name: str, env_var_name: str) -> str | None:
    env_override = os.environ.get(env_var_name, "").strip()
    if env_override:
        return resolve_bitnet_path(env_override)

    if platform.system() == "Windows":
        windows_candidates = [
            BUILD_BIN_DIR / "Release" / f"{binary_name}.exe",
            BUILD_BIN_DIR / f"{binary_name}.exe",
        ]
        for candidate in windows_candidates:
            if candidate.is_file():
                return str(candidate.resolve())
        return None

    linux_candidates = [
        BUILD_BIN_DIR / binary_name,
        BUILD_BIN_DIR / "Release" / binary_name,
    ]
    for candidate in linux_candidates:
        if candidate.is_file():
            return str(candidate.resolve())

    return None
