using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.GameView;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData;

internal sealed class ConstantData
{
	private const int ablIndex = 0;

	private const int expIndex = 1;

	private const int talentIndex = 2;

	private const int paramIndex = 3;

	private const int trainIndex = 4;

	private const int markIndex = 5;

	private const int itemIndex = 6;

	private const int baseIndex = 7;

	private const int sourceIndex = 8;

	private const int exIndex = 9;

	private const int strIndex = 10;

	private const int equipIndex = 11;

	private const int tequipIndex = 12;

	private const int flagIndex = 13;

	private const int tflagIndex = 14;

	private const int cflagIndex = 15;

	private const int tcvarIndex = 16;

	private const int cstrIndex = 17;

	private const int stainIndex = 18;

	private const int cdflag1Index = 19;

	private const int cdflag2Index = 20;

	private const int strnameIndex = 21;

	private const int tstrnameIndex = 22;

	private const int savestrnameIndex = 23;

	private const int globalIndex = 24;

	private const int globalsIndex = 25;

	private const int countNameCsv = 26;

	public int[] MaxDataList = new int[26];

	private List<VariableCode> changedCode = new List<VariableCode>();

	public int[] VariableIntArrayLength;

	public int[] VariableStrArrayLength;

	public long[] VariableIntArray2DLength;

	public long[] VariableStrArray2DLength;

	public long[] VariableIntArray3DLength;

	public long[] VariableStrArray3DLength;

	public int[] CharacterIntArrayLength;

	public int[] CharacterStrArrayLength;

	public long[] CharacterIntArray2DLength;

	public long[] CharacterStrArray2DLength;

	private readonly GameBase gamebase;

	private string[][] names = new string[26][];

	private Dictionary<string, int>[] nameToIntDics = new Dictionary<string, int>[26];

	private Dictionary<string, int> relationDic = new Dictionary<string, int>();

	public long[] ItemPrice;

	private readonly List<CharacterTemplate> CharacterTmplList;

	private EmueraConsole output;

	private readonly bool useCompatiName;

	public string[] GetCsvNameList(VariableCode code)
	{
		return names[(int)(code & VariableCode.__LOWERCASE__)];
	}

	public ConstantData(GameBase gamebase)
	{
		this.gamebase = gamebase;
		setDefaultArrayLength();
		CharacterTmplList = new List<CharacterTemplate>();
		useCompatiName = Config.CompatiCALLNAME;
	}

	private void setDefaultArrayLength()
	{
		MaxDataList[0] = 100;
		MaxDataList[2] = 1000;
		MaxDataList[1] = 100;
		MaxDataList[5] = 100;
		MaxDataList[4] = 1000;
		MaxDataList[3] = 200;
		MaxDataList[6] = 1000;
		MaxDataList[7] = 100;
		MaxDataList[8] = 1000;
		MaxDataList[9] = 100;
		MaxDataList[11] = 100;
		MaxDataList[12] = 100;
		MaxDataList[13] = 10000;
		MaxDataList[14] = 1000;
		MaxDataList[15] = 1000;
		MaxDataList[16] = 100;
		MaxDataList[17] = 100;
		MaxDataList[18] = 1000;
		MaxDataList[10] = 20000;
		MaxDataList[19] = 1;
		MaxDataList[20] = 1;
		MaxDataList[21] = 20000;
		MaxDataList[22] = 100;
		MaxDataList[23] = 100;
		MaxDataList[24] = 1000;
		MaxDataList[25] = 100;
		VariableIntArrayLength = new int[65];
		VariableStrArrayLength = new int[7];
		VariableIntArray2DLength = new long[6];
		VariableStrArray2DLength = new long[0];
		VariableIntArray3DLength = new long[2];
		VariableStrArray3DLength = new long[0];
		CharacterIntArrayLength = new int[84];
		CharacterStrArrayLength = new int[1];
		CharacterIntArray2DLength = new long[1];
		CharacterStrArray2DLength = new long[0];
		for (int i = 0; i < VariableIntArrayLength.Length; i++)
		{
			VariableIntArrayLength[i] = 1000;
		}
		VariableIntArrayLength[3] = 10000;
		VariableIntArrayLength[60] = MaxDataList[6];
		VariableIntArrayLength[64] = 625;
		for (int j = 0; j < VariableStrArrayLength.Length; j++)
		{
			VariableStrArrayLength[j] = 100;
		}
		VariableStrArrayLength[1] = MaxDataList[10];
		for (int k = 0; k < VariableIntArray2DLength.Length; k++)
		{
			VariableIntArray2DLength[k] = 429496729700L;
		}
		for (int l = 0; l < VariableStrArray2DLength.Length; l++)
		{
			VariableStrArray2DLength[l] = 429496729700L;
		}
		for (int m = 0; m < VariableIntArray3DLength.Length; m++)
		{
			VariableIntArray3DLength[m] = 109951267635300L;
		}
		for (int n = 0; n < VariableStrArray3DLength.Length; n++)
		{
			VariableStrArray3DLength[n] = 109951267635300L;
		}
		for (int num = 0; num < CharacterIntArrayLength.Length; num++)
		{
			CharacterIntArrayLength[num] = 100;
		}
		CharacterIntArrayLength[3] = 1000;
		CharacterIntArrayLength[9] = 1000;
		CharacterIntArrayLength[10] = 200;
		CharacterIntArrayLength[15] = 200;
		for (int num2 = 0; num2 < CharacterStrArrayLength.Length; num2++)
		{
			CharacterStrArrayLength[num2] = 100;
		}
		for (int num3 = 0; num3 < CharacterIntArray2DLength.Length; num3++)
		{
			CharacterIntArray2DLength[num3] = 4294967297L;
		}
		for (int num4 = 0; num4 < CharacterStrArray2DLength.Length; num4++)
		{
			CharacterStrArray2DLength[num4] = 4294967297L;
		}
	}

	private void loadVariableSizeData(string csvPath, bool disp)
	{
		if (!File.Exists(csvPath))
		{
			return;
		}
		EraStreamReader eraStreamReader = new EraStreamReader(useRename: false);
		if (!eraStreamReader.Open(csvPath))
		{
			output.PrintError(eraStreamReader.Filename + "のオープンに失敗しました");
			return;
		}
		ScriptPosition scriptPosition = null;
		if (disp)
		{
			output.PrintSystemLine(eraStreamReader.Filename + "読み込み中・・・");
		}
		try
		{
			StringStream stringStream = null;
			while ((stringStream = eraStreamReader.ReadEnabledLine()) != null)
			{
				scriptPosition = new ScriptPosition(eraStreamReader.Filename, eraStreamReader.LineNo, stringStream.RowString);
				changeVariableSizeData(stringStream.Substring(), scriptPosition);
			}
			scriptPosition = new ScriptPosition(eraStreamReader.Filename, -1, null);
		}
		catch
		{
			if (scriptPosition != null)
			{
				ParserMediator.Warn("予期しないエラーが発生しました", scriptPosition, 3);
			}
			else
			{
				output.PrintError("予期しないエラーが発生しました");
			}
			return;
		}
		finally
		{
			eraStreamReader.Close();
		}
		decideActualArraySize(scriptPosition);
	}

	private void changeVariableSizeData(string line, ScriptPosition position)
	{
		string[] array = line.Split(',');
		if (array.Length < 2)
		{
			ParserMediator.Warn("\",\"が必要です", position, 1);
			return;
		}
		VariableIdentifier variableId = VariableIdentifier.GetVariableId(array[0].Trim());
		if (variableId == null)
		{
			ParserMediator.Warn("一つ目の値を変数名として認識できません", position, 1);
			return;
		}
		if (!variableId.IsArray1D && !variableId.IsArray2D && !variableId.IsArray3D)
		{
			ParserMediator.Warn("配列変数でない変数" + variableId.ToString() + "のサイズを変更できません", position, 1);
			return;
		}
		if (variableId.IsCalc || variableId.Code == VariableCode.RANDDATA)
		{
			ParserMediator.Warn(variableId.ToString() + "のサイズは変更できません", position, 1);
			return;
		}
		int result = 0;
		int result2 = 0;
		int result3 = 0;
		if (!int.TryParse(array[1], out result))
		{
			ParserMediator.Warn("二つ目の値を整数値として認識できません", position, 1);
			return;
		}
		if (result <= 0)
		{
			if (result == 0)
			{
				ParserMediator.Warn("配列長に0は指定できません（変数を使用禁止にするには配列長に負の値を指定してください）", position, 2);
				return;
			}
			if (!variableId.CanForbid)
			{
				ParserMediator.Warn("使用禁止にできない変数に対して負の配列長が指定されています", position, 2);
				return;
			}
			if (array.Length > 2 && array[2].Length > 0 && array[2].Trim().Length > 0 && char.IsDigit(array[2].Trim()[0]))
			{
				ParserMediator.Warn("一次元配列のサイズ指定に不必要なデータは無視されます", position, 0);
			}
			result = 0;
		}
		else if (variableId.IsArray1D)
		{
			if (array.Length > 2 && array[2].Length > 0 && array[2].Trim().Length > 0 && char.IsDigit(array[2].Trim()[0]))
			{
				ParserMediator.Warn("一次元配列のサイズ指定に不必要なデータは無視されます", position, 0);
			}
			if (variableId.IsLocal && result < 1)
			{
				ParserMediator.Warn("ローカル変数のサイズを1未満にはできません", position, 1);
				return;
			}
			if (!variableId.IsLocal && result < 100)
			{
				ParserMediator.Warn("ローカル変数でない一次元配列のサイズを100未満にはできません", position, 1);
				return;
			}
			if (result > 1000000)
			{
				ParserMediator.Warn("一次元配列のサイズを1000000より大きくすることはできません", position, 1);
				return;
			}
		}
		else if (variableId.IsArray2D)
		{
			if (array.Length < 3)
			{
				ParserMediator.Warn("二次元配列のサイズ指定には2つの数値が必要です", position, 1);
				return;
			}
			if (array.Length > 3 && array[3].Length > 0 && array[3].Trim().Length > 0 && char.IsDigit(array[3].Trim()[0]))
			{
				ParserMediator.Warn("二次元配列のサイズ指定に不必要なデータは無視されます", position, 0);
			}
			if (!int.TryParse(array[2], out result2))
			{
				ParserMediator.Warn("三つ目の値を整数値として認識できません", position, 1);
				return;
			}
			if (result < 1 || result2 < 1)
			{
				ParserMediator.Warn("配列サイズを1未満にはできません", position, 1);
				return;
			}
			if (result > 1000000 || result2 > 1000000)
			{
				ParserMediator.Warn("配列サイズを1000000より大きくすることはできません", position, 1);
				return;
			}
			if (result * result2 > 1000000)
			{
				ParserMediator.Warn("二次元配列の要素数は最大で100万個までです", position, 1);
				return;
			}
		}
		else if (variableId.IsArray3D)
		{
			if (array.Length < 4)
			{
				ParserMediator.Warn("三次元配列のサイズ指定には3つの数値が必要です", position, 1);
				return;
			}
			if (array.Length > 4 && array[4].Length > 0 && array[4].Trim().Length > 0 && char.IsDigit(array[4].Trim()[0]))
			{
				ParserMediator.Warn("三次元配列のサイズ指定に不必要なデータは無視されます", position, 0);
			}
			if (!int.TryParse(array[2], out result2))
			{
				ParserMediator.Warn("三つ目の値を整数値として認識できません", position, 1);
				return;
			}
			if (!int.TryParse(array[3], out result3))
			{
				ParserMediator.Warn("四つ目の値を整数値として認識できません", position, 1);
				return;
			}
			if (result < 1 || result2 < 1 || result3 < 1)
			{
				ParserMediator.Warn("配列サイズを1未満にはできません", position, 1);
				return;
			}
			if (result > 1000000 || result2 > 1000000 || result3 > 1000000)
			{
				ParserMediator.Warn("配列サイズを1000000より大きくすることはできません", position, 1);
				return;
			}
			if (result * result2 * result3 > 10000000)
			{
				ParserMediator.Warn("三次元配列の要素数は最大で1000万個までです", position, 1);
				return;
			}
		}
		switch (variableId.Code)
		{
		case VariableCode.ITEMPRICE:
		case VariableCode.ITEMNAME:
			VariableIntArrayLength[60] = result;
			MaxDataList[6] = result;
			break;
		case VariableCode.STR:
			VariableStrArrayLength[1] = result;
			MaxDataList[10] = result;
			break;
		case VariableCode.ABLNAME:
		case VariableCode.EXPNAME:
		case VariableCode.TALENTNAME:
		case VariableCode.PALAMNAME:
		case VariableCode.MARKNAME:
		case VariableCode.TRAINNAME:
		case VariableCode.BASENAME:
		case VariableCode.SOURCENAME:
		case VariableCode.EXNAME:
		case VariableCode.EQUIPNAME:
		case VariableCode.TEQUIPNAME:
		case VariableCode.FLAGNAME:
		case VariableCode.TFLAGNAME:
		case VariableCode.CFLAGNAME:
		case VariableCode.TCVARNAME:
		case VariableCode.CSTRNAME:
		case VariableCode.STAINNAME:
		case VariableCode.CDFLAGNAME1:
		case VariableCode.CDFLAGNAME2:
		case VariableCode.STRNAME:
		case VariableCode.TSTRNAME:
		case VariableCode.SAVESTRNAME:
		case VariableCode.GLOBALNAME:
		case VariableCode.GLOBALSNAME:
			MaxDataList[(int)(variableId.Code & VariableCode.__LOWERCASE__)] = result;
			break;
		default:
			if (variableId.IsCharacterData)
			{
				if (variableId.IsArray2D)
				{
					long num = ((long)result << 32) + result2;
					if (variableId.IsInteger)
					{
						CharacterIntArray2DLength[variableId.CodeInt] = num;
					}
					else if (variableId.IsString)
					{
						CharacterStrArray2DLength[variableId.CodeInt] = num;
					}
				}
				else if (variableId.IsInteger)
				{
					CharacterIntArrayLength[variableId.CodeInt] = result;
				}
				else if (variableId.IsString)
				{
					CharacterStrArrayLength[variableId.CodeInt] = result;
				}
			}
			else if (variableId.IsArray2D)
			{
				long num2 = ((long)result << 32) + result2;
				if (variableId.IsInteger)
				{
					VariableIntArray2DLength[variableId.CodeInt] = num2;
				}
				else if (variableId.IsString)
				{
					VariableStrArray2DLength[variableId.CodeInt] = num2;
				}
			}
			else if (variableId.IsArray3D)
			{
				long num3 = ((long)result << 40) + ((long)result2 << 20) + result3;
				if (variableId.IsInteger)
				{
					VariableIntArray3DLength[variableId.CodeInt] = num3;
				}
				else
				{
					VariableStrArray3DLength[variableId.CodeInt] = num3;
				}
			}
			else if (variableId.IsInteger)
			{
				VariableIntArrayLength[variableId.CodeInt] = result;
			}
			else if (variableId.IsString)
			{
				VariableStrArrayLength[variableId.CodeInt] = result;
			}
			break;
		}
		if (changedCode.Contains(variableId.Code))
		{
			ParserMediator.Warn(variableId.Code.ToString() + "の要素数は既に定義されています（上書きします）", position, 1);
		}
		else
		{
			changedCode.Add(variableId.Code);
		}
	}

	private void _decideActualArraySize_sub(VariableCode mainCode, VariableCode nameCode, int[] arraylength, ScriptPosition position)
	{
		int num = (int)(nameCode & VariableCode.__LOWERCASE__);
		int num2 = (int)(mainCode & VariableCode.__LOWERCASE__);
		if (changedCode.Contains(nameCode) && changedCode.Contains(mainCode))
		{
			if (MaxDataList[num] != arraylength[num2])
			{
				int num3 = (arraylength[num2] = Math.Max(MaxDataList[num], arraylength[num2]));
				MaxDataList[num] = num3;
				if (MaxDataList[num] == 0 || arraylength[num2] == 0)
				{
					ParserMediator.Warn(mainCode.ToString() + "と" + nameCode.ToString() + "の禁止設定が異なります（使用禁止を解除します）", position, 1);
				}
				else
				{
					ParserMediator.Warn(mainCode.ToString() + "と" + nameCode.ToString() + "の要素数が異なります（大きい方に合わせます）", position, 1);
				}
			}
		}
		else if (changedCode.Contains(nameCode) && !changedCode.Contains(mainCode))
		{
			arraylength[num2] = MaxDataList[num];
		}
		else if (!changedCode.Contains(nameCode) && changedCode.Contains(mainCode))
		{
			MaxDataList[num] = arraylength[num2];
		}
	}

	private void decideActualArraySize(ScriptPosition position)
	{
		_decideActualArraySize_sub(VariableCode.ABL, VariableCode.ABLNAME, CharacterIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.TALENT, VariableCode.TALENTNAME, CharacterIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.EXP, VariableCode.EXPNAME, CharacterIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.MARK, VariableCode.MARKNAME, CharacterIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.BASE, VariableCode.BASENAME, CharacterIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.SOURCE, VariableCode.SOURCENAME, CharacterIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.EX, VariableCode.EXNAME, CharacterIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.EQUIP, VariableCode.EQUIPNAME, CharacterIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.TEQUIP, VariableCode.TEQUIPNAME, CharacterIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.FLAG, VariableCode.FLAGNAME, VariableIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.TFLAG, VariableCode.TFLAGNAME, VariableIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.CFLAG, VariableCode.CFLAGNAME, CharacterIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.TCVAR, VariableCode.TCVARNAME, CharacterIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.CSTR, VariableCode.CSTRNAME, CharacterStrArrayLength, position);
		_decideActualArraySize_sub(VariableCode.STAIN, VariableCode.STAINNAME, CharacterIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.STR, VariableCode.STRNAME, VariableStrArrayLength, position);
		_decideActualArraySize_sub(VariableCode.TSTR, VariableCode.TSTRNAME, VariableStrArrayLength, position);
		_decideActualArraySize_sub(VariableCode.SAVESTR, VariableCode.SAVESTRNAME, VariableStrArrayLength, position);
		_decideActualArraySize_sub(VariableCode.GLOBAL, VariableCode.GLOBALNAME, VariableIntArrayLength, position);
		_decideActualArraySize_sub(VariableCode.GLOBALS, VariableCode.GLOBALSNAME, VariableStrArrayLength, position);
		if (changedCode.Contains(VariableCode.PALAMNAME) && (changedCode.Contains(VariableCode.PALAM) || changedCode.Contains(VariableCode.JUEL)))
		{
			Math.Max(CharacterIntArrayLength[6], CharacterIntArrayLength[10]);
		}
		if (changedCode.Contains(VariableCode.PALAM) || changedCode.Contains(VariableCode.JUEL))
		{
			int num = Math.Max(CharacterIntArrayLength[6], CharacterIntArrayLength[10]);
			if (changedCode.Contains(VariableCode.PALAMNAME))
			{
				if (MaxDataList[3] != num)
				{
					int num2 = Math.Max(MaxDataList[3], num);
					MaxDataList[3] = num2;
					if (CharacterIntArrayLength[6] == num)
					{
						CharacterIntArrayLength[6] = num2;
					}
					if (CharacterIntArrayLength[10] == num)
					{
						CharacterIntArrayLength[10] = num2;
					}
					ParserMediator.Warn("PALAMとJUELとPALAMNAMEの要素数が不適切です", position, 1);
				}
			}
			else
			{
				MaxDataList[3] = num;
			}
		}
		else if (changedCode.Contains(VariableCode.PALAMNAME))
		{
			CharacterIntArrayLength[6] = MaxDataList[3];
			if (MaxDataList[3] < CharacterIntArrayLength[10])
			{
				ParserMediator.Warn("PALAMNAMEの要素数がJUELより少なくなっています（JUELに合わせます）", position, 1);
				MaxDataList[3] = CharacterIntArrayLength[10];
			}
		}
		bool flag = changedCode.Contains(VariableCode.CDFLAGNAME1) || changedCode.Contains(VariableCode.CDFLAGNAME2);
		int num3 = 0;
		long num4 = CharacterIntArray2DLength[num3];
		int num5 = (int)(num4 >> 32);
		int num6 = (int)(num4 & 0x7FFFFFFF);
		if (changedCode.Contains(VariableCode.CDFLAG) && flag)
		{
			if (num5 != MaxDataList[19] || num6 != MaxDataList[20])
			{
				throw new CodeEE("CDFLAGの要素数とCDFLAGNAME1及びCDFLAGNAME2の要素数が一致していません", position);
			}
		}
		else if (flag && !changedCode.Contains(VariableCode.CDFLAG))
		{
			num5 = MaxDataList[19];
			num6 = MaxDataList[20];
			if (num5 * num6 > 1000000)
			{
				throw new CodeEE("CDFLAGの要素数が多すぎます（CDFLAGNAME1とCDFLAGNAME2の要素数の積が100万を超えています）", position);
			}
			CharacterIntArray2DLength[num3] = ((long)num5 << 32) + num6;
		}
		else if (!flag && changedCode.Contains(VariableCode.CDFLAG))
		{
			MaxDataList[19] = num5;
			MaxDataList[20] = num6;
		}
		changedCode.Clear();
	}

	public void LoadData(string csvDir, EmueraConsole console, bool disp)
	{
		output = console;
		loadVariableSizeData(csvDir + "VariableSize.CSV", disp);
		for (int i = 0; i < 26; i++)
		{
			names[i] = new string[MaxDataList[i]];
			nameToIntDics[i] = new Dictionary<string, int>();
		}
		ItemPrice = new long[MaxDataList[6]];
		loadDataTo(csvDir + "ABL.CSV", 0, null, disp);
		loadDataTo(csvDir + "EXP.CSV", 1, null, disp);
		loadDataTo(csvDir + "TALENT.CSV", 2, null, disp);
		loadDataTo(csvDir + "PALAM.CSV", 3, null, disp);
		loadDataTo(csvDir + "TRAIN.CSV", 4, null, disp);
		loadDataTo(csvDir + "MARK.CSV", 5, null, disp);
		loadDataTo(csvDir + "ITEM.CSV", 6, ItemPrice, disp);
		loadDataTo(csvDir + "BASE.CSV", 7, null, disp);
		loadDataTo(csvDir + "SOURCE.CSV", 8, null, disp);
		loadDataTo(csvDir + "EX.CSV", 9, null, disp);
		loadDataTo(csvDir + "STR.CSV", 10, null, disp);
		loadDataTo(csvDir + "EQUIP.CSV", 11, null, disp);
		loadDataTo(csvDir + "TEQUIP.CSV", 12, null, disp);
		loadDataTo(csvDir + "FLAG.CSV", 13, null, disp);
		loadDataTo(csvDir + "TFLAG.CSV", 14, null, disp);
		loadDataTo(csvDir + "CFLAG.CSV", 15, null, disp);
		loadDataTo(csvDir + "TCVAR.CSV", 16, null, disp);
		loadDataTo(csvDir + "CSTR.CSV", 17, null, disp);
		loadDataTo(csvDir + "STAIN.CSV", 18, null, disp);
		loadDataTo(csvDir + "CDFLAG1.CSV", 19, null, disp);
		loadDataTo(csvDir + "CDFLAG2.CSV", 20, null, disp);
		loadDataTo(csvDir + "STRNAME.CSV", 21, null, disp);
		loadDataTo(csvDir + "TSTR.CSV", 22, null, disp);
		loadDataTo(csvDir + "SAVESTR.CSV", 23, null, disp);
		loadDataTo(csvDir + "GLOBAL.CSV", 24, null, disp);
		loadDataTo(csvDir + "GLOBALS.CSV", 25, null, disp);
		for (int j = 0; j < names.Length; j++)
		{
			if (j == 10)
			{
				continue;
			}
			string[] array = names[j];
			for (int k = 0; k < array.Length; k++)
			{
				if (!string.IsNullOrEmpty(array[k]) && !nameToIntDics[j].ContainsKey(array[k]))
				{
					nameToIntDics[j].Add(array[k], k);
				}
			}
		}
		loadCharacterData(csvDir, disp);
		for (int l = 0; l < CharacterTmplList.Count; l++)
		{
			CharacterTemplate characterTemplate = CharacterTmplList[l];
			if (!string.IsNullOrEmpty(characterTemplate.Name) && !relationDic.ContainsKey(characterTemplate.Name))
			{
				relationDic.Add(characterTemplate.Name, (int)characterTemplate.No);
			}
			if (!string.IsNullOrEmpty(characterTemplate.Callname) && !relationDic.ContainsKey(characterTemplate.Callname))
			{
				relationDic.Add(characterTemplate.Callname, (int)characterTemplate.No);
			}
			if (!string.IsNullOrEmpty(characterTemplate.Nickname) && !relationDic.ContainsKey(characterTemplate.Nickname))
			{
				relationDic.Add(characterTemplate.Nickname, (int)characterTemplate.No);
			}
		}
	}

	public bool isDefined(VariableCode varCode, string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return false;
		}
		string errPos = null;
		Dictionary<string, int> dictionary = null;
		if (varCode == VariableCode.CDFLAG)
		{
			dictionary = GetKeywordDictionary(out errPos, VariableCode.CDFLAGNAME1, -1);
			if (dictionary == null || !dictionary.ContainsKey(str))
			{
				dictionary = GetKeywordDictionary(out errPos, VariableCode.CDFLAGNAME2, -1);
			}
			return dictionary?.ContainsKey(str) ?? false;
		}
		return GetKeywordDictionary(out errPos, varCode, -1)?.ContainsKey(str) ?? false;
	}

	public bool TryKeywordToInteger(out int ret, VariableCode code, string key, int index)
	{
		ret = 0;
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		Dictionary<string, int> dictionary = null;
		try
		{
			dictionary = GetKeywordDictionary(out var _, code, index);
			if (dictionary == null)
			{
				return false;
			}
		}
		catch
		{
			return false;
		}
		return dictionary.TryGetValue(key, out ret);
	}

	public int KeywordToInteger(VariableCode code, string key, int index)
	{
		if (string.IsNullOrEmpty(key))
		{
			throw new CodeEE("キーワードを空には出来ません");
		}
		int value = -1;
		if (GetKeywordDictionary(out var errPos, code, index).TryGetValue(key, out value))
		{
			return value;
		}
		if (errPos == null)
		{
			throw new CodeEE("配列変数" + code.ToString() + "の要素を文字列で指定することはできません");
		}
		throw new CodeEE(errPos + "の中に\"" + key + "\"の定義がありません");
	}

	public Dictionary<string, int> GetKeywordDictionary(out string errPos, VariableCode code, int index)
	{
		errPos = null;
		int num = -1;
		Dictionary<string, int> dictionary = null;
		switch (code)
		{
		case VariableCode.ABL:
			dictionary = nameToIntDics[0];
			errPos = "abl.csv";
			num = 1;
			break;
		case VariableCode.EXP:
			dictionary = nameToIntDics[1];
			errPos = "exp.csv";
			num = 1;
			break;
		case VariableCode.TALENT:
			dictionary = nameToIntDics[2];
			errPos = "talent.csv";
			num = 1;
			break;
		case VariableCode.UP:
		case VariableCode.DOWN:
			dictionary = nameToIntDics[3];
			errPos = "palam.csv";
			num = 0;
			break;
		case VariableCode.PALAM:
		case VariableCode.JUEL:
		case VariableCode.GOTJUEL:
		case VariableCode.CUP:
		case VariableCode.CDOWN:
			dictionary = nameToIntDics[3];
			errPos = "palam.csv";
			num = 1;
			break;
		case VariableCode.TRAINNAME:
			dictionary = nameToIntDics[4];
			errPos = "train.csv";
			num = 0;
			break;
		case VariableCode.MARK:
			dictionary = nameToIntDics[5];
			errPos = "mark.csv";
			num = 1;
			break;
		case VariableCode.ITEM:
		case VariableCode.ITEMSALES:
		case VariableCode.ITEMPRICE:
			dictionary = nameToIntDics[6];
			errPos = "Item.csv";
			num = 0;
			break;
		case VariableCode.LOSEBASE:
			dictionary = nameToIntDics[7];
			errPos = "base.csv";
			num = 0;
			break;
		case VariableCode.BASE:
		case VariableCode.MAXBASE:
		case VariableCode.DOWNBASE:
			dictionary = nameToIntDics[7];
			errPos = "base.csv";
			num = 1;
			break;
		case VariableCode.SOURCE:
			dictionary = nameToIntDics[8];
			errPos = "source.csv";
			num = 1;
			break;
		case VariableCode.EX:
		case VariableCode.NOWEX:
			dictionary = nameToIntDics[9];
			errPos = "ex.csv";
			num = 1;
			break;
		case VariableCode.EQUIP:
			dictionary = nameToIntDics[11];
			errPos = "equip.csv";
			num = 1;
			break;
		case VariableCode.TEQUIP:
			dictionary = nameToIntDics[12];
			errPos = "tequip.csv";
			num = 1;
			break;
		case VariableCode.FLAG:
			dictionary = nameToIntDics[13];
			errPos = "flag.csv";
			num = 0;
			break;
		case VariableCode.TFLAG:
			dictionary = nameToIntDics[14];
			errPos = "tflag.csv";
			num = 0;
			break;
		case VariableCode.CFLAG:
			dictionary = nameToIntDics[15];
			errPos = "cflag.csv";
			num = 1;
			break;
		case VariableCode.TCVAR:
			dictionary = nameToIntDics[16];
			errPos = "tcvar.csv";
			num = 1;
			break;
		case VariableCode.CSTR:
			dictionary = nameToIntDics[17];
			errPos = "cstr.csv";
			num = 1;
			break;
		case VariableCode.STAIN:
			dictionary = nameToIntDics[18];
			errPos = "stain.csv";
			num = 1;
			break;
		case VariableCode.CDFLAGNAME1:
			dictionary = nameToIntDics[19];
			errPos = "cdflag1.csv";
			num = 0;
			break;
		case VariableCode.CDFLAGNAME2:
			dictionary = nameToIntDics[20];
			errPos = "cdflag2.csv";
			num = 0;
			break;
		case VariableCode.CDFLAG:
			if (index == 1)
			{
				dictionary = nameToIntDics[19];
				errPos = "cdflag1.csv";
			}
			else
			{
				if (index != 2)
				{
					if (index >= 0)
					{
						throw new CodeEE("配列変数" + code.ToString() + "の" + (index + 1) + "番目の要素を文字列で指定することはできません");
					}
					throw new CodeEE("CDFLAGの要素の取得にはCDFLAGNAME1又はCDFLAGNAME2を使用します");
				}
				dictionary = nameToIntDics[20];
				errPos = "cdflag2.csv";
			}
			return dictionary;
		case VariableCode.STR:
			dictionary = nameToIntDics[21];
			errPos = "strname.csv";
			num = 0;
			break;
		case VariableCode.TSTR:
			dictionary = nameToIntDics[22];
			errPos = "tstr.csv";
			num = 0;
			break;
		case VariableCode.SAVESTR:
			dictionary = nameToIntDics[23];
			errPos = "savestr.csv";
			num = 0;
			break;
		case VariableCode.GLOBAL:
			dictionary = nameToIntDics[24];
			errPos = "global.csv";
			num = 0;
			break;
		case VariableCode.GLOBALS:
			dictionary = nameToIntDics[25];
			errPos = "globals.csv";
			num = 0;
			break;
		case VariableCode.RELATION:
			dictionary = relationDic;
			errPos = "chara*.csv";
			num = 1;
			break;
		case VariableCode.NAME:
			dictionary = relationDic;
			errPos = "chara*.csv";
			num = -1;
			break;
		}
		if (index < 0)
		{
			return dictionary;
		}
		if (dictionary == null)
		{
			throw new CodeEE("配列変数" + code.ToString() + "の要素を文字列で指定することはできません");
		}
		if (index != num)
		{
			if (num < 0)
			{
				throw new CodeEE("配列変数" + code.ToString() + "の要素を文字列で指定することはできません");
			}
			throw new CodeEE("配列変数" + code.ToString() + "の" + (index + 1) + "番目の要素を文字列で指定することはできません");
		}
		return dictionary;
	}

	public CharacterTemplate GetCharacterTemplate(long index)
	{
		foreach (CharacterTemplate characterTmpl in CharacterTmplList)
		{
			if (characterTmpl.No == index)
			{
				return characterTmpl;
			}
		}
		return null;
	}

	public CharacterTemplate GetCharacterTemplate_UseSp(long index, bool sp)
	{
		foreach (CharacterTemplate characterTmpl in CharacterTmplList)
		{
			if (characterTmpl.No == index && (!Config.CompatiSPChara || sp == characterTmpl.IsSpchara))
			{
				return characterTmpl;
			}
		}
		return null;
	}

	public CharacterTemplate GetCharacterTemplateFromCsvNo(long index)
	{
		foreach (CharacterTemplate characterTmpl in CharacterTmplList)
		{
			if (characterTmpl.csvNo == index)
			{
				return characterTmpl;
			}
		}
		return null;
	}

	public CharacterTemplate GetPseudoChara()
	{
		return new CharacterTemplate(0L, this);
	}

	private void loadCharacterData(string csvDir, bool disp)
	{
		if (!Directory.Exists(csvDir))
		{
			return;
		}
		List<KeyValuePair<string, string>> files = Config.GetFiles(csvDir, "CHARA*.CSV");
		for (int i = 0; i < files.Count; i++)
		{
			loadCharacterDataFile(files[i].Value, files[i].Key, disp);
		}
		if (useCompatiName)
		{
			foreach (CharacterTemplate characterTmpl in CharacterTmplList)
			{
				if (string.IsNullOrEmpty(characterTmpl.Callname))
				{
					characterTmpl.Callname = characterTmpl.Name;
				}
			}
		}
		foreach (CharacterTemplate characterTmpl2 in CharacterTmplList)
		{
			characterTmpl2.SetSpFlag();
		}
		Dictionary<long, CharacterTemplate> dictionary = new Dictionary<long, CharacterTemplate>();
		Dictionary<long, CharacterTemplate> dictionary2 = new Dictionary<long, CharacterTemplate>();
		foreach (CharacterTemplate characterTmpl3 in CharacterTmplList)
		{
			Dictionary<long, CharacterTemplate> dictionary3 = dictionary;
			if (Config.CompatiSPChara && characterTmpl3.IsSpchara)
			{
				dictionary3 = dictionary2;
			}
			if (dictionary3.ContainsKey(characterTmpl3.No))
			{
				if (!Config.CompatiSPChara && characterTmpl3.IsSpchara != dictionary3[characterTmpl3.No].IsSpchara)
				{
					ParserMediator.Warn("番号" + characterTmpl3.No + "のキャラが複数回定義されています(SPキャラとして定義するには互換性オプション「SPキャラを使用する」をONにしてください)", null, 1);
				}
				else
				{
					ParserMediator.Warn("番号" + characterTmpl3.No + "のキャラが複数回定義されています", null, 1);
				}
			}
			else
			{
				dictionary3.Add(characterTmpl3.No, characterTmpl3);
			}
		}
	}

	private void loadCharacterDataFile(string csvPath, string csvName, bool disp)
	{
		CharacterTemplate characterTemplate = null;
		EraStreamReader eraStreamReader = new EraStreamReader(useRename: false);
		if (!eraStreamReader.Open(csvPath, csvName))
		{
			output.PrintError(eraStreamReader.Filename + "のオープンに失敗しました");
			return;
		}
		ScriptPosition scriptPosition = null;
		if (disp)
		{
			output.PrintSystemLine(eraStreamReader.Filename + "読み込み中・・・");
		}
		try
		{
			long result = -1L;
			StringStream stringStream = null;
			while ((stringStream = eraStreamReader.ReadEnabledLine()) != null)
			{
				scriptPosition = new ScriptPosition(eraStreamReader.Filename, eraStreamReader.LineNo, stringStream.RowString);
				string[] array = stringStream.Substring().Split(',');
				if (array.Length < 2)
				{
					ParserMediator.Warn("\",\"が必要です", scriptPosition, 1);
				}
				else if (array[0].Length == 0)
				{
					ParserMediator.Warn("\",\"で始まっています", scriptPosition, 1);
				}
				else if (array[0].Equals("NO", Config.SCVariable) || array[0].Equals("番号", Config.SCVariable))
				{
					if (characterTemplate != null)
					{
						ParserMediator.Warn("番号が二重に定義されました", scriptPosition, 1);
						continue;
					}
					if (!long.TryParse(array[1].TrimEnd(), out result))
					{
						ParserMediator.Warn(array[1] + "を整数値に変換できません", scriptPosition, 1);
						continue;
					}
					characterTemplate = new CharacterTemplate(result, this);
					string text = eraStreamReader.Filename.ToUpper();
					string s = text.Substring(text.IndexOf("CHARA") + 5);
					StringBuilder stringBuilder = new StringBuilder();
					StringStream stringStream2 = new StringStream(s);
					while (!stringStream2.EOS && char.IsDigit(stringStream2.Current))
					{
						stringBuilder.Append(stringStream2.Current);
						stringStream2.ShiftNext();
					}
					if (stringBuilder.Length > 0)
					{
						characterTemplate.csvNo = Convert.ToInt64(stringBuilder.ToString());
					}
					else
					{
						characterTemplate.csvNo = 0L;
					}
					CharacterTmplList.Add(characterTemplate);
				}
				else if (characterTemplate == null)
				{
					ParserMediator.Warn("番号が定義される前に他のデータが始まりました", scriptPosition, 1);
				}
				else
				{
					toCharacterTemplate(gamebase, scriptPosition, characterTemplate, array);
				}
			}
		}
		catch
		{
			if (scriptPosition != null)
			{
				ParserMediator.Warn("予期しないエラーが発生しました", scriptPosition, 3);
			}
			else
			{
				output.PrintError("予期しないエラーが発生しました");
			}
		}
		finally
		{
			eraStreamReader.Dispose();
		}
	}

	private bool tryToInt64(string str, out long p)
	{
		p = -1L;
		if (string.IsNullOrEmpty(str))
		{
			return false;
		}
		StringStream stringStream = new StringStream(str);
		int num = 1;
		if (stringStream.Current == '+')
		{
			stringStream.ShiftNext();
		}
		else if (stringStream.Current == '-')
		{
			num = -1;
			stringStream.ShiftNext();
		}
		switch (stringStream.Current)
		{
		default:
			return false;
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			try
			{
				p = LexicalAnalyzer.ReadInt64(stringStream, retZero: false);
				p *= num;
			}
			catch
			{
				return false;
			}
			return true;
		}
	}

	private void toCharacterTemplate(GameBase gamebase, ScriptPosition position, CharacterTemplate chara, string[] tokens)
	{
		if (chara == null)
		{
			return;
		}
		int num = -1;
		long p = -1L;
		long p2 = -1L;
		Dictionary<int, long> dictionary = null;
		Dictionary<int, string> dictionary2 = null;
		Dictionary<string, int> dictionary3 = null;
		string text = null;
		string text2 = tokens[0].ToUpper();
		switch (text2)
		{
		case "NAME":
		case "名前":
			chara.Name = tokens[1];
			return;
		case "CALLNAME":
		case "呼び名":
			chara.Callname = tokens[1];
			return;
		case "NICKNAME":
		case "あだ名":
			chara.Nickname = tokens[1];
			return;
		case "MASTERNAME":
		case "主人の呼び方":
			chara.Mastername = tokens[1];
			return;
		case "MARK":
		case "刻印":
			num = CharacterIntArrayLength[5];
			dictionary = chara.Mark;
			dictionary3 = nameToIntDics[5];
			text = "mark.csv";
			break;
		case "EXP":
		case "経験":
			num = CharacterIntArrayLength[4];
			dictionary = chara.Exp;
			dictionary3 = nameToIntDics[1];
			text = "exp.csv";
			break;
		case "ABL":
		case "能力":
			num = CharacterIntArrayLength[2];
			dictionary = chara.Abl;
			dictionary3 = nameToIntDics[0];
			text = "abl.csv";
			break;
		case "BASE":
		case "基礎":
			num = CharacterIntArrayLength[1];
			dictionary = chara.Maxbase;
			dictionary3 = nameToIntDics[7];
			text = "base.csv";
			break;
		case "TALENT":
		case "素質":
			num = CharacterIntArrayLength[3];
			dictionary = chara.Talent;
			dictionary3 = nameToIntDics[2];
			text = "talent.csv";
			break;
		case "RELATION":
		case "相性":
			num = CharacterIntArrayLength[11];
			dictionary = chara.Relation;
			dictionary3 = null;
			break;
		case "CFLAG":
		case "フラグ":
			num = CharacterIntArrayLength[9];
			dictionary = chara.CFlag;
			dictionary3 = nameToIntDics[15];
			text = "cflag.csv";
			break;
		case "EQUIP":
		case "装着物":
			num = CharacterIntArrayLength[12];
			dictionary = chara.Equip;
			dictionary3 = nameToIntDics[11];
			text = "equip.csv";
			break;
		case "JUEL":
		case "珠":
			num = CharacterIntArrayLength[10];
			dictionary = chara.Juel;
			dictionary3 = nameToIntDics[3];
			text = "palam.csv";
			break;
		case "CSTR":
			num = CharacterStrArrayLength[0];
			dictionary2 = chara.CStr;
			dictionary3 = nameToIntDics[17];
			text = "cstr.csv";
			break;
		default:
			ParserMediator.Warn("\"" + tokens[0] + "\"は解釈できない識別子です", position, 1);
			return;
		}
		if (num < 0)
		{
			ParserMediator.Warn("プログラムミス", position, 3);
			return;
		}
		if (num == 0)
		{
			ParserMediator.Warn(text2 + "は禁止設定された変数です", position, 2);
			return;
		}
		bool flag = tryToInt64(tokens[1].TrimEnd(), out p);
		if (flag && (p < 0 || p >= num))
		{
			ParserMediator.Warn(p + "は配列の範囲外です", position, 1);
			return;
		}
		int value = (int)p;
		if (!flag && dictionary3 != null)
		{
			if (!dictionary3.TryGetValue(tokens[1], out value))
			{
				ParserMediator.Warn(text + "に\"" + tokens[1] + "\"の定義がありません", position, 1);
				return;
			}
			if (value >= num)
			{
				ParserMediator.Warn("\"" + tokens[1] + "\"は配列の範囲外です", position, 1);
				return;
			}
		}
		if (value < 0 || value >= num)
		{
			if (flag)
			{
				ParserMediator.Warn(value + "は配列の範囲外です", position, 1);
			}
			else if (tokens[1].Length == 0)
			{
				ParserMediator.Warn("二つ目の識別子がありません", position, 1);
			}
			else
			{
				ParserMediator.Warn("\"" + tokens[1] + "\"は解釈できない識別子です", position, 1);
			}
		}
		else if (dictionary2 != null)
		{
			if (tokens.Length < 3)
			{
				ParserMediator.Warn("三つ目の識別子がありません", position, 1);
			}
			if (dictionary2.ContainsKey(value))
			{
				ParserMediator.Warn(text2 + "の" + value + "番目の要素は既に定義されています(上書きします)", position, 1);
			}
			dictionary2[value] = tokens[2];
		}
		else
		{
			if (tokens.Length < 3 || !tryToInt64(tokens[2], out p2))
			{
				p2 = 1L;
			}
			if (dictionary.ContainsKey(value))
			{
				ParserMediator.Warn(text2 + "の" + value + "番目の要素は既に定義されています(上書きします)", position, 1);
			}
			dictionary[value] = p2;
		}
	}

	private void loadDataTo(string csvPath, int targetIndex, long[] targetI, bool disp)
	{
		if (!File.Exists(csvPath))
		{
			return;
		}
		string[] array = names[targetIndex];
		List<int> list = new List<int>();
		EraStreamReader eraStreamReader = new EraStreamReader(useRename: false);
		if (!eraStreamReader.Open(csvPath))
		{
			output.PrintError(eraStreamReader.Filename + "のオープンに失敗しました");
			return;
		}
		ScriptPosition scriptPosition = null;
		if (disp || Program.AnalysisMode)
		{
			output.PrintSystemLine(eraStreamReader.Filename + "読み込み中・・・");
		}
		try
		{
			StringStream stringStream = null;
			while ((stringStream = eraStreamReader.ReadEnabledLine()) != null)
			{
				scriptPosition = new ScriptPosition(eraStreamReader.Filename, eraStreamReader.LineNo, stringStream.RowString);
				string[] array2 = stringStream.Substring().Split(',');
				if (array2.Length < 2)
				{
					ParserMediator.Warn("\",\"が必要です", scriptPosition, 1);
					continue;
				}
				int result = 0;
				if (!int.TryParse(array2[0], out result))
				{
					ParserMediator.Warn("一つ目の値を整数値に変換できません", scriptPosition, 1);
					continue;
				}
				if (array.Length == 0)
				{
					ParserMediator.Warn("禁止設定された名前配列です", scriptPosition, 2);
					break;
				}
				if (result < 0 || array.Length <= result)
				{
					ParserMediator.Warn(result + "は配列の範囲外です", scriptPosition, 1);
					continue;
				}
				if (list.Contains(result))
				{
					ParserMediator.Warn(result + "番目の要素はすでに定義されています（新しい値で上書きします）", scriptPosition, 1);
				}
				else
				{
					list.Add(result);
				}
				array[result] = array2[1];
				if (targetI != null && array2.Length >= 3)
				{
					if (!long.TryParse(array2[2].TrimEnd(), out var result2))
					{
						ParserMediator.Warn("金額が読み取れません", scriptPosition, 1);
					}
					else
					{
						targetI[result] = result2;
					}
				}
			}
		}
		catch
		{
			if (scriptPosition != null)
			{
				ParserMediator.Warn("予期しないエラーが発生しました", scriptPosition, 3);
			}
			else
			{
				output.PrintError("予期しないエラーが発生しました");
			}
		}
		finally
		{
			eraStreamReader.Close();
		}
	}
}
