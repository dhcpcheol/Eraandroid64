using System.Collections.Generic;
using System.Text.RegularExpressions;
using MinorShift._Library;
using MinorShift.Emuera.GameData;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Function;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.GameProc;
using MinorShift.Emuera.GameProc.Function;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera;

internal sealed class IdentifierDictionary
{
	private enum DefinedNameType
	{
		None,
		Reserved,
		SystemVariable,
		SystemMethod,
		SystemInstrument,
		UserGlobalVariable,
		UserMacro,
		UserRefMethod,
		NameSpace
	}

	private static readonly char[] badSymbolAsIdentifier = new char[33]
	{
		'+', '-', '*', '/', '%', '=', '!', '<', '>', '|',
		'&', '^', '~', ' ', '\u3000', '\t', '"', '(', ')', '{',
		'}', '[', ']', ',', '.', ':', '\\', '@', '$', '#',
		'?', ';', '\''
	};

	private static readonly Regex regexCom = new Regex("^COM[0-9]+$");

	private static readonly Regex regexComAble = new Regex("^COM_ABLE[0-9]+$");

	private static readonly Regex regexAblup = new Regex("^ABLUP[0-9]+$");

	private Dictionary<string, DefinedNameType> nameDic = new Dictionary<string, DefinedNameType>();

	private List<string> privateDimList = new List<string>();

	private List<string> disableList = new List<string>();

	private VariableData varData;

	private Dictionary<string, VariableToken> varTokenDic;

	private Dictionary<string, VariableLocal> localvarTokenDic;

	private Dictionary<string, FunctionIdentifier> instructionDic;

	private Dictionary<string, FunctionMethod> methodDic;

	private Dictionary<string, UserDefinedRefMethod> refmethodDic;

	public List<UserDefinedCharaVariableToken> CharaDimList = new List<UserDefinedCharaVariableToken>();

	private Dictionary<string, DefineMacro> macroDic = new Dictionary<string, DefineMacro>();

	public static bool IsEventLabelName(string labelName)
	{
		switch (labelName)
		{
		case "EVENTFIRST":
		case "EVENTTRAIN":
		case "EVENTSHOP":
		case "EVENTBUY":
		case "EVENTCOM":
		case "EVENTTURNEND":
		case "EVENTCOMEND":
		case "EVENTEND":
		case "EVENTLOAD":
			return true;
		default:
			return false;
		}
	}

	public static bool IsSystemLabelName(string labelName)
	{
		switch (labelName)
		{
		case "EVENTFIRST":
		case "EVENTTRAIN":
		case "EVENTSHOP":
		case "EVENTBUY":
		case "EVENTCOM":
		case "EVENTTURNEND":
		case "EVENTCOMEND":
		case "EVENTEND":
		case "SHOW_STATUS":
		case "SHOW_USERCOM":
		case "USERCOM":
		case "SOURCE_CHECK":
		case "CALLTRAINEND":
		case "SHOW_JUEL":
		case "SHOW_ABLUP_SELECT":
		case "USERABLUP":
		case "SHOW_SHOP":
		case "SAVEINFO":
		case "USERSHOP":
		case "EVENTLOAD":
		case "TITLE_LOADGAME":
		case "SYSTEM_AUTOSAVE":
		case "SYSTEM_TITLE":
		case "SYSTEM_LOADEND":
			return true;
		default:
			if (labelName.StartsWith("COM"))
			{
				if (regexCom.IsMatch(labelName))
				{
					return true;
				}
				if (regexComAble.IsMatch(labelName))
				{
					return true;
				}
			}
			if (labelName.StartsWith("ABLUP") && regexAblup.IsMatch(labelName))
			{
				return true;
			}
			return false;
		}
	}

	public IdentifierDictionary(VariableData varData)
	{
		this.varData = varData;
		nameDic.Clear();
		nameDic.Add("IS", DefinedNameType.Reserved);
		nameDic.Add("TO", DefinedNameType.Reserved);
		nameDic.Add("INT", DefinedNameType.Reserved);
		nameDic.Add("STR", DefinedNameType.Reserved);
		nameDic.Add("REFFUNC", DefinedNameType.Reserved);
		nameDic.Add("STATIC", DefinedNameType.Reserved);
		nameDic.Add("DYNAMIC", DefinedNameType.Reserved);
		nameDic.Add("GLOBAL", DefinedNameType.Reserved);
		nameDic.Add("PRIVATE", DefinedNameType.Reserved);
		nameDic.Add("SAVEDATA", DefinedNameType.Reserved);
		nameDic.Add("CHARADATA", DefinedNameType.Reserved);
		nameDic.Add("REF", DefinedNameType.Reserved);
		nameDic.Add("__DEBUG__", DefinedNameType.Reserved);
		nameDic.Add("__SKIP__", DefinedNameType.Reserved);
		nameDic.Add("_", DefinedNameType.Reserved);
		instructionDic = FunctionIdentifier.GetInstructionNameDic();
		varTokenDic = varData.GetVarTokenDicClone();
		localvarTokenDic = varData.GetLocalvarTokenDic();
		methodDic = FunctionMethodCreator.GetMethodList();
		refmethodDic = new Dictionary<string, UserDefinedRefMethod>();
		foreach (KeyValuePair<string, FunctionMethod> item in methodDic)
		{
			nameDic.Add(item.Key, DefinedNameType.SystemMethod);
		}
		foreach (KeyValuePair<string, VariableToken> item2 in varTokenDic)
		{
			if (!nameDic.ContainsKey(item2.Key))
			{
				nameDic.Add(item2.Key, DefinedNameType.SystemVariable);
			}
		}
		foreach (KeyValuePair<string, VariableLocal> item3 in localvarTokenDic)
		{
			nameDic.Add(item3.Key, DefinedNameType.SystemVariable);
		}
		foreach (KeyValuePair<string, FunctionIdentifier> item4 in instructionDic)
		{
			if (!nameDic.ContainsKey(item4.Key))
			{
				nameDic.Add(item4.Key, DefinedNameType.SystemInstrument);
			}
		}
	}

	public void CheckUserLabelName(ref string errMes, ref int warnLevel, bool isFunction, string labelName)
	{
		if (labelName.Length == 0)
		{
			errMes = "ラベル名がありません";
			warnLevel = 2;
		}
		else if (labelName.IndexOfAny(badSymbolAsIdentifier) >= 0)
		{
			errMes = "ラベル名" + labelName + "に\"_\"以外の記号が含まれています";
			warnLevel = 1;
		}
		else if (char.IsDigit(labelName[0]) && labelName[0].ToString().Length == LangManager.GetStrlenLang(labelName[0].ToString()))
		{
			errMes = "ラベル名" + labelName + "が半角数字から始まっています";
			warnLevel = 0;
		}
		else
		{
			if (!isFunction || !Config.WarnFunctionOverloading || !nameDic.ContainsKey(labelName) || !nameDic.ContainsKey(labelName))
			{
				return;
			}
			switch (nameDic[labelName])
			{
			case DefinedNameType.Reserved:
				if (Config.AllowFunctionOverloading)
				{
					errMes = "関数名" + labelName + "はEmueraの予約語と衝突しています。Emuera専用構文の構文解析に支障をきたす恐れがあります";
					warnLevel = 1;
				}
				else
				{
					errMes = "関数名" + labelName + "はEmueraの予約語です";
					warnLevel = 2;
				}
				break;
			case DefinedNameType.SystemMethod:
				if (Config.AllowFunctionOverloading)
				{
					errMes = "関数名" + labelName + "はEmueraの式中関数を上書きします";
					warnLevel = 1;
				}
				else
				{
					errMes = "関数名" + labelName + "はEmueraの式中関数名として使われています";
					warnLevel = 2;
				}
				break;
			case DefinedNameType.SystemVariable:
				errMes = "関数名" + labelName + "はEmueraの変数で使われています";
				warnLevel = 1;
				break;
			case DefinedNameType.SystemInstrument:
				errMes = "関数名" + labelName + "はEmueraの変数もしくは命令で使われています";
				warnLevel = 1;
				break;
			case DefinedNameType.UserMacro:
				errMes = "関数名" + labelName + "はマクロに使用されています";
				warnLevel = 2;
				break;
			case DefinedNameType.UserRefMethod:
				errMes = "関数名" + labelName + "は参照型関数の名称に使用されています";
				warnLevel = 2;
				break;
			case DefinedNameType.UserGlobalVariable:
				break;
			}
		}
	}

	public void CheckUserVarName(ref string errMes, ref int warnLevel, string varName)
	{
		if (varName.IndexOfAny(badSymbolAsIdentifier) >= 0)
		{
			errMes = "変数名" + varName + "に\"_\"以外の記号が含まれています";
			warnLevel = 2;
		}
		else if (nameDic.ContainsKey(varName))
		{
			switch (nameDic[varName])
			{
			case DefinedNameType.Reserved:
				errMes = "変数名" + varName + "はEmueraの予約語です";
				warnLevel = 2;
				break;
			case DefinedNameType.SystemMethod:
			case DefinedNameType.SystemInstrument:
				errMes = "変数名" + varName + "はEmueraの命令名として使われています";
				warnLevel = 2;
				break;
			case DefinedNameType.SystemVariable:
				errMes = "変数名" + varName + "はEmueraの変数名として使われています";
				warnLevel = 2;
				break;
			case DefinedNameType.UserMacro:
				errMes = "変数名" + varName + "は既にマクロ名に使用されています";
				warnLevel = 2;
				break;
			case DefinedNameType.UserGlobalVariable:
				errMes = "変数名" + varName + "はユーザー定義の広域変数名に使用されています";
				warnLevel = 2;
				break;
			case DefinedNameType.UserRefMethod:
				errMes = "変数名" + varName + "は参照型関数の名称に使用されています";
				warnLevel = 2;
				break;
			}
		}
	}

	public void CheckUserMacroName(ref string errMes, ref int warnLevel, string macroName)
	{
		if (macroName.IndexOfAny(badSymbolAsIdentifier) >= 0)
		{
			errMes = "マクロ名" + macroName + "に\"_\"以外の記号が含まれています";
			warnLevel = 2;
		}
		else if (nameDic.ContainsKey(macroName))
		{
			switch (nameDic[macroName])
			{
			case DefinedNameType.Reserved:
				errMes = "マクロ名" + macroName + "はEmueraの予約語です";
				warnLevel = 2;
				break;
			case DefinedNameType.SystemMethod:
			case DefinedNameType.SystemInstrument:
				errMes = "マクロ名" + macroName + "はEmueraの命令名として使われています";
				warnLevel = 2;
				break;
			case DefinedNameType.SystemVariable:
				errMes = "マクロ名" + macroName + "はEmueraの変数名として使われています";
				warnLevel = 2;
				break;
			case DefinedNameType.UserMacro:
				errMes = "マクロ名" + macroName + "は既にマクロ名に使用されています";
				warnLevel = 2;
				break;
			case DefinedNameType.UserGlobalVariable:
				errMes = "マクロ名" + macroName + "はユーザー定義の広域変数名に使用されています";
				warnLevel = 2;
				break;
			case DefinedNameType.UserRefMethod:
				errMes = "マクロ名" + macroName + "は参照型関数の名称に使用されています";
				warnLevel = 2;
				break;
			}
		}
	}

	public void CheckUserPrivateVarName(ref string errMes, ref int warnLevel, string varName)
	{
		if (varName.Length == 0)
		{
			errMes = "変数名がありません";
			warnLevel = 2;
			return;
		}
		if (varName.IndexOfAny(badSymbolAsIdentifier) >= 0)
		{
			errMes = "変数名" + varName + "に\"_\"以外の記号が含まれています";
			warnLevel = 2;
			return;
		}
		if (char.IsDigit(varName[0]))
		{
			errMes = "変数名" + varName + "が半角数字から始まっています";
			warnLevel = 2;
			return;
		}
		if (nameDic.ContainsKey(varName))
		{
			switch (nameDic[varName])
			{
			case DefinedNameType.Reserved:
				errMes = "変数名" + varName + "はEmueraの予約語です";
				warnLevel = 2;
				return;
			case DefinedNameType.SystemMethod:
			case DefinedNameType.SystemInstrument:
				errMes = "変数名" + varName + "はEmueraの命令名として使われています";
				warnLevel = 2;
				return;
			case DefinedNameType.SystemVariable:
				errMes = "変数名" + varName + "はEmueraの変数名として使われています";
				warnLevel = 2;
				break;
			case DefinedNameType.UserMacro:
				errMes = "変数名" + varName + "はマクロに使用されています";
				warnLevel = 2;
				break;
			case DefinedNameType.UserGlobalVariable:
				errMes = "変数名" + varName + "はユーザー定義の広域変数名に使用されています";
				warnLevel = 2;
				break;
			case DefinedNameType.UserRefMethod:
				errMes = "変数名" + varName + "は参照型関数の名称に使用されています";
				warnLevel = 2;
				break;
			}
		}
		privateDimList.Add(varName);
	}

	internal void AddUseDefinedVariable(VariableToken var)
	{
		varTokenDic.Add(var.Name, var);
		_ = var.IsCharacterData;
		nameDic.Add(var.Name, DefinedNameType.UserGlobalVariable);
	}

	internal void AddMacro(DefineMacro mac)
	{
		nameDic.Add(mac.Keyword, DefinedNameType.UserMacro);
		macroDic.Add(mac.Keyword, mac);
	}

	internal void AddRefMethod(UserDefinedRefMethod refm)
	{
		refmethodDic.Add(refm.Name, refm);
		nameDic.Add(refm.Name, DefinedNameType.UserRefMethod);
	}

	public bool UseMacro()
	{
		return macroDic.Count > 0;
	}

	public DefineMacro GetMacro(string key)
	{
		if (Config.ICVariable)
		{
			key = key.ToUpper();
		}
		if (macroDic.ContainsKey(key))
		{
			return macroDic[key];
		}
		return null;
	}

	public VariableToken GetVariableToken(string key, string subKey, bool allowPrivate)
	{
		VariableToken value = null;
		if (Config.ICVariable)
		{
			key = key.ToUpper();
		}
		if (allowPrivate)
		{
			LogicalLine scaningLine = GlobalStatic.Process.GetScaningLine();
			if (scaningLine != null && scaningLine.ParentLabelLine != null)
			{
				value = scaningLine.ParentLabelLine.GetPrivateVariable(key);
				if (value != null)
				{
					if (subKey != null)
					{
						throw new CodeEE("プライベート変数" + key + "に対して@が使われました");
					}
					return value;
				}
			}
		}
		if (localvarTokenDic.ContainsKey(key))
		{
			if (localvarTokenDic[key].IsForbid)
			{
				throw new CodeEE("呼び出された変数\"" + key + "\"は設定により使用が禁止されています");
			}
			LogicalLine scaningLine2 = GlobalStatic.Process.GetScaningLine();
			if (string.IsNullOrEmpty(subKey))
			{
				if (scaningLine2 == null || scaningLine2.ParentLabelLine == null)
				{
					throw new CodeEE("実行中の関数が存在しないため" + key + "を取得又は変更できませんでした");
				}
				subKey = scaningLine2.ParentLabelLine.LabelName;
			}
			else
			{
				ParserMediator.Warn("コード中でローカル変数を@付きで呼ぶことは推奨されません(代わりに*.ERHファイルの利用を検討してください)", scaningLine2, 1, isError: false, isBackComp: false);
				if (Config.ICFunction)
				{
					subKey = subKey.ToUpper();
				}
			}
			LocalVariableToken localVariableToken = localvarTokenDic[key].GetExistLocalVariableToken(subKey);
			if (localVariableToken == null)
			{
				localVariableToken = localvarTokenDic[key].GetNewLocalVariableToken(subKey, scaningLine2.ParentLabelLine);
			}
			return localVariableToken;
		}
		if (varTokenDic.TryGetValue(key, out value))
		{
			if (value.IsForbid)
			{
				if (!value.CanForbid)
				{
					throw new ExeEE("CanForbidでない変数\"" + value.Name + "\"にIsForbidがついている");
				}
				throw new CodeEE("呼び出された変数\"" + value.Name + "\"は設定により使用が禁止されています");
			}
			if (subKey != null)
			{
				throw new CodeEE("ローカル変数でない変数" + key + "に対して@が使われました");
			}
			return value;
		}
		if (subKey != null)
		{
			throw new CodeEE("@の使い方が不正です");
		}
		return null;
	}

	public FunctionIdentifier GetFunctionIdentifier(string str)
	{
		string text = str;
		FunctionIdentifier value = null;
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (Config.ICFunction)
		{
			text = text.ToUpper();
		}
		instructionDic.TryGetValue(text, out value);
		return value;
	}

	public List<string> GetOverloadedList(LabelDictionary labelDic)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, FunctionMethod> item in methodDic)
		{
			FunctionLabelLine nonEventLabel = labelDic.GetNonEventLabel(item.Key);
			if (nonEventLabel != null && nonEventLabel.IsMethod)
			{
				list.Add(item.Key);
			}
		}
		return list;
	}

	public UserDefinedRefMethod GetRefMethod(string codeStr)
	{
		if (Config.ICFunction)
		{
			codeStr = codeStr.ToUpper();
		}
		if (refmethodDic.ContainsKey(codeStr))
		{
			return refmethodDic[codeStr];
		}
		return null;
	}

	public IOperandTerm GetFunctionMethod(LabelDictionary labelDic, string codeStr, IOperandTerm[] arguments, bool userDefinedOnly)
	{
		if (Config.ICFunction)
		{
			codeStr = codeStr.ToUpper();
		}
		if (arguments == null)
		{
			if (refmethodDic.ContainsKey(codeStr))
			{
				return new UserDefinedRefMethodNoArgTerm(refmethodDic[codeStr]);
			}
			return null;
		}
		if (labelDic != null && labelDic.Initialized)
		{
			if (refmethodDic.ContainsKey(codeStr))
			{
				return new UserDefinedRefMethodTerm(refmethodDic[codeStr], arguments);
			}
			FunctionLabelLine nonEventLabel = labelDic.GetNonEventLabel(codeStr);
			if (nonEventLabel != null)
			{
				if (userDefinedOnly && !nonEventLabel.IsMethod)
				{
					throw new CodeEE("#FUNCTIONが指定されていない関数\"@" + nonEventLabel.LabelName + "\"をCALLF系命令で呼び出そうとしました");
				}
				if (nonEventLabel.IsMethod)
				{
					string errMes;
					return UserDefinedMethodTerm.Create(nonEventLabel, arguments, out errMes) ?? throw new CodeEE(errMes);
				}
				if (!methodDic.ContainsKey(codeStr))
				{
					throw new CodeEE("#FUNCTIONが定義されていない関数(" + nonEventLabel.Position.Filename + ":" + nonEventLabel.Position.LineNo + "行目)を式中で呼び出そうとしました");
				}
			}
		}
		if (userDefinedOnly)
		{
			return null;
		}
		FunctionMethod value = null;
		if (!methodDic.TryGetValue(codeStr, out value))
		{
			return null;
		}
		string text = value.CheckArgumentType(codeStr, arguments);
		if (text != null)
		{
			throw new CodeEE(text);
		}
		return new FunctionMethodTerm(value, arguments);
	}

	public void ThrowException(string str, bool isFunc)
	{
		string text = str;
		if (Config.ICFunction || Config.ICVariable)
		{
			text = text.ToUpper();
		}
		if (disableList.Contains(text))
		{
			throw new CodeEE("\"" + str + "\"は#DISABLEが宣言されています");
		}
		if (!isFunc && privateDimList.Contains(text))
		{
			throw new CodeEE("変数\"" + str + "\"はこの関数中では定義されていません");
		}
		if (nameDic.ContainsKey(text))
		{
			switch (nameDic[text])
			{
			case DefinedNameType.Reserved:
				throw new CodeEE("Emueraの予約語\"" + str + "\"が不正な使われ方をしています");
			case DefinedNameType.SystemVariable:
			case DefinedNameType.UserGlobalVariable:
				if (isFunc)
				{
					throw new CodeEE("変数名\"" + str + "\"が関数のように使われています");
				}
				break;
			case DefinedNameType.SystemMethod:
			case DefinedNameType.UserRefMethod:
				if (!isFunc)
				{
					throw new CodeEE("関数名\"" + str + "\"が変数のように使われています");
				}
				break;
			case DefinedNameType.UserMacro:
				throw new CodeEE("予期しないマクロ名\"" + str + "\"です");
			case DefinedNameType.SystemInstrument:
				if (isFunc)
				{
					throw new CodeEE("命令名\"" + str + "\"が関数のように使われています");
				}
				throw new CodeEE("命令名\"" + str + "\"が変数のように使われています");
			}
		}
		throw new CodeEE("\"" + text + "\"は解釈できない識別子です");
	}

	public void resizeLocalVars(string key, string subKey, int newSize)
	{
		localvarTokenDic[key].ResizeLocalVariableToken(subKey, newSize);
	}

	public int getLocalDefaultSize(string key)
	{
		return localvarTokenDic[key].GetDefaultSize();
	}

	public bool getLocalIsForbid(string key)
	{
		return localvarTokenDic[key].IsForbid;
	}

	public bool getVarTokenIsForbid(string key)
	{
		if (localvarTokenDic.ContainsKey(key))
		{
			return localvarTokenDic[key].IsForbid;
		}
		VariableToken value = null;
		varTokenDic.TryGetValue(key, out value);
		return value?.IsForbid ?? true;
	}
}
