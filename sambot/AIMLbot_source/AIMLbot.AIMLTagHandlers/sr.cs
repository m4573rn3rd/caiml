using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class sr : AIMLTagHandler
{
	public sr(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "sr")
		{
			XmlNode node = AIMLTagHandler.getNode("<star/>");
			star star2 = new star(bot, user, query, request, result, node);
			string text = star2.Transform();
			XmlNode node2 = AIMLTagHandler.getNode("<srai>" + text + "</srai>");
			srai srai2 = new srai(bot, user, query, request, result, node2);
			return srai2.Transform();
		}
		return string.Empty;
	}
}
