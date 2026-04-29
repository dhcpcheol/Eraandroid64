namespace MinorShift.Emuera.Sub;

internal sealed class TripleSymbolSubWord : SubWord
{
	private readonly char code;

	public char Code => code;

	public TripleSymbolSubWord(char c)
		: base(null)
	{
		code = c;
	}
}
