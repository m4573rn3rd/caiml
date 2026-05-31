using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using AIMLbot.Normalize;

namespace AIMLbot.Utils;

[Serializable]
public class Node
{
	private Dictionary<string, Node> children = new Dictionary<string, Node>();

	public string template = string.Empty;

	public string filename = string.Empty;

	public string word = string.Empty;

	public int NumberOfChildNodes => children.Count;

	public void addCategory(string path, string template, string filename)
	{
		if (template.Length == 0)
		{
			throw new XmlException("The category with a pattern: " + path + " found in file: " + filename + " has an empty template tag. ABORTING");
		}
		if (path.Trim().Length == 0)
		{
			this.template = template;
			this.filename = filename;
			return;
		}
		string[] array = path.Trim().Split(" ".ToCharArray());
		string text = MakeCaseInsensitive.TransformInput(array[0]);
		string path2 = path.Substring(text.Length, path.Length - text.Length).Trim();
		Node node;
		if (children.ContainsKey(text))
		{
			node = children[text];
			node.addCategory(path2, template, filename);
			return;
		}
		node = new Node();
		node.word = text;
		node.addCategory(path2, template, filename);
		children.Add(node.word, node);
	}

	public string evaluate(string path, SubQuery query, Request request, MatchState matchstate, StringBuilder wildcard)
	{
		if (request.StartedOn.AddMilliseconds(request.bot.TimeOut) < DateTime.Now)
		{
			request.bot.writeToLog("WARNING! Request timeout. User: " + request.user.UserID + " raw input: \"" + request.rawInput + "\"");
			request.hasTimedOut = true;
			return string.Empty;
		}
		path = path.Trim();
		if (children.Count == 0)
		{
			if (path.Length > 0)
			{
				storeWildCard(path, wildcard);
			}
			return template;
		}
		if (path.Length == 0)
		{
			return template;
		}
		string[] array = path.Split(" \r\n\t".ToCharArray());
		string text = MakeCaseInsensitive.TransformInput(array[0]);
		string path2 = path.Substring(text.Length, path.Length - text.Length);
		if (children.ContainsKey("_"))
		{
			Node node = children["_"];
			StringBuilder stringBuilder = new StringBuilder();
			storeWildCard(array[0], stringBuilder);
			string text2 = node.evaluate(path2, query, request, matchstate, stringBuilder);
			if (text2.Length > 0)
			{
				if (stringBuilder.Length > 0)
				{
					switch (matchstate)
					{
					case MatchState.UserInput:
						query.InputStar.Add(stringBuilder.ToString());
						stringBuilder.Remove(0, stringBuilder.Length);
						break;
					case MatchState.That:
						query.ThatStar.Add(stringBuilder.ToString());
						break;
					case MatchState.Topic:
						query.TopicStar.Add(stringBuilder.ToString());
						break;
					}
				}
				return text2;
			}
		}
		if (children.ContainsKey(text))
		{
			MatchState matchstate2 = matchstate;
			if (text == "<THAT>")
			{
				matchstate2 = MatchState.That;
			}
			else if (text == "<TOPIC>")
			{
				matchstate2 = MatchState.Topic;
			}
			Node node = children[text];
			StringBuilder stringBuilder = new StringBuilder();
			string text2 = node.evaluate(path2, query, request, matchstate2, stringBuilder);
			if (text2.Length > 0)
			{
				if (stringBuilder.Length > 0)
				{
					switch (matchstate)
					{
					case MatchState.UserInput:
						query.InputStar.Add(stringBuilder.ToString());
						stringBuilder.Remove(0, stringBuilder.Length);
						break;
					case MatchState.That:
						query.ThatStar.Add(stringBuilder.ToString());
						stringBuilder.Remove(0, stringBuilder.Length);
						break;
					case MatchState.Topic:
						query.TopicStar.Add(stringBuilder.ToString());
						stringBuilder.Remove(0, stringBuilder.Length);
						break;
					}
				}
				return text2;
			}
		}
		if (children.ContainsKey("*"))
		{
			Node node = children["*"];
			StringBuilder stringBuilder = new StringBuilder();
			storeWildCard(array[0], stringBuilder);
			string text2 = node.evaluate(path2, query, request, matchstate, stringBuilder);
			if (text2.Length > 0)
			{
				if (stringBuilder.Length > 0)
				{
					switch (matchstate)
					{
					case MatchState.UserInput:
						query.InputStar.Add(stringBuilder.ToString());
						stringBuilder.Remove(0, stringBuilder.Length);
						break;
					case MatchState.That:
						query.ThatStar.Add(stringBuilder.ToString());
						break;
					case MatchState.Topic:
						query.TopicStar.Add(stringBuilder.ToString());
						break;
					}
				}
				return text2;
			}
		}
		if (word == "_" || word == "*")
		{
			storeWildCard(array[0], wildcard);
			return evaluate(path2, query, request, matchstate, wildcard);
		}
		wildcard = new StringBuilder();
		return string.Empty;
	}

	private void storeWildCard(string word, StringBuilder wildcard)
	{
		if (wildcard.Length > 0)
		{
			wildcard.Append(" ");
		}
		wildcard.Append(word);
	}
}
