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

def run_inference():
    main_path = resolve_llama_binary("llama-cli", "LLAMA_CLI_PATH")
    if not main_path or not os.path.exists(main_path):
        print(
            "Could not find llama-cli. "
            "Provide LLAMA_CLI_PATH or build the local llama.cpp runtime first."
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
        f'{main_path}',
        '-m', model_path,
        '-n', str(args.n_predict),
        '-t', str(args.threads),
        '-p', args.prompt,
        '-ngl', '0',
        '-c', str(args.ctx_size),
        '--temp', str(args.temperature),
        "-b", "1",
    ]
    if args.conversation:
        command.append("-cnv")
    run_command(command)

def signal_handler(sig, frame):
    print("Ctrl+C pressed, exiting...")
    sys.exit(0)

if __name__ == "__main__":
    signal.signal(signal.SIGINT, signal_handler)
    parser = argparse.ArgumentParser(description='Run local GGUF inference')
    parser.add_argument("-m", "--model", type=str, help="Path to a GGUF model file", required=False, default="")
    parser.add_argument("-n", "--n-predict", type=int, help="Number of tokens to predict when generating text", required=False, default=128)
    parser.add_argument("-p", "--prompt", type=str, help="Prompt to generate text from", required=True)
    parser.add_argument("-t", "--threads", type=int, help="Number of threads to use", required=False, default=2)
    parser.add_argument("-c", "--ctx-size", type=int, help="Size of the prompt context", required=False, default=2048)
    parser.add_argument("-temp", "--temperature", type=float, help="Temperature, a hyperparameter that controls the randomness of the generated text", required=False, default=0.8)
    parser.add_argument("-cnv", "--conversation", action='store_true', help="Whether to enable chat mode or not (for instruct models.)")

    args = parser.parse_args()
    run_inference()
