using System;
using System.Collections.Generic;
using MinorShift.Emuera.GameProc;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal sealed class CharacterData : IDisposable
{
	private readonly long[] dataInteger;

	private readonly string[] dataString;

	private readonly long[][] dataIntegerArray;

	private readonly string[][] dataStringArray;

	private readonly long[][,] dataIntegerArray2D;

	private readonly string[][,] dataStringArray2D;

	private const int strCount = 2;

	private const int intCount = 2;

	private const int intArrayCount = 17;

	private const int strArrayCount = 0;

	public IComparable temp_SortKey;

	public int temp_CurrentOrder;

	public long[] DataInteger => dataInteger;

	public string[] DataString => dataString;

	public long[][] DataIntegerArray => dataIntegerArray;

	public string[][] DataStringArray => dataStringArray;

	public long[][,] DataIntegerArray2D => dataIntegerArray2D;

	public string[][,] DataStringArray2D => dataStringArray2D;

	public List<object> UserDefCVarDataList { get; set; }

	public long[] CFlag => dataIntegerArray[9];

	public long NO => dataInteger[1];

	public CharacterData(ConstantData constant, VariableData varData)
	{
		dataInteger = new long[2];
		dataString = new string[4];
		dataIntegerArray = new long[84][];
		dataStringArray = new string[1][];
		dataIntegerArray2D = new long[1][,];
		dataStringArray2D = new string[0][,];
		for (int i = 0; i < dataIntegerArray.Length; i++)
		{
			dataIntegerArray[i] = new long[constant.CharacterIntArrayLength[i]];
		}
		for (int j = 0; j < dataStringArray.Length; j++)
		{
			dataStringArray[j] = new string[constant.CharacterStrArrayLength[j]];
		}
		for (int k = 0; k < dataIntegerArray2D.Length; k++)
		{
			long num = constant.CharacterIntArray2DLength[k];
			int num2 = (int)(num >> 32);
			int num3 = (int)(num & 0x7FFFFFFF);
			dataIntegerArray2D[k] = new long[num2, num3];
		}
		for (int l = 0; l < dataStringArray2D.Length; l++)
		{
			long num4 = constant.CharacterStrArray2DLength[l];
			int num5 = (int)(num4 >> 32);
			int num6 = (int)(num4 & 0x7FFFFFFF);
			dataStringArray2D[l] = new string[num5, num6];
		}
		UserDefCVarDataList = new List<object>();
		for (int m = 0; m < varData.UserDefinedCharaVarList.Count; m++)
		{
			UserDefinedVariableData dimData = varData.UserDefinedCharaVarList[m].DimData;
			object obj = null;
			if (dimData.TypeIsStr)
			{
				switch (dimData.Dimension)
				{
				case 1:
					obj = new string[dimData.Lengths[0]];
					break;
				case 2:
					obj = new string[dimData.Lengths[0], dimData.Lengths[1]];
					break;
				case 3:
					obj = new string[dimData.Lengths[0], dimData.Lengths[1], dimData.Lengths[2]];
					break;
				}
			}
			else
			{
				switch (dimData.Dimension)
				{
				case 1:
					obj = new long[dimData.Lengths[0]];
					break;
				case 2:
					obj = new long[dimData.Lengths[0], dimData.Lengths[1]];
					break;
				case 3:
					obj = new long[dimData.Lengths[0], dimData.Lengths[1], dimData.Lengths[2]];
					break;
				}
			}
			if (obj == null)
			{
				throw new ExeEE("");
			}
			UserDefCVarDataList.Add(obj);
		}
	}

	public CharacterData(ConstantData constant, CharacterTemplate tmpl, VariableData varData)
		: this(constant, varData)
	{
		dataInteger[1] = tmpl.No;
		dataString[0] = tmpl.Name;
		dataString[1] = tmpl.Callname;
		dataString[2] = tmpl.Nickname;
		dataString[3] = tmpl.Mastername;
		long[] array = dataIntegerArray[1];
		long[] array2 = dataIntegerArray[0];
		foreach (KeyValuePair<int, long> item in tmpl.Maxbase)
		{
			array[item.Key] = item.Value;
			array2[item.Key] = item.Value;
		}
		array = dataIntegerArray[5];
		foreach (KeyValuePair<int, long> item2 in tmpl.Mark)
		{
			array[item2.Key] = item2.Value;
		}
		array = dataIntegerArray[4];
		foreach (KeyValuePair<int, long> item3 in tmpl.Exp)
		{
			array[item3.Key] = item3.Value;
		}
		array = dataIntegerArray[2];
		foreach (KeyValuePair<int, long> item4 in tmpl.Abl)
		{
			array[item4.Key] = item4.Value;
		}
		array = dataIntegerArray[3];
		foreach (KeyValuePair<int, long> item5 in tmpl.Talent)
		{
			array[item5.Key] = item5.Value;
		}
		array = dataIntegerArray[11];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Config.RelationDef;
		}
		foreach (KeyValuePair<int, long> item6 in tmpl.Relation)
		{
			array[item6.Key] = item6.Value;
		}
		array = dataIntegerArray[9];
		foreach (KeyValuePair<int, long> item7 in tmpl.CFlag)
		{
			array[item7.Key] = item7.Value;
		}
		array = dataIntegerArray[12];
		foreach (KeyValuePair<int, long> item8 in tmpl.Equip)
		{
			array[item8.Key] = item8.Value;
		}
		array = dataIntegerArray[10];
		foreach (KeyValuePair<int, long> item9 in tmpl.Juel)
		{
			array[item9.Key] = item9.Value;
		}
		string[] array3 = dataStringArray[0];
		foreach (KeyValuePair<int, string> item10 in tmpl.CStr)
		{
			array3[item10.Key] = item10.Value;
		}
	}

	public static int[] CharacterVarLength(VariableCode code, ConstantData constant)
	{
		int[] array = null;
		VariableCode variableCode = code & (VariableCode)672006144;
		int num = (int)(code & VariableCode.__LOWERCASE__);
		if (num >= 240)
		{
			return null;
		}
		long num2 = 0L;
		switch (variableCode)
		{
		case VariableCode.__INTEGER__:
		case VariableCode.__STRING__:
			array = new int[0];
			break;
		case (VariableCode)655360:
			array = new int[1] { constant.CharacterIntArrayLength[num] };
			break;
		case (VariableCode)786432:
			array = new int[1] { constant.CharacterStrArrayLength[num] };
			break;
		case (VariableCode)134348800:
			array = new int[2];
			num2 = constant.CharacterIntArray2DLength[num];
			array[0] = (int)(num2 >> 32);
			array[1] = (int)(num2 & 0x7FFFFFFF);
			break;
		case (VariableCode)134479872:
			array = new int[2];
			num2 = constant.CharacterStrArray2DLength[num];
			array[0] = (int)(num2 >> 32);
			array[1] = (int)(num2 & 0x7FFFFFFF);
			break;
		case (VariableCode)537001984:
			throw new NotImplCodeEE();
		case (VariableCode)537133056:
			throw new NotImplCodeEE();
		}
		return array;
	}

	public void CopyTo(CharacterData other)
	{
		for (int i = 0; i < dataInteger.Length; i++)
		{
			other.dataInteger[i] = dataInteger[i];
		}
		for (int j = 0; j < dataString.Length; j++)
		{
			other.dataString[j] = dataString[j];
		}
		for (int k = 0; k < dataIntegerArray.Length; k++)
		{
			for (int l = 0; l < dataIntegerArray[k].Length; l++)
			{
				other.dataIntegerArray[k][l] = dataIntegerArray[k][l];
			}
		}
		for (int m = 0; m < dataStringArray.Length; m++)
		{
			for (int n = 0; n < dataStringArray[m].Length; n++)
			{
				other.dataStringArray[m][n] = dataStringArray[m][n];
			}
		}
		for (int num = 0; num < dataIntegerArray2D.Length; num++)
		{
			int length = dataIntegerArray2D[num].GetLength(0);
			int length2 = dataIntegerArray2D[num].GetLength(1);
			for (int num2 = 0; num2 < length; num2++)
			{
				for (int num3 = 0; num3 < length2; num3++)
				{
					other.dataIntegerArray2D[num][num2, num3] = dataIntegerArray2D[num][num2, num3];
				}
			}
		}
		for (int num4 = 0; num4 < dataStringArray2D.Length; num4++)
		{
			int length3 = dataStringArray2D[num4].GetLength(0);
			int length4 = dataStringArray2D[num4].GetLength(1);
			for (int num5 = 0; num5 < length3; num5++)
			{
				for (int num6 = 0; num6 < length4; num6++)
				{
					other.dataStringArray2D[num4][num5, num6] = dataStringArray2D[num4][num5, num6];
				}
			}
		}
	}

	public void SaveToStream(EraDataWriter writer)
	{
		for (int i = 0; i < 2; i++)
		{
			writer.Write(dataString[i]);
		}
		for (int j = 0; j < 2; j++)
		{
			writer.Write(dataInteger[j]);
		}
		for (int k = 0; k < 17; k++)
		{
			writer.Write(dataIntegerArray[k]);
		}
		for (int l = 0; l < 0; l++)
		{
			writer.Write(dataStringArray[l]);
		}
	}

	public void LoadFromStream(EraDataReader reader)
	{
		for (int i = 0; i < 2; i++)
		{
			dataString[i] = reader.ReadString();
		}
		for (int j = 0; j < 2; j++)
		{
			dataInteger[j] = reader.ReadInt64();
		}
		for (int k = 0; k < 17; k++)
		{
			reader.ReadInt64Array(dataIntegerArray[k]);
		}
		for (int l = 0; l < 0; l++)
		{
			reader.ReadStringArray(dataStringArray[l]);
		}
	}

	public void SaveToStreamExtended(EraDataWriter writer)
	{
		foreach (VariableCode extSave in VariableIdentifier.GetExtSaveList(VariableCode.NAME))
		{
			writer.WriteExtended(extSave.ToString(), dataString[(int)(VariableCode.__LOWERCASE__ & extSave)]);
		}
		writer.EmuSeparete();
		foreach (VariableCode extSave2 in VariableIdentifier.GetExtSaveList(VariableCode.ISASSI))
		{
			writer.WriteExtended(extSave2.ToString(), dataInteger[(int)(VariableCode.__LOWERCASE__ & extSave2)]);
		}
		writer.EmuSeparete();
		foreach (VariableCode extSave3 in VariableIdentifier.GetExtSaveList((VariableCode)1835008))
		{
			writer.WriteExtended(extSave3.ToString(), dataStringArray[(int)(VariableCode.__LOWERCASE__ & extSave3)]);
		}
		writer.EmuSeparete();
		foreach (VariableCode extSave4 in VariableIdentifier.GetExtSaveList((VariableCode)1703936))
		{
			writer.WriteExtended(extSave4.ToString(), dataIntegerArray[(int)(VariableCode.__LOWERCASE__ & extSave4)]);
		}
		writer.EmuSeparete();
		foreach (VariableCode extSave5 in VariableIdentifier.GetExtSaveList((VariableCode)135528448))
		{
			writer.WriteExtended(extSave5.ToString(), dataStringArray2D[(int)(VariableCode.__LOWERCASE__ & extSave5)]);
		}
		writer.EmuSeparete();
		foreach (VariableCode extSave6 in VariableIdentifier.GetExtSaveList((VariableCode)135397376))
		{
			writer.WriteExtended(extSave6.ToString(), dataIntegerArray2D[(int)(VariableCode.__LOWERCASE__ & extSave6)]);
		}
		writer.EmuSeparete();
	}

	public void LoadFromStreamExtended(EraDataReader reader)
	{
		Dictionary<string, string> dictionary = reader.ReadStringExtended();
		Dictionary<string, long> dictionary2 = reader.ReadInt64Extended();
		Dictionary<string, List<string>> dictionary3 = reader.ReadStringArrayExtended();
		Dictionary<string, List<long>> dictionary4 = reader.ReadInt64ArrayExtended();
		Dictionary<string, List<string[]>> dictionary5 = reader.ReadStringArray2DExtended();
		Dictionary<string, List<long[]>> dictionary6 = reader.ReadInt64Array2DExtended();
		foreach (VariableCode extSave in VariableIdentifier.GetExtSaveList(VariableCode.NAME))
		{
			if (dictionary.ContainsKey(extSave.ToString()))
			{
				dataString[(int)(VariableCode.__LOWERCASE__ & extSave)] = dictionary[extSave.ToString()];
			}
		}
		foreach (VariableCode extSave2 in VariableIdentifier.GetExtSaveList(VariableCode.ISASSI))
		{
			if (dictionary2.ContainsKey(extSave2.ToString()))
			{
				dataInteger[(int)(VariableCode.__LOWERCASE__ & extSave2)] = dictionary2[extSave2.ToString()];
			}
		}
		foreach (VariableCode extSave3 in VariableIdentifier.GetExtSaveList((VariableCode)1835008))
		{
			if (dictionary3.ContainsKey(extSave3.ToString()))
			{
				copyListToArray(dictionary3[extSave3.ToString()], dataStringArray[(int)(VariableCode.__LOWERCASE__ & extSave3)]);
			}
		}
		foreach (VariableCode extSave4 in VariableIdentifier.GetExtSaveList((VariableCode)1703936))
		{
			if (dictionary4.ContainsKey(extSave4.ToString()))
			{
				copyListToArray(dictionary4[extSave4.ToString()], dataIntegerArray[(int)(VariableCode.__LOWERCASE__ & extSave4)]);
			}
		}
		foreach (VariableCode extSave5 in VariableIdentifier.GetExtSaveList((VariableCode)135528448))
		{
			if (dictionary6.ContainsKey(extSave5.ToString()))
			{
				copyListToArray2D(dictionary5[extSave5.ToString()], dataStringArray2D[(int)(VariableCode.__LOWERCASE__ & extSave5)]);
			}
		}
		foreach (VariableCode extSave6 in VariableIdentifier.GetExtSaveList((VariableCode)135397376))
		{
			if (dictionary6.ContainsKey(extSave6.ToString()))
			{
				copyListToArray2D(dictionary6[extSave6.ToString()], dataIntegerArray2D[(int)(VariableCode.__LOWERCASE__ & extSave6)]);
			}
		}
	}

	public void LoadFromStreamExtended_Old1802(EraDataReader reader)
	{
		Dictionary<string, string> dictionary = reader.ReadStringExtended();
		Dictionary<string, long> dictionary2 = reader.ReadInt64Extended();
		Dictionary<string, List<string>> dictionary3 = reader.ReadStringArrayExtended();
		Dictionary<string, List<long>> dictionary4 = reader.ReadInt64ArrayExtended();
		foreach (VariableCode extSave in VariableIdentifier.GetExtSaveList(VariableCode.NAME))
		{
			if (dictionary.ContainsKey(extSave.ToString()))
			{
				dataString[(int)(VariableCode.__LOWERCASE__ & extSave)] = dictionary[extSave.ToString()];
			}
		}
		foreach (VariableCode extSave2 in VariableIdentifier.GetExtSaveList(VariableCode.ISASSI))
		{
			if (dictionary2.ContainsKey(extSave2.ToString()))
			{
				dataInteger[(int)(VariableCode.__LOWERCASE__ & extSave2)] = dictionary2[extSave2.ToString()];
			}
		}
		foreach (VariableCode extSave3 in VariableIdentifier.GetExtSaveList((VariableCode)1835008))
		{
			if (dictionary3.ContainsKey(extSave3.ToString()))
			{
				copyListToArray(dictionary3[extSave3.ToString()], dataStringArray[(int)(VariableCode.__LOWERCASE__ & extSave3)]);
			}
		}
		foreach (VariableCode extSave4 in VariableIdentifier.GetExtSaveList((VariableCode)1703936))
		{
			if (dictionary4.ContainsKey(extSave4.ToString()))
			{
				copyListToArray(dictionary4[extSave4.ToString()], dataIntegerArray[(int)(VariableCode.__LOWERCASE__ & extSave4)]);
			}
		}
	}

	public void SaveToStreamBinary(EraBinaryDataWriter writer, VariableData varData)
	{
		foreach (KeyValuePair<string, VariableToken> item in varData.GetVarTokenDic())
		{
			VariableToken value = item.Value;
			if (value.IsSavedata && value.IsCharacterData && !value.IsGlobal)
			{
				VariableCode code = value.Code;
				VariableCode variableCode = code & (VariableCode)672006144;
				int codeInt = value.CodeInt;
				switch (variableCode)
				{
				case VariableCode.__INTEGER__:
					writer.WriteWithKey(code.ToString(), dataInteger[codeInt]);
					break;
				case VariableCode.__STRING__:
					writer.WriteWithKey(code.ToString(), dataString[codeInt]);
					break;
				case (VariableCode)655360:
					writer.WriteWithKey(code.ToString(), dataIntegerArray[codeInt]);
					break;
				case (VariableCode)786432:
					writer.WriteWithKey(code.ToString(), dataStringArray[codeInt]);
					break;
				case (VariableCode)134348800:
					writer.WriteWithKey(code.ToString(), dataIntegerArray2D[codeInt]);
					break;
				case (VariableCode)134479872:
					writer.WriteWithKey(code.ToString(), dataStringArray2D[codeInt]);
					break;
				}
			}
		}
		if (UserDefCVarDataList.Count != 0)
		{
			writer.WriteSeparator();
			foreach (UserDefinedCharaVariableToken userDefinedCharaVar in varData.UserDefinedCharaVarList)
			{
				if (userDefinedCharaVar.IsSavedata && userDefinedCharaVar.IsCharacterData && !userDefinedCharaVar.IsGlobal)
				{
					writer.WriteWithKey(userDefinedCharaVar.Name, UserDefCVarDataList[userDefinedCharaVar.ArrayIndex]);
				}
			}
		}
		writer.WriteEOC();
	}

	public void LoadFromStreamBinary(EraBinaryDataReader reader)
	{
		int num = 0;
		bool flag = false;
		while (true)
		{
			KeyValuePair<string, EraSaveDataType> keyValuePair = reader.ReadVariableCode();
			VariableToken variableToken = null;
			object obj = null;
			if (keyValuePair.Key != null)
			{
				variableToken = GlobalStatic.IdentifierDictionary.GetVariableToken(keyValuePair.Key, null, allowPrivate: false);
				if (flag)
				{
					obj = ((variableToken != null && variableToken.IsSavedata && variableToken.IsCharacterData && variableToken is UserDefinedCharaVariableToken) ? UserDefCVarDataList[((UserDefinedCharaVariableToken)variableToken).ArrayIndex] : null);
					variableToken = null;
				}
				else
				{
					num = (int)(VariableCode.__LOWERCASE__ & variableToken.Code);
					obj = null;
				}
			}
			switch (keyValuePair.Value)
			{
			case EraSaveDataType.Separator:
				flag = true;
				break;
			case EraSaveDataType.Int:
				if (variableToken == null || !variableToken.IsInteger || variableToken.Dimension != 0)
				{
					reader.ReadInt();
				}
				else
				{
					dataInteger[num] = reader.ReadInt();
				}
				break;
			case EraSaveDataType.Str:
				if (variableToken == null || !variableToken.IsString || variableToken.Dimension != 0)
				{
					reader.ReadString();
				}
				else
				{
					dataString[num] = reader.ReadString();
				}
				break;
			case EraSaveDataType.IntArray:
				if (flag && obj != null)
				{
					reader.ReadIntArray(obj as long[], needInit: true);
				}
				else if (variableToken == null || !variableToken.IsInteger || variableToken.Dimension != 1)
				{
					reader.ReadIntArray(null, needInit: true);
				}
				else
				{
					reader.ReadIntArray(dataIntegerArray[num], needInit: true);
				}
				break;
			case EraSaveDataType.StrArray:
				if (flag && obj != null)
				{
					reader.ReadStrArray(obj as string[], needInit: true);
				}
				else if (variableToken == null || !variableToken.IsString || variableToken.Dimension != 1)
				{
					reader.ReadStrArray(null, needInit: true);
				}
				else
				{
					reader.ReadStrArray(dataStringArray[num], needInit: true);
				}
				break;
			case EraSaveDataType.IntArray2D:
				if (flag && obj != null)
				{
					reader.ReadIntArray2D(obj as long[,], needInit: true);
				}
				else if (variableToken == null || !variableToken.IsInteger || variableToken.Dimension != 2)
				{
					reader.ReadIntArray2D(null, needInit: true);
				}
				else
				{
					reader.ReadIntArray2D(dataIntegerArray2D[num], needInit: true);
				}
				break;
			case EraSaveDataType.StrArray2D:
				if (flag && obj != null)
				{
					reader.ReadStrArray2D(obj as string[,], needInit: true);
				}
				else if (variableToken == null || !variableToken.IsString || variableToken.Dimension != 2)
				{
					reader.ReadStrArray2D(null, needInit: true);
				}
				else
				{
					reader.ReadStrArray2D(dataStringArray2D[num], needInit: true);
				}
				break;
			default:
				throw new FileEE("データ異常");
			case EraSaveDataType.EOC:
			case EraSaveDataType.EOF:
				return;
			}
		}
	}

	private void copyListToArray<T>(List<T> srcList, T[] destArray)
	{
		int count = Math.Min(srcList.Count, destArray.Length);
		srcList.CopyTo(0, destArray, 0, count);
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

	public void setValueAll(int varInt, long value)
	{
		dataInteger[varInt] = value;
	}

	public void setValueAll(int varInt, string value)
	{
		dataString[varInt] = value;
	}

	public void setValueAll1D(int varInt, long value, int start, int end)
	{
		long[] array = dataIntegerArray[varInt];
		for (int i = start; i < end; i++)
		{
			array[i] = value;
		}
	}

	public void setValueAll1D(int varInt, string value, int start, int end)
	{
		string[] array = dataStringArray[varInt];
		for (int i = start; i < end; i++)
		{
			array[i] = value;
		}
	}

	public void setValueAll2D(int varInt, long value)
	{
		long[,] array = dataIntegerArray2D[varInt];
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

	public void setValueAll2D(int varInt, string value)
	{
		string[,] array = dataStringArray2D[varInt];
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

	public void Dispose()
	{
		for (int i = 0; i < dataIntegerArray.Length; i++)
		{
			dataIntegerArray[i] = null;
		}
		for (int j = 0; j < dataStringArray.Length; j++)
		{
			dataStringArray[j] = null;
		}
		for (int k = 0; k < dataIntegerArray2D.Length; k++)
		{
			dataIntegerArray2D[k] = null;
		}
	}

	public static int AscCharacterComparison(CharacterData x, CharacterData y)
	{
		int num = x.temp_SortKey.CompareTo(y.temp_SortKey);
		if (num != 0)
		{
			return num;
		}
		return x.temp_CurrentOrder.CompareTo(y.temp_CurrentOrder);
	}

	public static int DescCharacterComparison(CharacterData x, CharacterData y)
	{
		int num = x.temp_SortKey.CompareTo(y.temp_SortKey);
		if (num != 0)
		{
			return -num;
		}
		return x.temp_CurrentOrder.CompareTo(y.temp_CurrentOrder);
	}

	public void SetSortKey(VariableToken sortkey, long elem64)
	{
		if (sortkey.IsString)
		{
			if (sortkey.IsArray2D)
			{
				string[,] array = dataStringArray2D[sortkey.CodeInt];
				int num = (int)(elem64 >> 32);
				int num2 = (int)(elem64 & 0x7FFFFFFF);
				if (num < 0 || num >= array.GetLength(0) || num2 < 0 || num2 >= array.GetLength(1))
				{
					throw new CodeEE("ソートキーが配列外を参照しています");
				}
				temp_SortKey = array[num, num2];
			}
			else if (sortkey.IsArray1D)
			{
				string[] array2 = dataStringArray[sortkey.CodeInt];
				if (elem64 < 0 || elem64 >= array2.Length)
				{
					throw new CodeEE("ソートキーが配列外を参照しています");
				}
				if (array2[(int)elem64] != null)
				{
					temp_SortKey = array2[(int)elem64];
				}
				else
				{
					temp_SortKey = "";
				}
			}
			else if (dataString[sortkey.CodeInt] != null)
			{
				temp_SortKey = dataString[sortkey.CodeInt];
			}
			else
			{
				temp_SortKey = "";
			}
		}
		else if (sortkey.IsArray2D)
		{
			long[,] array3 = dataIntegerArray2D[sortkey.CodeInt];
			int num3 = (int)(elem64 >> 32);
			int num4 = (int)(elem64 & 0x7FFFFFFF);
			if (num3 < 0 || num3 >= array3.GetLength(0) || num4 < 0 || num4 >= array3.GetLength(1))
			{
				throw new CodeEE("ソートキーが配列外を参照しています");
			}
			temp_SortKey = array3[num3, num4];
		}
		else if (sortkey.IsArray1D)
		{
			long[] array4 = dataIntegerArray[sortkey.CodeInt];
			if (elem64 < 0 || elem64 >= array4.Length)
			{
				throw new CodeEE("ソートキーが配列外を参照しています");
			}
			temp_SortKey = array4[(int)elem64];
		}
		else
		{
			temp_SortKey = dataInteger[sortkey.CodeInt];
		}
	}
}
