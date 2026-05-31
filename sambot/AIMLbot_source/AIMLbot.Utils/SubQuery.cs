using System.Collections.Generic;

namespace AIMLbot.Utils;

public class SubQuery
{
	public string FullPath;

	public string Template = string.Empty;

	public List<string> InputStar = new List<string>();

	public List<string> ThatStar = new List<string>();

	public List<string> TopicStar = new List<string>();

	public SubQuery(string fullPath)
	{
		FullPath = fullPath;
	}
}
