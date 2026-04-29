namespace MinorShift.Emuera.Sub;

internal abstract class SubWord
{
	private readonly WordCollection words;

	public bool IsMacro;

	public WordCollection Words => words;

	protected SubWord(WordCollection w)
	{
		words = w;
	}

	public virtual void SetIsMacro()
	{
		IsMacro = true;
		if (Words != null)
		{
			Words.SetIsMacro();
		}
	}
}
