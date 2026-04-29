using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MinorShift._Library;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameProc.Function;
using MinorShift.Emuera.GameView;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal sealed class VariableEvaluator : IDisposable
{
	private readonly GameBase gamebase;

	private readonly ConstantData constant;

	private readonly VariableData varData;

	private MTRandom rand = new MTRandom();

	public VariableData VariableData => varData;

	internal ConstantData Constant => constant;

	public long[] RESULT_ARRAY => varData.DataIntegerArray[10];

	public long RESULT
	{
		get
		{
			return varData.DataIntegerArray[10][0];
		}
		set
		{
			varData.DataIntegerArray[10][0] = value;
		}
	}

	public long COUNT
	{
		get
		{
			return varData.DataIntegerArray[11][0];
		}
		set
		{
			varData.DataIntegerArray[11][0] = value;
		}
	}

	public string RESULTS
	{
		get
		{
			string text = varData.DataStringArray[2][0];
			if (text == null)
			{
				return "";
			}
			return text;
		}
		set
		{
			varData.DataStringArray[2][0] = value;
		}
	}

	public string[] RESULTS_ARRAY => varData.DataStringArray[2];

	public long TARGET
	{
		get
		{
			return varData.DataIntegerArray[12][0];
		}
		set
		{
			varData.DataIntegerArray[12][0] = value;
		}
	}

	public long[] SELECTCOM_ARRAY => varData.DataIntegerArray[17];

	public long SELECTCOM
	{
		get
		{
			return varData.DataIntegerArray[17][0];
		}
		set
		{
			varData.DataIntegerArray[17][0] = value;
		}
	}

	public string[] ITEMNAME => constant.GetCsvNameList(VariableCode.ITEMNAME);

	public long[] ITEMSALES => varData.DataIntegerArray[23];

	public long[] ITEMPRICE => constant.ItemPrice;

	private long[] ITEM => varData.DataIntegerArray[2];

	public long[] RANDDATA => varData.DataIntegerArray[64];

	public string SAVEDATA_TEXT
	{
		get
		{
			return varData.DataString[0];
		}
		set
		{
			varData.DataString[0] = value;
		}
	}

	public long CHARANUM => varData.CharacterList.Count;

	public long MASTER
	{
		get
		{
			return get_Variable_canforbid(VariableCode.MASTER);
		}
		set
		{
			set_Variable_canforbid(VariableCode.MASTER, value);
		}
	}

	public long ASSI
	{
		get
		{
			return get_Variable_canforbid(VariableCode.ASSI);
		}
		set
		{
			set_Variable_canforbid(VariableCode.ASSI, value);
		}
	}

	public long ASSIPLAY
	{
		set
		{
			set_Variable_canforbid(VariableCode.ASSIPLAY, value);
		}
	}

	public long PREVCOM
	{
		get
		{
			return get_Variable_canforbid(VariableCode.PREVCOM);
		}
		set
		{
			set_Variable_canforbid(VariableCode.PREVCOM, value);
		}
	}

	public long NEXTCOM
	{
		get
		{
			return get_Variable_canforbid(VariableCode.NEXTCOM);
		}
		set
		{
			set_Variable_canforbid(VariableCode.NEXTCOM, value);
		}
	}

	private long MONEY
	{
		get
		{
			return get_Variable_canforbid(VariableCode.MONEY);
		}
		set
		{
			set_Variable_canforbid(VariableCode.MONEY, value);
		}
	}

	private long BOUGHT
	{
		set
		{
			set_Variable_canforbid(VariableCode.BOUGHT, value);
		}
	}

	public VariableEvaluator(GameBase gamebase, ConstantData constant)
	{
		this.gamebase = gamebase;
		this.constant = constant;
		varData = new VariableData(gamebase, constant);
		GlobalStatic.VariableData = varData;
	}

	public void Randomize(long seed)
	{
		rand = new MTRandom(seed);
	}

	public void InitRanddata()
	{
		rand.SetRand(RANDDATA);
	}

	public void DumpRanddata()
	{
		rand.GetRand(RANDDATA);
	}

	public long GetNextRand(long max)
	{
		return rand.NextInt64(max);
	}

	public long getPalamLv(long pl, long maxlv)
	{
		for (int i = 0; i < (int)maxlv; i++)
		{
			if (pl < varData.DataIntegerArray[6][i + 1])
			{
				return i;
			}
		}
		return maxlv;
	}

	public long getExpLv(long pl, long maxlv)
	{
		for (int i = 0; i < (int)maxlv; i++)
		{
			if (pl < varData.DataIntegerArray[7][i + 1])
			{
				return i;
			}
		}
		return maxlv;
	}

	public void SetValueAll(FixedVariableTerm p, long srcValue, int start, int end)
	{
		if (p.Identifier.IsCalc)
		{
			return;
		}
		if (p.Identifier.IsArray1D)
		{
			if (start != 0 || end != p.Identifier.GetLength())
			{
				p.IsArrayRangeValid(start, end, "VARSET", 3L, 4L);
			}
			else if (p.Identifier.IsCharacterData)
			{
				p.Identifier.CheckElement(new long[2] { p.Index1, p.Index2 });
			}
		}
		else if (p.Identifier.IsCharacterData)
		{
			p.Identifier.CheckElement(new long[3] { p.Index1, p.Index2, p.Index3 });
		}
		p.Identifier.SetValueAll(srcValue, start, end, (int)p.Index1);
	}

	public void SetValueAll(FixedVariableTerm p, string srcValue, int start, int end)
	{
		if (p.Identifier.IsCalc)
		{
			if (p.Identifier.Code == VariableCode.WINDOW_TITLE)
			{
				GlobalStatic.Console.SetWindowTitle(srcValue);
			}
			return;
		}
		if (p.Identifier.IsArray1D)
		{
			if (start != 0 || end != p.Identifier.GetLength())
			{
				p.IsArrayRangeValid(start, end, "VARSET", 3L, 4L);
			}
			else if (p.Identifier.IsCharacterData)
			{
				p.Identifier.CheckElement(new long[2] { p.Index1, p.Index2 });
			}
		}
		else if (p.Identifier.IsCharacterData)
		{
			p.Identifier.CheckElement(new long[3] { p.Index1, p.Index2, p.Index3 });
		}
		p.Identifier.SetValueAll(srcValue, start, end, (int)p.Index1);
	}

	public void SetValueAllEachChara(FixedVariableTerm p, SingleTerm index, long srcValue, int start, int end)
	{
		if (!p.Identifier.IsInteger)
		{
			throw new CodeEE("整数型でない変数" + p.Identifier.Name + "に整数値を代入しようとしました");
		}
		if (p.Identifier.IsConst)
		{
			throw new CodeEE("読み取り専用の変数" + p.Identifier.Name + "に代入しようとしました");
		}
		if (p.Identifier.IsCalc || varData.CharacterList.Count == 0)
		{
			return;
		}
		_ = varData.CharacterList[0];
		long num = -1L;
		if (p.Identifier.IsArray1D)
		{
			num = ((!(index.GetOperandType() == typeof(long))) ? constant.KeywordToInteger(p.Identifier.Code, index.Str, 1) : index.Int);
			if (num < 0 || num >= ((long[])p.Identifier.GetArrayChara(0)).Length)
			{
				throw new CodeEE("キャラクタ配列変数" + p.Identifier.Name + "の第２引数(" + num + ")は配列の範囲外です");
			}
		}
		for (int i = start; i < end; i++)
		{
			p.Identifier.SetValue(srcValue, new long[2] { i, num });
		}
	}

	public void SetValueAllEachChara(FixedVariableTerm p, SingleTerm index, string srcValue, int start, int end)
	{
		if (!p.Identifier.IsString)
		{
			throw new CodeEE("文字列型でない変数" + p.Identifier.Name + "に文字列型を代入しようとしました");
		}
		if (p.Identifier.IsConst)
		{
			throw new CodeEE("読み取り専用の変数" + p.Identifier.Name + "に代入しようとしました");
		}
		if (p.Identifier.IsCalc)
		{
			if (p.Identifier.Code == VariableCode.WINDOW_TITLE)
			{
				GlobalStatic.Console.SetWindowTitle(srcValue);
			}
		}
		else
		{
			if (varData.CharacterList.Count == 0)
			{
				return;
			}
			long num = -1L;
			if (p.Identifier.IsArray1D)
			{
				num = ((!(index.GetOperandType() == typeof(long))) ? constant.KeywordToInteger(p.Identifier.Code, index.Str, 1) : index.Int);
				if (num < 0 || num >= ((string[])p.Identifier.GetArrayChara(0)).Length)
				{
					throw new CodeEE("キャラクタ配列変数" + p.Identifier.Name + "の第２引数(" + num + ")は配列の範囲外です");
				}
			}
			for (int i = start; i < end; i++)
			{
				p.Identifier.SetValue(srcValue, new long[2] { i, num });
			}
		}
	}

	public long GetArraySum(FixedVariableTerm p, long index1, long index2)
	{
		long num = 0L;
		if (p.Identifier.IsCharacterData)
		{
			if (p.Identifier.IsArray1D)
			{
				for (int i = (int)index1; i < (int)index2; i++)
				{
					num += p.Identifier.GetIntValue(GlobalStatic.EMediator, new long[2] { p.Index1, i });
				}
			}
			else
			{
				for (int j = (int)index1; j < (int)index2; j++)
				{
					num += p.Identifier.GetIntValue(GlobalStatic.EMediator, new long[3] { p.Index1, p.Index2, j });
				}
			}
		}
		else if (p.Identifier.IsArray1D)
		{
			for (int k = (int)index1; k < (int)index2; k++)
			{
				num += p.Identifier.GetIntValue(GlobalStatic.EMediator, new long[1] { k });
			}
		}
		else if (p.Identifier.IsArray2D)
		{
			for (int l = (int)index1; l < (int)index2; l++)
			{
				num += p.Identifier.GetIntValue(GlobalStatic.EMediator, new long[2] { p.Index1, l });
			}
		}
		else
		{
			for (int m = (int)index1; m < (int)index2; m++)
			{
				num += p.Identifier.GetIntValue(GlobalStatic.EMediator, new long[3] { p.Index1, p.Index2, m });
			}
		}
		return num;
	}

	public long GetArraySumChara(FixedVariableTerm p, long index1, long index2)
	{
		long num = 0L;
		for (int i = (int)index1; i < (int)index2; i++)
		{
			num += p.Identifier.GetIntValue(GlobalStatic.EMediator, new long[2] { i, p.Index2 });
		}
		return num;
	}

	public long GetMatch(FixedVariableTerm p, long target, long start, long end)
	{
		long num = 0L;
		for (int i = (int)start; i < (int)end; i++)
		{
			if (p.Identifier.GetIntValue(GlobalStatic.EMediator, (!p.Identifier.IsCharacterData) ? new long[1] { i } : new long[2] { p.Index1, i }) == target)
			{
				num++;
			}
		}
		return num;
	}

	public long GetMatch(FixedVariableTerm p, string target, long start, long end)
	{
		long num = 0L;
		bool flag = string.IsNullOrEmpty(target);
		for (int i = (int)start; i < (int)end; i++)
		{
			if (p.Identifier.GetStrValue(GlobalStatic.EMediator, (!p.Identifier.IsCharacterData) ? new long[1] { i } : new long[2] { p.Index1, i }) == target || (flag && string.IsNullOrEmpty(p.Identifier.GetStrValue(GlobalStatic.EMediator, (!p.Identifier.IsCharacterData) ? new long[1] { i } : new long[2] { p.Index1, i }))))
			{
				num++;
			}
		}
		return num;
	}

	public long GetMatchChara(FixedVariableTerm p, long target, long start, long end)
	{
		long num = 0L;
		for (int i = (int)start; i < (int)end; i++)
		{
			if (p.Identifier.GetIntValue(GlobalStatic.EMediator, new long[3] { i, p.Index2, p.Index3 }) == target)
			{
				num++;
			}
		}
		return num;
	}

	public long GetMatchChara(FixedVariableTerm p, string target, long start, long end)
	{
		long num = 0L;
		bool flag = string.IsNullOrEmpty(target);
		for (int i = (int)start; i < (int)end; i++)
		{
			if (p.Identifier.GetStrValue(GlobalStatic.EMediator, new long[3] { i, p.Index2, p.Index3 }) == target || (flag && string.IsNullOrEmpty(p.Identifier.GetStrValue(GlobalStatic.EMediator, new long[3] { i, p.Index2, p.Index3 }))))
			{
				num++;
			}
		}
		return num;
	}

	public long FindElement(FixedVariableTerm p, long target, long start, long end, bool isExact, bool isLast)
	{
		if (start >= end)
		{
			return -1L;
		}
		long[] array = ((!p.Identifier.IsCharacterData) ? ((long[])p.Identifier.GetArray()) : ((long[])p.Identifier.GetArrayChara((int)p.Index1)));
		if (isLast)
		{
			for (int num = (int)end - 1; num >= (int)start; num--)
			{
				if (target == array[num])
				{
					return num;
				}
			}
		}
		else
		{
			for (int i = (int)start; i < (int)end; i++)
			{
				if (target == array[i])
				{
					return i;
				}
			}
		}
		return -1L;
	}

	public long FindElement(FixedVariableTerm p, Regex target, long start, long end, bool isExact, bool isLast)
	{
		if (start >= end)
		{
			return -1L;
		}
		string[] array = ((!p.Identifier.IsCharacterData) ? ((string[])p.Identifier.GetArray()) : ((string[])p.Identifier.GetArrayChara((int)p.Index1)));
		if (isLast)
		{
			for (int num = (int)end - 1; num >= (int)start; num--)
			{
				if (isExact)
				{
					if (array[num] != null)
					{
						Match match = target.Match(array[num]);
						if (match.Success && array[num].Length == match.Length)
						{
							return num;
						}
					}
				}
				else if (array[num] != null && target.IsMatch(array[num]))
				{
					return num;
				}
			}
		}
		else
		{
			for (int i = (int)start; i < (int)end; i++)
			{
				if (isExact)
				{
					if (array[i] != null)
					{
						Match match2 = target.Match(array[i]);
						if (match2.Success && array[i].Length == match2.Length)
						{
							return i;
						}
					}
				}
				else if (array[i] != null && target.IsMatch(array[i]))
				{
					return i;
				}
			}
		}
		return -1L;
	}

	public long GetMaxArray(FixedVariableTerm p, long start, long end, bool isMax)
	{
		long num = p.Identifier.GetIntValue(GlobalStatic.EMediator, (!p.Identifier.IsCharacterData) ? new long[1] { start } : new long[2] { p.Index1, start });
		for (int i = (int)start + 1; i < (int)end; i++)
		{
			long intValue = p.Identifier.GetIntValue(GlobalStatic.EMediator, (!p.Identifier.IsCharacterData) ? new long[1] { i } : new long[2] { p.Index1, i });
			if (isMax)
			{
				if (intValue > num)
				{
					num = intValue;
				}
			}
			else if (intValue < num)
			{
				num = intValue;
			}
		}
		return num;
	}

	public long GetMaxArrayChara(FixedVariableTerm p, long start, long end, bool isMax)
	{
		long num = p.Identifier.GetIntValue(GlobalStatic.EMediator, new long[3] { start, p.Index2, p.Index3 });
		for (int i = (int)start + 1; i < (int)end; i++)
		{
			long intValue = p.Identifier.GetIntValue(GlobalStatic.EMediator, new long[3] { i, p.Index2, p.Index3 });
			if (isMax)
			{
				if (intValue > num)
				{
					num = intValue;
				}
			}
			else if (intValue < num)
			{
				num = intValue;
			}
		}
		return num;
	}

	public long GetInRangeArray(FixedVariableTerm p, long min, long max, long start, long end)
	{
		long num = 0L;
		for (int i = (int)start; i < (int)end; i++)
		{
			long intValue = p.Identifier.GetIntValue(GlobalStatic.EMediator, (!p.Identifier.IsCharacterData) ? new long[1] { i } : new long[2] { p.Index1, i });
			if (intValue >= min && intValue < max)
			{
				num++;
			}
		}
		return num;
	}

	public long GetInRangeArrayChara(FixedVariableTerm p, long min, long max, long start, long end)
	{
		long num = 0L;
		for (int i = (int)start; i < (int)end; i++)
		{
			long intValue = p.Identifier.GetIntValue(GlobalStatic.EMediator, new long[3] { i, p.Index2, p.Index3 });
			if (intValue >= min && intValue < max)
			{
				num++;
			}
		}
		return num;
	}

	public void ShiftArray(FixedVariableTerm p, int shift, long def, int start, int num)
	{
		long[] array = ((!p.Identifier.IsCharacterData) ? ((long[])p.Identifier.GetArray()) : ((long[])p.Identifier.GetArrayChara((int)p.Index1)));
		if (start >= array.Length)
		{
			throw new CodeEE("命令ARRAYREMOVEの第４引数(" + start + ")が配列" + p.Identifier.Name + "の範囲を超えています");
		}
		if (num == -1)
		{
			num = array.Length - start;
		}
		if (start + num > array.Length)
		{
			num = array.Length - start;
		}
		if (Math.Abs(shift) >= array.Length && start == 0 && num >= array.Length)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = def;
			}
			return;
		}
		int num2 = 0;
		int num3 = start + shift;
		int num4 = num - Math.Abs(shift);
		if (shift < 0)
		{
			num2 = -shift;
			num3 = start;
		}
		long[] array2 = new long[num];
		Buffer.BlockCopy(array, start * 8, array2, 0, 8 * num);
		if (num2 == 0)
		{
			if (num4 <= 0)
			{
				for (int j = start; j < start + num; j++)
				{
					array[j] = def;
				}
				return;
			}
			for (int k = start; k < start + shift; k++)
			{
				array[k] = def;
			}
		}
		else
		{
			if (num4 <= 0)
			{
				for (int l = start; l < start + num; l++)
				{
					array[l] = def;
				}
				return;
			}
			for (int m = start + num4; m < start + num; m++)
			{
				array[m] = def;
			}
		}
		if (num4 > 0)
		{
			Buffer.BlockCopy(array2, num2 * 8, array, num3 * 8, num4 * 8);
		}
	}

	public void ShiftArray(FixedVariableTerm p, int shift, string def, int start, int num)
	{
		string[] array = ((!p.Identifier.IsCharacterData) ? ((string[])p.Identifier.GetArray()) : ((string[])p.Identifier.GetArrayChara((int)p.Index1)));
		if (start >= array.Length)
		{
			throw new CodeEE("命令ARRAYREMOVEの第４引数(" + start + ")が配列" + p.Identifier.Name + "の範囲を超えています");
		}
		if (num == -1)
		{
			num = array.Length - start;
		}
		if (start + num > array.Length)
		{
			num = array.Length - start;
		}
		if (Math.Abs(shift) >= array.Length && start == 0 && num >= array.Length)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = def;
			}
			return;
		}
		int sourceIndex = 0;
		int num2 = start + shift;
		int num3 = num - Math.Abs(shift);
		if (shift < 0)
		{
			sourceIndex = -shift;
			num2 = start;
		}
		string[] array2 = new string[num];
		Array.Copy(array, start, array2, 0, num);
		if (num2 > start)
		{
			if (num3 <= 0)
			{
				for (int j = start; j < start + num; j++)
				{
					array[j] = def;
				}
				return;
			}
			for (int k = start; k < start + shift; k++)
			{
				array[k] = def;
			}
		}
		else
		{
			if (num3 <= 0)
			{
				for (int l = start; l < start + num; l++)
				{
					array[l] = def;
				}
				return;
			}
			for (int m = start + num3; m < start + num; m++)
			{
				array[m] = def;
			}
		}
		if (num3 > 0)
		{
			Array.Copy(array2, sourceIndex, array, num2, num3);
		}
	}

	public void RemoveArray(FixedVariableTerm p, int start, int num)
	{
		if (p.Identifier.IsInteger)
		{
			long[] array = ((!p.Identifier.IsCharacterData) ? ((long[])p.Identifier.GetArray()) : ((long[])p.Identifier.GetArrayChara((int)p.Index1)));
			if (start >= array.Length)
			{
				throw new CodeEE("命令ARRAYREMOVEの第２引数(" + start + ")が配列" + p.Identifier.Name + "の範囲を超えています");
			}
			if (num <= 0)
			{
				num = array.Length;
			}
			long[] array2 = new long[array.Length];
			if (start > 0)
			{
				Buffer.BlockCopy(array, 0, array2, 0, start * 8);
			}
			if (start + num < array.Length)
			{
				Buffer.BlockCopy(array, (start + num) * 8, array2, start * 8, (array.Length - (start + num)) * 8);
			}
			Buffer.BlockCopy(array2, 0, array, 0, array2.Length * 8);
		}
		else
		{
			string[] array3 = ((!p.Identifier.IsCharacterData) ? ((string[])p.Identifier.GetArray()) : ((string[])p.Identifier.GetArrayChara((int)p.Index1)));
			if (num <= 0)
			{
				num = array3.Length;
			}
			string[] array4 = new string[array3.Length];
			if (start > 0)
			{
				Array.Copy(array3, 0, array4, 0, start);
			}
			if (start + num < array3.Length)
			{
				Array.Copy(array3, start + num, array4, start, array3.Length - (start + num));
			}
			array4.CopyTo(array3, 0);
		}
	}

	public void SortArray(FixedVariableTerm p, SortOrder order, int start, int num)
	{
		if (order == SortOrder.UNDEF)
		{
			order = SortOrder.ASCENDING;
		}
		if (p.Identifier.IsInteger)
		{
			long[] array = ((!p.Identifier.IsCharacterData) ? ((long[])p.Identifier.GetArray()) : ((long[])p.Identifier.GetArrayChara((int)p.Index1)));
			if (start >= array.Length)
			{
				throw new CodeEE("命令ARRAYSORTの第３引数(" + start + ")が配列" + p.Identifier.Name + "の範囲を超えています");
			}
			if (num <= 0)
			{
				num = array.Length - start;
			}
			long[] array2 = new long[num];
			Array.Copy(array, start, array2, 0, num);
			switch (order)
			{
			case SortOrder.ASCENDING:
				Array.Sort(array2);
				break;
			case SortOrder.DESENDING:
				Array.Sort(array2, (long a, long b) => b.CompareTo(a));
				break;
			}
			Array.Copy(array2, 0, array, start, num);
			return;
		}
		string[] array3 = ((!p.Identifier.IsCharacterData) ? ((string[])p.Identifier.GetArray()) : ((string[])p.Identifier.GetArrayChara((int)p.Index1)));
		if (start >= array3.Length)
		{
			throw new CodeEE("命令ARRAYSORTの第３引数(" + start + ")が配列" + p.Identifier.Name + "の範囲を超えています");
		}
		if (num <= 0)
		{
			num = array3.Length - start;
		}
		string[] array4 = new string[num];
		Array.Copy(array3, start, array4, 0, num);
		switch (order)
		{
		case SortOrder.ASCENDING:
			Array.Sort(array4);
			break;
		case SortOrder.DESENDING:
			Array.Sort(array4, (string a, string b) => b.CompareTo(a));
			break;
		}
		Array.Copy(array4, 0, array3, start, num);
	}

	public void CopyArray(VariableToken var1, VariableToken var2)
	{
		if (var1.IsInteger)
		{
			if (var1.IsArray1D)
			{
				long[] array = (long[])var1.GetArray();
				long[] array2 = (long[])var2.GetArray();
				int num = ((array.Length >= array2.Length) ? array2.Length : array.Length);
				for (int i = 0; i < num; i++)
				{
					array2[i] = array[i];
				}
				return;
			}
			if (var1.IsArray2D)
			{
				long[,] array3 = (long[,])var1.GetArray();
				long[,] array4 = (long[,])var2.GetArray();
				int num2 = ((array3.GetLength(0) >= array4.GetLength(0)) ? array4.GetLength(0) : array3.GetLength(0));
				int num3 = ((array3.GetLength(1) >= array4.GetLength(1)) ? array4.GetLength(1) : array3.GetLength(1));
				for (int j = 0; j < num2; j++)
				{
					for (int k = 0; k < num3; k++)
					{
						array4[j, k] = array3[j, k];
					}
				}
				return;
			}
			long[,,] array5 = (long[,,])var1.GetArray();
			long[,,] array6 = (long[,,])var2.GetArray();
			int num4 = ((array5.GetLength(0) >= array6.GetLength(0)) ? array6.GetLength(0) : array5.GetLength(0));
			int num5 = ((array5.GetLength(1) >= array6.GetLength(1)) ? array6.GetLength(1) : array5.GetLength(1));
			int num6 = ((array5.GetLength(2) >= array6.GetLength(2)) ? array6.GetLength(2) : array5.GetLength(2));
			for (int l = 0; l < num4; l++)
			{
				for (int m = 0; m < num5; m++)
				{
					for (int n = 0; n < num6; n++)
					{
						array6[l, m, n] = array5[l, m, n];
					}
				}
			}
			return;
		}
		if (var1.IsArray1D)
		{
			string[] array7 = (string[])var1.GetArray();
			string[] array8 = (string[])var2.GetArray();
			int num7 = ((array7.Length >= array8.Length) ? array8.Length : array7.Length);
			for (int num8 = 0; num8 < num7; num8++)
			{
				array8[num8] = array7[num8];
			}
			return;
		}
		if (var1.IsArray2D)
		{
			string[,] array9 = (string[,])var1.GetArray();
			string[,] array10 = (string[,])var2.GetArray();
			int num9 = ((array9.GetLength(0) >= array10.GetLength(0)) ? array10.GetLength(0) : array9.GetLength(0));
			int num10 = ((array9.GetLength(1) >= array10.GetLength(1)) ? array10.GetLength(1) : array9.GetLength(1));
			for (int num11 = 0; num11 < num9; num11++)
			{
				for (int num12 = 0; num12 < num10; num12++)
				{
					array10[num11, num12] = array9[num11, num12];
				}
			}
			return;
		}
		string[,,] array11 = (string[,,])var1.GetArray();
		string[,,] array12 = (string[,,])var2.GetArray();
		int num13 = ((array11.GetLength(0) >= array12.GetLength(0)) ? array12.GetLength(0) : array11.GetLength(0));
		int num14 = ((array11.GetLength(1) >= array12.GetLength(1)) ? array12.GetLength(1) : array11.GetLength(1));
		int num15 = ((array11.GetLength(2) >= array12.GetLength(2)) ? array12.GetLength(2) : array11.GetLength(2));
		for (int num16 = 0; num16 < num13; num16++)
		{
			for (int num17 = 0; num17 < num14; num17++)
			{
				for (int num18 = 0; num18 < num15; num18++)
				{
					array12[num16, num17, num18] = array11[num16, num17, num18];
				}
			}
		}
	}

	public string GetHavingItemsString()
	{
		long[] iTEM = ITEM;
		string[] iTEMNAME = ITEMNAME;
		int num = Math.Min(iTEM.Length, iTEMNAME.Length);
		int num2 = 0;
		StringBuilder stringBuilder = new StringBuilder(100);
		stringBuilder.Append("所持アイテム：");
		for (int i = 0; i < num; i++)
		{
			if (iTEM[i] != 0L)
			{
				num2++;
				if (iTEMNAME[i] != null)
				{
					stringBuilder.Append(iTEMNAME[i]);
				}
				stringBuilder.Append("(");
				stringBuilder.Append(iTEM[i].ToString());
				stringBuilder.Append(") ");
			}
		}
		if (num2 == 0)
		{
			stringBuilder.Append("なし");
		}
		return stringBuilder.ToString();
	}

	public string GetCharacterDataString(long target, FunctionCode func)
	{
		StringBuilder stringBuilder = new StringBuilder(100);
		if (target < 0 || target >= varData.CharacterList.Count)
		{
			throw new CodeEE("存在しない登録キャラクタを参照しようとしました");
		}
		CharacterData characterData = varData.CharacterList[(int)target];
		long[] array = null;
		string[] array2 = null;
		int num = 0;
		switch (func)
		{
		case FunctionCode.PRINT_ABL:
			array = characterData.DataIntegerArray[2];
			array2 = constant.GetCsvNameList(VariableCode.ABLNAME);
			for (num = 0; num < array.Length && num < array2.Length; num++)
			{
				if (array[num] != 0L && !string.IsNullOrEmpty(array2[num]))
				{
					stringBuilder.Append(array2[num]);
					stringBuilder.Append("LV");
					stringBuilder.Append(array[num].ToString());
					stringBuilder.Append(" ");
				}
			}
			break;
		case FunctionCode.PRINT_TALENT:
			array = characterData.DataIntegerArray[3];
			array2 = constant.GetCsvNameList(VariableCode.TALENTNAME);
			for (num = 0; num < array.Length && num < array2.Length; num++)
			{
				if (array[num] != 0L && !string.IsNullOrEmpty(array2[num]))
				{
					stringBuilder.Append("[");
					stringBuilder.Append(array2[num]);
					stringBuilder.Append("]");
				}
			}
			break;
		case FunctionCode.PRINT_MARK:
			array = characterData.DataIntegerArray[5];
			array2 = constant.GetCsvNameList(VariableCode.MARKNAME);
			for (num = 0; num < array.Length && num < array2.Length; num++)
			{
				if (array[num] != 0L && !string.IsNullOrEmpty(array2[num]))
				{
					stringBuilder.Append(array2[num]);
					stringBuilder.Append("LV");
					stringBuilder.Append(array[num].ToString());
					stringBuilder.Append(" ");
				}
			}
			break;
		case FunctionCode.PRINT_EXP:
			array = characterData.DataIntegerArray[4];
			array2 = constant.GetCsvNameList(VariableCode.EXPNAME);
			for (num = 0; num < array.Length && num < array2.Length; num++)
			{
				if (array[num] != 0L && !string.IsNullOrEmpty(array2[num]))
				{
					stringBuilder.Append(array2[num]);
					stringBuilder.Append(array[num].ToString());
					stringBuilder.Append(" ");
				}
			}
			break;
		}
		return stringBuilder.ToString();
	}

	public string GetCharacterParamString(long target, int paramCode)
	{
		if (target < 0 || target >= varData.CharacterList.Count)
		{
			throw new CodeEE("存在しない登録キャラクタを参照しようとしました");
		}
		long num = varData.CharacterList[(int)target].DataIntegerArray[6][paramCode];
		long[] array = varData.DataIntegerArray[6];
		string text = constant.GetCsvNameList(VariableCode.PALAMNAME)[paramCode];
		if (num == 0L && string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (text == null)
		{
			text = "";
		}
		char value = '-';
		long num2 = array[1];
		if (num >= num2)
		{
			value = '=';
			num2 = array[2];
		}
		if (num >= num2)
		{
			value = '>';
			num2 = array[3];
		}
		if (num >= num2)
		{
			value = '*';
			num2 = array[4];
		}
		StringBuilder stringBuilder = new StringBuilder(100);
		stringBuilder.Append('[');
		if (num2 <= 0 || num2 <= num)
		{
			stringBuilder.Append(value, 10);
		}
		else if (num <= 0)
		{
			stringBuilder.Append('.', 10);
		}
		else
		{
			int num3 = (int)(num * 10 / num2);
			stringBuilder.Append(value, num3);
			stringBuilder.Append('.', 10 - num3);
		}
		stringBuilder.Append(']');
		return $"{text}{stringBuilder.ToString()}{num,6}";
	}

	public void AddCharacter(long charaTmplNo)
	{
		CharacterTemplate characterTemplate = constant.GetCharacterTemplate(charaTmplNo);
		if (characterTemplate == null)
		{
			throw new CodeEE("定義していないキャラクタを作成しようとしました");
		}
		CharacterData item = new CharacterData(constant, characterTemplate, varData);
		varData.CharacterList.Add(item);
	}

	public void AddCharacter_UseSp(long charaTmplNo, bool isSp)
	{
		CharacterTemplate characterTemplate_UseSp = constant.GetCharacterTemplate_UseSp(charaTmplNo, isSp);
		if (characterTemplate_UseSp == null)
		{
			throw new CodeEE("定義していないキャラクタを作成しようとしました");
		}
		CharacterData item = new CharacterData(constant, characterTemplate_UseSp, varData);
		varData.CharacterList.Add(item);
	}

	public void AddCharacterFromCsvNo(long CsvNo)
	{
		CharacterTemplate characterTemplate = constant.GetCharacterTemplateFromCsvNo(CsvNo);
		if (characterTemplate == null)
		{
			characterTemplate = constant.GetPseudoChara();
		}
		CharacterData item = new CharacterData(constant, characterTemplate, varData);
		varData.CharacterList.Add(item);
	}

	public void AddPseudoCharacter()
	{
		CharacterTemplate pseudoChara = constant.GetPseudoChara();
		CharacterData item = new CharacterData(constant, pseudoChara, varData);
		varData.CharacterList.Add(item);
	}

	public void DelCharacter(long charaNo)
	{
		if (charaNo < 0 || charaNo >= varData.CharacterList.Count)
		{
			throw new CodeEE("存在しない登録キャラクタ(" + charaNo + ")を削除しようとしました");
		}
		varData.CharacterList[(int)charaNo].Dispose();
		varData.CharacterList.RemoveAt((int)charaNo);
	}

	public void DelCharacter(long[] charaNoList)
	{
		List<CharacterData> list = new List<CharacterData>();
		for (int i = 0; i < charaNoList.Length; i++)
		{
			long num = charaNoList[i];
			if (num < 0 || num >= varData.CharacterList.Count)
			{
				throw new CodeEE("存在しない登録キャラクタ(" + charaNoList.ToString() + ")を削除しようとしました");
			}
			CharacterData characterData = varData.CharacterList[(int)num];
			if (list.Contains(characterData))
			{
				throw new CodeEE("同一の登録キャラクタ番号(" + num + ")が複数回指定されました");
			}
			list.Add(characterData);
			characterData.Dispose();
		}
		foreach (CharacterData item in list)
		{
			varData.CharacterList.Remove(item);
		}
	}

	public void DelAllCharacter()
	{
		if (varData.CharacterList.Count == 0)
		{
			return;
		}
		foreach (CharacterData character in varData.CharacterList)
		{
			character.Dispose();
		}
		varData.CharacterList.Clear();
	}

	public void PickUpChara(long[] NoList)
	{
		List<long> list = new List<long>();
		long tARGET = TARGET;
		long aSSI = ASSI;
		long mASTER = MASTER;
		TARGET = -1L;
		ASSI = -1L;
		MASTER = -1L;
		for (int i = 0; i < NoList.Length; i++)
		{
			if (!list.Contains(NoList[i]) && NoList[i] >= 0)
			{
				list.Add(NoList[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (j != list[j])
			{
				SwapChara(list[j], j);
				if (list.IndexOf(j) > j)
				{
					list[list.IndexOf(j)] = list[j];
				}
			}
			if (TARGET < 0 && list[j] == tARGET)
			{
				TARGET = j;
			}
			if (ASSI < 0 && list[j] == aSSI)
			{
				ASSI = j;
			}
			if (MASTER < 0 && list[j] == mASTER)
			{
				MASTER = j;
			}
		}
		if (list.Count < varData.CharacterList.Count)
		{
			for (int num = varData.CharacterList.Count - 1; num >= list.Count; num--)
			{
				DelCharacter(num);
			}
		}
	}

	public void ResetData()
	{
		varData.SetDefaultLocalValue();
		varData.SetDefaultValue(constant);
		foreach (CharacterData character in varData.CharacterList)
		{
			character.Dispose();
		}
		varData.CharacterList.Clear();
	}

	public void ResetGlobalData()
	{
		varData.SetDefaultGlobalValue();
	}

	public void CopyChara(long x, long y)
	{
		if (x < 0 || x >= varData.CharacterList.Count)
		{
			throw new CodeEE("コピー元のキャラクタが存在しません");
		}
		if (y < 0 || y >= varData.CharacterList.Count)
		{
			throw new CodeEE("コピー先のキャラクタが存在しません");
		}
		varData.CharacterList[(int)x].CopyTo(varData.CharacterList[(int)y]);
	}

	public void AddCopyChara(long x)
	{
		if (x < 0 || x >= varData.CharacterList.Count)
		{
			throw new CodeEE("コピー元のキャラクタが存在しません");
		}
		AddPseudoCharacter();
		varData.CharacterList[(int)x].CopyTo(varData.CharacterList[varData.CharacterList.Count - 1]);
	}

	public void SwapChara(long x, long y)
	{
		if (x < 0 || x >= varData.CharacterList.Count || y < 0 || y >= varData.CharacterList.Count)
		{
			throw new CodeEE("存在しない登録キャラクタを入れ替えようとしました");
		}
		if (x != y)
		{
			CharacterData value = varData.CharacterList[(int)y];
			varData.CharacterList[(int)y] = varData.CharacterList[(int)x];
			varData.CharacterList[(int)x] = value;
		}
	}

	public void SortChara(VariableToken sortkey, long elem, SortOrder sortorder, bool fixMaster)
	{
		if (varData.CharacterList.Count <= 1)
		{
			return;
		}
		if (sortorder == SortOrder.UNDEF)
		{
			sortorder = SortOrder.ASCENDING;
		}
		if (sortkey == null)
		{
			sortkey = GlobalStatic.VariableData.GetSystemVariableToken("NO");
		}
		CharacterData characterData = null;
		CharacterData characterData2 = null;
		CharacterData characterData3 = null;
		if (MASTER >= 0 && MASTER < varData.CharacterList.Count)
		{
			characterData = varData.CharacterList[(int)MASTER];
		}
		if (TARGET >= 0 && TARGET < varData.CharacterList.Count)
		{
			characterData2 = varData.CharacterList[(int)TARGET];
		}
		if (ASSI >= 0 && ASSI < varData.CharacterList.Count)
		{
			characterData3 = varData.CharacterList[(int)ASSI];
		}
		for (int i = 0; i < varData.CharacterList.Count; i++)
		{
			varData.CharacterList[i].temp_CurrentOrder = i;
			varData.CharacterList[i].SetSortKey(sortkey, elem);
		}
		if (fixMaster && characterData != null)
		{
			if (varData.CharacterList.Count <= 2)
			{
				return;
			}
			varData.CharacterList.Remove(characterData);
		}
		if (sortorder == SortOrder.ASCENDING)
		{
			varData.CharacterList.Sort(CharacterData.AscCharacterComparison);
		}
		else
		{
			varData.CharacterList.Sort(CharacterData.DescCharacterComparison);
		}
		if (fixMaster && characterData != null)
		{
			varData.CharacterList.Insert((int)MASTER, characterData);
		}
		for (int j = 0; j < varData.CharacterList.Count; j++)
		{
			varData.CharacterList[j].temp_CurrentOrder = j;
		}
		if (characterData != null && !fixMaster)
		{
			MASTER = characterData.temp_CurrentOrder;
		}
		if (characterData2 != null)
		{
			TARGET = characterData2.temp_CurrentOrder;
		}
		if (characterData3 != null)
		{
			ASSI = characterData3.temp_CurrentOrder;
		}
	}

	internal long FindChara(VariableToken varID, long elem64, string word, long startIndex, long lastIndex, bool isLast)
	{
		if (startIndex >= lastIndex)
		{
			return -1L;
		}
		FixedVariableTerm fixedVariableTerm = new FixedVariableTerm(varID);
		if (varID.IsArray1D)
		{
			fixedVariableTerm.Index2 = elem64;
		}
		else if (varID.IsArray2D)
		{
			fixedVariableTerm.Index2 = elem64 >> 32;
			fixedVariableTerm.Index3 = elem64 & 0x7FFFFFFF;
		}
		_ = varData.CharacterList.Count;
		if (isLast)
		{
			for (long num = lastIndex - 1; num >= startIndex; num--)
			{
				fixedVariableTerm.Index1 = num;
				if (word == fixedVariableTerm.GetStrValue(null))
				{
					return num;
				}
			}
		}
		else
		{
			for (long num2 = startIndex; num2 < lastIndex; num2++)
			{
				fixedVariableTerm.Index1 = num2;
				if (word == fixedVariableTerm.GetStrValue(null))
				{
					return num2;
				}
			}
		}
		return -1L;
	}

	internal long FindChara(VariableToken varID, long elem64, long word, long startIndex, long lastIndex, bool isLast)
	{
		if (startIndex >= lastIndex)
		{
			return -1L;
		}
		FixedVariableTerm fixedVariableTerm = new FixedVariableTerm(varID);
		if (varID.IsArray1D)
		{
			fixedVariableTerm.Index2 = elem64;
		}
		else if (varID.IsArray2D)
		{
			fixedVariableTerm.Index2 = elem64 >> 32;
			fixedVariableTerm.Index3 = elem64 & 0x7FFFFFFF;
		}
		_ = varData.CharacterList.Count;
		if (isLast)
		{
			for (long num = lastIndex - 1; num >= startIndex; num--)
			{
				fixedVariableTerm.Index1 = num;
				if (word == fixedVariableTerm.GetIntValue(null))
				{
					return num;
				}
			}
		}
		else
		{
			for (long num2 = startIndex; num2 < lastIndex; num2++)
			{
				fixedVariableTerm.Index1 = num2;
				if (word == fixedVariableTerm.GetIntValue(null))
				{
					return num2;
				}
			}
		}
		return -1L;
	}

	public long GetChara(long charaNo)
	{
		for (int i = 0; i < varData.CharacterList.Count; i++)
		{
			if (varData.CharacterList[i].NO == charaNo)
			{
				return i;
			}
		}
		return -1L;
	}

	public long GetChara_UseSp(long charaNo, bool getSp)
	{
		for (int i = 0; i < varData.CharacterList.Count; i++)
		{
			if (varData.CharacterList[i].NO == charaNo && varData.CharacterList[i].CFlag[0] != 0 == getSp)
			{
				return i;
			}
		}
		return -1L;
	}

	public long ExistCsv(long charaNo, bool getSp)
	{
		if (constant.GetCharacterTemplate_UseSp(charaNo, getSp) == null)
		{
			return 0L;
		}
		return 1L;
	}

	public string GetCharacterStrfromCSVData(long charaTmplNo, CharacterStrData type, bool isSp, long arg2Long)
	{
		CharacterTemplate characterTemplate_UseSp = constant.GetCharacterTemplate_UseSp(charaTmplNo, isSp);
		if (characterTemplate_UseSp == null)
		{
			throw new CodeEE("定義していないキャラクタを参照しようとしました");
		}
		int num = (int)arg2Long;
		switch (type)
		{
		case CharacterStrData.CALLNAME:
			if (characterTemplate_UseSp.Callname != null)
			{
				return characterTemplate_UseSp.Callname;
			}
			return "";
		case CharacterStrData.NAME:
			if (characterTemplate_UseSp.Name != null)
			{
				return characterTemplate_UseSp.Name;
			}
			return "";
		case CharacterStrData.NICKNAME:
			if (characterTemplate_UseSp.Nickname != null)
			{
				return characterTemplate_UseSp.Nickname;
			}
			return "";
		case CharacterStrData.MASTERNAME:
			if (characterTemplate_UseSp.Mastername != null)
			{
				return characterTemplate_UseSp.Mastername;
			}
			return "";
		case CharacterStrData.CSTR:
			if (characterTemplate_UseSp.CStr != null)
			{
				string value = null;
				if (num >= characterTemplate_UseSp.ArrayStrLength(CharacterStrData.CSTR) || num < 0)
				{
					throw new CodeEE("CSTRの参照可能範囲外を参照しました");
				}
				if (characterTemplate_UseSp.CStr.TryGetValue(num, out value))
				{
					return value;
				}
				return "";
			}
			return "";
		default:
			throw new CodeEE("存在しないデータを参照しようとしました");
		}
	}

	public long GetCharacterIntfromCSVData(long charaTmplNo, CharacterIntData type, bool isSp, long arg2Long)
	{
		CharacterTemplate characterTemplate_UseSp = constant.GetCharacterTemplate_UseSp(charaTmplNo, isSp);
		if (characterTemplate_UseSp == null)
		{
			throw new CodeEE("定義していないキャラクタを参照しようとしました");
		}
		if (arg2Long >= characterTemplate_UseSp.ArrayLength(type) || arg2Long < 0)
		{
			throw new CodeEE("参照可能範囲外を参照しました");
		}
		int key = (int)arg2Long;
		Dictionary<int, long> dictionary = null;
		if ((type switch
		{
			CharacterIntData.BASE => characterTemplate_UseSp.Maxbase, 
			CharacterIntData.MARK => characterTemplate_UseSp.Mark, 
			CharacterIntData.ABL => characterTemplate_UseSp.Abl, 
			CharacterIntData.EXP => characterTemplate_UseSp.Exp, 
			CharacterIntData.RELATION => characterTemplate_UseSp.Relation, 
			CharacterIntData.TALENT => characterTemplate_UseSp.Talent, 
			CharacterIntData.CFLAG => characterTemplate_UseSp.CFlag, 
			CharacterIntData.EQUIP => characterTemplate_UseSp.Equip, 
			CharacterIntData.JUEL => characterTemplate_UseSp.Juel, 
			_ => throw new CodeEE("存在しないデータを参照しようとしました"), 
		}).TryGetValue(key, out var value))
		{
			return value;
		}
		return 0L;
	}

	public void UpdateInBeginTrain()
	{
		ASSIPLAY = 0L;
		PREVCOM = -1L;
		NEXTCOM = -1L;
		long[] array = varData.DataIntegerArray[4];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 0L;
		}
		string[] array2 = varData.DataStringArray[6];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = "";
		}
		foreach (CharacterData character in varData.CharacterList)
		{
			array = character.DataIntegerArray[15];
			for (int k = 0; k < array.Length; k++)
			{
				array[k] = 0L;
			}
			array = character.DataIntegerArray[13];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = 0L;
			}
			array = character.DataIntegerArray[8];
			for (int m = 0; m < array.Length; m++)
			{
				array[m] = 0L;
			}
			setDefaultStain(character);
			array = character.DataIntegerArray[6];
			for (int n = 0; n < array.Length; n++)
			{
				array[n] = 0L;
			}
			array = character.DataIntegerArray[7];
			for (int num = 0; num < array.Length; num++)
			{
				array[num] = 0L;
			}
			array = character.DataIntegerArray[20];
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				array[num2] = 0L;
			}
		}
	}

	public void UpdateAfterShowUsercom()
	{
		long[] array = varData.DataIntegerArray[5];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 0L;
		}
		array = varData.DataIntegerArray[9];
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = 0L;
		}
		array = varData.DataIntegerArray[16];
		for (int k = 0; k < array.Length; k++)
		{
			array[k] = 0L;
		}
		foreach (CharacterData character in varData.CharacterList)
		{
			array = character.DataIntegerArray[17];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = 0L;
			}
			array = character.DataIntegerArray[18];
			for (int m = 0; m < array.Length; m++)
			{
				array[m] = 0L;
			}
			array = character.DataIntegerArray[19];
			for (int n = 0; n < array.Length; n++)
			{
				array[n] = 0L;
			}
		}
	}

	public void UpdateAfterInputCom()
	{
		foreach (CharacterData character in varData.CharacterList)
		{
			long[] array = character.DataIntegerArray[16];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 0L;
			}
		}
	}

	public void UpdateAfterSourceCheck()
	{
		foreach (CharacterData character in varData.CharacterList)
		{
			long[] array = character.DataIntegerArray[7];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 0L;
			}
		}
	}

	public void UpdateInUpcheck(EmueraConsole window, bool skipPrint)
	{
		string[] csvNameList = constant.GetCsvNameList(VariableCode.PALAMNAME);
		long[] array = varData.DataIntegerArray[5];
		long[] array2 = varData.DataIntegerArray[9];
		long tARGET = TARGET;
		if (tARGET >= 0 && tARGET < varData.CharacterList.Count)
		{
			long[] array3 = varData.CharacterList[(int)tARGET].DataIntegerArray[6];
			int num = array3.Length;
			if (array3.Length > array.Length)
			{
				num = array.Length;
			}
			if (array3.Length > array2.Length)
			{
				num = array2.Length;
			}
			for (int i = 0; i < num; i++)
			{
				if (array[i] <= 0 && array2[i] <= 0)
				{
					continue;
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (!skipPrint)
				{
					stringBuilder.Append(csvNameList[i]);
					stringBuilder.Append(' ');
					stringBuilder.Append(array3[i].ToString());
					if (array[i] > 0)
					{
						stringBuilder.Append('+');
						stringBuilder.Append(array[i].ToString());
					}
					if (array2[i] > 0)
					{
						stringBuilder.Append('-');
						stringBuilder.Append(array2[i].ToString());
					}
				}
				array3[i] += array[i] - array2[i];
				if (!skipPrint)
				{
					stringBuilder.Append('=');
					stringBuilder.Append(array3[i].ToString());
					window.Print(stringBuilder.ToString());
					window.NewLine();
				}
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = 0L;
		}
		for (int k = 0; k < array2.Length; k++)
		{
			array2[k] = 0L;
		}
	}

	public void CUpdateInUpcheck(EmueraConsole window, long target, bool skipPrint)
	{
		string[] csvNameList = constant.GetCsvNameList(VariableCode.PALAMNAME);
		if (target < 0 || target >= varData.CharacterList.Count)
		{
			return;
		}
		CharacterData characterData = varData.CharacterList[(int)target];
		long[] array = characterData.DataIntegerArray[18];
		long[] array2 = characterData.DataIntegerArray[19];
		long[] array3 = characterData.DataIntegerArray[6];
		int num = array3.Length;
		if (array3.Length > array.Length)
		{
			num = array.Length;
		}
		if (array3.Length > array2.Length)
		{
			num = array2.Length;
		}
		for (int i = 0; i < num; i++)
		{
			if (array[i] <= 0 && array2[i] <= 0)
			{
				continue;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (!skipPrint)
			{
				stringBuilder.Append(csvNameList[i]);
				stringBuilder.Append(' ');
				stringBuilder.Append(array3[i].ToString());
				if (array[i] > 0)
				{
					stringBuilder.Append('+');
					stringBuilder.Append(array[i].ToString());
				}
				if (array2[i] > 0)
				{
					stringBuilder.Append('-');
					stringBuilder.Append(array2[i].ToString());
				}
			}
			array3[i] += array[i] - array2[i];
			if (!skipPrint)
			{
				stringBuilder.Append('=');
				stringBuilder.Append(array3[i].ToString());
				window.Print(stringBuilder.ToString());
				window.NewLine();
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = 0L;
		}
		for (int k = 0; k < array2.Length; k++)
		{
			array2[k] = 0L;
		}
	}

	private void setDefaultStain(CharacterData chara)
	{
		long[] array = chara.DataIntegerArray[14];
		if (array.Length >= Config.StainDefault.Count)
		{
			Config.StainDefault.CopyTo(array);
			for (int i = Config.StainDefault.Count; i < array.Length; i++)
			{
				array[i] = 0L;
			}
		}
		else
		{
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = Config.StainDefault[j];
			}
		}
	}

	public void SetDefaultStain(long no)
	{
		if (no < 0 || no >= varData.CharacterList.Count)
		{
			throw new CodeEE("存在しないキャラクターを参照しようとしました");
		}
		CharacterData defaultStain = varData.CharacterList[(int)no];
		setDefaultStain(defaultStain);
	}

	public void VarSize(VariableToken varID)
	{
		long[] rESULT_ARRAY = RESULT_ARRAY;
		if (varID.IsArray2D)
		{
			rESULT_ARRAY[0] = varID.GetLength(0);
			rESULT_ARRAY[1] = varID.GetLength(1);
		}
		else if (varID.IsArray3D)
		{
			rESULT_ARRAY[0] = varID.GetLength(0);
			rESULT_ARRAY[1] = varID.GetLength(1);
			rESULT_ARRAY[2] = varID.GetLength(2);
		}
		else
		{
			rESULT_ARRAY[0] = varID.GetLength();
		}
	}

	public bool ItemSales(long itemNo)
	{
		long[] iTEMSALES = ITEMSALES;
		string[] csvNameList = constant.GetCsvNameList(VariableCode.ITEMNAME);
		if (itemNo < 0 || itemNo >= iTEMSALES.Length || itemNo >= csvNameList.Length)
		{
			return false;
		}
		int num = (int)itemNo;
		if (iTEMSALES[num] != 0L)
		{
			return csvNameList[num] != null;
		}
		return false;
	}

	public bool BuyItem(long itemNo)
	{
		if (!ItemSales(itemNo))
		{
			return false;
		}
		long[] itemPrice = constant.ItemPrice;
		if (itemNo >= itemPrice.Length)
		{
			return false;
		}
		int num = (int)itemNo;
		if (MONEY < itemPrice[num])
		{
			return false;
		}
		MONEY -= itemPrice[num];
		ITEM[num]++;
		BOUGHT = itemNo;
		return true;
	}

	public void SetEncodingResult(int[] ary)
	{
		long[] array = varData.DataIntegerArray[10];
		array[0] = ary.Length;
		for (int i = 0; i < ary.Length; i++)
		{
			array[i + 1] = ary[i];
		}
	}

	public void IamaMunchkin()
	{
		if (MASTER >= 0 && MASTER < varData.CharacterList.Count)
		{
			varData.CharacterList[(int)MASTER].DataString[0] = "イカサマ";
			varData.CharacterList[(int)MASTER].DataString[1] = "イカサマ";
			varData.CharacterList[(int)MASTER].DataString[2] = "イカサマ";
		}
	}

	public void SetResultX(List<long> values)
	{
		for (int i = 0; i < values.Count && i < varData.DataIntegerArray[10].Length; i++)
		{
			varData.DataIntegerArray[10][i] = values[i];
		}
	}

	private string getSaveDataPathG()
	{
		return Config.SavDir + "global.sav";
	}

	private string getSaveDataPath(int index)
	{
		return $"{Config.SavDir}save{index:00}.sav";
	}

	private string getSaveDataPath(string s)
	{
		return $"{Config.SavDir}save{s:00}.sav";
	}

	private string getSaveDataPathV(int index)
	{
		return Program.DatDir + $"var_{index:00}.dat";
	}

	private string getSaveDataPathC(int index)
	{
		return Program.DatDir + $"chara_{index:00}.dat";
	}

	private string getSaveDataPathV(string s)
	{
		return Program.DatDir + "var_" + s + ".dat";
	}

	private string getSaveDataPathC(string s)
	{
		return Program.DatDir + "chara_" + s + ".dat";
	}

	public void CreateDatFolder()
	{
		if (Directory.Exists(Program.DatDir))
		{
			return;
		}
		try
		{
			Directory.CreateDirectory(Program.DatDir);
		}
		catch
		{
			MessageBox.Show("datフォルダーの作成に失敗しました");
			throw new CodeEE("datフォルダーの作成に失敗しました");
		}
	}

	public List<string> GetDatFiles(bool charadat, string pattern)
	{
		List<string> list = new List<string>();
		if (!Directory.Exists(Program.DatDir))
		{
			return list;
		}
		string searchPattern = "var_" + pattern + ".dat";
		if (charadat)
		{
			searchPattern = "chara_" + pattern + ".dat";
		}
		string[] files = Directory.GetFiles(Program.DatDir, searchPattern, SearchOption.TopDirectoryOnly);
		foreach (string path in files)
		{
			if (Path.GetExtension(path).Equals(".dat", StringComparison.OrdinalIgnoreCase))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
				fileNameWithoutExtension = ((!charadat) ? fileNameWithoutExtension.Substring(4) : fileNameWithoutExtension.Substring(6));
				if (!string.IsNullOrEmpty(fileNameWithoutExtension))
				{
					list.Add(fileNameWithoutExtension);
				}
			}
		}
		return list;
	}

	public string CheckDatFilename(string datfilename)
	{
		if (string.IsNullOrEmpty(datfilename))
		{
			return "ファイル名が指定されていません";
		}
		if (datfilename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
		{
			return "ファイル名に不正な文字が含まれています";
		}
		return null;
	}

	public EraDataResult CheckData(string savename, EraSaveFileType type)
	{
		string filename = null;
		switch (type)
		{
		case EraSaveFileType.Normal:
			filename = getSaveDataPath(savename);
			break;
		case EraSaveFileType.Global:
			filename = getSaveDataPathG();
			break;
		case EraSaveFileType.Var:
			filename = getSaveDataPathV(savename);
			break;
		case EraSaveFileType.CharVar:
			filename = getSaveDataPathC(savename);
			break;
		}
		return CheckDataByFilename(filename, type);
	}

	public EraDataResult CheckData(int saveIndex, EraSaveFileType type)
	{
		string filename = null;
		switch (type)
		{
		case EraSaveFileType.Normal:
			filename = getSaveDataPath(saveIndex);
			break;
		case EraSaveFileType.Global:
			filename = getSaveDataPathG();
			break;
		case EraSaveFileType.Var:
			filename = getSaveDataPathV(saveIndex);
			break;
		case EraSaveFileType.CharVar:
			filename = getSaveDataPathC(saveIndex);
			break;
		}
		return CheckDataByFilename(filename, type);
	}

	public EraDataResult CheckDataByFilename(string filename, EraSaveFileType type)
	{
		EraDataResult eraDataResult = new EraDataResult();
		if (!File.Exists(filename))
		{
			eraDataResult.State = EraDataState.FILENOTFOUND;
			eraDataResult.DataMes = "----";
			return eraDataResult;
		}
		FileStream fileStream = null;
		EraBinaryDataReader eraBinaryDataReader = null;
		EraDataReader eraDataReader = null;
		long num = 0L;
		try
		{
			fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
			eraBinaryDataReader = EraBinaryDataReader.CreateReader(fileStream);
			if (eraBinaryDataReader == null)
			{
				eraDataReader = new EraDataReader(fileStream);
				if (!gamebase.UniqueCodeEqualTo(eraDataReader.ReadInt64()))
				{
					eraDataResult.State = EraDataState.GAME_ERROR;
					eraDataResult.DataMes = "異なるゲームのセーブデータです";
					return eraDataResult;
				}
				num = eraDataReader.ReadInt64();
				if (!gamebase.CheckVersion(num))
				{
					eraDataResult.State = EraDataState.VIRSION_ERROR;
					eraDataResult.DataMes = "セーブデータのバーションが異なります";
					return eraDataResult;
				}
				eraDataResult.State = EraDataState.OK;
				eraDataResult.DataMes = eraDataReader.ReadString();
				return eraDataResult;
			}
			EraSaveFileType eraSaveFileType = eraBinaryDataReader.ReadFileType();
			if (type != eraSaveFileType)
			{
				eraDataResult.State = EraDataState.ETC_ERROR;
				eraDataResult.DataMes = "セーブデータが壊れています";
				return eraDataResult;
			}
			if (!gamebase.UniqueCodeEqualTo(eraBinaryDataReader.ReadInt64()))
			{
				eraDataResult.State = EraDataState.GAME_ERROR;
				eraDataResult.DataMes = "異なるゲームのセーブデータです";
				return eraDataResult;
			}
			num = eraBinaryDataReader.ReadInt64();
			if (!gamebase.CheckVersion(num))
			{
				eraDataResult.State = EraDataState.VIRSION_ERROR;
				eraDataResult.DataMes = "セーブデータのバーションが異なります";
				return eraDataResult;
			}
			eraDataResult.State = EraDataState.OK;
			eraDataResult.DataMes = eraBinaryDataReader.ReadString();
			return eraDataResult;
		}
		catch (FileEE fileEE)
		{
			eraDataResult.State = EraDataState.ETC_ERROR;
			eraDataResult.DataMes = fileEE.Message;
		}
		catch (Exception)
		{
			eraDataResult.State = EraDataState.ETC_ERROR;
			eraDataResult.DataMes = "読み込み中にエラーが発生しました";
		}
		finally
		{
			if (eraDataReader != null)
			{
				eraDataReader.Close();
			}
			else if (eraBinaryDataReader != null)
			{
				eraBinaryDataReader.Close();
			}
			else
			{
				fileStream?.Close();
			}
		}
		return eraDataResult;
	}

	public void SaveChara(string savename, string savMes, int[] charas)
	{
		CreateDatFolder();
		CheckDatFilename(savename);
		string saveDataPathC = getSaveDataPathC(savename);
		EraBinaryDataWriter eraBinaryDataWriter = null;
		FileStream fileStream = null;
		try
		{
			Config.CreateSavDir();
			fileStream = new FileStream(saveDataPathC, FileMode.Create, FileAccess.Write);
			eraBinaryDataWriter = new EraBinaryDataWriter(fileStream);
			eraBinaryDataWriter.WriteHeader();
			eraBinaryDataWriter.WriteFileType(EraSaveFileType.CharVar);
			eraBinaryDataWriter.WriteInt64(gamebase.ScriptUniqueCode);
			eraBinaryDataWriter.WriteInt64(gamebase.ScriptVersion);
			eraBinaryDataWriter.WriteString(savMes);
			eraBinaryDataWriter.WriteInt64(charas.Length);
			for (int i = 0; i < charas.Length; i++)
			{
				varData.CharacterList[charas[i]].SaveToStreamBinary(eraBinaryDataWriter, varData);
			}
			eraBinaryDataWriter.WriteEOF();
		}
		finally
		{
			if (eraBinaryDataWriter != null)
			{
				eraBinaryDataWriter.Close();
			}
			else
			{
				fileStream?.Close();
			}
		}
	}

	public void LoadChara(string savename)
	{
		string saveDataPathC = getSaveDataPathC(savename);
		RESULT = 0L;
		if (!File.Exists(saveDataPathC))
		{
			return;
		}
		EraBinaryDataReader eraBinaryDataReader = null;
		FileStream fileStream = null;
		try
		{
			List<CharacterData> list = new List<CharacterData>();
			fileStream = new FileStream(saveDataPathC, FileMode.Open, FileAccess.Read);
			eraBinaryDataReader = EraBinaryDataReader.CreateReader(fileStream);
			if (eraBinaryDataReader == null || eraBinaryDataReader.ReadFileType() != EraSaveFileType.CharVar || !gamebase.UniqueCodeEqualTo(eraBinaryDataReader.ReadInt64()))
			{
				return;
			}
			long target = eraBinaryDataReader.ReadInt64();
			if (gamebase.CheckVersion(target))
			{
				eraBinaryDataReader.ReadString();
				long num = eraBinaryDataReader.ReadInt64();
				for (int i = 0; i < num; i++)
				{
					CharacterData characterData = new CharacterData(constant, varData);
					characterData.LoadFromStreamBinary(eraBinaryDataReader);
					list.Add(characterData);
				}
				varData.CharacterList.AddRange(list);
				RESULT = 1L;
			}
		}
		finally
		{
			if (eraBinaryDataReader != null)
			{
				eraBinaryDataReader.Close();
			}
			else
			{
				fileStream?.Close();
			}
		}
	}

	public void SaveVariable(string savename, string savMes, VariableToken[] vars)
	{
		CreateDatFolder();
		CheckDatFilename(savename);
		string saveDataPathV = getSaveDataPathV(savename);
		EraBinaryDataWriter eraBinaryDataWriter = null;
		FileStream fileStream = null;
		try
		{
			Config.CreateSavDir();
			fileStream = new FileStream(saveDataPathV, FileMode.Create, FileAccess.Write);
			eraBinaryDataWriter = new EraBinaryDataWriter(fileStream);
			eraBinaryDataWriter.WriteHeader();
			eraBinaryDataWriter.WriteFileType(EraSaveFileType.Var);
			eraBinaryDataWriter.WriteInt64(gamebase.ScriptUniqueCode);
			eraBinaryDataWriter.WriteInt64(gamebase.ScriptVersion);
			eraBinaryDataWriter.WriteString(savMes);
			for (int i = 0; i < vars.Length; i++)
			{
				eraBinaryDataWriter.WriteWithKey(vars[i].Name, vars[i].GetArray());
			}
			eraBinaryDataWriter.WriteEOF();
		}
		finally
		{
			if (eraBinaryDataWriter != null)
			{
				eraBinaryDataWriter.Close();
			}
			else
			{
				fileStream?.Close();
			}
		}
	}

	public void LoadVariable(string savename)
	{
		string saveDataPathV = getSaveDataPathV(savename);
		RESULT = 0L;
		if (!File.Exists(saveDataPathV))
		{
			return;
		}
		EraBinaryDataReader eraBinaryDataReader = null;
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(saveDataPathV, FileMode.Open, FileAccess.Read);
			eraBinaryDataReader = EraBinaryDataReader.CreateReader(fileStream);
			if (eraBinaryDataReader == null || eraBinaryDataReader.ReadFileType() != EraSaveFileType.Var || !gamebase.UniqueCodeEqualTo(eraBinaryDataReader.ReadInt64()))
			{
				return;
			}
			long target = eraBinaryDataReader.ReadInt64();
			if (gamebase.CheckVersion(target))
			{
				eraBinaryDataReader.ReadString();
				while (varData.LoadVariableBinary(eraBinaryDataReader))
				{
				}
				RESULT = 1L;
			}
		}
		finally
		{
			if (eraBinaryDataReader != null)
			{
				eraBinaryDataReader.Close();
			}
			else
			{
				fileStream?.Close();
			}
		}
	}

	public void SaveToStream(EraDataWriter writer, string saveDataText)
	{
		writer.Write(gamebase.ScriptUniqueCode);
		writer.Write(gamebase.ScriptVersion);
		writer.Write(saveDataText);
		writer.Write(varData.CharacterList.Count);
		for (int i = 0; i < varData.CharacterList.Count; i++)
		{
			varData.CharacterList[i].SaveToStream(writer);
		}
		varData.SaveToStream(writer);
		writer.EmuStart();
		for (int j = 0; j < varData.CharacterList.Count; j++)
		{
			varData.CharacterList[j].SaveToStreamExtended(writer);
		}
		varData.SaveToStreamExtended(writer);
	}

	public void LoadFromStream(EraDataReader reader)
	{
		if (!gamebase.UniqueCodeEqualTo(reader.ReadInt64()))
		{
			throw new FileEE("異なるゲームのセーブデータです");
		}
		long num = reader.ReadInt64();
		if (!gamebase.CheckVersion(num))
		{
			throw new FileEE("セーブデータのバーションが異なります");
		}
		string lastLoadText = reader.ReadString();
		varData.SetDefaultValue(constant);
		varData.SetDefaultLocalValue();
		varData.LastLoadVersion = num;
		varData.LastLoadText = lastLoadText;
		int num2 = (int)reader.ReadInt64();
		varData.CharacterList.Clear();
		for (int i = 0; i < num2; i++)
		{
			CharacterData characterData = new CharacterData(constant, varData);
			varData.CharacterList.Add(characterData);
			characterData.LoadFromStream(reader);
		}
		varData.LoadFromStream(reader);
		if (!reader.SeekEmuStart())
		{
			return;
		}
		if (reader.DataVersion < 1803)
		{
			for (int j = 0; j < num2; j++)
			{
				varData.CharacterList[j].LoadFromStreamExtended_Old1802(reader);
			}
		}
		else
		{
			for (int k = 0; k < num2; k++)
			{
				varData.CharacterList[k].LoadFromStreamExtended(reader);
			}
		}
		varData.LoadFromStreamExtended(reader, reader.DataVersion);
	}

	public bool SaveGlobal()
	{
		string saveDataPathG = getSaveDataPathG();
		EraDataWriter eraDataWriter = null;
		EraBinaryDataWriter eraBinaryDataWriter = null;
		FileStream fileStream = null;
		try
		{
			Config.CreateSavDir();
			fileStream = new FileStream(saveDataPathG, FileMode.Create, FileAccess.Write);
			if (Config.SystemSaveInBinary)
			{
				eraBinaryDataWriter = new EraBinaryDataWriter(fileStream);
				eraBinaryDataWriter.WriteHeader();
				eraBinaryDataWriter.WriteFileType(EraSaveFileType.Global);
				eraBinaryDataWriter.WriteInt64(gamebase.ScriptUniqueCode);
				eraBinaryDataWriter.WriteInt64(gamebase.ScriptVersion);
				eraBinaryDataWriter.WriteString("");
				varData.SaveGlobalToStreamBinary(eraBinaryDataWriter);
				eraBinaryDataWriter.WriteEOF();
			}
			else
			{
				eraDataWriter = new EraDataWriter(fileStream);
				eraDataWriter.Write(gamebase.ScriptUniqueCode);
				eraDataWriter.Write(gamebase.ScriptVersion);
				varData.SaveGlobalToStream(eraDataWriter);
				eraDataWriter.EmuStart();
				varData.SaveGlobalToStream1808(eraDataWriter);
			}
		}
		finally
		{
			if (eraDataWriter != null)
			{
				eraDataWriter.Close();
			}
			else if (eraBinaryDataWriter != null)
			{
				eraBinaryDataWriter.Close();
			}
			else
			{
				fileStream?.Close();
			}
		}
		return true;
	}

	public bool LoadGlobal()
	{
		string saveDataPathG = getSaveDataPathG();
		if (!File.Exists(saveDataPathG))
		{
			return false;
		}
		EraDataReader eraDataReader = null;
		EraBinaryDataReader eraBinaryDataReader = null;
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(saveDataPathG, FileMode.Open, FileAccess.Read);
			eraBinaryDataReader = EraBinaryDataReader.CreateReader(fileStream);
			if (eraBinaryDataReader != null)
			{
				if (eraBinaryDataReader.ReadFileType() != EraSaveFileType.Global)
				{
					return false;
				}
				if (!gamebase.UniqueCodeEqualTo(eraBinaryDataReader.ReadInt64()))
				{
					return false;
				}
				long target = eraBinaryDataReader.ReadInt64();
				if (!gamebase.CheckVersion(target))
				{
					return false;
				}
				eraBinaryDataReader.ReadString();
				varData.LoadFromStreamBinary(eraBinaryDataReader);
			}
			else
			{
				eraDataReader = new EraDataReader(fileStream);
				if (!gamebase.UniqueCodeEqualTo(eraDataReader.ReadInt64()))
				{
					return false;
				}
				long target2 = eraDataReader.ReadInt64();
				if (!gamebase.CheckVersion(target2))
				{
					return false;
				}
				varData.LoadGlobalFromStream(eraDataReader);
				if (eraDataReader.SeekEmuStart())
				{
					varData.LoadGlobalFromStream1808(eraDataReader);
				}
			}
			return true;
		}
		catch
		{
			return false;
		}
		finally
		{
			if (eraDataReader != null)
			{
				eraDataReader.Close();
			}
			else if (eraBinaryDataReader != null)
			{
				eraBinaryDataReader.Close();
			}
			else
			{
				fileStream?.Close();
			}
		}
	}

	public void SaveToStreamBinary(EraBinaryDataWriter bWriter, string saveDataText)
	{
		bWriter.WriteHeader();
		bWriter.WriteFileType(EraSaveFileType.Normal);
		bWriter.WriteInt64(gamebase.ScriptUniqueCode);
		bWriter.WriteInt64(gamebase.ScriptVersion);
		bWriter.WriteString(saveDataText);
		bWriter.WriteInt64(varData.CharacterList.Count);
		for (int i = 0; i < varData.CharacterList.Count; i++)
		{
			varData.CharacterList[i].SaveToStreamBinary(bWriter, varData);
		}
		varData.SaveToStreamBinary(bWriter);
		bWriter.WriteEOF();
	}

	public void LoadFromStreamBinary(EraBinaryDataReader bReader)
	{
		if (bReader.ReadFileType() != EraSaveFileType.Normal)
		{
			throw new FileEE("セーブデータが壊れています");
		}
		if (!gamebase.UniqueCodeEqualTo(bReader.ReadInt64()))
		{
			throw new FileEE("異なるゲームのセーブデータです");
		}
		long num = bReader.ReadInt64();
		if (!gamebase.CheckVersion(num))
		{
			throw new FileEE("セーブデータのバーションが異なります");
		}
		string lastLoadText = bReader.ReadString();
		varData.SetDefaultValue(constant);
		varData.SetDefaultLocalValue();
		varData.LastLoadVersion = num;
		varData.LastLoadText = lastLoadText;
		int num2 = (int)bReader.ReadInt64();
		varData.CharacterList.Clear();
		for (int i = 0; i < num2; i++)
		{
			CharacterData characterData = new CharacterData(constant, varData);
			varData.CharacterList.Add(characterData);
			characterData.LoadFromStreamBinary(bReader);
		}
		varData.LoadFromStreamBinary(bReader);
	}

	public bool SaveTo(int saveIndex, string saveText)
	{
		string saveDataPath = getSaveDataPath(saveIndex);
		FileStream fileStream = null;
		EraDataWriter eraDataWriter = null;
		EraBinaryDataWriter eraBinaryDataWriter = null;
		try
		{
			Config.CreateSavDir();
			fileStream = new FileStream(saveDataPath, FileMode.Create, FileAccess.Write);
			if (Config.SystemSaveInBinary)
			{
				eraBinaryDataWriter = new EraBinaryDataWriter(fileStream);
				SaveToStreamBinary(eraBinaryDataWriter, saveText);
			}
			else
			{
				eraDataWriter = new EraDataWriter(fileStream);
				SaveToStream(eraDataWriter, saveText);
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
		finally
		{
			if (eraDataWriter != null)
			{
				eraDataWriter.Close();
			}
			else if (eraBinaryDataWriter != null)
			{
				eraBinaryDataWriter.Close();
			}
			else
			{
				fileStream?.Close();
			}
		}
	}

	public bool LoadFrom(int dataIndex)
	{
		string saveDataPath = getSaveDataPath(dataIndex);
		if (!File.Exists(saveDataPath))
		{
			throw new ExeEE("存在しないパスを呼び出した");
		}
		EraDataReader eraDataReader = null;
		EraBinaryDataReader eraBinaryDataReader = null;
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(saveDataPath, FileMode.Open, FileAccess.Read);
			eraBinaryDataReader = EraBinaryDataReader.CreateReader(fileStream);
			if (eraBinaryDataReader != null)
			{
				LoadFromStreamBinary(eraBinaryDataReader);
			}
			else
			{
				eraDataReader = new EraDataReader(fileStream);
				LoadFromStream(eraDataReader);
			}
			varData.LastLoadNo = dataIndex;
		}
		finally
		{
			if (eraDataReader != null)
			{
				eraDataReader.Close();
			}
			else if (eraBinaryDataReader != null)
			{
				eraBinaryDataReader.Close();
			}
			else
			{
				fileStream?.Close();
			}
		}
		return true;
	}

	public void DelData(int dataIndex)
	{
		string saveDataPath = getSaveDataPath(dataIndex);
		if (File.Exists(saveDataPath))
		{
			if ((File.GetAttributes(saveDataPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
			{
				throw new CodeEE("指定されたファイル\"" + saveDataPath + "\"は読み込み専用のため削除できません");
			}
			File.Delete(saveDataPath);
		}
	}

	public void Dispose()
	{
		varData.Dispose();
	}

	private long get_Variable_canforbid(VariableCode code)
	{
		long[] array = varData.DataIntegerArray[(int)(code & VariableCode.__LOWERCASE__)];
		if (array.Length == 0)
		{
			return -1L;
		}
		return array[0];
	}

	private void set_Variable_canforbid(VariableCode code, long value)
	{
		long[] array = varData.DataIntegerArray[(int)(code & VariableCode.__LOWERCASE__)];
		if (array.Length != 0)
		{
			array[0] = value;
		}
	}
}
