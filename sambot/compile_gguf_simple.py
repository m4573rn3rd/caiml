#!/usr/bin/env python3
"""
Simple AIML to GGUF converter wrapper
Bypasses BitNet dependency by using gguf-py directly
"""

import sys
from pathlib import Path

# Add gguf-py to path
gguf_py_path = r"C:\Users\brand\Downloads\llama.cpp-master\llama.cpp-master\gguf-py"
sys.path.insert(0, gguf_py_path)

# Now import and run the main script
import subprocess
import os

def main():
    aiml_dir = r"x:\GitHub\caiml\sambot\bin\debug\aiml"
    output_file = r"x:\GitHub\caiml\sambot\models\sambot-aiml-knowledge-pack.gguf"
    model_name = "sambot-aiml-knowledge-pack"
    
    # Ensure output directory exists
    output_dir = Path(output_file).parent
    output_dir.mkdir(parents=True, exist_ok=True)
    
    # Set PYTHONPATH to include gguf-py
    env = os.environ.copy()
    env['PYTHONPATH'] = f"{gguf_py_path};{env.get('PYTHONPATH', '')}"
    
    # Run the original script with environment
    script_path = r"x:\GitHub\caiml\sambot\create_aiml_gguf.py"
    
    cmd = [
        sys.executable,
        script_path,
        "--aiml-dir", aiml_dir,
        "--output", output_file,
        "--model-name", model_name
    ]
    
    print("Running AIML to GGUF conversion...")
    print(f"Command: {' '.join(cmd)}")
    print()
    
    result = subprocess.run(cmd, env=env)
    return result.returncode

if __name__ == "__main__":
    sys.exit(main())
