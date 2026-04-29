using System;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal sealed class UserDefinedFunctionArgument
{
	public readonly IOperandTerm[] Arguments;

	public readonly long[] TransporterInt;

	public readonly string[] TransporterStr;

	public readonly Array[] TransporterRef;

	public readonly bool[] isRef;

	public UserDefinedFunctionArgument(IOperandTerm[] srcArgs, VariableTerm[] destArgs)
	{
		Arguments = srcArgs;
		TransporterInt = new long[Arguments.Length];
		TransporterStr = new string[Arguments.Length];
		TransporterRef = new Array[Arguments.Length];
		isRef = new bool[Arguments.Length];
		for (int i = 0; i < Arguments.Length; i++)
		{
			isRef[i] = destArgs[i].Identifier.IsReference;
		}
	}

	public void SetTransporter(ExpressionMediator exm)
	{
		for (int i = 0; i < Arguments.Length; i++)
		{
			if (Arguments[i] == null)
			{
				continue;
			}
			if (isRef[i])
			{
				VariableTerm variableTerm = (VariableTerm)Arguments[i];
				if (variableTerm.Identifier.IsCharacterData)
				{
					long elementInt = variableTerm.GetElementInt(0, exm);
					if (elementInt < 0 || elementInt >= GlobalStatic.VariableData.CharacterList.Count)
					{
						throw new CodeEE("キャラクタ配列変数" + variableTerm.Identifier.Name + "の第１引数(" + elementInt + ")はキャラ登録番号の範囲外です");
					}
					TransporterRef[i] = (Array)variableTerm.Identifier.GetArrayChara((int)elementInt);
				}
				else
				{
					TransporterRef[i] = (Array)variableTerm.Identifier.GetArray();
				}
			}
			else if (Arguments[i].GetOperandType() == typeof(long))
			{
				TransporterInt[i] = Arguments[i].GetIntValue(exm);
			}
			else
			{
				TransporterStr[i] = Arguments[i].GetStrValue(exm);
			}
		}
	}

	public UserDefinedFunctionArgument Restructure(ExpressionMediator exm)
	{
		for (int i = 0; i < Arguments.Length; i++)
		{
			if (Arguments[i] != null)
			{
				if (isRef[i])
				{
					Arguments[i].Restructure(exm);
				}
				else
				{
					Arguments[i] = Arguments[i].Restructure(exm);
				}
			}
		}
		return this;
	}
}
