namespace MinorShift.Emuera.Sub;

internal sealed class YenAtSubWord : SubWord
{
	private readonly StrFormWord left;

	private readonly StrFormWord right;

	public StrFormWord Left => left;

	public StrFormWord Right => right;

	public YenAtSubWord(WordCollection w, StrFormWord fsLeft, StrFormWord fsRight)
		: base(w)
	{
		left = fsLeft;
		right = fsRight;
	}

	public override void SetIsMacro()
	{
		IsMacro = true;
		base.Words.SetIsMacro();
		Left.SetIsMacro();
		Right.SetIsMacro();
	}
}
