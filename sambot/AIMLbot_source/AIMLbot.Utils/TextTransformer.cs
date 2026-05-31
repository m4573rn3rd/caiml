namespace AIMLbot.Utils;

public abstract class TextTransformer
{
	protected string inputString;

	public Bot bot;

	public string InputString
	{
		get
		{
			return inputString;
		}
		set
		{
			inputString = value;
		}
	}

	public string OutputString => Transform();

	public TextTransformer(Bot bot, string inputString)
	{
		this.bot = bot;
		this.inputString = inputString;
	}

	public TextTransformer(Bot bot)
	{
		this.bot = bot;
		inputString = string.Empty;
	}

	public TextTransformer()
	{
		bot = null;
		inputString = string.Empty;
	}

	public string Transform(string input)
	{
		inputString = input;
		return Transform();
	}

	public string Transform()
	{
		if (inputString.Length > 0)
		{
			return ProcessChange();
		}
		return string.Empty;
	}

	protected abstract string ProcessChange();
}
