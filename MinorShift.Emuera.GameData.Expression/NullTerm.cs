namespace MinorShift.Emuera.GameData.Expression;

internal sealed class NullTerm : IOperandTerm
{
	public NullTerm(long i)
		: base(typeof(long))
	{
	}

	public NullTerm(string s)
		: base(typeof(string))
	{
	}
}
