using System;
using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class star : AIMLTagHandler
{
	public star(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "star")
		{
			if (query.InputStar.Count > 0)
			{
				if (templateNode.Attributes.Count == 0)
				{
					return query.InputStar[0];
				}
				if (templateNode.Attributes.Count == 1 && templateNode.Attributes[0].Name.ToLower() == "index")
				{
					try
					{
						int num = Convert.ToInt32(templateNode.Attributes[0].Value);
						num--;
						if ((num >= 0) & (num < query.InputStar.Count))
						{
							return query.InputStar[num];
						}
						bot.writeToLog("InputStar out of bounds reference caused by input: " + request.rawInput);
					}
					catch
					{
						bot.writeToLog("Index set to non-integer value whilst processing star tag in response to the input: " + request.rawInput);
					}
				}
			}
			else
			{
				bot.writeToLog("A star tag tried to reference an empty InputStar collection when processing the input: " + request.rawInput);
			}
		}
		return string.Empty;
	}
}
