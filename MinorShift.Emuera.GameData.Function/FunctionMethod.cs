using System;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Function;

internal abstract class FunctionMethod
{
	protected Type[] argumentTypeArray;

	public Type ReturnType { get; protected set; }

	public bool CanRestructure { get; protected set; }

	public bool HasUniqueRestructure { get; protected set; }

	public virtual string CheckArgumentType(string name, IOperandTerm[] arguments)
	{
		if (arguments.Length != argumentTypeArray.Length)
		{
			return name + "関数の引数の数が正しくありません";
		}
		for (int i = 0; i < argumentTypeArray.Length; i++)
		{
			if (arguments[i] == null)
			{
				return name + "関数の" + (i + 1) + "番目の引数は省略できません";
			}
			if (argumentTypeArray[i] != arguments[i].GetOperandType())
			{
				return name + "関数の" + (i + 1) + "番目の引数の型が正しくありません";
			}
		}
		return null;
	}

	public virtual long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
	{
		throw new ExeEE("戻り値の型が違う or 未実装");
	}

	public virtual string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
	{
		throw new ExeEE("戻り値の型が違う or 未実装");
	}

	public virtual SingleTerm GetReturnValue(ExpressionMediator exm, IOperandTerm[] arguments)
	{
		if (ReturnType == typeof(long))
		{
			return new SingleTerm(GetIntValue(exm, arguments));
		}
		return new SingleTerm(GetStrValue(exm, arguments));
	}

	public virtual bool UniqueRestructure(ExpressionMediator exm, IOperandTerm[] arguments)
	{
		throw new ExeEE("未実装？");
	}
}
