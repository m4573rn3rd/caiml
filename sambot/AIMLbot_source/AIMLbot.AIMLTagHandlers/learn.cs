using System.IO;
using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class learn : AIMLTagHandler
{
	public learn(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "learn" && templateNode.InnerText.Length > 0)
		{
			string innerText = templateNode.InnerText;
			FileInfo fileInfo = new FileInfo(innerText);
			if (fileInfo.Exists)
			{
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					xmlDocument.Load(innerText);
					bot.loadAIMLFromXML(xmlDocument, innerText);
				}
				catch
				{
					bot.writeToLog("ERROR! Attempted (but failed) to <learn> some new AIML from the following URI: " + innerText);
				}
			}
		}
		return string.Empty;
	}
}
