$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$modelDir = Join-Path $root 'models'
$modelFile = Join-Path $modelDir 'qwen2.5-0.5b-instruct-q4_k_m.gguf'
$modelUrl = 'https://huggingface.co/Qwen/Qwen2.5-0.5B-Instruct-GGUF/resolve/main/qwen2.5-0.5b-instruct-q4_k_m.gguf?download=true'

New-Item -ItemType Directory -Force -Path $modelDir | Out-Null

if ((Test-Path $modelFile) -and (Get-Item $modelFile).Length -gt 400MB) {
    Write-Host "Model already exists: $modelFile"
    exit 0
}

Write-Host "Downloading Qwen2.5 0.5B Instruct GGUF Q4_K_M..."
curl.exe -L --fail --retry 3 --retry-delay 5 -o $modelFile $modelUrl

if (!(Test-Path $modelFile) -or (Get-Item $modelFile).Length -lt 400MB) {
    throw "Model download did not produce the expected GGUF file: $modelFile"
}

Write-Host "Model ready: $modelFile"
