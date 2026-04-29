namespace MinorShift.Emuera.Sub;

internal sealed class StrFormWord : Word
{
	private readonly string[] strs;

	private readonly SubWord[] subwords;

	public string[] Strs => strs;

	public SubWord[] SubWords => subwords;

	public override char Type => 'F';

	public StrFormWord(string[] s, SubWord[] SWT)
	{
		strs = s;
		subwords = SWT;
	}

	public override void SetIsMacro()
	{
		IsMacro = true;
		SubWord[] subWords = SubWords;
		for (int i = 0; i < subWords.Length; i++)
		{
			subWords[i].SetIsMacro();
		}
	}
}
