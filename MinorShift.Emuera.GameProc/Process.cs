using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Android.Graphics;
using MinorShift._Library;
using MinorShift.Emuera.Content;
using MinorShift.Emuera.GameData;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Function;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.GameProc.Function;
using MinorShift.Emuera.GameView;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal sealed class Process
{
	private delegate void SystemProcess();

	private LabelDictionary labelDic;

	private VariableEvaluator vEvaluator;

	private ExpressionMediator exm;

	private GameBase gamebase;

	private readonly EmueraConsole console;

	private IdentifierDictionary idDic;

	private ProcessState state;

	private ProcessState originalState;

	private bool noError;

	private bool initialiing;

	private uint startTime;

	private int methodStack;

	private readonly string scaningScope;

	public LogicalLine scaningLine;

	private bool saveSkip;

	private bool userDefinedSkip;

	private List<ProcessState> prevStateList = new List<ProcessState>();

	private string[] TrainName;

	private Dictionary<SystemStateCode, SystemProcess> systemProcessDictionary = new Dictionary<SystemStateCode, SystemProcess>();

	private long systemResult;

	private int lastCalledComable = -1;

	private int lastAddCom = -1;

	private int[] comAble;

	private List<long> coms = new List<long>();

	private bool isCTrain;

	private int count;

	private bool skipPrint;

	private int printComCount;

	private long doTrainSelectCom = -1L;

	public bool NeedWaitToEventComEnd;

	private bool needCheck = true;

	private bool[] dataIsAvailable = new bool[21];

	private bool isFirstTime = true;

	private const int AutoSaveIndex = 99;

	private int page;

	private int saveTarget = -1;

	public LogicalLine getCurrentLine => state.CurrentLine;

	public LabelDictionary LabelDictionary => labelDic;

	public VariableEvaluator VEvaluator => vEvaluator;

	public bool inInitializeing => initialiing;

	public ProcessState getCurrentState => state;

	public bool SkipPrint
	{
		get
		{
			return skipPrint;
		}
		set
		{
			skipPrint = value;
		}
	}

	public Process(EmueraConsole view)
	{
		console = view;
	}

	public bool Initialize()
	{
		LexicalAnalyzer.UseMacro = false;
		state = new ProcessState(console);
		originalState = state;
		initialiing = true;
		try
		{
			ParserMediator.Initialize(console);
			if (ParserMediator.HasWarning)
			{
				ParserMediator.FlushWarningList();
			}
			AppContents.LoadContents();
			if (Config.UseKeyMacro && !Program.AnalysisMode && File.Exists(Program.ExeDir + "macro.txt"))
			{
				if (Config.DisplayReport)
				{
					console.PrintSystemLine("macro.txt読み込み中・・・");
				}
				KeyMacro.LoadMacroFile(Program.ExeDir + "macro.txt");
			}
			if (Config.UseReplaceFile && !Program.AnalysisMode && File.Exists(Program.CsvDir + "_Replace.csv"))
			{
				if (Config.DisplayReport)
				{
					console.PrintSystemLine("_Replace.csv読み込み中・・・");
				}
				ConfigData.Instance.LoadReplaceFile(Program.CsvDir + "_Replace.csv");
				if (ParserMediator.HasWarning)
				{
					ParserMediator.FlushWarningList();
				}
			}
			Config.SetReplace(ConfigData.Instance);
			console.setStBar(Config.DrawLineString);
			if (Config.UseRenameFile)
			{
				if (File.Exists(Program.CsvDir + "_Rename.csv"))
				{
					if (Config.DisplayReport || Program.AnalysisMode)
					{
						console.PrintSystemLine("_Rename.csv読み込み中・・・");
					}
					ParserMediator.LoadEraExRenameFile(Program.CsvDir + "_Rename.csv");
				}
				else
				{
					console.PrintError("csv\\_Rename.csvが見つかりません");
				}
			}
			if (!Config.DisplayReport)
			{
				console.PrintSingleLine(Config.LoadLabel);
				console.RefreshStrings(force_Paint: true);
			}
			gamebase = new GameBase();
			if (!gamebase.LoadGameBaseCsv(Program.CsvDir + "GAMEBASE.CSV"))
			{
				console.PrintSystemLine("GAMEBASE.CSVの読み込み中に問題が発生したため処理を終了しました");
				return false;
			}
			console.SetWindowTitle(gamebase.ScriptWindowTitle);
			GlobalStatic.GameBaseData = gamebase;
			ConstantData constantData = new ConstantData(gamebase);
			constantData.LoadData(Program.CsvDir, console, Config.DisplayReport);
			GlobalStatic.ConstantData = constantData;
			TrainName = constantData.GetCsvNameList(VariableCode.TRAINNAME);
			vEvaluator = new VariableEvaluator(gamebase, constantData);
			GlobalStatic.VEvaluator = vEvaluator;
			idDic = new IdentifierDictionary(vEvaluator.VariableData);
			GlobalStatic.IdentifierDictionary = idDic;
			StrForm.Initialize();
			VariableParser.Initialize();
			exm = new ExpressionMediator(this, vEvaluator, console);
			GlobalStatic.EMediator = exm;
			labelDic = new LabelDictionary();
			GlobalStatic.LabelDictionary = labelDic;
			HeaderFileLoader headerFileLoader = new HeaderFileLoader(console, idDic, this);
			LexicalAnalyzer.UseMacro = false;
			if (!headerFileLoader.LoadHeaderFiles(Program.ErbDir, Config.DisplayReport))
			{
				console.PrintSystemLine("ERHの読み込み中にエラーが発生したため処理を終了しました");
				return false;
			}
			LexicalAnalyzer.UseMacro = idDic.UseMacro();
			ErbLoader erbLoader = new ErbLoader(console, exm, this);
			if (Program.AnalysisMode)
			{
				noError = erbLoader.loadErbs(Program.AnalysisFiles, labelDic);
			}
			else
			{
				noError = erbLoader.LoadErbFiles(Program.ErbDir, Config.DisplayReport, labelDic);
			}
			initSystemProcess();
			initialiing = false;
		}
		catch (Exception exc)
		{
			handleException(exc, null, playSound: true);
			console.PrintSystemLine("初期化中に致命的なエラーが発生したため処理を終了しました");
			return false;
		}
		if (labelDic == null)
		{
			return false;
		}
		state.Begin(BeginType.TITLE);
		GC.Collect();
		return true;
	}

	public void ReloadErb()
	{
		saveCurrentState(single: false);
		state.SystemState = SystemStateCode.System_Reloaderb;
		new ErbLoader(console, exm, this).LoadErbFiles(Program.ErbDir, displayReport: false, labelDic);
		console.ReadAnyKey();
	}

	public void ReloadPartialErb(List<string> path)
	{
		saveCurrentState(single: false);
		state.SystemState = SystemStateCode.System_Reloaderb;
		new ErbLoader(console, exm, this).loadErbs(path, labelDic);
		console.ReadAnyKey();
	}

	public void SetCommnds(long count)
	{
		coms = new List<long>((int)count);
		isCTrain = true;
		long[] sELECTCOM_ARRAY = vEvaluator.SELECTCOM_ARRAY;
		if (count >= sELECTCOM_ARRAY.Length)
		{
			throw new CodeEE("CALLTRAIN命令の引数の値がSELECTCOMの要素数を超えています");
		}
		for (int i = 0; i < (int)count; i++)
		{
			coms.Add(sELECTCOM_ARRAY[i + 1]);
		}
	}

	public bool ClearCommands()
	{
		coms.Clear();
		count = 0;
		isCTrain = false;
		skipPrint = true;
		return callFunction("CALLTRAINEND", force: false, isEvent: false);
	}

	public void InputInteger(long i)
	{
		vEvaluator.RESULT = i;
	}

	public void InputSystemInteger(long i)
	{
		systemResult = i;
	}

	public void InputString(string s)
	{
		vEvaluator.RESULTS = s;
	}

	public void DoScript()
	{
		startTime = WinmmTimer.TickCount;
		state.lineCount = 0;
		bool flag = true;
		try
		{
			while (true)
			{
				methodStack = 0;
				flag = true;
				while (state.ScriptEnd && console.IsRunning)
				{
					runSystemProc();
				}
				if (console.IsRunning)
				{
					flag = false;
					runScriptProc();
					continue;
				}
				break;
			}
		}
		catch (Exception exc)
		{
			LogicalLine logicalLine = state.ErrorLine;
			if (logicalLine != null && logicalLine is NullLine)
			{
				logicalLine = null;
			}
			if (flag)
			{
				handleExceptionInSystemProc(exc, logicalLine, playSound: true);
			}
			else
			{
				handleException(exc, logicalLine, playSound: true);
			}
		}
	}

	public void BeginTitle()
	{
		vEvaluator.ResetData();
		state = originalState;
		state.Begin(BeginType.TITLE);
	}

	private void checkInfiniteLoop()
	{
		uint num = WinmmTimer.TickCount - startTime;
		if (num >= Config.InfiniteLoopAlertTime)
		{
			LogicalLine currentLine = state.CurrentLine;
			if (currentLine != null && !(currentLine is NullLine) && console.Enabled)
			{
				_ = $"無限ループの可能性があります";
				string.Format("現在、{0}の{1}行目を実行中です。\n最後の入力から{3}ミリ秒経過し{2}行が実行されました。\n処理を中断し強制終了しますか？", currentLine.Position.Filename, currentLine.Position.LineNo, state.lineCount, num);
			}
		}
	}

	public SingleTerm GetValue(SuperUserDefinedMethodTerm udmt)
	{
		methodStack++;
		if (methodStack > 100)
		{
			throw new CodeEE("関数の呼び出しスタックが溢れました(無限に再帰呼び出しされていませんか？)");
		}
		SingleTerm singleTerm = null;
		int currentMin = state.currentMin;
		state.currentMin = state.functionCount;
		udmt.Call.updateRetAddress(state.CurrentLine);
		try
		{
			state.IntoFunction(udmt.Call, udmt.Argument, exm);
			runScriptProc();
			return state.MethodReturnValue;
		}
		finally
		{
			if (udmt.Call.TopLabel.hasPrivDynamicVar)
			{
				udmt.Call.TopLabel.Out();
			}
			state.currentMin = currentMin;
			methodStack--;
		}
	}

	public void clearMethodStack()
	{
		methodStack = 0;
	}

	public int MethodStack()
	{
		return methodStack;
	}

	public ScriptPosition GetRunningPosition()
	{
		return state.ErrorLine?.Position;
	}

	private string GetScaningScope()
	{
		if (scaningScope != null)
		{
			return scaningScope;
		}
		return state.Scope;
	}

	internal LogicalLine GetScaningLine()
	{
		if (scaningLine != null)
		{
			return scaningLine;
		}
		LogicalLine errorLine = state.ErrorLine;
		if (errorLine == null)
		{
			return null;
		}
		return errorLine;
	}

	private void handleExceptionInSystemProc(Exception exc, LogicalLine current, bool playSound)
	{
		console.ThrowError(playSound);
		if (exc is CodeEE)
		{
			console.PrintError("関数の終端でエラーが発生しました:" + Program.ExeName);
			console.PrintError(exc.Message);
			return;
		}
		if (exc is ExeEE)
		{
			console.PrintError("関数の終端でEmueraのエラーが発生しました:" + Program.ExeName);
			console.PrintError(exc.Message);
			return;
		}
		console.PrintError("関数の終端で予期しないエラーが発生しました:" + Program.ExeName);
		console.PrintError(exc.GetType().ToString() + ":" + exc.Message);
		string[] array = exc.StackTrace.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			console.PrintError(array[i]);
		}
	}

	private void handleException(Exception exc, LogicalLine current, bool playSound)
	{
		console.ThrowError(playSound);
		ScriptPosition scriptPosition = null;
		if (exc is EmueraException { Position: not null } ex)
		{
			scriptPosition = ex.Position;
		}
		else if (current != null && current.Position != null)
		{
			scriptPosition = current.Position;
		}
		string text = "";
		if (scriptPosition != null)
		{
			text = ((scriptPosition.LineNo < 0) ? (scriptPosition.Filename + "で") : (scriptPosition.Filename + "の" + scriptPosition.LineNo + "行目で"));
		}
		if (exc is CodeEE)
		{
			if (scriptPosition != null)
			{
				if (current is InstructionLine { FunctionCode: FunctionCode.THROW })
				{
					console.PrintErrorButton(text + "THROWが発生しました", scriptPosition);
					if (scriptPosition.RowLine != null)
					{
						console.PrintError(scriptPosition.RowLine);
					}
					console.PrintError("THROW内容：" + exc.Message);
				}
				else
				{
					console.PrintErrorButton(text + "エラーが発生しました:" + Program.ExeName, scriptPosition);
					if (scriptPosition.RowLine != null)
					{
						console.PrintError(scriptPosition.RowLine);
					}
					console.PrintError("エラー内容：" + exc.Message);
				}
				console.PrintError("現在の関数：@" + current.ParentLabelLine.LabelName + "（" + current.ParentLabelLine.Position.Filename + "の" + current.ParentLabelLine.Position.LineNo + "行目）");
				console.PrintError("関数呼び出しスタック：");
				LogicalLine logicalLine = null;
				int num = 0;
				while ((logicalLine = state.GetReturnAddressSequensial(num++)) != null)
				{
					if (logicalLine.Position != null)
					{
						console.PrintErrorButton("↑" + logicalLine.Position.Filename + "の" + logicalLine.Position.LineNo + "行目（関数@" + logicalLine.ParentLabelLine.LabelName + "内）", logicalLine.Position);
					}
				}
			}
			else
			{
				console.PrintError(text + "エラーが発生しました:" + Program.ExeName);
				console.PrintError(exc.Message);
			}
		}
		else if (exc is ExeEE)
		{
			console.PrintError(text + "Emueraのエラーが発生しました:" + Program.ExeName);
			console.PrintError(exc.Message);
		}
		else
		{
			console.PrintError(text + "予期しないエラーが発生しました:" + Program.ExeName);
			console.PrintError(exc.GetType().ToString() + ":" + exc.Message);
			string[] array = exc.StackTrace.Split('\n');
			for (int i = 0; i < array.Length; i++)
			{
				console.PrintError(array[i]);
			}
		}
	}

	private void runScriptProc()
	{
		do
		{
			IL_0000:
			state.ShiftNextLine();
			if (Config.InfiniteLoopAlertTime > 0 && state.lineCount % 10000 == 0)
			{
				checkInfiniteLoop();
			}
			LogicalLine currentLine = state.CurrentLine;
			InstructionLine instructionLine = currentLine as InstructionLine;
			if (currentLine.IsError)
			{
				throw new CodeEE(currentLine.ErrMes);
			}
			if (instructionLine != null)
			{
				if (!Program.DebugMode && instructionLine.Function.IsDebug())
				{
					goto IL_0000;
				}
				if (instructionLine.Argument == null)
				{
					ArgumentParser.SetArgumentTo(instructionLine);
					if (instructionLine.IsError)
					{
						throw new CodeEE(instructionLine.ErrMes);
					}
				}
				if (skipPrint && instructionLine.Function.IsPrint())
				{
					if (userDefinedSkip && instructionLine.Function.IsInput())
					{
						console.PrintError("表示スキップ中にデフォルト値を持たないINPUTに遭遇しました");
						console.PrintError("INPUTに必要な処理をNOSKIP～ENDNOSKIPで囲むか、SKIPDISP 0～SKIPDISP 1で囲ってください");
						throw new CodeEE("無限ループに入る可能性が高いため実行を終了します");
					}
					goto IL_0000;
				}
				if (instructionLine.Function.Instruction != null)
				{
					instructionLine.Function.Instruction.DoInstruction(exm, instructionLine, state);
				}
				else if (instructionLine.Function.IsFlowContorol())
				{
					doFlowControlFunction(instructionLine);
				}
				else
				{
					doNormalFunction(instructionLine);
				}
				continue;
			}
			if (currentLine is NullLine || currentLine is FunctionLabelLine)
			{
				if (!state.IsFunctionMethod)
				{
					vEvaluator.RESULT = 0L;
				}
				state.Return(0L);
				continue;
			}
			if (currentLine is GotoLabelLine)
			{
				goto IL_0000;
			}
			if (currentLine is InvalidLine)
			{
				if (string.IsNullOrEmpty(currentLine.ErrMes))
				{
					throw new CodeEE("読込に失敗した行が実行されました。エラーの詳細は読込時の警告を参照してください。");
				}
				throw new CodeEE(currentLine.ErrMes);
			}
		}
		while (console.IsRunning && !state.ScriptEnd);
	}

	public void DoDebugNormalFunction(InstructionLine func, bool munchkin)
	{
		if (func.Function.Instruction != null)
		{
			func.Function.Instruction.DoInstruction(exm, func, state);
		}
		else
		{
			doNormalFunction(func);
		}
		if (munchkin)
		{
			vEvaluator.IamaMunchkin();
		}
	}

	private void doNormalFunction(InstructionLine func)
	{
		long num = 0L;
		string text = null;
		IOperandTerm operandTerm = null;
		switch (func.FunctionCode)
		{
		case FunctionCode.PRINTBUTTON:
			if (!skipPrint)
			{
				SpButtonArgument spButtonArgument2 = (SpButtonArgument)func.Argument;
				text = spButtonArgument2.PrintStrTerm.GetStrValue(exm);
				text = text.Replace("\n", "");
				if (spButtonArgument2.ButtonWord.GetOperandType() == typeof(long))
				{
					exm.Console.PrintButton(text, spButtonArgument2.ButtonWord.GetIntValue(exm));
				}
				else
				{
					exm.Console.PrintButton(text, spButtonArgument2.ButtonWord.GetStrValue(exm));
				}
			}
			break;
		case FunctionCode.PRINTBUTTONC:
		case FunctionCode.PRINTBUTTONLC:
			if (!skipPrint)
			{
				SpButtonArgument spButtonArgument = (SpButtonArgument)func.Argument;
				text = spButtonArgument.PrintStrTerm.GetStrValue(exm);
				text = text.Replace("\n", "");
				bool isRight = func.FunctionCode == FunctionCode.PRINTBUTTONC;
				if (spButtonArgument.ButtonWord.GetOperandType() == typeof(long))
				{
					exm.Console.PrintButtonC(text, spButtonArgument.ButtonWord.GetIntValue(exm), isRight);
				}
				else
				{
					exm.Console.PrintButtonC(text, spButtonArgument.ButtonWord.GetStrValue(exm), isRight);
				}
			}
			break;
		case FunctionCode.PRINTPLAIN:
		case FunctionCode.PRINTPLAINFORM:
			if (!skipPrint)
			{
				operandTerm = ((ExpressionArgument)func.Argument).Term;
				exm.Console.PrintPlain(operandTerm.GetStrValue(exm));
			}
			break;
		case FunctionCode.DRAWLINE:
			if (!skipPrint)
			{
				exm.Console.PrintBar();
				exm.Console.NewLine();
			}
			break;
		case FunctionCode.CUSTOMDRAWLINE:
		case FunctionCode.DRAWLINEFORM:
			if (!skipPrint)
			{
				operandTerm = ((ExpressionArgument)func.Argument).Term;
				text = operandTerm.GetStrValue(exm);
				exm.Console.printCustomBar(text);
				exm.Console.NewLine();
			}
			break;
		case FunctionCode.PRINT_ABL:
		case FunctionCode.PRINT_TALENT:
		case FunctionCode.PRINT_MARK:
		case FunctionCode.PRINT_EXP:
			if (!skipPrint)
			{
				long intValue2 = ((ExpressionArgument)func.Argument).Term.GetIntValue(exm);
				exm.Console.Print(vEvaluator.GetCharacterDataString(intValue2, func.FunctionCode));
				exm.Console.NewLine();
			}
			break;
		case FunctionCode.PRINT_PALAM:
		{
			if (skipPrint)
			{
				break;
			}
			long intValue8 = ((ExpressionArgument)func.Argument).Term.GetIntValue(exm);
			int num19 = 0;
			for (int k = 0; k < 100; k++)
			{
				string characterParamString = vEvaluator.GetCharacterParamString(intValue8, k);
				if (characterParamString != null)
				{
					exm.Console.PrintC(characterParamString, alignmentRight: true);
					num19++;
					if (Config.PrintCPerLine > 0 && num19 % Config.PrintCPerLine == 0)
					{
						exm.Console.PrintFlush(force: false);
					}
				}
			}
			exm.Console.PrintFlush(force: false);
			exm.Console.RefreshStrings(force_Paint: false);
			break;
		}
		case FunctionCode.PRINT_ITEM:
			if (!skipPrint)
			{
				exm.Console.Print(vEvaluator.GetHavingItemsString());
				exm.Console.NewLine();
			}
			break;
		case FunctionCode.PRINT_SHOPITEM:
		{
			if (skipPrint)
			{
				break;
			}
			int num20 = Math.Min(vEvaluator.ITEMSALES.Length, vEvaluator.ITEMNAME.Length);
			if (num20 > vEvaluator.ITEMPRICE.Length)
			{
				num20 = vEvaluator.ITEMPRICE.Length;
			}
			int num21 = 0;
			for (int l = 0; l < num20; l++)
			{
				if (vEvaluator.ItemSales(l))
				{
					string text2 = vEvaluator.ITEMNAME[l];
					if (text2 == null)
					{
						text2 = "";
					}
					long num22 = vEvaluator.ITEMPRICE[l];
					if (Config.MoneyFirst)
					{
						exm.Console.PrintC(string.Format("[{2}] {0}({3}{1})", text2, num22, l, Config.MoneyLabel), alignmentRight: false);
					}
					else
					{
						exm.Console.PrintC(string.Format("[{2}] {0}({1}{3})", text2, num22, l, Config.MoneyLabel), alignmentRight: false);
					}
					num21++;
					if (Config.PrintCPerLine > 0 && num21 % Config.PrintCPerLine == 0)
					{
						exm.Console.PrintFlush(force: false);
					}
				}
			}
			exm.Console.PrintFlush(force: false);
			exm.Console.RefreshStrings(force_Paint: false);
			break;
		}
		case FunctionCode.UPCHECK:
			vEvaluator.UpdateInUpcheck(exm.Console, skipPrint);
			break;
		case FunctionCode.CUPCHECK:
		{
			long intValue6 = ((ExpressionArgument)func.Argument).Term.GetIntValue(exm);
			vEvaluator.CUpdateInUpcheck(exm.Console, intValue6, skipPrint);
			break;
		}
		case FunctionCode.DELALLCHARA:
			vEvaluator.DelAllCharacter();
			break;
		case FunctionCode.PICKUPCHARA:
		{
			ExpressionArrayArgument expressionArrayArgument = (ExpressionArrayArgument)func.Argument;
			long[] array4 = new long[expressionArrayArgument.TermList.Length];
			long cHARANUM = vEvaluator.CHARANUM;
			for (int j = 0; j < expressionArrayArgument.TermList.Length; j++)
			{
				IOperandTerm operandTerm2 = expressionArrayArgument.TermList[j];
				array4[j] = operandTerm2.GetIntValue(exm);
				if ((!(operandTerm2 is VariableTerm) || (((VariableTerm)operandTerm2).Identifier.Code != VariableCode.MASTER && ((VariableTerm)operandTerm2).Identifier.Code != VariableCode.ASSI && ((VariableTerm)operandTerm2).Identifier.Code != VariableCode.TARGET)) && (array4[j] < 0 || array4[j] >= cHARANUM))
				{
					throw new CodeEE("命令PICKUPCHARAの第" + (j + 1) + "引数にキャラリストの範囲外の値(" + array4[j] + ")が与えられました");
				}
			}
			vEvaluator.PickUpChara(array4);
			break;
		}
		case FunctionCode.ADDDEFCHARA:
			if (func.ParentLabelLine != null && func.ParentLabelLine.LabelName != "SYSTEM_TITLE")
			{
				throw new CodeEE("@SYSTEM_TITLE以外でこの命令を使うことはできません");
			}
			vEvaluator.AddCharacterFromCsvNo(0L);
			if (GlobalStatic.GameBaseData.DefaultCharacter > 0)
			{
				vEvaluator.AddCharacterFromCsvNo(GlobalStatic.GameBaseData.DefaultCharacter);
			}
			break;
		case FunctionCode.PUTFORM:
			operandTerm = ((ExpressionArgument)func.Argument).Term;
			text = operandTerm.GetStrValue(exm);
			if (vEvaluator.SAVEDATA_TEXT != null)
			{
				vEvaluator.SAVEDATA_TEXT += text;
			}
			else
			{
				vEvaluator.SAVEDATA_TEXT = text;
			}
			break;
		case FunctionCode.QUIT:
			exm.Console.Quit();
			break;
		case FunctionCode.VARSIZE:
		{
			VariableToken variableID = ((SpVarsizeArgument)func.Argument).VariableID;
			vEvaluator.VarSize(variableID);
			break;
		}
		case FunctionCode.SAVEDATA:
		{
			SpSaveDataArgument obj2 = (SpSaveDataArgument)func.Argument;
			long intValue3 = obj2.Target.GetIntValue(exm);
			if (intValue3 < 0)
			{
				throw new CodeEE("SAVEDATAの引数に負の値(" + intValue3 + ")が指定されました");
			}
			if (intValue3 > int.MaxValue)
			{
				throw new CodeEE("SAVEDATAの引数(" + intValue3 + ")が大きすぎます");
			}
			string strValue2 = obj2.StrExpression.GetStrValue(exm);
			if (strValue2.Contains("\n"))
			{
				throw new CodeEE("SAVEDATAのセーブテキストに改行文字が与えられました（セーブデータが破損するため改行文字は使えません）");
			}
			if (!vEvaluator.SaveTo((int)intValue3, strValue2))
			{
				console.PrintError("SAVEDATA命令によるセーブ中に予期しないエラーが発生しました");
			}
			break;
		}
		case FunctionCode.POWER:
		{
			SpPowerArgument obj4 = (SpPowerArgument)func.Argument;
			double x = obj4.X.GetIntValue(exm);
			double y = obj4.Y.GetIntValue(exm);
			double num12 = Math.Pow(x, y);
			if (double.IsNaN(num12))
			{
				throw new CodeEE("累乗結果が非数値です");
			}
			if (double.IsInfinity(num12))
			{
				throw new CodeEE("累乗結果が無限大です");
			}
			if (num12 >= 9.223372036854776E+18 || num12 <= -9.223372036854776E+18)
			{
				throw new CodeEE("累乗結果(" + num12 + ")が64ビット符号付き整数の範囲外です");
			}
			obj4.VariableDest.SetValue((long)num12, exm);
			break;
		}
		case FunctionCode.SWAP:
		{
			SpSwapVarArgument spSwapVarArgument = (SpSwapVarArgument)func.Argument;
			FixedVariableTerm fixedVariableTerm4 = spSwapVarArgument.var1.GetFixedVariableTerm(exm);
			FixedVariableTerm fixedVariableTerm5 = spSwapVarArgument.var2.GetFixedVariableTerm(exm);
			if (fixedVariableTerm4.GetOperandType() != fixedVariableTerm5.GetOperandType())
			{
				throw new CodeEE("入れ替える変数の型が異なります");
			}
			if (fixedVariableTerm4.GetOperandType() == typeof(long))
			{
				long intValue7 = fixedVariableTerm4.GetIntValue(exm);
				fixedVariableTerm4.SetValue(fixedVariableTerm5.GetIntValue(exm), exm);
				fixedVariableTerm5.SetValue(intValue7, exm);
				break;
			}
			if (spSwapVarArgument.var1.GetOperandType() == typeof(string))
			{
				string strValue5 = fixedVariableTerm4.GetStrValue(exm);
				fixedVariableTerm4.SetValue(fixedVariableTerm5.GetStrValue(exm), exm);
				fixedVariableTerm5.SetValue(strValue5, exm);
				break;
			}
			throw new CodeEE("不明な変数型です");
		}
		case FunctionCode.GETTIME:
		{
			long num23 = DateTime.Now.Year;
			num23 = num23 * 100 + DateTime.Now.Month;
			num23 = num23 * 100 + DateTime.Now.Day;
			num23 = num23 * 100 + DateTime.Now.Hour;
			num23 = num23 * 100 + DateTime.Now.Minute;
			num23 = num23 * 100 + DateTime.Now.Second;
			num23 = num23 * 1000 + DateTime.Now.Millisecond;
			vEvaluator.RESULT = num23;
			vEvaluator.RESULTS = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
			break;
		}
		case FunctionCode.SETCOLOR:
		{
			SpColorArgument spColorArgument = (SpColorArgument)func.Argument;
			long num4;
			long num5;
			long num6;
			if (spColorArgument.RGB != null)
			{
				long intValue = spColorArgument.RGB.GetIntValue(exm);
				num4 = (intValue & 0xFF0000) >> 16;
				num5 = (intValue & 0xFF00) >> 8;
				num6 = intValue & 0xFF;
			}
			else
			{
				num4 = spColorArgument.R.GetIntValue(exm);
				num5 = spColorArgument.G.GetIntValue(exm);
				num6 = spColorArgument.B.GetIntValue(exm);
				if (num4 < 0 || num5 < 0 || num6 < 0)
				{
					throw new CodeEE("SETCOLORの引数に0未満の値が指定されました");
				}
				if (num4 > 255 || num5 > 255 || num6 > 255)
				{
					throw new CodeEE("SETCOLORの引数に255を超える値が指定されました");
				}
			}
			Android.Graphics.Color stringStyle = Android.Graphics.Color.Argb(255, (int)num4, (int)num5, (int)num6);
			exm.Console.SetStringStyle(stringStyle);
			break;
		}
		case FunctionCode.SETCOLORBYNAME:
		{
			string constStr2 = func.Argument.ConstStr;
			System.Drawing.Color color2 = System.Drawing.Color.FromName(constStr2);
			if (color2.A == 0)
			{
				if (text.Equals("transparent", StringComparison.OrdinalIgnoreCase))
				{
					throw new CodeEE("無色透明(Transparent)は色として指定できません");
				}
				throw new CodeEE("指定された色名\"" + constStr2 + "\"は無効な色名です");
			}
			exm.Console.SetStringStyle(new Android.Graphics.Color(color2.ToArgb()));
			break;
		}
		case FunctionCode.SETBGCOLOR:
		{
			SpColorArgument spColorArgument2 = (SpColorArgument)func.Argument;
			long num13;
			long num14;
			long num15;
			if (spColorArgument2.IsConst)
			{
				long constInt = spColorArgument2.ConstInt;
				num13 = (constInt & 0xFF0000) >> 16;
				num14 = (constInt & 0xFF00) >> 8;
				num15 = constInt & 0xFF;
			}
			else if (spColorArgument2.RGB != null)
			{
				long intValue4 = spColorArgument2.RGB.GetIntValue(exm);
				num13 = (intValue4 & 0xFF0000) >> 16;
				num14 = (intValue4 & 0xFF00) >> 8;
				num15 = intValue4 & 0xFF;
			}
			else
			{
				num13 = spColorArgument2.R.GetIntValue(exm);
				num14 = spColorArgument2.G.GetIntValue(exm);
				num15 = spColorArgument2.B.GetIntValue(exm);
				if (num13 < 0 || num14 < 0 || num15 < 0)
				{
					throw new CodeEE("SETCOLORの引数に0未満の値が指定されました");
				}
				if (num13 > 255 || num14 > 255 || num15 > 255)
				{
					throw new CodeEE("SETCOLORの引数に255を超える値が指定されました");
				}
			}
			Android.Graphics.Color bgColor = Android.Graphics.Color.Argb(255, (int)num13, (int)num14, (int)num15);
			exm.Console.SetBgColor(bgColor);
			break;
		}
		case FunctionCode.SETBGCOLORBYNAME:
		{
			string constStr = func.Argument.ConstStr;
			System.Drawing.Color color = System.Drawing.Color.FromName(constStr);
			if (color.A == 0)
			{
				if (text.Equals("transparent", StringComparison.OrdinalIgnoreCase))
				{
					throw new CodeEE("無色透明(Transparent)は色として指定できません");
				}
				throw new CodeEE("指定された色名\"" + constStr + "\"は無効な色名です");
			}
			exm.Console.SetBgColor(new Android.Graphics.Color(color.ToArgb()));
			break;
		}
		case FunctionCode.SETFONT:
			text = ((!func.Argument.IsConst) ? ((ExpressionArgument)func.Argument).Term.GetStrValue(exm) : func.Argument.ConstStr);
			exm.Console.SetFont(text);
			break;
		case FunctionCode.ALIGNMENT:
			text = func.Argument.ConstStr;
			if (text.Equals("LEFT", Config.SCVariable))
			{
				exm.Console.Alignment = DisplayLineAlignment.LEFT;
				break;
			}
			if (text.Equals("CENTER", Config.SCVariable))
			{
				exm.Console.Alignment = DisplayLineAlignment.CENTER;
				break;
			}
			if (text.Equals("RIGHT", Config.SCVariable))
			{
				exm.Console.Alignment = DisplayLineAlignment.RIGHT;
				break;
			}
			throw new CodeEE("ALIGNMENTのキーワード\"" + text + "\"は未定義です");
		case FunctionCode.REDRAW:
			num = ((!func.Argument.IsConst) ? ((ExpressionArgument)func.Argument).Term.GetIntValue(exm) : func.Argument.ConstInt);
			exm.Console.SetRedraw(num);
			break;
		case FunctionCode.RESET_STAIN:
			num = ((!func.Argument.IsConst) ? ((ExpressionArgument)func.Argument).Term.GetIntValue(exm) : func.Argument.ConstInt);
			vEvaluator.SetDefaultStain(num);
			break;
		case FunctionCode.SPLIT:
		{
			SpSplitArgument spSplitArgument = (SpSplitArgument)func.Argument;
			string strValue3 = spSplitArgument.TargetStr.GetStrValue(exm);
			string[] separator = new string[1] { spSplitArgument.Split.GetStrValue(exm) };
			string[] array5 = strValue3.Split(separator, StringSplitOptions.None);
			spSplitArgument.Num.SetValue(array5.Length, exm);
			if (array5.Length > spSplitArgument.Var.GetLength(0))
			{
				string[] sourceArray = array5;
				array5 = new string[spSplitArgument.Var.GetLength(0)];
				Array.Copy(sourceArray, array5, array5.Length);
			}
			spSplitArgument.Var.SetValue(array5, new long[3]);
			break;
		}
		case FunctionCode.PRINTCPERLINE:
			((SpGetIntArgument)func.Argument).VarToken.SetValue(Config.PrintCPerLine, exm);
			break;
		case FunctionCode.SAVENOS:
			((SpGetIntArgument)func.Argument).VarToken.SetValue(Config.SaveDataNos, exm);
			break;
		case FunctionCode.FORCEKANA:
			num = ((!func.Argument.IsConst) ? ((ExpressionArgument)func.Argument).Term.GetIntValue(exm) : func.Argument.ConstInt);
			exm.ForceKana(num);
			break;
		case FunctionCode.SKIPDISP:
			num = (func.Argument.IsConst ? func.Argument.ConstInt : ((ExpressionArgument)func.Argument).Term.GetIntValue(exm));
			skipPrint = num != 0;
			userDefinedSkip = num != 0;
			vEvaluator.RESULT = (skipPrint ? 1 : 0);
			break;
		case FunctionCode.NOSKIP:
			if (func.JumpTo == null)
			{
				throw new CodeEE("対応するENDNOSKIPのないNOSKIPです");
			}
			saveSkip = skipPrint;
			if (skipPrint)
			{
				skipPrint = false;
			}
			break;
		case FunctionCode.ENDNOSKIP:
			if (func.JumpTo == null)
			{
				throw new CodeEE("対応するNOSKIPのないENDNOSKIPです");
			}
			if (saveSkip)
			{
				skipPrint = true;
			}
			break;
		case FunctionCode.OUTPUTLOG:
			exm.Console.OutputLog(null);
			break;
		case FunctionCode.ARRAYSHIFT:
		{
			SpArrayShiftArgument spArrayShiftArgument = (SpArrayShiftArgument)func.Argument;
			if (!spArrayShiftArgument.VarToken.Identifier.IsArray1D)
			{
				throw new CodeEE("ARRAYSHIFTは1次元配列および配列型キャラクタ変数のみに対応しています");
			}
			FixedVariableTerm fixedVariableTerm3 = spArrayShiftArgument.VarToken.GetFixedVariableTerm(exm);
			int num16 = (int)spArrayShiftArgument.Num1.GetIntValue(exm);
			if (num16 == 0)
			{
				break;
			}
			int num17 = (int)spArrayShiftArgument.Num3.GetIntValue(exm);
			if (num17 < 0)
			{
				throw new CodeEE("ARRAYSHIFTの第４引数が負の値(" + num17 + ")です");
			}
			int num18;
			if (spArrayShiftArgument.Num4 != null)
			{
				num18 = (int)spArrayShiftArgument.Num4.GetIntValue(exm);
				if (num18 < 0)
				{
					throw new CodeEE("ARRAYSHIFTの第５引数が負の値(" + num17 + ")です");
				}
				if (num18 == 0)
				{
					break;
				}
			}
			else
			{
				num18 = -1;
			}
			if (fixedVariableTerm3.Identifier.IsInteger)
			{
				long intValue5 = spArrayShiftArgument.Num2.GetIntValue(exm);
				vEvaluator.ShiftArray(fixedVariableTerm3, num16, intValue5, num17, num18);
			}
			else
			{
				string strValue4 = spArrayShiftArgument.Num2.GetStrValue(exm);
				vEvaluator.ShiftArray(fixedVariableTerm3, num16, strValue4, num17, num18);
			}
			break;
		}
		case FunctionCode.ARRAYREMOVE:
		{
			SpArrayControlArgument obj3 = (SpArrayControlArgument)func.Argument;
			if (!obj3.VarToken.Identifier.IsArray1D)
			{
				throw new CodeEE("ARRAYREMOVEは1次元配列および配列型キャラクタ変数のみに対応しています");
			}
			FixedVariableTerm fixedVariableTerm2 = obj3.VarToken.GetFixedVariableTerm(exm);
			int num10 = (int)obj3.Num1.GetIntValue(exm);
			int num11 = (int)obj3.Num2.GetIntValue(exm);
			if (num10 < 0)
			{
				throw new CodeEE("ARRAYREMOVEの第２引数が負の値(" + num10 + ")です");
			}
			if (num11 < 0)
			{
				throw new CodeEE("ARRAYREMOVEの第３引数が負の値(" + num10 + ")です");
			}
			if (num11 != 0)
			{
				vEvaluator.RemoveArray(fixedVariableTerm2, num10, num11);
			}
			break;
		}
		case FunctionCode.ARRAYSORT:
		{
			SpArraySortArgument spArraySortArgument = (SpArraySortArgument)func.Argument;
			if (!spArraySortArgument.VarToken.Identifier.IsArray1D)
			{
				throw new CodeEE("ARRAYRESORTは1次元配列および配列型キャラクタ変数のみに対応しています");
			}
			FixedVariableTerm fixedVariableTerm = spArraySortArgument.VarToken.GetFixedVariableTerm(exm);
			int num8 = (int)spArraySortArgument.Num1.GetIntValue(exm);
			if (num8 < 0)
			{
				throw new CodeEE("ARRAYSORTの第３引数が負の値(" + num8 + ")です");
			}
			int num9 = 0;
			if (spArraySortArgument.Num2 != null)
			{
				num9 = (int)spArraySortArgument.Num2.GetIntValue(exm);
				if (num9 < 0)
				{
					throw new CodeEE("ARRAYSORTの第４引数が負の値(" + num8 + ")です");
				}
				if (num9 == 0)
				{
					break;
				}
			}
			else
			{
				num9 = -1;
			}
			vEvaluator.SortArray(fixedVariableTerm, spArraySortArgument.Order, num8, num9);
			break;
		}
		case FunctionCode.ARRAYCOPY:
		{
			SpCopyArrayArgument obj = (SpCopyArrayArgument)func.Argument;
			IOperandTerm varName = obj.VarName1;
			IOperandTerm varName2 = obj.VarName2;
			VariableToken[] array2 = new VariableToken[2];
			if (!(varName is SingleTerm) || !(varName2 is SingleTerm))
			{
				string[] array3 = new string[2]
				{
					varName.GetStrValue(exm),
					varName2.GetStrValue(exm)
				};
				if ((array2[0] = GlobalStatic.IdentifierDictionary.GetVariableToken(array3[0], null, allowPrivate: true)) == null)
				{
					throw new CodeEE("ARRAYCOPY命令の第１引数(" + array3[0] + ")が有効な変数名ではありません");
				}
				if (!array2[0].IsArray1D && !array2[0].IsArray2D && !array2[0].IsArray3D)
				{
					throw new CodeEE("ARRAYCOPY命令の第１引数\"" + array3[0] + "\"は配列変数ではありません");
				}
				if (array2[0].IsCharacterData)
				{
					throw new CodeEE("ARRAYCOPY命令の第１引数\"" + array3[0] + "\"はキャラクタ変数です（対応していません）");
				}
				if ((array2[1] = GlobalStatic.IdentifierDictionary.GetVariableToken(array3[1], null, allowPrivate: true)) == null)
				{
					throw new CodeEE("ARRAYCOPY命令の第２引数(" + array3[0] + ")が有効な変数名ではありません");
				}
				if (!array2[1].IsArray1D && !array2[1].IsArray2D && !array2[1].IsArray3D)
				{
					throw new CodeEE("ARRAYCOPY命令の第２引数\"" + array3[1] + "\"は配列変数ではありません");
				}
				if (array2[1].IsCharacterData)
				{
					throw new CodeEE("ARRAYCOPY命令の第２引数\"" + array3[1] + "\"はキャラクタ変数です（対応していません）");
				}
				if (array2[1].IsConst)
				{
					throw new CodeEE("ARRAYCOPY命令の第２引数\"" + array3[1] + "\"は値を変更できない変数です");
				}
				if ((array2[0].IsArray1D && !array2[1].IsArray1D) || (array2[0].IsArray2D && !array2[1].IsArray2D) || (array2[0].IsArray3D && !array2[1].IsArray3D))
				{
					throw new CodeEE("ARRAYCOPY命令の２つの配列変数の次元数が一致していません");
				}
				if ((array2[0].IsInteger && array2[1].IsString) || (array2[0].IsString && array2[1].IsInteger))
				{
					throw new CodeEE("ARRAYCOPY命令の２つの配列変数の型が一致していません");
				}
			}
			else
			{
				array2[0] = GlobalStatic.IdentifierDictionary.GetVariableToken(((SingleTerm)varName).Str, null, allowPrivate: true);
				array2[1] = GlobalStatic.IdentifierDictionary.GetVariableToken(((SingleTerm)varName2).Str, null, allowPrivate: true);
				if ((array2[0].IsInteger && array2[1].IsString) || (array2[0].IsString && array2[1].IsInteger))
				{
					throw new CodeEE("ARRAYCOPY命令の２つの配列変数の型が一致していません");
				}
			}
			vEvaluator.CopyArray(array2[0], array2[1]);
			break;
		}
		case FunctionCode.ENCODETOUNI:
		{
			operandTerm = ((ExpressionArgument)func.Argument).Term;
			string strValue = operandTerm.GetStrValue(exm);
			int num7 = vEvaluator.RESULT_ARRAY.Length;
			if (strValue.Length > num7 - 1)
			{
				throw new CodeEE($"ENCODETOUNIの引数が長すぎます（現在{strValue.Length}文字。最大{num7 - 1}文字まで）");
			}
			int[] array = new int[strValue.Length];
			for (int i = 0; i < strValue.Length; i++)
			{
				array[i] = char.ConvertToUtf32(strValue, i);
			}
			vEvaluator.SetEncodingResult(array);
			break;
		}
		case FunctionCode.ASSERT:
			if (((ExpressionArgument)func.Argument).Term.GetIntValue(exm) == 0L)
			{
				throw new CodeEE("ASSERT文の引数が0です");
			}
			break;
		case FunctionCode.THROW:
			throw new CodeEE(((ExpressionArgument)func.Argument).Term.GetStrValue(exm));
		case FunctionCode.STRDATA:
		{
			if (func.dataList.Count == 0)
			{
				state.JumpTo(func.JumpTo);
				break;
			}
			int num2 = func.dataList.Count;
			int index = (int)exm.VEvaluator.GetNextRand(num2);
			List<InstructionLine> list = func.dataList[index];
			int num3 = 0;
			foreach (InstructionLine item in list)
			{
				state.CurrentLine = item;
				if (item.Argument == null)
				{
					ArgumentParser.SetArgumentTo(item);
				}
				operandTerm = ((ExpressionArgument)item.Argument).Term;
				text += operandTerm.GetStrValue(exm);
				if (++num3 < list.Count)
				{
					text += "\n";
				}
			}
			((StrDataArgument)func.Argument).Var.SetValue(text, exm);
			state.JumpTo(func.JumpTo);
			break;
		}
		}
	}

	private bool doFlowControlFunction(InstructionLine func)
	{
		switch (func.FunctionCode)
		{
		case FunctionCode.LOADDATA:
		{
			long intValue3 = ((ExpressionArgument)func.Argument).Term.GetIntValue(exm);
			if (intValue3 < 0)
			{
				throw new CodeEE("LOADDATAの引数に負の値(" + intValue3 + ")が指定されました");
			}
			if (intValue3 > int.MaxValue)
			{
				throw new CodeEE("LOADDATAの引数(" + intValue3 + ")が大きすぎます");
			}
			if (vEvaluator.CheckData((int)intValue3, EraSaveFileType.Normal).State != EraDataState.OK)
			{
				throw new CodeEE("不正なデータをロードしようとしました");
			}
			if (!vEvaluator.LoadFrom((int)intValue3))
			{
				throw new ExeEE("ファイルのロード中に予期しないエラーが発生しました");
			}
			state.ClearFunctionList();
			state.SystemState = SystemStateCode.LoadData_DataLoaded;
			return false;
		}
		case FunctionCode.TRYCALLLIST:
		case FunctionCode.TRYJUMPLIST:
		{
			string text2 = "";
			CalledFunction calledFunction = null;
			SpCallArgment spCallArgment = null;
			foreach (InstructionLine call in func.callList)
			{
				spCallArgment = (SpCallArgment)call.Argument;
				text2 = spCallArgment.FuncnameTerm.GetStrValue(exm);
				if (Config.ICFunction)
				{
					text2 = text2.ToUpper();
				}
				calledFunction = CalledFunction.CallFunction(this, text2, func.JumpTo);
				if (calledFunction != null)
				{
					calledFunction.IsJump = func.Function.IsJump();
					string errMes;
					UserDefinedFunctionArgument userDefinedFunctionArgument = calledFunction.ConvertArg(spCallArgment.RowArgs, out errMes);
					if (userDefinedFunctionArgument == null)
					{
						throw new CodeEE(errMes);
					}
					state.IntoFunction(calledFunction, userDefinedFunctionArgument, exm);
					return true;
				}
			}
			state.JumpTo(func.JumpTo);
			break;
		}
		case FunctionCode.TRYGOTOLIST:
		{
			string text = "";
			LogicalLine logicalLine = null;
			foreach (InstructionLine call2 in func.callList)
			{
				if (call2.Argument == null)
				{
					ArgumentParser.SetArgumentTo(call2);
				}
				text = ((SpCallArgment)call2.Argument).FuncnameTerm.GetStrValue(exm);
				if (Config.ICVariable)
				{
					text = text.ToUpper();
				}
				logicalLine = state.CurrentCalled.CallLabel(this, text);
				if (logicalLine != null)
				{
					break;
				}
			}
			if (logicalLine == null)
			{
				state.JumpTo(func.JumpTo);
			}
			else
			{
				state.JumpTo(logicalLine);
			}
			break;
		}
		case FunctionCode.CALLTRAIN:
		{
			long intValue2 = ((ExpressionArgument)func.Argument).Term.GetIntValue(exm);
			SetCommnds(intValue2);
			return false;
		}
		case FunctionCode.STOPCALLTRAIN:
			if (isCTrain)
			{
				ClearCommands();
				skipPrint = false;
			}
			return false;
		case FunctionCode.DOTRAIN:
		{
			SystemStateCode systemState = state.SystemState;
			if ((uint)(systemState - 17) > 1u && systemState != SystemStateCode.Train_CallShowUserCom && systemState != SystemStateCode.Train_CallEventComEnd)
			{
				exm.Console.PrintSystemLine(state.SystemState.ToString());
				throw new CodeEE("DOTRAIN命令をこの位置で実行することはできません");
			}
			coms.Clear();
			isCTrain = false;
			count = 0;
			long intValue = ((ExpressionArgument)func.Argument).Term.GetIntValue(exm);
			if (intValue < 0)
			{
				throw new CodeEE("DOTRAIN命令に0未満の値が渡されました");
			}
			if (intValue >= TrainName.Length)
			{
				throw new CodeEE("DOTRAIN命令にTRAINNAMEの配列数以上の値が渡されました");
			}
			doTrainSelectCom = intValue;
			state.SystemState = SystemStateCode.Train_DoTrain;
			return false;
		}
		}
		return true;
	}

	public void saveCurrentState(bool single)
	{
		if (state != null)
		{
			prevStateList.Add(state);
			state = state.Clone();
		}
	}

	public void loadPrevState()
	{
		if (state != null)
		{
			state.ClearFunctionList();
			state = prevStateList[prevStateList.Count - 1];
			deletePrevState();
		}
	}

	private void deletePrevState()
	{
		if (prevStateList.Count != 0)
		{
			prevStateList.RemoveAt(prevStateList.Count - 1);
		}
	}

	private void deleteAllPrevState()
	{
		foreach (ProcessState prevState in prevStateList)
		{
			prevState.ClearFunctionList();
		}
		prevStateList.Clear();
	}

	private void initSystemProcess()
	{
		comAble = new int[TrainName.Length];
		systemProcessDictionary.Add(SystemStateCode.Title_Begin, beginTitle);
		systemProcessDictionary.Add(SystemStateCode.Openning, endOpenning);
		systemProcessDictionary.Add(SystemStateCode.Train_Begin, beginTrain);
		systemProcessDictionary.Add(SystemStateCode.Train_CallEventTrain, endCallEventTrain);
		systemProcessDictionary.Add(SystemStateCode.Train_CallShowStatus, endCallShowStatus);
		systemProcessDictionary.Add(SystemStateCode.Train_CallComAbleXX, endCallComAbleXX);
		systemProcessDictionary.Add(SystemStateCode.Train_CallShowUserCom, endCallShowUserCom);
		systemProcessDictionary.Add(SystemStateCode.Train_WaitInput, trainWaitInput);
		systemProcessDictionary.Add(SystemStateCode.Train_CallEventCom, endEventCom);
		systemProcessDictionary.Add(SystemStateCode.Train_CallComXX, endCallComXX);
		systemProcessDictionary.Add(SystemStateCode.Train_CallSourceCheck, endCallSourceCheck);
		systemProcessDictionary.Add(SystemStateCode.Train_CallEventComEnd, endCallEventComEnd);
		systemProcessDictionary.Add(SystemStateCode.Train_DoTrain, doTrain);
		systemProcessDictionary.Add(SystemStateCode.AfterTrain_Begin, beginAfterTrain);
		systemProcessDictionary.Add(SystemStateCode.Ablup_Begin, beginAblup);
		systemProcessDictionary.Add(SystemStateCode.Ablup_CallShowJuel, endCallShowJuel);
		systemProcessDictionary.Add(SystemStateCode.Ablup_CallShowAblupSelect, endCallShowAblupSelect);
		systemProcessDictionary.Add(SystemStateCode.Ablup_WaitInput, ablupWaitInput);
		systemProcessDictionary.Add(SystemStateCode.Ablup_CallAblupXX, endCallAblupXX);
		systemProcessDictionary.Add(SystemStateCode.Turnend_Begin, beginTurnend);
		systemProcessDictionary.Add(SystemStateCode.Shop_Begin, beginShop);
		systemProcessDictionary.Add(SystemStateCode.Shop_CallEventShop, endCallEventShop);
		systemProcessDictionary.Add(SystemStateCode.Shop_CallShowShop, endCallShowShop);
		systemProcessDictionary.Add(SystemStateCode.Shop_WaitInput, shopWaitInput);
		systemProcessDictionary.Add(SystemStateCode.Shop_CallEventBuy, endCallEventBuy);
		systemProcessDictionary.Add(SystemStateCode.SaveGame_Begin, beginSaveGame);
		systemProcessDictionary.Add(SystemStateCode.SaveGame_WaitInput, saveGameWaitInput);
		systemProcessDictionary.Add(SystemStateCode.SaveGame_WaitInputOverwrite, saveGameWaitInputOverwrite);
		systemProcessDictionary.Add(SystemStateCode.SaveGame_CallSaveInfo, endCallSaveInfo);
		systemProcessDictionary.Add(SystemStateCode.LoadGame_Begin, beginLoadGame);
		systemProcessDictionary.Add(SystemStateCode.LoadGame_WaitInput, loadGameWaitInput);
		systemProcessDictionary.Add(SystemStateCode.LoadGameOpenning_Begin, beginLoadGameOpening);
		systemProcessDictionary.Add(SystemStateCode.LoadGameOpenning_WaitInput, loadGameWaitInput);
		systemProcessDictionary.Add(SystemStateCode.AutoSave_CallSaveInfo, endAutoSaveCallSaveInfo);
		systemProcessDictionary.Add(SystemStateCode.AutoSave_CallUniqueAutosave, endAutoSave);
		systemProcessDictionary.Add(SystemStateCode.LoadData_DataLoaded, beginDataLoaded);
		systemProcessDictionary.Add(SystemStateCode.LoadData_CallSystemLoad, endSystemLoad);
		systemProcessDictionary.Add(SystemStateCode.LoadData_CallEventLoad, endEventLoad);
		systemProcessDictionary.Add(SystemStateCode.Openning_TitleLoadgame, endTitleLoadgame);
		systemProcessDictionary.Add(SystemStateCode.System_Reloaderb, endReloaderb);
		systemProcessDictionary.Add(SystemStateCode.First_Begin, beginFirst);
		systemProcessDictionary.Add(SystemStateCode.Normal, endNormal);
	}

	private void runSystemProc()
	{
		systemProcessDictionary[state.SystemState]();
	}

	private void setWait()
	{
		console.ReadAnyKey();
	}

	private void setWaitInput()
	{
		InputRequest inputRequest = new InputRequest();
		inputRequest.InputType = InputType.IntValue;
		inputRequest.IsSystemInput = true;
		console.WaitInput(inputRequest);
	}

	private bool callFunction(string functionName, bool force, bool isEvent)
	{
		CalledFunction calledFunction = null;
		calledFunction = ((!isEvent) ? CalledFunction.CallFunction(this, functionName, null) : CalledFunction.CallEventFunction(this, functionName, null));
		if (calledFunction == null)
		{
			if (!force)
			{
				return false;
			}
			throw new CodeEE("関数\"@" + functionName + "\"が見つかりません");
		}
		state.IntoFunction(calledFunction, null, null);
		return true;
	}

	private void beginTitle()
	{
		if (isCTrain && ClearCommands())
		{
			return;
		}
		skipPrint = false;
		console.ResetStyle();
		deleteAllPrevState();
		if (Program.AnalysisMode)
		{
			console.PrintSystemLine("ファイル解析終了：Analysis.logに出力します");
			console.OutputLog(Program.ExeDir + "Analysis.log");
			console.noOutputLog = true;
			console.PrintSystemLine("エンターキーもしくはクリックで終了します");
			console.ThrowTitleError(error: false);
			return;
		}
		if (!noError && !Config.CompatiErrorLine)
		{
			console.PrintSystemLine("ERBコードに解釈不可能な行があるためEmueraを終了します");
			console.PrintSystemLine("※互換性オプション「" + Config.GetConfigName(ConfigCode.CompatiErrorLine) + "」により強制的に動作させることができます");
			console.PrintSystemLine("emuera.logにログを出力します");
			console.OutputLog(Program.ExeDir + "emuera.log");
			console.noOutputLog = true;
			console.PrintSystemLine("エンターキーもしくはクリックで終了します");
			console.ThrowTitleError(error: true);
			return;
		}
		if (callFunction("SYSTEM_TITLE", force: false, isEvent: false))
		{
			state.SystemState = SystemStateCode.Normal;
			return;
		}
		console.PrintBar();
		console.NewLine();
		console.Alignment = DisplayLineAlignment.CENTER;
		console.PrintSingleLine(gamebase.ScriptTitle);
		if (gamebase.ScriptVersion != 0L)
		{
			console.PrintSingleLine(gamebase.ScriptVersionText);
		}
		console.PrintSingleLine(gamebase.ScriptAutherName);
		console.PrintSingleLine("(" + gamebase.ScriptYear + ")");
		console.NewLine();
		console.PrintSingleLine(gamebase.ScriptDetail);
		console.Alignment = DisplayLineAlignment.LEFT;
		console.PrintBar();
		console.NewLine();
		console.PrintSingleLine("[0] " + Config.TitleMenuString0);
		console.PrintSingleLine("[1] " + Config.TitleMenuString1);
		openingInput();
	}

	private void openingInput()
	{
		setWaitInput();
		state.SystemState = SystemStateCode.Openning;
	}

	private void endOpenning()
	{
		if (systemResult == 0L)
		{
			vEvaluator.ResetData();
			vEvaluator.AddCharacterFromCsvNo(0L);
			if (gamebase.DefaultCharacter > 0)
			{
				vEvaluator.AddCharacterFromCsvNo(gamebase.DefaultCharacter);
			}
			console.PrintBar();
			console.NewLine();
			beginFirst();
		}
		else if (systemResult == 1)
		{
			if (callFunction("TITLE_LOADGAME", force: false, isEvent: false))
			{
				state.SystemState = SystemStateCode.Openning_TitleLoadgame;
			}
			else
			{
				beginLoadGameOpening();
			}
		}
		else
		{
			console.deleteLine(1);
			console.PrintTemporaryLine("無効な値です");
			console.updatedGeneration = true;
			openingInput();
		}
	}

	private void beginFirst()
	{
		state.SystemState = SystemStateCode.Normal;
		if (!isCTrain || !ClearCommands())
		{
			skipPrint = false;
			callFunction("EVENTFIRST", force: true, isEvent: true);
		}
	}

	private void endTitleLoadgame()
	{
		beginTitle();
	}

	private void beginTrain()
	{
		vEvaluator.UpdateInBeginTrain();
		state.SystemState = SystemStateCode.Train_CallEventTrain;
		if (!callFunction("EVENTTRAIN", force: false, isEvent: true))
		{
			endCallEventTrain();
		}
	}

	private void endCallEventTrain()
	{
		if (vEvaluator.NEXTCOM >= 0)
		{
			state.SystemState = SystemStateCode.Train_CallEventCom;
			vEvaluator.SELECTCOM = vEvaluator.NEXTCOM;
			vEvaluator.NEXTCOM = 0L;
			callEventCom();
			return;
		}
		if (isCTrain)
		{
			skipPrint = true;
		}
		callFunction("SHOW_STATUS", force: true, isEvent: false);
		state.SystemState = SystemStateCode.Train_CallShowStatus;
	}

	private void endCallShowStatus()
	{
		state.SystemState = SystemStateCode.Train_CallComAbleXX;
		lastCalledComable = -1;
		lastAddCom = -1;
		printComCount = 0;
		for (int i = 0; i < comAble.Length; i++)
		{
			comAble[i] = -1;
		}
		endCallComAbleXX();
	}

	private string getTrainComString(int trainCode, int comNo)
	{
		string arg = TrainName[trainCode];
		return $"{arg}[{comNo,3}]";
	}

	private void endCallComAbleXX()
	{
		if (lastCalledComable >= 0 && TrainName[lastCalledComable] != null)
		{
			lastAddCom++;
			if (vEvaluator.RESULT != 0L)
			{
				comAble[lastAddCom] = lastCalledComable;
				if (!isCTrain)
				{
					console.PrintC(getTrainComString(lastCalledComable, lastAddCom), alignmentRight: true);
					printComCount++;
					if (Config.PrintCPerLine > 0 && printComCount % Config.PrintCPerLine == 0)
					{
						console.PrintFlush(force: false);
					}
				}
				console.RefreshStrings(force_Paint: false);
			}
		}
		while (++lastCalledComable < TrainName.Length)
		{
			if (TrainName[lastCalledComable] == null)
			{
				continue;
			}
			string functionName = $"COM_ABLE{lastCalledComable}";
			if (!callFunction(functionName, force: false, isEvent: false))
			{
				lastAddCom++;
				if (Config.ComAbleDefault == 0)
				{
					continue;
				}
				comAble[lastAddCom] = lastCalledComable;
				if (!isCTrain)
				{
					console.PrintC(getTrainComString(lastCalledComable, lastAddCom), alignmentRight: true);
					printComCount++;
					if (Config.PrintCPerLine > 0 && printComCount % Config.PrintCPerLine == 0)
					{
						console.PrintFlush(force: false);
					}
				}
				continue;
			}
			console.RefreshStrings(force_Paint: false);
			return;
		}
		if (lastCalledComable >= TrainName.Length)
		{
			state.SystemState = SystemStateCode.Train_CallShowUserCom;
			console.PrintFlush(force: false);
			console.RefreshStrings(force_Paint: false);
			callFunction("SHOW_USERCOM", force: true, isEvent: false);
		}
	}

	private void endCallShowUserCom()
	{
		if (skipPrint)
		{
			skipPrint = false;
		}
		vEvaluator.UpdateAfterShowUsercom();
		if (!isCTrain)
		{
			setWaitInput();
			state.SystemState = SystemStateCode.Train_WaitInput;
		}
		else if (count < coms.Count)
		{
			systemResult = coms[count];
			count++;
			trainWaitInput();
		}
	}

	private void trainWaitInput()
	{
		int num = -1;
		if (!isCTrain)
		{
			if (systemResult >= 0 && systemResult < comAble.Length)
			{
				num = comAble[systemResult];
			}
		}
		else
		{
			for (int i = 0; i < comAble.Length; i++)
			{
				if (comAble[i] == systemResult)
				{
					num = (int)systemResult;
				}
			}
			console.PrintSingleLine($"＜コマンド連続実行：{count}/{coms.Count}＞");
		}
		if (num >= 0)
		{
			vEvaluator.SELECTCOM = num;
			callEventCom();
			return;
		}
		if (isCTrain)
		{
			console.PrintSingleLine("コマンドを実行できませんでした");
		}
		vEvaluator.RESULT = systemResult;
		state.SystemState = SystemStateCode.Train_CallEventComEnd;
		callFunction("USERCOM", force: true, isEvent: false);
	}

	private void doTrain()
	{
		vEvaluator.UpdateAfterShowUsercom();
		vEvaluator.SELECTCOM = doTrainSelectCom;
		callEventCom();
	}

	private void callEventCom()
	{
		vEvaluator.UpdateAfterInputCom();
		state.SystemState = SystemStateCode.Train_CallEventCom;
		if (!callFunction("EVENTCOM", force: false, isEvent: true))
		{
			endEventCom();
		}
	}

	private void endEventCom()
	{
		long sELECTCOM = vEvaluator.SELECTCOM;
		string functionName = $"COM{sELECTCOM}";
		state.SystemState = SystemStateCode.Train_CallComXX;
		callFunction(functionName, force: true, isEvent: false);
	}

	private void endCallComXX()
	{
		if (vEvaluator.RESULT == 0L)
		{
			endCallEventComEnd();
			return;
		}
		state.SystemState = SystemStateCode.Train_CallSourceCheck;
		callFunction("SOURCE_CHECK", force: true, isEvent: false);
	}

	private void endCallSourceCheck()
	{
		vEvaluator.UpdateAfterSourceCheck();
		state.SystemState = SystemStateCode.Train_CallEventComEnd;
		NeedWaitToEventComEnd = true;
		if (!callFunction("EVENTCOMEND", force: false, isEvent: true))
		{
			endCallEventComEnd();
		}
	}

	private void endCallEventComEnd()
	{
		if (console.LastLineIsTemporary && !isCTrain && needCheck)
		{
			console.deleteLine(2);
			console.PrintTemporaryLine("無効な値です");
			console.updatedGeneration = true;
			endCallShowUserCom();
			return;
		}
		if (isCTrain && count == coms.Count)
		{
			isCTrain = false;
			skipPrint = false;
			coms.Clear();
			count = 0;
			if (callFunction("CALLTRAINEND", force: false, isEvent: false))
			{
				needCheck = false;
				return;
			}
		}
		needCheck = true;
		if (NeedWaitToEventComEnd)
		{
			setWait();
		}
		NeedWaitToEventComEnd = false;
		endCallEventTrain();
	}

	private void beginAfterTrain()
	{
		if (!isCTrain || !ClearCommands())
		{
			skipPrint = false;
			state.SystemState = SystemStateCode.Normal;
			callFunction("EVENTEND", force: true, isEvent: true);
		}
	}

	private void beginAblup()
	{
		if (!isCTrain || !ClearCommands())
		{
			skipPrint = false;
			state.SystemState = SystemStateCode.Ablup_CallShowJuel;
			callFunction("SHOW_JUEL", force: true, isEvent: false);
		}
	}

	private void endCallShowJuel()
	{
		state.SystemState = SystemStateCode.Ablup_CallShowAblupSelect;
		callFunction("SHOW_ABLUP_SELECT", force: true, isEvent: false);
	}

	private void endCallShowAblupSelect()
	{
		setWaitInput();
		state.SystemState = SystemStateCode.Ablup_WaitInput;
	}

	private void ablupWaitInput()
	{
		if (systemResult >= 0 && systemResult < 100)
		{
			state.SystemState = SystemStateCode.Ablup_CallAblupXX;
			string functionName = $"ABLUP{systemResult}";
			if (!callFunction(functionName, force: false, isEvent: false))
			{
				console.deleteLine(1);
				console.PrintTemporaryLine("無効な値です");
				console.updatedGeneration = true;
				endCallShowAblupSelect();
			}
		}
		else
		{
			vEvaluator.RESULT = systemResult;
			state.SystemState = SystemStateCode.Ablup_CallAblupXX;
			callFunction("USERABLUP", force: true, isEvent: false);
		}
	}

	private void endCallAblupXX()
	{
		if (console.LastLineIsTemporary)
		{
			console.deleteLine(2);
			console.PrintTemporaryLine("無効な値です");
			console.updatedGeneration = true;
			endCallShowAblupSelect();
		}
		else
		{
			beginAblup();
		}
	}

	private void beginTurnend()
	{
		if (!isCTrain || !ClearCommands())
		{
			skipPrint = false;
			callFunction("EVENTTURNEND", force: true, isEvent: true);
			state.SystemState = SystemStateCode.Normal;
		}
	}

	private void beginShop()
	{
		if (!isCTrain || !ClearCommands())
		{
			skipPrint = false;
			state.SystemState = SystemStateCode.Shop_CallEventShop;
			if (!callFunction("EVENTSHOP", force: false, isEvent: true))
			{
				endCallEventShop();
			}
		}
	}

	private void endCallEventShop()
	{
		saveTarget = -1;
		if (Config.AutoSave && state.calledWhenNormal)
		{
			beginAutoSave();
			return;
		}
		state.SystemState = SystemStateCode.AutoSave_Skipped;
		endAutoSaveCallSaveInfo();
	}

	private void beginAutoSave()
	{
		if (callFunction("SYSTEM_AUTOSAVE", force: false, isEvent: false))
		{
			state.SystemState = SystemStateCode.AutoSave_CallUniqueAutosave;
			return;
		}
		saveTarget = 99;
		vEvaluator.SAVEDATA_TEXT = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " ";
		state.SystemState = SystemStateCode.AutoSave_CallSaveInfo;
		if (!callFunction("SAVEINFO", force: false, isEvent: false))
		{
			endAutoSaveCallSaveInfo();
		}
	}

	private void endAutoSaveCallSaveInfo()
	{
		if (saveTarget == 99 && !vEvaluator.SaveTo(saveTarget, vEvaluator.SAVEDATA_TEXT))
		{
			console.PrintError("オートセーブ中に予期しないエラーが発生しました");
			console.PrintError("オートセーブをスキップします");
			console.ReadAnyKey();
		}
		endAutoSave();
	}

	private void endAutoSave()
	{
		if (state.isBegun)
		{
			state.Begin();
			return;
		}
		state.SystemState = SystemStateCode.Shop_CallShowShop;
		callFunction("SHOW_SHOP", force: true, isEvent: false);
	}

	private void endCallShowShop()
	{
		setWaitInput();
		state.SystemState = SystemStateCode.Shop_WaitInput;
	}

	private void shopWaitInput()
	{
		if (systemResult >= 0 && systemResult < Config.MaxShopItem)
		{
			if (vEvaluator.ItemSales(systemResult))
			{
				if (vEvaluator.BuyItem(systemResult))
				{
					state.SystemState = SystemStateCode.Shop_CallEventBuy;
					if (!callFunction("EVENTBUY", force: false, isEvent: true))
					{
						endCallEventBuy();
					}
					return;
				}
				console.deleteLine(1);
				console.PrintTemporaryLine("お金が足りません。");
			}
			else
			{
				console.deleteLine(1);
				console.PrintTemporaryLine("売っていません。");
			}
			endCallShowShop();
		}
		else
		{
			vEvaluator.RESULT = systemResult;
			callFunction("USERSHOP", force: true, isEvent: false);
			state.SystemState = SystemStateCode.Shop_CallEventBuy;
		}
	}

	private void endCallEventBuy()
	{
		if (console.LastLineIsTemporary)
		{
			console.deleteLine(2);
			console.PrintTemporaryLine("無効な値です");
			console.updatedGeneration = true;
			endCallShowShop();
		}
		else
		{
			endAutoSave();
		}
	}

	private void beginDataLoaded()
	{
		state.SystemState = SystemStateCode.LoadData_CallSystemLoad;
		if (!callFunction("SYSTEM_LOADEND", force: false, isEvent: false))
		{
			endSystemLoad();
		}
	}

	private void endSystemLoad()
	{
		state.SystemState = SystemStateCode.LoadData_CallEventLoad;
		if (!callFunction("EVENTLOAD", force: false, isEvent: true))
		{
			endAutoSave();
		}
	}

	private void endEventLoad()
	{
		endAutoSave();
	}

	private void beginSaveGame()
	{
		console.PrintSingleLine("何番にセーブしますか？");
		state.SystemState = SystemStateCode.SaveGame_Begin;
		printSaveDataText();
	}

	private void beginLoadGame()
	{
		console.PrintSingleLine("何番をロードしますか？");
		state.SystemState = SystemStateCode.LoadGame_Begin;
		printSaveDataText();
	}

	private void beginLoadGameOpening()
	{
		console.PrintSingleLine("何番をロードしますか？");
		state.SystemState = SystemStateCode.LoadGameOpenning_Begin;
		printSaveDataText();
	}

	private void printSaveDataText()
	{
		if (isFirstTime)
		{
			isFirstTime = false;
			dataIsAvailable = new bool[Config.SaveDataNos + 1];
		}
		int num = 0;
		for (int i = 0; i < page; i++)
		{
			console.PrintFlush(force: false);
			console.Print(string.Format("[{0, 2}] セーブデータ{0, 2}～{1, 2}を表示", i * 20, i * 20 + 19));
		}
		for (int j = 0; j < 20; j++)
		{
			num = page * 20 + j;
			if (num == dataIsAvailable.Length - 1)
			{
				break;
			}
			dataIsAvailable[num] = false;
			console.PrintFlush(force: false);
			console.Print($"[{num,2}] ");
			if (writeSavedataTextFrom(num))
			{
				dataIsAvailable[num] = true;
			}
		}
		for (int k = page; k < (dataIsAvailable.Length - 2) / 20; k++)
		{
			console.PrintFlush(force: false);
			console.Print(string.Format("[{0, 2}] セーブデータ{0, 2}～{1, 2}を表示", (k + 1) * 20, (k + 1) * 20 + 19));
		}
		dataIsAvailable[dataIsAvailable.Length - 1] = false;
		if (state.SystemState != SystemStateCode.SaveGame_Begin)
		{
			num = 99;
			console.PrintFlush(force: false);
			console.Print($"[{num,2}] ");
			if (writeSavedataTextFrom(num))
			{
				dataIsAvailable[dataIsAvailable.Length - 1] = true;
			}
		}
		console.RefreshStrings(force_Paint: false);
		console.PrintSingleLine("[100] 戻る");
		setWaitInput();
		if (state.SystemState == SystemStateCode.SaveGame_Begin)
		{
			state.SystemState = SystemStateCode.SaveGame_WaitInput;
		}
		else if (state.SystemState == SystemStateCode.LoadGame_Begin)
		{
			state.SystemState = SystemStateCode.LoadGame_WaitInput;
		}
		else
		{
			state.SystemState = SystemStateCode.LoadGameOpenning_WaitInput;
		}
	}

	private void saveGameWaitInput()
	{
		if (systemResult == 100)
		{
			loadPrevState();
			return;
		}
		if ((int)systemResult / 20 != page && systemResult != 99 && systemResult >= 0 && systemResult < dataIsAvailable.Length - 1)
		{
			page = (int)systemResult / 20;
			state.SystemState = SystemStateCode.SaveGame_Begin;
			printSaveDataText();
			return;
		}
		bool flag = false;
		if (systemResult >= 0 && systemResult < dataIsAvailable.Length - 1)
		{
			flag = dataIsAvailable[systemResult];
			saveTarget = (int)systemResult;
			if (flag)
			{
				console.PrintSingleLine("既にデータが存在します。上書きしますか？");
				console.PrintC("[0] はい", alignmentRight: false);
				console.PrintC("[1] いいえ", alignmentRight: false);
				setWaitInput();
				state.SystemState = SystemStateCode.SaveGame_WaitInputOverwrite;
			}
			else
			{
				systemResult = 0L;
				saveGameWaitInputOverwrite();
			}
		}
		else
		{
			console.deleteLine(1);
			console.PrintTemporaryLine("無効な値です");
			console.updatedGeneration = true;
			setWaitInput();
		}
	}

	private void saveGameWaitInputOverwrite()
	{
		if (systemResult == 1)
		{
			beginSaveGame();
			return;
		}
		if (systemResult != 0L)
		{
			console.deleteLine(1);
			console.PrintTemporaryLine("無効な値です");
			console.updatedGeneration = true;
			setWaitInput();
			return;
		}
		vEvaluator.SAVEDATA_TEXT = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " ";
		state.SystemState = SystemStateCode.SaveGame_CallSaveInfo;
		if (!callFunction("SAVEINFO", force: false, isEvent: false))
		{
			endCallSaveInfo();
		}
	}

	private void endCallSaveInfo()
	{
		if (!vEvaluator.SaveTo(saveTarget, vEvaluator.SAVEDATA_TEXT))
		{
			console.PrintError("セーブ中に予期しないエラーが発生しました");
			console.ReadAnyKey();
		}
		loadPrevState();
	}

	private void loadGameWaitInput()
	{
		if (systemResult == 100)
		{
			if (state.SystemState == SystemStateCode.LoadGameOpenning_WaitInput)
			{
				beginTitle();
			}
			else
			{
				loadPrevState();
			}
			return;
		}
		if ((int)systemResult / 20 != page && systemResult != 99 && systemResult >= 0 && systemResult < dataIsAvailable.Length - 1)
		{
			page = (int)systemResult / 20;
			if (state.SystemState == SystemStateCode.LoadGameOpenning_WaitInput)
			{
				state.SystemState = SystemStateCode.LoadGameOpenning_Begin;
			}
			else
			{
				state.SystemState = SystemStateCode.LoadGame_Begin;
			}
			printSaveDataText();
			return;
		}
		bool flag = false;
		if (systemResult >= 0 && systemResult < dataIsAvailable.Length - 1)
		{
			flag = dataIsAvailable[systemResult];
		}
		else
		{
			if (systemResult != 99)
			{
				console.deleteLine(1);
				console.PrintTemporaryLine("無効な値です");
				console.updatedGeneration = true;
				setWaitInput();
				return;
			}
			flag = dataIsAvailable[dataIsAvailable.Length - 1];
		}
		if (!flag)
		{
			console.PrintSingleLine(systemResult.ToString());
			console.PrintError("データがありません");
			if (state.SystemState == SystemStateCode.LoadGameOpenning_WaitInput)
			{
				beginLoadGameOpening();
			}
			else
			{
				beginLoadGame();
			}
		}
		else
		{
			if (!vEvaluator.LoadFrom((int)systemResult))
			{
				throw new ExeEE("ファイルのロード中に予期しないエラーが発生しました");
			}
			deletePrevState();
			beginDataLoaded();
		}
	}

	private void endNormal()
	{
		throw new CodeEE("予期しないスクリプト終端です");
	}

	private void endReloaderb()
	{
		loadPrevState();
		console.ReloadErbFinished();
	}

	private bool writeSavedataTextFrom(int saveIndex)
	{
		EraDataResult eraDataResult = vEvaluator.CheckData(saveIndex, EraSaveFileType.Normal);
		console.Print(eraDataResult.DataMes);
		console.NewLine();
		return eraDataResult.State == EraDataState.OK;
	}
}
