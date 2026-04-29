using System.Collections.Generic;
using System.Text;
using MinorShift._Library;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Function;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData;

internal sealed class StrForm
{
	private abstract class FormattedStringMethod : FunctionMethod
	{
		public FormattedStringMethod()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(string);
			argumentTypeArray = null;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			throw new ExeEE("型チェックは呼び出し元が行うこと");
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			throw new ExeEE("戻り値の型が違う");
		}

		public override SingleTerm GetReturnValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return new SingleTerm(GetStrValue(exm, arguments));
		}
	}

	private sealed class FormatCurlyBrace : FormattedStringMethod
	{
		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string text = arguments[0].GetIntValue(exm).ToString();
			if (arguments[1] == null)
			{
				return text;
			}
			if (arguments[2] != null)
			{
				return text.PadRight((int)arguments[1].GetIntValue(exm), ' ');
			}
			return text.PadLeft((int)arguments[1].GetIntValue(exm), ' ');
		}
	}

	private sealed class FormatPercent : FormattedStringMethod
	{
		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			if (arguments[1] == null)
			{
				return strValue;
			}
			int num = (int)arguments[1].GetIntValue(exm);
			int strlenLang = LangManager.GetStrlenLang(strValue);
			num -= strlenLang - strValue.Length;
			if (num < strValue.Length)
			{
				return strValue;
			}
			if (arguments[2] != null)
			{
				return strValue.PadRight(num, ' ');
			}
			return strValue.PadLeft(num, ' ');
		}
	}

	private sealed class FormatYenAt : FormattedStringMethod
	{
		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) == 0L)
			{
				return arguments[2].GetStrValue(exm);
			}
			return arguments[1].GetStrValue(exm);
		}
	}

	private string[] strs;

	private IOperandTerm[] terms;

	private static FormattedStringMethod formatCurlyBrace;

	private static FormattedStringMethod formatPercent;

	private static FormattedStringMethod formatYenAt;

	private static FunctionMethodTerm NameTarget;

	private static FunctionMethodTerm CallnameMaster;

	private static FunctionMethodTerm CallnamePlayer;

	private static FunctionMethodTerm NameAssi;

	private static FunctionMethodTerm CallnameTarget;

	public bool IsConst => strs.Length == 1;

	private StrForm()
	{
	}

	public static void Initialize()
	{
		formatCurlyBrace = new FormatCurlyBrace();
		formatPercent = new FormatPercent();
		formatYenAt = new FormatYenAt();
		VariableToken systemVariableToken = GlobalStatic.VariableData.GetSystemVariableToken("NAME");
		VariableToken systemVariableToken2 = GlobalStatic.VariableData.GetSystemVariableToken("CALLNAME");
		IOperandTerm[] args = new IOperandTerm[1]
		{
			new SingleTerm(0L)
		};
		VariableTerm variableTerm = new VariableTerm(GlobalStatic.VariableData.GetSystemVariableToken("TARGET"), args);
		VariableTerm variableTerm2 = new VariableTerm(GlobalStatic.VariableData.GetSystemVariableToken("MASTER"), args);
		VariableTerm variableTerm3 = new VariableTerm(GlobalStatic.VariableData.GetSystemVariableToken("PLAYER"), args);
		VariableTerm variableTerm4 = new VariableTerm(GlobalStatic.VariableData.GetSystemVariableToken("ASSI"), args);
		VariableTerm variableTerm5 = new VariableTerm(systemVariableToken, new IOperandTerm[1] { variableTerm });
		VariableTerm variableTerm6 = new VariableTerm(systemVariableToken2, new IOperandTerm[1] { variableTerm2 });
		VariableTerm variableTerm7 = new VariableTerm(systemVariableToken2, new IOperandTerm[1] { variableTerm3 });
		VariableTerm variableTerm8 = new VariableTerm(systemVariableToken, new IOperandTerm[1] { variableTerm4 });
		VariableTerm variableTerm9 = new VariableTerm(systemVariableToken2, new IOperandTerm[1] { variableTerm });
		NameTarget = new FunctionMethodTerm(formatPercent, new IOperandTerm[3] { variableTerm5, null, null });
		CallnameMaster = new FunctionMethodTerm(formatPercent, new IOperandTerm[3] { variableTerm6, null, null });
		CallnamePlayer = new FunctionMethodTerm(formatPercent, new IOperandTerm[3] { variableTerm7, null, null });
		NameAssi = new FunctionMethodTerm(formatPercent, new IOperandTerm[3] { variableTerm8, null, null });
		CallnameTarget = new FunctionMethodTerm(formatPercent, new IOperandTerm[3] { variableTerm9, null, null });
	}

	public static StrForm FromWordToken(StrFormWord wt)
	{
		StrForm strForm = new StrForm();
		strForm.strs = wt.Strs;
		IOperandTerm[] array = new IOperandTerm[wt.SubWords.Length];
		for (int i = 0; i < wt.SubWords.Length; i++)
		{
			SubWord subWord = wt.SubWords[i];
			if (subWord is TripleSymbolSubWord { Code: var code })
			{
				switch (code)
				{
				case '*':
					array[i] = NameTarget;
					break;
				case '+':
					array[i] = CallnameMaster;
					break;
				case '=':
					array[i] = CallnamePlayer;
					break;
				case '/':
					array[i] = NameAssi;
					break;
				case '$':
					array[i] = CallnameTarget;
					break;
				default:
					throw new ExeEE("何かおかしい");
				}
				continue;
			}
			WordCollection wordCollection = null;
			IOperandTerm operandTerm = null;
			if (subWord is YenAtSubWord { Words: var words } yenAtSubWord)
			{
				if (words != null)
				{
					operandTerm = ExpressionParser.ReduceIntegerTerm(words, TermEndWith.EoL);
					if (!words.EOL)
					{
						throw new CodeEE("三項演算子\\@の第一オペランドが異常です");
					}
				}
				else
				{
					operandTerm = new SingleTerm(0L);
				}
				IOperandTerm operandTerm2 = new StrFormTerm(FromWordToken(yenAtSubWord.Left));
				IOperandTerm operandTerm3 = null;
				operandTerm3 = ((yenAtSubWord.Right != null) ? ((IOperandTerm)new StrFormTerm(FromWordToken(yenAtSubWord.Right))) : ((IOperandTerm)new SingleTerm("")));
				array[i] = new FunctionMethodTerm(formatYenAt, new IOperandTerm[3] { operandTerm, operandTerm2, operandTerm3 });
				continue;
			}
			wordCollection = subWord.Words;
			operandTerm = ExpressionParser.ReduceExpressionTerm(wordCollection, TermEndWith.Comma);
			if (operandTerm == null)
			{
				if (subWord is CurlyBraceSubWord)
				{
					throw new CodeEE("{}の中に式が存在しません");
				}
				throw new CodeEE("%%の中に式が存在しません");
			}
			IOperandTerm operandTerm4 = null;
			SingleTerm singleTerm = null;
			wordCollection.ShiftNext();
			if (!wordCollection.EOL)
			{
				operandTerm4 = ExpressionParser.ReduceIntegerTerm(wordCollection, TermEndWith.Comma);
				wordCollection.ShiftNext();
				if (!wordCollection.EOL)
				{
					if (!(wordCollection.Current is IdentifierWord identifierWord))
					{
						throw new CodeEE("','の後にRIGHT又はLEFTがありません");
					}
					if (string.Equals(identifierWord.Code, "LEFT", Config.SCVariable))
					{
						singleTerm = new SingleTerm(1L);
					}
					else if (!string.Equals(identifierWord.Code, "RIGHT", Config.SCVariable))
					{
						throw new CodeEE("','の後にRIGHT又はLEFT以外の単語があります");
					}
					wordCollection.ShiftNext();
				}
				if (!wordCollection.EOL)
				{
					throw new CodeEE("RIGHT又はLEFTの後に余分な文字があります");
				}
			}
			if (subWord is CurlyBraceSubWord)
			{
				if (operandTerm.GetOperandType() != typeof(long))
				{
					throw new CodeEE("{}の中の式が数式ではありません");
				}
				array[i] = new FunctionMethodTerm(formatCurlyBrace, new IOperandTerm[3] { operandTerm, operandTerm4, singleTerm });
			}
			else
			{
				if (operandTerm.GetOperandType() != typeof(string))
				{
					throw new CodeEE("%%の中の式が文字列式ではありません");
				}
				array[i] = new FunctionMethodTerm(formatPercent, new IOperandTerm[3] { operandTerm, operandTerm4, singleTerm });
			}
		}
		strForm.terms = array;
		return strForm;
	}

	public IOperandTerm GetIOperandTerm()
	{
		if (strs.Length == 2 && strs[0].Length == 0 && strs[1].Length == 0)
		{
			return terms[0];
		}
		return null;
	}

	public void Restructure(ExpressionMediator exm)
	{
		if (strs.Length == 1)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < terms.Length; i++)
		{
			terms[i] = terms[i].Restructure(exm);
			if (terms[i] is SingleTerm)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		List<string> list = new List<string>();
		List<IOperandTerm> list2 = new List<IOperandTerm>();
		list.AddRange(strs);
		list2.AddRange(terms);
		for (int j = 0; j < list2.Count; j++)
		{
			if (list2[j] is SingleTerm)
			{
				string strValue = list2[j].GetStrValue(exm);
				list[j] = list[j] + strValue + list[j + 1];
				list2.RemoveAt(j);
				list.RemoveAt(j + 1);
				j--;
			}
		}
		strs = new string[list.Count];
		terms = new IOperandTerm[list2.Count];
		list.CopyTo(strs);
		list2.CopyTo(terms);
	}

	public string GetString(ExpressionMediator exm)
	{
		if (strs.Length == 1)
		{
			return strs[0];
		}
		StringBuilder stringBuilder = new StringBuilder(100);
		for (int i = 0; i < strs.Length - 1; i++)
		{
			stringBuilder.Append(strs[i]);
			stringBuilder.Append(terms[i].GetStrValue(exm));
		}
		stringBuilder.Append(strs[strs.Length - 1]);
		return stringBuilder.ToString();
	}
}
