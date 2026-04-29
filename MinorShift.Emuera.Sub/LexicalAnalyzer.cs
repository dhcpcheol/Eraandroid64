using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MinorShift.Emuera.GameData;
using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.Sub;

internal static class LexicalAnalyzer
{
	private const int MAX_EXPAND_MACRO = 100;

	private static readonly IList<char> hexadecimalDigits = new char[12]
	{
		'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D',
		'E', 'F'
	};

	public static bool UseMacro = true;

	public static long ReadInt64(StringStream st, bool retZero)
	{
		long num = 0L;
		int num2 = 0;
		int num3 = 0;
		int currentPosition = st.CurrentPosition;
		int currentPosition2 = st.CurrentPosition;
		int num4 = 10;
		if (st.Current == '0')
		{
			switch (st.Next)
			{
			case 'X':
			case 'x':
				num4 = 16;
				st.ShiftNext();
				st.ShiftNext();
				break;
			case 'B':
			case 'b':
				num4 = 2;
				st.ShiftNext();
				st.ShiftNext();
				break;
			}
		}
		if (retZero && st.Current != '+' && st.Current != '-' && !char.IsDigit(st.Current))
		{
			if (num4 != 16)
			{
				return 0L;
			}
			if (!hexadecimalDigits.Contains(st.Current))
			{
				return 0L;
			}
		}
		num = readDigits(st, num4);
		if (st.Current == 'p' || st.Current == 'P')
		{
			num2 = 2;
		}
		else if (st.Current == 'e' || st.Current == 'E')
		{
			num2 = 10;
		}
		if (num2 != 0)
		{
			st.ShiftNext();
			num3 = (int)readDigits(st, num4);
		}
		currentPosition2 = st.CurrentPosition;
		if (num2 != 0 && num3 != 0)
		{
			double num5 = (double)num * Math.Pow(num2, num3);
			if (double.IsNaN(num5) || double.IsInfinity(num5) || num5 > 9.223372036854776E+18 || num5 < -9.223372036854776E+18)
			{
				throw new CodeEE("\"" + st.Substring(currentPosition, currentPosition2) + "\"は64ビット符号付整数の範囲を超えています");
			}
			num = (long)num5;
		}
		return num;
	}

	private static long readDigits(StringStream st, int fromBase)
	{
		int currentPosition = st.CurrentPosition;
		char current = st.Current;
		if (current == '-' || current == '+')
		{
			st.ShiftNext();
		}
		if (fromBase == 10)
		{
			while (!st.EOS)
			{
				current = st.Current;
				if (!char.IsDigit(current))
				{
					break;
				}
				st.ShiftNext();
			}
		}
		else if (fromBase == 16)
		{
			while (!st.EOS)
			{
				current = st.Current;
				if (!char.IsDigit(current) && !hexadecimalDigits.Contains(current))
				{
					break;
				}
				st.ShiftNext();
			}
		}
		else if (fromBase == 2)
		{
			while (!st.EOS)
			{
				current = st.Current;
				if (!char.IsDigit(current))
				{
					break;
				}
				if (current != '0' && current != '1')
				{
					throw new CodeEE("二進法表記の中で使用できない文字が使われています");
				}
				st.ShiftNext();
			}
		}
		string text = st.Substring(currentPosition, st.CurrentPosition - currentPosition);
		try
		{
			return Convert.ToInt64(text, fromBase);
		}
		catch (FormatException)
		{
			throw new CodeEE("\"" + text + "\"は整数値に変換できません");
		}
		catch (OverflowException)
		{
			throw new CodeEE("\"" + text + "\"は64ビット符号付き整数の範囲を超えています");
		}
		catch (ArgumentOutOfRangeException)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new CodeEE("数値として認識できる文字が必要です");
			}
			throw new CodeEE("文字列\"" + text + "\"は数値として認識できません");
		}
	}

	public static double ReadDouble(StringStream st)
	{
		int currentPosition = st.CurrentPosition;
		if (st.Current == '-' || st.Current == '+')
		{
			st.ShiftNext();
		}
		while (!st.EOS)
		{
			char current = st.Current;
			if (!char.IsDigit(current) && current != '.')
			{
				break;
			}
			st.ShiftNext();
		}
		if (st.Current == 'e' || st.Current == 'E')
		{
			st.ShiftNext();
			if (st.Current == '-')
			{
				st.ShiftNext();
			}
			while (!st.EOS)
			{
				char current2 = st.Current;
				if (!char.IsDigit(current2) && current2 != '.')
				{
					break;
				}
				st.ShiftNext();
			}
		}
		return Convert.ToDouble(st.Substring(currentPosition, st.CurrentPosition - currentPosition));
	}

	public static IdentifierWord ReadFirstIdentifierWord(StringStream st)
	{
		_ = st.CurrentPosition;
		string text = ReadSingleIdentifier(st);
		if (string.IsNullOrEmpty(text))
		{
			throw new CodeEE("不正な文字で行が始まっています");
		}
		return new IdentifierWord(text);
	}

	public static IdentifierWord ReadSingleIdentifierWord(StringStream st)
	{
		string text = ReadSingleIdentifier(st);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (UseMacro)
		{
			int num = 0;
			while (true)
			{
				DefineMacro macro = GlobalStatic.IdentifierDictionary.GetMacro(text);
				num++;
				if (num > 100)
				{
					throw new CodeEE("マクロの展開数が1文あたりの上限値" + 100 + "を超えました(自己参照・循環参照のおそれ)");
				}
				if (macro == null)
				{
					break;
				}
				if (macro.IDWord != null)
				{
					throw new CodeEE("マクロ" + macro.Keyword + "はこの文脈では使用できません(1単語に置き換えるマクロのみが使用できます)");
				}
				text = macro.IDWord.Code;
			}
		}
		return new IdentifierWord(text);
	}

	public static string ReadSingleIdentifier(StringStream st)
	{
		int currentPosition = st.CurrentPosition;
		for (; !st.EOS; st.ShiftNext())
		{
			switch (st.Current)
			{
			case '\u3000':
				if (!Config.SystemAllowFullSpace)
				{
					throw new CodeEE("予期しない全角スペースを発見しました(この警告はシステムオプション「" + Config.GetConfigName(ConfigCode.SystemAllowFullSpace) + "」により無視できます)");
				}
				break;
			default:
				continue;
			case '\t':
			case ' ':
			case '!':
			case '"':
			case '#':
			case '$':
			case '%':
			case '&':
			case '\'':
			case '(':
			case ')':
			case '*':
			case '+':
			case ',':
			case '-':
			case '.':
			case '/':
			case ':':
			case ';':
			case '<':
			case '=':
			case '>':
			case '?':
			case '@':
			case '[':
			case '\\':
			case ']':
			case '^':
			case '{':
			case '|':
			case '}':
			case '~':
				break;
			}
			break;
		}
		return st.Substring(currentPosition, st.CurrentPosition - currentPosition);
	}

	public static string ReadString(StringStream st, StrEndWith endWith)
	{
		StringBuilder stringBuilder = new StringBuilder(100);
		while (true)
		{
			char current = st.Current;
			if ((uint)current <= 40u)
			{
				if ((uint)current <= 34u)
				{
					if (current == '\0' || (current == '"' && endWith == StrEndWith.DoubleQuotation))
					{
						break;
					}
				}
				else if (current != '\'')
				{
					if (current == '(')
					{
						goto IL_0086;
					}
				}
				else if (endWith == StrEndWith.SingleQuotation)
				{
					break;
				}
			}
			else if ((uint)current <= 59u)
			{
				if (current != ',')
				{
					if (current == ';')
					{
						goto IL_0086;
					}
				}
				else if (endWith == StrEndWith.Comma || endWith == StrEndWith.LeftParenthesis_Bracket_Comma_Semicolon)
				{
					break;
				}
			}
			else
			{
				if (current == '[')
				{
					goto IL_0086;
				}
				if (current == '\\')
				{
					st.ShiftNext();
					switch (st.Current)
					{
					case '\0':
						throw new CodeEE("エスケープ文字\\の後に文字がありません");
					case 's':
						stringBuilder.Append(' ');
						break;
					case 'S':
						stringBuilder.Append('\u3000');
						break;
					case 't':
						stringBuilder.Append('\t');
						break;
					case 'n':
						stringBuilder.Append('\n');
						break;
					default:
						stringBuilder.Append(st.Current);
						break;
					case '\n':
						break;
					}
					st.ShiftNext();
					continue;
				}
			}
			goto IL_0116;
			IL_0086:
			if (endWith == StrEndWith.LeftParenthesis_Bracket_Comma_Semicolon)
			{
				break;
			}
			goto IL_0116;
			IL_0116:
			stringBuilder.Append(st.Current);
			st.ShiftNext();
		}
		return stringBuilder.ToString();
	}

	public static OperatorCode ReadOperator(StringStream st, bool allowAssignment)
	{
		char current = st.Current;
		st.ShiftNext();
		char current2 = st.Current;
		switch (current)
		{
		case '+':
			if (current2 == '+')
			{
				st.ShiftNext();
				return OperatorCode.Increment;
			}
			return OperatorCode.Plus;
		case '-':
			if (current2 == '-')
			{
				st.ShiftNext();
				return OperatorCode.Decrement;
			}
			return OperatorCode.Minus;
		case '*':
			return OperatorCode.Mult;
		case '/':
			return OperatorCode.Div;
		case '%':
			return OperatorCode.Mod;
		case '=':
			if (current2 == '=')
			{
				st.ShiftNext();
				return OperatorCode.Equal;
			}
			if (allowAssignment)
			{
				return OperatorCode.Assignment;
			}
			throw new CodeEE("予期しない代入演算子'='を発見しました(等価比較には'=='を使用してください)");
		case '!':
			switch (current2)
			{
			case '=':
				st.ShiftNext();
				return OperatorCode.NotEqual;
			case '&':
				st.ShiftNext();
				return OperatorCode.Nand;
			case '|':
				st.ShiftNext();
				return OperatorCode.Nor;
			default:
				return OperatorCode.Not;
			}
		case '<':
			switch (current2)
			{
			case '=':
				st.ShiftNext();
				return OperatorCode.LessEqual;
			case '<':
				st.ShiftNext();
				return OperatorCode.LeftShift;
			default:
				return OperatorCode.Less;
			}
		case '>':
			switch (current2)
			{
			case '=':
				st.ShiftNext();
				return OperatorCode.GreaterEqual;
			case '>':
				st.ShiftNext();
				return OperatorCode.RightShift;
			default:
				return OperatorCode.Greater;
			}
		case '|':
			if (current2 == '|')
			{
				st.ShiftNext();
				return OperatorCode.Or;
			}
			return OperatorCode.BitOr;
		case '&':
			if (current2 == '&')
			{
				st.ShiftNext();
				return OperatorCode.And;
			}
			return OperatorCode.BitAnd;
		case '^':
			if (current2 == '^')
			{
				st.ShiftNext();
				return OperatorCode.Xor;
			}
			return OperatorCode.BitXor;
		case '~':
			return OperatorCode.BitNot;
		case '?':
			return OperatorCode.Ternary_a;
		case '#':
			return OperatorCode.Ternary_b;
		default:
			throw new CodeEE("'" + current + "'は演算子として認識できません");
		}
	}

	public static OperatorCode ReadAssignmentOperator(StringStream st)
	{
		OperatorCode operatorCode = OperatorCode.NULL;
		char current = st.Current;
		st.ShiftNext();
		char current2 = st.Current;
		switch (current)
		{
		case '+':
			switch (current2)
			{
			case '+':
				operatorCode = OperatorCode.Increment;
				break;
			case '=':
				operatorCode = OperatorCode.Plus;
				break;
			}
			break;
		case '-':
			switch (current2)
			{
			case '-':
				operatorCode = OperatorCode.Decrement;
				break;
			case '=':
				operatorCode = OperatorCode.Minus;
				break;
			}
			break;
		case '*':
			if (current2 == '=')
			{
				operatorCode = OperatorCode.Mult;
			}
			break;
		case '/':
			if (current2 == '=')
			{
				operatorCode = OperatorCode.Div;
			}
			break;
		case '%':
			if (current2 == '=')
			{
				operatorCode = OperatorCode.Mod;
			}
			break;
		case '=':
			if (current2 == '=')
			{
				operatorCode = OperatorCode.Equal;
				break;
			}
			return OperatorCode.Assignment;
		case '\'':
			if (current2 == '=')
			{
				operatorCode = OperatorCode.AssignmentStr;
				break;
			}
			throw new CodeEE("\"'\"は代入演算子として認識できません");
		case '<':
			if (current2 == '<')
			{
				st.ShiftNext();
				if (st.Current != '=')
				{
					throw new CodeEE("'<'は代入演算子として認識できません");
				}
				operatorCode = OperatorCode.LeftShift;
			}
			break;
		case '>':
			if (current2 == '>')
			{
				st.ShiftNext();
				if (st.Current != '=')
				{
					throw new CodeEE("'>'は代入演算子として認識できません");
				}
				operatorCode = OperatorCode.RightShift;
			}
			break;
		case '|':
			if (current2 == '=')
			{
				operatorCode = OperatorCode.BitOr;
			}
			break;
		case '&':
			if (current2 == '=')
			{
				operatorCode = OperatorCode.BitAnd;
			}
			break;
		case '^':
			if (current2 == '=')
			{
				operatorCode = OperatorCode.BitXor;
			}
			break;
		}
		if (operatorCode == OperatorCode.NULL)
		{
			throw new CodeEE("'" + current + "'は代入演算子として認識できません");
		}
		st.ShiftNext();
		return operatorCode;
	}

	public static int SkipAllSpace(StringStream st)
	{
		int num = 0;
		while (true)
		{
			char current = st.Current;
			if (current != '\t' && current != ' ' && current != '\u3000')
			{
				break;
			}
			num++;
			st.ShiftNext();
		}
		return num;
	}

	public static bool IsWhiteSpace(char c)
	{
		if (c != ' ' && c != '\t')
		{
			return c == '\u3000';
		}
		return true;
	}

	public static int SkipWhiteSpace(StringStream st)
	{
		int num = 0;
		while (true)
		{
			char current = st.Current;
			if ((uint)current <= 32u)
			{
				if (current != '\t' && current != ' ')
				{
					break;
				}
			}
			else
			{
				if (current == ';')
				{
					if (st.CurrentEqualTo(";#;") && Program.DebugMode)
					{
						st.Jump(3);
						continue;
					}
					if (st.CurrentEqualTo(";!;"))
					{
						st.Jump(3);
						continue;
					}
					st.Seek(0, SeekOrigin.End);
					return num;
				}
				if (current != '\u3000')
				{
					break;
				}
				if (!Config.SystemAllowFullSpace)
				{
					return num;
				}
			}
			num++;
			st.ShiftNext();
		}
		return num;
	}

	public static int SkipHalfSpace(StringStream st)
	{
		int num = 0;
		while (st.Current == ' ')
		{
			num++;
			st.ShiftNext();
		}
		return num;
	}

	public static WordCollection Analyse(StringStream st, LexEndWith endWith, LexAnalyzeFlag flag)
	{
		WordCollection wordCollection = new WordCollection();
		int num = 0;
		int num2 = 0;
		while (true)
		{
			switch (st.Current)
			{
			case '\t':
			case ' ':
				st.ShiftNext();
				continue;
			case '\u3000':
				if (!Config.SystemAllowFullSpace)
				{
					throw new CodeEE("字句解析中に予期しない全角スペースを発見しました(この警告はシステムオプション「" + Config.GetConfigName(ConfigCode.SystemAllowFullSpace) + "」により無視できます)");
				}
				st.ShiftNext();
				continue;
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
				wordCollection.Add(new LiteralIntegerWord(ReadInt64(st, retZero: false)));
				continue;
			case '>':
				if (endWith == LexEndWith.GreaterThan)
				{
					break;
				}
				goto case '!';
			case '!':
			case '#':
			case '%':
			case '&':
			case '*':
			case '+':
			case '-':
			case '/':
			case '<':
			case '=':
			case '?':
			case '^':
			case '|':
			case '~':
				if (num != 0 || num2 != 0 || (endWith != LexEndWith.Operator && (endWith != LexEndWith.Percent || st.Current != '%') && (endWith != LexEndWith.Question || st.Current != '?')))
				{
					wordCollection.Add(new OperatorWord(ReadOperator(st, (flag & LexAnalyzeFlag.AllowAssignment) == LexAnalyzeFlag.AllowAssignment)));
					continue;
				}
				break;
			case ')':
				wordCollection.Add(new SymbolWord(')'));
				num--;
				st.ShiftNext();
				continue;
			case ']':
				wordCollection.Add(new SymbolWord(']'));
				num2--;
				st.ShiftNext();
				continue;
			case '(':
				wordCollection.Add(new SymbolWord('('));
				num++;
				st.ShiftNext();
				continue;
			case '[':
				if (st.Next == '[')
				{
					if (ParserMediator.RenameDic == null)
					{
						throw new CodeEE("字句解析中に予期しない文字\"[[\"を発見しました");
					}
					int currentPosition = st.CurrentPosition;
					int num3 = st.Find("]]");
					if (num3 <= 2)
					{
						if (num3 == 2)
						{
							throw new CodeEE("空の[[]]です");
						}
						throw new CodeEE("対応する\"]]\"のない\"[[\"です");
					}
					string text = st.Substring(currentPosition, num3 + 2);
					throw new CodeEE("字句解析中に置換(rename)できない符号" + text + "を発見しました");
				}
				wordCollection.Add(new SymbolWord('['));
				num2++;
				st.ShiftNext();
				continue;
			case ':':
				wordCollection.Add(new SymbolWord(':'));
				st.ShiftNext();
				continue;
			case ',':
				if (endWith != LexEndWith.Comma || num != 0)
				{
					wordCollection.Add(new SymbolWord(','));
					st.ShiftNext();
					continue;
				}
				break;
			case '\'':
				if ((flag & LexAnalyzeFlag.AllowSingleQuotationStr) == LexAnalyzeFlag.AllowSingleQuotationStr)
				{
					st.ShiftNext();
					wordCollection.Add(new LiteralStringWord(ReadString(st, StrEndWith.SingleQuotation)));
					if (st.Current != '\'')
					{
						throw new CodeEE("'が閉じられていません");
					}
					st.ShiftNext();
					continue;
				}
				if ((flag & LexAnalyzeFlag.AnalyzePrintV) != LexAnalyzeFlag.AnalyzePrintV)
				{
					if (endWith != LexEndWith.Operator || num != 0 || num2 != 0 || st.Next != '=')
					{
						throw new CodeEE("字句解析中に予期しない文字'" + st.Current + "'を発見しました");
					}
					break;
				}
				st.ShiftNext();
				wordCollection.Add(new LiteralStringWord(ReadString(st, StrEndWith.Comma)));
				if (st.Current != ',')
				{
					break;
				}
				goto case ',';
			case '}':
				if (endWith != LexEndWith.RightCurlyBrace)
				{
					throw new CodeEE("字句解析中に予期しない文字'" + st.Current + "'を発見しました");
				}
				break;
			case '"':
				st.ShiftNext();
				wordCollection.Add(new LiteralStringWord(ReadString(st, StrEndWith.DoubleQuotation)));
				if (st.Current != '"')
				{
					throw new CodeEE("\"が閉じられていません");
				}
				st.ShiftNext();
				continue;
			case '@':
				if (st.Next != '"')
				{
					wordCollection.Add(new SymbolWord('@'));
					st.ShiftNext();
					continue;
				}
				st.ShiftNext();
				st.ShiftNext();
				wordCollection.Add(AnalyseFormattedString(st, FormStrEndWith.DoubleQuotation, trim: false));
				if (st.Current != '"')
				{
					throw new CodeEE("\"が閉じられていません");
				}
				st.ShiftNext();
				continue;
			case '.':
				wordCollection.Add(new SymbolWord('.'));
				st.ShiftNext();
				continue;
			case '\\':
				if (st.Next != '@')
				{
					throw new CodeEE("字句解析中に予期しない文字'" + st.Current + "'を発見しました");
				}
				st.Jump(2);
				wordCollection.Add(new StrFormWord(new string[2] { "", "" }, new SubWord[1] { AnalyseYenAt(st) }));
				continue;
			case '$':
			case '{':
				throw new CodeEE("字句解析中に予期しない文字'" + st.Current + "'を発見しました");
			case ';':
				if (st.CurrentEqualTo(";#;") && Program.DebugMode)
				{
					st.Jump(3);
					continue;
				}
				if (st.CurrentEqualTo(";!;"))
				{
					st.Jump(3);
					continue;
				}
				st.Seek(0, SeekOrigin.End);
				break;
			default:
				wordCollection.Add(new IdentifierWord(ReadSingleIdentifier(st)));
				continue;
			case '\0':
			case '\n':
				break;
			}
			break;
		}
		if (num != 0 || num2 != 0)
		{
			if (num < 0)
			{
				throw new CodeEE("字句解析中に対応する'('のない')'を発見しました");
			}
			if (num > 0)
			{
				throw new CodeEE("字句解析中に対応する')'のない'('を発見しました");
			}
			if (num2 < 0)
			{
				throw new CodeEE("字句解析中に対応する'['のない']'を発見しました");
			}
			if (num2 > 0)
			{
				throw new CodeEE("字句解析中に対応する']'のない'['を発見しました");
			}
		}
		if (UseMacro)
		{
			return expandMacro(wordCollection);
		}
		return wordCollection;
	}

	private static WordCollection expandMacro(WordCollection wc)
	{
		wc.Pointer = 0;
		int num = 0;
		while (!wc.EOL)
		{
			if (!(wc.Current is IdentifierWord { Code: var code }))
			{
				wc.ShiftNext();
				continue;
			}
			DefineMacro macro = GlobalStatic.IdentifierDictionary.GetMacro(code);
			if (macro == null)
			{
				wc.ShiftNext();
				continue;
			}
			num++;
			if (num > 100)
			{
				throw new CodeEE("マクロの展開数が1文あたりの上限" + 100 + "を超えました(自己参照・循環参照のおそれ)");
			}
			if (!macro.HasArguments)
			{
				wc.Remove();
				wc.InsertRange(macro.Statement);
			}
			else
			{
				wc = expandFunctionlikeMacro(macro, wc);
			}
		}
		wc.Pointer = 0;
		return wc;
	}

	private static WordCollection expandFunctionlikeMacro(DefineMacro macro, WordCollection wc)
	{
		int pointer = wc.Pointer;
		wc.ShiftNext();
		if (!(wc.Current is SymbolWord { Type: '(' }))
		{
			throw new CodeEE("関数形式のマクロ" + macro.Keyword + "に引数がありません");
		}
		WordCollection wordCollection = macro.Statement.Clone();
		WordCollection[] array = new WordCollection[macro.ArgCount];
		for (int i = 0; i < macro.ArgCount; i++)
		{
			int num = 0;
			array[i] = new WordCollection();
			while (true)
			{
				wc.ShiftNext();
				if (wc.EOL)
				{
					throw new CodeEE("関数形式のマクロ" + macro.Keyword + "の用法が正しくありません");
				}
				if (!(wc.Current is SymbolWord symbolWord2))
				{
					array[i].Add(wc.Current);
					continue;
				}
				switch (symbolWord2.Type)
				{
				case '(':
					num++;
					goto default;
				case ')':
					if (num <= 0)
					{
						break;
					}
					num--;
					goto default;
				case ',':
					if (num == 0)
					{
						if (array[i].Collection.Count == 0)
						{
							throw new CodeEE("関数形式のマクロ" + macro.Keyword + "の引数を省略することはできません");
						}
						goto end_IL_006d;
					}
					goto default;
				default:
					array[i].Add(wc.Current);
					continue;
				}
				goto IL_00f3;
				continue;
				end_IL_006d:
				break;
			}
			continue;
			IL_00f3:
			if (i == macro.ArgCount - 1)
			{
				break;
			}
			throw new CodeEE("関数形式のマクロ" + macro.Keyword + "の引数の数が正しくありません");
		}
		if (!(wc.Current is SymbolWord { Type: ')' }))
		{
			throw new CodeEE("関数形式のマクロ" + macro.Keyword + "の用法が正しくありません");
		}
		int num2 = wc.Pointer - pointer + 1;
		wc.Pointer = pointer;
		for (int j = 0; j < num2; j++)
		{
			wc.Collection.RemoveAt(pointer);
		}
		while (!wordCollection.EOL)
		{
			if (!(wordCollection.Current is MacroWord macroWord))
			{
				wordCollection.ShiftNext();
				continue;
			}
			wordCollection.Remove();
			wordCollection.InsertRange(array[macroWord.Number]);
			wordCollection.Pointer += array[macroWord.Number].Collection.Count;
		}
		wc.InsertRange(wordCollection);
		wc.Pointer = pointer;
		return wc;
	}

	public static StrFormWord AnalyseFormattedString(StringStream st, FormStrEndWith endWith, bool trim)
	{
		List<string> list = new List<string>();
		List<SubWord> list2 = new List<SubWord>();
		StringBuilder stringBuilder = new StringBuilder(100);
		while (true)
		{
			char current = st.Current;
			switch (current)
			{
			case '"':
				if (endWith == FormStrEndWith.DoubleQuotation)
				{
					break;
				}
				stringBuilder.Append(current);
				goto IL_02dd;
			case '#':
				if (endWith == FormStrEndWith.Sharp)
				{
					break;
				}
				stringBuilder.Append(current);
				goto IL_02dd;
			case ',':
				if (endWith == FormStrEndWith.Comma || endWith == FormStrEndWith.LeftParenthesis_Bracket_Comma_Semicolon)
				{
					break;
				}
				stringBuilder.Append(current);
				goto IL_02dd;
			case '(':
			case ';':
			case '[':
				if (endWith == FormStrEndWith.LeftParenthesis_Bracket_Comma_Semicolon)
				{
					break;
				}
				stringBuilder.Append(current);
				goto IL_02dd;
			case '%':
				list.Add(stringBuilder.ToString());
				stringBuilder.Remove(0, stringBuilder.Length);
				st.ShiftNext();
				list2.Add(new PercentSubWord(Analyse(st, LexEndWith.Percent, LexAnalyzeFlag.None)));
				if (st.Current != '%')
				{
					throw new CodeEE("'%'が使われましたが対応する'%'が見つかりません");
				}
				goto IL_02dd;
			case '{':
				list.Add(stringBuilder.ToString());
				stringBuilder.Remove(0, stringBuilder.Length);
				st.ShiftNext();
				list2.Add(new CurlyBraceSubWord(Analyse(st, LexEndWith.RightCurlyBrace, LexAnalyzeFlag.None)));
				if (st.Current != '}')
				{
					throw new CodeEE("'{'が使われましたが対応する'}'が見つかりません");
				}
				goto IL_02dd;
			case '$':
			case '*':
			case '+':
			case '/':
			case '=':
				if (!Config.SystemIgnoreTripleSymbol && st.TripleSymbol())
				{
					list.Add(stringBuilder.ToString());
					stringBuilder.Remove(0, stringBuilder.Length);
					st.Jump(3);
					list2.Add(new TripleSymbolSubWord(current));
					continue;
				}
				stringBuilder.Append(current);
				goto IL_02dd;
			case '\\':
				st.ShiftNext();
				current = st.Current;
				if ((uint)current <= 64u)
				{
					if (current == '\0')
					{
						throw new CodeEE("エスケープ文字\\の後に文字がありません");
					}
					if (current != '\n')
					{
						if (current != '@')
						{
							goto IL_02c0;
						}
						if (endWith != FormStrEndWith.YenAt && endWith != FormStrEndWith.Sharp)
						{
							list.Add(stringBuilder.ToString());
							stringBuilder.Remove(0, stringBuilder.Length);
							st.ShiftNext();
							list2.Add(AnalyseYenAt(st));
							continue;
						}
						break;
					}
				}
				else if ((uint)current <= 110u)
				{
					if (current != 'S')
					{
						if (current != 'n')
						{
							goto IL_02c0;
						}
						stringBuilder.Append('\n');
					}
					else
					{
						stringBuilder.Append('\u3000');
					}
				}
				else if (current != 's')
				{
					if (current != 't')
					{
						goto IL_02c0;
					}
					stringBuilder.Append('\t');
				}
				else
				{
					stringBuilder.Append(' ');
				}
				goto IL_02dd;
			default:
				stringBuilder.Append(current);
				goto IL_02dd;
			case '\0':
			case '\n':
				break;
				IL_02dd:
				st.ShiftNext();
				continue;
				IL_02c0:
				stringBuilder.Append(current);
				st.ShiftNext();
				continue;
			}
			break;
		}
		list.Add(stringBuilder.ToString());
		string[] array = new string[list.Count];
		SubWord[] array2 = new SubWord[list2.Count];
		list.CopyTo(array);
		list2.CopyTo(array2);
		if (trim && array.Length != 0)
		{
			array[0] = array[0].TrimStart(' ', '\t');
			array[array.Length - 1] = array[array.Length - 1].TrimEnd(' ', '\t');
		}
		return new StrFormWord(array, array2);
	}

	public static YenAtSubWord AnalyseYenAt(StringStream st)
	{
		WordCollection w = Analyse(st, LexEndWith.Question, LexAnalyzeFlag.None);
		if (st.Current != '?')
		{
			throw new CodeEE("'\\@'が使われましたが対応する'?'が見つかりません");
		}
		st.ShiftNext();
		StrFormWord fsLeft = AnalyseFormattedString(st, FormStrEndWith.Sharp, trim: true);
		if (st.Current != '#')
		{
			if (st.Current != '@')
			{
				throw new CodeEE("'\\@','?'が使われましたが対応する'#'が見つかりません");
			}
			st.ShiftNext();
			ParserMediator.Warn("'\\@','?'が使われましたが対応する'#'が見つかりません", GlobalStatic.Process.GetScaningLine(), 1, isError: false, isBackComp: false);
			return new YenAtSubWord(w, fsLeft, null);
		}
		st.ShiftNext();
		StrFormWord fsRight = AnalyseFormattedString(st, FormStrEndWith.YenAt, trim: true);
		if (st.Current != '@')
		{
			throw new CodeEE("'\\@','?','#'が使われましたが対応する'\\@'が見つかりません");
		}
		st.ShiftNext();
		return new YenAtSubWord(w, fsLeft, fsRight);
	}
}
