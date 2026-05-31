using System.Text;
using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class formal : AIMLTagHandler
{
	public formal(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "formal")
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (templateNode.InnerText.Length > 0)
			{
				string[] array = templateNode.InnerText.ToLower().Split();
				string[] array2 = array;
				foreach (string text in array2)
				{
					string text2 = text.Substring(0, 1);
					text2 = text2.ToUpper();
					if (text.Length > 1)
					{
						text2 += text.Substring(1);
					}
					stringBuilder.Append(text2 + " ");
				}
			}
			return stringBuilder.ToString().Trim();
		}
		return string.Empty;
	}
}
