using System;
using System.Collections.Generic;
using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class random : AIMLTagHandler
{
	public random(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
		isRecursive = false;
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "random" && templateNode.HasChildNodes)
		{
			List<XmlNode> list = new List<XmlNode>();
			foreach (XmlNode childNode in templateNode.ChildNodes)
			{
				if (childNode.Name == "li")
				{
					list.Add(childNode);
				}
			}
			if (list.Count > 0)
			{
				Random random2 = new Random();
				XmlNode xmlNode2 = list[random2.Next(list.Count)];
				return xmlNode2.InnerXml;
			}
		}
		return string.Empty;
	}
}
