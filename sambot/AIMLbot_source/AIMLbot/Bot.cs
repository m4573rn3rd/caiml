using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using AIMLbot.AIMLTagHandlers;
using AIMLbot.Normalize;
using AIMLbot.Utils;

namespace AIMLbot;

public class Bot
{
	public delegate void LogMessageDelegate();

	public SettingsDictionary GlobalSettings;

	public SettingsDictionary GenderSubstitutions;

	public SettingsDictionary Person2Substitutions;

	public SettingsDictionary PersonSubstitutions;

	public SettingsDictionary Substitutions;

	public SettingsDictionary DefaultPredicates;

	private Dictionary<string, TagHandler> CustomTags;

	private Dictionary<string, Assembly> LateBindingAssemblies = new Dictionary<string, Assembly>();

	public List<string> Splitters = new List<string>();

	private List<string> LogBuffer = new List<string>();

	public bool isAcceptingUserInput = true;

	public DateTime StartedOn = DateTime.Now;

	public int Size;

	public Node Graphmaster;

	public bool TrustAIML = true;

	public int MaxThatSize = 256;

	public string LastLogMessage = string.Empty;

	private int MaxLogBufferSize => Convert.ToInt32(GlobalSettings.grabSetting("maxlogbuffersize"));

	private string NotAcceptingUserInputMessage => GlobalSettings.grabSetting("notacceptinguserinputmessage");

	public double TimeOut => Convert.ToDouble(GlobalSettings.grabSetting("timeout"));

	public string TimeOutMessage => GlobalSettings.grabSetting("timeoutmessage");

	public CultureInfo Locale => new CultureInfo(GlobalSettings.grabSetting("culture"));

	public Regex Strippers => new Regex(GlobalSettings.grabSetting("stripperregex"), RegexOptions.IgnorePatternWhitespace);

	public string AdminEmail
	{
		get
		{
			return GlobalSettings.grabSetting("adminemail");
		}
		set
		{
			if (value.Length > 0)
			{
				string pattern = "^(([^<>()[\\]\\\\.,;:\\s@\\\"]+(\\.[^<>()[\\]\\\\.,;:\\s@\\\"]+)*)|(\\\".+\\\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$";
				Regex regex = new Regex(pattern);
				if (!regex.IsMatch(value))
				{
					throw new Exception("The AdminEmail is not a valid email address");
				}
				GlobalSettings.addSetting("adminemail", value);
			}
			else
			{
				GlobalSettings.addSetting("adminemail", "");
			}
		}
	}

	public bool IsLogging
	{
		get
		{
			string text = GlobalSettings.grabSetting("islogging");
			if (text.ToLower() == "true")
			{
				return true;
			}
			return false;
		}
	}

	public bool WillCallHome
	{
		get
		{
			string text = GlobalSettings.grabSetting("willcallhome");
			if (text.ToLower() == "true")
			{
				return true;
			}
			return false;
		}
	}

	public Gender Sex => Convert.ToInt32(GlobalSettings.grabSetting("gender")) switch
	{
		-1 => Gender.Unknown, 
		0 => Gender.Female, 
		1 => Gender.Male, 
		_ => Gender.Unknown, 
	};

	public string PathToAIML => Path.Combine(Environment.CurrentDirectory, GlobalSettings.grabSetting("aimldirectory"));

	public string PathToConfigFiles => Path.Combine(Environment.CurrentDirectory, GlobalSettings.grabSetting("configdirectory"));

	public string PathToLogs => Path.Combine(Environment.CurrentDirectory, GlobalSettings.grabSetting("logdirectory"));

	public event LogMessageDelegate WrittenToLog;

	public Bot()
	{
		setup();
	}

	public void loadAIMLFromFiles()
	{
		AIMLLoader aIMLLoader = new AIMLLoader(this);
		aIMLLoader.loadAIML();
	}

	public void loadAIMLFromXML(XmlDocument newAIML, string filename)
	{
		AIMLLoader aIMLLoader = new AIMLLoader(this);
		aIMLLoader.loadAIMLFromXML(newAIML, filename);
	}

	private void setup()
	{
		GlobalSettings = new SettingsDictionary(this);
		GenderSubstitutions = new SettingsDictionary(this);
		Person2Substitutions = new SettingsDictionary(this);
		PersonSubstitutions = new SettingsDictionary(this);
		Substitutions = new SettingsDictionary(this);
		DefaultPredicates = new SettingsDictionary(this);
		CustomTags = new Dictionary<string, TagHandler>();
		Graphmaster = new Node();
	}

	public void loadSettings()
	{
		string pathToSettings = Path.Combine(Environment.CurrentDirectory, Path.Combine("config", "Settings.xml"));
		loadSettings(pathToSettings);
	}

	public void loadSettings(string pathToSettings)
	{
		GlobalSettings.loadSettings(pathToSettings);
		if (!GlobalSettings.containsSettingCalled("version"))
		{
			GlobalSettings.addSetting("version", Environment.Version.ToString());
		}
		if (!GlobalSettings.containsSettingCalled("name"))
		{
			GlobalSettings.addSetting("name", "Unknown");
		}
		if (!GlobalSettings.containsSettingCalled("botmaster"))
		{
			GlobalSettings.addSetting("botmaster", "Unknown");
		}
		if (!GlobalSettings.containsSettingCalled("master"))
		{
			GlobalSettings.addSetting("botmaster", "Unknown");
		}
		if (!GlobalSettings.containsSettingCalled("author"))
		{
			GlobalSettings.addSetting("author", "Nicholas H.Tollervey");
		}
		if (!GlobalSettings.containsSettingCalled("location"))
		{
			GlobalSettings.addSetting("location", "Unknown");
		}
		if (!GlobalSettings.containsSettingCalled("gender"))
		{
			GlobalSettings.addSetting("gender", "-1");
		}
		if (!GlobalSettings.containsSettingCalled("birthday"))
		{
			GlobalSettings.addSetting("birthday", "2006/11/08");
		}
		if (!GlobalSettings.containsSettingCalled("birthplace"))
		{
			GlobalSettings.addSetting("birthplace", "Towcester, Northamptonshire, UK");
		}
		if (!GlobalSettings.containsSettingCalled("website"))
		{
			GlobalSettings.addSetting("website", "http://unitedwild.com");
		}
		if (GlobalSettings.containsSettingCalled("adminemail"))
		{
			string adminEmail = GlobalSettings.grabSetting("adminemail");
			AdminEmail = adminEmail;
		}
		else
		{
			GlobalSettings.addSetting("adminemail", "");
		}
		if (!GlobalSettings.containsSettingCalled("islogging"))
		{
			GlobalSettings.addSetting("islogging", "False");
		}
		if (!GlobalSettings.containsSettingCalled("willcallhome"))
		{
			GlobalSettings.addSetting("willcallhome", "False");
		}
		if (!GlobalSettings.containsSettingCalled("timeout"))
		{
			GlobalSettings.addSetting("timeout", "2000");
		}
		if (!GlobalSettings.containsSettingCalled("timeoutmessage"))
		{
			GlobalSettings.addSetting("timeoutmessage", "ERROR: The request has timed out.");
		}
		if (!GlobalSettings.containsSettingCalled("culture"))
		{
			GlobalSettings.addSetting("culture", "en-US");
		}
		if (!GlobalSettings.containsSettingCalled("splittersfile"))
		{
			GlobalSettings.addSetting("splittersfile", "Splitters.xml");
		}
		if (!GlobalSettings.containsSettingCalled("person2substitutionsfile"))
		{
			GlobalSettings.addSetting("person2substitutionsfile", "Person2Substitutions.xml");
		}
		if (!GlobalSettings.containsSettingCalled("personsubstitutionsfile"))
		{
			GlobalSettings.addSetting("personsubstitutionsfile", "PersonSubstitutions.xml");
		}
		if (!GlobalSettings.containsSettingCalled("gendersubstitutionsfile"))
		{
			GlobalSettings.addSetting("gendersubstitutionsfile", "GenderSubstitutions.xml");
		}
		if (!GlobalSettings.containsSettingCalled("defaultpredicates"))
		{
			GlobalSettings.addSetting("defaultpredicates", "DefaultPredicates.xml");
		}
		if (!GlobalSettings.containsSettingCalled("substitutionsfile"))
		{
			GlobalSettings.addSetting("substitutionsfile", "Substitutions.xml");
		}
		if (!GlobalSettings.containsSettingCalled("aimldirectory"))
		{
			GlobalSettings.addSetting("aimldirectory", "aiml");
		}
		if (!GlobalSettings.containsSettingCalled("configdirectory"))
		{
			GlobalSettings.addSetting("configdirectory", "config");
		}
		if (!GlobalSettings.containsSettingCalled("logdirectory"))
		{
			GlobalSettings.addSetting("logdirectory", "logs");
		}
		if (!GlobalSettings.containsSettingCalled("maxlogbuffersize"))
		{
			GlobalSettings.addSetting("maxlogbuffersize", "64");
		}
		if (!GlobalSettings.containsSettingCalled("notacceptinguserinputmessage"))
		{
			GlobalSettings.addSetting("notacceptinguserinputmessage", "This bot is currently set to not accept user input.");
		}
		if (!GlobalSettings.containsSettingCalled("stripperregex"))
		{
			GlobalSettings.addSetting("stripperregex", "[^0-9a-zA-Z]");
		}
		Person2Substitutions.loadSettings(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("person2substitutionsfile")));
		PersonSubstitutions.loadSettings(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("personsubstitutionsfile")));
		GenderSubstitutions.loadSettings(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("gendersubstitutionsfile")));
		DefaultPredicates.loadSettings(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("defaultpredicates")));
		Substitutions.loadSettings(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("substitutionsfile")));
		loadSplitters(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("splittersfile")));
	}

	private void loadSplitters(string pathToSplitters)
	{
		FileInfo fileInfo = new FileInfo(pathToSplitters);
		if (fileInfo.Exists)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(pathToSplitters);
			if (xmlDocument.ChildNodes.Count == 2 && xmlDocument.LastChild.HasChildNodes)
			{
				foreach (XmlNode childNode in xmlDocument.LastChild.ChildNodes)
				{
					if ((childNode.Name == "item") & (childNode.Attributes.Count == 1))
					{
						string value = childNode.Attributes["value"].Value;
						Splitters.Add(value);
					}
				}
			}
		}
		if (Splitters.Count == 0)
		{
			Splitters.Add(".");
			Splitters.Add("!");
			Splitters.Add("?");
			Splitters.Add(";");
		}
	}

	public void writeToLog(string message)
	{
		LastLogMessage = message;
		if (IsLogging)
		{
			LogBuffer.Add(DateTime.Now.ToString() + ": " + message + Environment.NewLine);
			if (LogBuffer.Count > MaxLogBufferSize - 1)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(PathToLogs);
				if (!directoryInfo.Exists)
				{
					directoryInfo.Create();
				}
				string path = DateTime.Now.ToString("yyyyMMdd") + ".log";
				FileInfo fileInfo = new FileInfo(Path.Combine(PathToLogs, path));
				StreamWriter streamWriter = (fileInfo.Exists ? fileInfo.AppendText() : fileInfo.CreateText());
				foreach (string item in LogBuffer)
				{
					streamWriter.WriteLine(item);
				}
				streamWriter.Close();
				LogBuffer.Clear();
			}
		}
		if (!object.Equals(null, this.WrittenToLog))
		{
			this.WrittenToLog();
		}
	}

	public Result Chat(string rawInput, string UserGUID)
	{
		Request request = new Request(rawInput, new User(UserGUID, this), this);
		return Chat(request);
	}

	public Result Chat(Request request)
	{
		Result result = new Result(request.user, this, request);
		if (isAcceptingUserInput)
		{
			AIMLLoader aIMLLoader = new AIMLLoader(this);
			SplitIntoSentences splitIntoSentences = new SplitIntoSentences(this);
			string[] array = splitIntoSentences.Transform(request.rawInput);
			string[] array2 = array;
			foreach (string text in array2)
			{
				result.InputSentences.Add(text);
				string item = aIMLLoader.generatePath(text, request.user.getLastBotOutput(), request.user.Topic, isUserInput: true);
				result.NormalizedPaths.Add(item);
			}
			foreach (string normalizedPath in result.NormalizedPaths)
			{
				SubQuery subQuery = new SubQuery(normalizedPath);
				subQuery.Template = Graphmaster.evaluate(normalizedPath, subQuery, request, MatchState.UserInput, new StringBuilder());
				result.SubQueries.Add(subQuery);
			}
			foreach (SubQuery subQuery2 in result.SubQueries)
			{
				if (subQuery2.Template.Length <= 0)
				{
					continue;
				}
				try
				{
					XmlNode node = AIMLTagHandler.getNode(subQuery2.Template);
					string text2 = processNode(node, subQuery2, request, result, request.user);
					if (text2.Length > 0)
					{
						result.OutputSentences.Add(text2);
					}
				}
				catch (Exception ex)
				{
					if (WillCallHome)
					{
						phoneHome(ex.Message, request);
					}
					writeToLog("WARNING! A problem was encountered when trying to process the input: " + request.rawInput + " with the template: \"" + subQuery2.Template + "\"");
				}
			}
		}
		else
		{
			result.OutputSentences.Add(NotAcceptingUserInputMessage);
		}
		result.Duration = DateTime.Now - request.StartedOn;
		request.user.addResult(result);
		return result;
	}

	private string processNode(XmlNode node, SubQuery query, Request request, Result result, User user)
	{
		if (request.StartedOn.AddMilliseconds(request.bot.TimeOut) < DateTime.Now)
		{
			request.bot.writeToLog("WARNING! Request timeout. User: " + request.user.UserID + " raw input: \"" + request.rawInput + "\" processing template: \"" + query.Template + "\"");
			request.hasTimedOut = true;
			return string.Empty;
		}
		string text = node.Name.ToLower();
		if (text == "template")
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (node.HasChildNodes)
			{
				foreach (XmlNode childNode in node.ChildNodes)
				{
					stringBuilder.Append(processNode(childNode, query, request, result, user));
				}
			}
			return stringBuilder.ToString();
		}
		AIMLTagHandler aIMLTagHandler = null;
		aIMLTagHandler = getBespokeTags(user, query, request, result, node);
		if (object.Equals(null, aIMLTagHandler))
		{
			aIMLTagHandler = text switch
			{
				"bot" => new bot(this, user, query, request, result, node), 
				"condition" => new condition(this, user, query, request, result, node), 
				"date" => new date(this, user, query, request, result, node), 
				"formal" => new formal(this, user, query, request, result, node), 
				"gender" => new gender(this, user, query, request, result, node), 
				"get" => new get(this, user, query, request, result, node), 
				"gossip" => new gossip(this, user, query, request, result, node), 
				"id" => new id(this, user, query, request, result, node), 
				"input" => new input(this, user, query, request, result, node), 
				"javascript" => new javascript(this, user, query, request, result, node), 
				"learn" => new learn(this, user, query, request, result, node), 
				"lowercase" => new lowercase(this, user, query, request, result, node), 
				"person" => new person(this, user, query, request, result, node), 
				"person2" => new person2(this, user, query, request, result, node), 
				"random" => new random(this, user, query, request, result, node), 
				"sentence" => new sentence(this, user, query, request, result, node), 
				"set" => new set(this, user, query, request, result, node), 
				"size" => new size(this, user, query, request, result, node), 
				"sr" => new sr(this, user, query, request, result, node), 
				"srai" => new srai(this, user, query, request, result, node), 
				"star" => new star(this, user, query, request, result, node), 
				"system" => new system(this, user, query, request, result, node), 
				"that" => new that(this, user, query, request, result, node), 
				"thatstar" => new thatstar(this, user, query, request, result, node), 
				"think" => new think(this, user, query, request, result, node), 
				"topicstar" => new topicstar(this, user, query, request, result, node), 
				"uppercase" => new uppercase(this, user, query, request, result, node), 
				"version" => new version(this, user, query, request, result, node), 
				_ => null, 
			};
		}
		if (object.Equals(null, aIMLTagHandler))
		{
			return node.InnerText;
		}
		if (aIMLTagHandler.isRecursive)
		{
			if (node.HasChildNodes)
			{
				foreach (XmlNode childNode2 in node.ChildNodes)
				{
					if (childNode2.NodeType != XmlNodeType.Text)
					{
						childNode2.InnerXml = processNode(childNode2, query, request, result, user);
					}
				}
			}
			return aIMLTagHandler.Transform();
		}
		string text2 = aIMLTagHandler.Transform();
		XmlNode node3 = AIMLTagHandler.getNode("<node>" + text2 + "</node>");
		if (node3.HasChildNodes)
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (XmlNode childNode3 in node3.ChildNodes)
			{
				stringBuilder2.Append(processNode(childNode3, query, request, result, user));
			}
			return stringBuilder2.ToString();
		}
		return node3.InnerXml;
	}

	public AIMLTagHandler getBespokeTags(User user, SubQuery query, Request request, Result result, XmlNode node)
	{
		if (CustomTags.ContainsKey(node.Name.ToLower()))
		{
			TagHandler tagHandler = CustomTags[node.Name.ToLower()];
			AIMLTagHandler aIMLTagHandler = tagHandler.Instantiate(LateBindingAssemblies);
			if (object.Equals(null, aIMLTagHandler))
			{
				return null;
			}
			aIMLTagHandler.user = user;
			aIMLTagHandler.query = query;
			aIMLTagHandler.request = request;
			aIMLTagHandler.result = result;
			aIMLTagHandler.templateNode = node;
			aIMLTagHandler.bot = this;
			return aIMLTagHandler;
		}
		return null;
	}

	public void saveToBinaryFile(string path)
	{
		FileInfo fileInfo = new FileInfo(path);
		if (fileInfo.Exists)
		{
			fileInfo.Delete();
		}
		FileStream fileStream = File.Create(path);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		binaryFormatter.Serialize(fileStream, Graphmaster);
		fileStream.Close();
	}

	public void loadFromBinaryFile(string path)
	{
		FileStream fileStream = File.OpenRead(path);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		Graphmaster = (Node)binaryFormatter.Deserialize(fileStream);
		fileStream.Close();
	}

	public void loadCustomTagHandlers(string pathToDLL)
	{
		Assembly assembly = Assembly.LoadFrom(pathToDLL);
		Type[] types = assembly.GetTypes();
		for (int i = 0; i < types.Length; i++)
		{
			object[] customAttributes = types[i].GetCustomAttributes(inherit: false);
			for (int j = 0; j < customAttributes.Length; j++)
			{
				if (customAttributes[j] is CustomTagAttribute)
				{
					if (!LateBindingAssemblies.ContainsKey(assembly.FullName))
					{
						LateBindingAssemblies.Add(assembly.FullName, assembly);
					}
					TagHandler tagHandler = new TagHandler();
					tagHandler.AssemblyName = assembly.FullName;
					tagHandler.ClassName = types[i].FullName;
					tagHandler.TagName = types[i].Name.ToLower();
					if (CustomTags.ContainsKey(tagHandler.TagName))
					{
						throw new Exception("ERROR! Unable to add the custom tag: <" + tagHandler.TagName + ">, found in: " + pathToDLL + " as a handler for this tag already exists.");
					}
					CustomTags.Add(tagHandler.TagName, tagHandler);
				}
			}
		}
	}

	public void phoneHome(string errorMessage, Request request)
	{
		MailMessage mailMessage = new MailMessage("donotreply@aimlbot.com", AdminEmail);
		mailMessage.Subject = "WARNING! AIMLBot has encountered a problem...";
		string text = "Dear Botmaster,\r\n\r\nThis is an automatically generated email to report errors with your bot.\r\n\r\nAt *TIME* the bot encountered the following error:\r\n\r\n\"*MESSAGE*\"\r\n\r\nwhilst processing the following input:\r\n\r\n\"*RAWINPUT*\"\r\n\r\nfrom the user with an id of: *USER*\r\n\r\nThe normalized paths generated by the raw input were as follows:\r\n\r\n*PATHS*\r\n\r\nPlease check your AIML!\r\n\r\nRegards,\r\n\r\nThe AIMLbot program.\r\n";
		text = text.Replace("*TIME*", DateTime.Now.ToString());
		text = text.Replace("*MESSAGE*", errorMessage);
		text = text.Replace("*RAWINPUT*", request.rawInput);
		text = text.Replace("*USER*", request.user.UserID);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string normalizedPath in request.result.NormalizedPaths)
		{
			stringBuilder.Append(normalizedPath + Environment.NewLine);
		}
		text = text.Replace("*PATHS*", stringBuilder.ToString());
		mailMessage.Body = text;
		mailMessage.IsBodyHtml = false;
		try
		{
			if (mailMessage.To.Count > 0)
			{
				SmtpClient smtpClient = new SmtpClient();
				smtpClient.Send(mailMessage);
			}
		}
		catch
		{
		}
	}
}

