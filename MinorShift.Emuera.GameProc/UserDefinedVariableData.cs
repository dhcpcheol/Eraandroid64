using System.Collections.Generic;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal sealed class UserDefinedVariableData
{
	public string Name;

	public bool TypeIsStr;

	public bool Reference;

	public int Dimension = 1;

	public int[] Lengths;

	public long[] DefaultInt;

	public string[] DefaultStr;

	public bool Global;

	public bool Save;

	public bool Static = true;

	public bool Private;

	public bool CharaData;

	public bool Const;

	public static UserDefinedVariableData Create(WordCollection wc, bool dims, bool isPrivate, ScriptPosition sc)
	{
		string obj = (dims ? "#DIM" : "#DIMS");
		UserDefinedVariableData userDefinedVariableData = new UserDefinedVariableData
		{
			TypeIsStr = dims
		};
		IdentifierWord identifierWord = null;
		bool flag = false;
		userDefinedVariableData.Const = false;
		string text = obj;
		new List<string>();
		while (!wc.EOL && wc.Current is IdentifierWord identifierWord2)
		{
			wc.ShiftNext();
			text = identifierWord2.Code;
			if (Config.ICVariable)
			{
				text = text.ToUpper();
			}
			switch (text)
			{
			case "CONST":
				if (userDefinedVariableData.CharaData)
				{
					throw new CodeEE(text + "とCHARADATAキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Global)
				{
					throw new CodeEE(text + "とGLOBALキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Save)
				{
					throw new CodeEE(text + "とSAVEDATAキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Reference)
				{
					throw new CodeEE(text + "とREFキーワードは同時に指定できません", sc);
				}
				if (!userDefinedVariableData.Static)
				{
					throw new CodeEE(text + "とDYNAMICキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Const)
				{
					throw new CodeEE(text + "キーワードが二重に指定されています", sc);
				}
				userDefinedVariableData.Const = true;
				continue;
			case "REF":
				if (flag && userDefinedVariableData.Static)
				{
					throw new CodeEE(text + "とSTATICキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.CharaData)
				{
					throw new CodeEE(text + "とCHARADATAキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Global)
				{
					throw new CodeEE(text + "とGLOBALキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Save)
				{
					throw new CodeEE(text + "とSAVEDATAキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Const)
				{
					throw new CodeEE(text + "とCONSTキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Reference)
				{
					throw new CodeEE(text + "キーワードが二重に指定されています", sc);
				}
				userDefinedVariableData.Reference = true;
				userDefinedVariableData.Static = true;
				continue;
			case "DYNAMIC":
				if (!isPrivate)
				{
					throw new CodeEE("広域変数の宣言に" + text + "キーワードは指定できません", sc);
				}
				if (userDefinedVariableData.CharaData)
				{
					throw new CodeEE(text + "とCHARADATAキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Const)
				{
					throw new CodeEE(text + "とCONSTキーワードは同時に指定できません", sc);
				}
				if (flag)
				{
					if (userDefinedVariableData.Static)
					{
						throw new CodeEE("STATICとDYNAMICキーワードは同時に指定できません", sc);
					}
					throw new CodeEE(text + "キーワードが二重に指定されています", sc);
				}
				flag = true;
				userDefinedVariableData.Static = false;
				continue;
			case "STATIC":
				if (!isPrivate)
				{
					throw new CodeEE("広域変数の宣言に" + text + "キーワードは指定できません", sc);
				}
				if (userDefinedVariableData.CharaData)
				{
					throw new CodeEE(text + "とCHARADATAキーワードは同時に指定できません", sc);
				}
				if (flag)
				{
					if (!userDefinedVariableData.Static)
					{
						throw new CodeEE("STATICとDYNAMICキーワードは同時に指定できません", sc);
					}
					throw new CodeEE(text + "キーワードが二重に指定されています", sc);
				}
				if (userDefinedVariableData.Reference)
				{
					throw new CodeEE(text + "とREFキーワードは同時に指定できません", sc);
				}
				flag = true;
				userDefinedVariableData.Static = true;
				continue;
			case "GLOBAL":
				if (isPrivate)
				{
					throw new CodeEE("ローカル変数の宣言に" + text + "キーワードは指定できません", sc);
				}
				if (userDefinedVariableData.CharaData)
				{
					throw new CodeEE(text + "とCHARADATAキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Reference)
				{
					throw new CodeEE(text + "とREFキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Const)
				{
					throw new CodeEE(text + "とCONSTキーワードは同時に指定できません", sc);
				}
				if (flag)
				{
					if (userDefinedVariableData.Static)
					{
						throw new CodeEE("STATICとGLOBALキーワードは同時に指定できません", sc);
					}
					throw new CodeEE("DYNAMICとGLOBALキーワードは同時に指定できません", sc);
				}
				userDefinedVariableData.Global = true;
				continue;
			case "SAVEDATA":
				if (isPrivate)
				{
					throw new CodeEE("ローカル変数の宣言に" + text + "キーワードは指定できません", sc);
				}
				if (flag)
				{
					if (userDefinedVariableData.Static)
					{
						throw new CodeEE("STATICとSAVEDATAキーワードは同時に指定できません", sc);
					}
					throw new CodeEE("DYNAMICとSAVEDATAキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Reference)
				{
					throw new CodeEE(text + "とREFキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Const)
				{
					throw new CodeEE(text + "とCONSTキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Save)
				{
					throw new CodeEE(text + "キーワードが二重に指定されています", sc);
				}
				userDefinedVariableData.Save = true;
				continue;
			case "CHARADATA":
				if (isPrivate)
				{
					throw new CodeEE("ローカル変数の宣言に" + text + "キーワードは指定できません", sc);
				}
				if (userDefinedVariableData.Reference)
				{
					throw new CodeEE(text + "とREFキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Const)
				{
					throw new CodeEE(text + "とCONSTキーワードは同時に指定できません", sc);
				}
				if (flag)
				{
					if (userDefinedVariableData.Static)
					{
						throw new CodeEE(text + "とSTATICキーワードは同時に指定できません", sc);
					}
					throw new CodeEE(text + "とDYNAMICキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.Global)
				{
					throw new CodeEE(text + "とGLOBALキーワードは同時に指定できません", sc);
				}
				if (userDefinedVariableData.CharaData)
				{
					throw new CodeEE(text + "キーワードが二重に指定されています", sc);
				}
				userDefinedVariableData.CharaData = true;
				continue;
			}
			userDefinedVariableData.Name = text;
			break;
		}
		if (userDefinedVariableData.Name == null)
		{
			throw new CodeEE(text + "の後に有効な変数名が指定されていません", sc);
		}
		string errMes = "";
		int warnLevel = -1;
		if (isPrivate)
		{
			GlobalStatic.IdentifierDictionary.CheckUserPrivateVarName(ref errMes, ref warnLevel, userDefinedVariableData.Name);
		}
		else
		{
			GlobalStatic.IdentifierDictionary.CheckUserVarName(ref errMes, ref warnLevel, userDefinedVariableData.Name);
		}
		if (warnLevel >= 0)
		{
			if (warnLevel >= 2)
			{
				throw new CodeEE(errMes, sc);
			}
			ParserMediator.Warn(errMes, sc, warnLevel);
		}
		List<int> list = new List<int>();
		if (wc.EOL)
		{
			if (userDefinedVariableData.Const)
			{
				throw new CodeEE("CONSTキーワードが指定されていますが初期値が設定されていません");
			}
			list.Add(1);
		}
		else if (wc.Current.Type == ',')
		{
			while (!wc.EOL && wc.Current.Type != '=')
			{
				if (wc.Current.Type != ',')
				{
					throw new CodeEE("書式が間違っています", sc);
				}
				wc.ShiftNext();
				if (userDefinedVariableData.Reference)
				{
					list.Add(0);
					if (wc.EOL)
					{
						break;
					}
					if (wc.Current.Type == ',')
					{
						continue;
					}
				}
				if (wc.EOL)
				{
					throw new CodeEE("カンマの後に有効な定数式が指定されていません", sc);
				}
				if (!(ExpressionParser.ReduceIntegerTerm(wc, TermEndWith.Comma_Assignment).Restructure(GlobalStatic.EMediator) is SingleTerm singleTerm) || singleTerm.GetOperandType() != typeof(long))
				{
					throw new CodeEE("カンマの後に有効な定数式が指定されていません", sc);
				}
				if (userDefinedVariableData.Reference)
				{
					if (singleTerm.Int != 0L)
					{
						throw new CodeEE("参照型変数にはサイズを指定できません(サイズを省略するか0を指定してください)", sc);
					}
					continue;
				}
				if (singleTerm.Int <= 0 || singleTerm.Int > 1000000)
				{
					throw new CodeEE("ユーザー定義変数のサイズは1以上1000000以下でなければなりません", sc);
				}
				list.Add((int)singleTerm.Int);
			}
		}
		if (wc.Current.Type != '=')
		{
			if (userDefinedVariableData.Const)
			{
				throw new CodeEE("CONSTキーワードが指定されていますが初期値が設定されていません");
			}
		}
		else
		{
			if (((OperatorWord)wc.Current).Code != OperatorCode.Assignment)
			{
				throw new CodeEE("予期しない演算子を発見しました");
			}
			if (userDefinedVariableData.Reference)
			{
				throw new CodeEE("参照型変数には初期値を設定できません");
			}
			if (list.Count >= 2)
			{
				throw new CodeEE("多次元変数には初期値を設定できません");
			}
			if (userDefinedVariableData.CharaData)
			{
				throw new CodeEE("キャラ型変数には初期値を設定できません");
			}
			int num = 0;
			if (list.Count == 1)
			{
				num = list[0];
			}
			wc.ShiftNext();
			IOperandTerm[] array = ExpressionParser.ReduceArguments(wc, ArgsEndWith.EoL, isDefine: false);
			if (array.Length == 0)
			{
				throw new CodeEE("配列の初期値は省略できません");
			}
			if (num > 0)
			{
				if (array.Length > num)
				{
					throw new CodeEE("初期値の数が配列のサイズを超えています");
				}
				if (userDefinedVariableData.Const && array.Length != num)
				{
					throw new CodeEE("定数の初期値の数が配列のサイズと一致しません");
				}
			}
			if (dims)
			{
				userDefinedVariableData.DefaultStr = new string[array.Length];
			}
			else
			{
				userDefinedVariableData.DefaultInt = new long[array.Length];
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					throw new CodeEE("配列の初期値は省略できません");
				}
				array[i] = array[i].Restructure(GlobalStatic.EMediator);
				if (!(array[i] is SingleTerm singleTerm2))
				{
					throw new CodeEE("配列の初期値には定数のみ指定できます");
				}
				if (dims != singleTerm2.IsString)
				{
					throw new CodeEE("変数の型と初期値の型が一致していません");
				}
				if (dims)
				{
					userDefinedVariableData.DefaultStr[i] = singleTerm2.Str;
				}
				else
				{
					userDefinedVariableData.DefaultInt[i] = singleTerm2.Int;
				}
			}
			if (list.Count == 0)
			{
				list.Add(array.Length);
			}
		}
		if (!wc.EOL)
		{
			throw new CodeEE("書式が間違っています", sc);
		}
		if (list.Count == 0)
		{
			list.Add(1);
		}
		userDefinedVariableData.Private = isPrivate;
		userDefinedVariableData.Dimension = list.Count;
		if (userDefinedVariableData.Const && userDefinedVariableData.Dimension > 1)
		{
			throw new CodeEE("CONSTキーワードが指定された変数を多次元配列にはできません");
		}
		if (userDefinedVariableData.CharaData && userDefinedVariableData.Dimension > 2)
		{
			throw new CodeEE("3次元以上のキャラ型変数を宣言することはできません", sc);
		}
		if (userDefinedVariableData.Dimension > 3)
		{
			throw new CodeEE("4次元以上の配列変数を宣言することはできません", sc);
		}
		userDefinedVariableData.Lengths = new int[list.Count];
		if (userDefinedVariableData.Reference)
		{
			return userDefinedVariableData;
		}
		long num2 = 1L;
		for (int j = 0; j < list.Count; j++)
		{
			userDefinedVariableData.Lengths[j] = list[j];
			num2 *= userDefinedVariableData.Lengths[j];
		}
		if (num2 <= 0 || num2 > 1000000)
		{
			throw new CodeEE("ユーザー定義変数のサイズは1以上1000000以下でなければなりません", sc);
		}
		if (!isPrivate && userDefinedVariableData.Save && !Config.SystemSaveInBinary)
		{
			if (dims && userDefinedVariableData.Dimension > 1)
			{
				throw new CodeEE("文字列型の多次元配列変数にSAVEDATAフラグを付ける場合には「バイナリ型セーブ」オプションが必須です", sc);
			}
			if (userDefinedVariableData.CharaData)
			{
				throw new CodeEE("キャラ型変数にSAVEDATAフラグを付ける場合には「バイナリ型セーブ」オプションが必須です", sc);
			}
		}
		return userDefinedVariableData;
	}
}
