using System;
using System.Collections.Generic;
using MinorShift.Emuera.GameProc;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal abstract class ReferenceToken : UserDefinedVariableToken
{
	protected List<Array> arrayList;

	protected Array array;

	private int counter;

	protected ReferenceToken(VariableCode varCode, UserDefinedVariableData data)
		: base(varCode, data)
	{
		base.CanRestructure = false;
		base.IsStatic = !data.Private;
		base.IsReference = true;
		arrayList = new List<Array>();
		base.IsForbid = false;
	}

	public override void SetDefault()
	{
	}

	public override int GetLength()
	{
		if (array == null)
		{
			throw new CodeEE("参照型変数" + varName + "は何も参照していません");
		}
		if (base.Dimension != 1)
		{
			throw new CodeEE(base.Dimension + "次元配列型変数" + varName + "の長さを取得しようとしました");
		}
		return array.Length;
	}

	public override int GetLength(int dimension)
	{
		if (array == null)
		{
			throw new CodeEE("参照型変数" + varName + "は何も参照していません");
		}
		if (dimension < base.Dimension)
		{
			return array.GetLength(dimension);
		}
		throw new CodeEE("配列型変数" + varName + "の存在しない次元の長さを取得しようとしました");
	}

	public override void CheckElement(long[] arguments, bool[] doCheck)
	{
		if (array == null)
		{
			throw new CodeEE("参照型変数" + varName + "は何も参照していません");
		}
		if (doCheck[0] && (arguments[0] < 0 || arguments[0] >= array.GetLength(0)))
		{
			throw new CodeEE("配列型変数" + varName + "の第１引数(" + arguments[0] + ")は配列の範囲外です");
		}
		if (base.Dimension >= 2 && (arguments[1] < 0 || arguments[1] >= array.GetLength(1)))
		{
			throw new CodeEE("配列型変数" + varName + "の第２引数(" + arguments[1] + ")は配列の範囲外です");
		}
		if (base.Dimension >= 3 && (arguments[2] < 0 || arguments[2] >= array.GetLength(2)))
		{
			throw new CodeEE("配列型変数" + varName + "の第３引数(" + arguments[2] + ")は配列の範囲外です");
		}
	}

	public override void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
	{
		CheckElement(arguments);
		if (index1 < 0 || index1 > array.GetLength(base.Dimension - 1))
		{
			throw new CodeEE(funcName + "命令の第" + i1 + "引数(" + index1 + ")は配列" + varName + "の範囲外です");
		}
		if (index2 < 0 || index2 > array.GetLength(base.Dimension - 1))
		{
			throw new CodeEE(funcName + "命令の第" + i2 + "引数(" + index2 + ")は配列" + varName + "の範囲外です");
		}
	}

	public override void In()
	{
		if (counter > 0)
		{
			arrayList.Add(array);
		}
		counter++;
		array = null;
	}

	public override void Out()
	{
		if (arrayList.Count > 0)
		{
			array = arrayList[arrayList.Count - 1];
			arrayList.RemoveAt(arrayList.Count - 1);
		}
		else
		{
			array = null;
		}
		counter--;
	}

	public override object GetArray()
	{
		if (array == null)
		{
			throw new CodeEE("参照型変数" + varName + "は何も参照していません");
		}
		return array;
	}

	public void SetRef(Array refArray)
	{
		array = refArray;
	}

	public bool MatchType(VariableToken rother, bool allowChara, out string errMes)
	{
		errMes = "";
		if (rother == null)
		{
			errMes = "参照先変数は省略できません";
			return false;
		}
		if (rother.IsCalc)
		{
			errMes = "疑似変数は参照できません";
			return false;
		}
		if (rother.IsConst)
		{
			errMes = "定数は参照できません";
			return false;
		}
		if (!base.IsPrivate && (rother.IsPrivate || rother.IsLocal))
		{
			errMes = "広域の参照変数はローカル変数を参照できません";
			return false;
		}
		if (rother.IsCharacterData && !allowChara)
		{
			errMes = "キャラ変数は参照できません";
			return false;
		}
		if (base.IsInteger != rother.IsInteger)
		{
			errMes = "型が異なる変数は参照できません";
			return false;
		}
		if (base.Dimension != rother.Dimension)
		{
			errMes = "次元数が異なる変数は参照できません";
			return false;
		}
		return true;
	}
}
