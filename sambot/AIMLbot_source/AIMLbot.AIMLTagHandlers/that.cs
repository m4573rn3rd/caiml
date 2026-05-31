using System;
using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class that : AIMLTagHandler
{
	public that(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "that")
		{
			if (templateNode.Attributes.Count == 0)
			{
				return user.getThat();
			}
			if (templateNode.Attributes.Count == 1 && templateNode.Attributes[0].Name.ToLower() == "index" && templateNode.Attributes[0].Value.Length > 0)
			{
				try
				{
					string[] array = templateNode.Attributes[0].Value.Split(",".ToCharArray());
					if (array.Length == 2)
					{
						int num = Convert.ToInt32(array[0].Trim());
						int num2 = Convert.ToInt32(array[1].Trim());
						if (num > 0 && num2 > 0)
						{
							return user.getThat(num - 1, num2 - 1);
						}
						bot.writeToLog("ERROR! An input tag with a bady formed index (" + templateNode.Attributes[0].Value + ") was encountered processing the input: " + request.rawInput);
					}
					else
					{
						int num = Convert.ToInt32(templateNode.Attributes[0].Value.Trim());
						if (num > 0)
						{
							return user.getThat(num - 1);
						}
						bot.writeToLog("ERROR! An input tag with a bady formed index (" + templateNode.Attributes[0].Value + ") was encountered processing the input: " + request.rawInput);
					}
				}
				catch
				{
					bot.writeToLog("ERROR! An input tag with a bady formed index (" + templateNode.Attributes[0].Value + ") was encountered processing the input: " + request.rawInput);
				}
			}
		}
		return string.Empty;
	}
}
