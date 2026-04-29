using System;
using System.Collections.Generic;
using System.IO;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameProc.Function;
using MinorShift.Emuera.GameView;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal static class LogicalLineParser
{
	public static bool ParseSharpLine(FunctionLabelLine label, StringStream st, ScriptPosition position, List<string> OnlyLabel)
	{
		st.ShiftNext();
		string text = LexicalAnalyzer.ReadSingleIdentifier(st);
		if (Config.ICFunction)
		{
			text = text.ToUpper();
		}
		if (text == null || (text != "SINGLE" && text != "LATER" && text != "PRI" && text != "ONLY" && text != "FUNCTION" && text != "FUNCTIONS" && text != "LOCALSIZE" && text != "LOCALSSIZE" && text != "DIM" && text != "DIMS"))
		{
			ParserMediator.Warn("解釈できない#行です", position, 1);
			return false;
		}
		try
		{
			WordCollection wordCollection = LexicalAnalyzer.Analyse(st, LexEndWith.EoL, LexAnalyzeFlag.AllowAssignment);
			switch (text)
			{
			case "SINGLE":
				if (label.IsMethod)
				{
					ParserMediator.Warn("式中関数では#SINGLEは機能しません", position, 1);
				}
				else if (!label.IsEvent)
				{
					ParserMediator.Warn("イベント関数以外では#SINGLEは機能しません", position, 1);
				}
				else if (label.IsSingle)
				{
					ParserMediator.Warn("#SINGLEが重複して使われています", position, 1);
				}
				else if (label.IsOnly)
				{
					ParserMediator.Warn("#ONLYが指定されたイベント関数では#SINGLEは機能しません", position, 1);
				}
				else
				{
					label.IsSingle = true;
				}
				break;
			case "LATER":
				if (label.IsMethod)
				{
					ParserMediator.Warn("式中関数では#LATERは機能しません", position, 1);
					break;
				}
				if (!label.IsEvent)
				{
					ParserMediator.Warn("イベント関数以外では#LATERは機能しません", position, 1);
					break;
				}
				if (label.IsLater)
				{
					ParserMediator.Warn("#LATERが重複して使われています", position, 1);
					break;
				}
				if (label.IsOnly)
				{
					ParserMediator.Warn("#ONLYが指定されたイベント関数では#LATERは機能しません", position, 1);
					break;
				}
				if (label.IsPri)
				{
					ParserMediator.Warn("#PRIと#LATERが重複して使われています(この関数は2度呼ばれます)", position, 1);
				}
				label.IsLater = true;
				break;
			case "PRI":
				if (label.IsMethod)
				{
					ParserMediator.Warn("式中関数では#PRIは機能しません", position, 1);
					break;
				}
				if (!label.IsEvent)
				{
					ParserMediator.Warn("イベント関数以外では#PRIは機能しません", position, 1);
					break;
				}
				if (label.IsPri)
				{
					ParserMediator.Warn("#PRIが重複して使われています", position, 1);
					break;
				}
				if (label.IsOnly)
				{
					ParserMediator.Warn("#ONLYが指定されたイベント関数では#PRIは機能しません", position, 1);
					break;
				}
				if (label.IsLater)
				{
					ParserMediator.Warn("#PRIと#LATERが重複して使われています(この関数は2度呼ばれます)", position, 1);
				}
				label.IsPri = true;
				break;
			case "ONLY":
				if (label.IsMethod)
				{
					ParserMediator.Warn("式中関数では#ONLYは機能しません", position, 1);
					break;
				}
				if (!label.IsEvent)
				{
					ParserMediator.Warn("イベント関数以外では#ONLYは機能しません", position, 1);
					break;
				}
				if (label.IsOnly)
				{
					ParserMediator.Warn("#ONLYが重複して使われています", position, 1);
					break;
				}
				if (OnlyLabel.Contains(label.LabelName))
				{
					ParserMediator.Warn("このイベント関数\"@" + label.LabelName + "\"にはすでに#ONLYが宣言されています（この関数は実行されません）", position, 1);
				}
				OnlyLabel.Add(label.LabelName);
				label.IsOnly = true;
				if (label.IsPri)
				{
					ParserMediator.Warn("このイベント関数には#PRIが宣言されていますが無視されます", position, 1);
					label.IsPri = false;
				}
				if (label.IsLater)
				{
					ParserMediator.Warn("このイベント関数には#LATERが宣言されていますが無視されます", position, 1);
					label.IsLater = false;
				}
				if (label.IsSingle)
				{
					ParserMediator.Warn("このイベント関数には#SINGLEが宣言されていますが無視されます", position, 1);
					label.IsSingle = false;
				}
				break;
			case "FUNCTION":
			case "FUNCTIONS":
				if (!string.IsNullOrEmpty(label.LabelName) && char.IsDigit(label.LabelName[0]))
				{
					ParserMediator.Warn("#" + text + "属性は関数名が数字で始まる関数には指定できません", position, 1);
					label.IsError = true;
					label.ErrMes = "関数名が数字で始まっています";
					break;
				}
				if (label.IsMethod)
				{
					if ((label.MethodType == typeof(long) && text == "FUNCTION") || (label.MethodType == typeof(string) && text == "FUNCTIONS"))
					{
						ParserMediator.Warn("関数" + label.LabelName + "にはすでに#" + text + "が宣言されています(この行は無視されます)", position, 1);
						return false;
					}
					if (label.MethodType == typeof(long) && text == "FUNCTIONS")
					{
						ParserMediator.Warn("関数" + label.LabelName + "にはすでに#FUNCTIONが宣言されています", position, 2);
					}
					else if (label.MethodType == typeof(string) && text == "FUNCTION")
					{
						ParserMediator.Warn("関数" + label.LabelName + "にはすでに#FUNCTIONSが宣言されています", position, 2);
					}
					return false;
				}
				if (label.Depth == 0)
				{
					ParserMediator.Warn("システム関数に#" + text + "が指定されています", position, 2);
					return false;
				}
				label.IsMethod = true;
				label.Depth = 0;
				if (text == "FUNCTIONS")
				{
					label.MethodType = typeof(string);
				}
				else
				{
					label.MethodType = typeof(long);
				}
				if (label.IsPri)
				{
					ParserMediator.Warn("式中関数では#PRIは機能しません", position, 1);
					label.IsPri = false;
				}
				if (label.IsLater)
				{
					ParserMediator.Warn("式中関数では#LATERは機能しません", position, 1);
					label.IsLater = false;
				}
				if (label.IsSingle)
				{
					ParserMediator.Warn("式中関数では#SINGLEは機能しません", position, 1);
					label.IsSingle = false;
				}
				if (label.IsOnly)
				{
					ParserMediator.Warn("式中関数では#ONLYは機能しません", position, 1);
					label.IsOnly = false;
				}
				break;
			case "LOCALSIZE":
			case "LOCALSSIZE":
			{
				if (wordCollection.EOL)
				{
					ParserMediator.Warn("#" + text + "の後に有効な数値が指定されていません", position, 2);
					break;
				}
				if (label.IsEvent)
				{
					ParserMediator.Warn("イベント関数では#" + text + "による" + text.Substring(0, text.Length - 4) + "のサイズ指定は無視されます", position, 1);
					break;
				}
				if (!(ExpressionParser.ReduceIntegerTerm(wordCollection, TermEndWith.EoL).Restructure(null) is SingleTerm singleTerm) || singleTerm.GetOperandType() != typeof(long))
				{
					ParserMediator.Warn("#" + text + "の後に有効な定数式が指定されていません", position, 2);
					break;
				}
				if (singleTerm.Int <= 0)
				{
					ParserMediator.Warn("#" + text + "に0以下の値(" + singleTerm.Int + ")が与えられました。設定は無視されます", position, 1);
					break;
				}
				if (singleTerm.Int >= int.MaxValue)
				{
					ParserMediator.Warn("#" + text + "に大きすぎる値(" + singleTerm.Int + ")が与えられました。設定は無視されます", position, 1);
					break;
				}
				int num = (int)singleTerm.Int;
				if (text == "LOCALSIZE")
				{
					if (GlobalStatic.IdentifierDictionary.getLocalIsForbid("LOCAL"))
					{
						ParserMediator.Warn("#" + text + "が指定されていますが変数LOCALは使用禁止されています", position, 2);
						break;
					}
					if (label.LocalLength > 0)
					{
						ParserMediator.Warn("この関数にはすでに#LOCALSIZEが定義されています。（以前の定義は無視されます）", position, 1);
					}
					label.LocalLength = num;
				}
				else if (GlobalStatic.IdentifierDictionary.getLocalIsForbid("LOCALS"))
				{
					ParserMediator.Warn("#" + text + "が指定されていますが変数LOCALSは使用禁止されています", position, 2);
				}
				else
				{
					if (label.LocalsLength > 0)
					{
						ParserMediator.Warn("この関数にはすでに#LOCALSSIZEが定義されています。（以前の定義は無視されます）", position, 1);
					}
					label.LocalsLength = num;
				}
				break;
			}
			case "DIM":
			case "DIMS":
			{
				UserDefinedVariableData userDefinedVariableData = UserDefinedVariableData.Create(wordCollection, text == "DIMS", isPrivate: true, position);
				if (!label.AddPrivateVariable(userDefinedVariableData))
				{
					ParserMediator.Warn("変数名" + userDefinedVariableData.Name + "は既に使用されています", position, 2);
					return false;
				}
				break;
			}
			default:
				ParserMediator.Warn("解釈できない#行です", position, 1);
				break;
			}
			if (!wordCollection.EOL)
			{
				ParserMediator.Warn("#の識別子の後に余分な文字があります", position, 1);
			}
		}
		catch (Exception ex)
		{
			ParserMediator.Warn(ex.Message, position, 2);
			goto IL_093e;
		}
		return true;
		IL_093e:
		return false;
	}

	public static LogicalLine ParseLine(string str, EmueraConsole console)
	{
		ScriptPosition position = new ScriptPosition(str);
		return ParseLine(new StringStream(str), position, console);
	}

	public static LogicalLine ParseLabelLine(StringStream stream, ScriptPosition position, EmueraConsole console)
	{
		bool flag = stream.Current == '@';
		_ = position.LineNo;
		string text = "";
		string errMes = "";
		try
		{
			int warnLevel = -1;
			stream.ShiftNext();
			WordCollection wordCollection = LexicalAnalyzer.Analyse(stream, LexEndWith.EoL, LexAnalyzeFlag.AllowAssignment);
			if (wordCollection.EOL || !(wordCollection.Current is IdentifierWord))
			{
				errMes = "関数名が不正であるか存在しません";
			}
			else
			{
				text = ((IdentifierWord)wordCollection.Current).Code;
				wordCollection.ShiftNext();
				if (Config.ICVariable)
				{
					text = text.ToUpper();
				}
				GlobalStatic.IdentifierDictionary.CheckUserLabelName(ref errMes, ref warnLevel, flag, text);
				if (warnLevel < 0)
				{
					goto IL_009e;
				}
				if (warnLevel < 2)
				{
					ParserMediator.Warn(errMes, position, warnLevel);
					goto IL_009e;
				}
			}
			goto end_IL_001e;
			IL_009e:
			if (!flag)
			{
				if (!wordCollection.EOL)
				{
					ParserMediator.Warn("$で始まるラベルに引数が設定されています", position, 1);
				}
				return new GotoLabelLine(position, text);
			}
			if (Program.AnalysisMode)
			{
				console.PrintC("@" + text, alignmentRight: false);
			}
			FunctionLabelLine functionLabelLine = new FunctionLabelLine(position, text, wordCollection);
			if (IdentifierDictionary.IsEventLabelName(text))
			{
				functionLabelLine.IsEvent = true;
				functionLabelLine.IsSystem = true;
				functionLabelLine.Depth = 0;
			}
			else if (IdentifierDictionary.IsSystemLabelName(text))
			{
				functionLabelLine.IsSystem = true;
				functionLabelLine.Depth = 0;
			}
			return functionLabelLine;
			end_IL_001e:;
		}
		catch (CodeEE codeEE)
		{
			errMes = codeEE.Message;
		}
		if (flag)
		{
			if (text.Length == 0)
			{
				text = "<Error>";
			}
			return new InvalidLabelLine(position, text, errMes);
		}
		return new InvalidLine(position, errMes);
	}

	public static LogicalLine ParseLine(StringStream stream, ScriptPosition position, EmueraConsole console)
	{
		_ = position.LineNo;
		string text = "";
		LexicalAnalyzer.SkipWhiteSpace(stream);
		if (stream.EOS)
		{
			return null;
		}
		try
		{
			if (stream.Current == '+' || stream.Current == '-')
			{
				char current = stream.Current;
				WordCollection wordCollection = LexicalAnalyzer.Analyse(stream, LexEndWith.EoL, LexAnalyzeFlag.None);
				if (wordCollection.Current is OperatorWord operatorWord && (operatorWord.Code == OperatorCode.Increment || operatorWord.Code == OperatorCode.Decrement))
				{
					wordCollection.ShiftNext();
					return new InstructionLine(position, FunctionIdentifier.SETFunction, operatorWord.Code, wordCollection, null);
				}
				text = ((current != '+') ? "行が'-'から始まっていますが、デクリメントではありません" : "行が'+'から始まっていますが、インクリメントではありません");
			}
			else
			{
				IdentifierWord identifierWord = LexicalAnalyzer.ReadFirstIdentifierWord(stream);
				if (identifierWord != null)
				{
					FunctionIdentifier functionIdentifier = GlobalStatic.IdentifierDictionary.GetFunctionIdentifier(identifierWord.Code);
					if (functionIdentifier != null)
					{
						if (stream.EOS)
						{
							return new InstructionLine(position, functionIdentifier, stream);
						}
						if (stream.Current == ';' || stream.Current == ' ' || stream.Current == '\t' || (Config.SystemAllowFullSpace && stream.Current == '\u3000'))
						{
							stream.ShiftNext();
							return new InstructionLine(position, functionIdentifier, stream);
						}
						text = ((stream.Current != '\u3000') ? "命令で行が始まっていますが、命令の直後に半角スペース・タブ以外の文字が来ています" : ("命令で行が始まっていますが、命令の直後に半角スペース・タブ以外の文字が来ています(この警告はシステムオプション「" + Config.GetConfigName(ConfigCode.SystemAllowFullSpace) + "」により無視できます)"));
						goto IL_01d7;
					}
				}
				LexicalAnalyzer.SkipWhiteSpace(stream);
				if (!stream.EOS)
				{
					stream.Seek(0, SeekOrigin.Begin);
					OperatorCode operatorCode = OperatorCode.NULL;
					WordCollection dest = LexicalAnalyzer.Analyse(stream, LexEndWith.Operator, LexAnalyzeFlag.None);
					try
					{
						operatorCode = LexicalAnalyzer.ReadAssignmentOperator(stream);
					}
					catch (CodeEE)
					{
						text = "解釈できない行です";
						goto IL_01d7;
					}
					if (operatorCode == OperatorCode.Equal)
					{
						if (console != null)
						{
							ParserMediator.Warn("代入演算子に\"==\"が使われています", position, 0);
						}
						operatorCode = OperatorCode.Assignment;
					}
					return new InstructionLine(position, FunctionIdentifier.SETFunction, operatorCode, dest, stream);
				}
				text = "解釈できない行です";
			}
			goto IL_01d7;
			IL_01d7:
			return new InvalidLine(position, text);
		}
		catch (CodeEE codeEE2)
		{
			return new InvalidLine(position, codeEE2.Message);
		}
	}
}
