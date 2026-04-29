using System;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameProc;

namespace MinorShift.Emuera.GameData.Function;

internal abstract class SuperUserDefinedMethodTerm : IOperandTerm
{
	public abstract UserDefinedFunctionArgument Argument { get; }

	public abstract CalledFunction Call { get; }

	protected SuperUserDefinedMethodTerm(Type returnType)
		: base(returnType)
	{
	}

	public override long GetIntValue(ExpressionMediator exm)
	{
		return exm.Process.GetValue(this)?.Int ?? 0;
	}

	public override string GetStrValue(ExpressionMediator exm)
	{
		SingleTerm value = exm.Process.GetValue(this);
		if (value == null)
		{
			return "";
		}
		return value.Str;
	}

	public override SingleTerm GetValue(ExpressionMediator exm)
	{
		SingleTerm value = exm.Process.GetValue(this);
		if (value == null)
		{
			if (GetOperandType() == typeof(long))
			{
				return new SingleTerm(0L);
			}
			return new SingleTerm("");
		}
		return value;
	}
}
