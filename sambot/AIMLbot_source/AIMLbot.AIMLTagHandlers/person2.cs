using System.Xml;
using AIMLbot.Normalize;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class person2 : AIMLTagHandler
{
	public person2(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "person2")
		{
			if (templateNode.InnerText.Length > 0)
			{
				return ApplySubstitutions.Substitute(bot, bot.Person2Substitutions, templateNode.InnerText);
			}
			XmlNode node = AIMLTagHandler.getNode("<star/>");
			star star2 = new star(bot, user, query, request, result, node);
			templateNode.InnerText = star2.Transform();
			if (templateNode.InnerText.Length > 0)
			{
				return ProcessChange();
			}
			return string.Empty;
		}
		return string.Empty;
	}
}
