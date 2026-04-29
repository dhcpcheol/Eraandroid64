using System;
using System.Collections.Generic;
using System.Drawing;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Function;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc.Function;

internal static class ArgumentParser
{
	private sealed class SP_PRINTV_ArgumentBuilder : ArgumentBuilder
	{
		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = ExpressionParser.ReduceArguments(LexicalAnalyzer.Analyse(line.PopArgumentPrimitive(), LexEndWith.EoL, LexAnalyzeFlag.AnalyzePrintV), ArgsEndWith.EoL, isDefine: false);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					warn("引数を省略することはできません", line, 2, isBackComp: false);
					return null;
				}
				array[i] = array[i].Restructure(exm);
			}
			return new SpPrintVArgument(array);
		}
	}

	private sealed class SP_TIMES_ArgumentBuilder : ArgumentBuilder
	{
		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			StringStream stringStream = line.PopArgumentPrimitive();
			WordCollection wc = LexicalAnalyzer.Analyse(stringStream, LexEndWith.Comma, LexAnalyzeFlag.None);
			stringStream.ShiftNext();
			if (stringStream.EOS)
			{
				warn("引数が足りません", line, 2, isBackComp: false);
				return null;
			}
			double num = 0.0;
			try
			{
				LexicalAnalyzer.SkipWhiteSpace(stringStream);
				num = LexicalAnalyzer.ReadDouble(stringStream);
				LexicalAnalyzer.SkipWhiteSpace(stringStream);
				if (!stringStream.EOS)
				{
					warn("引数が多すぎます", line, 1, isBackComp: false);
				}
			}
			catch
			{
				warn("第２引数が実数値ではありません（常に0と解釈されます）", line, 1, isBackComp: false);
				num = 0.0;
			}
			IOperandTerm operandTerm = ExpressionParser.ReduceExpressionTerm(wc, TermEndWith.EoL);
			if (operandTerm == null)
			{
				warn("書式が間違っています", line, 2, isBackComp: false);
				return null;
			}
			if (!(operandTerm.Restructure(exm) is VariableTerm variableTerm))
			{
				warn("第１引数に変数以外を指定することはできません", line, 2, isBackComp: false);
				return null;
			}
			if (variableTerm.IsString)
			{
				warn("第１引数を文字列変数にすることはできません", line, 2, isBackComp: false);
				return null;
			}
			if (variableTerm.Identifier.IsConst)
			{
				warn("第１引数に変更できない変数を指定することはできません", line, 2, isBackComp: false);
				return null;
			}
			return new SpTimesArgument(variableTerm, num);
		}
	}

	private sealed class FORM_STR_ANY_ArgumentBuilder : ArgumentBuilder
	{
		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			StringStream stringStream = line.PopArgumentPrimitive();
			List<IOperandTerm> list = new List<IOperandTerm>();
			LexicalAnalyzer.SkipHalfSpace(stringStream);
			if (stringStream.EOS)
			{
				if (line.FunctionCode == FunctionCode.RETURNFORM)
				{
					list.Add(new SingleTerm("0"));
					return new ExpressionArrayArgument(list)
					{
						IsConst = true,
						ConstInt = 0L
					};
				}
				warn("引数が設定されていません", line, 2, isBackComp: false);
				return null;
			}
			while (true)
			{
				IOperandTerm operandTerm = ExpressionParser.ToStrFormTerm(LexicalAnalyzer.AnalyseFormattedString(stringStream, FormStrEndWith.Comma, trim: false));
				operandTerm = operandTerm.Restructure(exm);
				list.Add(operandTerm);
				stringStream.ShiftNext();
				if (stringStream.EOS)
				{
					break;
				}
				LexicalAnalyzer.SkipHalfSpace(stringStream);
				if (stringStream.EOS)
				{
					warn("','の後ろに引数がありません。", line, 1, isBackComp: false);
					break;
				}
			}
			return new ExpressionArrayArgument(list);
		}
	}

	private sealed class VOID_ArgumentBuilder : ArgumentBuilder
	{
		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			StringStream stringStream = line.PopArgumentPrimitive();
			LexicalAnalyzer.SkipWhiteSpace(stringStream);
			if (!stringStream.EOS)
			{
				warn("引数は不要です", line, 1, isBackComp: false);
			}
			return new VoidArgument();
		}
	}

	private sealed class STR_ArgumentBuilder : ArgumentBuilder
	{
		private bool nullable;

		public STR_ArgumentBuilder(bool nullable)
		{
			this.nullable = nullable;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			StringStream stringStream = line.PopArgumentPrimitive();
			string text = null;
			if (stringStream.EOS)
			{
				if (!nullable)
				{
					warn("引数が設定されていません", line, 2, isBackComp: false);
					return null;
				}
				text = "";
			}
			else
			{
				text = stringStream.Substring();
			}
			if ((line.FunctionCode == FunctionCode.SETCOLORBYNAME || line.FunctionCode == FunctionCode.SETBGCOLORBYNAME) && Color.FromName(text).A == 0)
			{
				if (text.Equals("transparent", StringComparison.OrdinalIgnoreCase))
				{
					throw new CodeEE("無色透明(Transparent)は色として指定できません");
				}
				throw new CodeEE("指定された色名\"" + text + "\"は無効な色名です");
			}
			return new ExpressionArgument(new SingleTerm(text))
			{
				ConstStr = text,
				IsConst = true
			};
		}
	}

	private sealed class FORM_STR_ArgumentBuilder : ArgumentBuilder
	{
		private bool nullable;

		public FORM_STR_ArgumentBuilder(bool nullable)
		{
			this.nullable = nullable;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			StringStream stringStream = line.PopArgumentPrimitive();
			Argument argument = null;
			if (stringStream.EOS)
			{
				if (!nullable)
				{
					warn("引数が設定されていません", line, 2, isBackComp: false);
					return null;
				}
				argument = new ExpressionArgument(new SingleTerm(""));
				argument.ConstStr = "";
				argument.IsConst = true;
				return argument;
			}
			IOperandTerm operandTerm = ExpressionParser.ToStrFormTerm(LexicalAnalyzer.AnalyseFormattedString(stringStream, FormStrEndWith.EoL, trim: false));
			operandTerm = operandTerm.Restructure(exm);
			argument = new ExpressionArgument(operandTerm);
			if (operandTerm is SingleTerm)
			{
				argument.ConstStr = operandTerm.GetStrValue(exm);
				argument.IsConst = true;
			}
			return argument;
		}
	}

	private sealed class SP_VAR_ArgumentBuilder : ArgumentBuilder
	{
		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			StringStream stringStream = line.PopArgumentPrimitive();
			IdentifierWord identifierWord = LexicalAnalyzer.ReadSingleIdentifierWord(stringStream);
			if (identifierWord == null)
			{
				warn("第１引数を読み取ることができません", line, 2, isBackComp: false);
				return null;
			}
			string code = identifierWord.Code;
			VariableToken variableToken = GlobalStatic.IdentifierDictionary.GetVariableToken(code, null, allowPrivate: true);
			if (variableToken == null)
			{
				warn("第１引数に変数以外を指定することはできません", line, 2, isBackComp: false);
				return null;
			}
			if ((!variableToken.IsArray1D && !variableToken.IsArray2D && !variableToken.IsArray3D) || variableToken.Code == VariableCode.RAND)
			{
				warn("第１引数に配列でない変数を指定することはできません", line, 2, isBackComp: false);
				return null;
			}
			LexicalAnalyzer.SkipWhiteSpace(stringStream);
			if (!stringStream.EOS)
			{
				warn("引数の後に余分な文字があります", line, 1, isBackComp: false);
			}
			return new SpVarsizeArgument(variableToken);
		}
	}

	private sealed class SP_SORTCHARA_ArgumentBuilder : ArgumentBuilder
	{
		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			VariableTerm variableTerm = new VariableTerm(GlobalStatic.VariableData.GetSystemVariableToken("NO"), new IOperandTerm[1]
			{
				new SingleTerm(0L)
			});
			SortOrder order = SortOrder.ASCENDING;
			WordCollection wordCollection = popWords(line);
			IdentifierWord identifierWord = wordCollection.Current as IdentifierWord;
			if (wordCollection.EOL)
			{
				return new SpSortcharaArgument(variableTerm, order);
			}
			if (identifierWord != null && (identifierWord.Code.Equals("FORWARD", Config.SCVariable) || identifierWord.Code.Equals("BACK", Config.SCVariable)))
			{
				if (identifierWord.Code.Equals("BACK", Config.SCVariable))
				{
					order = SortOrder.DESENDING;
				}
				wordCollection.ShiftNext();
				if (!wordCollection.EOL)
				{
					warn("引数が多すぎます", line, 1, isBackComp: false);
				}
			}
			else
			{
				IOperandTerm operandTerm = ExpressionParser.ReduceExpressionTerm(wordCollection, TermEndWith.Comma);
				if (operandTerm == null)
				{
					warn("書式が間違っています", line, 2, isBackComp: false);
					return null;
				}
				variableTerm = operandTerm.Restructure(exm) as VariableTerm;
				if (variableTerm == null)
				{
					warn("第１引数に変数以外を指定することはできません", line, 2, isBackComp: false);
					return null;
				}
				if (!variableTerm.Identifier.IsCharacterData)
				{
					warn("第１引数はキャラクタ変数でなければなりません", line, 2, isBackComp: false);
					return null;
				}
				wordCollection.ShiftNext();
				if (!wordCollection.EOL)
				{
					if (!(wordCollection.Current is IdentifierWord identifierWord2) || (!identifierWord2.Code.Equals("FORWARD", Config.SCVariable) && !identifierWord2.Code.Equals("BACK", Config.SCVariable)))
					{
						warn("書式が間違っています", line, 2, isBackComp: false);
						return null;
					}
					if (identifierWord2.Code.Equals("BACK", Config.SCVariable))
					{
						order = SortOrder.DESENDING;
					}
					wordCollection.ShiftNext();
					if (!wordCollection.EOL)
					{
						warn("引数が多すぎます", line, 1, isBackComp: false);
					}
				}
			}
			return new SpSortcharaArgument(variableTerm, order);
		}
	}

	private sealed class SP_SORT_ARRAY_ArgumentBuilder : ArgumentBuilder
	{
		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			SortOrder order = SortOrder.ASCENDING;
			WordCollection wordCollection = popWords(line);
			IOperandTerm operandTerm = new SingleTerm(0L);
			IOperandTerm operandTerm2 = null;
			if (wordCollection.EOL)
			{
				warn("書式が間違っています", line, 2, isBackComp: false);
				return null;
			}
			IOperandTerm operandTerm3 = ExpressionParser.ReduceExpressionTerm(wordCollection, TermEndWith.Comma);
			if (operandTerm3 == null)
			{
				warn("書式が間違っています", line, 2, isBackComp: false);
				return null;
			}
			if (!(operandTerm3.Restructure(exm) is VariableTerm variableTerm))
			{
				warn("第１引数に変数以外を指定することはできません", line, 2, isBackComp: false);
				return null;
			}
			if (variableTerm.Identifier.IsConst)
			{
				warn("第１引数が変更できない変数です", line, 2, isBackComp: false);
				return null;
			}
			if (!variableTerm.Identifier.IsArray1D)
			{
				warn("第１引数に１次元配列もしくは配列型キャラクタ変数以外を指定することはできません", line, 2, isBackComp: false);
				return null;
			}
			wordCollection.ShiftNext();
			IdentifierWord identifierWord = wordCollection.Current as IdentifierWord;
			if (identifierWord != null && (identifierWord.Code.Equals("FORWARD", Config.SCVariable) || identifierWord.Code.Equals("BACK", Config.SCVariable)))
			{
				if (identifierWord.Code.Equals("BACK", Config.SCVariable))
				{
					order = SortOrder.DESENDING;
				}
				wordCollection.ShiftNext();
			}
			else if (identifierWord != null)
			{
				warn("第２引数にソート方法指定子（FORWARD or BACK）以外が指定されています", line, 2, isBackComp: false);
				return null;
			}
			if (identifierWord != null)
			{
				wordCollection.ShiftNext();
				if (!wordCollection.EOL)
				{
					operandTerm = ExpressionParser.ReduceExpressionTerm(wordCollection, TermEndWith.Comma);
					if (operandTerm == null)
					{
						warn("第３引数が解釈出来ません", line, 2, isBackComp: false);
						return null;
					}
					if (!operandTerm.IsInteger)
					{
						warn("第３引数が数値ではありません", line, 2, isBackComp: false);
						return null;
					}
					wordCollection.ShiftNext();
					if (!wordCollection.EOL)
					{
						operandTerm2 = ExpressionParser.ReduceExpressionTerm(wordCollection, TermEndWith.Comma);
						if (operandTerm2 == null)
						{
							warn("第４引数が解釈出来ません", line, 2, isBackComp: false);
							return null;
						}
						if (!operandTerm2.IsInteger)
						{
							warn("第４引数が数値ではありません", line, 2, isBackComp: false);
							return null;
						}
						wordCollection.ShiftNext();
						if (!wordCollection.EOL)
						{
							warn("引数が多すぎます", line, 1, isBackComp: false);
						}
					}
				}
			}
			return new SpArraySortArgument(variableTerm, order, operandTerm, operandTerm2);
		}
	}

	private sealed class SP_CALL_ArgumentBuilder : ArgumentBuilder
	{
		private bool form;

		private bool callf;

		public SP_CALL_ArgumentBuilder(bool callf, bool form)
		{
			this.form = form;
			this.callf = callf;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			StringStream stringStream = line.PopArgumentPrimitive();
			IOperandTerm operandTerm = null;
			if (form)
			{
				operandTerm = ExpressionParser.ToStrFormTerm(LexicalAnalyzer.AnalyseFormattedString(stringStream, FormStrEndWith.LeftParenthesis_Bracket_Comma_Semicolon, trim: true));
				operandTerm = operandTerm.Restructure(exm);
			}
			else
			{
				operandTerm = new SingleTerm(LexicalAnalyzer.ReadString(stringStream, StrEndWith.LeftParenthesis_Bracket_Comma_Semicolon).Trim(' ', '\t'));
			}
			char current = stringStream.Current;
			WordCollection wordCollection = LexicalAnalyzer.Analyse(stringStream, LexEndWith.EoL, LexAnalyzeFlag.None);
			wordCollection.ShiftNext();
			IOperandTerm[] array = null;
			IOperandTerm[] array2 = null;
			if (current == '[')
			{
				array = ExpressionParser.ReduceArguments(wordCollection, ArgsEndWith.RightBracket, isDefine: false);
				if (!wordCollection.EOL)
				{
					if (wordCollection.Current.Type != '(')
					{
						wordCollection.ShiftNext();
					}
					array2 = ExpressionParser.ReduceArguments(wordCollection, ArgsEndWith.RightParenthesis, isDefine: false);
				}
			}
			if (current == '(' || current == ',')
			{
				array2 = ((current != '(') ? ExpressionParser.ReduceArguments(wordCollection, ArgsEndWith.EoL, isDefine: false) : ExpressionParser.ReduceArguments(wordCollection, ArgsEndWith.RightParenthesis, isDefine: false));
				if (!wordCollection.EOL)
				{
					warn("書式が間違っています", line, 2, isBackComp: false);
					return null;
				}
			}
			if (array == null)
			{
				array = new IOperandTerm[0];
			}
			if (array2 == null)
			{
				array2 = new IOperandTerm[0];
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array != null)
				{
					array[i] = array[i].Restructure(exm);
				}
			}
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j] != null)
				{
					array2[j] = array2[j].Restructure(exm);
				}
			}
			Argument argument = null;
			argument = ((!callf) ? ((Argument)new SpCallArgment(operandTerm, array, array2)) : ((Argument)new SpCallFArgment(operandTerm, array, array2)));
			if (operandTerm is SingleTerm)
			{
				argument.IsConst = true;
				argument.ConstStr = operandTerm.GetStrValue(null);
				if (argument.ConstStr == "")
				{
					warn("関数名が指定されていません", line, 2, isBackComp: false);
					return null;
				}
			}
			return argument;
		}
	}

	private sealed class CASE_ArgumentBuilder : ArgumentBuilder
	{
		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			WordCollection wordCollection = popWords(line);
			CaseExpression[] array = ExpressionParser.ReduceCaseExpressions(wordCollection);
			if (!wordCollection.EOL || array.Length == 0)
			{
				warn("書式が間違っています", line, 2, isBackComp: false);
				return null;
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Reduce(exm);
			}
			return new CaseArgument(array);
		}
	}

	private sealed class SP_SET_ArgumentBuilder : ArgumentBuilder
	{
		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = ExpressionParser.ReduceArguments(line.PopAssignmentDestStr(), ArgsEndWith.EoL, isDefine: false);
			SpSetArgument spSetArgument = null;
			if (array.Length == 0 || array[0] == null)
			{
				assignwarn("代入文の左辺の読み取りに失敗しました", line, 2, isBackComp: false);
				return null;
			}
			if (array.Length != 1)
			{
				assignwarn("代入文の左辺に余分な','があります", line, 2, isBackComp: false);
				return null;
			}
			if (!(array[0] is VariableTerm variableTerm))
			{
				assignwarn("代入文の左辺に変数以外を指定することはできません", line, 2, isBackComp: false);
				return null;
			}
			if (variableTerm.Identifier.IsConst)
			{
				assignwarn("代入文の左辺に変更できない変数を指定することはできません", line, 2, isBackComp: false);
				return null;
			}
			variableTerm.Restructure(exm);
			StringStream stringStream = line.PopArgumentPrimitive();
			if (stringStream == null)
			{
				stringStream = new StringStream("");
			}
			OperatorCode assignOperator = line.AssignOperator;
			IOperandTerm operandTerm = null;
			if (variableTerm.IsInteger)
			{
				switch (assignOperator)
				{
				case OperatorCode.AssignmentStr:
					assignwarn("整数型の代入に演算子" + OperatorManager.ToOperatorString(assignOperator) + "は使用できません", line, 2, isBackComp: false);
					return null;
				case OperatorCode.Increment:
				case OperatorCode.Decrement:
					LexicalAnalyzer.SkipWhiteSpace(stringStream);
					if (!stringStream.EOS)
					{
						if (assignOperator == OperatorCode.Increment)
						{
							assignwarn("インクリメント行でインクリメント以外の処理が定義されています", line, 2, isBackComp: false);
							return null;
						}
						assignwarn("デクリメント行でデクリメント以外の処理が定義されています", line, 2, isBackComp: false);
						return null;
					}
					spSetArgument = new SpSetArgument(variableTerm, null);
					spSetArgument.IsConst = true;
					if (assignOperator == OperatorCode.Increment)
					{
						spSetArgument.ConstInt = 1L;
					}
					else
					{
						spSetArgument.ConstInt = -1L;
					}
					spSetArgument.AddConst = true;
					return spSetArgument;
				default:
				{
					IOperandTerm[] array2 = ExpressionParser.ReduceArguments(LexicalAnalyzer.Analyse(stringStream, LexEndWith.EoL, LexAnalyzeFlag.None), ArgsEndWith.EoL, isDefine: false);
					if (array2.Length == 0 || array2[0] == null)
					{
						assignwarn("代入文の右辺の読み取りに失敗しました", line, 2, isBackComp: false);
						return null;
					}
					if (array2.Length != 1)
					{
						if (assignOperator != OperatorCode.Assignment)
						{
							assignwarn("複合代入演算では右辺に複数の値を含めることはできません", line, 2, isBackComp: false);
							return null;
						}
						bool flag = true;
						long[] array3 = new long[array2.Length];
						for (int i = 0; i < array2.Length; i++)
						{
							if (array2[i] == null)
							{
								assignwarn("代入式の右辺の値は省略できません", line, 2, isBackComp: false);
								return null;
							}
							if (!array2[i].IsInteger)
							{
								assignwarn("数値型変数に文字列は代入できません", line, 2, isBackComp: false);
								return null;
							}
							array2[i] = array2[i].Restructure(exm);
							if (flag && array2[i] is SingleTerm)
							{
								array3[i] = array2[i].GetIntValue(null);
							}
							else
							{
								flag = false;
							}
						}
						return new SpSetArrayArgument(variableTerm, array2, array3)
						{
							IsConst = flag
						};
					}
					if (!array2[0].IsInteger)
					{
						assignwarn("数値型変数に文字列は代入できません", line, 2, isBackComp: false);
						return null;
					}
					operandTerm = array2[0].Restructure(exm);
					switch (assignOperator)
					{
					case OperatorCode.Assignment:
						spSetArgument = new SpSetArgument(variableTerm, operandTerm);
						if (operandTerm is SingleTerm)
						{
							spSetArgument.IsConst = true;
							spSetArgument.AddConst = false;
							spSetArgument.ConstInt = operandTerm.GetIntValue(null);
						}
						return spSetArgument;
					case OperatorCode.Plus:
					case OperatorCode.Minus:
						if (operandTerm is SingleTerm)
						{
							spSetArgument = new SpSetArgument(variableTerm, null);
							spSetArgument.IsConst = true;
							spSetArgument.AddConst = true;
							if (assignOperator == OperatorCode.Plus)
							{
								spSetArgument.ConstInt = operandTerm.GetIntValue(null);
							}
							else
							{
								spSetArgument.ConstInt = -operandTerm.GetIntValue(null);
							}
							return spSetArgument;
						}
						break;
					}
					operandTerm = OperatorMethodManager.ReduceBinaryTerm(assignOperator, variableTerm, operandTerm);
					return new SpSetArgument(variableTerm, operandTerm);
				}
				}
			}
			switch (assignOperator)
			{
			case OperatorCode.Assignment:
				LexicalAnalyzer.SkipHalfSpace(stringStream);
				operandTerm = ExpressionParser.ToStrFormTerm(LexicalAnalyzer.AnalyseFormattedString(stringStream, FormStrEndWith.EoL, trim: true)).Restructure(exm);
				spSetArgument = new SpSetArgument(variableTerm, operandTerm);
				if (operandTerm is SingleTerm)
				{
					spSetArgument.IsConst = true;
					spSetArgument.AddConst = false;
					spSetArgument.ConstStr = operandTerm.GetStrValue(null);
				}
				return spSetArgument;
			case OperatorCode.AssignmentStr:
			case OperatorCode.Mult:
			case OperatorCode.Plus:
			{
				IOperandTerm[] array4 = ExpressionParser.ReduceArguments(LexicalAnalyzer.Analyse(stringStream, LexEndWith.EoL, LexAnalyzeFlag.None), ArgsEndWith.EoL, isDefine: false);
				if (array4.Length == 0 || array4[0] == null)
				{
					assignwarn("代入文の右辺の読み取りに失敗しました", line, 2, isBackComp: false);
					return null;
				}
				if (assignOperator == OperatorCode.AssignmentStr)
				{
					if (array4.Length == 1)
					{
						if (array4[0].IsInteger)
						{
							assignwarn("文字列変数に数値型は代入できません", line, 2, isBackComp: false);
							return null;
						}
						operandTerm = array4[0].Restructure(exm);
						spSetArgument = new SpSetArgument(variableTerm, operandTerm);
						if (operandTerm is SingleTerm)
						{
							spSetArgument.IsConst = true;
							spSetArgument.AddConst = false;
							spSetArgument.ConstStr = operandTerm.GetStrValue(null);
						}
						return spSetArgument;
					}
					bool flag2 = true;
					string[] array5 = new string[array4.Length];
					for (int j = 0; j < array4.Length; j++)
					{
						if (array4[j] == null)
						{
							assignwarn("代入式の右辺の値は省略できません", line, 2, isBackComp: false);
							return null;
						}
						if (array4[j].IsInteger)
						{
							assignwarn("文字列変数に数値型は代入できません", line, 2, isBackComp: false);
							return null;
						}
						array4[j] = array4[j].Restructure(exm);
						if (flag2 && array4[j] is SingleTerm)
						{
							array5[j] = array4[j].GetStrValue(null);
						}
						else
						{
							flag2 = false;
						}
					}
					return new SpSetArrayArgument(variableTerm, array4, array5)
					{
						IsConst = flag2
					};
				}
				if (array4.Length != 1)
				{
					assignwarn("代入文の右辺に余分な','があります", line, 2, isBackComp: false);
					return null;
				}
				operandTerm = array4[0].Restructure(exm);
				operandTerm = OperatorMethodManager.ReduceBinaryTerm(assignOperator, variableTerm, operandTerm);
				return new SpSetArgument(variableTerm, operandTerm);
			}
			default:
				assignwarn("代入式に使用できない演算子が使われました", line, 2, isBackComp: false);
				return null;
			}
		}
	}

	private sealed class METHOD_ArgumentBuilder : ArgumentBuilder
	{
		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			string text = line.Function.Method.CheckArgumentType(line.Function.Name, array);
			if (text != null)
			{
				throw new CodeEE(text);
			}
			return new MethodArgument(new FunctionMethodTerm(line.Function.Method, array).Restructure(exm));
		}
	}

	private sealed class SP_INPUTS_ArgumentBuilder : ArgumentBuilder
	{
		public SP_INPUTS_ArgumentBuilder()
		{
			argumentTypeArray = new Type[1] { typeof(string) };
			minArg = 0;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			StringStream stringStream = line.PopArgumentPrimitive();
			Argument argument = null;
			if (stringStream.EOS)
			{
				return new ExpressionArgument(null);
			}
			StrFormWord sfw = LexicalAnalyzer.AnalyseFormattedString(stringStream, FormStrEndWith.EoL, trim: false);
			if (!stringStream.EOS)
			{
				warn("引数が多すぎます", line, 1, isBackComp: false);
			}
			IOperandTerm operandTerm = ExpressionParser.ToStrFormTerm(sfw);
			operandTerm = operandTerm.Restructure(exm);
			argument = new ExpressionArgument(operandTerm);
			if (operandTerm is SingleTerm)
			{
				argument.ConstStr = operandTerm.GetStrValue(exm);
				if (line.FunctionCode == FunctionCode.ONEINPUTS)
				{
					if (string.IsNullOrEmpty(argument.ConstStr))
					{
						warn("引数が空文字列なため、引数は無視されます", line, 1, isBackComp: false);
						return new ExpressionArgument(null);
					}
					if (argument.ConstStr.Length > 1)
					{
						warn("ONEINPUTSの引数に２文字以上の文字列が渡されています（２文字目以降は無視されます）", line, 1, isBackComp: false);
						argument.ConstStr = argument.ConstStr.Remove(1);
					}
				}
				argument.IsConst = true;
			}
			return argument;
		}
	}

	private sealed class INT_EXPRESSION_ArgumentBuilder : ArgumentBuilder
	{
		private bool nullable;

		public INT_EXPRESSION_ArgumentBuilder(bool nullable)
		{
			argumentTypeArray = new Type[1] { typeof(long) };
			minArg = 0;
			this.nullable = nullable;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			IOperandTerm operandTerm = null;
			if (array.Length == 0)
			{
				operandTerm = new SingleTerm(0L);
				if (!nullable)
				{
					if (line.Function.IsExtended())
					{
						warn("省略できない引数が省略されています。Emueraは0を補います", line, 1, isBackComp: false);
					}
					else
					{
						warn("省略できない引数が省略されています。Emueraは0を補いますがeramakerの動作は不定です", line, 1, isBackComp: false);
					}
				}
			}
			else
			{
				operandTerm = array[0];
			}
			if (line.FunctionCode == FunctionCode.REPEAT)
			{
				if (operandTerm is SingleTerm && operandTerm.GetIntValue(null) <= 0)
				{
					warn("0回以下のREPEATです。(eramakerではエラーになります)", line, 0, isBackComp: true);
				}
				VariableTerm variableTerm = new VariableTerm(GlobalStatic.VariableData.GetSystemVariableToken("COUNT"), new IOperandTerm[1]
				{
					new SingleTerm(0L)
				});
				variableTerm.Restructure(exm);
				return new SpForNextArgment(variableTerm, new SingleTerm(0L), operandTerm, new SingleTerm(1L));
			}
			ExpressionArgument expressionArgument = new ExpressionArgument(operandTerm);
			if (operandTerm is SingleTerm)
			{
				long num = (expressionArgument.ConstInt = operandTerm.GetIntValue(null));
				expressionArgument.IsConst = true;
				if (line.FunctionCode == FunctionCode.CLEARLINE)
				{
					if (num <= 0)
					{
						warn("引数に0以下の値が渡されています(この行は何もしません)", line, 1, isBackComp: false);
					}
				}
				else if (line.FunctionCode == FunctionCode.FONTSTYLE && num < 0)
				{
					warn("引数に負の値が渡されています(結果は不定です)", line, 1, isBackComp: false);
				}
			}
			return expressionArgument;
		}
	}

	private sealed class INT_ANY_ArgumentBuilder : ArgumentBuilder
	{
		public INT_ANY_ArgumentBuilder()
		{
			argumentTypeArray = new Type[1] { typeof(long) };
			minArg = 0;
			argAny = true;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			List<IOperandTerm> list = new List<IOperandTerm>();
			list.AddRange(array);
			ExpressionArrayArgument expressionArrayArgument = new ExpressionArrayArgument(list);
			if (array.Length == 0)
			{
				if (line.FunctionCode == FunctionCode.RETURN)
				{
					list.Add(new SingleTerm(0L));
					expressionArrayArgument.IsConst = true;
					expressionArrayArgument.ConstInt = 0L;
					return expressionArrayArgument;
				}
				warn("引数が設定されていません", line, 2, isBackComp: false);
				return null;
			}
			if (array.Length == 1)
			{
				if (array[0] is SingleTerm singleTerm)
				{
					expressionArrayArgument.IsConst = true;
					expressionArrayArgument.ConstInt = singleTerm.Int;
					return expressionArrayArgument;
				}
				if (line.FunctionCode == FunctionCode.RETURN)
				{
					if (array[0] is VariableTerm)
					{
						warn("RETURNの引数に変数が渡されています(eramaker：常に0を返します)", line, 0, isBackComp: true);
					}
					else
					{
						warn("RETURNの引数に数式が渡されています(eramaker：Emueraとは異なる値を返します)", line, 0, isBackComp: true);
					}
				}
			}
			else
			{
				warn(line.Function.Name + "の引数に複数の値が与えられています(eramaker：非対応です)", line, 0, isBackComp: true);
			}
			return expressionArrayArgument;
		}
	}

	private sealed class STR_EXPRESSION_ArgumentBuilder : ArgumentBuilder
	{
		public STR_EXPRESSION_ArgumentBuilder(bool nullable)
		{
			argumentTypeArray = new Type[1] { typeof(string) };
			if (nullable)
			{
				minArg = 0;
			}
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			if (array.Length == 0)
			{
				return new ExpressionArgument(new SingleTerm(""))
				{
					ConstStr = "",
					IsConst = true
				};
			}
			return new ExpressionArgument(array[0]);
		}
	}

	private sealed class EXPRESSION_ArgumentBuilder : ArgumentBuilder
	{
		public EXPRESSION_ArgumentBuilder(bool nullable)
		{
			argumentTypeArray = new Type[1] { typeof(void) };
			if (nullable)
			{
				minArg = 0;
			}
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			if (array.Length == 0)
			{
				return new ExpressionArgument(null)
				{
					ConstStr = "",
					ConstInt = 0L,
					IsConst = true
				};
			}
			return new ExpressionArgument(array[0]);
		}
	}

	private sealed class SP_BAR_ArgumentBuilder : ArgumentBuilder
	{
		public SP_BAR_ArgumentBuilder()
		{
			argumentTypeArray = new Type[3]
			{
				typeof(long),
				typeof(long),
				typeof(long)
			};
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			return new SpBarArgument(array[0], array[1], array[2]);
		}
	}

	private sealed class SP_SWAP_ArgumentBuilder : ArgumentBuilder
	{
		public SP_SWAP_ArgumentBuilder(bool nullable)
		{
			argumentTypeArray = new Type[2]
			{
				typeof(long),
				typeof(long)
			};
			if (nullable)
			{
				minArg = 1;
			}
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			if (array.Length == 1)
			{
				array = new IOperandTerm[2]
				{
					array[0],
					null
				};
			}
			return new SpSwapCharaArgument(array[0], array[1]);
		}
	}

	private sealed class SP_SAVEDATA_ArgumentBuilder : ArgumentBuilder
	{
		public SP_SAVEDATA_ArgumentBuilder()
		{
			argumentTypeArray = new Type[2]
			{
				typeof(long),
				typeof(string)
			};
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			return new SpSaveDataArgument(array[0], array[1]);
		}
	}

	private sealed class SP_TINPUT_ArgumentBuilder : ArgumentBuilder
	{
		public SP_TINPUT_ArgumentBuilder()
		{
			argumentTypeArray = new Type[4]
			{
				typeof(long),
				typeof(long),
				typeof(long),
				typeof(string)
			};
			minArg = 2;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			IOperandTerm disp = null;
			IOperandTerm timeout = null;
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			if (array.Length > 2)
			{
				disp = array[2];
			}
			if (array.Length > 3)
			{
				timeout = array[3];
			}
			return new SpTInputsArgument(array[0], array[1], disp, timeout);
		}
	}

	private sealed class SP_TINPUTS_ArgumentBuilder : ArgumentBuilder
	{
		public SP_TINPUTS_ArgumentBuilder()
		{
			argumentTypeArray = new Type[4]
			{
				typeof(long),
				typeof(string),
				typeof(long),
				typeof(string)
			};
			minArg = 2;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			IOperandTerm disp = null;
			IOperandTerm timeout = null;
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			if (array.Length > 2)
			{
				disp = array[2];
			}
			if (array.Length > 3)
			{
				timeout = array[3];
			}
			return new SpTInputsArgument(array[0], array[1], disp, timeout);
		}
	}

	private sealed class SP_FOR_NEXT_ArgumentBuilder : ArgumentBuilder
	{
		public SP_FOR_NEXT_ArgumentBuilder()
		{
			argumentTypeArray = new Type[4]
			{
				typeof(long),
				null,
				typeof(long),
				typeof(long)
			};
			minArg = 3;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableTerm changeableVariable = getChangeableVariable(array, 1, line);
			if (changeableVariable == null)
			{
				return null;
			}
			if (changeableVariable.Identifier.IsCharacterData)
			{
				warn("第1引数にキャラクタ変数を指定することはできません", line, 2, isBackComp: false);
				return null;
			}
			IOperandTerm operandTerm = array[1];
			IOperandTerm end = array[2];
			IOperandTerm operandTerm2 = null;
			if (operandTerm == null)
			{
				operandTerm = new SingleTerm(0L);
			}
			operandTerm2 = ((array.Length <= 3 || array[3] == null) ? new SingleTerm(1L) : array[3]);
			if (!operandTerm.IsInteger)
			{
				warn("第2引数の型が違います", line, 2, isBackComp: false);
				return null;
			}
			return new SpForNextArgment(changeableVariable, operandTerm, end, operandTerm2);
		}
	}

	private sealed class SP_POWER_ArgumentBuilder : ArgumentBuilder
	{
		public SP_POWER_ArgumentBuilder()
		{
			argumentTypeArray = new Type[3]
			{
				typeof(long),
				typeof(long),
				typeof(long)
			};
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableTerm changeableVariable = getChangeableVariable(array, 1, line);
			if (changeableVariable == null)
			{
				return null;
			}
			return new SpPowerArgument(changeableVariable, array[1], array[2]);
		}
	}

	private sealed class SP_SWAPVAR_ArgumentBuilder : ArgumentBuilder
	{
		public SP_SWAPVAR_ArgumentBuilder()
		{
			argumentTypeArray = new Type[2]
			{
				typeof(void),
				typeof(void)
			};
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableTerm changeableVariable = getChangeableVariable(array, 1, line);
			if (changeableVariable == null)
			{
				return null;
			}
			VariableTerm changeableVariable2 = getChangeableVariable(array, 2, line);
			if (changeableVariable2 == null)
			{
				return null;
			}
			if (changeableVariable.GetOperandType() != changeableVariable2.GetOperandType())
			{
				warn("引数の型が異なります", line, 2, isBackComp: false);
				return null;
			}
			return new SpSwapVarArgument(changeableVariable, changeableVariable2);
		}
	}

	private sealed class VAR_INT_ArgumentBuilder : ArgumentBuilder
	{
		public VAR_INT_ArgumentBuilder()
		{
			argumentTypeArray = new Type[1] { typeof(long) };
			minArg = 0;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (array.Length == 0)
			{
				return new PrintDataArgument(null);
			}
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableTerm changeableVariable = getChangeableVariable(array, 1, line);
			if (changeableVariable == null)
			{
				return null;
			}
			return new PrintDataArgument(changeableVariable);
		}
	}

	private sealed class VAR_STR_ArgumentBuilder : ArgumentBuilder
	{
		public VAR_STR_ArgumentBuilder()
		{
			argumentTypeArray = new Type[1] { typeof(string) };
			minArg = 0;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (array.Length == 0)
			{
				return new StrDataArgument(new VariableTerm(GlobalStatic.VariableData.GetSystemVariableToken("RESULTS"), new IOperandTerm[1]
				{
					new SingleTerm(0L)
				}));
			}
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableTerm changeableVariable = getChangeableVariable(array, 1, line);
			if (changeableVariable == null)
			{
				return null;
			}
			return new StrDataArgument(changeableVariable);
		}
	}

	private sealed class BIT_ARG_ArgumentBuilder : ArgumentBuilder
	{
		public BIT_ARG_ArgumentBuilder()
		{
			argumentTypeArray = new Type[2]
			{
				typeof(long),
				typeof(long)
			};
			minArg = 2;
			argAny = true;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableTerm changeableVariable = getChangeableVariable(array, 1, line);
			if (changeableVariable == null)
			{
				return null;
			}
			List<IOperandTerm> list = new List<IOperandTerm>();
			list.AddRange(array);
			list.RemoveAt(0);
			BitArgument result = new BitArgument(changeableVariable, list.ToArray());
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] is SingleTerm)
				{
					long num = ((SingleTerm)list[i]).Int;
					if (num < 0 || num > 63)
					{
						throw new NotImplementedException();
					}
				}
			}
			return result;
		}
	}

	private sealed class SP_VAR_SET_ArgumentBuilder : ArgumentBuilder
	{
		public SP_VAR_SET_ArgumentBuilder()
		{
			argumentTypeArray = new Type[4]
			{
				typeof(void),
				typeof(void),
				typeof(long),
				typeof(long)
			};
			minArg = 1;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableTerm changeableVariable = getChangeableVariable(array, 1, line);
			if (changeableVariable == null)
			{
				return null;
			}
			if (changeableVariable.Identifier.IsConst)
			{
				warn("値を変更できない変数" + changeableVariable.Identifier.Name + "が指定されました", line, 2, isBackComp: false);
				return null;
			}
			IOperandTerm start = null;
			IOperandTerm end = null;
			IOperandTerm operandTerm = ((array.Length > 1) ? array[1] : ((!changeableVariable.IsString) ? new SingleTerm(0L) : new SingleTerm("")));
			if (changeableVariable is VariableNoArgTerm)
			{
				if (array.Length > 2)
				{
					warn("対象となる変数" + changeableVariable.Identifier.Name + "の要素を省略する場合には第3引数以降を設定できません", line, 2, isBackComp: false);
					return null;
				}
				return new SpVarSetArgument(new FixedVariableTerm(changeableVariable.Identifier), operandTerm, null, null);
			}
			if (array.Length > 2)
			{
				start = array[2];
			}
			if (array.Length > 3)
			{
				end = array[3];
			}
			if (array.Length >= 3 && !changeableVariable.Identifier.IsArray1D)
			{
				warn("第３引数以降は1次元配列以外では無視されます", line, 1, isBackComp: false);
			}
			if (operandTerm.GetOperandType() != changeableVariable.GetOperandType())
			{
				warn("２つの引数の型が一致していません", line, 2, isBackComp: false);
				return null;
			}
			return new SpVarSetArgument(changeableVariable, operandTerm, start, end);
		}
	}

	private sealed class SP_CVAR_SET_ArgumentBuilder : ArgumentBuilder
	{
		public SP_CVAR_SET_ArgumentBuilder()
		{
			argumentTypeArray = new Type[5]
			{
				typeof(void),
				typeof(void),
				typeof(void),
				typeof(long),
				typeof(long)
			};
			minArg = 1;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableTerm changeableVariable = getChangeableVariable(array, 1, line);
			if (changeableVariable == null)
			{
				return null;
			}
			if (!changeableVariable.Identifier.IsCharacterData)
			{
				warn("第１引数にキャラクタ変数以外の変数を指定することはできません", line, 2, isBackComp: false);
				return null;
			}
			if (changeableVariable.Identifier.IsArray2D)
			{
				warn("第１引数に二次元配列の変数を指定することはできません", line, 2, isBackComp: false);
				return null;
			}
			IOperandTerm start = null;
			IOperandTerm end = null;
			IOperandTerm operandTerm = ((array.Length <= 1) ? new SingleTerm(0L) : array[1]);
			IOperandTerm operandTerm2 = ((array.Length > 2) ? array[2] : ((!changeableVariable.IsString) ? new SingleTerm(0L) : new SingleTerm("")));
			if (array.Length > 3)
			{
				start = array[3];
			}
			if (array.Length > 4)
			{
				end = array[4];
			}
			if (operandTerm is SingleTerm && operandTerm.GetOperandType() == typeof(string) && changeableVariable.Identifier.IsArray1D && !GlobalStatic.ConstantData.isDefined(changeableVariable.Identifier.Code, ((SingleTerm)operandTerm).Str))
			{
				warn("文字列" + operandTerm.GetStrValue(null) + "は変数" + changeableVariable.Identifier.Name + "の要素ではありません", line, 2, isBackComp: false);
				return null;
			}
			if (array.Length > 3 && !changeableVariable.Identifier.IsArray1D)
			{
				warn("第４引数以降は1次元配列以外では無視されます", line, 1, isBackComp: false);
			}
			if (operandTerm2.GetOperandType() != changeableVariable.GetOperandType())
			{
				warn("２つの引数の型が一致していません", line, 2, isBackComp: false);
				return null;
			}
			return new SpCVarSetArgument(changeableVariable, operandTerm, operandTerm2, start, end);
		}
	}

	private sealed class SP_BUTTON_ArgumentBuilder : ArgumentBuilder
	{
		public SP_BUTTON_ArgumentBuilder()
		{
			argumentTypeArray = new Type[2]
			{
				typeof(string),
				typeof(void)
			};
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			return new SpButtonArgument(array[0], array[1]);
		}
	}

	private sealed class SP_COLOR_ArgumentBuilder : ArgumentBuilder
	{
		public SP_COLOR_ArgumentBuilder()
		{
			argumentTypeArray = new Type[3]
			{
				typeof(long),
				typeof(long),
				typeof(long)
			};
			minArg = 1;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			if (array.Length == 2)
			{
				warn("SETCOLORの引数の数が不正です(SETCOLORの引数は1個もしくは3個です)", line, 2, isBackComp: false);
				return null;
			}
			SpColorArgument spColorArgument = null;
			if (array.Length == 1)
			{
				spColorArgument = new SpColorArgument(array[0]);
				if (array[0] is SingleTerm)
				{
					spColorArgument.ConstInt = array[0].GetIntValue(exm);
					spColorArgument.IsConst = true;
				}
			}
			else
			{
				spColorArgument = new SpColorArgument(array[0], array[1], array[2]);
				if (array[0] is SingleTerm && array[1] is SingleTerm && array[2] is SingleTerm)
				{
					spColorArgument.ConstInt = (array[0].GetIntValue(exm) << 16) + (array[1].GetIntValue(exm) << 8) + array[2].GetIntValue(exm);
					spColorArgument.IsConst = true;
				}
			}
			return spColorArgument;
		}
	}

	private sealed class SP_SPLIT_ArgumentBuilder : ArgumentBuilder
	{
		public SP_SPLIT_ArgumentBuilder()
		{
			argumentTypeArray = new Type[4]
			{
				typeof(string),
				typeof(string),
				typeof(string),
				typeof(long)
			};
			minArg = 3;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableTerm changeableVariable = getChangeableVariable(array, 3, line);
			if (changeableVariable == null)
			{
				return null;
			}
			if (!changeableVariable.Identifier.IsArray1D && !changeableVariable.Identifier.IsArray2D && !changeableVariable.Identifier.IsArray3D)
			{
				warn("第３引数は配列変数でなければなりません", line, 2, isBackComp: false);
				return null;
			}
			VariableTerm num = ((array.Length >= 4) ? getChangeableVariable(array, 4, line) : new VariableTerm(GlobalStatic.VariableData.GetSystemVariableToken("RESULT"), new IOperandTerm[1]
			{
				new SingleTerm(0L)
			}));
			return new SpSplitArgument(array[0], array[1], changeableVariable.Identifier, num);
		}
	}

	private sealed class SP_HTMLSPLIT_ArgumentBuilder : ArgumentBuilder
	{
		public SP_HTMLSPLIT_ArgumentBuilder()
		{
			argumentTypeArray = new Type[3]
			{
				typeof(string),
				typeof(string),
				typeof(long)
			};
			minArg = 1;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableToken variableToken = null;
			VariableTerm variableTerm = null;
			VariableTerm variableTerm2 = null;
			if (array.Length >= 2)
			{
				variableTerm = getChangeableVariable(array, 2, line);
			}
			variableToken = ((variableTerm == null) ? GlobalStatic.VariableData.GetSystemVariableToken("RESULTS") : variableTerm.Identifier);
			if (!variableToken.IsArray1D || variableToken.IsCharacterData)
			{
				warn("第２引数は非キャラ型の1次元配列変数でなければなりません", line, 2, isBackComp: false);
				return null;
			}
			if (array.Length >= 3)
			{
				variableTerm2 = getChangeableVariable(array, 3, line);
			}
			if (variableTerm2 == null)
			{
				variableTerm2 = new VariableTerm(GlobalStatic.VariableData.GetSystemVariableToken("RESULT"), new IOperandTerm[1]
				{
					new SingleTerm(0L)
				});
			}
			return new SpHtmlSplitArgument(array[0], variableToken, variableTerm2);
		}
	}

	private sealed class SP_GETINT_ArgumentBuilder : ArgumentBuilder
	{
		public SP_GETINT_ArgumentBuilder()
		{
			argumentTypeArray = new Type[1] { typeof(long) };
			minArg = 0;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (array.Length == 0)
			{
				return new SpGetIntArgument(new VariableTerm(GlobalStatic.VariableData.GetSystemVariableToken("RESULT"), new IOperandTerm[1]
				{
					new SingleTerm(0L)
				}));
			}
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableTerm changeableVariable = getChangeableVariable(array, 1, line);
			if (changeableVariable == null)
			{
				return null;
			}
			return new SpGetIntArgument(changeableVariable);
		}
	}

	private sealed class SP_CONTROL_ARRAY_ArgumentBuilder : ArgumentBuilder
	{
		public SP_CONTROL_ARRAY_ArgumentBuilder()
		{
			argumentTypeArray = new Type[3]
			{
				typeof(void),
				typeof(long),
				typeof(long)
			};
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableTerm changeableVariable = getChangeableVariable(array, 1, line);
			if (changeableVariable == null)
			{
				return null;
			}
			return new SpArrayControlArgument(changeableVariable, array[1], array[2]);
		}
	}

	private sealed class SP_SHIFT_ARRAY_ArgumentBuilder : ArgumentBuilder
	{
		public SP_SHIFT_ARRAY_ArgumentBuilder()
		{
			argumentTypeArray = new Type[5]
			{
				typeof(void),
				typeof(long),
				typeof(void),
				typeof(long),
				typeof(long)
			};
			minArg = 3;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableTerm changeableVariable = getChangeableVariable(array, 1, line);
			if (changeableVariable == null)
			{
				return null;
			}
			if (!changeableVariable.Identifier.IsArray1D)
			{
				warn("第１引数に１次元配列もしくは配列型キャラクタ変数以外を指定することはできません", line, 2, isBackComp: false);
				return null;
			}
			if (line.FunctionCode == FunctionCode.ARRAYSHIFT && array[0].GetOperandType() != array[2].GetOperandType())
			{
				warn("第１引数と第３引数の型が違います", line, 2, isBackComp: false);
				return null;
			}
			IOperandTerm num = ((array.Length >= 4) ? array[3] : new SingleTerm(0L));
			IOperandTerm num2 = ((array.Length >= 5) ? array[4] : null);
			return new SpArrayShiftArgument(changeableVariable, array[1], array[2], num, num2);
		}
	}

	private sealed class SP_SAVEVAR_ArgumentBuilder : ArgumentBuilder
	{
		public SP_SAVEVAR_ArgumentBuilder()
		{
			argumentTypeArray = new Type[3]
			{
				typeof(string),
				typeof(string),
				typeof(void)
			};
			argAny = true;
			minArg = 3;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			List<VariableToken> list = new List<VariableToken>();
			for (int i = 2; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					warn("第" + (i + 1) + "引数を省略できません", line, 2, isBackComp: false);
					return null;
				}
				VariableTerm changeableVariable = getChangeableVariable(array, i + 1, line);
				if (changeableVariable == null)
				{
					return null;
				}
				VariableToken identifier = changeableVariable.Identifier;
				if (identifier.IsCharacterData)
				{
					warn("キャラクタ変数" + identifier.Name + "はセーブできません(キャラクタ変数のSAVEにはSAVECHARAを使用します)", line, 2, isBackComp: false);
					return null;
				}
				if (identifier.IsPrivate)
				{
					warn("プライベート変数" + identifier.Name + "はセーブできません", line, 2, isBackComp: false);
					return null;
				}
				if (identifier.IsLocal)
				{
					warn("ローカル変数" + identifier.Name + "はセーブできません", line, 2, isBackComp: false);
					return null;
				}
				if (identifier.IsConst)
				{
					warn("値を変更できない変数はセーブできません", line, 2, isBackComp: false);
					return null;
				}
				if (identifier.IsCalc)
				{
					warn("疑似変数はセーブできません", line, 2, isBackComp: false);
					return null;
				}
				if (identifier.IsReference)
				{
					warn("参照型変数はセーブできません", line, 2, isBackComp: false);
					return null;
				}
				list.Add(identifier);
			}
			for (int j = 0; j < list.Count; j++)
			{
				for (int k = j + 1; k < list.Count; k++)
				{
					if (list[j] == list[k])
					{
						warn("変数" + list[j].Name + "を二度以上保存しようとしています", line, 1, isBackComp: false);
						return null;
					}
				}
			}
			VariableToken[] array2 = new VariableToken[list.Count];
			list.CopyTo(array2);
			return new SpSaveVarArgument(array[0], array[1], array2);
		}
	}

	private sealed class SP_SAVECHARA_ArgumentBuilder : ArgumentBuilder
	{
		public SP_SAVECHARA_ArgumentBuilder()
		{
			argumentTypeArray = new Type[3]
			{
				typeof(string),
				typeof(string),
				typeof(long)
			};
			minArg = 3;
			argAny = true;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			List<IOperandTerm> list = new List<IOperandTerm>();
			list.AddRange(array);
			ExpressionArrayArgument result = new ExpressionArrayArgument(list);
			for (int i = 2; i < list.Count; i++)
			{
				if (!(list[i] is SingleTerm))
				{
					continue;
				}
				long intValue = list[i].GetIntValue(null);
				if (intValue < 0)
				{
					warn("キャラ登録番号は正の値でなければなりません", line, 2, isBackComp: false);
					return null;
				}
				if (intValue > int.MaxValue)
				{
					warn("キャラ登録番号が32bit符号付整数の上限を超えています", line, 2, isBackComp: false);
					return null;
				}
				for (int j = i + 1; j < list.Count; j++)
				{
					if (list[j] is SingleTerm && intValue == list[j].GetIntValue(null))
					{
						warn("キャラ登録番号" + intValue + "を二度以上保存しようとしています", line, 1, isBackComp: false);
						return null;
					}
				}
			}
			return result;
		}
	}

	private sealed class SP_REF_ArgumentBuilder : ArgumentBuilder
	{
		private bool byname;

		public SP_REF_ArgumentBuilder(bool byname)
		{
			argumentTypeArray = new Type[2]
			{
				typeof(void),
				typeof(void)
			};
			minArg = 2;
			this.byname = byname;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			WordCollection wordCollection = popWords(line);
			IdentifierWord identifierWord = wordCollection.Current as IdentifierWord;
			wordCollection.ShiftNext();
			if (identifierWord == null || wordCollection.Current.Type != ',')
			{
				warn("書式が間違っています", line, 2, isBackComp: false);
				return null;
			}
			wordCollection.ShiftNext();
			IOperandTerm operandTerm = null;
			IdentifierWord identifierWord2 = null;
			string text = null;
			if (byname)
			{
				operandTerm = ExpressionParser.ReduceExpressionTerm(wordCollection, TermEndWith.EoL);
				if (operandTerm == null || operandTerm.IsInteger || !wordCollection.EOL)
				{
					warn("書式が間違っています", line, 2, isBackComp: false);
					return null;
				}
				operandTerm = operandTerm.Restructure(exm);
				if (operandTerm is SingleTerm)
				{
					text = operandTerm.GetStrValue(exm);
				}
			}
			else
			{
				identifierWord2 = wordCollection.Current as IdentifierWord;
				wordCollection.ShiftNext();
				if (identifierWord2 == null || !wordCollection.EOL)
				{
					warn("書式が間違っています", line, 2, isBackComp: false);
					return null;
				}
				text = identifierWord2.Code;
			}
			UserDefinedRefMethod refMethod = GlobalStatic.IdentifierDictionary.GetRefMethod(identifierWord.Code);
			ReferenceToken vt = null;
			if (refMethod == null)
			{
				VariableToken variableToken = GlobalStatic.IdentifierDictionary.GetVariableToken(identifierWord.Code, null, allowPrivate: true);
				if (variableToken == null || !variableToken.IsReference)
				{
					warn("第一引数は関数参照か参照型変数でなければなりません", line, 2, isBackComp: false);
					return null;
				}
				vt = (ReferenceToken)variableToken;
			}
			if (refMethod != null)
			{
				if (text == null)
				{
					return new RefArgument(refMethod, operandTerm);
				}
				UserDefinedRefMethod refMethod2 = GlobalStatic.IdentifierDictionary.GetRefMethod(text);
				if (refMethod2 != null)
				{
					return new RefArgument(refMethod, refMethod2);
				}
				FunctionLabelLine nonEventLabel = GlobalStatic.LabelDictionary.GetNonEventLabel(text);
				if (nonEventLabel == null)
				{
					warn("式中関数" + text + "が見つかりません", line, 2, isBackComp: false);
					return null;
				}
				if (!nonEventLabel.IsMethod)
				{
					warn("#FUNCTION(S)属性を持たない関数" + text + "は参照できません", line, 2, isBackComp: false);
					return null;
				}
				CalledFunction src = CalledFunction.CreateCalledFunctionMethod(nonEventLabel, nonEventLabel.LabelName);
				return new RefArgument(refMethod, src);
			}
			if (text == null)
			{
				return new RefArgument(vt, operandTerm);
			}
			VariableToken variableToken2 = GlobalStatic.IdentifierDictionary.GetVariableToken(text, null, allowPrivate: true);
			if (variableToken2 == null)
			{
				warn("変数" + text + "が見つかりません", line, 2, isBackComp: false);
				return null;
			}
			return new RefArgument(vt, variableToken2);
		}
	}

	private sealed class SP_INPUT_ArgumentBuilder : ArgumentBuilder
	{
		public SP_INPUT_ArgumentBuilder()
		{
			argumentTypeArray = new Type[1] { typeof(long) };
			minArg = 0;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			IOperandTerm termSrc = null;
			if (array.Length == 0)
			{
				return new ExpressionArgument(termSrc);
			}
			termSrc = array[0];
			ExpressionArgument expressionArgument = new ExpressionArgument(termSrc);
			if (termSrc is SingleTerm)
			{
				long num = termSrc.GetIntValue(null);
				if (line.FunctionCode == FunctionCode.ONEINPUT)
				{
					if (num < 0)
					{
						warn("ONEINPUTの引数にONEINPUTが受け取れない負の数数が指定されています（引数を無効とします）", line, 1, isBackComp: false);
						return new ExpressionArgument(null);
					}
					if (num > 9)
					{
						warn("ONEINPUTの引数にONEINPUTが受け取れない2桁以上の数数が指定されています（最初の桁を引数と見なします）", line, 1, isBackComp: false);
						num = long.Parse(num.ToString().Remove(1));
					}
				}
				expressionArgument.ConstInt = num;
				expressionArgument.IsConst = true;
			}
			return expressionArgument;
		}
	}

	private sealed class SP_COPY_ARRAY_Arguments : ArgumentBuilder
	{
		public SP_COPY_ARRAY_Arguments()
		{
			argumentTypeArray = new Type[2]
			{
				typeof(string),
				typeof(string)
			};
			minArg = 2;
		}

		public override Argument CreateArgument(InstructionLine line, ExpressionMediator exm)
		{
			IOperandTerm[] array = popTerms(line);
			if (!checkArgumentType(line, exm, array))
			{
				return null;
			}
			VariableToken[] array2 = new VariableToken[2];
			if (array[0] is SingleTerm)
			{
				if ((array2[0] = GlobalStatic.IdentifierDictionary.GetVariableToken(((SingleTerm)array[0]).Str, null, allowPrivate: true)) == null)
				{
					warn("ARRAYCOPY命令の第１引数\"" + ((SingleTerm)array[0]).Str + "\"は変数名として存在しません", line, 2, isBackComp: false);
					return null;
				}
				if (!array2[0].IsArray1D && !array2[0].IsArray2D && !array2[0].IsArray3D)
				{
					warn("ARRAYCOPY命令の第１引数\"" + ((SingleTerm)array[0]).Str + "\"は配列変数ではありません", line, 2, isBackComp: false);
					return null;
				}
				if (array2[0].IsCharacterData)
				{
					warn("ARRAYCOPY命令の第１引数\"" + ((SingleTerm)array[0]).Str + "\"はキャラクタ変数です（対応していません）", line, 2, isBackComp: false);
					return null;
				}
			}
			if (array[1] is SingleTerm)
			{
				if ((array2[1] = GlobalStatic.IdentifierDictionary.GetVariableToken(((SingleTerm)array[1]).Str, null, allowPrivate: true)) == null)
				{
					warn("ARRAYCOPY命令の第２引数\"" + ((SingleTerm)array[1]).Str + "\"は変数名として存在しません", line, 2, isBackComp: false);
					return null;
				}
				if (!array2[1].IsArray1D && !array2[1].IsArray2D && !array2[1].IsArray3D)
				{
					warn("ARRAYCOPY命令の第２引数\"" + ((SingleTerm)array[1]).Str + "\"は配列変数ではありません", line, 2, isBackComp: false);
				}
				if (array2[1].IsCharacterData)
				{
					warn("ARRAYCOPY命令の第２引数\"" + ((SingleTerm)array[1]).Str + "\"はキャラクタ変数です（対応していません）", line, 2, isBackComp: false);
					return null;
				}
				if (array2[1].IsConst)
				{
					warn("ARRAYCOPY命令の第２引数\"" + ((SingleTerm)array[1]).Str + "\"は値を変更できない変数です", line, 2, isBackComp: false);
					return null;
				}
			}
			if (array2[0] != null && array2[1] != null)
			{
				if ((array2[0].IsArray1D && !array2[1].IsArray1D) || (array2[0].IsArray2D && !array2[1].IsArray2D) || (array2[0].IsArray3D && !array2[1].IsArray3D))
				{
					warn("ARRAYCOPY命令の2つの引数の次元が異なります", line, 2, isBackComp: false);
					return null;
				}
				if ((array2[0].IsInteger && array2[1].IsString) || (array2[0].IsString && array2[1].IsInteger))
				{
					warn("ARRAYCOPY命令の２つの配列変数の型が一致していません", line, 2, isBackComp: false);
					return null;
				}
			}
			return new SpCopyArrayArgument(array[0], array[1]);
		}
	}

	private static readonly Dictionary<FunctionArgType, ArgumentBuilder> argb;

	public static Dictionary<FunctionArgType, ArgumentBuilder> GetArgumentBuilderDictionary()
	{
		return argb;
	}

	public static ArgumentBuilder GetArgumentBuilder(FunctionArgType key)
	{
		return argb[key];
	}

	static ArgumentParser()
	{
		argb = new Dictionary<FunctionArgType, ArgumentBuilder>();
		argb[FunctionArgType.METHOD] = new METHOD_ArgumentBuilder();
		argb[FunctionArgType.VOID] = new VOID_ArgumentBuilder();
		argb[FunctionArgType.INT_EXPRESSION] = new INT_EXPRESSION_ArgumentBuilder(nullable: false);
		argb[FunctionArgType.INT_EXPRESSION_NULLABLE] = new INT_EXPRESSION_ArgumentBuilder(nullable: true);
		argb[FunctionArgType.STR_EXPRESSION] = new STR_EXPRESSION_ArgumentBuilder(nullable: false);
		argb[FunctionArgType.STR_EXPRESSION_NULLABLE] = new STR_EXPRESSION_ArgumentBuilder(nullable: true);
		argb[FunctionArgType.STR] = new STR_ArgumentBuilder(nullable: false);
		argb[FunctionArgType.STR_NULLABLE] = new STR_ArgumentBuilder(nullable: true);
		argb[FunctionArgType.FORM_STR] = new FORM_STR_ArgumentBuilder(nullable: false);
		argb[FunctionArgType.FORM_STR_NULLABLE] = new FORM_STR_ArgumentBuilder(nullable: true);
		argb[FunctionArgType.SP_PRINTV] = new SP_PRINTV_ArgumentBuilder();
		argb[FunctionArgType.SP_TIMES] = new SP_TIMES_ArgumentBuilder();
		argb[FunctionArgType.SP_BAR] = new SP_BAR_ArgumentBuilder();
		argb[FunctionArgType.SP_SET] = new SP_SET_ArgumentBuilder();
		argb[FunctionArgType.SP_SETS] = new SP_SET_ArgumentBuilder();
		argb[FunctionArgType.SP_SWAP] = new SP_SWAP_ArgumentBuilder(nullable: false);
		argb[FunctionArgType.SP_VAR] = new SP_VAR_ArgumentBuilder();
		argb[FunctionArgType.SP_SAVEDATA] = new SP_SAVEDATA_ArgumentBuilder();
		argb[FunctionArgType.SP_TINPUT] = new SP_TINPUT_ArgumentBuilder();
		argb[FunctionArgType.SP_TINPUTS] = new SP_TINPUTS_ArgumentBuilder();
		argb[FunctionArgType.SP_SORTCHARA] = new SP_SORTCHARA_ArgumentBuilder();
		argb[FunctionArgType.SP_CALL] = new SP_CALL_ArgumentBuilder(callf: false, form: false);
		argb[FunctionArgType.SP_CALLF] = new SP_CALL_ArgumentBuilder(callf: true, form: false);
		argb[FunctionArgType.SP_CALLFORM] = new SP_CALL_ArgumentBuilder(callf: false, form: true);
		argb[FunctionArgType.SP_CALLFORMF] = new SP_CALL_ArgumentBuilder(callf: true, form: true);
		argb[FunctionArgType.SP_FOR_NEXT] = new SP_FOR_NEXT_ArgumentBuilder();
		argb[FunctionArgType.SP_POWER] = new SP_POWER_ArgumentBuilder();
		argb[FunctionArgType.SP_SWAPVAR] = new SP_SWAPVAR_ArgumentBuilder();
		argb[FunctionArgType.EXPRESSION] = new EXPRESSION_ArgumentBuilder(nullable: false);
		argb[FunctionArgType.EXPRESSION_NULLABLE] = new EXPRESSION_ArgumentBuilder(nullable: true);
		argb[FunctionArgType.CASE] = new CASE_ArgumentBuilder();
		argb[FunctionArgType.VAR_INT] = new VAR_INT_ArgumentBuilder();
		argb[FunctionArgType.VAR_STR] = new VAR_STR_ArgumentBuilder();
		argb[FunctionArgType.BIT_ARG] = new BIT_ARG_ArgumentBuilder();
		argb[FunctionArgType.SP_VAR_SET] = new SP_VAR_SET_ArgumentBuilder();
		argb[FunctionArgType.SP_BUTTON] = new SP_BUTTON_ArgumentBuilder();
		argb[FunctionArgType.SP_COLOR] = new SP_COLOR_ArgumentBuilder();
		argb[FunctionArgType.SP_SPLIT] = new SP_SPLIT_ArgumentBuilder();
		argb[FunctionArgType.SP_GETINT] = new SP_GETINT_ArgumentBuilder();
		argb[FunctionArgType.SP_CVAR_SET] = new SP_CVAR_SET_ArgumentBuilder();
		argb[FunctionArgType.SP_CONTROL_ARRAY] = new SP_CONTROL_ARRAY_ArgumentBuilder();
		argb[FunctionArgType.SP_SHIFT_ARRAY] = new SP_SHIFT_ARRAY_ArgumentBuilder();
		argb[FunctionArgType.SP_SORTARRAY] = new SP_SORT_ARRAY_ArgumentBuilder();
		argb[FunctionArgType.INT_ANY] = new INT_ANY_ArgumentBuilder();
		argb[FunctionArgType.FORM_STR_ANY] = new FORM_STR_ANY_ArgumentBuilder();
		argb[FunctionArgType.SP_COPYCHARA] = new SP_SWAP_ArgumentBuilder(nullable: true);
		argb[FunctionArgType.SP_INPUT] = new SP_INPUT_ArgumentBuilder();
		argb[FunctionArgType.SP_INPUTS] = new SP_INPUTS_ArgumentBuilder();
		argb[FunctionArgType.SP_COPY_ARRAY] = new SP_COPY_ARRAY_Arguments();
		argb[FunctionArgType.SP_SAVEVAR] = new SP_SAVEVAR_ArgumentBuilder();
		argb[FunctionArgType.SP_SAVECHARA] = new SP_SAVECHARA_ArgumentBuilder();
		argb[FunctionArgType.SP_REF] = new SP_REF_ArgumentBuilder(byname: false);
		argb[FunctionArgType.SP_REFBYNAME] = new SP_REF_ArgumentBuilder(byname: true);
		argb[FunctionArgType.SP_HTMLSPLIT] = new SP_HTMLSPLIT_ArgumentBuilder();
	}

	public static bool SetArgumentTo(InstructionLine line)
	{
		if (line == null)
		{
			return false;
		}
		if (line.Argument != null)
		{
			return true;
		}
		if (line.IsError)
		{
			return false;
		}
		if (!Program.DebugMode && line.Function.IsDebug())
		{
			line.Argument = null;
			return true;
		}
		Argument argument = null;
		string text = null;
		try
		{
			argument = line.Function.ArgBuilder.CreateArgument(line, GlobalStatic.EMediator);
		}
		catch (EmueraException ex)
		{
			text = ex.Message;
			goto IL_0083;
		}
		if (argument == null)
		{
			if (!line.IsError)
			{
				text = "命令の引数解析中に特定できないエラーが発生";
				goto IL_0083;
			}
			return false;
		}
		line.Argument = argument;
		if (argument == null)
		{
			line.IsError = true;
		}
		return true;
		IL_0083:
		line.IsError = true;
		line.ErrMes = text;
		ParserMediator.Warn(text, line, 2, isError: true, isBackComp: false);
		return false;
	}
}
