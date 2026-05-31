using System.Text.RegularExpressions;
using System.Xml;
using AIMLbot.Utils;

namespace AIMLbot.AIMLTagHandlers;

public class condition : AIMLTagHandler
{
	public condition(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, user, query, request, result, templateNode)
	{
		isRecursive = false;
	}

	protected override string ProcessChange()
	{
		if (templateNode.Name.ToLower() == "condition")
		{
			if (templateNode.Attributes.Count == 2)
			{
				string text = "";
				string text2 = "";
				if (templateNode.Attributes[0].Name == "name")
				{
					text = templateNode.Attributes[0].Value;
				}
				else if (templateNode.Attributes[0].Name == "value")
				{
					text2 = templateNode.Attributes[0].Value;
				}
				if (templateNode.Attributes[1].Name == "name")
				{
					text = templateNode.Attributes[1].Value;
				}
				else if (templateNode.Attributes[1].Name == "value")
				{
					text2 = templateNode.Attributes[1].Value;
				}
				if ((text.Length > 0) & (text2.Length > 0))
				{
					string text3 = user.Predicates.grabSetting(text);
					Regex regex = new Regex(text2.Replace(" ", "\\s").Replace("*", "[\\sA-Z0-9]+"), RegexOptions.IgnoreCase);
					if (regex.IsMatch(text3))
					{
						return templateNode.InnerXml;
					}
				}
			}
			else if (templateNode.Attributes.Count == 1)
			{
				if (templateNode.Attributes[0].Name == "name")
				{
					string text = templateNode.Attributes[0].Value;
					foreach (XmlNode childNode in templateNode.ChildNodes)
					{
						if (!(childNode.Name.ToLower() == "li"))
						{
							continue;
						}
						if (childNode.Attributes.Count == 1)
						{
							if (childNode.Attributes[0].Name.ToLower() == "value")
							{
								string text3 = user.Predicates.grabSetting(text);
								Regex regex = new Regex(childNode.Attributes[0].Value.Replace(" ", "\\s").Replace("*", "[\\sA-Z0-9]+"), RegexOptions.IgnoreCase);
								if (regex.IsMatch(text3))
								{
									return childNode.InnerXml;
								}
							}
						}
						else if (childNode.Attributes.Count == 0)
						{
							return childNode.InnerXml;
						}
					}
				}
			}
			else if (templateNode.Attributes.Count == 0)
			{
				foreach (XmlNode childNode2 in templateNode.ChildNodes)
				{
					if (!(childNode2.Name.ToLower() == "li"))
					{
						continue;
					}
					if (childNode2.Attributes.Count == 2)
					{
						string text = "";
						string text2 = "";
						if (childNode2.Attributes[0].Name == "name")
						{
							text = childNode2.Attributes[0].Value;
						}
						else if (childNode2.Attributes[0].Name == "value")
						{
							text2 = childNode2.Attributes[0].Value;
						}
						if (childNode2.Attributes[1].Name == "name")
						{
							text = childNode2.Attributes[1].Value;
						}
						else if (childNode2.Attributes[1].Name == "value")
						{
							text2 = childNode2.Attributes[1].Value;
						}
						if ((text.Length > 0) & (text2.Length > 0))
						{
							string text3 = user.Predicates.grabSetting(text);
							Regex regex = new Regex(text2.Replace(" ", "\\s").Replace("*", "[\\sA-Z0-9]+"), RegexOptions.IgnoreCase);
							if (regex.IsMatch(text3))
							{
								return childNode2.InnerXml;
							}
						}
					}
					else if (childNode2.Attributes.Count == 0)
					{
						return childNode2.InnerXml;
					}
				}
			}
		}
		return string.Empty;
	}
}
