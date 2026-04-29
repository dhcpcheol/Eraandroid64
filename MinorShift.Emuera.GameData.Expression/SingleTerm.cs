namespace MinorShift.Emuera.GameData.Expression;

internal sealed class SingleTerm : IOperandTerm
{
	private readonly long iValue;

	private string sValue;

	public string Str => sValue;

	public long Int => iValue;

	public SingleTerm(bool i)
		: base(typeof(long))
	{
		if (i)
		{
			iValue = 1L;
		}
		else
		{
			iValue = 0L;
		}
	}

	public SingleTerm(long i)
		: base(typeof(long))
	{
		iValue = i;
	}

	public SingleTerm(string s)
		: base(typeof(string))
	{
		sValue = s;
	}

	public override long GetIntValue(ExpressionMediator exm)
	{
		return iValue;
	}

	public override string GetStrValue(ExpressionMediator exm)
	{
		return sValue;
	}

	public override SingleTerm GetValue(ExpressionMediator exm)
	{
		return this;
	}

	public override string ToString()
	{
		if (GetOperandType() == typeof(long))
		{
			return iValue.ToString();
		}
		if (GetOperandType() == typeof(string))
		{
			return sValue.ToString();
		}
		return base.ToString();
	}

	public override IOperandTerm Restructure(ExpressionMediator exm)
	{
		return this;
	}
}
