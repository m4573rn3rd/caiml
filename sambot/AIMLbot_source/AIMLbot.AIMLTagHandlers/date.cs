using System;
using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class date : AIMLTagHandler
{
	public date(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "date")
		{
			return DateTime.Now.ToString(bot.Locale);
		}
		return string.Empty;
	}
}
