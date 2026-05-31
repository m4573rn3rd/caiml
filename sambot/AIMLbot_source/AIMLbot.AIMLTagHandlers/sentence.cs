using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class sentence : AIMLTagHandler
{
	public sentence(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "sentence")
		{
			if (templateNode.InnerText.Length > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				char[] array = templateNode.InnerText.Trim().ToCharArray();
				bool flag = true;
				for (int i = 0; i < array.Length; i++)
				{
					string text = Convert.ToString(array[i]);
					if (bot.Splitters.Contains(text))
					{
						flag = true;
					}
					Regex regex = new Regex("[a-zA-Z]");
					if (regex.IsMatch(text))
					{
						if (flag)
						{
							stringBuilder.Append(text.ToUpper(bot.Locale));
							flag = false;
						}
						else
						{
							stringBuilder.Append(text.ToLower(bot.Locale));
						}
					}
					else
					{
						stringBuilder.Append(text);
					}
				}
				return stringBuilder.ToString();
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
