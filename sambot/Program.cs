using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Speech.Synthesis;
using System.Text;
using System.Web.Script.Serialization;
using AIMLbot;

namespace sambot
{
    internal enum ChatMode
    {
        Aim,
        BitNet,
        AimlModel,
        NewsModel
    }

    internal class Program
    {
        private const string DefaultBitNetUrl = "http://127.0.0.1:5052";
        private const string DefaultBitNetModel = "local-gguf";
        private const string DefaultNewsApiUrl = "https://unitedwild.com/api/news/articles";
        private static readonly JavaScriptSerializer JsonSerializer = new JavaScriptSerializer();
        private static Process bitNetServerProcess;
        private static string bitNetStartupError = "";
        private static string startupAimlModelPath = "";
        private static string startupNewsModelPath = "";
        private static string cachedAimlModelPath = "";
        private static DateTime cachedAimlModelTimestampUtc = DateTime.MinValue;
        private static string cachedAimlModelAimlDirectory = "";
        private static string cachedNewsModelPath = "";
        private static DateTime cachedNewsModelTimestampUtc = DateTime.MinValue;
        private static string cachedNewsModelArticlesPath = "";
        private static List<NewsArticleEntry> cachedNewsModelArticles = new List<NewsArticleEntry>();

        private static void Main(string[] args)
        {
            Console.Title = "Sam Bot";
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            if (TryHandleStartupCommand(args))
            {
                return;
            }

            startupAimlModelPath = ReadOptionValue(args, "--aimlmodel-path", "--aiml-model-path");
            startupNewsModelPath = ReadOptionValue(args, "--newsmodel-path", "--news-model-path");

            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();

            ChatMode currentMode = ResolveInitialMode(args);
            Bot aimBot = null;
            User aimUser = null;
            Bot aimlModelBot = null;
            User aimlModelUser = null;
            if (currentMode == ChatMode.Aim)
            {
                EnsureAimBot(ref aimBot, ref aimUser);
            }
            else if (currentMode == ChatMode.BitNet)
            {
                string startupMessage;
                EnsureBitNetServerStarted(true, out startupMessage);
            }
            else if (currentMode == ChatMode.AimlModel)
            {
                TryWarmAimlModelBot(ref aimlModelBot, ref aimlModelUser);
            }
            else
            {
                TryWarmNewsModel();
            }

            PrintStartupHelp(currentMode);

            while (true)
            {
                try
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("Human: ");
                    Console.ResetColor();

                    string input = Console.ReadLine();
                    if (input == null)
                    {
                        break;
                    }

                    input = input.Trim();
                    if (input.Length == 0)
                    {
                        continue;
                    }

                    bool shouldExit;
                    if (HandleCommand(input, synth, ref currentMode, out shouldExit))
                    {
                        if (shouldExit)
                        {
                            break;
                        }
                        continue;
                    }

                    PrintThinking(currentMode);

                    string output = currentMode == ChatMode.BitNet
                        ? SendBitNetRequest(input)
                        : (currentMode == ChatMode.Aim
                            ? SendAimRequest(input, ref aimBot, ref aimUser)
                            : (currentMode == ChatMode.AimlModel
                                ? SendAimlModelRequest(input, ref aimlModelBot, ref aimlModelUser)
                                : SendNewsModelRequest(input)));

                    Console.BackgroundColor = currentMode == ChatMode.BitNet
                        ? ConsoleColor.DarkCyan
                        : (currentMode == ChatMode.AimlModel
                            ? ConsoleColor.DarkGreen
                            : (currentMode == ChatMode.NewsModel ? ConsoleColor.DarkMagenta : ConsoleColor.Blue));
                    Console.WriteLine(GetModeLabel(currentMode) + ": " + output);
                    SpeakResponse(synth, currentMode, output);
                    Console.ResetColor();
                }
                finally
                {
                    Console.ResetColor();
                }
            }

            StopStartedBitNetServer();
        }

        private static Bot CreateAimBot()
        {
            return CreateAimBot("");
        }

        private static Bot CreateAimBot(string aimlDirectory)
        {
            Bot bot = new Bot();
            bot.loadSettings();
            if (!String.IsNullOrWhiteSpace(aimlDirectory))
            {
                bot.GlobalSettings.addSetting("aimldirectory", aimlDirectory);
            }
            bot.isAcceptingUserInput = false;
            bot.loadAIMLFromFiles();
            bot.isAcceptingUserInput = true;
            return bot;
        }

        private static void EnsureAimBot(ref Bot bot, ref User user)
        {
            if (bot != null && user != null)
            {
                return;
            }

            bot = CreateAimBot();
            user = new User("consoleUser", bot);
        }

        private static ChatMode ResolveInitialMode(string[] args)
        {
            ChatMode parsedMode;
            if (TryParseModeArguments(args, out parsedMode))
            {
                return parsedMode;
            }

            string envMode = Environment.GetEnvironmentVariable("SAMBOT_MODE");
            if (TryParseMode(envMode, out parsedMode))
            {
                return parsedMode;
            }

            Console.WriteLine("Choose chat mode:");
            Console.WriteLine("  1) AIM");
            Console.WriteLine("  2) BitNet");
            Console.WriteLine("  3) AIML GGUF Test");
            Console.WriteLine("  4) NEWS GGUF Test");
            Console.Write("Mode [AIM]: ");

            string selectedMode = Console.ReadLine();
            if (TryParseMode(selectedMode, out parsedMode))
            {
                return parsedMode;
            }

            return ChatMode.Aim;
        }

        private static bool TryParseModeArguments(string[] args, out ChatMode mode)
        {
            mode = ChatMode.Aim;
            if (args == null)
            {
                return false;
            }

            for (int index = 0; index < args.Length; index++)
            {
                string value = args[index] ?? "";
                if (value.StartsWith("--mode=", StringComparison.OrdinalIgnoreCase))
                {
                    if (TryParseMode(value.Substring("--mode=".Length), out mode))
                    {
                        return true;
                    }
                }
                else if (
                    value.Equals("--mode", StringComparison.OrdinalIgnoreCase)
                    || value.Equals("-m", StringComparison.OrdinalIgnoreCase)
                )
                {
                    if (index + 1 < args.Length && TryParseMode(args[index + 1], out mode))
                    {
                        return true;
                    }
                }
                else if (TryParseMode(value, out mode))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryParseMode(string value, out ChatMode mode)
        {
            mode = ChatMode.Aim;
            string normalized = (value ?? "").Trim().ToLowerInvariant();
            if (normalized.Length == 0)
            {
                return false;
            }

            if (normalized == "1" || normalized == "aim" || normalized == "aiml")
            {
                mode = ChatMode.Aim;
                return true;
            }

            if (normalized == "2" || normalized == "bitnet" || normalized == "bit")
            {
                mode = ChatMode.BitNet;
                return true;
            }

            if (
                normalized == "3"
                || normalized == "aimlmodel"
                || normalized == "aiml-gguf"
                || normalized == "aimlgguf"
                || normalized == "modeltest"
            )
            {
                mode = ChatMode.AimlModel;
                return true;
            }

            if (
                normalized == "4"
                || normalized == "newsmodel"
                || normalized == "news-gguf"
                || normalized == "newsgguf"
                || normalized == "newstest"
                || normalized == "news"
            )
            {
                mode = ChatMode.NewsModel;
                return true;
            }

            return false;
        }

        private static void PrintStartupHelp(ChatMode currentMode)
        {
            Console.WriteLine("Sam Bot is running in " + GetModeLabel(currentMode) + " mode.");
            Console.WriteLine("Commands: mode aim, mode bitnet, mode aimlmodel, mode newsmodel, mode, cls, clear, quit, exit, end.");
            Console.WriteLine("PowerShell: .\\sambot.exe aim  or  .\\sambot.exe bitnet  or  .\\sambot.exe aimlmodel  or  .\\sambot.exe newsmodel");
            Console.WriteLine("Build AIML GGUF: .\\sambot.exe -create -aimlmodel");
            Console.WriteLine("Build News GGUF: .\\sambot.exe -create -newsmodel");
            Console.WriteLine("Optional model path: --aimlmodel-path <path-to-aiml-gguf>");
            Console.WriteLine("Optional news model path: --newsmodel-path <path-to-news-gguf>");
            Console.WriteLine("BitNet URL: " + GetBitNetBaseUrl());
            Console.WriteLine();
        }

        private static bool TryHandleStartupCommand(string[] args)
        {
            bool hasCreate = HasArgument(args, "create");
            if (!hasCreate)
            {
                return false;
            }

            bool hasAimlModel = HasArgument(args, "aimlmodel");
            bool hasNewsModel = HasArgument(args, "newsmodel");

            if (hasAimlModel && hasNewsModel)
            {
                Console.WriteLine("Use one target: -aimlmodel or -newsmodel");
                return true;
            }

            if (!hasAimlModel && !hasNewsModel)
            {
                Console.WriteLine("Use: .\\sambot.exe -create -aimlmodel [--aiml-dir <path>] [--output <path>]");
                Console.WriteLine("  or: .\\sambot.exe -create -newsmodel [--news-api-url <url>] [--output <path>]");
                return true;
            }

            string statusMessage;
            bool succeeded = hasAimlModel
                ? TryCreateAimlGgufModel(args, out statusMessage)
                : TryCreateNewsGgufModel(args, out statusMessage);

            if (succeeded)
            {
                Console.WriteLine(statusMessage);
            }
            else
            {
                string modelType = hasNewsModel ? "News" : "AIML";
                Console.WriteLine(modelType + " GGUF creation failed: " + statusMessage);
            }

            return true;
        }

        private static bool TryCreateAimlGgufModel(string[] args, out string message)
        {
            string sambotRoot = ResolveSambotRoot();
            if (String.IsNullOrWhiteSpace(sambotRoot))
            {
                message = "Could not locate sambot root directory.";
                return false;
            }

            string converterScriptPath = Path.Combine(sambotRoot, "create_aiml_gguf.py");
            if (!File.Exists(converterScriptPath))
            {
                message = "Missing converter script: " + converterScriptPath;
                return false;
            }

            string aimlDirectory = ReadOptionValue(args, "--aiml-dir", "-d");
            if (String.IsNullOrWhiteSpace(aimlDirectory))
            {
                aimlDirectory = ResolveAimlDirectory();
            }

            if (String.IsNullOrWhiteSpace(aimlDirectory) || !Directory.Exists(aimlDirectory))
            {
                message = "Could not find an AIML directory. Set SAMBOT_AIML_DIR or use --aiml-dir.";
                return false;
            }

            if (!ContainsAimlFiles(aimlDirectory))
            {
                message = "No .aiml files were found under: " + aimlDirectory;
                return false;
            }

            string outputPath = ReadOptionValue(args, "--output", "-o");
            if (String.IsNullOrWhiteSpace(outputPath))
            {
                string modelDirectory = Path.Combine(sambotRoot, "models");
                Directory.CreateDirectory(modelDirectory);
                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                outputPath = Path.Combine(modelDirectory, "aiml-" + timestamp + ".gguf");
            }
            else
            {
                if (!outputPath.EndsWith(".gguf", StringComparison.OrdinalIgnoreCase))
                {
                    outputPath += ".gguf";
                }

                outputPath = Path.GetFullPath(outputPath);
                string outputDirectory = Path.GetDirectoryName(outputPath);
                if (!String.IsNullOrWhiteSpace(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }
            }

            string pythonExe = GetSetting("SAMBOT_PYTHON_EXE", "PYTHON_EXE", "python");
            string bitNetRoot = ResolveBitNetRoot();
            string pythonArguments = BuildCreateAimlModelArguments(
                converterScriptPath,
                aimlDirectory,
                outputPath,
                bitNetRoot
            );

            ProcessStartInfo startInfo = new ProcessStartInfo(pythonExe, pythonArguments)
            {
                WorkingDirectory = sambotRoot,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {
                Process process = Process.Start(startInfo);
                if (process == null)
                {
                    message = "Python process could not be started.";
                    return false;
                }

                using (process)
                {
                    string stdout = process.StandardOutput.ReadToEnd().Trim();
                    string stderr = process.StandardError.ReadToEnd().Trim();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        message = stderr.Length > 0
                            ? stderr
                            : (stdout.Length > 0 ? stdout : "Converter exited with code " + process.ExitCode + ".");
                        return false;
                    }

                    message = stdout.Length > 0
                        ? stdout
                        : "AIML GGUF model created at: " + outputPath;
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "Could not launch Python converter. " + ex.Message;
                return false;
            }
        }

        private static string BuildCreateAimlModelArguments(
            string converterScriptPath,
            string aimlDirectory,
            string outputPath,
            string bitNetRoot
        )
        {
            StringBuilder arguments = new StringBuilder();
            arguments.Append(QuoteCommandArgument(converterScriptPath));
            arguments.Append(" --aiml-dir ");
            arguments.Append(QuoteCommandArgument(aimlDirectory));
            arguments.Append(" --output ");
            arguments.Append(QuoteCommandArgument(outputPath));

            if (!String.IsNullOrWhiteSpace(bitNetRoot))
            {
                arguments.Append(" --bitnet-root ");
                arguments.Append(QuoteCommandArgument(bitNetRoot));
            }

            return arguments.ToString();
        }

        private static bool TryCreateNewsGgufModel(string[] args, out string message)
        {
            string sambotRoot = ResolveSambotRoot();
            if (String.IsNullOrWhiteSpace(sambotRoot))
            {
                message = "Could not locate sambot root directory.";
                return false;
            }

            string converterScriptPath = Path.Combine(sambotRoot, "create_news_gguf.py");
            if (!File.Exists(converterScriptPath))
            {
                message = "Missing converter script: " + converterScriptPath;
                return false;
            }

            string newsApiUrl = ReadOptionValue(args, "--news-api-url", "--news-url");
            if (String.IsNullOrWhiteSpace(newsApiUrl))
            {
                newsApiUrl = GetSetting("SAMBOT_NEWS_API_URL", "NEWS_API_URL", DefaultNewsApiUrl);
            }

            string outputPath = ReadOptionValue(args, "--output", "-o");
            if (String.IsNullOrWhiteSpace(outputPath))
            {
                string modelDirectory = Path.Combine(sambotRoot, "models");
                Directory.CreateDirectory(modelDirectory);
                outputPath = Path.Combine(modelDirectory, "sambot_news.gguf");
            }
            else
            {
                if (!outputPath.EndsWith(".gguf", StringComparison.OrdinalIgnoreCase))
                {
                    outputPath += ".gguf";
                }

                outputPath = Path.GetFullPath(outputPath);
                string outputDirectory = Path.GetDirectoryName(outputPath);
                if (!String.IsNullOrWhiteSpace(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }
            }

            string pythonExe = GetSetting("SAMBOT_PYTHON_EXE", "PYTHON_EXE", "python");
            string bitNetRoot = ResolveBitNetRoot();
            string pythonArguments = BuildCreateNewsModelArguments(
                converterScriptPath,
                newsApiUrl,
                outputPath,
                bitNetRoot
            );

            ProcessStartInfo startInfo = new ProcessStartInfo(pythonExe, pythonArguments)
            {
                WorkingDirectory = sambotRoot,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {
                Process process = Process.Start(startInfo);
                if (process == null)
                {
                    message = "Python process could not be started.";
                    return false;
                }

                using (process)
                {
                    string stdout = process.StandardOutput.ReadToEnd().Trim();
                    string stderr = process.StandardError.ReadToEnd().Trim();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        message = stderr.Length > 0
                            ? stderr
                            : (stdout.Length > 0 ? stdout : "Converter exited with code " + process.ExitCode + ".");
                        return false;
                    }

                    message = stdout.Length > 0
                        ? stdout
                        : "News GGUF model created at: " + outputPath;
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "Could not launch Python converter. " + ex.Message;
                return false;
            }
        }

        private static string BuildCreateNewsModelArguments(
            string converterScriptPath,
            string newsApiUrl,
            string outputPath,
            string bitNetRoot
        )
        {
            StringBuilder arguments = new StringBuilder();
            arguments.Append(QuoteCommandArgument(converterScriptPath));
            arguments.Append(" --news-api-url ");
            arguments.Append(QuoteCommandArgument(newsApiUrl));
            arguments.Append(" --output ");
            arguments.Append(QuoteCommandArgument(outputPath));

            if (!String.IsNullOrWhiteSpace(bitNetRoot))
            {
                arguments.Append(" --bitnet-root ");
                arguments.Append(QuoteCommandArgument(bitNetRoot));
            }

            return arguments.ToString();
        }

        private static string QuoteCommandArgument(string value)
        {
            return "\"" + (value ?? "").Replace("\"", "\\\"") + "\"";
        }

        private static bool HasArgument(string[] args, string name)
        {
            if (args == null || String.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            string expectedName = NormalizeArgumentName(name);
            for (int index = 0; index < args.Length; index++)
            {
                if (NormalizeArgumentName(args[index]) == expectedName)
                {
                    return true;
                }
            }

            return false;
        }

        private static string ReadOptionValue(string[] args, string longOption, string shortOption)
        {
            if (args == null)
            {
                return "";
            }

            string normalizedLong = NormalizeArgumentName(longOption);
            string normalizedShort = NormalizeArgumentName(shortOption);

            for (int index = 0; index < args.Length; index++)
            {
                string value = args[index] ?? "";
                int equalsIndex = value.IndexOf('=');
                if (equalsIndex > 0)
                {
                    string optionName = NormalizeArgumentName(value.Substring(0, equalsIndex));
                    if (optionName == normalizedLong || optionName == normalizedShort)
                    {
                        return value.Substring(equalsIndex + 1).Trim().Trim('"');
                    }
                }

                string normalizedValue = NormalizeArgumentName(value);
                if ((normalizedValue == normalizedLong || normalizedValue == normalizedShort) && index + 1 < args.Length)
                {
                    return (args[index + 1] ?? "").Trim().Trim('"');
                }
            }

            return "";
        }

        private static string NormalizeArgumentName(string value)
        {
            string normalized = (value ?? "").Trim().ToLowerInvariant();
            while (normalized.StartsWith("-", StringComparison.OrdinalIgnoreCase) || normalized.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(1);
            }

            int equalsIndex = normalized.IndexOf('=');
            if (equalsIndex >= 0)
            {
                normalized = normalized.Substring(0, equalsIndex);
            }

            return normalized;
        }

        private static string ResolveAimlDirectory()
        {
            string configuredPath = Environment.GetEnvironmentVariable("SAMBOT_AIML_DIR");
            if (!String.IsNullOrWhiteSpace(configuredPath) && Directory.Exists(configuredPath))
            {
                return configuredPath.Trim();
            }

            List<string> candidates = new List<string>();
            string repositoryRoot = ResolveRepositoryRoot();
            if (!String.IsNullOrWhiteSpace(repositoryRoot))
            {
                AddUniquePath(candidates, Path.Combine(repositoryRoot, "aiml"));
            }

            string sambotRoot = ResolveSambotRoot();
            if (!String.IsNullOrWhiteSpace(sambotRoot))
            {
                AddUniquePath(candidates, Path.Combine(sambotRoot, "aiml"));

                DirectoryInfo sambotRootDirectory = new DirectoryInfo(sambotRoot);
                DirectoryInfo parent = sambotRootDirectory.Parent;
                while (parent != null)
                {
                    AddUniquePath(candidates, Path.Combine(parent.FullName, "aiml"));
                    parent = parent.Parent;
                }
            }

            AddUniquePath(candidates, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "aiml"));

            foreach (string candidate in candidates)
            {
                if (ContainsAimlFiles(candidate))
                {
                    return candidate;
                }
            }

            foreach (string candidate in candidates)
            {
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }
            }

            return "";
        }

        private static bool ContainsAimlFiles(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return false;
            }

            try
            {
                return Directory.GetFiles(directory, "*.aiml", SearchOption.AllDirectories).Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private static void TryWarmAimlModelBot(ref Bot bot, ref User user)
        {
            string message;
            if (!TryEnsureAimlModelBot(ref bot, ref user, true, out message))
            {
                Console.WriteLine("AIML GGUF test mode is not ready: " + message);
            }
        }

        private static bool TryEnsureAimlModelBot(ref Bot bot, ref User user, bool showMessages, out string message)
        {
            string modelPath = ResolveAimlKnowledgeModelPath();
            if (String.IsNullOrWhiteSpace(modelPath))
            {
                message = "Could not find an AIML GGUF model. Put aiml-*.gguf in sambot\\models or set SAMBOT_AIML_MODEL_PATH.";
                return false;
            }

            DateTime modelTimestampUtc = File.GetLastWriteTimeUtc(modelPath);
            string extractedAimlDirectory;
            if (
                !TryEnsureAimlModelAimlDirectory(
                    modelPath,
                    modelTimestampUtc,
                    showMessages,
                    out extractedAimlDirectory,
                    out message
                )
            )
            {
                return false;
            }

            bool needsReload =
                bot == null
                || user == null
                || !cachedAimlModelPath.Equals(modelPath, StringComparison.OrdinalIgnoreCase)
                || cachedAimlModelTimestampUtc != modelTimestampUtc
                || !cachedAimlModelAimlDirectory.Equals(extractedAimlDirectory, StringComparison.OrdinalIgnoreCase);

            if (!needsReload)
            {
                message = "";
                return true;
            }

            try
            {
                bot = CreateAimBot(extractedAimlDirectory);
                user = new User("consoleUser", bot);
                cachedAimlModelPath = modelPath;
                cachedAimlModelTimestampUtc = modelTimestampUtc;
                cachedAimlModelAimlDirectory = extractedAimlDirectory;

                if (showMessages)
                {
                    Console.WriteLine("AIML GGUF model loaded: " + modelPath);
                }

                message = "";
                return true;
            }
            catch (Exception ex)
            {
                message = "Could not load AIML content from GGUF: " + ex.Message;
                return false;
            }
        }

        private static bool TryEnsureAimlModelAimlDirectory(
            string modelPath,
            DateTime modelTimestampUtc,
            bool showMessages,
            out string aimlDirectory,
            out string message
        )
        {
            aimlDirectory = "";
            message = "";

            try
            {
                string cacheRoot = Path.Combine(Path.GetTempPath(), "sambot-aimlmodel-cache");
                Directory.CreateDirectory(cacheRoot);

                string cacheKey = modelPath.ToLowerInvariant() + "|" + modelTimestampUtc.Ticks;
                string cacheDirectoryName = "aiml-" + ComputeStableHash(cacheKey);
                string cacheDirectory = Path.Combine(cacheRoot, cacheDirectoryName);

                if (ContainsAimlFiles(cacheDirectory))
                {
                    aimlDirectory = cacheDirectory;
                    return true;
                }

                if (Directory.Exists(cacheDirectory))
                {
                    try
                    {
                        Directory.Delete(cacheDirectory, true);
                    }
                    catch
                    {
                    }
                }
                Directory.CreateDirectory(cacheDirectory);

                string sambotRoot = ResolveSambotRoot();
                if (String.IsNullOrWhiteSpace(sambotRoot))
                {
                    message = "Could not locate sambot root directory.";
                    return false;
                }

                string extractorScript = Path.Combine(sambotRoot, "extract_aiml_gguf.py");
                if (!File.Exists(extractorScript))
                {
                    message = "Missing AIML GGUF extractor script: " + extractorScript;
                    return false;
                }

                string pythonExe = GetSetting("SAMBOT_PYTHON_EXE", "PYTHON_EXE", "python");
                string bitNetRoot = ResolveBitNetRoot();
                string arguments = BuildExtractAimlModelArguments(
                    extractorScript,
                    modelPath,
                    cacheDirectory,
                    bitNetRoot
                );

                ProcessStartInfo startInfo = new ProcessStartInfo(pythonExe, arguments)
                {
                    WorkingDirectory = sambotRoot,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                Process process = Process.Start(startInfo);
                if (process == null)
                {
                    message = "Python process could not be started.";
                    return false;
                }

                using (process)
                {
                    string stdout = process.StandardOutput.ReadToEnd().Trim();
                    string stderr = process.StandardError.ReadToEnd().Trim();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        message = stderr.Length > 0
                            ? stderr
                            : (stdout.Length > 0 ? stdout : "Extractor exited with code " + process.ExitCode + ".");
                        return false;
                    }

                    if (!ContainsAimlFiles(cacheDirectory))
                    {
                        message = "Extractor succeeded but no AIML files were produced.";
                        return false;
                    }

                    aimlDirectory = cacheDirectory;
                    cachedAimlModelAimlDirectory = cacheDirectory;
                    if (showMessages && stdout.Length > 0)
                    {
                        Console.WriteLine(stdout);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "AIML GGUF extraction failed: " + ex.Message;
                return false;
            }
        }

        private static string BuildExtractAimlModelArguments(
            string extractorScriptPath,
            string modelPath,
            string outputDirectory,
            string bitNetRoot
        )
        {
            StringBuilder arguments = new StringBuilder();
            arguments.Append(QuoteCommandArgument(extractorScriptPath));
            arguments.Append(" --model ");
            arguments.Append(QuoteCommandArgument(modelPath));
            arguments.Append(" --output-dir ");
            arguments.Append(QuoteCommandArgument(outputDirectory));

            if (!String.IsNullOrWhiteSpace(bitNetRoot))
            {
                arguments.Append(" --bitnet-root ");
                arguments.Append(QuoteCommandArgument(bitNetRoot));
            }

            return arguments.ToString();
        }

        private static string ResolveAimlKnowledgeModelPath()
        {
            if (!String.IsNullOrWhiteSpace(startupAimlModelPath) && File.Exists(startupAimlModelPath))
            {
                return Path.GetFullPath(startupAimlModelPath.Trim());
            }

            string configuredPath = Environment.GetEnvironmentVariable("SAMBOT_AIML_MODEL_PATH");
            if (!String.IsNullOrWhiteSpace(configuredPath) && File.Exists(configuredPath))
            {
                return Path.GetFullPath(configuredPath.Trim());
            }

            List<string> modelDirectories = new List<string>();
            AddUniquePath(modelDirectories, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models"));

            string sambotRoot = ResolveSambotRoot();
            if (!String.IsNullOrWhiteSpace(sambotRoot))
            {
                AddUniquePath(modelDirectories, Path.Combine(sambotRoot, "models"));
            }

            string bitNetRoot = ResolveBitNetRoot();
            if (!String.IsNullOrWhiteSpace(bitNetRoot))
            {
                AddUniquePath(modelDirectories, Path.Combine(bitNetRoot, "models"));
            }

            List<string> candidates = new List<string>();
            foreach (string directory in modelDirectories)
            {
                if (!Directory.Exists(directory))
                {
                    continue;
                }

                try
                {
                    string[] preferred = Directory.GetFiles(directory, "aiml*.gguf", SearchOption.AllDirectories);
                    foreach (string path in preferred)
                    {
                        AddUniquePath(candidates, path);
                    }

                    string[] fallback = Directory.GetFiles(directory, "*aiml*.gguf", SearchOption.AllDirectories);
                    foreach (string path in fallback)
                    {
                        AddUniquePath(candidates, path);
                    }
                }
                catch
                {
                }
            }

            string newestPath = "";
            DateTime newestTimestamp = DateTime.MinValue;
            foreach (string candidate in candidates)
            {
                try
                {
                    DateTime timestamp = File.GetLastWriteTimeUtc(candidate);
                    if (newestPath.Length == 0 || timestamp > newestTimestamp)
                    {
                        newestPath = candidate;
                        newestTimestamp = timestamp;
                    }
                }
                catch
                {
                }
            }

            return newestPath;
        }

        private static void TryWarmNewsModel()
        {
            string message;
            if (!TryEnsureNewsModelLoaded(true, out message))
            {
                Console.WriteLine("NEWS GGUF test mode is not ready: " + message);
            }
        }

        private static bool TryEnsureNewsModelLoaded(bool showMessages, out string message)
        {
            string modelPath = ResolveNewsKnowledgeModelPath();
            if (String.IsNullOrWhiteSpace(modelPath))
            {
                message = "Could not find a News GGUF model. Put *news*.gguf in sambot\\models or set SAMBOT_NEWS_MODEL_PATH.";
                return false;
            }

            DateTime modelTimestampUtc = File.GetLastWriteTimeUtc(modelPath);
            string articleFilePath;
            if (
                !TryEnsureNewsModelArticlesFile(
                    modelPath,
                    modelTimestampUtc,
                    showMessages,
                    out articleFilePath,
                    out message
                )
            )
            {
                return false;
            }

            bool needsReload =
                cachedNewsModelArticles.Count == 0
                || !cachedNewsModelPath.Equals(modelPath, StringComparison.OrdinalIgnoreCase)
                || cachedNewsModelTimestampUtc != modelTimestampUtc
                || !cachedNewsModelArticlesPath.Equals(articleFilePath, StringComparison.OrdinalIgnoreCase);

            if (!needsReload)
            {
                message = "";
                return true;
            }

            try
            {
                List<NewsArticleEntry> loadedArticles = LoadNewsArticlesFromTsv(articleFilePath);
                if (loadedArticles.Count == 0)
                {
                    message = "Extractor produced zero news articles.";
                    return false;
                }

                cachedNewsModelArticles = loadedArticles;
                cachedNewsModelPath = modelPath;
                cachedNewsModelTimestampUtc = modelTimestampUtc;
                cachedNewsModelArticlesPath = articleFilePath;

                if (showMessages)
                {
                    Console.WriteLine("News GGUF model loaded: " + modelPath);
                    Console.WriteLine("News articles loaded: " + loadedArticles.Count);
                }

                message = "";
                return true;
            }
            catch (Exception ex)
            {
                message = "Could not load News GGUF entries: " + ex.Message;
                return false;
            }
        }

        private static bool TryEnsureNewsModelArticlesFile(
            string modelPath,
            DateTime modelTimestampUtc,
            bool showMessages,
            out string articleFilePath,
            out string message
        )
        {
            articleFilePath = "";
            message = "";

            try
            {
                string cacheRoot = Path.Combine(Path.GetTempPath(), "sambot-newsmodel-cache");
                Directory.CreateDirectory(cacheRoot);

                string cacheKey = modelPath.ToLowerInvariant() + "|" + modelTimestampUtc.Ticks;
                string cacheDirectoryName = "news-" + ComputeStableHash(cacheKey);
                string cacheDirectory = Path.Combine(cacheRoot, cacheDirectoryName);
                string targetArticleFile = Path.Combine(cacheDirectory, "articles.tsv");

                if (File.Exists(targetArticleFile) && new FileInfo(targetArticleFile).Length > 0)
                {
                    articleFilePath = targetArticleFile;
                    return true;
                }

                if (Directory.Exists(cacheDirectory))
                {
                    try
                    {
                        Directory.Delete(cacheDirectory, true);
                    }
                    catch
                    {
                    }
                }
                Directory.CreateDirectory(cacheDirectory);

                string sambotRoot = ResolveSambotRoot();
                if (String.IsNullOrWhiteSpace(sambotRoot))
                {
                    message = "Could not locate sambot root directory.";
                    return false;
                }

                string extractorScript = Path.Combine(sambotRoot, "extract_news_gguf.py");
                if (!File.Exists(extractorScript))
                {
                    message = "Missing News GGUF extractor script: " + extractorScript;
                    return false;
                }

                string pythonExe = GetSetting("SAMBOT_PYTHON_EXE", "PYTHON_EXE", "python");
                string bitNetRoot = ResolveBitNetRoot();
                string arguments = BuildExtractNewsModelArguments(
                    extractorScript,
                    modelPath,
                    targetArticleFile,
                    bitNetRoot
                );

                ProcessStartInfo startInfo = new ProcessStartInfo(pythonExe, arguments)
                {
                    WorkingDirectory = sambotRoot,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                Process process = Process.Start(startInfo);
                if (process == null)
                {
                    message = "Python process could not be started.";
                    return false;
                }

                using (process)
                {
                    string stdout = process.StandardOutput.ReadToEnd().Trim();
                    string stderr = process.StandardError.ReadToEnd().Trim();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        message = stderr.Length > 0
                            ? stderr
                            : (stdout.Length > 0 ? stdout : "Extractor exited with code " + process.ExitCode + ".");
                        return false;
                    }

                    if (!File.Exists(targetArticleFile) || new FileInfo(targetArticleFile).Length == 0)
                    {
                        message = "Extractor succeeded but no article file was produced.";
                        return false;
                    }

                    articleFilePath = targetArticleFile;
                    if (showMessages && stdout.Length > 0)
                    {
                        Console.WriteLine(stdout);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "News GGUF extraction failed: " + ex.Message;
                return false;
            }
        }

        private static string BuildExtractNewsModelArguments(
            string extractorScriptPath,
            string modelPath,
            string outputPath,
            string bitNetRoot
        )
        {
            StringBuilder arguments = new StringBuilder();
            arguments.Append(QuoteCommandArgument(extractorScriptPath));
            arguments.Append(" --model ");
            arguments.Append(QuoteCommandArgument(modelPath));
            arguments.Append(" --output ");
            arguments.Append(QuoteCommandArgument(outputPath));

            if (!String.IsNullOrWhiteSpace(bitNetRoot))
            {
                arguments.Append(" --bitnet-root ");
                arguments.Append(QuoteCommandArgument(bitNetRoot));
            }

            return arguments.ToString();
        }

        private static string ResolveNewsKnowledgeModelPath()
        {
            if (!String.IsNullOrWhiteSpace(startupNewsModelPath) && File.Exists(startupNewsModelPath))
            {
                return Path.GetFullPath(startupNewsModelPath.Trim());
            }

            string configuredPath = Environment.GetEnvironmentVariable("SAMBOT_NEWS_MODEL_PATH");
            if (!String.IsNullOrWhiteSpace(configuredPath) && File.Exists(configuredPath))
            {
                return Path.GetFullPath(configuredPath.Trim());
            }

            List<string> modelDirectories = new List<string>();
            AddUniquePath(modelDirectories, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models"));

            string sambotRoot = ResolveSambotRoot();
            if (!String.IsNullOrWhiteSpace(sambotRoot))
            {
                AddUniquePath(modelDirectories, Path.Combine(sambotRoot, "models"));
            }

            string bitNetRoot = ResolveBitNetRoot();
            if (!String.IsNullOrWhiteSpace(bitNetRoot))
            {
                AddUniquePath(modelDirectories, Path.Combine(bitNetRoot, "models"));
            }

            List<string> candidates = new List<string>();
            foreach (string directory in modelDirectories)
            {
                if (!Directory.Exists(directory))
                {
                    continue;
                }

                try
                {
                    string[] preferred = Directory.GetFiles(directory, "sambot_news*.gguf", SearchOption.AllDirectories);
                    foreach (string path in preferred)
                    {
                        AddUniquePath(candidates, path);
                    }

                    string[] newsPrefixed = Directory.GetFiles(directory, "news*.gguf", SearchOption.AllDirectories);
                    foreach (string path in newsPrefixed)
                    {
                        AddUniquePath(candidates, path);
                    }

                    string[] fallback = Directory.GetFiles(directory, "*news*.gguf", SearchOption.AllDirectories);
                    foreach (string path in fallback)
                    {
                        AddUniquePath(candidates, path);
                    }
                }
                catch
                {
                }
            }

            string newestPath = "";
            DateTime newestTimestamp = DateTime.MinValue;
            foreach (string candidate in candidates)
            {
                try
                {
                    DateTime timestamp = File.GetLastWriteTimeUtc(candidate);
                    if (newestPath.Length == 0 || timestamp > newestTimestamp)
                    {
                        newestPath = candidate;
                        newestTimestamp = timestamp;
                    }
                }
                catch
                {
                }
            }

            return newestPath;
        }

        private static List<NewsArticleEntry> LoadNewsArticlesFromTsv(string filePath)
        {
            List<NewsArticleEntry> articles = new List<NewsArticleEntry>();
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    NewsArticleEntry article;
                    if (TryParseNewsArticleLine(line, out article))
                    {
                        articles.Add(article);
                    }
                }
            }

            return articles;
        }

        private static bool TryParseNewsArticleLine(string line, out NewsArticleEntry article)
        {
            article = null;
            if (String.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            string[] fields = line.Split('\t');
            if (fields.Length < 6)
            {
                return false;
            }

            string title = fields[0].Trim();
            string summary = fields[1].Trim();
            string body = fields[2].Trim();
            string published = fields[3].Trim();
            string category = fields[4].Trim();
            string link = fields[5].Trim();

            article = new NewsArticleEntry
            {
                Title = title,
                Summary = summary,
                ArticleText = body,
                PublishedLabel = published,
                CategorySlug = category,
                Link = link,
                SearchTitle = NormalizeNewsSearchText(title),
                SearchSummary = NormalizeNewsSearchText(summary),
                SearchBody = NormalizeNewsSearchText(body),
                SearchCategory = NormalizeNewsSearchText(category)
            };
            return true;
        }

        private static string NormalizeNewsSearchText(string value)
        {
            string source = (value ?? "").ToLowerInvariant();
            StringBuilder normalized = new StringBuilder(source.Length);
            bool previousWasSpace = true;
            for (int i = 0; i < source.Length; i++)
            {
                char current = source[i];
                if (Char.IsLetterOrDigit(current))
                {
                    normalized.Append(current);
                    previousWasSpace = false;
                }
                else if (!previousWasSpace)
                {
                    normalized.Append(' ');
                    previousWasSpace = true;
                }
            }

            return normalized.ToString().Trim();
        }

        private static List<string> BuildNewsSearchTokens(string normalizedQuery)
        {
            List<string> tokens = new List<string>();
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string[] parts = normalizedQuery.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                string token = parts[i].Trim();
                if (token.Length < 2 || seen.Contains(token))
                {
                    continue;
                }

                seen.Add(token);
                tokens.Add(token);
            }

            return tokens;
        }

        private static int ScoreNewsArticle(
            NewsArticleEntry article,
            string normalizedQuery,
            List<string> queryTokens
        )
        {
            int score = 0;

            if (normalizedQuery.Length > 0)
            {
                if (article.SearchTitle.Contains(normalizedQuery))
                {
                    score += 30;
                }
                if (article.SearchSummary.Contains(normalizedQuery))
                {
                    score += 18;
                }
                if (article.SearchBody.Contains(normalizedQuery))
                {
                    score += 12;
                }
                if (article.SearchCategory.Contains(normalizedQuery))
                {
                    score += 10;
                }
            }

            for (int i = 0; i < queryTokens.Count; i++)
            {
                string token = queryTokens[i];
                if (article.SearchTitle.Contains(token))
                {
                    score += 8;
                }
                if (article.SearchSummary.Contains(token))
                {
                    score += 4;
                }
                if (article.SearchBody.Contains(token))
                {
                    score += 2;
                }
                if (article.SearchCategory.Contains(token))
                {
                    score += 3;
                }
            }

            return score;
        }

        private static string TruncateText(string value, int maxLength)
        {
            string cleaned = (value ?? "").Trim();
            if (cleaned.Length <= maxLength)
            {
                return cleaned;
            }

            return cleaned.Substring(0, maxLength).TrimEnd() + "...";
        }

        private static string ComputeStableHash(string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value ?? "");
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(data);
                StringBuilder builder = new StringBuilder(hash.Length * 2);
                foreach (byte valueByte in hash)
                {
                    builder.Append(valueByte.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static bool HandleCommand(
            string input,
            SpeechSynthesizer synth,
            ref ChatMode currentMode,
            out bool shouldExit
        )
        {
            shouldExit = false;
            string normalized = input.Trim().ToLowerInvariant();

            if (normalized == "quit" || normalized == "exit")
            {
                Console.WriteLine("Good bye human :(");
                shouldExit = true;
                return true;
            }

            if (normalized == "end")
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.WriteLine("3");
                Console.WriteLine("2");
                Console.WriteLine("1");
                synth.Speak("Stopping");
                Console.ResetColor();
                shouldExit = true;
                return true;
            }

            if (normalized == "mode")
            {
                Console.WriteLine("Current mode: " + GetModeLabel(currentMode));
                return true;
            }

            if (normalized.StartsWith("mode ", StringComparison.OrdinalIgnoreCase))
            {
                ChatMode requestedMode;
                if (TryParseMode(input.Substring(5), out requestedMode))
                {
                    currentMode = requestedMode;
                    Console.WriteLine("Mode changed to " + GetModeLabel(currentMode) + ".");
                    if (currentMode == ChatMode.BitNet)
                    {
                        string startupMessage;
                        EnsureBitNetServerStarted(true, out startupMessage);
                    }
                }
                else
                {
                    Console.WriteLine("Use: mode aim  or  mode bitnet  or  mode aimlmodel  or  mode newsmodel");
                }
                return true;
            }

            if (normalized == "cls" || normalized == "clear")
            {
                Console.Clear();
                return true;
            }

            if (normalized == "1776")
            {
                Process.Start("cmd.exe", "echo hello");
                return true;
            }

            if (normalized == "sleep now")
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.Write("Putting Display To Sleep In Three Seconds");
                synth.Speak("Putting Display To Sleep In Three Seconds");
                System.Threading.Thread.Sleep(3000);
                Process.Start("sleeptime.bat");
                Console.ResetColor();
                return true;
            }

            if (normalized == "reset 1")
            {
                Process.Start("network_reset.bat");
                return true;
            }

            if (normalized == "lock")
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.Write("Locking System");
                synth.Speak("Locking System");
                Process.Start("Rundll32.exe", "User32.dll,LockWorkStation");
                Console.ResetColor();
                return true;
            }

            if (normalized == "ip1")
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.WriteLine("IP command is ready.");
                Console.ResetColor();
                return true;
            }

            return false;
        }

        private static string SendAimRequest(string input, ref Bot bot, ref User user)
        {
            try
            {
                EnsureAimBot(ref bot, ref user);
            }
            catch (Exception ex)
            {
                return "AIM could not start: " + ex.Message;
            }

            Request request = new Request(input, user, bot);
            Result result = bot.Chat(request);
            return result.Output;
        }

        private static string SendAimlModelRequest(string input, ref Bot bot, ref User user)
        {
            string message;
            if (!TryEnsureAimlModelBot(ref bot, ref user, false, out message))
            {
                return "AIML GGUF model test mode is not ready: " + message;
            }

            Request request = new Request(input, user, bot);
            Result result = bot.Chat(request);
            return result.Output;
        }

        private static string SendNewsModelRequest(string input)
        {
            string message;
            if (!TryEnsureNewsModelLoaded(false, out message))
            {
                return "NEWS GGUF test mode is not ready: " + message;
            }

            string normalizedQuery = NormalizeNewsSearchText(input);
            if (normalizedQuery.Length == 0)
            {
                return "Ask about a topic and I will search the loaded news model.";
            }

            List<string> tokens = BuildNewsSearchTokens(normalizedQuery);
            if (tokens.Count == 0)
            {
                return "Try a longer topic so I can search the news model.";
            }

            List<NewsSearchMatch> matches = new List<NewsSearchMatch>();
            for (int i = 0; i < cachedNewsModelArticles.Count; i++)
            {
                NewsArticleEntry article = cachedNewsModelArticles[i];
                int score = ScoreNewsArticle(article, normalizedQuery, tokens);
                if (score > 0)
                {
                    matches.Add(new NewsSearchMatch(article, score));
                }
            }

            if (matches.Count == 0)
            {
                return "No close news match was found in the loaded News GGUF model.";
            }

            matches.Sort((left, right) => right.Score.CompareTo(left.Score));
            int resultCount = Math.Min(3, matches.Count);

            StringBuilder response = new StringBuilder();
            response.Append("Top news matches:");
            for (int index = 0; index < resultCount; index++)
            {
                NewsArticleEntry article = matches[index].Article;
                string title = article.Title.Length > 0 ? article.Title : "(untitled article)";
                string snippet = article.Summary.Length > 0 ? article.Summary : article.ArticleText;
                snippet = TruncateText(snippet, 220);

                response.AppendLine();
                response.Append(index + 1);
                response.Append(") ");
                response.Append(title);

                if (article.PublishedLabel.Length > 0 || article.CategorySlug.Length > 0)
                {
                    response.Append(" [");
                    if (article.PublishedLabel.Length > 0)
                    {
                        response.Append(article.PublishedLabel);
                    }
                    if (article.PublishedLabel.Length > 0 && article.CategorySlug.Length > 0)
                    {
                        response.Append(" | ");
                    }
                    if (article.CategorySlug.Length > 0)
                    {
                        response.Append(article.CategorySlug);
                    }
                    response.Append("]");
                }

                if (snippet.Length > 0)
                {
                    response.Append(" - ");
                    response.Append(snippet);
                }

                if (article.Link.Length > 0)
                {
                    response.Append(" (");
                    response.Append(article.Link);
                    response.Append(")");
                }
            }

            return response.ToString();
        }

        private static string SendBitNetRequest(string input)
        {
            string startupMessage;
            if (!EnsureBitNetServerStarted(false, out startupMessage))
            {
                return "BitNet is not ready: " + startupMessage;
            }

            string baseUrl = GetBitNetBaseUrl();
            string model = GetSetting("SAMBOT_BITNET_MODEL", "BITNET_MODEL_NAME", DefaultBitNetModel);
            int maxTokens = GetIntSetting("SAMBOT_BITNET_MAX_TOKENS", "BITNET_MAX_TOKENS", 256, 32, 4096);
            int timeoutSeconds = GetIntSetting("SAMBOT_BITNET_TIMEOUT_SECONDS", "BITNET_TIMEOUT_SECONDS", 90, 5, 600);
            double temperature = GetDoubleSetting("SAMBOT_BITNET_TEMPERATURE", "BITNET_TEMPERATURE", 0.7, 0.0, 2.0);

            Dictionary<string, object> chatPayload = new Dictionary<string, object>();
            chatPayload["model"] = model;
            chatPayload["messages"] = new object[]
            {
                new Dictionary<string, object>
                {
                    {"role", "system"},
                    {"content", "You are Sam Bot using the local BitNet/GGUF backend. Answer directly and keep replies focused."}
                },
                new Dictionary<string, object>
                {
                    {"role", "user"},
                    {"content", input}
                }
            };
            chatPayload["temperature"] = temperature;
            chatPayload["max_tokens"] = maxTokens;
            chatPayload["stream"] = false;

            ApiResponse chatResponse = PostJson(baseUrl + "/v1/chat/completions", chatPayload, timeoutSeconds);
            if (chatResponse.Success)
            {
                string reply = ExtractReply(chatResponse.Body);
                if (reply.Length > 0)
                {
                    return reply;
                }

                return "BitNet returned a response, but no reply text was found.";
            }

            if (chatResponse.StatusCode != 404 && chatResponse.Error.Length > 0)
            {
                return "BitNet request failed: " + chatResponse.Error;
            }

            Dictionary<string, object> completionPayload = new Dictionary<string, object>();
            completionPayload["prompt"] = BuildCompletionPrompt(input);
            completionPayload["n_predict"] = maxTokens;
            completionPayload["temperature"] = temperature;
            completionPayload["stream"] = false;
            completionPayload["stop"] = new string[] { "\nUSER:", "\nUser:", "\nSYSTEM:" };

            ApiResponse completionResponse = PostJson(baseUrl + "/completion", completionPayload, timeoutSeconds);
            if (!completionResponse.Success)
            {
                return "BitNet request failed: " + completionResponse.Error;
            }

            string completionReply = ExtractReply(completionResponse.Body);
            return completionReply.Length > 0
                ? completionReply
                : "BitNet returned a response, but no reply text was found.";
        }

        private static void SpeakResponse(SpeechSynthesizer synth, ChatMode mode, string output)
        {
            if (synth == null || !IsSpeechEnabled())
            {
                return;
            }

            if (mode == ChatMode.NewsModel && !IsNewsSpeechEnabled())
            {
                return;
            }

            string text = (output ?? "").Trim();
            if (text.Length == 0)
            {
                return;
            }

            int maxCharacters = GetIntSetting("SAMBOT_TTS_MAX_CHARS", "TTS_MAX_CHARS", 320, 32, 4000);
            if (text.Length > maxCharacters)
            {
                text = text.Substring(0, maxCharacters).TrimEnd() + "...";
            }

            try
            {
                synth.SpeakAsyncCancelAll();
                synth.SpeakAsync(text);
            }
            catch
            {
            }
        }

        private static bool IsSpeechEnabled()
        {
            string configured = Environment.GetEnvironmentVariable("SAMBOT_TTS_ENABLED");
            if (String.IsNullOrWhiteSpace(configured))
            {
                return true;
            }

            return IsTruthySetting(configured);
        }

        private static bool IsNewsSpeechEnabled()
        {
            string configured = Environment.GetEnvironmentVariable("SAMBOT_TTS_NEWS_ENABLED");
            if (String.IsNullOrWhiteSpace(configured))
            {
                return false;
            }

            return IsTruthySetting(configured);
        }

        private static bool IsTruthySetting(string value)
        {
            string normalized = (value ?? "").Trim().ToLowerInvariant();
            return normalized == "1"
                || normalized == "true"
                || normalized == "yes"
                || normalized == "on";
        }

        private static ApiResponse PostJson(string url, Dictionary<string, object> payload, int timeoutSeconds)
        {
            string json = JsonSerializer.Serialize(payload);
            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Timeout = timeoutSeconds * 1000;
            request.ReadWriteTimeout = timeoutSeconds * 1000;
            request.ContentLength = body.Length;

            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string responseText = ReadResponseText(response);
                    int statusCode = (int)response.StatusCode;
                    return new ApiResponse(statusCode >= 200 && statusCode < 300, statusCode, responseText, "");
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    string responseText = ReadResponseText(response);
                    int statusCode = (int)response.StatusCode;
                    string message = responseText.Length > 0 ? responseText : response.StatusDescription;
                    return new ApiResponse(false, statusCode, responseText, message);
                }

                return new ApiResponse(false, 0, "", ex.Message);
            }
            catch (Exception ex)
            {
                return new ApiResponse(false, 0, "", ex.Message);
            }
        }

        private static string ReadResponseText(HttpWebResponse response)
        {
            using (Stream stream = response.GetResponseStream())
            {
                if (stream == null)
                {
                    return "";
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static string ExtractReply(string responseBody)
        {
            if ((responseBody ?? "").Trim().Length == 0)
            {
                return "";
            }

            try
            {
                Dictionary<string, object> payload = JsonSerializer.DeserializeObject(responseBody) as Dictionary<string, object>;
                if (payload == null)
                {
                    return "";
                }

                object choicesObject;
                if (payload.TryGetValue("choices", out choicesObject))
                {
                    object[] choices = choicesObject as object[];
                    if (choices != null && choices.Length > 0)
                    {
                        Dictionary<string, object> firstChoice = choices[0] as Dictionary<string, object>;
                        if (firstChoice != null)
                        {
                            object messageObject;
                            if (firstChoice.TryGetValue("message", out messageObject))
                            {
                                Dictionary<string, object> message = messageObject as Dictionary<string, object>;
                                if (message != null)
                                {
                                    string content = GetStringValue(message, "content");
                                    if (content.Length > 0)
                                    {
                                        return content;
                                    }
                                }
                            }

                            string text = GetStringValue(firstChoice, "text");
                            if (text.Length > 0)
                            {
                                return text;
                            }
                        }
                    }
                }

                string[] keys = new string[] { "content", "response", "generated_text", "text" };
                foreach (string key in keys)
                {
                    string value = GetStringValue(payload, key);
                    if (value.Length > 0)
                    {
                        return value;
                    }
                }
            }
            catch
            {
                return "";
            }

            return "";
        }

        private static string GetStringValue(Dictionary<string, object> payload, string key)
        {
            object value;
            if (!payload.TryGetValue(key, out value) || value == null)
            {
                return "";
            }

            return value.ToString().Trim();
        }

        private static string BuildCompletionPrompt(string input)
        {
            return "SYSTEM: You are Sam Bot using the local BitNet/GGUF backend. Answer directly and keep replies focused.\n\n"
                + "USER: " + input + "\nASSISTANT:";
        }

        private static string GetBitNetBaseUrl()
        {
            return GetSetting("SAMBOT_BITNET_URL", "BITNET_SERVER_URL", DefaultBitNetUrl).TrimEnd('/');
        }

        private static bool EnsureBitNetServerStarted(bool showMessages, out string statusMessage)
        {
            if (IsBitNetReachable())
            {
                bitNetStartupError = "";
                statusMessage = "";
                return true;
            }

            if (bitNetServerProcess != null && !bitNetServerProcess.HasExited)
            {
                statusMessage = "Local GGUF server is still starting.";
                return false;
            }

            string customCommand = Environment.GetEnvironmentVariable("SAMBOT_BITNET_START_COMMAND");
            if (String.IsNullOrWhiteSpace(customCommand))
            {
                string defaultStartIssue = GetDefaultBitNetStartupIssue();
                if (defaultStartIssue.Length > 0)
                {
                    bitNetStartupError = defaultStartIssue;
                    statusMessage = bitNetStartupError;
                    if (showMessages)
                    {
                        Console.WriteLine("Local GGUF server cannot start: " + bitNetStartupError);
                    }
                    return false;
                }
            }

            ProcessStartInfo startInfo = !String.IsNullOrWhiteSpace(customCommand)
                ? BuildCustomStartInfo(customCommand.Trim())
                : BuildDefaultBitNetStartInfo();

            if (startInfo == null)
            {
                bitNetStartupError = "Set SAMBOT_BITNET_START_COMMAND or build BitNet\\build\\bin\\Release\\llama-server.exe.";
                statusMessage = bitNetStartupError;
                if (showMessages)
                {
                    Console.WriteLine("BitNet server is not running. " + bitNetStartupError);
                }
                return false;
            }

            try
            {
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                bitNetServerProcess = Process.Start(startInfo);
                if (showMessages)
                {
                    Console.WriteLine("Starting local GGUF server...");
                }

                if (!WaitForBitNet(20))
                {
                    string startupError = ReadStartedProcessError();
                    bitNetStartupError = startupError.Length > 0
                        ? startupError
                        : "Local GGUF server did not become ready yet.";
                    statusMessage = bitNetStartupError;
                    if (showMessages)
                    {
                        Console.WriteLine("Local GGUF server did not become ready: " + bitNetStartupError);
                    }
                    return false;
                }

                bitNetStartupError = "";
                statusMessage = "";
                return true;
            }
            catch (Exception ex)
            {
                bitNetStartupError = ex.Message;
                statusMessage = bitNetStartupError;
                if (showMessages)
                {
                    Console.WriteLine("Could not start local GGUF server: " + bitNetStartupError);
                }
                return false;
            }
        }

        private static void PrintThinking(ChatMode mode)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(GetModeLabel(mode) + ": Thinking...");
            Console.ResetColor();
        }

        private static string GetDefaultBitNetStartupIssue()
        {
            if (String.IsNullOrWhiteSpace(ResolveBitNetServerExe()))
            {
                return "Could not find llama-server.exe. Build BitNet first or set SAMBOT_BITNET_SERVER_EXE.";
            }

            if (String.IsNullOrWhiteSpace(ResolveBitNetModelPath()))
            {
                return "Could not find a GGUF model. Add a standard .gguf or .ggml model under sambot\\models or BitNet\\models, or set SAMBOT_BITNET_MODEL_PATH.";
            }

            return "";
        }

        private static ProcessStartInfo BuildCustomStartInfo(string command)
        {
            return new ProcessStartInfo("cmd.exe", "/c " + command);
        }

        private static ProcessStartInfo BuildDefaultBitNetStartInfo()
        {
            string serverExe = ResolveBitNetServerExe();
            string modelPath = ResolveBitNetModelPath();
            if (!String.IsNullOrWhiteSpace(serverExe) && !String.IsNullOrWhiteSpace(modelPath))
            {
                string args =
                    "-m \"" + modelPath + "\" "
                    + "-c 2048 -t 2 -n 256 -ngl 0 --temp 0.8 "
                    + "--host 127.0.0.1 --port " + GetBitNetPort() + " -cb";
                return new ProcessStartInfo(serverExe, args)
                {
                    WorkingDirectory = Path.GetDirectoryName(serverExe)
                };
            }

            string bitNetRoot = ResolveBitNetRoot();
            string serverScript = String.IsNullOrWhiteSpace(bitNetRoot)
                ? ""
                : Path.Combine(bitNetRoot, "run_inference_server.py");
            if (File.Exists(serverScript))
            {
                string pythonExe = GetSetting("SAMBOT_PYTHON_EXE", "PYTHON_EXE", "python");
                string args = "\"" + serverScript + "\" --host 127.0.0.1 --port " + GetBitNetPort();
                if (!String.IsNullOrWhiteSpace(modelPath))
                {
                    args += " --model \"" + modelPath + "\"";
                }

                return new ProcessStartInfo(pythonExe, args)
                {
                    WorkingDirectory = bitNetRoot
                };
            }

            return null;
        }

        private static string ResolveBitNetServerExe()
        {
            string configuredPath = Environment.GetEnvironmentVariable("SAMBOT_BITNET_SERVER_EXE");
            if (!String.IsNullOrWhiteSpace(configuredPath) && File.Exists(configuredPath))
            {
                return configuredPath.Trim();
            }

            string root = ResolveBitNetRoot();
            if (String.IsNullOrWhiteSpace(root))
            {
                return "";
            }

            string[] candidates = new string[]
            {
                Path.Combine(root, "build", "bin", "Release", "llama-server.exe"),
                Path.Combine(root, "build", "bin", "llama-server.exe")
            };

            foreach (string candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return "";
        }

        private static string ResolveBitNetModelPath()
        {
            string configuredPath = Environment.GetEnvironmentVariable("SAMBOT_BITNET_MODEL_PATH");
            if (!String.IsNullOrWhiteSpace(configuredPath) && File.Exists(configuredPath))
            {
                return configuredPath.Trim();
            }

            List<string> modelDirectories = new List<string>();
            AddUniquePath(modelDirectories, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models"));

            string sambotRoot = ResolveSambotRoot();
            if (!String.IsNullOrWhiteSpace(sambotRoot))
            {
                AddUniquePath(modelDirectories, Path.Combine(sambotRoot, "models"));
            }

            string root = ResolveBitNetRoot();
            if (!String.IsNullOrWhiteSpace(root))
            {
                AddUniquePath(modelDirectories, Path.Combine(root, "models"));
            }

            foreach (string modelsDirectory in modelDirectories)
            {
                string modelPath = FindFirstModelPath(modelsDirectory);
                if (modelPath.Length > 0)
                {
                    return modelPath;
                }
            }

            return "";
        }

        private static void AddUniquePath(List<string> paths, string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                return;
            }

            foreach (string existingPath in paths)
            {
                if (existingPath.Equals(path, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            paths.Add(path);
        }

        private static string FindFirstModelPath(string modelsDirectory)
        {
            if (!Directory.Exists(modelsDirectory))
            {
                return "";
            }

            string[] candidates = Directory.GetFiles(modelsDirectory, "*.gguf", SearchOption.AllDirectories);
            if (candidates.Length == 0)
            {
                candidates = Directory.GetFiles(modelsDirectory, "*.ggml", SearchOption.AllDirectories);
            }

            return candidates.Length > 0 ? candidates[0] : "";
        }

        private static string ResolveSambotRoot()
        {
            DirectoryInfo directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (directory != null)
            {
                if (
                    File.Exists(Path.Combine(directory.FullName, "sambot.csproj"))
                    || File.Exists(Path.Combine(directory.FullName, "build_aimlbot.ps1"))
                )
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            return "";
        }

        private static string ResolveRepositoryRoot()
        {
            DirectoryInfo directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (directory != null)
            {
                if (Directory.Exists(Path.Combine(directory.FullName, ".git")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            return "";
        }

        private static string ResolveBitNetRoot()
        {
            DirectoryInfo directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (directory != null)
            {
                string scriptPath = Path.Combine(directory.FullName, "run_inference_server.py");
                if (File.Exists(scriptPath))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            return "";
        }

        private static int GetBitNetPort()
        {
            Uri uri;
            if (Uri.TryCreate(GetBitNetBaseUrl(), UriKind.Absolute, out uri) && uri.Port > 0)
            {
                return uri.Port;
            }

            return 5052;
        }

        private static bool WaitForBitNet(int timeoutSeconds)
        {
            DateTime deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);
            while (DateTime.UtcNow < deadline)
            {
                if (IsBitNetReachable())
                {
                    Console.WriteLine("Local GGUF server is ready.");
                    return true;
                }

                System.Threading.Thread.Sleep(1000);
            }

            return false;
        }

        private static bool IsBitNetReachable()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetBitNetBaseUrl() + "/health");
                request.Method = "GET";
                request.Timeout = 1500;
                request.ReadWriteTimeout = 1500;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    int statusCode = (int)response.StatusCode;
                    return statusCode >= 200 && statusCode < 500;
                }
            }
            catch
            {
                return false;
            }
        }

        private static string ReadStartedProcessError()
        {
            if (bitNetServerProcess == null)
            {
                return "";
            }

            try
            {
                if (!bitNetServerProcess.HasExited)
                {
                    return "";
                }

                string stderr = bitNetServerProcess.StandardError.ReadToEnd().Trim();
                string stdout = bitNetServerProcess.StandardOutput.ReadToEnd().Trim();
                if (stderr.Length > 0)
                {
                    return stderr;
                }

                return stdout;
            }
            catch
            {
                return "";
            }
        }

        private static void StopStartedBitNetServer()
        {
            if (bitNetServerProcess == null)
            {
                return;
            }

            try
            {
                if (!bitNetServerProcess.HasExited)
                {
                    Process.Start("taskkill.exe", "/PID " + bitNetServerProcess.Id + " /T /F");
                }
            }
            catch
            {
            }
        }

        private static string GetSetting(string primaryName, string fallbackName, string defaultValue)
        {
            string value = Environment.GetEnvironmentVariable(primaryName);
            if (!String.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            value = Environment.GetEnvironmentVariable(fallbackName);
            if (!String.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            return defaultValue;
        }

        private static int GetIntSetting(
            string primaryName,
            string fallbackName,
            int defaultValue,
            int minValue,
            int maxValue
        )
        {
            int parsed;
            if (
                Int32.TryParse(Environment.GetEnvironmentVariable(primaryName), out parsed)
                || Int32.TryParse(Environment.GetEnvironmentVariable(fallbackName), out parsed)
            )
            {
                return Math.Max(minValue, Math.Min(maxValue, parsed));
            }

            return defaultValue;
        }

        private static double GetDoubleSetting(
            string primaryName,
            string fallbackName,
            double defaultValue,
            double minValue,
            double maxValue
        )
        {
            double parsed;
            if (
                Double.TryParse(Environment.GetEnvironmentVariable(primaryName), out parsed)
                || Double.TryParse(Environment.GetEnvironmentVariable(fallbackName), out parsed)
            )
            {
                return Math.Max(minValue, Math.Min(maxValue, parsed));
            }

            return defaultValue;
        }

        private static string GetModeLabel(ChatMode mode)
        {
            if (mode == ChatMode.BitNet)
            {
                return "BitNet";
            }

            if (mode == ChatMode.AimlModel)
            {
                return "AIML-GGUF";
            }

            if (mode == ChatMode.NewsModel)
            {
                return "NEWS-GGUF";
            }

            return "AIM";
        }
    }

    internal class NewsArticleEntry
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ArticleText { get; set; }
        public string PublishedLabel { get; set; }
        public string CategorySlug { get; set; }
        public string Link { get; set; }
        public string SearchTitle { get; set; }
        public string SearchSummary { get; set; }
        public string SearchBody { get; set; }
        public string SearchCategory { get; set; }
    }

    internal class NewsSearchMatch
    {
        public NewsSearchMatch(NewsArticleEntry article, int score)
        {
            Article = article;
            Score = score;
        }

        public NewsArticleEntry Article { get; private set; }
        public int Score { get; private set; }
    }

    internal class ApiResponse
    {
        public ApiResponse(bool success, int statusCode, string body, string error)
        {
            Success = success;
            StatusCode = statusCode;
            Body = body ?? "";
            Error = error ?? "";
        }

        public bool Success { get; private set; }
        public int StatusCode { get; private set; }
        public string Body { get; private set; }
        public string Error { get; private set; }
    }
}
