using System;
using System.Collections.Generic;
using EmueraFramework;
using MinorShift._Library;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.GameProc.Function;
using MinorShift.Emuera.GameView;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal sealed class ErbLoader
{
	private sealed class PPState
	{
		private bool skip;

		private bool done;

		public bool Disabled;

		private Stack<bool> disabledStack = new Stack<bool>();

		private Stack<bool> doneStack = new Stack<bool>();

		private Stack<string> ppMatch = new Stack<string>();

		internal void AddKeyWord(string token, string token2, ScriptPosition position)
		{
			string.IsNullOrEmpty(token2);
			switch (token)
			{
			case "SKIPSTART":
				if (!string.IsNullOrEmpty(token2))
				{
					ParserMediator.Warn(token + "に余分な引数があります", position, 1);
					break;
				}
				if (skip)
				{
					ParserMediator.Warn("[SKIPSTART]が重複して使用されています", position, 1);
					break;
				}
				ppMatch.Push("SKIPEND");
				disabledStack.Push(Disabled);
				doneStack.Push(done);
				skip = true;
				Disabled = true;
				done = false;
				break;
			case "IF_DEBUG":
				if (!string.IsNullOrEmpty(token2))
				{
					ParserMediator.Warn(token + "に余分な引数があります", position, 1);
					break;
				}
				ppMatch.Push("ELSEIF");
				disabledStack.Push(Disabled);
				doneStack.Push(done);
				Disabled = !Program.DebugMode;
				done = !Disabled;
				break;
			case "IF_NDEBUG":
				if (!string.IsNullOrEmpty(token2))
				{
					ParserMediator.Warn(token + "に余分な引数があります", position, 1);
					break;
				}
				ppMatch.Push("ELSEIF");
				disabledStack.Push(Disabled);
				doneStack.Push(done);
				Disabled = Program.DebugMode;
				done = !Disabled;
				break;
			case "IF":
				if (string.IsNullOrEmpty(token2))
				{
					ParserMediator.Warn(token + "に引数がありません", position, 1);
					break;
				}
				ppMatch.Push("ELSEIF");
				disabledStack.Push(Disabled);
				doneStack.Push(done);
				Disabled = GlobalStatic.IdentifierDictionary.GetMacro(token2) == null;
				done = !Disabled;
				break;
			case "ELSEIF":
				if (string.IsNullOrEmpty(token2))
				{
					ParserMediator.Warn(token + "に引数がありません", position, 1);
					break;
				}
				if (ppMatch.Count == 0 || ppMatch.Pop() != "ELSEIF")
				{
					ParserMediator.Warn("不適切な[ELSEIF]です", position, 1);
					break;
				}
				ppMatch.Push("ELSEIF");
				Disabled = done || GlobalStatic.IdentifierDictionary.GetMacro(token2) == null;
				done |= !Disabled;
				break;
			case "ELSE":
				if (!string.IsNullOrEmpty(token2))
				{
					ParserMediator.Warn(token + "に余分な引数があります", position, 1);
					break;
				}
				if (ppMatch.Count == 0 || ppMatch.Pop() != "ELSEIF")
				{
					ParserMediator.Warn("不適切な[ELSE]です", position, 1);
					break;
				}
				ppMatch.Push("ENDIF");
				Disabled = done;
				done = true;
				break;
			case "SKIPEND":
				if (!string.IsNullOrEmpty(token2))
				{
					ParserMediator.Warn(token + "に余分な引数があります", position, 1);
					break;
				}
				if (((ppMatch.Count == 0) ? "" : ppMatch.Pop()) != "SKIPEND")
				{
					ParserMediator.Warn("[SKIPSTART]と対応しない[SKIPEND]です", position, 1);
					break;
				}
				skip = false;
				Disabled = disabledStack.Pop();
				done = doneStack.Pop();
				break;
			case "ENDIF":
			{
				if (!string.IsNullOrEmpty(token2))
				{
					ParserMediator.Warn(token + "に余分な引数があります", position, 1);
					break;
				}
				string text = ((ppMatch.Count == 0) ? "" : ppMatch.Pop());
				if (text != "ENDIF" && text != "ELSEIF")
				{
					ParserMediator.Warn("対応する[IF]のない[ENDIF]です", position, 1);
					break;
				}
				Disabled = disabledStack.Pop();
				done = doneStack.Pop();
				break;
			}
			default:
				ParserMediator.Warn("認識できないプリプロセッサです", position, 1);
				break;
			}
			if (skip)
			{
				Disabled = true;
			}
		}

		internal void FileEnd(ScriptPosition position)
		{
			if (ppMatch.Count != 0)
			{
				string text = ppMatch.Pop();
				if (text == "ELSEIF")
				{
					text = "ENDIF";
				}
				ParserMediator.Warn("[" + text + "]がありません", position, 1);
			}
		}
	}

	private readonly Process parentProcess;

	private readonly ExpressionMediator exm;

	private readonly EmueraConsole output;

	private List<string> ignoredFNFWarningFileList = new List<string>();

	private int ignoredFNFWarningCount;

	private int enabledLineCount;

	private LabelDictionary labelDic;

	private bool noError = true;

	public bool useCallForm;

	public Dictionary<string, long> warningDic = new Dictionary<string, long>();

	public ErbLoader(EmueraConsole main, ExpressionMediator exm, Process proc)
	{
		output = main;
		parentProcess = proc;
		this.exm = exm;
	}

	public bool LoadErbFiles(string erbDir, bool displayReport, LabelDictionary labelDictionary)
	{
		labelDic = labelDictionary;
		labelDic.Initialized = false;
		List<KeyValuePair<string, string>> files = Config.GetFiles(erbDir, "*.ERB");
		FileLog.Info("ReadFile", "All ERB Files Count : " + files.Count);
		List<string> list = new List<string>();
		noError = true;
		_ = WinmmTimer.TickCount;
		try
		{
			labelDic.RemoveAll();
            for (int i = 0; i < files.Count; i++)
            {
                string key = files[i].Key;
                string value = files[i].Value;

                // 현재 ERB 파일 읽기 진행 상황을 Android 화면과 로그에 표시한다.
                EraAndroid.EraAndroidFileProvider.ReportStatus("ERB 읽기 중: " + (i + 1) + " / " + files.Count + "\n" + key);
                FileLog.Info("ERB Load", "ERB 읽기 중: " + (i + 1) + " / " + files.Count + " / " + key);

                if (displayReport)
                {
                    output.PrintSystemLine(key + "読み込み中・・・");
                }
                loadErb(value, key, list);
            }
            ParserMediator.FlushWarningList();
			if (displayReport)
			{
				output.PrintSystemLine("ユーザー定義関数のリストを構築中・・・");
			}
			setLabelsArg();
			ParserMediator.FlushWarningList();
			labelDic.Initialized = true;
			if (displayReport)
			{
				output.PrintSystemLine("スクリプトの構文チェック中・・・");
			}
			checkScript();
			ParserMediator.FlushWarningList();
			if (displayReport)
			{
				output.PrintSystemLine("ロード完了");
			}
		}
		catch (Exception ex)
		{
			ParserMediator.FlushWarningList();
			output.PrintError("予期しないエラーが発生しました:" + Program.ExeName);
			output.PrintError(ex.GetType().ToString() + ":" + ex.Message);
			return false;
		}
		finally
		{
			parentProcess.scaningLine = null;
		}
		list.Clear();
		return noError;
	}

	public bool loadErbs(List<string> path, LabelDictionary labelDictionary)
	{
		List<string> list = new List<string>();
		noError = true;
		labelDic = labelDictionary;
		labelDic.Initialized = false;
		foreach (string item in path)
		{
			string text = ((!item.StartsWith(Program.ErbDir, StringComparison.OrdinalIgnoreCase) || Program.AnalysisMode) ? item : item.Substring(Program.ErbDir.Length));
			if (Program.AnalysisMode)
			{
				output.PrintSystemLine(text + "読み込み中・・・");
			}
			loadErb(item, text, list);
		}
		if (Program.AnalysisMode)
		{
			output.NewLine();
		}
		ParserMediator.FlushWarningList();
		setLabelsArg();
		ParserMediator.FlushWarningList();
		labelDic.Initialized = true;
		checkScript();
		ParserMediator.FlushWarningList();
		parentProcess.scaningLine = null;
		list.Clear();
		return noError;
	}

	private void loadErb(string filepath, string filename, List<string> isOnlyEvent)
	{
		labelDic.AddFilename(filename);
		EraStreamReader eraStreamReader = new EraStreamReader(Config.UseRenameFile && ParserMediator.RenameDic != null);
		if (!eraStreamReader.Open(filepath, filename))
		{
			output.PrintError(eraStreamReader.Filename + "のオープンに失敗しました");
			return;
		}
		try
		{
			PPState pPState = new PPState();
			LogicalLine logicalLine = new NullLine();
			LogicalLine logicalLine2 = new NullLine();
			FunctionLabelLine functionLabelLine = null;
			StringStream stringStream = null;
			string text = null;
			ScriptPosition scriptPosition = null;
			int num = 0;
			if (Program.AnalysisMode)
			{
				output.PrintSystemLine("\u3000");
			}
			while ((stringStream = eraStreamReader.ReadEnabledLine()) != null)
			{
				text = stringStream.RowString;
				scriptPosition = new ScriptPosition(eraStreamReader.Filename, eraStreamReader.LineNo, text);
				if (stringStream.Current == '[' && stringStream.Next != '[')
				{
					stringStream.ShiftNext();
					string text2 = LexicalAnalyzer.ReadSingleIdentifier(stringStream);
					LexicalAnalyzer.SkipWhiteSpace(stringStream);
					string token = LexicalAnalyzer.ReadSingleIdentifier(stringStream);
					if (string.IsNullOrEmpty(text2) || stringStream.Current != ']')
					{
						ParserMediator.Warn("[]の使い方が不正です", scriptPosition, 1);
					}
					lock (this)
					{
						pPState.AddKeyWord(text2, token, scriptPosition);
					}
					stringStream.ShiftNext();
					if (!stringStream.EOS)
					{
						ParserMediator.Warn("[" + text2 + "]の後ろは無視されます。", scriptPosition, 1);
					}
				}
				else
				{
					if (pPState.Disabled)
					{
						continue;
					}
					if (stringStream.Current == '#')
					{
						if (logicalLine2 == null || !(logicalLine2 is FunctionLabelLine))
						{
							ParserMediator.Warn("関数宣言の直後以外で#行が使われています", scriptPosition, 1);
						}
						else if (!LogicalLineParser.ParseSharpLine((FunctionLabelLine)logicalLine2, stringStream, scriptPosition, isOnlyEvent))
						{
							noError = false;
						}
						continue;
					}
					if (stringStream.Current == '$' || stringStream.Current == '@')
					{
						bool num2 = stringStream.Current == '@';
						logicalLine = LogicalLineParser.ParseLabelLine(stringStream, scriptPosition, output);
						if (num2)
						{
							FunctionLabelLine functionLabelLine2 = (FunctionLabelLine)logicalLine;
							functionLabelLine = functionLabelLine2;
							if (functionLabelLine2 is InvalidLabelLine)
							{
								noError = false;
								ParserMediator.Warn(logicalLine.ErrMes, scriptPosition, 2);
								labelDic.AddInvalidLabel(functionLabelLine2);
							}
							else
							{
								labelDic.AddLabel(functionLabelLine2);
								if (!functionLabelLine2.IsEvent && (Config.WarnNormalFunctionOverloading || Program.AnalysisMode))
								{
									FunctionLabelLine sameNameLabel = labelDic.GetSameNameLabel(functionLabelLine2);
									if (sameNameLabel != null)
									{
										ParserMediator.Warn("関数@" + functionLabelLine2.LabelName + "は既に定義(" + sameNameLabel.Position.Filename + "の" + sameNameLabel.Position.LineNo + "行目)されています", scriptPosition, 1);
										num = -1;
									}
								}
								num++;
								if (Program.AnalysisMode && Config.PrintCPerLine > 0 && num % Config.PrintCPerLine == 0)
								{
									output.NewLine();
									output.PrintSystemLine("\u3000");
								}
							}
						}
						else if (logicalLine is GotoLabelLine)
						{
							GotoLabelLine gotoLabelLine = (GotoLabelLine)logicalLine;
							gotoLabelLine.ParentLabelLine = functionLabelLine;
							if (functionLabelLine != null && !labelDic.AddLabelDollar(gotoLabelLine))
							{
								ScriptPosition position = labelDic.GetLabelDollar(gotoLabelLine.LabelName, functionLabelLine).Position;
								ParserMediator.Warn("ラベル名$" + gotoLabelLine.LabelName + "は既に同じ関数内(" + position.Filename + "の" + position.LineNo + "行目)で使用されています", scriptPosition, 2);
							}
						}
						if (logicalLine is InvalidLine)
						{
							noError = false;
							ParserMediator.Warn(logicalLine.ErrMes, scriptPosition, 2);
						}
					}
					else
					{
						logicalLine = LogicalLineParser.ParseLine(stringStream, scriptPosition, output);
						if (logicalLine == null)
						{
							continue;
						}
						if (logicalLine is InvalidLine)
						{
							noError = false;
							ParserMediator.Warn(logicalLine.ErrMes, scriptPosition, 2);
						}
					}
					if (functionLabelLine == null)
					{
						ParserMediator.Warn("関数が定義されるより前に行があります", scriptPosition, 1);
					}
					logicalLine.ParentLabelLine = functionLabelLine;
					logicalLine2 = addLine(logicalLine, logicalLine2);
				}
			}
			addLine(new NullLine(), logicalLine2);
			scriptPosition = new ScriptPosition(eraStreamReader.Filename, -1, null);
			pPState.FileEnd(scriptPosition);
		}
		finally
		{
			eraStreamReader.Close();
		}
	}

	private LogicalLine addLine(LogicalLine nextLine, LogicalLine lastLine)
	{
		if (nextLine == null)
		{
			return null;
		}
		enabledLineCount++;
		lastLine.NextLine = nextLine;
		return nextLine;
	}

	private void setLabelsArg()
	{
		foreach (FunctionLabelLine allLabel in labelDic.GetAllLabels(getInvalidList: false))
		{
			try
			{
				if (allLabel.Arg == null)
				{
					parentProcess.scaningLine = allLabel;
					parseLabel(allLabel);
				}
			}
			catch (Exception ex)
			{
				string text = ex.Message;
				if (!(ex is EmueraException))
				{
					text = ex.GetType().ToString() + ":" + text;
				}
				ParserMediator.Warn("関数@" + allLabel.LabelName + " の引数のエラー:" + text, allLabel, 2, isError: true, isBackComp: false);
				allLabel.ErrMes = "ロード時に解析に失敗した関数が呼び出されました";
				allLabel.IsError = true;
			}
			finally
			{
				parentProcess.scaningLine = null;
			}
		}
		labelDic.SortLabels();
	}

	private void parseLabel(FunctionLabelLine label)
	{
		WordCollection wordCollection = label.PopRowArgs();
		string text = null;
		SingleTerm[] array = new SingleTerm[0];
		VariableTerm[] array2 = new VariableTerm[0];
		SingleTerm[] array3 = new SingleTerm[0];
		int num = -1;
		int num2 = -1;
		if (label.IsEvent)
		{
			if (!wordCollection.EOL)
			{
				ParserMediator.Warn("イベント関数@" + label.LabelName + " に引数は設定できません", label, 2, isError: true, isBackComp: false);
			}
			label.Arg = array2;
			label.Def = array3;
			label.ArgLength = -1;
			label.ArgsLength = -1;
			return;
		}
		if (wordCollection.EOL)
		{
			goto IL_03d1;
		}
		if (label.IsSystem)
		{
			ParserMediator.Warn("システム関数@" + label.LabelName + " に引数が設定されています", label, 1, isError: false, isBackComp: false);
		}
		SymbolWord symbolWord = wordCollection.Current as SymbolWord;
		wordCollection.ShiftNext();
		if (symbolWord == null)
		{
			text = "引数の書式が間違っています";
		}
		else
		{
			if (symbolWord.Type != '[')
			{
				goto IL_0173;
			}
			IOperandTerm[] array4 = ExpressionParser.ReduceArguments(wordCollection, ArgsEndWith.RightBracket, isDefine: false);
			if (array4.Length == 0)
			{
				text = "関数定義の[]内の引数は空にできません";
			}
			else
			{
				array = new SingleTerm[array4.Length];
				int num3 = 0;
				while (num3 < array4.Length)
				{
					if (array4[num3] == null)
					{
						goto IL_0102;
					}
					IOperandTerm operandTerm = array4[num3].Restructure(exm);
					array[num3] = operandTerm as SingleTerm;
					if (array[num3] != null)
					{
						num3++;
						continue;
					}
					goto IL_0130;
				}
				symbolWord = wordCollection.Current as SymbolWord;
				if (wordCollection.EOL || symbolWord != null)
				{
					wordCollection.ShiftNext();
					goto IL_0173;
				}
				text = "引数の書式が間違っています";
			}
		}
		goto IL_0401;
		IL_0102:
		text = "関数定義の引数は省略できません";
		goto IL_0401;
		IL_0173:
		VariableTerm variableTerm;
		if (!wordCollection.EOL)
		{
			IOperandTerm[] array5 = null;
			if (symbolWord.Type == ',')
			{
				array5 = ExpressionParser.ReduceArguments(wordCollection, ArgsEndWith.EoL, isDefine: true);
			}
			else
			{
				if (symbolWord.Type != '(')
				{
					text = "引数の書式が間違っています";
					goto IL_0401;
				}
				array5 = ExpressionParser.ReduceArguments(wordCollection, ArgsEndWith.RightParenthesis, isDefine: true);
			}
			int num4 = array5.Length / 2;
			array2 = new VariableTerm[num4];
			array3 = new SingleTerm[num4];
			SingleTerm singleTerm;
			for (int i = 0; i < num4; array2[i] = variableTerm, array3[i] = singleTerm, i++)
			{
				variableTerm = null;
				singleTerm = null;
				IOperandTerm operandTerm2 = array5[i * 2];
				variableTerm = operandTerm2.Restructure(exm) as VariableTerm;
				if (variableTerm == null || variableTerm.Identifier.IsConst)
				{
					goto IL_0210;
				}
				if (!variableTerm.Identifier.IsReference)
				{
					if (variableTerm is VariableNoArgTerm)
					{
						goto IL_0232;
					}
					if (!variableTerm.isAllConst)
					{
						goto IL_025c;
					}
				}
				for (int j = 0; j < i; j++)
				{
				}
				if (variableTerm.Identifier.Code == VariableCode.ARG)
				{
					if (num < variableTerm.getEl1forArg + 1)
					{
						num = variableTerm.getEl1forArg + 1;
					}
				}
				else if (variableTerm.Identifier.Code == VariableCode.ARGS && num2 < variableTerm.getEl1forArg + 1)
				{
					num2 = variableTerm.getEl1forArg + 1;
				}
				bool flag = variableTerm.Identifier.Code == VariableCode.ARG || variableTerm.Identifier.Code == VariableCode.ARGS || variableTerm.Identifier.IsPrivate;
				operandTerm2 = array5[i * 2 + 1];
				if (operandTerm2 is NullTerm)
				{
					if (flag)
					{
						singleTerm = ((!(variableTerm.GetOperandType() == typeof(long))) ? new SingleTerm("") : new SingleTerm(0L));
					}
					continue;
				}
				singleTerm = operandTerm2.Restructure(exm) as SingleTerm;
				if (singleTerm == null)
				{
					goto IL_036b;
				}
				if (!flag)
				{
					goto IL_037a;
				}
				if (variableTerm.Identifier.IsReference)
				{
					goto IL_0390;
				}
				if (!(variableTerm.GetOperandType() != singleTerm.GetOperandType()))
				{
					continue;
				}
				goto IL_03ad;
			}
		}
		goto IL_03d1;
		IL_0210:
		text = "関数定義の引数には代入可能な変数を指定してください";
		goto IL_0401;
		IL_03d1:
		if (!wordCollection.EOL)
		{
			text = "引数の書式が間違っています";
			goto IL_0401;
		}
		label.Arg = array2;
		label.Def = array3;
		label.ArgLength = num;
		label.ArgsLength = num2;
		return;
		IL_0232:
		text = "関数定義の参照型でない引数\"" + variableTerm.Identifier.Name + "\"に添え字が指定されていません";
		goto IL_0401;
		IL_025c:
		text = "関数定義の引数の添え字には定数を指定してください";
		goto IL_0401;
		IL_0390:
		text = "参照渡しの引数に初期値は定義できません";
		goto IL_0401;
		IL_0401:
		ParserMediator.Warn("関数@" + label.LabelName + " の引数のエラー:" + text, label, 2, isError: true, isBackComp: false);
		return;
		IL_037a:
		text = "引数の初期値を定義できるのは\"ARG\"、\"ARGS\"またはプライベート変数のみです";
		goto IL_0401;
		IL_036b:
		text = "引数の初期値には定数のみを指定できます";
		goto IL_0401;
		IL_03ad:
		text = "引数の型と初期値の型が一致していません";
		goto IL_0401;
		IL_0130:
		text = "関数定義の[]内の引数は定数のみ指定できます";
		goto IL_0401;
	}

	private void checkScript()
	{
		int num = 0;
		int num2 = -1;
		List<FunctionLabelLine> allLabels = labelDic.GetAllLabels(getInvalidList: true);
		int num3;
		do
		{
			num2++;
			num3 = 0;
			foreach (FunctionLabelLine item in allLabels)
			{
				if (item.Depth == num2)
				{
					num++;
					num3++;
					checkFunctionWithCatch(item);
				}
			}
		}
		while (num3 != 0);
		num2 = -1;
		List<string> list = new List<string>();
		int num4 = 0;
		bool flag = false;
		DisplayWarningFlag functionNotCalledWarning = Config.FunctionNotCalledWarning;
		if ((uint)functionNotCalledWarning <= 1u)
		{
			flag = true;
		}
		if (useCallForm)
		{
			if (Program.AnalysisMode)
			{
				output.PrintSystemLine("CALLFORM系命令が使われたため、呼び出されない関数のチェックは行われません。");
			}
			foreach (FunctionLabelLine item2 in allLabels)
			{
				if (item2.Depth == num2)
				{
					checkFunctionWithCatch(item2);
				}
			}
		}
		else
		{
			bool ignoreUncalledFunction = Config.IgnoreUncalledFunction;
			foreach (FunctionLabelLine item3 in allLabels)
			{
				if (item3.Depth != num2)
				{
					continue;
				}
				if (Program.AnalysisMode)
				{
					checkFunctionWithCatch(item3);
				}
				bool flag2 = false;
				if (functionNotCalledWarning == DisplayWarningFlag.ONCE)
				{
					string text = item3.Position.Filename.ToUpper();
					if (!string.IsNullOrEmpty(text))
					{
						if (list.Contains(text))
						{
							flag2 = true;
						}
						else
						{
							flag2 = false;
							list.Add(text);
						}
					}
				}
				if (flag || flag2)
				{
					num4++;
				}
				else
				{
					ParserMediator.Warn("関数@" + item3.LabelName + "は定義されていますが一度も呼び出されません", item3, 1, isError: false, isBackComp: false);
				}
				if (!ignoreUncalledFunction)
				{
					checkFunctionWithCatch(item3);
				}
				else if (!(item3.NextLine is NullLine) && !(item3.NextLine is FunctionLabelLine) && !item3.NextLine.IsError)
				{
					item3.NextLine.IsError = true;
					item3.NextLine.ErrMes = "呼び出されないはずの関数が呼ばれた";
				}
			}
		}
		if (Program.AnalysisMode && (warningDic.Keys.Count > 0 || GlobalStatic.tempDic.Keys.Count > 0))
		{
			output.PrintError("・定義が見つからなかった関数: 他のファイルで定義されている場合はこの警告は無視できます");
			if (warningDic.Keys.Count > 0)
			{
				output.PrintError("\u3000○一般関数:");
				foreach (string key in warningDic.Keys)
				{
					output.PrintError("\u3000\u3000" + key + ": " + warningDic[key] + "回");
				}
			}
			if (GlobalStatic.tempDic.Keys.Count > 0)
			{
				output.PrintError("\u3000○文中関数:");
				foreach (string key2 in GlobalStatic.tempDic.Keys)
				{
					output.PrintError("\u3000\u3000" + key2 + ": " + GlobalStatic.tempDic[key2] + "回");
				}
			}
		}
		else
		{
			if (num4 > 0 && Config.DisplayWarningLevel <= 1 && functionNotCalledWarning != DisplayWarningFlag.IGNORE)
			{
				output.PrintError($"警告Lv1:定義された関数が一度も呼び出されていない事に関する警告を{num4}件無視しました");
			}
			if (ignoredFNFWarningCount > 0 && Config.DisplayWarningLevel <= 2 && functionNotCalledWarning != DisplayWarningFlag.IGNORE)
			{
				output.PrintError($"警告Lv2:定義されていない関数を呼び出した事に関する警告を{ignoredFNFWarningCount}件無視しました");
			}
		}
		ParserMediator.FlushWarningList();
		if (Config.DisplayReport)
		{
			output.PrintError($"非コメント行数:{enabledLineCount}, 全関数合計:{labelDic.Count}, 被呼出関数合計:{num}");
		}
		if (!Config.AllowFunctionOverloading || !Config.WarnFunctionOverloading)
		{
			return;
		}
		List<string> overloadedList = GlobalStatic.IdentifierDictionary.GetOverloadedList(labelDic);
		if (overloadedList.Count <= 0)
		{
			return;
		}
		output.NewLine();
		output.PrintError("＊＊＊＊＊警告＊＊＊＊＊");
		foreach (string item4 in overloadedList)
		{
			output.PrintSystemLine("  システム関数\"" + item4 + "\"がユーザー定義関数によって上書きされています");
		}
		output.PrintSystemLine("  上記の関数を利用するスクリプトは意図通りに動かない可能性があります");
		output.NewLine();
		output.PrintSystemLine("  ※この警告は該当する式中関数を利用しているEmuera専用スクリプト向けの警告です。");
		output.PrintSystemLine("  eramaker用のスクリプトの動作には影響しません。");
		output.PrintSystemLine("  今後この警告が不要ならばコンフィグの「システム関数が上書きされたとき警告を表示する」をOFFにして下さい。");
		output.PrintSystemLine("＊＊＊＊＊＊＊＊＊＊＊＊");
	}

	private void printFunctionNotFoundWarning(string str, LogicalLine line, int level, bool isError)
	{
		if (Program.AnalysisMode)
		{
			if (warningDic.ContainsKey(str))
			{
				warningDic[str]++;
			}
			else
			{
				warningDic.Add(str, 1L);
			}
			return;
		}
		if (isError)
		{
			line.IsError = true;
			line.ErrMes = str;
		}
		if (level < Config.DisplayWarningLevel)
		{
			return;
		}
		bool flag = false;
		switch (Config.FunctionNotFoundWarning)
		{
		case DisplayWarningFlag.IGNORE:
			flag = true;
			break;
		case DisplayWarningFlag.DISPLAY:
			flag = false;
			break;
		case DisplayWarningFlag.ONCE:
		{
			string text = line.Position.Filename.ToUpper();
			if (!string.IsNullOrEmpty(text))
			{
				if (ignoredFNFWarningFileList.Contains(text))
				{
					flag = true;
					break;
				}
				flag = false;
				ignoredFNFWarningFileList.Add(text);
			}
			break;
		}
		}
		if (flag && !Program.AnalysisMode)
		{
			ignoredFNFWarningCount++;
		}
		else
		{
			ParserMediator.Warn(str, line, level, isError, isBackComp: false);
		}
	}

	private void checkFunctionWithCatch(FunctionLabelLine label)
	{
		try
		{
			label.Position.Filename.ToUpper();
			setArgument(label);
			nestCheck(label);
			setJumpTo(label);
		}
		catch (Exception ex)
		{
			string text = ((ex is EmueraException) ? ex.Message : (ex.GetType().ToString() + ":" + ex.Message));
			ParserMediator.Warn("@" + label.LabelName + " の解析中にエラー:" + text, label, 2, isError: true, isBackComp: false, (!(ex is EmueraException)) ? ex.StackTrace : null);
			label.ErrMes = "ロード時に解析に失敗した関数が呼び出されました";
		}
		finally
		{
			parentProcess.scaningLine = null;
		}
	}

	private void setArgument(FunctionLabelLine label)
	{
		LogicalLine logicalLine = label;
		bool isMethod = label.IsMethod;
		while (true)
		{
			logicalLine = logicalLine.NextLine;
			parentProcess.scaningLine = logicalLine;
			if (!(logicalLine is InstructionLine instructionLine))
			{
				if (logicalLine is NullLine || logicalLine is FunctionLabelLine)
				{
					break;
				}
			}
			else if (isMethod && !instructionLine.Function.IsMethodSafe())
			{
				ParserMediator.Warn(instructionLine.Function.Name + "命令は#FUNCTION中で使うことはできません", logicalLine, 2, isError: true, isBackComp: false);
			}
			else if (Config.NeedReduceArgumentOnLoad || Program.AnalysisMode || instructionLine.Function.IsForceSetArg())
			{
				ArgumentParser.SetArgumentTo(instructionLine);
			}
		}
	}

	private void nestCheck(FunctionLabelLine label)
	{
		LogicalLine logicalLine = label;
		List<InstructionLine> list = new List<InstructionLine>();
		Stack<InstructionLine> stack = new Stack<InstructionLine>();
		Stack<InstructionLine> stack2 = new Stack<InstructionLine>();
		InstructionLine instructionLine = null;
		while (true)
		{
			logicalLine = logicalLine.NextLine;
			parentProcess.scaningLine = logicalLine;
			if (logicalLine is NullLine || logicalLine is FunctionLabelLine)
			{
				break;
			}
			if (!(logicalLine is InstructionLine))
			{
				if (logicalLine is GotoLabelLine)
				{
					InstructionLine instructionLine2 = ((stack.Count == 0) ? null : stack.Peek());
					if (instructionLine2 != null && (instructionLine2.FunctionCode == FunctionCode.PRINTDATA || instructionLine2.FunctionCode == FunctionCode.PRINTDATAL || instructionLine2.FunctionCode == FunctionCode.PRINTDATAW || instructionLine2.FunctionCode == FunctionCode.PRINTDATAD || instructionLine2.FunctionCode == FunctionCode.PRINTDATADL || instructionLine2.FunctionCode == FunctionCode.PRINTDATADW || instructionLine2.FunctionCode == FunctionCode.PRINTDATAK || instructionLine2.FunctionCode == FunctionCode.PRINTDATAKL || instructionLine2.FunctionCode == FunctionCode.PRINTDATAKW || instructionLine2.FunctionCode == FunctionCode.STRDATA || instructionLine2.FunctionCode == FunctionCode.DATALIST || instructionLine2.FunctionCode == FunctionCode.TRYCALLLIST || instructionLine2.FunctionCode == FunctionCode.TRYJUMPLIST || instructionLine2.FunctionCode == FunctionCode.TRYGOTOLIST))
					{
						ParserMediator.Warn(instructionLine2.Function.Name + "構文中に$ラベルを定義することはできません", logicalLine, 2, isError: true, isBackComp: false);
					}
				}
				continue;
			}
			InstructionLine instructionLine3 = (InstructionLine)logicalLine;
			instructionLine = null;
			InstructionLine instructionLine4 = ((stack.Count == 0) ? null : stack.Peek());
			if (instructionLine4 != null)
			{
				if (instructionLine4.Function.IsPrintData() || instructionLine4.FunctionCode == FunctionCode.STRDATA)
				{
					if (instructionLine3.FunctionCode != FunctionCode.DATA && instructionLine3.FunctionCode != FunctionCode.DATAFORM && instructionLine3.FunctionCode != FunctionCode.DATALIST && instructionLine3.FunctionCode != FunctionCode.ENDLIST && instructionLine3.FunctionCode != FunctionCode.ENDDATA)
					{
						ParserMediator.Warn(instructionLine4.Function.Name + "構文に使用できない命令'" + instructionLine3.Function.Name + "'が含まれています", instructionLine3, 2, isError: true, isBackComp: false);
						continue;
					}
				}
				else if (instructionLine4.FunctionCode == FunctionCode.DATALIST)
				{
					if (instructionLine3.FunctionCode != FunctionCode.DATA && instructionLine3.FunctionCode != FunctionCode.DATAFORM && instructionLine3.FunctionCode != FunctionCode.ENDLIST)
					{
						ParserMediator.Warn("DATALIST構文に使用できない命令'" + instructionLine3.Function.Name + "'が含まれています", instructionLine3, 2, isError: true, isBackComp: false);
						continue;
					}
				}
				else if (instructionLine4.FunctionCode == FunctionCode.TRYCALLLIST || instructionLine4.FunctionCode == FunctionCode.TRYJUMPLIST || instructionLine4.FunctionCode == FunctionCode.TRYGOTOLIST)
				{
					if (instructionLine3.FunctionCode != FunctionCode.FUNC && instructionLine3.FunctionCode != FunctionCode.ENDFUNC)
					{
						ParserMediator.Warn(instructionLine4.Function.Name + "構文に使用できない命令'" + instructionLine3.Function.Name + "'が含まれています", instructionLine3, 2, isError: true, isBackComp: false);
						continue;
					}
				}
				else if (instructionLine4.FunctionCode == FunctionCode.SELECTCASE && instructionLine4.IfCaseList.Count == 0 && instructionLine3.FunctionCode != FunctionCode.CASE && instructionLine3.FunctionCode != FunctionCode.CASEELSE && instructionLine3.FunctionCode != FunctionCode.ENDSELECT)
				{
					ParserMediator.Warn("SELECTCASE構文の分岐の外に命令'" + instructionLine3.Function.Name + "'が含まれています", instructionLine3, 2, isError: true, isBackComp: false);
					continue;
				}
			}
			switch (instructionLine3.FunctionCode)
			{
			case FunctionCode.REPEAT:
				foreach (InstructionLine item in stack)
				{
					if (item.FunctionCode == FunctionCode.REPEAT)
					{
						ParserMediator.Warn("REPEAT文が入れ子にされています", instructionLine3, 2, isError: true, isBackComp: false);
						break;
					}
				}
				if (!instructionLine3.IsError)
				{
					stack.Push(instructionLine3);
				}
				break;
			case FunctionCode.IF:
				stack.Push(instructionLine3);
				instructionLine3.IfCaseList = new List<InstructionLine>();
				instructionLine3.IfCaseList.Add(instructionLine3);
				break;
			case FunctionCode.SELECTCASE:
				stack.Push(instructionLine3);
				instructionLine3.IfCaseList = new List<InstructionLine>();
				stack2.Push(instructionLine3);
				break;
			case FunctionCode.TRYCJUMP:
			case FunctionCode.TRYCCALL:
			case FunctionCode.TRYCGOTO:
			case FunctionCode.TRYCJUMPFORM:
			case FunctionCode.TRYCCALLFORM:
			case FunctionCode.TRYCGOTOFORM:
			case FunctionCode.FOR:
			case FunctionCode.WHILE:
			case FunctionCode.DO:
				stack.Push(instructionLine3);
				break;
			case FunctionCode.CONTINUE:
			case FunctionCode.BREAK:
			{
				InstructionLine[] array2 = stack.ToArray();
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j].FunctionCode == FunctionCode.REPEAT || array2[j].FunctionCode == FunctionCode.FOR || array2[j].FunctionCode == FunctionCode.WHILE || array2[j].FunctionCode == FunctionCode.DO)
					{
						instructionLine = array2[j];
						break;
					}
				}
				if (instructionLine == null)
				{
					ParserMediator.Warn("REPEAT, FOR, WHILE, DOの中以外で" + instructionLine3.Function.Name + "文が使われました", instructionLine3, 2, isError: true, isBackComp: false);
				}
				else
				{
					instructionLine3.JumpTo = instructionLine;
				}
				break;
			}
			case FunctionCode.ELSE:
			case FunctionCode.ELSEIF:
			{
				InstructionLine instructionLine7 = ((stack.Count == 0) ? null : stack.Peek());
				if (instructionLine7 == null || instructionLine7.FunctionCode != FunctionCode.IF)
				{
					ParserMediator.Warn("IF～ENDIFの外で" + instructionLine3.Function.Name + "文が使われました", instructionLine3, 2, isError: true, isBackComp: false);
					break;
				}
				if (instructionLine7.IfCaseList[instructionLine7.IfCaseList.Count - 1].FunctionCode == FunctionCode.ELSE)
				{
					ParserMediator.Warn("ELSE文より後で" + instructionLine3.Function.Name + "文が使われました", instructionLine3, 1, isError: false, isBackComp: false);
				}
				instructionLine7.IfCaseList.Add(instructionLine3);
				break;
			}
			case FunctionCode.ENDIF:
			{
				InstructionLine instructionLine6 = ((stack.Count == 0) ? null : stack.Peek());
				if (instructionLine6 == null || instructionLine6.FunctionCode != FunctionCode.IF)
				{
					ParserMediator.Warn("対応するIFの無いENDIF文です", instructionLine3, 2, isError: true, isBackComp: false);
					break;
				}
				foreach (InstructionLine ifCase in instructionLine6.IfCaseList)
				{
					ifCase.JumpTo = instructionLine3;
				}
				stack.Pop();
				break;
			}
			case FunctionCode.CASE:
			case FunctionCode.CASEELSE:
			{
				InstructionLine instructionLine13 = ((stack.Count == 0) ? null : stack.Peek());
				if (instructionLine13 == null || (instructionLine13.FunctionCode != FunctionCode.SELECTCASE && stack2.Count == 0))
				{
					ParserMediator.Warn("SELECTCASE～ENDSELECTの外で" + instructionLine3.Function.Name + "文が使われました", instructionLine3, 2, isError: true, isBackComp: false);
				}
				else if (instructionLine13.FunctionCode != FunctionCode.SELECTCASE && stack2.Count > 0)
				{
					do
					{
						ParserMediator.Warn(instructionLine13.Function.Name + "文に対応する" + FunctionIdentifier.getMatchFunction(instructionLine13.FunctionCode) + "がない状態で" + instructionLine3.Function.Name + "文に到達しました", instructionLine3, 2, isError: true, isBackComp: false);
						stack.Pop();
						instructionLine13 = ((stack.Count == 0) ? null : stack.Peek());
					}
					while (instructionLine13 != null && instructionLine13.FunctionCode != FunctionCode.SELECTCASE);
				}
				else
				{
					if (instructionLine13.IfCaseList.Count > 0 && instructionLine13.IfCaseList[instructionLine13.IfCaseList.Count - 1].FunctionCode == FunctionCode.CASEELSE)
					{
						ParserMediator.Warn("CASEELSE文より後で" + instructionLine3.Function.Name + "文が使われました", instructionLine3, 1, isError: false, isBackComp: false);
					}
					instructionLine13.IfCaseList.Add(instructionLine3);
				}
				break;
			}
			case FunctionCode.ENDSELECT:
			{
				InstructionLine instructionLine9 = ((stack.Count == 0) ? null : stack.Peek());
				if (instructionLine9 == null || (instructionLine9.FunctionCode != FunctionCode.SELECTCASE && stack2.Count == 0))
				{
					ParserMediator.Warn("対応するSELECTCASEの無いENDSELECT文です", instructionLine3, 2, isError: true, isBackComp: false);
					break;
				}
				if (instructionLine9.FunctionCode != FunctionCode.SELECTCASE && stack2.Count > 0)
				{
					do
					{
						ParserMediator.Warn(instructionLine9.Function.Name + "文に対応する" + FunctionIdentifier.getMatchFunction(instructionLine9.FunctionCode) + "がない状態で" + instructionLine3.Function.Name + "文に到達しました", instructionLine3, 2, isError: true, isBackComp: false);
						stack.Pop();
						instructionLine9 = ((stack.Count == 0) ? null : stack.Peek());
					}
					while (instructionLine9 != null && instructionLine9.FunctionCode != FunctionCode.SELECTCASE);
					stack2.Pop();
					stack.Pop();
					break;
				}
				stack.Pop();
				stack2.Pop();
				instructionLine9.JumpTo = instructionLine3;
				if (instructionLine9.IsError)
				{
					break;
				}
				IOperandTerm term = ((ExpressionArgument)instructionLine9.Argument).Term;
				if (term == null)
				{
					ParserMediator.Warn("SELECTCASEの引数がありません", instructionLine9, 2, isError: true, isBackComp: false);
					break;
				}
				foreach (InstructionLine ifCase2 in instructionLine9.IfCaseList)
				{
					ifCase2.JumpTo = instructionLine3;
					if (ifCase2.IsError || ifCase2.FunctionCode == FunctionCode.CASEELSE)
					{
						continue;
					}
					CaseExpression[] caseExps = ((CaseArgument)ifCase2.Argument).CaseExps;
					if (caseExps.Length == 0)
					{
						ParserMediator.Warn("CASEの引数がありません", ifCase2, 2, isError: true, isBackComp: false);
					}
					CaseExpression[] array = caseExps;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].GetOperandType() != term.GetOperandType())
						{
							ParserMediator.Warn("CASEの引数の型がSELECTCASEと一致しません", ifCase2, 2, isError: true, isBackComp: false);
						}
					}
				}
				break;
			}
			case FunctionCode.REND:
			case FunctionCode.NEXT:
			case FunctionCode.WEND:
			case FunctionCode.LOOP:
			{
				FunctionCode parentFunc = FunctionIdentifier.getParentFunc(instructionLine3.FunctionCode);
				if (stack.Count == 0 || stack.Peek().FunctionCode != parentFunc)
				{
					ParserMediator.Warn("対応する" + parentFunc.ToString() + "の無い" + instructionLine3.Function.Name + "文です", instructionLine3, 2, isError: true, isBackComp: false);
				}
				else
				{
					instructionLine = (InstructionLine)(instructionLine3.JumpTo = stack.Pop());
					instructionLine.JumpTo = instructionLine3;
				}
				break;
			}
			case FunctionCode.CATCH:
				instructionLine = ((stack.Count == 0) ? null : stack.Peek());
				if (instructionLine == null || (instructionLine.FunctionCode != FunctionCode.TRYCGOTO && instructionLine.FunctionCode != FunctionCode.TRYCCALL && instructionLine.FunctionCode != FunctionCode.TRYCJUMP && instructionLine.FunctionCode != FunctionCode.TRYCGOTOFORM && instructionLine.FunctionCode != FunctionCode.TRYCCALLFORM && instructionLine.FunctionCode != FunctionCode.TRYCJUMPFORM))
				{
					ParserMediator.Warn("対応するTRYC系命令がありません", instructionLine3, 2, isError: true, isBackComp: false);
					break;
				}
				instructionLine = stack.Pop();
				instructionLine.JumpToEndCatch = instructionLine3;
				stack.Push(instructionLine3);
				break;
			case FunctionCode.ENDCATCH:
				if (stack.Count == 0 || stack.Peek().FunctionCode != FunctionCode.CATCH)
				{
					ParserMediator.Warn("対応するCATCHのないENDCATCHです", instructionLine3, 2, isError: true, isBackComp: false);
					break;
				}
				instructionLine = stack.Pop();
				instructionLine.JumpToEndCatch = instructionLine3;
				break;
			case FunctionCode.PRINTDATA:
			case FunctionCode.PRINTDATAL:
			case FunctionCode.PRINTDATAW:
			case FunctionCode.PRINTDATAK:
			case FunctionCode.PRINTDATAKL:
			case FunctionCode.PRINTDATAKW:
			case FunctionCode.PRINTDATAD:
			case FunctionCode.PRINTDATADL:
			case FunctionCode.PRINTDATADW:
				foreach (InstructionLine item2 in stack)
				{
					if (item2.Function.IsPrintData())
					{
						ParserMediator.Warn("PRINTDATA系命令が入れ子にされています", instructionLine3, 2, isError: true, isBackComp: false);
						break;
					}
					if (item2.FunctionCode == FunctionCode.STRDATA)
					{
						ParserMediator.Warn("PRINTDATA系命令の中にSTRDATA系命令が含まれています", instructionLine3, 2, isError: true, isBackComp: false);
						break;
					}
				}
				if (!instructionLine3.IsError)
				{
					instructionLine3.dataList = new List<List<InstructionLine>>();
					stack.Push(instructionLine3);
				}
				break;
			case FunctionCode.STRDATA:
				foreach (InstructionLine item3 in stack)
				{
					if (item3.FunctionCode == FunctionCode.STRDATA)
					{
						ParserMediator.Warn("STRDATA命令が入れ子にされています", instructionLine3, 2, isError: true, isBackComp: false);
						break;
					}
					if (item3.Function.IsPrintData())
					{
						ParserMediator.Warn("STRDATA系命令の中にPRINTDATA系命令が含まれています", instructionLine3, 2, isError: true, isBackComp: false);
						break;
					}
				}
				if (!instructionLine3.IsError)
				{
					instructionLine3.dataList = new List<List<InstructionLine>>();
					stack.Push(instructionLine3);
				}
				break;
			case FunctionCode.DATALIST:
			{
				InstructionLine instructionLine11 = ((stack.Count == 0) ? null : stack.Peek());
				if (instructionLine11 == null || (!instructionLine11.Function.IsPrintData() && instructionLine11.FunctionCode != FunctionCode.STRDATA))
				{
					ParserMediator.Warn("対応するPRINTDATA系命令のないDATALISTです", instructionLine3, 2, isError: true, isBackComp: false);
					break;
				}
				list = new List<InstructionLine>();
				stack.Push(instructionLine3);
				break;
			}
			case FunctionCode.ENDLIST:
				if (stack.Count == 0 || stack.Peek().FunctionCode != FunctionCode.DATALIST)
				{
					ParserMediator.Warn("対応するDATALISTのないENDLISTです", instructionLine3, 2, isError: true, isBackComp: false);
					break;
				}
				if (list.Count == 0)
				{
					ParserMediator.Warn("DATALIST命令に表示データが与えられていません（このDATALISTは空文字列を表示します）", instructionLine3, 1, isError: false, isBackComp: false);
				}
				stack.Pop();
				stack.Peek().dataList.Add(list);
				break;
			case FunctionCode.DATA:
			case FunctionCode.DATAFORM:
			{
				InstructionLine instructionLine8 = ((stack.Count == 0) ? null : stack.Peek());
				if (instructionLine8 == null || (!instructionLine8.Function.IsPrintData() && instructionLine8.FunctionCode != FunctionCode.DATALIST && instructionLine8.FunctionCode != FunctionCode.STRDATA))
				{
					ParserMediator.Warn("対応するPRINTDATA系命令のない" + instructionLine3.Function.Name + "です", instructionLine3, 2, isError: true, isBackComp: false);
					break;
				}
				List<InstructionLine> list2 = new List<InstructionLine>();
				if (instructionLine8.FunctionCode != FunctionCode.DATALIST)
				{
					list2.Add(instructionLine3);
					instructionLine8.dataList.Add(list2);
				}
				else
				{
					list.Add(instructionLine3);
				}
				break;
			}
			case FunctionCode.ENDDATA:
			{
				InstructionLine instructionLine12 = ((stack.Count == 0) ? null : stack.Peek());
				if (instructionLine12 == null || (!instructionLine12.Function.IsPrintData() && instructionLine12.FunctionCode != FunctionCode.STRDATA))
				{
					ParserMediator.Warn("対応するPRINTDATA系命令もしくはSTRDATAのない" + instructionLine3.Function.Name + "です", instructionLine3, 2, isError: true, isBackComp: false);
					break;
				}
				if (instructionLine12.FunctionCode == FunctionCode.DATALIST)
				{
					ParserMediator.Warn("DATALISTが閉じられていません", instructionLine3, 2, isError: true, isBackComp: false);
				}
				if (instructionLine12.dataList.Count == 0)
				{
					ParserMediator.Warn(instructionLine12.Function.Name + "命令に表示データがありません（この命令は無視されます）", instructionLine3, 1, isError: false, isBackComp: false);
				}
				instructionLine12.JumpTo = instructionLine3;
				stack.Pop();
				break;
			}
			case FunctionCode.TRYCALLLIST:
			case FunctionCode.TRYJUMPLIST:
			case FunctionCode.TRYGOTOLIST:
				foreach (InstructionLine item4 in stack)
				{
					if (item4.FunctionCode == FunctionCode.TRYCALLLIST || item4.FunctionCode == FunctionCode.TRYJUMPLIST || item4.FunctionCode == FunctionCode.TRYGOTOLIST)
					{
						ParserMediator.Warn("TRYCALLLIST系命令が入れ子にされています", instructionLine3, 2, isError: true, isBackComp: false);
						break;
					}
				}
				if (!instructionLine3.IsError)
				{
					instructionLine3.callList = new List<InstructionLine>();
					stack.Push(instructionLine3);
				}
				break;
			case FunctionCode.FUNC:
			{
				InstructionLine instructionLine14 = ((stack.Count == 0) ? null : stack.Peek());
				if (instructionLine14 == null || (instructionLine14.FunctionCode != FunctionCode.TRYCALLLIST && instructionLine14.FunctionCode != FunctionCode.TRYJUMPLIST && instructionLine14.FunctionCode != FunctionCode.TRYGOTOLIST))
				{
					ParserMediator.Warn("対応するTRYCALLLIST系命令のない" + instructionLine3.Function.Name + "です", instructionLine3, 2, isError: true, isBackComp: false);
					break;
				}
				if (instructionLine3.Argument == null)
				{
					ParserMediator.Warn("TRYCALLLIST系命令中に無効な" + instructionLine3.Function.Name + "が存在します", instructionLine14, 2, isError: true, isBackComp: false);
					break;
				}
				if (instructionLine14.FunctionCode == FunctionCode.TRYGOTOLIST)
				{
					if (((SpCallArgment)instructionLine3.Argument).SubNames.Length != 0)
					{
						ParserMediator.Warn("TRYGOTOLISTの呼び出し対象に[～～]が設定されています", instructionLine3, 2, isError: true, isBackComp: false);
						break;
					}
					if (((SpCallArgment)instructionLine3.Argument).RowArgs.Length != 0)
					{
						ParserMediator.Warn("TRYGOTOLISTの呼び出し対象に引数が設定されています", instructionLine3, 2, isError: true, isBackComp: false);
						break;
					}
				}
				instructionLine14.callList.Add(instructionLine3);
				break;
			}
			case FunctionCode.ENDFUNC:
			{
				InstructionLine instructionLine10 = ((stack.Count == 0) ? null : stack.Peek());
				if (instructionLine10 == null || (instructionLine10.FunctionCode != FunctionCode.TRYCALLLIST && instructionLine10.FunctionCode != FunctionCode.TRYJUMPLIST && instructionLine10.FunctionCode != FunctionCode.TRYGOTOLIST))
				{
					ParserMediator.Warn("対応するTRYCALLLIST系命令のない" + instructionLine3.Function.Name + "です", instructionLine3, 2, isError: true, isBackComp: false);
					break;
				}
				instructionLine10.JumpTo = instructionLine3;
				stack.Pop();
				break;
			}
			case FunctionCode.NOSKIP:
				foreach (InstructionLine item5 in stack)
				{
					if (item5.FunctionCode == FunctionCode.NOSKIP)
					{
						ParserMediator.Warn("NOSKIP系命令が入れ子にされています", instructionLine3, 2, isError: true, isBackComp: false);
						break;
					}
				}
				if (!instructionLine3.IsError)
				{
					stack.Push(instructionLine3);
				}
				break;
			case FunctionCode.ENDNOSKIP:
			{
				InstructionLine instructionLine5 = ((stack.Count == 0) ? null : stack.Peek());
				if (instructionLine5 == null || instructionLine5.FunctionCode != FunctionCode.NOSKIP)
				{
					ParserMediator.Warn("対応するNOSKIP系命令のない" + instructionLine3.Function.Name + "です", instructionLine3, 2, isError: true, isBackComp: false);
					break;
				}
				instructionLine5.JumpTo = instructionLine3;
				instructionLine3.JumpTo = instructionLine5;
				stack.Pop();
				break;
			}
			}
		}
		while (stack.Count != 0)
		{
			InstructionLine instructionLine15 = stack.Pop();
			string name = instructionLine15.Function.Name;
			string matchFunction = FunctionIdentifier.getMatchFunction(instructionLine15.FunctionCode);
			if (instructionLine15 != null)
			{
				ParserMediator.Warn(name + "に対応する" + matchFunction + "が見つかりません", instructionLine15, 2, isError: true, isBackComp: false);
			}
			else
			{
				ParserMediator.Warn("ディフォルトエラー（Emuera設定漏れ）", instructionLine15, 2, isError: true, isBackComp: false);
			}
		}
		stack2.Clear();
	}

	private void setJumpTo(FunctionLabelLine label)
	{
		LogicalLine logicalLine = label;
		int num = label.Depth;
		if (num < 0)
		{
			num = -2;
		}
		while (true)
		{
			logicalLine = logicalLine.NextLine;
			if (!(logicalLine is InstructionLine instructionLine))
			{
				if (logicalLine is NullLine || logicalLine is FunctionLabelLine)
				{
					break;
				}
			}
			else
			{
				if (instructionLine.IsError)
				{
					continue;
				}
				parentProcess.scaningLine = instructionLine;
				if (instructionLine.Function.Instruction != null)
				{
					string FunctionoNotFoundName = null;
					try
					{
						instructionLine.Function.Instruction.SetJumpTo(ref useCallForm, instructionLine, num, ref FunctionoNotFoundName);
					}
					catch (CodeEE codeEE)
					{
						ParserMediator.Warn(codeEE.Message, instructionLine, 2, isError: true, isBackComp: false);
						continue;
					}
					if (FunctionoNotFoundName != null)
					{
						if (!Program.AnalysisMode)
						{
							printFunctionNotFoundWarning("指定された関数名\"@" + FunctionoNotFoundName + "\"は存在しません", instructionLine, 2, isError: true);
						}
						else
						{
							printFunctionNotFoundWarning(FunctionoNotFoundName, instructionLine, 2, isError: true);
						}
					}
				}
				else if (instructionLine.FunctionCode == FunctionCode.TRYCALLLIST || instructionLine.FunctionCode == FunctionCode.TRYJUMPLIST)
				{
					useCallForm = true;
				}
			}
		}
	}
}
