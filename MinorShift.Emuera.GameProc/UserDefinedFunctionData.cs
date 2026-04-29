using System.Collections.Generic;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal sealed class UserDefinedFunctionData
{
	public string Name;

	public bool TypeIsStr;

	public UserDifinedFunctionDataArgType[] ArgList;

	private UserDefinedFunctionData()
	{
	}

	public static UserDefinedFunctionData Create(WordCollection wc, bool dims, ScriptPosition sc)
	{
		string obj = (dims ? "#FUNCTION" : "#FUNCTIONS");
		UserDefinedFunctionData userDefinedFunctionData = new UserDefinedFunctionData
		{
			TypeIsStr = dims
		};
		IdentifierWord identifierWord = null;
		string text = obj;
		if (!wc.EOL && wc.Current is IdentifierWord identifierWord2)
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
			case "REF":
			case "DYNAMIC":
			case "STATIC":
			case "GLOBAL":
			case "SAVEDATA":
			case "CHARADATA":
				throw new CodeEE(dims + "中では" + text + "キーワードは指定できません", sc);
			}
			userDefinedFunctionData.Name = text;
		}
		if (userDefinedFunctionData.Name == null)
		{
			throw new CodeEE(text + "の後に有効な識別子が指定されていません", sc);
		}
		if (wc.EOL || wc.Current.Type != '(')
		{
			throw new CodeEE("識別子の後に引数定義がありません", sc);
		}
		string errMes = "";
		int warnLevel = -1;
		GlobalStatic.IdentifierDictionary.CheckUserLabelName(ref errMes, ref warnLevel, isFunction: true, userDefinedFunctionData.Name);
		if (warnLevel == 0)
		{
			GlobalStatic.IdentifierDictionary.CheckUserVarName(ref errMes, ref warnLevel, userDefinedFunctionData.Name);
		}
		if (warnLevel >= 0)
		{
			if (warnLevel >= 2)
			{
				throw new CodeEE(errMes, sc);
			}
			ParserMediator.Warn(errMes, sc, warnLevel);
		}
		List<UserDifinedFunctionDataArgType> list = new List<UserDifinedFunctionDataArgType>();
		UserDifinedFunctionDataArgType userDifinedFunctionDataArgType = UserDifinedFunctionDataArgType.Null;
		int num = 0;
		while (true)
		{
			wc.ShiftNext();
			switch (wc.Current.Type)
			{
			case '\0':
				throw new CodeEE("括弧が閉じられていません", sc);
			case ')':
				switch (num)
				{
				case 4:
				case 5:
					if ((userDifinedFunctionDataArgType & UserDifinedFunctionDataArgType.__Dimention) == 0)
					{
						throw new CodeEE("REF引数は配列変数でなければなりません", sc);
					}
					num = 2;
					list.Add(userDifinedFunctionDataArgType);
					break;
				default:
					throw new CodeEE("予期しない括弧です", sc);
				case 0:
				case 1:
					break;
				}
				wc.ShiftNext();
				if (!wc.EOL)
				{
					throw new CodeEE("宣言の後に余分な文字があります", sc);
				}
				userDefinedFunctionData.ArgList = new UserDifinedFunctionDataArgType[list.Count];
				list.CopyTo(userDefinedFunctionData.ArgList);
				return userDefinedFunctionData;
			case '0':
				if (((LiteralIntegerWord)wc.Current).Int == 0L && num == 5)
				{
					num = 4;
					continue;
				}
				break;
			case ':':
				if (num == 4 || num == 5)
				{
					num = 5;
					userDifinedFunctionDataArgType++;
					if ((userDifinedFunctionDataArgType & UserDifinedFunctionDataArgType.__Dimention) <= (UserDifinedFunctionDataArgType)3)
					{
						continue;
					}
					throw new CodeEE("REF引数は4次元以上の配列にできません", sc);
				}
				break;
			case ',':
				switch (num)
				{
				case 1:
					num = 2;
					continue;
				case 4:
				case 5:
					if ((userDifinedFunctionDataArgType & UserDifinedFunctionDataArgType.__Dimention) == 0)
					{
						throw new CodeEE("REF引数は配列変数でなければなりません", sc);
					}
					num = 2;
					list.Add(userDifinedFunctionDataArgType);
					continue;
				}
				break;
			case 'A':
			{
				string text2 = ((IdentifierWord)wc.Current).Code;
				if (Config.ICVariable)
				{
					text2 = text2.ToUpper();
				}
				switch (text2)
				{
				case "REF":
					if (num == 0 || num == 2)
					{
						num = 3;
						continue;
					}
					break;
				case "INT":
				case "STR":
					userDifinedFunctionDataArgType = ((!(text2 == "INT")) ? UserDifinedFunctionDataArgType.Str : UserDifinedFunctionDataArgType.Int);
					switch (num)
					{
					case 0:
					case 2:
						num = 1;
						list.Add(userDifinedFunctionDataArgType);
						continue;
					case 3:
						userDifinedFunctionDataArgType |= UserDifinedFunctionDataArgType.__Ref;
						num = 4;
						continue;
					}
					break;
				}
				break;
			}
			}
			break;
		}
		if (!wc.EOL)
		{
			throw new CodeEE("引数の解析中に予期しないトークン" + wc.Current.ToString() + "を発見しました", sc);
		}
		throw new CodeEE("引数の解析中にエラーが発生しました", sc);
	}
}
