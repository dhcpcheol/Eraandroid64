namespace MinorShift.Emuera.Sub;

internal sealed class NullWord : Word
{
	public override char Type => '\0';

	public override string ToString()
	{
		return "/null/";
	}
}
