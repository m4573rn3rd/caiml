using System;
using System.Text;
using System.Text.RegularExpressions;
using AIMLbot.Utils;

namespace AIMLbot.Normalize;

public class ApplySubstitutions : TextTransformer
{
	public ApplySubstitutions(Bot bot, string inputString)
		: base(bot, inputString)
	{
	}

	public ApplySubstitutions(Bot bot)
		: base(bot)
	{
	}

	private static string getMarker(int len)
	{
		char[] array = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
		StringBuilder stringBuilder = new StringBuilder();
		Random random = new Random();
		for (int i = 0; i < len; i++)
		{
			stringBuilder.Append(array[random.Next(array.Length)]);
		}
		return stringBuilder.ToString();
	}

	protected override string ProcessChange()
	{
		return Substitute(bot, bot.Substitutions, inputString);
	}

	public static string Substitute(Bot bot, SettingsDictionary dictionary, string target)
	{
		string marker = getMarker(5);
		string text = target;
		string[] settingNames = dictionary.SettingNames;
		foreach (string text2 in settingNames)
		{
			string text3 = makeRegexSafe(text2);
			string pattern = "\\b" + text3.TrimEnd().TrimStart() + "\\b";
			string replacement = marker + dictionary.grabSetting(text2).Trim() + marker;
			text = Regex.Replace(text, pattern, replacement, RegexOptions.IgnoreCase);
		}
		return text.Replace(marker, "");
	}

	private static string makeRegexSafe(string input)
	{
		string text = input.Replace("\\", "");
		text = text.Replace(")", "\\)");
		text = text.Replace("(", "\\(");
		return text.Replace(".", "\\.");
	}
}
