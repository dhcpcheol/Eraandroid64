using System;
using System.Collections.Generic;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameProc;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal sealed class VariableData : IDisposable
{
	private sealed class IntVariableToken : VariableToken
	{
		private long[] array;

		public IntVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
			array = varData.DataInteger;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return array[VarCodeInt];
		}

		public override void SetValue(long value, long[] arguments)
		{
			array[VarCodeInt] = value;
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			array[VarCodeInt] = value;
		}

		public override long PlusValue(long value, long[] arguments)
		{
			array[VarCodeInt] += value;
			return array[VarCodeInt];
		}
	}

	private sealed class Int1DVariableToken : VariableToken
	{
		private long[] array;

		public Int1DVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
			array = varData.DataIntegerArray[VarCodeInt];
			base.IsForbid = array.Length == 0;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return array[arguments[0]];
		}

		public override void SetValue(long value, long[] arguments)
		{
			array[arguments[0]] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			int num = (int)arguments[0];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			for (int i = start; i < end; i++)
			{
				array[i] = value;
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			array[arguments[0]] += value;
			return array[arguments[0]];
		}

		public override int GetLength()
		{
			return array.Length;
		}

		public override int GetLength(int dimension)
		{
			if (dimension == 0)
			{
				return array.Length;
			}
			throw new CodeEE("配列型変数" + varName + "の存在しない次元の長さを取得しようとしました");
		}

		public override object GetArray()
		{
			return array;
		}

		public override void CheckElement(long[] arguments, bool[] doCheck)
		{
			if (doCheck[0] && (arguments[0] < 0 || arguments[0] >= array.Length))
			{
				throw new CodeEE("配列変数" + varName + "の第１引数(" + arguments[0] + ")は配列の範囲外です");
			}
		}

		public override void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
		{
			CheckElement(arguments);
			if (index1 < 0 || index1 > array.Length)
			{
				throw new CodeEE(funcName + "命令の第" + i1 + "引数(" + index1 + ")は配列" + varName + "の範囲外です");
			}
			if (index2 < 0 || index2 > array.Length)
			{
				throw new CodeEE(funcName + "命令の第" + i2 + "引数(" + index2 + ")は配列" + varName + "の範囲外です");
			}
		}
	}

	private sealed class Int2DVariableToken : VariableToken
	{
		private long[,] array;

		public Int2DVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
			array = varData.DataIntegerArray2D[VarCodeInt];
			base.IsForbid = array.Length == 0;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])];
		}

		public override void SetValue(long value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			int num = (int)arguments[1];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[(int)checked((nint)arguments[0]), i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					array[i, j] = value;
				}
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])] += value;
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])];
		}

		public override int GetLength()
		{
			throw new CodeEE("2次元配列型変数" + varName + "の長さを取得しようとしました");
		}

		public override int GetLength(int dimension)
		{
			if (dimension == 0 || dimension == 1)
			{
				return array.GetLength(dimension);
			}
			throw new CodeEE("配列型変数" + varName + "の存在しない次元の長さを取得しようとしました");
		}

		public override object GetArray()
		{
			return array;
		}

		public override void CheckElement(long[] arguments, bool[] doCheck)
		{
			if (doCheck[0] && (arguments[0] < 0 || arguments[0] >= array.GetLength(0)))
			{
				throw new CodeEE("二次元配列" + varName + "の第１引数(" + arguments[0] + ")は配列の範囲外です");
			}
			if (doCheck[1] && (arguments[1] < 0 || arguments[1] >= array.GetLength(1)))
			{
				throw new CodeEE("二次元配列" + varName + "の第２引数(" + arguments[1] + ")は配列の範囲外です");
			}
		}

		public override void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
		{
			CheckElement(arguments);
			if (index1 < 0 || index1 > array.GetLength(1))
			{
				throw new CodeEE(funcName + "命令の第" + i1 + "引数(" + index1 + ")は配列" + varName + "の範囲外です");
			}
			if (index2 < 0 || index2 > array.GetLength(1))
			{
				throw new CodeEE(funcName + "命令の第" + i2 + "引数(" + index2 + ")は配列" + varName + "の範囲外です");
			}
		}
	}

	private sealed class Int3DVariableToken : VariableToken
	{
		private long[,,] array;

		public Int3DVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
			array = varData.DataIntegerArray3D[VarCodeInt];
			base.IsForbid = array.Length == 0;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override void SetValue(long value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			int length3 = array.GetLength(2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						array[i, j, k] = value;
					}
				}
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] += value;
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override int GetLength()
		{
			throw new CodeEE("3次元配列型変数" + varName + "の長さを取得しようとしました");
		}

		public override int GetLength(int dimension)
		{
			if (dimension == 0 || dimension == 1 || dimension == 2)
			{
				return array.GetLength(dimension);
			}
			throw new CodeEE("配列型変数" + varName + "の存在しない次元の長さを取得しようとしました");
		}

		public override object GetArray()
		{
			return array;
		}

		public override void CheckElement(long[] arguments, bool[] doCheck)
		{
			if (doCheck[0] && (arguments[0] < 0 || arguments[0] >= array.GetLength(0)))
			{
				throw new CodeEE("三次元配列" + varName + "の第１引数(" + arguments[0] + ")は配列の範囲外です");
			}
			if (doCheck[1] && (arguments[1] < 0 || arguments[1] >= array.GetLength(1)))
			{
				throw new CodeEE("三次元配列" + varName + "の第２引数(" + arguments[1] + ")は配列の範囲外です");
			}
			if (doCheck[2] && (arguments[2] < 0 || arguments[2] >= array.GetLength(2)))
			{
				throw new CodeEE("三次元配列" + varName + "の第３引数(" + arguments[2] + ")は配列の範囲外です");
			}
		}

		public override void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
		{
			CheckElement(arguments);
			if (index1 < 0 || index1 > array.GetLength(2))
			{
				throw new CodeEE(funcName + "命令の第" + i1 + "引数(" + index1 + ")は配列" + varName + "の範囲外です");
			}
			if (index2 < 0 || index2 > array.GetLength(2))
			{
				throw new CodeEE(funcName + "命令の第" + i2 + "引数(" + index2 + ")は配列" + varName + "の範囲外です");
			}
		}
	}

	private sealed class StrVariableToken : VariableToken
	{
		private string[] array;

		public StrVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
			array = varData.DataString;
			base.IsForbid = array.Length == 0;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return array[VarCodeInt];
		}

		public override void SetValue(string value, long[] arguments)
		{
			array[VarCodeInt] = value;
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			array[VarCodeInt] = value;
		}
	}

	private sealed class Str1DVariableToken : VariableToken
	{
		private string[] array;

		public Str1DVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
			array = varData.DataStringArray[VarCodeInt];
			base.IsForbid = array.Length == 0;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return array[arguments[0]];
		}

		public override void SetValue(string value, long[] arguments)
		{
			array[arguments[0]] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			int num = (int)arguments[0];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			for (int i = start; i < end; i++)
			{
				array[i] = value;
			}
		}

		public override int GetLength()
		{
			return array.Length;
		}

		public override int GetLength(int dimension)
		{
			if (dimension == 0)
			{
				return array.Length;
			}
			throw new CodeEE("配列型変数" + varName + "の存在しない次元の長さを取得しようとしました");
		}

		public override object GetArray()
		{
			return array;
		}

		public override void CheckElement(long[] arguments, bool[] doCheck)
		{
			if (doCheck[0] && (arguments[0] < 0 || arguments[0] >= array.Length))
			{
				throw new CodeEE("配列変数" + varName + "の第１引数(" + arguments[0] + ")は配列の範囲外です");
			}
		}

		public override void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
		{
			CheckElement(arguments);
			if (index1 < 0 || index1 > array.Length)
			{
				throw new CodeEE(funcName + "命令の第" + i1 + "引数(" + index1 + ")は配列" + varName + "の範囲外です");
			}
			if (index2 < 0 || index2 > array.Length)
			{
				throw new CodeEE(funcName + "命令の第" + i2 + "引数(" + index2 + ")は配列" + varName + "の範囲外です");
			}
		}
	}

	private sealed class Str2DVariableToken : VariableToken
	{
		private string[,] array;

		public Str2DVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
			array = varData.DataStringArray2D[VarCodeInt];
			base.IsForbid = array.Length == 0;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])];
		}

		public override void SetValue(string value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			int num = (int)arguments[1];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[(int)checked((nint)arguments[0]), i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					array[i, j] = value;
				}
			}
		}

		public override int GetLength()
		{
			throw new CodeEE("2次元配列型変数" + varName + "の長さを取得しようとしました");
		}

		public override int GetLength(int dimension)
		{
			if (dimension == 0 || dimension == 1)
			{
				return array.GetLength(dimension);
			}
			throw new CodeEE("配列型変数" + varName + "の存在しない次元の長さを取得しようとしました");
		}

		public override object GetArray()
		{
			return array;
		}

		public override void CheckElement(long[] arguments, bool[] doCheck)
		{
			if (doCheck[0] && (arguments[0] < 0 || arguments[0] >= array.GetLength(0)))
			{
				throw new CodeEE("二次元配列" + varName + "の第１引数(" + arguments[0] + ")は配列の範囲外です");
			}
			if (doCheck[1] && (arguments[1] < 0 || arguments[1] >= array.GetLength(1)))
			{
				throw new CodeEE("二次元配列" + varName + "の第２引数(" + arguments[1] + ")は配列の範囲外です");
			}
		}

		public override void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
		{
			CheckElement(arguments);
			if (index1 < 0 || index1 > array.GetLength(1))
			{
				throw new CodeEE(funcName + "命令の第" + i1 + "引数(" + index1 + ")は配列" + varName + "の範囲外です");
			}
			if (index2 < 0 || index2 > array.GetLength(1))
			{
				throw new CodeEE(funcName + "命令の第" + i2 + "引数(" + index2 + ")は配列" + varName + "の範囲外です");
			}
		}
	}

	private sealed class Str3DVariableToken : VariableToken
	{
		private string[,,] array;

		public Str3DVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
			array = varData.DataStringArray3D[VarCodeInt];
			base.IsForbid = array.Length == 0;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override void SetValue(string value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			int length3 = array.GetLength(2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						array[i, j, k] = value;
					}
				}
			}
		}

		public override int GetLength()
		{
			throw new CodeEE("3次元配列型変数" + varName + "の長さを取得しようとしました");
		}

		public override int GetLength(int dimension)
		{
			if (dimension == 0 || dimension == 1 || dimension == 2)
			{
				return array.GetLength(dimension);
			}
			throw new CodeEE("配列型変数" + varName + "の存在しない次元の長さを取得しようとしました");
		}

		public override object GetArray()
		{
			return array;
		}

		public override void CheckElement(long[] arguments, bool[] doCheck)
		{
			if (doCheck[0] && (arguments[0] < 0 || arguments[0] >= array.GetLength(0)))
			{
				throw new CodeEE("三次元配列" + varName + "の第１引数(" + arguments[0] + ")は配列の範囲外です");
			}
			if (doCheck[1] && (arguments[1] < 0 || arguments[1] >= array.GetLength(1)))
			{
				throw new CodeEE("三次元配列" + varName + "の第２引数(" + arguments[1] + ")は配列の範囲外です");
			}
			if (doCheck[2] && (arguments[2] < 0 || arguments[2] >= array.GetLength(2)))
			{
				throw new CodeEE("三次元配列" + varName + "の第３引数(" + arguments[2] + ")は配列の範囲外です");
			}
		}

		public override void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
		{
			CheckElement(arguments);
			if (index1 < 0 || index1 > array.GetLength(2))
			{
				throw new CodeEE(funcName + "命令の第" + i1 + "引数(" + index1 + ")は配列" + varName + "の範囲外です");
			}
			if (index2 < 0 || index2 > array.GetLength(2))
			{
				throw new CodeEE(funcName + "命令の第" + i2 + "引数(" + index2 + ")は配列" + varName + "の範囲外です");
			}
		}
	}

	private sealed class CharaIntVariableToken : CharaVariableToken
	{
		public CharaIntVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return varData.CharacterList[(int)arguments[0]].DataInteger[VarCodeInt];
		}

		public override void SetValue(long value, long[] arguments)
		{
			varData.CharacterList[(int)arguments[0]].DataInteger[VarCodeInt] = value;
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			varData.CharacterList[charaPos].setValueAll(VarCodeInt, value);
		}

		public override long PlusValue(long value, long[] arguments)
		{
			CharacterData characterData = varData.CharacterList[(int)arguments[0]];
			characterData.DataInteger[VarCodeInt] += value;
			return characterData.DataInteger[VarCodeInt];
		}
	}

	private sealed class CharaInt1DVariableToken : CharaVariableToken
	{
		public CharaInt1DVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return varData.CharacterList[(int)arguments[0]].DataIntegerArray[VarCodeInt][arguments[1]];
		}

		public override void SetValue(long value, long[] arguments)
		{
			varData.CharacterList[(int)arguments[0]].DataIntegerArray[VarCodeInt][arguments[1]] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			long[] array = varData.CharacterList[(int)arguments[0]].DataIntegerArray[VarCodeInt];
			int num = (int)arguments[1];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			varData.CharacterList[charaPos].setValueAll1D(VarCodeInt, value, start, end);
		}

		public override long PlusValue(long value, long[] arguments)
		{
			CharacterData characterData = varData.CharacterList[(int)arguments[0]];
			characterData.DataIntegerArray[VarCodeInt][arguments[1]] += value;
			return characterData.DataIntegerArray[VarCodeInt][arguments[1]];
		}

		public override object GetArrayChara(int charano)
		{
			return varData.CharacterList[charano].DataIntegerArray[VarCodeInt];
		}
	}

	private sealed class CharaStrVariableToken : CharaVariableToken
	{
		public CharaStrVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return varData.CharacterList[(int)arguments[0]].DataString[VarCodeInt];
		}

		public override void SetValue(string value, long[] arguments)
		{
			varData.CharacterList[(int)arguments[0]].DataString[VarCodeInt] = value;
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			varData.CharacterList[charaPos].setValueAll(VarCodeInt, value);
		}
	}

	private sealed class CharaStr1DVariableToken : CharaVariableToken
	{
		public CharaStr1DVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return varData.CharacterList[(int)arguments[0]].DataStringArray[VarCodeInt][arguments[1]];
		}

		public override void SetValue(string value, long[] arguments)
		{
			varData.CharacterList[(int)arguments[0]].DataStringArray[VarCodeInt][arguments[1]] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			string[] array = varData.CharacterList[(int)arguments[0]].DataStringArray[VarCodeInt];
			int num = (int)arguments[1];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			varData.CharacterList[charaPos].setValueAll1D(VarCodeInt, value, start, end);
		}

		public override object GetArrayChara(int charano)
		{
			return varData.CharacterList[charano].DataStringArray[VarCodeInt];
		}
	}

	private sealed class CharaInt2DVariableToken : CharaVariableToken
	{
		public CharaInt2DVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return varData.CharacterList[(int)arguments[0]].DataIntegerArray2D[VarCodeInt][(int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override void SetValue(long value, long[] arguments)
		{
			varData.CharacterList[(int)arguments[0]].DataIntegerArray2D[VarCodeInt][(int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			long[,] array = varData.CharacterList[(int)arguments[0]].DataIntegerArray2D[VarCodeInt];
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			int num3 = (int)arguments[1];
			for (int i = num; i < num2; i++)
			{
				array[num3, i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			varData.characterList[charaPos].setValueAll2D(VarCodeInt, value);
		}

		public override long PlusValue(long value, long[] arguments)
		{
			CharacterData characterData = varData.CharacterList[(int)arguments[0]];
			characterData.DataIntegerArray2D[VarCodeInt][(int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] += value;
			return characterData.DataIntegerArray2D[VarCodeInt][(int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override object GetArrayChara(int charano)
		{
			return varData.CharacterList[charano].DataIntegerArray2D[VarCodeInt];
		}
	}

	private sealed class CharaStr2DVariableToken : CharaVariableToken
	{
		public CharaStr2DVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return varData.CharacterList[(int)arguments[0]].DataStringArray2D[VarCodeInt][(int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override void SetValue(string value, long[] arguments)
		{
			varData.CharacterList[(int)arguments[0]].DataStringArray2D[VarCodeInt][(int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			string[,] array = varData.CharacterList[(int)arguments[0]].DataStringArray2D[VarCodeInt];
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			int num3 = (int)arguments[1];
			for (int i = num; i < num2; i++)
			{
				array[num3, i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			varData.characterList[charaPos].setValueAll2D(VarCodeInt, value);
		}

		public override object GetArrayChara(int charano)
		{
			return varData.CharacterList[charano].DataStringArray2D[VarCodeInt];
		}
	}

	private abstract class ConstantToken : VariableToken
	{
		public ConstantToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = true;
		}

		public override void SetValue(long value, long[] arguments)
		{
			throw new CodeEE("読み取り専用の変数" + varName + "に代入しようとしました");
		}

		public override void SetValue(string value, long[] arguments)
		{
			throw new CodeEE("読み取り専用の変数" + varName + "に代入しようとしました");
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			throw new CodeEE("読み取り専用の変数" + varName + "に代入しようとしました");
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			throw new CodeEE("読み取り専用の変数" + varName + "に代入しようとしました");
		}

		public override long PlusValue(long value, long[] arguments)
		{
			throw new CodeEE("読み取り専用の変数" + varName + "に代入しようとしました");
		}
	}

	private sealed class IntConstantToken : ConstantToken
	{
		private long i;

		public IntConstantToken(VariableCode varCode, VariableData varData, long i)
			: base(varCode, varData)
		{
			this.i = i;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return i;
		}
	}

	private sealed class StrConstantToken : ConstantToken
	{
		private string s;

		public StrConstantToken(VariableCode varCode, VariableData varData, string s)
			: base(varCode, varData)
		{
			this.s = s;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return s;
		}
	}

	private sealed class Int1DConstantToken : ConstantToken
	{
		private long[] array;

		public Int1DConstantToken(VariableCode varCode, VariableData varData, long[] array)
			: base(varCode, varData)
		{
			this.array = array;
			base.IsForbid = array.Length == 0;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return array[arguments[0]];
		}

		public override int GetLength()
		{
			return array.Length;
		}

		public override int GetLength(int dimension)
		{
			if (dimension == 0)
			{
				return array.Length;
			}
			throw new CodeEE("配列型変数" + varName + "の存在しない次元の長さを取得しようとしました");
		}

		public override object GetArray()
		{
			return array;
		}

		public override void CheckElement(long[] arguments, bool[] doCheck)
		{
			if (doCheck[0] && (arguments[0] < 0 || arguments[0] >= array.Length))
			{
				throw new CodeEE("配列変数" + varName + "の第１引数(" + arguments[0] + ")は配列の範囲外です");
			}
		}

		public override void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
		{
			CheckElement(arguments);
			if (index1 < 0 || index1 > array.Length)
			{
				throw new CodeEE(funcName + "命令の第" + i1 + "引数(" + index1 + ")は配列" + varName + "の範囲外です");
			}
			if (index2 < 0 || index2 > array.Length)
			{
				throw new CodeEE(funcName + "命令の第" + i2 + "引数(" + index2 + ")は配列" + varName + "の範囲外です");
			}
		}
	}

	private sealed class Str1DConstantToken : ConstantToken
	{
		private string[] array;

		public Str1DConstantToken(VariableCode varCode, VariableData varData, string[] array)
			: base(varCode, varData)
		{
			this.array = array;
			base.IsForbid = array.Length == 0;
		}

		public Str1DConstantToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			array = varData.constant.GetCsvNameList(varCode);
			base.IsForbid = array.Length == 0;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return array[arguments[0]];
		}

		public override int GetLength()
		{
			return array.Length;
		}

		public override int GetLength(int dimension)
		{
			if (dimension == 0)
			{
				return array.Length;
			}
			throw new CodeEE("配列型変数" + varName + "の存在しない次元の長さを取得しようとしました");
		}

		public override object GetArray()
		{
			return array;
		}

		public override void CheckElement(long[] arguments, bool[] doCheck)
		{
			if (doCheck[0] && (arguments[0] < 0 || arguments[0] >= array.Length))
			{
				throw new CodeEE("配列変数" + varName + "の第１引数(" + arguments[0] + ")は配列の範囲外です");
			}
		}

		public override void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
		{
			CheckElement(arguments);
			if (index1 < 0 || index1 > array.Length)
			{
				throw new CodeEE(funcName + "命令の第" + i1 + "引数(" + index1 + ")は配列" + varName + "の範囲外です");
			}
			if (index2 < 0 || index2 > array.Length)
			{
				throw new CodeEE(funcName + "命令の第" + i2 + "引数(" + index2 + ")は配列" + varName + "の範囲外です");
			}
		}
	}

	private abstract class PseudoVariableToken : VariableToken
	{
		protected PseudoVariableToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
		}

		public override void SetValue(long value, long[] arguments)
		{
			throw new CodeEE("擬似変数" + varName + "に代入しようとしました");
		}

		public override void SetValue(string value, long[] arguments)
		{
			throw new CodeEE("擬似変数" + varName + "に代入しようとしました");
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			throw new CodeEE("擬似変数" + varName + "に代入しようとしました");
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			throw new CodeEE("擬似変数" + varName + "に代入しようとしました");
		}

		public override long PlusValue(long value, long[] arguments)
		{
			throw new CodeEE("擬似変数" + varName + "に代入しようとしました");
		}

		public override int GetLength()
		{
			throw new CodeEE("擬似変数" + varName + "の長さを取得しようとしました");
		}

		public override int GetLength(int dimension)
		{
			throw new CodeEE("擬似変数" + varName + "の長さを取得しようとしました");
		}

		public override object GetArray()
		{
			throw new CodeEE("擬似変数" + varName + "の配列を取得しようとしました");
		}
	}

	private sealed class RandToken : PseudoVariableToken
	{
		public RandToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			long num = arguments[0];
			if (num <= 0)
			{
				throw new CodeEE("RANDの引数に0以下の値(" + num + ")が指定されました");
			}
			return exm.VEvaluator.GetNextRand(num);
		}
	}

	private sealed class CompatiRandToken : PseudoVariableToken
	{
		public CompatiRandToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			long num = arguments[0];
			if (num == 0L)
			{
				return 0L;
			}
			if (num < 0)
			{
				num = -num;
			}
			return exm.VEvaluator.GetNextRand(32768L) % num;
		}
	}

	private sealed class CHARANUM_Token : PseudoVariableToken
	{
		public CHARANUM_Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return varData.CharacterList.Count;
		}
	}

	private sealed class LASTLOAD_TEXT_Token : PseudoVariableToken
	{
		public LASTLOAD_TEXT_Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return varData.LastLoadText;
		}
	}

	private sealed class LASTLOAD_VERSION_Token : PseudoVariableToken
	{
		public LASTLOAD_VERSION_Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return varData.LastLoadVersion;
		}
	}

	private sealed class LASTLOAD_NO_Token : PseudoVariableToken
	{
		public LASTLOAD_NO_Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return varData.LastLoadNo;
		}
	}

	private sealed class LINECOUNT_Token : PseudoVariableToken
	{
		public LINECOUNT_Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return exm.Console.LineCount;
		}
	}

	private sealed class WINDOW_TITLE_Token : VariableToken
	{
		public WINDOW_TITLE_Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return GlobalStatic.Console.GetWindowTitle();
		}

		public override void SetValue(string value, long[] arguments)
		{
			GlobalStatic.Console.SetWindowTitle(value);
		}
	}

	private sealed class MONEYLABEL_Token : VariableToken
	{
		public MONEYLABEL_Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return Config.MoneyLabel;
		}
	}

	private sealed class DRAWLINESTR_Token : VariableToken
	{
		public DRAWLINESTR_Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return exm.Console.getDefStBar();
		}
	}

	private sealed class EmptyStrToken : PseudoVariableToken
	{
		public EmptyStrToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return "";
		}
	}

	private sealed class EmptyIntToken : PseudoVariableToken
	{
		public EmptyIntToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return 0L;
		}
	}

	private sealed class Debug__FILE__Token : PseudoVariableToken
	{
		public Debug__FILE__Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			LogicalLine scaningLine = exm.Process.GetScaningLine();
			if (scaningLine == null || scaningLine.Position == null)
			{
				return "";
			}
			return scaningLine.Position.Filename;
		}
	}

	private sealed class Debug__FUNCTION__Token : PseudoVariableToken
	{
		public Debug__FUNCTION__Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			LogicalLine scaningLine = exm.Process.GetScaningLine();
			if (scaningLine == null || scaningLine.ParentLabelLine == null)
			{
				return "";
			}
			return scaningLine.ParentLabelLine.LabelName;
		}
	}

	private sealed class Debug__LINE__Token : PseudoVariableToken
	{
		public Debug__LINE__Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			LogicalLine scaningLine = exm.Process.GetScaningLine();
			if (scaningLine == null || scaningLine.Position == null)
			{
				return -1L;
			}
			return scaningLine.Position.LineNo;
		}
	}

	private sealed class ISTIMEOUTToken : PseudoVariableToken
	{
		public ISTIMEOUTToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = false;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return Convert.ToInt64(GlobalStatic.Console.IsTimeOut);
		}
	}

	private sealed class __INT_MAX__Token : PseudoVariableToken
	{
		public __INT_MAX__Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return long.MaxValue;
		}
	}

	private sealed class __INT_MIN__Token : PseudoVariableToken
	{
		public __INT_MIN__Token(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return long.MinValue;
		}
	}

	private sealed class EMUERA_VERSIONToken : PseudoVariableToken
	{
		public EMUERA_VERSIONToken(VariableCode varCode, VariableData varData)
			: base(varCode, varData)
		{
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return GlobalStatic.FrontEnd.InternalEmueraVer;
		}
	}

	private sealed class LocalInt1DVariableToken : LocalVariableToken
	{
		private long[] array;

		public LocalInt1DVariableToken(VariableCode varCode, VariableData varData, string subId, int size)
			: base(varCode, varData, subId, size)
		{
		}

		public override void SetDefault()
		{
			if (array != null)
			{
				Array.Clear(array, 0, size);
			}
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			if (array == null)
			{
				array = new long[size];
			}
			return array[arguments[0]];
		}

		public override void SetValue(long value, long[] arguments)
		{
			if (array == null)
			{
				array = new long[size];
			}
			array[arguments[0]] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			if (array == null)
			{
				array = new long[size];
			}
			int num = (int)arguments[0];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			if (array == null)
			{
				array = new long[size];
			}
			for (int i = start; i < end; i++)
			{
				array[i] = value;
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			if (array == null)
			{
				array = new long[size];
			}
			array[arguments[0]] += value;
			return array[arguments[0]];
		}

		public override object GetArray()
		{
			if (array == null)
			{
				array = new long[size];
			}
			return array;
		}

		public override void resize(int newSize)
		{
			size = newSize;
			array = null;
		}
	}

	private sealed class LocalStr1DVariableToken : LocalVariableToken
	{
		private string[] array;

		public LocalStr1DVariableToken(VariableCode varCode, VariableData varData, string subId, int size)
			: base(varCode, varData, subId, size)
		{
		}

		public override void SetDefault()
		{
			if (array != null)
			{
				Array.Clear(array, 0, size);
			}
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			if (array == null)
			{
				array = new string[size];
			}
			return array[arguments[0]];
		}

		public override void SetValue(string value, long[] arguments)
		{
			if (array == null)
			{
				array = new string[size];
			}
			array[arguments[0]] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			if (array == null)
			{
				array = new string[size];
			}
			int num = (int)arguments[0];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			if (array == null)
			{
				array = new string[size];
			}
			for (int i = start; i < end; i++)
			{
				array[i] = value;
			}
		}

		public override object GetArray()
		{
			if (array == null)
			{
				array = new string[size];
			}
			return array;
		}

		public override void resize(int newSize)
		{
			size = newSize;
			array = null;
		}
	}

	private sealed class StaticInt1DVariableToken : UserDefinedVariableToken
	{
		private long[] array;

		private long[] defArray;

		public StaticInt1DVariableToken(UserDefinedVariableData data)
			: base(VariableCode.VAR, data)
		{
			int[] lengths = data.Lengths;
			base.IsStatic = true;
			array = new long[lengths[0]];
			defArray = data.DefaultInt;
			if (defArray != null)
			{
				Array.Copy(defArray, array, defArray.Length);
			}
		}

		public override void SetDefault()
		{
			Array.Clear(array, 0, totalSize);
			if (defArray != null)
			{
				Array.Copy(defArray, array, defArray.Length);
			}
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return array[arguments[0]];
		}

		public override void SetValue(long value, long[] arguments)
		{
			array[arguments[0]] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			int num = (int)arguments[0];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			for (int i = start; i < end; i++)
			{
				array[i] = value;
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			array[arguments[0]] += value;
			return array[arguments[0]];
		}

		public override object GetArray()
		{
			return array;
		}

		public override void In()
		{
		}

		public override void Out()
		{
		}
	}

	private sealed class StaticInt2DVariableToken : UserDefinedVariableToken
	{
		private long[,] array;

		public StaticInt2DVariableToken(UserDefinedVariableData data)
			: base(VariableCode.VAR2D, data)
		{
			int[] lengths = data.Lengths;
			base.IsStatic = true;
			array = new long[lengths[0], lengths[1]];
		}

		public override void SetDefault()
		{
			Array.Clear(array, 0, totalSize);
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])];
		}

		public override void SetValue(long value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			int num = (int)arguments[1];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[(int)checked((nint)arguments[0]), i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					array[i, j] = value;
				}
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])] += value;
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])];
		}

		public override object GetArray()
		{
			return array;
		}

		public override void In()
		{
		}

		public override void Out()
		{
		}
	}

	private sealed class StaticInt3DVariableToken : UserDefinedVariableToken
	{
		private long[,,] array;

		public StaticInt3DVariableToken(UserDefinedVariableData data)
			: base(VariableCode.VAR3D, data)
		{
			int[] lengths = data.Lengths;
			base.IsStatic = true;
			array = new long[lengths[0], lengths[1], lengths[2]];
		}

		public override void SetDefault()
		{
			Array.Clear(array, 0, totalSize);
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override void SetValue(long value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			int length3 = array.GetLength(2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						array[i, j, k] = value;
					}
				}
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] += value;
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override object GetArray()
		{
			return array;
		}

		public override void In()
		{
		}

		public override void Out()
		{
		}
	}

	private sealed class StaticStr1DVariableToken : UserDefinedVariableToken
	{
		private string[] array;

		private string[] defArray;

		public StaticStr1DVariableToken(UserDefinedVariableData data)
			: base(VariableCode.VARS, data)
		{
			int[] lengths = data.Lengths;
			base.IsStatic = true;
			array = new string[lengths[0]];
			defArray = data.DefaultStr;
			if (defArray != null)
			{
				Array.Copy(defArray, array, defArray.Length);
			}
		}

		public override void SetDefault()
		{
			Array.Clear(array, 0, totalSize);
			if (defArray != null)
			{
				Array.Copy(defArray, array, defArray.Length);
			}
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return array[arguments[0]];
		}

		public override void SetValue(string value, long[] arguments)
		{
			array[arguments[0]] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			int num = (int)arguments[0];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			for (int i = start; i < end; i++)
			{
				array[i] = value;
			}
		}

		public override object GetArray()
		{
			return array;
		}

		public override void In()
		{
		}

		public override void Out()
		{
		}
	}

	private sealed class StaticStr2DVariableToken : UserDefinedVariableToken
	{
		private string[,] array;

		public StaticStr2DVariableToken(UserDefinedVariableData data)
			: base(VariableCode.VARS2D, data)
		{
			int[] lengths = data.Lengths;
			base.IsStatic = true;
			array = new string[lengths[0], lengths[1]];
		}

		public override void SetDefault()
		{
			Array.Clear(array, 0, totalSize);
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])];
		}

		public override void SetValue(string value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			int num = (int)arguments[1];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[(int)checked((nint)arguments[0]), i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					array[i, j] = value;
				}
			}
		}

		public override object GetArray()
		{
			return array;
		}

		public override void In()
		{
		}

		public override void Out()
		{
		}
	}

	private sealed class StaticStr3DVariableToken : UserDefinedVariableToken
	{
		private string[,,] array;

		public StaticStr3DVariableToken(UserDefinedVariableData data)
			: base(VariableCode.VARS3D, data)
		{
			int[] lengths = data.Lengths;
			base.IsStatic = true;
			array = new string[lengths[0], lengths[1], lengths[2]];
		}

		public override void SetDefault()
		{
			Array.Clear(array, 0, totalSize);
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override void SetValue(string value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			int length3 = array.GetLength(2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						array[i, j, k] = value;
					}
				}
			}
		}

		public override object GetArray()
		{
			return array;
		}

		public override void In()
		{
		}

		public override void Out()
		{
		}
	}

	private sealed class PrivateInt1DVariableToken : UserDefinedVariableToken
	{
		private readonly List<long[]> arrayList;

		private long[] array;

		private long[] defArray;

		public PrivateInt1DVariableToken(UserDefinedVariableData data)
			: base(VariableCode.VAR, data)
		{
			_ = data.Lengths;
			base.IsStatic = false;
			arrayList = new List<long[]>();
			defArray = data.DefaultInt;
		}

		public override void SetDefault()
		{
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return array[arguments[0]];
		}

		public override void SetValue(long value, long[] arguments)
		{
			array[arguments[0]] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			int num = (int)arguments[0];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			for (int i = start; i < end; i++)
			{
				array[i] = value;
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			array[arguments[0]] += value;
			return array[arguments[0]];
		}

		public override object GetArray()
		{
			return array;
		}

		public override void In()
		{
			if (array != null)
			{
				arrayList.Add(array);
			}
			array = new long[sizes[0]];
			if (defArray != null)
			{
				Array.Copy(defArray, array, defArray.Length);
			}
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
		}
	}

	private sealed class PrivateInt2DVariableToken : UserDefinedVariableToken
	{
		private readonly List<long[,]> arrayList;

		private long[,] array;

		public PrivateInt2DVariableToken(UserDefinedVariableData data)
			: base(VariableCode.VAR2D, data)
		{
			_ = data.Lengths;
			base.IsStatic = false;
			arrayList = new List<long[,]>();
		}

		public override void SetDefault()
		{
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])];
		}

		public override void SetValue(long value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			int num = (int)arguments[1];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[(int)checked((nint)arguments[0]), i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					array[i, j] = value;
				}
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])] += value;
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])];
		}

		public override object GetArray()
		{
			return array;
		}

		public override void In()
		{
			if (array != null)
			{
				arrayList.Add(array);
			}
			array = new long[sizes[0], sizes[1]];
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
		}
	}

	private sealed class PrivateInt3DVariableToken : UserDefinedVariableToken
	{
		private readonly List<long[,,]> arrayList;

		private long[,,] array;

		public PrivateInt3DVariableToken(UserDefinedVariableData data)
			: base(VariableCode.VAR3D, data)
		{
			_ = data.Lengths;
			base.IsStatic = false;
			arrayList = new List<long[,,]>();
		}

		public override void SetDefault()
		{
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override void SetValue(long value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			int length3 = array.GetLength(2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						array[i, j, k] = value;
					}
				}
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] += value;
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override object GetArray()
		{
			return array;
		}

		public override void In()
		{
			if (array != null)
			{
				arrayList.Add(array);
			}
			array = new long[sizes[0], sizes[1], sizes[2]];
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
		}
	}

	private sealed class PrivateStr1DVariableToken : UserDefinedVariableToken
	{
		private readonly List<string[]> arrayList;

		private string[] array;

		private string[] defArray;

		public PrivateStr1DVariableToken(UserDefinedVariableData data)
			: base(VariableCode.VARS, data)
		{
			_ = data.Lengths;
			base.IsStatic = false;
			arrayList = new List<string[]>();
			defArray = data.DefaultStr;
		}

		public override void SetDefault()
		{
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return array[arguments[0]];
		}

		public override void SetValue(string value, long[] arguments)
		{
			array[arguments[0]] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			int num = (int)arguments[0];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			for (int i = start; i < end; i++)
			{
				array[i] = value;
			}
		}

		public override object GetArray()
		{
			return array;
		}

		public override void In()
		{
			if (array != null)
			{
				arrayList.Add(array);
			}
			array = new string[sizes[0]];
			if (defArray != null)
			{
				Array.Copy(defArray, array, defArray.Length);
			}
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
		}
	}

	private sealed class PrivateStr2DVariableToken : UserDefinedVariableToken
	{
		private readonly List<string[,]> arrayList;

		private string[,] array;

		public PrivateStr2DVariableToken(UserDefinedVariableData data)
			: base(VariableCode.VARS2D, data)
		{
			_ = data.Lengths;
			base.IsStatic = false;
			arrayList = new List<string[,]>();
		}

		public override void SetDefault()
		{
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])];
		}

		public override void SetValue(string value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			int num = (int)arguments[1];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[(int)checked((nint)arguments[0]), i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					array[i, j] = value;
				}
			}
		}

		public override object GetArray()
		{
			return array;
		}

		public override void In()
		{
			if (array != null)
			{
				arrayList.Add(array);
			}
			array = new string[sizes[0], sizes[1]];
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
		}
	}

	private sealed class PrivateStr3DVariableToken : UserDefinedVariableToken
	{
		private readonly List<string[,,]> arrayList;

		private string[,,] array;

		public PrivateStr3DVariableToken(UserDefinedVariableData data)
			: base(VariableCode.VARS3D, data)
		{
			_ = data.Lengths;
			base.IsStatic = false;
			arrayList = new List<string[,,]>();
		}

		public override void SetDefault()
		{
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override void SetValue(string value, long[] arguments)
		{
			array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			int length3 = array.GetLength(2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						array[i, j, k] = value;
					}
				}
			}
		}

		public override object GetArray()
		{
			return array;
		}

		public override void In()
		{
			if (array != null)
			{
				arrayList.Add(array);
			}
			array = new string[sizes[0], sizes[1], sizes[2]];
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
		}
	}

	private sealed class ReferenceInt1DToken : ReferenceToken
	{
		public ReferenceInt1DToken(UserDefinedVariableData data)
			: base(VariableCode.REF, data)
		{
			base.CanRestructure = false;
			base.IsStatic = !data.Private;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			return ((long[])array)[arguments[0]];
		}

		public override void SetValue(long value, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			((long[])array)[arguments[0]] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			int num = (int)arguments[0];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				((long[])array)[i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			for (int i = start; i < end; i++)
			{
				((long[])array)[i] = value;
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			((long[])array)[arguments[0]] += value;
			return ((long[])array)[arguments[0]];
		}
	}

	private sealed class ReferenceInt2DToken : ReferenceToken
	{
		public ReferenceInt2DToken(UserDefinedVariableData data)
			: base(VariableCode.REF2D, data)
		{
			base.CanRestructure = false;
			base.IsStatic = !data.Private;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			return ((long[,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])];
		}

		public override void SetValue(long value, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			((long[,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				((long[,])array)[(int)checked((nint)arguments[0]), i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					((long[,])array)[i, j] = value;
				}
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			((long[,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])] += value;
			return ((long[,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])];
		}
	}

	private sealed class ReferenceInt3DToken : ReferenceToken
	{
		public ReferenceInt3DToken(UserDefinedVariableData data)
			: base(VariableCode.REF3D, data)
		{
			base.CanRestructure = false;
			base.IsStatic = !data.Private;
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			return ((long[,,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override void SetValue(long value, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			((long[,,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				((long[,,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			int length3 = array.GetLength(2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						((long[,,])array)[i, j, k] = value;
					}
				}
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			((long[,,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] += value;
			return ((long[,,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}
	}

	private sealed class ReferenceStr1DToken : ReferenceToken
	{
		public ReferenceStr1DToken(UserDefinedVariableData data)
			: base(VariableCode.REFS, data)
		{
			base.CanRestructure = false;
			base.IsStatic = !data.Private;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			return ((string[])array)[arguments[0]];
		}

		public override void SetValue(string value, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			((string[])array)[arguments[0]] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			int num = (int)arguments[0];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				((string[])array)[i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			for (int i = start; i < end; i++)
			{
				((string[])array)[i] = value;
			}
		}
	}

	private sealed class ReferenceStr2DToken : ReferenceToken
	{
		public ReferenceStr2DToken(UserDefinedVariableData data)
			: base(VariableCode.REFS2D, data)
		{
			base.CanRestructure = false;
			base.IsStatic = !data.Private;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			return ((string[,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])];
		}

		public override void SetValue(string value, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			((string[,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1])] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				((string[,])array)[(int)checked((nint)arguments[0]), i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					((string[,])array)[i, j] = value;
				}
			}
		}
	}

	private sealed class ReferenceStr3DToken : ReferenceToken
	{
		public ReferenceStr3DToken(UserDefinedVariableData data)
			: base(VariableCode.REFS3D, data)
		{
			base.CanRestructure = false;
			base.IsStatic = !data.Private;
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			return ((string[,,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override void SetValue(string value, long[] arguments)
		{
			if (array == null)
			{
				throw new CodeEE("参照型変数" + varName + "は何も参照していません");
			}
			((string[,,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				((string[,,])array)[(int)checked((nint)arguments[0]), (int)checked((nint)arguments[1]), i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			int length3 = array.GetLength(2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						((string[,,])array)[i, j, k] = value;
					}
				}
			}
		}
	}

	private sealed class UserDefinedCharaInt1DVariableToken : UserDefinedCharaVariableToken
	{
		public UserDefinedCharaInt1DVariableToken(UserDefinedVariableData data, VariableData varData, int arrayIndex)
			: base(VariableCode.CVAR, data, varData, arrayIndex)
		{
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return ((long[])GetArrayChara((int)arguments[0]))[arguments[1]];
		}

		public override void SetValue(long value, long[] arguments)
		{
			((long[])GetArrayChara((int)arguments[0]))[arguments[1]] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			long[] array = (long[])GetArrayChara((int)arguments[0]);
			int num = (int)arguments[1];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			long[] array = (long[])GetArrayChara(charaPos);
			for (int i = start; i < end; i++)
			{
				array[i] = value;
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			long[] obj = (long[])GetArrayChara((int)arguments[0]);
			obj[arguments[1]] += value;
			return obj[arguments[1]];
		}
	}

	private sealed class UserDefinedCharaStr1DVariableToken : UserDefinedCharaVariableToken
	{
		public UserDefinedCharaStr1DVariableToken(UserDefinedVariableData data, VariableData varData, int arrayIndex)
			: base(VariableCode.CVARS, data, varData, arrayIndex)
		{
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return ((string[])GetArrayChara((int)arguments[0]))[arguments[1]];
		}

		public override void SetValue(string value, long[] arguments)
		{
			((string[])GetArrayChara((int)arguments[0]))[arguments[1]] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			string[] array = (string[])GetArrayChara((int)arguments[0]);
			int num = (int)arguments[1];
			int num2 = num + values.Length;
			for (int i = num; i < num2; i++)
			{
				array[i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			string[] array = (string[])GetArrayChara(charaPos);
			for (int i = start; i < end; i++)
			{
				array[i] = value;
			}
		}
	}

	private sealed class UserDefinedCharaInt2DVariableToken : UserDefinedCharaVariableToken
	{
		public UserDefinedCharaInt2DVariableToken(UserDefinedVariableData data, VariableData varData, int arrayIndex)
			: base(VariableCode.CVAR2D, data, varData, arrayIndex)
		{
		}

		public override long GetIntValue(ExpressionMediator exm, long[] arguments)
		{
			return ((long[,])GetArrayChara((int)arguments[0]))[(int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override void SetValue(long value, long[] arguments)
		{
			((long[,])GetArrayChara((int)arguments[0]))[(int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] = value;
		}

		public override void SetValue(long[] values, long[] arguments)
		{
			long[,] array = (long[,])GetArrayChara((int)arguments[0]);
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			int num3 = (int)arguments[1];
			for (int i = num; i < num2; i++)
			{
				array[num3, i] = values[i - num];
			}
		}

		public override void SetValueAll(long value, int start, int end, int charaPos)
		{
			long[,] array = (long[,])GetArrayChara(charaPos);
			int num = sizes[0];
			int num2 = sizes[1];
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					array[i, j] = value;
				}
			}
		}

		public override long PlusValue(long value, long[] arguments)
		{
			long[,] obj = (long[,])GetArrayChara((int)arguments[0]);
			obj[(int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] += value;
			return obj[(int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}
	}

	private sealed class UserDefinedCharaStr2DVariableToken : UserDefinedCharaVariableToken
	{
		public UserDefinedCharaStr2DVariableToken(UserDefinedVariableData data, VariableData varData, int arrayIndex)
			: base(VariableCode.CVARS2D, data, varData, arrayIndex)
		{
		}

		public override string GetStrValue(ExpressionMediator exm, long[] arguments)
		{
			return ((string[,])GetArrayChara((int)arguments[0]))[(int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])];
		}

		public override void SetValue(string value, long[] arguments)
		{
			((string[,])GetArrayChara((int)arguments[0]))[(int)checked((nint)arguments[1]), (int)checked((nint)arguments[2])] = value;
		}

		public override void SetValue(string[] values, long[] arguments)
		{
			string[,] array = (string[,])GetArrayChara((int)arguments[0]);
			int num = (int)arguments[2];
			int num2 = num + values.Length;
			int num3 = (int)arguments[1];
			for (int i = num; i < num2; i++)
			{
				array[num3, i] = values[i - num];
			}
		}

		public override void SetValueAll(string value, int start, int end, int charaPos)
		{
			string[,] array = (string[,])GetArrayChara(charaPos);
			int num = sizes[0];
			int num2 = sizes[1];
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					array[i, j] = value;
				}
			}
		}
	}

	private readonly long[] dataInteger;

	private readonly string[] dataString;

	private readonly long[][] dataIntegerArray;

	private readonly string[][] dataStringArray;

	private readonly long[][,] dataIntegerArray2D;

	private readonly string[][,] dataStringArray2D;

	private readonly long[][,,] dataIntegerArray3D;

	private readonly string[][,,] dataStringArray3D;

	private readonly List<CharacterData> characterList;

	private readonly GameBase gamebase;

	private readonly ConstantData constant;

	public long LastLoadVersion = -1L;

	public long LastLoadNo = -1L;

	public string LastLoadText = "";

	private Dictionary<string, VariableToken> varTokenDic = new Dictionary<string, VariableToken>();

	private Dictionary<string, VariableLocal> localvarTokenDic = new Dictionary<string, VariableLocal>();

	private List<UserDefinedVariableToken> userDefinedStaticVarList = new List<UserDefinedVariableToken>();

	private List<UserDefinedVariableToken> userDefinedGlobalVarList = new List<UserDefinedVariableToken>();

	private List<UserDefinedVariableToken>[] userDefinedSaveVarList = new List<UserDefinedVariableToken>[6];

	private List<UserDefinedVariableToken>[] userDefinedGlobalSaveVarList = new List<UserDefinedVariableToken>[6];

	public List<UserDefinedCharaVariableToken> UserDefinedCharaVarList = new List<UserDefinedCharaVariableToken>();

	private const int strCount = 0;

	private const int intCount = 0;

	private const int intArrayCount = 60;

	private const int strArrayCount = 1;

	public long[] DataInteger => dataInteger;

	public string[] DataString => dataString;

	public long[][] DataIntegerArray => dataIntegerArray;

	public string[][] DataStringArray => dataStringArray;

	public long[][,] DataIntegerArray2D => dataIntegerArray2D;

	public string[][,] DataStringArray2D => dataStringArray2D;

	public long[][,,] DataIntegerArray3D => dataIntegerArray3D;

	public string[][,,] DataStringArray3D => dataStringArray3D;

	public List<CharacterData> CharacterList => characterList;

	internal GameBase GameBase => gamebase;

	internal ConstantData Constant => constant;

	public VariableData(GameBase gamebase, ConstantData constant)
	{
		this.gamebase = gamebase;
		this.constant = constant;
		characterList = new List<CharacterData>();
		dataInteger = new long[0];
		dataIntegerArray = new long[65][];
		for (int i = 0; i < dataIntegerArray.Length; i++)
		{
			dataIntegerArray[i] = new long[constant.VariableIntArrayLength[i]];
		}
		dataString = new string[1];
		dataStringArray = new string[7][];
		for (int j = 0; j < dataStringArray.Length; j++)
		{
			dataStringArray[j] = new string[constant.VariableStrArrayLength[j]];
		}
		dataIntegerArray2D = new long[6][,];
		for (int k = 0; k < dataIntegerArray2D.Length; k++)
		{
			long num = constant.VariableIntArray2DLength[k];
			int num2 = (int)(num >> 32);
			int num3 = (int)(num & 0x7FFFFFFF);
			dataIntegerArray2D[k] = new long[num2, num3];
		}
		dataStringArray2D = new string[0][,];
		for (int l = 0; l < dataStringArray2D.Length; l++)
		{
			long num4 = constant.VariableStrArray2DLength[l];
			int num5 = (int)(num4 >> 32);
			int num6 = (int)(num4 & 0x7FFFFFFF);
			dataStringArray2D[l] = new string[num5, num6];
		}
		dataIntegerArray3D = new long[2][,,];
		for (int m = 0; m < dataIntegerArray3D.Length; m++)
		{
			long num7 = constant.VariableIntArray3DLength[m];
			int num8 = (int)(num7 >> 40);
			int num9 = (int)((num7 >> 20) & 0xFFFFF);
			int num10 = (int)(num7 & 0xFFFFF);
			dataIntegerArray3D[m] = new long[num8, num9, num10];
		}
		dataStringArray3D = new string[0][,,];
		for (int n = 0; n < dataStringArray3D.Length; n++)
		{
			long num11 = constant.VariableStrArray3DLength[n];
			int num12 = (int)(num11 >> 40);
			int num13 = (int)((num11 >> 20) & 0xFFFFF);
			int num14 = (int)(num11 & 0xFFFFF);
			dataStringArray3D[n] = new string[num12, num13, num14];
		}
		for (int num15 = 0; num15 < 6; num15++)
		{
			userDefinedSaveVarList[num15] = new List<UserDefinedVariableToken>();
			userDefinedGlobalSaveVarList[num15] = new List<UserDefinedVariableToken>();
		}
		SetDefaultValue(constant);
		varTokenDic.Add("DAY", new Int1DVariableToken(VariableCode.DAY, this));
		varTokenDic.Add("MONEY", new Int1DVariableToken(VariableCode.MONEY, this));
		varTokenDic.Add("ITEM", new Int1DVariableToken(VariableCode.ITEM, this));
		varTokenDic.Add("FLAG", new Int1DVariableToken(VariableCode.FLAG, this));
		varTokenDic.Add("TFLAG", new Int1DVariableToken(VariableCode.TFLAG, this));
		varTokenDic.Add("UP", new Int1DVariableToken(VariableCode.UP, this));
		varTokenDic.Add("PALAMLV", new Int1DVariableToken(VariableCode.PALAMLV, this));
		varTokenDic.Add("EXPLV", new Int1DVariableToken(VariableCode.EXPLV, this));
		varTokenDic.Add("EJAC", new Int1DVariableToken(VariableCode.EJAC, this));
		varTokenDic.Add("DOWN", new Int1DVariableToken(VariableCode.DOWN, this));
		varTokenDic.Add("RESULT", new Int1DVariableToken(VariableCode.RESULT, this));
		varTokenDic.Add("COUNT", new Int1DVariableToken(VariableCode.COUNT, this));
		varTokenDic.Add("TARGET", new Int1DVariableToken(VariableCode.TARGET, this));
		varTokenDic.Add("ASSI", new Int1DVariableToken(VariableCode.ASSI, this));
		varTokenDic.Add("MASTER", new Int1DVariableToken(VariableCode.MASTER, this));
		varTokenDic.Add("NOITEM", new Int1DVariableToken(VariableCode.NOITEM, this));
		varTokenDic.Add("LOSEBASE", new Int1DVariableToken(VariableCode.LOSEBASE, this));
		varTokenDic.Add("SELECTCOM", new Int1DVariableToken(VariableCode.SELECTCOM, this));
		varTokenDic.Add("ASSIPLAY", new Int1DVariableToken(VariableCode.ASSIPLAY, this));
		varTokenDic.Add("PREVCOM", new Int1DVariableToken(VariableCode.PREVCOM, this));
		varTokenDic.Add("TIME", new Int1DVariableToken(VariableCode.TIME, this));
		varTokenDic.Add("ITEMSALES", new Int1DVariableToken(VariableCode.ITEMSALES, this));
		varTokenDic.Add("PLAYER", new Int1DVariableToken(VariableCode.PLAYER, this));
		varTokenDic.Add("NEXTCOM", new Int1DVariableToken(VariableCode.NEXTCOM, this));
		varTokenDic.Add("PBAND", new Int1DVariableToken(VariableCode.PBAND, this));
		varTokenDic.Add("BOUGHT", new Int1DVariableToken(VariableCode.BOUGHT, this));
		varTokenDic.Add("A", new Int1DVariableToken(VariableCode.A, this));
		varTokenDic.Add("B", new Int1DVariableToken(VariableCode.B, this));
		varTokenDic.Add("C", new Int1DVariableToken(VariableCode.C, this));
		varTokenDic.Add("D", new Int1DVariableToken(VariableCode.D, this));
		varTokenDic.Add("E", new Int1DVariableToken(VariableCode.E, this));
		varTokenDic.Add("F", new Int1DVariableToken(VariableCode.F, this));
		varTokenDic.Add("G", new Int1DVariableToken(VariableCode.G, this));
		varTokenDic.Add("H", new Int1DVariableToken(VariableCode.H, this));
		varTokenDic.Add("I", new Int1DVariableToken(VariableCode.I, this));
		varTokenDic.Add("J", new Int1DVariableToken(VariableCode.J, this));
		varTokenDic.Add("K", new Int1DVariableToken(VariableCode.K, this));
		varTokenDic.Add("L", new Int1DVariableToken(VariableCode.L, this));
		varTokenDic.Add("M", new Int1DVariableToken(VariableCode.M, this));
		varTokenDic.Add("N", new Int1DVariableToken(VariableCode.N, this));
		varTokenDic.Add("O", new Int1DVariableToken(VariableCode.O, this));
		varTokenDic.Add("P", new Int1DVariableToken(VariableCode.P, this));
		varTokenDic.Add("Q", new Int1DVariableToken(VariableCode.Q, this));
		varTokenDic.Add("R", new Int1DVariableToken(VariableCode.R, this));
		varTokenDic.Add("S", new Int1DVariableToken(VariableCode.S, this));
		varTokenDic.Add("T", new Int1DVariableToken(VariableCode.T, this));
		varTokenDic.Add("U", new Int1DVariableToken(VariableCode.U, this));
		varTokenDic.Add("V", new Int1DVariableToken(VariableCode.V, this));
		varTokenDic.Add("W", new Int1DVariableToken(VariableCode.W, this));
		varTokenDic.Add("X", new Int1DVariableToken(VariableCode.X, this));
		varTokenDic.Add("Y", new Int1DVariableToken(VariableCode.Y, this));
		varTokenDic.Add("Z", new Int1DVariableToken(VariableCode.Z, this));
		varTokenDic.Add("GLOBAL", new Int1DVariableToken(VariableCode.GLOBAL, this));
		varTokenDic.Add("RANDDATA", new Int1DVariableToken(VariableCode.RANDDATA, this));
		varTokenDic.Add("SAVESTR", new Str1DVariableToken(VariableCode.SAVESTR, this));
		varTokenDic.Add("TSTR", new Str1DVariableToken(VariableCode.TSTR, this));
		varTokenDic.Add("STR", new Str1DVariableToken(VariableCode.STR, this));
		varTokenDic.Add("RESULTS", new Str1DVariableToken(VariableCode.RESULTS, this));
		varTokenDic.Add("GLOBALS", new Str1DVariableToken(VariableCode.GLOBALS, this));
		varTokenDic.Add("SAVEDATA_TEXT", new StrVariableToken(VariableCode.SAVEDATA_TEXT, this));
		varTokenDic.Add("ISASSI", new CharaIntVariableToken(VariableCode.ISASSI, this));
		varTokenDic.Add("NO", new CharaIntVariableToken(VariableCode.NO, this));
		varTokenDic.Add("BASE", new CharaInt1DVariableToken(VariableCode.BASE, this));
		varTokenDic.Add("MAXBASE", new CharaInt1DVariableToken(VariableCode.MAXBASE, this));
		varTokenDic.Add("ABL", new CharaInt1DVariableToken(VariableCode.ABL, this));
		varTokenDic.Add("TALENT", new CharaInt1DVariableToken(VariableCode.TALENT, this));
		varTokenDic.Add("EXP", new CharaInt1DVariableToken(VariableCode.EXP, this));
		varTokenDic.Add("MARK", new CharaInt1DVariableToken(VariableCode.MARK, this));
		varTokenDic.Add("PALAM", new CharaInt1DVariableToken(VariableCode.PALAM, this));
		varTokenDic.Add("SOURCE", new CharaInt1DVariableToken(VariableCode.SOURCE, this));
		varTokenDic.Add("EX", new CharaInt1DVariableToken(VariableCode.EX, this));
		varTokenDic.Add("CFLAG", new CharaInt1DVariableToken(VariableCode.CFLAG, this));
		varTokenDic.Add("JUEL", new CharaInt1DVariableToken(VariableCode.JUEL, this));
		varTokenDic.Add("RELATION", new CharaInt1DVariableToken(VariableCode.RELATION, this));
		varTokenDic.Add("EQUIP", new CharaInt1DVariableToken(VariableCode.EQUIP, this));
		varTokenDic.Add("TEQUIP", new CharaInt1DVariableToken(VariableCode.TEQUIP, this));
		varTokenDic.Add("STAIN", new CharaInt1DVariableToken(VariableCode.STAIN, this));
		varTokenDic.Add("GOTJUEL", new CharaInt1DVariableToken(VariableCode.GOTJUEL, this));
		varTokenDic.Add("NOWEX", new CharaInt1DVariableToken(VariableCode.NOWEX, this));
		varTokenDic.Add("DOWNBASE", new CharaInt1DVariableToken(VariableCode.DOWNBASE, this));
		varTokenDic.Add("CUP", new CharaInt1DVariableToken(VariableCode.CUP, this));
		varTokenDic.Add("CDOWN", new CharaInt1DVariableToken(VariableCode.CDOWN, this));
		varTokenDic.Add("TCVAR", new CharaInt1DVariableToken(VariableCode.TCVAR, this));
		varTokenDic.Add("NAME", new CharaStrVariableToken(VariableCode.NAME, this));
		varTokenDic.Add("CALLNAME", new CharaStrVariableToken(VariableCode.CALLNAME, this));
		varTokenDic.Add("NICKNAME", new CharaStrVariableToken(VariableCode.NICKNAME, this));
		varTokenDic.Add("MASTERNAME", new CharaStrVariableToken(VariableCode.MASTERNAME, this));
		varTokenDic.Add("CSTR", new CharaStr1DVariableToken(VariableCode.CSTR, this));
		varTokenDic.Add("CDFLAG", new CharaInt2DVariableToken(VariableCode.CDFLAG, this));
		varTokenDic.Add("DITEMTYPE", new Int2DVariableToken(VariableCode.DITEMTYPE, this));
		varTokenDic.Add("DA", new Int2DVariableToken(VariableCode.DA, this));
		varTokenDic.Add("DB", new Int2DVariableToken(VariableCode.DB, this));
		varTokenDic.Add("DC", new Int2DVariableToken(VariableCode.DC, this));
		varTokenDic.Add("DD", new Int2DVariableToken(VariableCode.DD, this));
		varTokenDic.Add("DE", new Int2DVariableToken(VariableCode.DE, this));
		varTokenDic.Add("TA", new Int3DVariableToken(VariableCode.TA, this));
		varTokenDic.Add("TB", new Int3DVariableToken(VariableCode.TB, this));
		varTokenDic.Add("ITEMPRICE", new Int1DConstantToken(VariableCode.ITEMPRICE, this, constant.ItemPrice));
		varTokenDic.Add("ABLNAME", new Str1DConstantToken(VariableCode.ABLNAME, this));
		varTokenDic.Add("TALENTNAME", new Str1DConstantToken(VariableCode.TALENTNAME, this));
		varTokenDic.Add("EXPNAME", new Str1DConstantToken(VariableCode.EXPNAME, this));
		varTokenDic.Add("MARKNAME", new Str1DConstantToken(VariableCode.MARKNAME, this));
		varTokenDic.Add("PALAMNAME", new Str1DConstantToken(VariableCode.PALAMNAME, this));
		varTokenDic.Add("ITEMNAME", new Str1DConstantToken(VariableCode.ITEMNAME, this));
		varTokenDic.Add("TRAINNAME", new Str1DConstantToken(VariableCode.TRAINNAME, this));
		varTokenDic.Add("BASENAME", new Str1DConstantToken(VariableCode.BASENAME, this));
		varTokenDic.Add("SOURCENAME", new Str1DConstantToken(VariableCode.SOURCENAME, this));
		varTokenDic.Add("EXNAME", new Str1DConstantToken(VariableCode.EXNAME, this));
		varTokenDic.Add("EQUIPNAME", new Str1DConstantToken(VariableCode.EQUIPNAME, this));
		varTokenDic.Add("TEQUIPNAME", new Str1DConstantToken(VariableCode.TEQUIPNAME, this));
		varTokenDic.Add("FLAGNAME", new Str1DConstantToken(VariableCode.FLAGNAME, this));
		varTokenDic.Add("TFLAGNAME", new Str1DConstantToken(VariableCode.TFLAGNAME, this));
		varTokenDic.Add("CFLAGNAME", new Str1DConstantToken(VariableCode.CFLAGNAME, this));
		varTokenDic.Add("TCVARNAME", new Str1DConstantToken(VariableCode.TCVARNAME, this));
		varTokenDic.Add("CSTRNAME", new Str1DConstantToken(VariableCode.CSTRNAME, this));
		varTokenDic.Add("STAINNAME", new Str1DConstantToken(VariableCode.STAINNAME, this));
		varTokenDic.Add("CDFLAGNAME1", new Str1DConstantToken(VariableCode.CDFLAGNAME1, this));
		varTokenDic.Add("CDFLAGNAME2", new Str1DConstantToken(VariableCode.CDFLAGNAME2, this));
		varTokenDic.Add("STRNAME", new Str1DConstantToken(VariableCode.STRNAME, this));
		varTokenDic.Add("TSTRNAME", new Str1DConstantToken(VariableCode.TSTRNAME, this));
		varTokenDic.Add("SAVESTRNAME", new Str1DConstantToken(VariableCode.SAVESTRNAME, this));
		varTokenDic.Add("GLOBALNAME", new Str1DConstantToken(VariableCode.GLOBALNAME, this));
		varTokenDic.Add("GLOBALSNAME", new Str1DConstantToken(VariableCode.GLOBALSNAME, this));
		StrConstantToken value = new StrConstantToken(VariableCode.GAMEBASE_AUTHOR, this, gamebase.ScriptAutherName);
		varTokenDic.Add("GAMEBASE_AUTHER", value);
		varTokenDic.Add("GAMEBASE_AUTHOR", value);
		varTokenDic.Add("GAMEBASE_INFO", new StrConstantToken(VariableCode.GAMEBASE_INFO, this, gamebase.ScriptDetail));
		varTokenDic.Add("GAMEBASE_YEAR", new StrConstantToken(VariableCode.GAMEBASE_YEAR, this, gamebase.ScriptYear));
		varTokenDic.Add("GAMEBASE_TITLE", new StrConstantToken(VariableCode.GAMEBASE_TITLE, this, gamebase.ScriptTitle));
		varTokenDic.Add("GAMEBASE_GAMECODE", new IntConstantToken(VariableCode.GAMEBASE_GAMECODE, this, gamebase.ScriptUniqueCode));
		varTokenDic.Add("GAMEBASE_VERSION", new IntConstantToken(VariableCode.GAMEBASE_VERSION, this, gamebase.ScriptVersion));
		varTokenDic.Add("GAMEBASE_ALLOWVERSION", new IntConstantToken(VariableCode.GAMEBASE_ALLOWVERSION, this, gamebase.ScriptCompatibleMinVersion));
		varTokenDic.Add("GAMEBASE_DEFAULTCHARA", new IntConstantToken(VariableCode.GAMEBASE_DEFAULTCHARA, this, gamebase.DefaultCharacter));
		varTokenDic.Add("GAMEBASE_NOITEM", new IntConstantToken(VariableCode.GAMEBASE_NOITEM, this, gamebase.DefaultNoItem));
		VariableToken variableToken = null;
		variableToken = ((!Config.CompatiRAND) ? ((PseudoVariableToken)new RandToken(VariableCode.RAND, this)) : ((PseudoVariableToken)new CompatiRandToken(VariableCode.RAND, this)));
		varTokenDic.Add("RAND", variableToken);
		varTokenDic.Add("CHARANUM", new CHARANUM_Token(VariableCode.CHARANUM, this));
		varTokenDic.Add("LASTLOAD_TEXT", new LASTLOAD_TEXT_Token(VariableCode.LASTLOAD_TEXT, this));
		varTokenDic.Add("LASTLOAD_VERSION", new LASTLOAD_VERSION_Token(VariableCode.LASTLOAD_VERSION, this));
		varTokenDic.Add("LASTLOAD_NO", new LASTLOAD_NO_Token(VariableCode.LASTLOAD_NO, this));
		varTokenDic.Add("LINECOUNT", new LINECOUNT_Token(VariableCode.LINECOUNT, this));
		varTokenDic.Add("ISTIMEOUT", new ISTIMEOUTToken(VariableCode.ISTIMEOUT, this));
		varTokenDic.Add("__INT_MAX__", new __INT_MAX__Token(VariableCode.__INT_MAX__, this));
		varTokenDic.Add("__INT_MIN__", new __INT_MIN__Token(VariableCode.__INT_MIN__, this));
		varTokenDic.Add("EMUERA_VERSION", new EMUERA_VERSIONToken(VariableCode.EMUERA_VERSION, this));
		varTokenDic.Add("WINDOW_TITLE", new WINDOW_TITLE_Token(VariableCode.WINDOW_TITLE, this));
		varTokenDic.Add("MONEYLABEL", new MONEYLABEL_Token(VariableCode.MONEYLABEL, this));
		varTokenDic.Add("DRAWLINESTR", new DRAWLINESTR_Token(VariableCode.DRAWLINESTR, this));
		if (!Program.DebugMode)
		{
			varTokenDic.Add("__FILE__", new EmptyStrToken(VariableCode.__FILE__, this));
			varTokenDic.Add("__FUNCTION__", new EmptyStrToken(VariableCode.__FUNCTION__, this));
			varTokenDic.Add("__LINE__", new EmptyIntToken(VariableCode.__LINE__, this));
		}
		else
		{
			varTokenDic.Add("__FILE__", new Debug__FILE__Token(VariableCode.__FILE__, this));
			varTokenDic.Add("__FUNCTION__", new Debug__FUNCTION__Token(VariableCode.__FUNCTION__, this));
			varTokenDic.Add("__LINE__", new Debug__LINE__Token(VariableCode.__LINE__, this));
		}
		int size = constant.VariableIntArrayLength[61];
		localvarTokenDic.Add("LOCAL", new VariableLocal(VariableCode.LOCAL, size, CreateLocalInt));
		size = constant.VariableIntArrayLength[62];
		localvarTokenDic.Add("ARG", new VariableLocal(VariableCode.ARG, size, CreateLocalInt));
		size = constant.VariableStrArrayLength[3];
		localvarTokenDic.Add("LOCALS", new VariableLocal(VariableCode.LOCALS, size, CreateLocalStr));
		size = constant.VariableStrArrayLength[4];
		localvarTokenDic.Add("ARGS", new VariableLocal(VariableCode.ARGS, size, CreateLocalStr));
	}

	private LocalVariableToken CreateLocalInt(VariableCode varCode, string subKey, int size)
	{
		return new LocalInt1DVariableToken(varCode, this, subKey, size);
	}

	private LocalVariableToken CreateLocalStr(VariableCode varCode, string subKey, int size)
	{
		return new LocalStr1DVariableToken(varCode, this, subKey, size);
	}

	public Dictionary<string, VariableToken> GetVarTokenDicClone()
	{
		Dictionary<string, VariableToken> dictionary = new Dictionary<string, VariableToken>();
		foreach (KeyValuePair<string, VariableToken> item in varTokenDic)
		{
			dictionary.Add(item.Key, item.Value);
		}
		return dictionary;
	}

	public Dictionary<string, VariableToken> GetVarTokenDic()
	{
		return varTokenDic;
	}

	public Dictionary<string, VariableLocal> GetLocalvarTokenDic()
	{
		return localvarTokenDic;
	}

	public VariableToken GetSystemVariableToken(string str)
	{
		return varTokenDic[str];
	}

	public UserDefinedCharaVariableToken CreateUserDefCharaVariable(UserDefinedVariableData data)
	{
		UserDefinedCharaVariableToken userDefinedCharaVariableToken = null;
		if (data.CharaData)
		{
			int count = UserDefinedCharaVarList.Count;
			userDefinedCharaVariableToken = (data.TypeIsStr ? (data.Dimension switch
			{
				1 => new UserDefinedCharaStr1DVariableToken(data, this, count), 
				2 => new UserDefinedCharaStr2DVariableToken(data, this, count), 
				_ => throw new ExeEE("異常な変数宣言"), 
			}) : (data.Dimension switch
			{
				1 => new UserDefinedCharaInt1DVariableToken(data, this, count), 
				2 => new UserDefinedCharaInt2DVariableToken(data, this, count), 
				_ => throw new ExeEE("異常な変数宣言"), 
			}));
		}
		UserDefinedCharaVarList.Add(userDefinedCharaVariableToken);
		return userDefinedCharaVariableToken;
	}

	public UserDefinedVariableToken CreateUserDefVariable(UserDefinedVariableData data)
	{
		UserDefinedVariableToken userDefinedVariableToken = null;
		userDefinedVariableToken = (data.TypeIsStr ? (data.Dimension switch
		{
			1 => new StaticStr1DVariableToken(data), 
			2 => new StaticStr2DVariableToken(data), 
			3 => new StaticStr3DVariableToken(data), 
			_ => throw new ExeEE("異常な変数宣言"), 
		}) : (data.Dimension switch
		{
			1 => new StaticInt1DVariableToken(data), 
			2 => new StaticInt2DVariableToken(data), 
			3 => new StaticInt3DVariableToken(data), 
			_ => throw new ExeEE("異常な変数宣言"), 
		}));
		if (userDefinedVariableToken.IsGlobal)
		{
			userDefinedGlobalVarList.Add(userDefinedVariableToken);
		}
		else
		{
			userDefinedStaticVarList.Add(userDefinedVariableToken);
		}
		if (userDefinedVariableToken.IsSavedata)
		{
			int num = userDefinedVariableToken.Dimension * 2 - 2;
			if (!userDefinedVariableToken.IsString)
			{
				num++;
			}
			if (userDefinedVariableToken.IsGlobal)
			{
				userDefinedGlobalSaveVarList[num].Add(userDefinedVariableToken);
			}
			else
			{
				userDefinedSaveVarList[num].Add(userDefinedVariableToken);
			}
		}
		return userDefinedVariableToken;
	}

	public UserDefinedVariableToken CreatePrivateVariable(UserDefinedVariableData data)
	{
		UserDefinedVariableToken userDefinedVariableToken = null;
		if (data.Reference)
		{
			userDefinedVariableToken = (data.TypeIsStr ? (data.Dimension switch
			{
				1 => new ReferenceStr1DToken(data), 
				2 => new ReferenceStr2DToken(data), 
				3 => new ReferenceStr3DToken(data), 
				_ => throw new ExeEE("異常な変数宣言"), 
			}) : (data.Dimension switch
			{
				1 => new ReferenceInt1DToken(data), 
				2 => new ReferenceInt2DToken(data), 
				3 => new ReferenceInt3DToken(data), 
				_ => throw new ExeEE("異常な変数宣言"), 
			}));
		}
		else if (!data.Static)
		{
			userDefinedVariableToken = (data.TypeIsStr ? (data.Dimension switch
			{
				1 => new PrivateStr1DVariableToken(data), 
				2 => new PrivateStr2DVariableToken(data), 
				3 => new PrivateStr3DVariableToken(data), 
				_ => throw new ExeEE("異常な変数宣言"), 
			}) : (data.Dimension switch
			{
				1 => new PrivateInt1DVariableToken(data), 
				2 => new PrivateInt2DVariableToken(data), 
				3 => new PrivateInt3DVariableToken(data), 
				_ => throw new ExeEE("異常な変数宣言"), 
			}));
		}
		else
		{
			userDefinedVariableToken = (data.TypeIsStr ? (data.Dimension switch
			{
				1 => new StaticStr1DVariableToken(data), 
				2 => new StaticStr2DVariableToken(data), 
				3 => new StaticStr3DVariableToken(data), 
				_ => throw new ExeEE("異常な変数宣言"), 
			}) : (data.Dimension switch
			{
				1 => new StaticInt1DVariableToken(data), 
				2 => new StaticInt2DVariableToken(data), 
				3 => new StaticInt3DVariableToken(data), 
				_ => throw new ExeEE("異常な変数宣言"), 
			}));
			userDefinedStaticVarList.Add(userDefinedVariableToken);
		}
		return userDefinedVariableToken;
	}

	public void SetDefaultGlobalValue()
	{
		long[] array = dataIntegerArray[63];
		string[] array2 = dataStringArray[5];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 0L;
		}
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = null;
		}
		foreach (UserDefinedVariableToken userDefinedGlobalVar in userDefinedGlobalVarList)
		{
			userDefinedGlobalVar.SetDefault();
		}
	}

	public void SetDefaultLocalValue()
	{
		foreach (VariableLocal value in localvarTokenDic.Values)
		{
			value.SetDefault();
		}
		foreach (UserDefinedVariableToken userDefinedStaticVar in userDefinedStaticVarList)
		{
			userDefinedStaticVar.SetDefault();
		}
	}

	public void ClearLocalValue()
	{
		foreach (VariableLocal value in localvarTokenDic.Values)
		{
			value.Clear();
		}
	}

	public void SetDefaultValue(ConstantData constant)
	{
		for (int i = 0; i < dataInteger.Length; i++)
		{
			dataInteger[i] = 0L;
		}
		for (int j = 0; j < dataIntegerArray.Length; j++)
		{
			switch (j)
			{
			case 60:
				Buffer.BlockCopy(constant.ItemPrice, 0, dataIntegerArray[j], 0, 8 * dataIntegerArray[j].Length);
				continue;
			case 63:
				continue;
			}
			for (int k = 0; k < dataIntegerArray[j].Length; k++)
			{
				dataIntegerArray[j][k] = 0L;
			}
		}
		for (int l = 0; l < dataString.Length; l++)
		{
			dataString[l] = null;
		}
		for (int m = 0; m < dataStringArray.Length; m++)
		{
			switch (m)
			{
			case 1:
				constant.GetCsvNameList(VariableCode.__DUMMY_STR__).CopyTo(dataStringArray[m], 0);
				continue;
			case 5:
				continue;
			}
			for (int n = 0; n < dataStringArray[m].Length; n++)
			{
				dataStringArray[m][n] = null;
			}
		}
		for (int num = 0; num < dataIntegerArray2D.Length; num++)
		{
			long[,] array = dataIntegerArray2D[num];
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			for (int num2 = 0; num2 < length; num2++)
			{
				for (int num3 = 0; num3 < length2; num3++)
				{
					array[num2, num3] = 0L;
				}
			}
		}
		for (int num4 = 0; num4 < dataStringArray2D.Length; num4++)
		{
			string[,] array2 = dataStringArray2D[num4];
			int length3 = array2.GetLength(0);
			int length4 = array2.GetLength(1);
			for (int num5 = 0; num5 < length3; num5++)
			{
				for (int num6 = 0; num6 < length4; num6++)
				{
					array2[num5, num6] = null;
				}
			}
		}
		for (int num7 = 0; num7 < dataIntegerArray3D.Length; num7++)
		{
			long[,,] array3 = dataIntegerArray3D[num7];
			int length5 = array3.GetLength(0);
			int length6 = array3.GetLength(1);
			int length7 = array3.GetLength(2);
			for (int num8 = 0; num8 < length5; num8++)
			{
				for (int num9 = 0; num9 < length6; num9++)
				{
					for (int num10 = 0; num10 < length7; num10++)
					{
						array3[num8, num9, num10] = 0L;
					}
				}
			}
		}
		for (int num11 = 0; num11 < dataStringArray3D.Length; num11++)
		{
			string[,,] array4 = dataStringArray3D[num11];
			int length8 = array4.GetLength(0);
			int length9 = array4.GetLength(1);
			int length10 = array4.GetLength(2);
			for (int num12 = 0; num12 < length8; num12++)
			{
				for (int num13 = 0; num13 < length9; num13++)
				{
					for (int num14 = 0; num14 < length10; num14++)
					{
						array4[num12, num13, num14] = null;
					}
				}
			}
		}
		long[] array5 = dataIntegerArray[6];
		List<long> palamLvDef = Config.PalamLvDef;
		palamLvDef.CopyTo(0, array5, 0, Math.Min(array5.Length, palamLvDef.Count));
		long[] array6 = dataIntegerArray[7];
		List<long> expLvDef = Config.ExpLvDef;
		expLvDef.CopyTo(0, array6, 0, Math.Min(array6.Length, expLvDef.Count));
		long[] array7 = dataIntegerArray[13];
		if (array7.Length != 0)
		{
			array7[0] = -1L;
		}
		array7 = dataIntegerArray[12];
		if (array7.Length != 0)
		{
			array7[0] = 1L;
		}
		array7 = dataIntegerArray[26];
		if (array7.Length != 0)
		{
			array7[0] = Config.PbandDef;
		}
		array7 = dataIntegerArray[8];
		if (array7.Length != 0)
		{
			array7[0] = 10000L;
		}
		LastLoadVersion = -1L;
		LastLoadNo = -1L;
		LastLoadText = "";
	}

	public void SaveToStream(EraDataWriter writer)
	{
		for (int i = 0; i < 0; i++)
		{
			writer.Write(dataString[i]);
		}
		for (int j = 0; j < 0; j++)
		{
			writer.Write(dataInteger[j]);
		}
		for (int k = 0; k < 60; k++)
		{
			writer.Write(dataIntegerArray[k]);
		}
		for (int l = 0; l < 1; l++)
		{
			writer.Write(dataStringArray[l]);
		}
	}

	public void LoadFromStream(EraDataReader reader)
	{
		for (int i = 0; i < 0; i++)
		{
			dataString[i] = reader.ReadString();
		}
		for (int j = 0; j < 0; j++)
		{
			dataInteger[j] = reader.ReadInt64();
		}
		for (int k = 0; k < 60; k++)
		{
			reader.ReadInt64Array(dataIntegerArray[k]);
		}
		for (int l = 0; l < 1; l++)
		{
			reader.ReadStringArray(dataStringArray[l]);
		}
	}

	public void SaveToStreamExtended(EraDataWriter writer)
	{
		foreach (VariableCode extSave in VariableIdentifier.GetExtSaveList(VariableCode.__STRING__))
		{
			writer.WriteExtended(extSave.ToString(), dataString[(int)(VariableCode.__LOWERCASE__ & extSave)]);
		}
		writer.EmuSeparete();
		foreach (VariableCode extSave2 in VariableIdentifier.GetExtSaveList(VariableCode.__INTEGER__))
		{
			writer.WriteExtended(extSave2.ToString(), dataInteger[(int)(VariableCode.__LOWERCASE__ & extSave2)]);
		}
		writer.EmuSeparete();
		foreach (VariableCode extSave3 in VariableIdentifier.GetExtSaveList((VariableCode)786432))
		{
			writer.WriteExtended(extSave3.ToString(), dataStringArray[(int)(VariableCode.__LOWERCASE__ & extSave3)]);
		}
		writer.EmuSeparete();
		foreach (VariableCode extSave4 in VariableIdentifier.GetExtSaveList((VariableCode)655360))
		{
			writer.WriteExtended(extSave4.ToString(), dataIntegerArray[(int)(VariableCode.__LOWERCASE__ & extSave4)]);
		}
		writer.EmuSeparete();
		foreach (VariableCode extSave5 in VariableIdentifier.GetExtSaveList((VariableCode)134479872))
		{
			writer.WriteExtended(extSave5.ToString(), dataStringArray2D[(int)(VariableCode.__LOWERCASE__ & extSave5)]);
		}
		writer.EmuSeparete();
		foreach (VariableCode extSave6 in VariableIdentifier.GetExtSaveList((VariableCode)134348800))
		{
			writer.WriteExtended(extSave6.ToString(), dataIntegerArray2D[(int)(VariableCode.__LOWERCASE__ & extSave6)]);
		}
		writer.EmuSeparete();
		foreach (VariableCode extSave7 in VariableIdentifier.GetExtSaveList((VariableCode)537133056))
		{
			writer.WriteExtended(extSave7.ToString(), dataStringArray3D[(int)(VariableCode.__LOWERCASE__ & extSave7)]);
		}
		writer.EmuSeparete();
		foreach (VariableCode extSave8 in VariableIdentifier.GetExtSaveList((VariableCode)537001984))
		{
			writer.WriteExtended(extSave8.ToString(), dataIntegerArray3D[(int)(VariableCode.__LOWERCASE__ & extSave8)]);
		}
		writer.EmuSeparete();
		for (int i = 0; i < 6; i++)
		{
			foreach (UserDefinedVariableToken item in userDefinedSaveVarList[i])
			{
				switch (i)
				{
				case 0:
					writer.WriteExtended(item.Name, (string[])item.GetArray());
					break;
				case 1:
					writer.WriteExtended(item.Name, (long[])item.GetArray());
					break;
				case 2:
					writer.WriteExtended(item.Name, (string[,])item.GetArray());
					break;
				case 3:
					writer.WriteExtended(item.Name, (long[,])item.GetArray());
					break;
				case 4:
					writer.WriteExtended(item.Name, (string[,,])item.GetArray());
					break;
				case 5:
					writer.WriteExtended(item.Name, (long[,,])item.GetArray());
					break;
				}
			}
			writer.EmuSeparete();
		}
	}

	public void LoadFromStreamExtended(EraDataReader reader, int version)
	{
		Dictionary<string, string> dictionary = reader.ReadStringExtended();
		Dictionary<string, long> dictionary2 = reader.ReadInt64Extended();
		Dictionary<string, List<string>> dictionary3 = reader.ReadStringArrayExtended();
		Dictionary<string, List<long>> dictionary4 = reader.ReadInt64ArrayExtended();
		Dictionary<string, List<string[]>> dictionary5 = reader.ReadStringArray2DExtended();
		Dictionary<string, List<long[]>> dictionary6 = reader.ReadInt64Array2DExtended();
		Dictionary<string, List<List<string[]>>> dictionary7 = reader.ReadStringArray3DExtended();
		Dictionary<string, List<List<long[]>>> dictionary8 = reader.ReadInt64Array3DExtended();
		foreach (VariableCode extSave in VariableIdentifier.GetExtSaveList(VariableCode.__STRING__))
		{
			if (dictionary.ContainsKey(extSave.ToString()))
			{
				dataString[(int)(VariableCode.__LOWERCASE__ & extSave)] = dictionary[extSave.ToString()];
			}
		}
		foreach (VariableCode extSave2 in VariableIdentifier.GetExtSaveList(VariableCode.__INTEGER__))
		{
			if (dictionary2.ContainsKey(extSave2.ToString()))
			{
				dataInteger[(int)(VariableCode.__LOWERCASE__ & extSave2)] = dictionary2[extSave2.ToString()];
			}
		}
		foreach (VariableCode extSave3 in VariableIdentifier.GetExtSaveList((VariableCode)786432))
		{
			if (dictionary3.ContainsKey(extSave3.ToString()))
			{
				copyListToArray(dictionary3[extSave3.ToString()], dataStringArray[(int)(VariableCode.__LOWERCASE__ & extSave3)]);
			}
		}
		foreach (VariableCode extSave4 in VariableIdentifier.GetExtSaveList((VariableCode)655360))
		{
			if (dictionary4.ContainsKey(extSave4.ToString()))
			{
				copyListToArray(dictionary4[extSave4.ToString()], dataIntegerArray[(int)(VariableCode.__LOWERCASE__ & extSave4)]);
			}
		}
		foreach (VariableCode extSave5 in VariableIdentifier.GetExtSaveList((VariableCode)134479872))
		{
			if (dictionary5.ContainsKey(extSave5.ToString()))
			{
				copyListToArray2D(dictionary5[extSave5.ToString()], dataStringArray2D[(int)(VariableCode.__LOWERCASE__ & extSave5)]);
			}
		}
		foreach (VariableCode extSave6 in VariableIdentifier.GetExtSaveList((VariableCode)134348800))
		{
			if (dictionary6.ContainsKey(extSave6.ToString()))
			{
				copyListToArray2D(dictionary6[extSave6.ToString()], dataIntegerArray2D[(int)(VariableCode.__LOWERCASE__ & extSave6)]);
			}
		}
		foreach (VariableCode extSave7 in VariableIdentifier.GetExtSaveList((VariableCode)537133056))
		{
			if (dictionary7.ContainsKey(extSave7.ToString()))
			{
				copyListToArray3D(dictionary7[extSave7.ToString()], dataStringArray3D[(int)(VariableCode.__LOWERCASE__ & extSave7)]);
			}
		}
		foreach (VariableCode extSave8 in VariableIdentifier.GetExtSaveList((VariableCode)537001984))
		{
			if (dictionary8.ContainsKey(extSave8.ToString()))
			{
				copyListToArray3D(dictionary8[extSave8.ToString()], dataIntegerArray3D[(int)(VariableCode.__LOWERCASE__ & extSave8)]);
			}
		}
		if (version < 1808)
		{
			return;
		}
		dictionary3 = reader.ReadStringArrayExtended();
		dictionary4 = reader.ReadInt64ArrayExtended();
		dictionary5 = reader.ReadStringArray2DExtended();
		dictionary6 = reader.ReadInt64Array2DExtended();
		dictionary7 = reader.ReadStringArray3DExtended();
		dictionary8 = reader.ReadInt64Array3DExtended();
		int num = 0;
		List<UserDefinedVariableToken> obj = userDefinedSaveVarList[num];
		num++;
		foreach (UserDefinedVariableToken item in obj)
		{
			if (dictionary3.ContainsKey(item.Name))
			{
				copyListToArray(dictionary3[item.Name], (string[])item.GetArray());
			}
		}
		List<UserDefinedVariableToken> obj2 = userDefinedSaveVarList[num];
		num++;
		foreach (UserDefinedVariableToken item2 in obj2)
		{
			if (dictionary4.ContainsKey(item2.Name))
			{
				copyListToArray(dictionary4[item2.Name], (long[])item2.GetArray());
			}
		}
		List<UserDefinedVariableToken> obj3 = userDefinedSaveVarList[num];
		num++;
		foreach (UserDefinedVariableToken item3 in obj3)
		{
			if (dictionary5.ContainsKey(item3.Name))
			{
				copyListToArray2D(dictionary5[item3.Name], (string[,])item3.GetArray());
			}
		}
		List<UserDefinedVariableToken> obj4 = userDefinedSaveVarList[num];
		num++;
		foreach (UserDefinedVariableToken item4 in obj4)
		{
			if (dictionary6.ContainsKey(item4.Name))
			{
				copyListToArray2D(dictionary6[item4.Name], (long[,])item4.GetArray());
			}
		}
		List<UserDefinedVariableToken> obj5 = userDefinedSaveVarList[num];
		num++;
		foreach (UserDefinedVariableToken item5 in obj5)
		{
			if (dictionary7.ContainsKey(item5.Name))
			{
				copyListToArray3D(dictionary7[item5.Name], (string[,,])item5.GetArray());
			}
		}
		List<UserDefinedVariableToken> obj6 = userDefinedSaveVarList[num];
		num++;
		foreach (UserDefinedVariableToken item6 in obj6)
		{
			if (dictionary8.ContainsKey(item6.Name))
			{
				copyListToArray3D(dictionary8[item6.Name], (long[,,])item6.GetArray());
			}
		}
	}

	private void copyListToArray<T>(List<T> srcList, T[] destArray)
	{
		int num = Math.Min(srcList.Count, destArray.Length);
		for (int i = 0; i < num; i++)
		{
			destArray[i] = srcList[i];
		}
	}

	private void copyListToArray2D<T>(List<T[]> srcList, T[,] destArray)
	{
		int num = Math.Min(srcList.Count, destArray.GetLength(0));
		int length = destArray.GetLength(1);
		for (int i = 0; i < num; i++)
		{
			T[] array = srcList[i];
			int num2 = Math.Min(array.Length, length);
			for (int j = 0; j < num2; j++)
			{
				destArray[i, j] = array[j];
			}
		}
	}

	private void copyListToArray3D<T>(List<List<T[]>> srcList, T[,,] destArray)
	{
		int num = Math.Min(srcList.Count, destArray.GetLength(0));
		int length = destArray.GetLength(1);
		int length2 = destArray.GetLength(2);
		for (int i = 0; i < num; i++)
		{
			List<T[]> list = srcList[i];
			int num2 = Math.Min(list.Count, length);
			for (int j = 0; j < num2; j++)
			{
				T[] array = list[j];
				int num3 = Math.Min(array.Length, length2);
				for (int k = 0; k < num3; k++)
				{
					destArray[i, j, k] = array[k];
				}
			}
		}
	}

	public void SaveGlobalToStream(EraDataWriter writer)
	{
		writer.Write(dataIntegerArray[63]);
		writer.Write(dataStringArray[5]);
	}

	public void LoadGlobalFromStream(EraDataReader reader)
	{
		reader.ReadInt64Array(dataIntegerArray[63]);
		reader.ReadStringArray(dataStringArray[5]);
	}

	public void SaveGlobalToStream1808(EraDataWriter writer)
	{
		for (int i = 0; i < 6; i++)
		{
			foreach (UserDefinedVariableToken item in userDefinedGlobalSaveVarList[i])
			{
				switch (i)
				{
				case 0:
					writer.WriteExtended(item.Name, (string[])item.GetArray());
					break;
				case 1:
					writer.WriteExtended(item.Name, (long[])item.GetArray());
					break;
				case 2:
					writer.WriteExtended(item.Name, (string[,])item.GetArray());
					break;
				case 3:
					writer.WriteExtended(item.Name, (long[,])item.GetArray());
					break;
				case 4:
					writer.WriteExtended(item.Name, (string[,,])item.GetArray());
					break;
				case 5:
					writer.WriteExtended(item.Name, (long[,,])item.GetArray());
					break;
				}
			}
			writer.EmuSeparete();
		}
	}

	public void LoadGlobalFromStream1808(EraDataReader reader)
	{
		Dictionary<string, List<string>> dictionary = null;
		Dictionary<string, List<long>> dictionary2 = null;
		Dictionary<string, List<string[]>> dictionary3 = null;
		Dictionary<string, List<long[]>> dictionary4 = null;
		Dictionary<string, List<List<string[]>>> dictionary5 = null;
		Dictionary<string, List<List<long[]>>> dictionary6 = null;
		dictionary = reader.ReadStringArrayExtended();
		dictionary2 = reader.ReadInt64ArrayExtended();
		dictionary3 = reader.ReadStringArray2DExtended();
		dictionary4 = reader.ReadInt64Array2DExtended();
		dictionary5 = reader.ReadStringArray3DExtended();
		dictionary6 = reader.ReadInt64Array3DExtended();
		int num = 0;
		List<UserDefinedVariableToken> obj = userDefinedGlobalSaveVarList[num];
		num++;
		foreach (UserDefinedVariableToken item in obj)
		{
			if (dictionary.ContainsKey(item.Name))
			{
				copyListToArray(dictionary[item.Name], (string[])item.GetArray());
			}
		}
		List<UserDefinedVariableToken> obj2 = userDefinedGlobalSaveVarList[num];
		num++;
		foreach (UserDefinedVariableToken item2 in obj2)
		{
			if (dictionary2.ContainsKey(item2.Name))
			{
				copyListToArray(dictionary2[item2.Name], (long[])item2.GetArray());
			}
		}
		List<UserDefinedVariableToken> obj3 = userDefinedGlobalSaveVarList[num];
		num++;
		foreach (UserDefinedVariableToken item3 in obj3)
		{
			if (dictionary3.ContainsKey(item3.Name))
			{
				copyListToArray2D(dictionary3[item3.Name], (string[,])item3.GetArray());
			}
		}
		List<UserDefinedVariableToken> obj4 = userDefinedGlobalSaveVarList[num];
		num++;
		foreach (UserDefinedVariableToken item4 in obj4)
		{
			if (dictionary4.ContainsKey(item4.Name))
			{
				copyListToArray2D(dictionary4[item4.Name], (long[,])item4.GetArray());
			}
		}
		List<UserDefinedVariableToken> obj5 = userDefinedGlobalSaveVarList[num];
		num++;
		foreach (UserDefinedVariableToken item5 in obj5)
		{
			if (dictionary5.ContainsKey(item5.Name))
			{
				copyListToArray3D(dictionary5[item5.Name], (string[,,])item5.GetArray());
			}
		}
		List<UserDefinedVariableToken> obj6 = userDefinedGlobalSaveVarList[num];
		num++;
		foreach (UserDefinedVariableToken item6 in obj6)
		{
			if (dictionary6.ContainsKey(item6.Name))
			{
				copyListToArray3D(dictionary6[item6.Name], (long[,,])item6.GetArray());
			}
		}
	}

	public void SaveGlobalToStreamBinary(EraBinaryDataWriter writer)
	{
		foreach (KeyValuePair<string, VariableToken> item in varTokenDic)
		{
			VariableToken value = item.Value;
			if (value.IsSavedata && !value.IsCharacterData && value.IsGlobal)
			{
				writer.WriteWithKey(item.Key, value.GetArray());
			}
		}
		foreach (UserDefinedVariableToken userDefinedGlobalVar in userDefinedGlobalVarList)
		{
			if (userDefinedGlobalVar.IsSavedata)
			{
				writer.WriteWithKey(userDefinedGlobalVar.Name, userDefinedGlobalVar.GetArray());
			}
		}
	}

	public void SaveToStreamBinary(EraBinaryDataWriter writer)
	{
		foreach (KeyValuePair<string, VariableToken> item in varTokenDic)
		{
			VariableToken value = item.Value;
			if (value.IsSavedata && !value.IsCharacterData && !value.IsGlobal)
			{
				writer.WriteWithKey(item.Key, value.GetArray());
			}
		}
		foreach (UserDefinedVariableToken userDefinedStaticVar in userDefinedStaticVarList)
		{
			if (userDefinedStaticVar.IsSavedata)
			{
				writer.WriteWithKey(userDefinedStaticVar.Name, userDefinedStaticVar.GetArray());
			}
		}
	}

	public void LoadFromStreamBinary(EraBinaryDataReader bReader)
	{
		while (LoadVariableBinary(bReader))
		{
		}
	}

	public bool LoadVariableBinary(EraBinaryDataReader reader)
	{
		KeyValuePair<string, EraSaveDataType> keyValuePair = reader.ReadVariableCode();
		VariableToken variableToken = null;
		if (keyValuePair.Key != null && !GlobalStatic.IdentifierDictionary.getVarTokenIsForbid(keyValuePair.Key))
		{
			variableToken = GlobalStatic.IdentifierDictionary.GetVariableToken(keyValuePair.Key, null, allowPrivate: false);
		}
		if (variableToken != null && (variableToken.IsCharacterData || variableToken.IsConst || variableToken.IsPrivate || variableToken.IsLocal || variableToken.IsCalc))
		{
			variableToken = null;
		}
		switch (keyValuePair.Value)
		{
		case EraSaveDataType.EOF:
			return false;
		case EraSaveDataType.Int:
			if (variableToken == null || !variableToken.IsInteger || variableToken.Dimension != 0)
			{
				reader.ReadInt();
			}
			else
			{
				variableToken.SetValue(reader.ReadInt(), null);
			}
			break;
		case EraSaveDataType.Str:
			if (variableToken == null || !variableToken.IsString || variableToken.Dimension != 0)
			{
				reader.ReadString();
			}
			else
			{
				variableToken.SetValue(reader.ReadString(), null);
			}
			break;
		case EraSaveDataType.IntArray:
			if (variableToken == null || !variableToken.IsInteger || variableToken.Dimension != 1)
			{
				reader.ReadIntArray(null, needInit: true);
			}
			else
			{
				reader.ReadIntArray((long[])variableToken.GetArray(), needInit: true);
			}
			break;
		case EraSaveDataType.IntArray2D:
			if (variableToken == null || !variableToken.IsInteger || variableToken.Dimension != 2)
			{
				reader.ReadIntArray2D(null, needInit: true);
			}
			else
			{
				reader.ReadIntArray2D((long[,])variableToken.GetArray(), needInit: true);
			}
			break;
		case EraSaveDataType.IntArray3D:
			if (variableToken == null || !variableToken.IsInteger || variableToken.Dimension != 3)
			{
				reader.ReadIntArray3D(null, needInit: true);
			}
			else
			{
				reader.ReadIntArray3D((long[,,])variableToken.GetArray(), needInit: true);
			}
			break;
		case EraSaveDataType.StrArray:
			if (variableToken == null || !variableToken.IsString || variableToken.Dimension != 1)
			{
				reader.ReadStrArray(null, needInit: true);
			}
			else
			{
				reader.ReadStrArray((string[])variableToken.GetArray(), needInit: true);
			}
			break;
		case EraSaveDataType.StrArray2D:
			if (variableToken == null || !variableToken.IsString || variableToken.Dimension != 2)
			{
				reader.ReadStrArray2D(null, needInit: true);
			}
			else
			{
				reader.ReadStrArray2D((string[,])variableToken.GetArray(), needInit: true);
			}
			break;
		case EraSaveDataType.StrArray3D:
			if (variableToken == null || !variableToken.IsString || variableToken.Dimension != 3)
			{
				reader.ReadStrArray3D(null, needInit: true);
			}
			else
			{
				reader.ReadStrArray3D((string[,,])variableToken.GetArray(), needInit: true);
			}
			break;
		default:
			throw new FileEE("データ異常");
		}
		return true;
	}

	public void Dispose()
	{
		ClearLocalValue();
		for (int i = 0; i < dataIntegerArray.Length; i++)
		{
			dataIntegerArray[i] = null;
		}
		for (int j = 0; j < dataStringArray.Length; j++)
		{
			dataStringArray[j] = null;
		}
		for (int k = 0; k < characterList.Count; k++)
		{
			characterList[k].Dispose();
		}
		characterList.Clear();
	}
}
