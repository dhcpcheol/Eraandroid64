namespace MinorShift.Emuera.Sub;

internal sealed class MacroWord : Word
{
	private readonly int num;

	public int Number => num;

	public override char Type => 'M';

	public MacroWord(int num)
	{
		this.num = num;
	}

	public override string ToString()
	{
		return "Arg" + num;
	}
}
