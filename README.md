# SamBot - Multi-Backend AI Chatbot

A C# chatbot application for Windows that supports multiple AI backends including AIML, BitNet, and GGUF-based models. Features interactive console chat, speech synthesis, and knowledge pack generation from AIML files.

## Features

- **Multiple Chat Modes**
  - **AIM Mode**: AIML-based responses using pattern matching and templates
  - **BitNet Mode**: Integration with BitNet quantized models running locally
  - **AIML Model Mode**: Knowledge packs exported as GGUF format for efficient inference
  - **News Model Mode**: Specialized model for news-related queries

- **Speech Synthesis**: Built-in audio responses using Windows Speech API
- **GGUF Knowledge Packs**: Convert AIML knowledge bases to GGUF format for compact storage and fast inference
- **Flexible Configuration**: Command-line arguments or environment variables to control bot behavior
- **Console Interface**: Interactive red/blue color-coded input/output with mode-specific styling

## Project Structure

```
sambot/
├── Program.cs                    # Main chatbot application
├── AIMLbot_source/               # AIML engine source implementation
├── AIMLbot/                      # Compiled AIML library
├── models/                       # Directory for model files (GGUF, BitNet, etc.)
├── Properties/                   # .NET project properties
├── app.config                    # .NET application configuration
├── sambot.csproj                 # Visual Studio project file
├── create_aiml_gguf.py          # Convert AIML files to GGUF knowledge pack
├── extract_aiml_gguf.py         # Extract AIML data from GGUF files
├── create_news_gguf.py          # Create news model GGUF pack
├── extract_news_gguf.py         # Extract news model data from GGUF
├── build_aimlbot.ps1            # PowerShell build script for AIMLbot
├── download_bitnet_model.ps1    # Script to download BitNet models
└── icon.ico                      # Application icon
```

## Requirements

- **.NET Framework 4.8** or later
- **Windows OS** (uses Windows Speech API)
- **Python 3.7+** (for GGUF generation scripts)
- **NumPy** (for Python GGUF scripts)
- **BitNet** (optional, for BitNet mode) - local inference server running on `http://127.0.0.1:5052`

## Building

### Using Visual Studio
1. Open `sambot.sln` in Visual Studio 2015 or later
2. Build the solution (Ctrl+Shift+B)
3. Output goes to `bin\Debug\` or `bin\Release\`

### Using PowerShell
Run the build script:
```powershell
.\build_aimlbot.ps1
```

## Usage

### Running the Chatbot

**Interactive Mode** (select chat mode on startup):
```cmd
sambot.exe
```
You'll be prompted to choose a chat mode (1=AIM, 2=BitNet, 3=AIML Model, 4=News Model)

**Command-Line Mode** (specify mode directly):
```cmd
sambot.exe --mode aim
sambot.exe --mode bitnet
sambot.exe --mode aiml-model --aiml-model-path C:\path\to\model.gguf
sambot.exe --mode news-model --news-model-path C:\path\to\news.gguf
```

**Environment Variable**:
```cmd
set SAMBOT_MODE=aim
sambot.exe
```

### Creating GGUF Knowledge Packs

Convert AIML files to a compressed GGUF knowledge pack:
```bash
python create_aiml_gguf.py \
  --aiml-dir C:\path\to\aiml\files \
  --output C:\path\to\output.gguf \
  --bitnet-root C:\path\to\bitnet \
  --model-name my-aiml-pack
```

Extract AIML categories from a GGUF pack:
```bash
python extract_aiml_gguf.py \
  --gguf C:\path\to\model.gguf \
  --output extracted_data.json \
  --bitnet-root C:\path\to\bitnet
```

### Downloading BitNet Models

Download and set up BitNet models using the provided script:
```powershell
.\download_bitnet_model.ps1 -ModelName "model-name"
```

## Commands

While chatting, you can use special commands:

| Command | Action |
|---------|--------|
| `/exit` or `/quit` | Exit the application |
| `/mode <mode>` | Switch chat mode (aim, bitnet, aiml-model, news-model) |
| `/aiml-model <path>` | Load AIML GGUF model from path |
| `/news-model <path>` | Load News GGUF model from path |
| `/clear` | Clear conversation history (mode-dependent) |

## Chat Modes Explained

### AIM Mode (AIML)
Uses pattern-matching AIML rules to generate responses. Fast but requires hand-crafted knowledge base.
- Color: Blue
- Best for: Interactive learning, structured responses

### BitNet Mode
Connects to a local BitNet inference server for AI-powered responses.
- Color: Dark Cyan
- Requirements: BitNet server running on `http://127.0.0.1:5052`
- Best for: Natural language understanding, open-ended conversations

### AIML Model Mode (GGUF)
Exports AIML knowledge as a compact GGUF file for efficient inference.
- Color: Dark Green
- Best for: Embedding AIML knowledge in quantized models

### News Model Mode (GGUF)
Specialized model for news article retrieval and analysis.
- Color: Dark Magenta
- API: Queries `https://unitedwild.com/api/news/articles`
- Best for: News-related queries and article summaries

## Configuration

Edit `app.config` to configure:
```xml
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1"/>
  </startup>
</configuration>
```

## Architecture

The application uses a modular design with:
- **AIMLbot Library**: Core AIML parser and inference engine
- **Speech Synthesizer**: Windows Speech API for audio output
- **HTTP Client**: For BitNet server communication
- **GGUF Python Tools**: For knowledge pack serialization

## License

MIT License - See [LICENSE](LICENSE) file for details

## Author

Brandon McClain

---

For questions or issues, please refer to the codebase or submit an issue on GitHub.
