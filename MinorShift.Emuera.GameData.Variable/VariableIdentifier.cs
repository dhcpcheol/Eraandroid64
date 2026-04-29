using System;
using System.Collections.Generic;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal sealed class VariableIdentifier
{
	private readonly VariableCode code;

	private readonly string scope;

	private static readonly Dictionary<string, VariableCode> nameDic;

	private static readonly Dictionary<string, VariableCode> localvarNameDic;

	private static readonly Dictionary<VariableCode, List<VariableCode>> extSaveListDic;

	public VariableCode Code => code;

	public string Scope => scope;

	public int CodeInt => (int)(code & VariableCode.__LOWERCASE__);

	public VariableCode CodeFlag => code & VariableCode.__UPPERCASE__;

	public bool IsNull => code == VariableCode.__NULL__;

	public bool IsCharacterData => (code & VariableCode.__CHARACTER_DATA__) == VariableCode.__CHARACTER_DATA__;

	public bool IsInteger => (code & VariableCode.__INTEGER__) == VariableCode.__INTEGER__;

	public bool IsString => (code & VariableCode.__STRING__) == VariableCode.__STRING__;

	public bool IsArray1D => (code & VariableCode.__ARRAY_1D__) == VariableCode.__ARRAY_1D__;

	public bool IsArray2D => (code & VariableCode.__ARRAY_2D__) == VariableCode.__ARRAY_2D__;

	public bool IsArray3D => (code & VariableCode.__ARRAY_3D__) == VariableCode.__ARRAY_3D__;

	public bool Readonly => (code & VariableCode.__UNCHANGEABLE__) == VariableCode.__UNCHANGEABLE__;

	public bool IsCalc => (code & VariableCode.__CALC__) == VariableCode.__CALC__;

	public bool IsLocal => (code & VariableCode.__LOCAL__) == VariableCode.__LOCAL__;

	public bool CanForbid => (code & VariableCode.__CAN_FORBID__) == VariableCode.__CAN_FORBID__;

	private VariableIdentifier(VariableCode code)
	{
		this.code = code;
	}

	private VariableIdentifier(VariableCode code, string scope)
	{
		this.code = code;
		this.scope = scope;
	}

	public static Dictionary<string, VariableCode> GetVarNameDic()
	{
		return nameDic;
	}

	static VariableIdentifier()
	{
		nameDic = new Dictionary<string, VariableCode>();
		localvarNameDic = new Dictionary<string, VariableCode>();
		extSaveListDic = new Dictionary<VariableCode, List<VariableCode>>();
		Array values = Enum.GetValues(typeof(VariableCode));
		nameDic.Add(VariableCode.__FILE__.ToString(), VariableCode.__FILE__);
		nameDic.Add(VariableCode.__LINE__.ToString(), VariableCode.__LINE__);
		nameDic.Add(VariableCode.__FUNCTION__.ToString(), VariableCode.__FUNCTION__);
		foreach (VariableCode item in values)
		{
			string text = item.ToString();
			if (text == null || (text.StartsWith("__") && text.EndsWith("__")))
			{
				continue;
			}
			if (Config.ICVariable)
			{
				text = text.ToUpper();
			}
			if (nameDic.ContainsKey(text))
			{
				continue;
			}
			nameDic.Add(text, item);
			if ((item & VariableCode.__LOCAL__) == VariableCode.__LOCAL__)
			{
				localvarNameDic.Add(text, item);
			}
			if ((item & VariableCode.__SAVE_EXTENDED__) == VariableCode.__SAVE_EXTENDED__)
			{
				VariableCode key = item & (VariableCode)673054720;
				if (!extSaveListDic.ContainsKey(key))
				{
					extSaveListDic.Add(key, new List<VariableCode>());
				}
				extSaveListDic[key].Add(item);
			}
		}
	}

	public static List<VariableCode> GetExtSaveList(VariableCode flag)
	{
		VariableCode key = flag & (VariableCode)673054720;
		if (!extSaveListDic.ContainsKey(key))
		{
			return new List<VariableCode>();
		}
		return extSaveListDic[key];
	}

	public static VariableIdentifier GetVariableId(VariableCode code)
	{
		return new VariableIdentifier(code);
	}

	public static VariableIdentifier GetVariableId(string key)
	{
		return GetVariableId(key, null);
	}

	public static VariableIdentifier GetVariableId(string key, string subStr)
	{
		VariableCode value = VariableCode.__NULL__;
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		if (Config.ICVariable)
		{
			key = key.ToUpper();
		}
		if (subStr != null)
		{
			if (Config.ICFunction)
			{
				subStr = subStr.ToUpper();
			}
			if (localvarNameDic.TryGetValue(key, out value))
			{
				return new VariableIdentifier(value, subStr);
			}
			if (nameDic.ContainsKey(key))
			{
				throw new CodeEE("ローカル変数でない変数" + key + "に対して@が使われました");
			}
			throw new CodeEE("@の使い方が不正です");
		}
		nameDic.TryGetValue(key, out value);
		return new VariableIdentifier(value);
	}

	public override string ToString()
	{
		return code.ToString();
	}
}
