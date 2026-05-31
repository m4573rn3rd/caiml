using System;
using System.Collections.Generic;
using System.Text;
using AIMLbot.Utils;

namespace AIMLbot;

public class Result
{
	public Bot bot;

	public User user;

	public Request request;

	public List<string> NormalizedPaths = new List<string>();

	public TimeSpan Duration;

	public List<SubQuery> SubQueries = new List<SubQuery>();

	public List<string> OutputSentences = new List<string>();

	public List<string> InputSentences = new List<string>();

	public string RawInput => request.rawInput;

	public string Output
	{
		get
		{
			if (OutputSentences.Count > 0)
			{
				return RawOutput;
			}
			if (request.hasTimedOut)
			{
				return bot.TimeOutMessage;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string normalizedPath in NormalizedPaths)
			{
				stringBuilder.Append(normalizedPath + Environment.NewLine);
			}
			bot.writeToLog("The bot could not find any response for the input: " + RawInput + " with the path(s): " + Environment.NewLine + stringBuilder.ToString() + " from the user with an id: " + user.UserID);
			return string.Empty;
		}
	}

	public string RawOutput
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string outputSentence in OutputSentences)
			{
				string text = outputSentence.Trim();
				if (!checkEndsAsSentence(text))
				{
					text += ".";
				}
				stringBuilder.Append(text + " ");
			}
			return stringBuilder.ToString().Trim();
		}
	}

	public Result(User user, Bot bot, Request request)
	{
		this.user = user;
		this.bot = bot;
		this.request = request;
		this.request.result = this;
	}

	public override string ToString()
	{
		return Output;
	}

	private bool checkEndsAsSentence(string sentence)
	{
		foreach (string splitter in bot.Splitters)
		{
			if (sentence.Trim().EndsWith(splitter))
			{
				return true;
			}
		}
		return false;
	}
}
