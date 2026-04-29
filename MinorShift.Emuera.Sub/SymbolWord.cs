namespace MinorShift.Emuera.Sub;

internal sealed class SymbolWord : Word
{
	private readonly char code;

	public override char Type => code;

	public SymbolWord(char c)
	{
		code = c;
	}

	public override string ToString()
	{
		return code.ToString();
	}
}
