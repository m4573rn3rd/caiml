using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class srai : AIMLTagHandler
{
	public srai(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "srai" && templateNode.InnerText.Length > 0)
		{
			Request request = new Request(templateNode.InnerText, user, bot);
			request.StartedOn = base.request.StartedOn;
			Result result = bot.Chat(request);
			base.request.hasTimedOut = request.hasTimedOut;
			return result.Output;
		}
		return string.Empty;
	}
}
