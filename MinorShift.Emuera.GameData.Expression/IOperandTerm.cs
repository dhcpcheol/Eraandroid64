using System;

namespace MinorShift.Emuera.GameData.Expression;

internal abstract class IOperandTerm
{
	private readonly Type type;

	public bool IsInteger => type == typeof(long);

	public bool IsString => type == typeof(string);

	public IOperandTerm(Type t)
	{
		type = t;
	}

	public Type GetOperandType()
	{
		return type;
	}

	public virtual long GetIntValue(ExpressionMediator exm)
	{
		return 0L;
	}

	public virtual string GetStrValue(ExpressionMediator exm)
	{
		return "";
	}

	public virtual SingleTerm GetValue(ExpressionMediator exm)
	{
		if (type == typeof(long))
		{
			return new SingleTerm(0L);
		}
		return new SingleTerm("");
	}

	public virtual IOperandTerm Restructure(ExpressionMediator exm)
	{
		return this;
	}
}
