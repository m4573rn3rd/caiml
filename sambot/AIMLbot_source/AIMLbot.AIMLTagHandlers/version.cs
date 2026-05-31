using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class version : AIMLTagHandler
{
	public version(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "version")
		{
			return bot.GlobalSettings.grabSetting("version");
		}
		return string.Empty;
	}
}
