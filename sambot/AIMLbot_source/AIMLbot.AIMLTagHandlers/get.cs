using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class get : AIMLTagHandler
{
	public get(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "get" && bot.GlobalSettings.Count > 0 && templateNode.Attributes.Count == 1 && templateNode.Attributes[0].Name.ToLower() == "name")
		{
			return user.Predicates.grabSetting(templateNode.Attributes[0].Value);
		}
		return string.Empty;
	}
}
