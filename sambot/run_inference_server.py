import os
import sys
import signal
import argparse
import subprocess

from runtime_utils import (
    is_legacy_bitnet_model,
    resolve_llama_binary,
    resolve_model_path,
)

def run_command(command, shell=False):
    """Run a system command and ensure it succeeds."""
    try:
        subprocess.run(command, shell=shell, check=True)
    except subprocess.CalledProcessError as e:
        print(f"Error occurred while running command: {e}")
        sys.exit(1)

def run_server():
    server_path = resolve_llama_binary("llama-server", "LLAMA_SERVER_PATH")
    if not server_path or not os.path.exists(server_path):
        print(
            "Could not find llama-server. "
            "Provide LLAMA_SERVER_PATH or build the local llama.cpp runtime first."
        )
        sys.exit(1)

    model_path = resolve_model_path(args.model)
    if not model_path:
        print(
            "Could not find a GGUF model. Add a standard .gguf or .ggml model under "
            "BitNet/models or pass one with --model."
        )
        sys.exit(1)
    if is_legacy_bitnet_model(model_path):
        print(
            "The selected model looks like a legacy BitNet i2_s/tl* model. "
            "This local llama.cpp backend needs a standard GGUF model instead."
        )
        sys.exit(1)

    command = [
        f'{server_path}',
        '-m', model_path,
        '-c', str(args.ctx_size),
        '-t', str(args.threads),
        '-n', str(args.n_predict),
        '-ngl', '0',
        '--temp', str(args.temperature),
        '--host', args.host,
        '--port', str(args.port),
        '-cb'  # Enable continuous batching
    ]
    
    if args.prompt:
        command.extend(['-p', args.prompt])
    
    # Note: -cnv flag is removed as it's not supported by the server
    
    print(f"Starting server on {args.host}:{args.port}")
    run_command(command)

def signal_handler(sig, frame):
    print("Ctrl+C pressed, shutting down server...")
    sys.exit(0)

if __name__ == "__main__":
    signal.signal(signal.SIGINT, signal_handler)
    
    parser = argparse.ArgumentParser(description='Run a local llama.cpp server')
    parser.add_argument("-m", "--model", type=str, help="Path to a GGUF model file", required=False, default="")
    parser.add_argument("-p", "--prompt", type=str, help="System prompt for the model", required=False)
    parser.add_argument("-n", "--n-predict", type=int, help="Number of tokens to predict", required=False, default=256)
    parser.add_argument("-t", "--threads", type=int, help="Number of threads to use", required=False, default=2)
    parser.add_argument("-c", "--ctx-size", type=int, help="Size of the context window", required=False, default=2048)
    parser.add_argument("--temperature", type=float, help="Temperature for sampling", required=False, default=0.8)
    parser.add_argument("--host", type=str, help="IP address to listen on", required=False, default="127.0.0.1")
    parser.add_argument("--port", type=int, help="Port to listen on", required=False, default=5052)
    
    args = parser.parse_args()
    run_server()
