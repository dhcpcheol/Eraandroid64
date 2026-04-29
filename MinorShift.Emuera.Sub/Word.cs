namespace MinorShift.Emuera.Sub;

internal abstract class Word
{
	public bool IsMacro;

	public abstract char Type { get; }

	public virtual void SetIsMacro()
	{
		IsMacro = true;
	}
}
