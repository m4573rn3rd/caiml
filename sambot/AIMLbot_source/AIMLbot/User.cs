using System;
using System.Collections.Generic;
using AIMLbot.Utils;

namespace AIMLbot;

public class User
{
	private string id;

	public Bot bot;

	private List<Result> Results = new List<Result>();

	public SettingsDictionary Predicates;

	public string UserID => id;

	public string Topic => Predicates.grabSetting("topic");

	public Result LastResult
	{
		get
		{
			if (Results.Count > 0)
			{
				return Results[0];
			}
			return null;
		}
	}

	public User(string UserID, Bot bot)
	{
		if (UserID.Length > 0)
		{
			id = UserID;
			this.bot = bot;
			Predicates = new SettingsDictionary(this.bot);
			this.bot.DefaultPredicates.Clone(Predicates);
			Predicates.addSetting("topic", "*");
			return;
		}
		throw new Exception("The UserID cannot be empty");
	}

	public string getLastBotOutput()
	{
		if (Results.Count > 0)
		{
			return Results[0].RawOutput;
		}
		return "*";
	}

	public string getThat()
	{
		return getThat(0, 0);
	}

	public string getThat(int n)
	{
		return getThat(n, 0);
	}

	public string getThat(int n, int sentence)
	{
		if ((n >= 0) & (n < Results.Count))
		{
			Result result = Results[n];
			if ((sentence >= 0) & (sentence < result.OutputSentences.Count))
			{
				return result.OutputSentences[sentence];
			}
		}
		return string.Empty;
	}

	public string getResultSentence()
	{
		return getResultSentence(0, 0);
	}

	public string getResultSentence(int n)
	{
		return getResultSentence(n, 0);
	}

	public string getResultSentence(int n, int sentence)
	{
		if ((n >= 0) & (n < Results.Count))
		{
			Result result = Results[n];
			if ((sentence >= 0) & (sentence < result.InputSentences.Count))
			{
				return result.InputSentences[sentence];
			}
		}
		return string.Empty;
	}

	public void addResult(Result latestResult)
	{
		Results.Insert(0, latestResult);
	}
}
