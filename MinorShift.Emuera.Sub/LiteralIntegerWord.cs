namespace MinorShift.Emuera.Sub;

internal sealed class LiteralIntegerWord : Word
{
	private readonly long code;

	public long Int => code;

	public override char Type => '0';

	public LiteralIntegerWord(long i)
	{
		code = i;
	}

	public override string ToString()
	{
		return code.ToString();
	}
}
