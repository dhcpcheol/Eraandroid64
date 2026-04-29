namespace MinorShift.Emuera.Sub;

internal sealed class LiteralStringWord : Word
{
	private readonly string code;

	public string Str => code;

	public override char Type => '"';

	public LiteralStringWord(string s)
	{
		code = s;
	}

	public override string ToString()
	{
		return "\"" + code + "\"";
	}
}
