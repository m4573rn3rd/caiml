using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class set : AIMLTagHandler
{
	public set(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "set" && bot.GlobalSettings.Count > 0 && templateNode.Attributes.Count == 1 && templateNode.Attributes[0].Name.ToLower() == "name")
		{
			if (templateNode.InnerText.Length > 0)
			{
				user.Predicates.addSetting(templateNode.Attributes[0].Value, templateNode.InnerText);
				return user.Predicates.grabSetting(templateNode.Attributes[0].Value);
			}
			user.Predicates.removeSetting(templateNode.Attributes[0].Value);
			return string.Empty;
		}
		return string.Empty;
	}
}
