using System.Collections.Generic;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData;

internal sealed class CharacterTemplate
{
	private int[] arraySize;

	private int cstrSize;

	public string Name;

	public string Callname;

	public string Nickname;

	public string Mastername;

	public readonly long No;

	public readonly Dictionary<int, long> Maxbase = new Dictionary<int, long>();

	public readonly Dictionary<int, long> Mark = new Dictionary<int, long>();

	public readonly Dictionary<int, long> Exp = new Dictionary<int, long>();

	public readonly Dictionary<int, long> Abl = new Dictionary<int, long>();

	public readonly Dictionary<int, long> Talent = new Dictionary<int, long>();

	public readonly Dictionary<int, long> Relation = new Dictionary<int, long>();

	public readonly Dictionary<int, long> CFlag = new Dictionary<int, long>();

	public readonly Dictionary<int, long> Equip = new Dictionary<int, long>();

	public readonly Dictionary<int, long> Juel = new Dictionary<int, long>();

	public readonly Dictionary<int, string> CStr = new Dictionary<int, string>();

	public long csvNo;

	public bool IsSpchara { get; private set; }

	public CharacterTemplate(long index, ConstantData constant)
	{
		arraySize = constant.CharacterIntArrayLength;
		cstrSize = constant.CharacterStrArrayLength[0];
		No = index;
	}

	public int ArrayStrLength(CharacterStrData type)
	{
		if (type == CharacterStrData.CSTR)
		{
			return cstrSize;
		}
		throw new CodeEE("存在しないキーを参照しました");
	}

	public int ArrayLength(CharacterIntData type)
	{
		switch (type)
		{
		case CharacterIntData.BASE:
		{
			int num = arraySize[0];
			int num2 = arraySize[1];
			if (num <= num2)
			{
				return num2;
			}
			return num;
		}
		case CharacterIntData.MARK:
			return arraySize[5];
		case CharacterIntData.ABL:
			return arraySize[2];
		case CharacterIntData.EXP:
			return arraySize[4];
		case CharacterIntData.RELATION:
			return arraySize[11];
		case CharacterIntData.TALENT:
			return arraySize[3];
		case CharacterIntData.CFLAG:
			return arraySize[9];
		case CharacterIntData.EQUIP:
			return arraySize[12];
		case CharacterIntData.JUEL:
			return arraySize[10];
		default:
			throw new CodeEE("存在しないキーを参照しました");
		}
	}

	internal void SetSpFlag()
	{
		if (CFlag.ContainsKey(0) && CFlag[0] != 0L)
		{
			IsSpchara = true;
		}
	}
}
