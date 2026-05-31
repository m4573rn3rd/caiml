using System;
using System.Collections.Generic;

namespace AIMLbot.Normalize;

public class SplitIntoSentences
{
	private Bot bot;

	private string inputString;

	public SplitIntoSentences(Bot bot, string inputString)
	{
		this.bot = bot;
		this.inputString = inputString;
	}

	public SplitIntoSentences(Bot bot)
	{
		this.bot = bot;
	}

	public string[] Transform(string inputString)
	{
		this.inputString = inputString;
		return Transform();
	}

	public string[] Transform()
	{
		string[] separator = bot.Splitters.ToArray();
		string[] array = inputString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		List<string> list = new List<string>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			string text2 = text.Trim();
			if (text2.Length > 0)
			{
				list.Add(text2);
			}
		}
		return list.ToArray();
	}
}
