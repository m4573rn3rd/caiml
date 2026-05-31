using AIMLbot.Utils;

namespace AIMLbot.Normalize;

public class MakeCaseInsensitive : TextTransformer
{
	public MakeCaseInsensitive(Bot bot, string inputString)
		: base(bot, inputString)
	{
	}

	public MakeCaseInsensitive(Bot bot)
		: base(bot)
	{
	}

	protected override string ProcessChange()
	{
		return inputString.ToUpper();
	}

	public static string TransformInput(string input)
	{
		return input.ToUpper();
	}
}
