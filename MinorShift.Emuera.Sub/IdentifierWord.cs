namespace MinorShift.Emuera.Sub;

internal sealed class IdentifierWord : Word
{
	private readonly string code;

	public string Code => code;

	public override char Type => 'A';

	public IdentifierWord(string s)
	{
		code = s;
	}

	public override string ToString()
	{
		return code;
	}
}
