using System;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal abstract class VariableToken
{
	public readonly VariableCode Code;

	public readonly int VarCodeInt;

	protected readonly VariableData varData;

	protected string varName;

	public Type VariableType { get; protected set; }

	public bool CanRestructure { get; protected set; }

	public string Name => varName;

	public int CodeInt => VarCodeInt;

	public VariableCode CodeFlag => Code & VariableCode.__UPPERCASE__;

	public bool IsNull => Code == VariableCode.__NULL__;

	public bool IsCharacterData => (Code & VariableCode.__CHARACTER_DATA__) == VariableCode.__CHARACTER_DATA__;

	public bool IsInteger => (Code & VariableCode.__INTEGER__) == VariableCode.__INTEGER__;

	public bool IsString => (Code & VariableCode.__STRING__) == VariableCode.__STRING__;

	public bool IsArray1D => (Code & VariableCode.__ARRAY_1D__) == VariableCode.__ARRAY_1D__;

	public bool IsArray2D => (Code & VariableCode.__ARRAY_2D__) == VariableCode.__ARRAY_2D__;

	public bool IsArray3D => (Code & VariableCode.__ARRAY_3D__) == VariableCode.__ARRAY_3D__;

	public virtual bool IsConst => (Code & VariableCode.__UNCHANGEABLE__) == VariableCode.__UNCHANGEABLE__;

	public bool IsCalc => (Code & VariableCode.__CALC__) == VariableCode.__CALC__;

	public bool IsLocal => (Code & VariableCode.__LOCAL__) == VariableCode.__LOCAL__;

	public bool CanForbid => (Code & VariableCode.__CAN_FORBID__) == VariableCode.__CAN_FORBID__;

	public bool IsForbid { get; protected set; }

	public bool IsPrivate { get; protected set; }

	public bool IsGlobal { get; protected set; }

	public bool IsSavedata { get; protected set; }

	public bool IsReference { get; protected set; }

	public int Dimension { get; protected set; }

	protected VariableToken(VariableCode varCode, VariableData varData)
	{
		Code = varCode;
		VariableType = (((varCode & VariableCode.__INTEGER__) == VariableCode.__INTEGER__) ? typeof(long) : typeof(string));
		VarCodeInt = (int)(varCode & VariableCode.__LOWERCASE__);
		varName = varCode.ToString();
		this.varData = varData;
		IsForbid = false;
		IsPrivate = false;
		IsReference = false;
		Dimension = 0;
		IsGlobal = Code == VariableCode.GLOBAL || Code == VariableCode.GLOBALS;
		if ((Code & VariableCode.__ARRAY_1D__) == VariableCode.__ARRAY_1D__)
		{
			Dimension = 1;
		}
		if ((Code & VariableCode.__ARRAY_2D__) == VariableCode.__ARRAY_2D__)
		{
			Dimension = 2;
		}
		if ((Code & VariableCode.__ARRAY_3D__) == VariableCode.__ARRAY_3D__)
		{
			Dimension = 3;
		}
		IsSavedata = false;
		if (Code == VariableCode.GLOBAL || Code == VariableCode.GLOBALS)
		{
			IsSavedata = true;
		}
		else if ((Code & VariableCode.__SAVE_EXTENDED__) == VariableCode.__SAVE_EXTENDED__)
		{
			IsSavedata = true;
		}
		else
		{
			if ((Code & VariableCode.__EXTENDED__) == VariableCode.__EXTENDED__ || (Code & VariableCode.__CALC__) == VariableCode.__CALC__ || (Code & VariableCode.__UNCHANGEABLE__) == VariableCode.__UNCHANGEABLE__ || (Code & VariableCode.__LOCAL__) == VariableCode.__LOCAL__ || varName.StartsWith("NOTUSE_"))
			{
				return;
			}
			switch (Code & (VariableCode)673054720)
			{
			case VariableCode.ISASSI:
				if (VarCodeInt < 2)
				{
					IsSavedata = true;
				}
				break;
			case VariableCode.NAME:
				if (VarCodeInt < 2)
				{
					IsSavedata = true;
				}
				break;
			case (VariableCode)1703936:
				if (VarCodeInt < 17)
				{
					IsSavedata = true;
				}
				break;
			case (VariableCode)1835008:
				if (VarCodeInt < 0)
				{
					IsSavedata = true;
				}
				break;
			case VariableCode.__INTEGER__:
				if (VarCodeInt < 0)
				{
					IsSavedata = true;
				}
				break;
			case VariableCode.__STRING__:
				if (VarCodeInt < 0)
				{
					IsSavedata = true;
				}
				break;
			case (VariableCode)655360:
				if (VarCodeInt < 60)
				{
					IsSavedata = true;
				}
				break;
			case (VariableCode)786432:
				if (VarCodeInt < 1)
				{
					IsSavedata = true;
				}
				break;
			}
		}
	}

	public virtual long GetIntValue(ExpressionMediator exm, long[] arguments)
	{
		throw new CodeEE("整数型でない変数" + varName + "を整数型として呼び出しました");
	}

	public virtual string GetStrValue(ExpressionMediator exm, long[] arguments)
	{
		throw new CodeEE("文字列型でない変数" + varName + "を文字列型として呼び出しました");
	}

	public virtual void SetValue(long value, long[] arguments)
	{
		throw new CodeEE("整数型でない変数" + varName + "を整数型として呼び出しました");
	}

	public virtual void SetValue(string value, long[] arguments)
	{
		throw new CodeEE("文字列型でない変数" + varName + "を文字列型として呼び出しました");
	}

	public virtual void SetValue(long[] values, long[] arguments)
	{
		throw new CodeEE("整数型配列でない変数" + varName + "を整数型配列として呼び出しました");
	}

	public virtual void SetValue(string[] values, long[] arguments)
	{
		throw new CodeEE("文字列型配列でない変数" + varName + "を文字列型配列として呼び出しました");
	}

	public virtual void SetValueAll(long value, int start, int end, int charaPos)
	{
		throw new CodeEE("整数型配列でない変数" + varName + "を整数型配列として呼び出しました");
	}

	public virtual void SetValueAll(string value, int start, int end, int charaPos)
	{
		throw new CodeEE("文字列型配列でない変数" + varName + "を文字列型配列として呼び出しました");
	}

	public virtual long PlusValue(long value, long[] arguments)
	{
		throw new CodeEE("整数型でない変数" + varName + "を整数型として呼び出しました");
	}

	public virtual int GetLength()
	{
		throw new CodeEE("配列型でない変数" + varName + "の長さを取得しようとしました");
	}

	public virtual int GetLength(int dimension)
	{
		throw new CodeEE("配列型でない変数" + varName + "の長さを取得しようとしました");
	}

	public virtual object GetArray()
	{
		if (IsCharacterData)
		{
			throw new CodeEE("キャラクタ変数" + varName + "を非キャラ変数として呼び出しました");
		}
		throw new CodeEE("配列型でない変数" + varName + "の配列を取得しようとしました");
	}

	public virtual object GetArrayChara(int charano)
	{
		if (!IsCharacterData)
		{
			throw new CodeEE("非キャラクタ変数" + varName + "をキャラ変数として呼び出しました");
		}
		throw new CodeEE("配列型でない変数" + varName + "の配列を取得しようとしました");
	}

	public void throwOutOfRangeException(long[] arguments, Exception e)
	{
		CheckElement(arguments, new bool[3] { true, true, true });
		throw e;
	}

	public virtual void CheckElement(long[] arguments, bool[] doCheck)
	{
	}

	public void CheckElement(long[] arguments)
	{
		CheckElement(arguments, new bool[3] { true, true, true });
	}

	public virtual void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
	{
		CheckElement(arguments, new bool[3] { true, true, true });
	}
}
