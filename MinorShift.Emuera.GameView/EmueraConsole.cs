using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Timers;
using Android.App;
using Android.Graphics;
using Android.Util;
using MinorShift._Library;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameProc;
using MinorShift.Emuera.GameProc.Function;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameView;

public sealed class EmueraConsole : IDisposable
{
	private const string ErrorButtonsText = "__openFileWithDebug__";

	private MinorShift.Emuera.GameProc.Process emuera;

	internal ConsoleState state;

	public bool notToTitle;

	public bool byError;

	private bool lastButtonIsInput = true;

	public bool updatedGeneration;

	private int lastButtonGeneration;

	private int newButtonGeneration;

	private LogicalLine lastInputLine;

	private ConsoleButtonString selectingButton;

	private ConsoleButtonString lastSelectingButton;

	private bool tooltipUsed;

	private ConsoleButtonString pointingString;

	private ConsoleButtonString lastPointingString;

	private InputRequest inputReq;

	private System.Timers.Timer timer;

	private long timerID = -1L;

	private int countTime;

	private bool wait_timeout;

	private bool isTimeout;

	private readonly string[] spliter = new string[4] { "\\n", "\r\n", "\n", "\r" };

	public bool MesSkip;

	private bool inProcess;

	public volatile bool KillMacro;

	private bool runningERBfromMemory;

	private uint lastUpdate;

	private uint msPerFrame = 16u;

	private ConsoleRedraw redraw = ConsoleRedraw.Normal;

	private string debugTitle;

	private StringBuilder dConsoleLog = new StringBuilder("");

	private List<string> dTraceLogList = new List<string>();

	private bool dTraceLogChanged = true;

	private bool force_temporary;

	private bool timer_suspended;

	private ConsoleState prevState;

	private InputRequest prevReq;

	private readonly List<ConsoleDisplayLine> displayLineList;

	public bool noOutputLog;

	public Android.Graphics.Color bgColor = Config.BackColor;

	private readonly PrintStringBuffer printBuffer;

	private readonly StringMeasure stringMeasure = new StringMeasure();

	private StringStyle defaultStyle = new StringStyle(Config.ForeColor, TypefaceStyle.Normal, null);

	private StringStyle userStyle = new StringStyle(Config.ForeColor, TypefaceStyle.Normal, null);

	private DisplayLineAlignment alignment;

	private string stBar;

	private uint lastBgColorChange;

	private bool forceTextBoxColor;

	private int lastDrawnLineNo = -1;

	private int lineNo;

	private long logicalLineCount;

	private int printCWidth = -1;

	private int printCWidthL = -1;

	private int printCWidthL2 = -1;

	public bool Enabled => GlobalStatic.MainWindow.ApplicationInfo.Enabled;

	internal bool IsRunning
	{
		get
		{
			if (state == ConsoleState.Initializing)
			{
				return true;
			}
			if (state != ConsoleState.Running)
			{
				return runningERBfromMemory;
			}
			return true;
		}
	}

	internal bool IsInProcess
	{
		get
		{
			if (state == ConsoleState.Initializing)
			{
				return true;
			}
			if (inProcess)
			{
				return true;
			}
			if (state != ConsoleState.Running)
			{
				return runningERBfromMemory;
			}
			return true;
		}
	}

	public bool IsError => state == ConsoleState.Error;

	public bool IsWaitingEnterKey
	{
		get
		{
			if (state == ConsoleState.Quit || state == ConsoleState.Error)
			{
				return true;
			}
			if (state == ConsoleState.WaitInput)
			{
				if (inputReq.InputType != InputType.AnyKey)
				{
					return inputReq.InputType == InputType.EnterKey;
				}
				return true;
			}
			return false;
		}
	}

	internal bool IsWaitAnyKey
	{
		get
		{
			if (state == ConsoleState.WaitInput)
			{
				return inputReq.InputType == InputType.AnyKey;
			}
			return false;
		}
	}

	public bool IsWaintingOnePhrase
	{
		get
		{
			if (state == ConsoleState.WaitInput)
			{
				return inputReq.OneInput;
			}
			return false;
		}
	}

	internal bool IsRunningTimer
	{
		get
		{
			if (state == ConsoleState.WaitInput && inputReq.Timelimit > 0)
			{
				return !isTimeout;
			}
			return false;
		}
	}

	public string SelectedString
	{
		get
		{
			if (selectingButton == null)
			{
				return null;
			}
			if (state == ConsoleState.Error)
			{
				return selectingButton.Inputs;
			}
			if (state != ConsoleState.WaitInput)
			{
				return null;
			}
			if (inputReq.InputType == InputType.IntValue && selectingButton.IsInteger)
			{
				return selectingButton.Input.ToString();
			}
			if (inputReq.InputType == InputType.StrValue)
			{
				return selectingButton.Inputs;
			}
			return null;
		}
	}

	public int NewButtonGeneration => newButtonGeneration;

	internal ConsoleButtonString SelectingButton => selectingButton;

	public bool IsTimeOut => isTimeout;

	public bool RunERBFromMemory
	{
		get
		{
			return runningERBfromMemory;
		}
		set
		{
			runningERBfromMemory = value;
		}
	}

	internal ConsoleRedraw Redraw => redraw;

	private int MaxShowLineCount => GlobalStatic.FrontEnd.Height / Config.LineHeight;

	private int BottomLineIndex => Math.Min(TopLineIndex + MaxShowLineCount, DisplayLineCount) - 1;

	private int TopLineIndex => Math.Max(0, DisplayLineCount - MaxShowLineCount);

	public string DebugConsoleLog => dConsoleLog.ToString();

	public int DisplayLineCount => displayLineList.Count;

	public bool UseUserStyle { get; set; }

	public bool UseSetColorStyle { get; set; }

	private StringStyle Style
	{
		get
		{
			if (!UseUserStyle)
			{
				return defaultStyle;
			}
			if (UseSetColorStyle)
			{
				return userStyle;
			}
			if (userStyle.Color == defaultStyle.Color)
			{
				return userStyle;
			}
			return new StringStyle(defaultStyle.Color, userStyle.FontStyle, userStyle.Fontname);
		}
	}

	internal StringStyle StringStyle => userStyle;

	internal DisplayLineAlignment Alignment
	{
		get
		{
			return alignment;
		}
		set
		{
			alignment = value;
		}
	}

	public bool EmptyLine => printBuffer.IsEmpty;

	public long LineCount => logicalLineCount;

	public bool LastLineIsTemporary
	{
		get
		{
			if (displayLineList.Count == 0)
			{
				return false;
			}
			return displayLineList[displayLineList.Count - 1].IsTemporary;
		}
	}

	public EmueraConsole(Android.App.Activity parent)
	{
		GlobalStatic.MainWindow = parent;
		state = ConsoleState.Initializing;
		if (Config.FPS > 0)
		{
			msPerFrame = 1000u / (uint)Config.FPS;
		}
		displayLineList = new List<ConsoleDisplayLine>();
		printBuffer = new PrintStringBuffer(this);
        timer = new System.Timers.Timer();
        timer.Enabled = false;
		timer.Elapsed += tickTimer;
		timer.Interval = 100.0;
	}

	public void Initialize()
	{
		GlobalStatic.Console = this;
		emuera = new MinorShift.Emuera.GameProc.Process(this);
		GlobalStatic.Process = emuera;
		ClearDisplay();
		Print("This program is based on Emuera1821");
		NewLine();
		NewLine();
		Print("LICENSE");
		NewLine();
		NewLine();
		Print("Copyright (C) 2008-2015 MinorShift, 妊）|дﾟ)の中の人\r\n\r\n本ソフトウェアは「現状のまま」で、明示であるか暗黙であるかを問わず、何らの保証もなく提供されます。 本ソフトウェアの使用によって生じるいかなる損害についても、作者は一切の責任を負わないものとします。 \r\n\r\n以下の制限に従う限り、商用アプリケーションを含めて、本ソフトウェアを任意の目的に使用し、自由に改変して再頒布することをすべての人に許可します。 \r\n\r\n1.本ソフトウェアの出自について虚偽の表示をしてはなりません。あなたがオリジナルのソフトウェアを作成したと主張してはなりません。 あなたが本ソフトウェアを製品内で使用する場合、製品の文書に謝辞を入れていただければ幸いですが、必須ではありません。 \r\n2.ソースを変更した場合は、そのことを明示しなければなりません。オリジナルのソフトウェアであるという虚偽の表示をしてはなりません。 \r\n3.ソースの頒布物から、この表示を削除したり、表示の内容を変更したりしてはなりません。\r\n");
		PrintBar();
		NewLine();
		if (!emuera.Initialize())
		{
			state = ConsoleState.Error;
			OutputLog(null);
			PrintFlush(force: false);
			RefreshStrings(force_Paint: true);
		}
		else
		{
			callEmueraProgram("");
			RefreshStrings(force_Paint: true);
		}
	}

	public void Quit()
	{
		state = ConsoleState.Quit;
	}

	public void ThrowTitleError(bool error)
	{
		state = ConsoleState.Error;
		notToTitle = true;
		byError = error;
	}

	public void ThrowError(bool playSound)
	{
		forceUpdateGeneration();
		UseUserStyle = false;
		PrintFlush(force: false);
		RefreshStrings(force_Paint: false);
		state = ConsoleState.Error;
	}

	public void UpdateGeneration()
	{
		lastButtonGeneration = newButtonGeneration;
		updatedGeneration = true;
	}

	public void forceUpdateGeneration()
	{
		newButtonGeneration++;
		lastButtonGeneration = newButtonGeneration;
		updatedGeneration = true;
	}

	private void newGeneration()
	{
		if (state != ConsoleState.WaitInput || !inputReq.NeedValue)
		{
			return;
		}
		if (!updatedGeneration && emuera.getCurrentLine != lastInputLine)
		{
			lastButtonGeneration = newButtonGeneration;
		}
		else
		{
			updatedGeneration = false;
		}
		lastInputLine = emuera.getCurrentLine;
		if (inputReq.InputType == InputType.IntValue)
		{
			if (lastButtonGeneration == newButtonGeneration)
			{
				newButtonGeneration++;
			}
			else if (!lastButtonIsInput)
			{
				lastButtonGeneration = newButtonGeneration;
			}
			lastButtonIsInput = true;
		}
		if (inputReq.InputType == InputType.StrValue)
		{
			if (lastButtonGeneration == newButtonGeneration)
			{
				newButtonGeneration++;
			}
			else if (lastButtonIsInput)
			{
				lastButtonGeneration = newButtonGeneration;
			}
			lastButtonIsInput = false;
		}
	}

	internal bool ButtonIsSelected(ConsoleButtonString button)
	{
		return selectingButton == button;
	}

	internal void WaitInput(InputRequest req)
	{
		state = ConsoleState.WaitInput;
		inputReq = req;
		if (req.Timelimit > 0)
		{
			if (req.OneInput)
			{
				GlobalStatic.FrontEnd.update_lastinput();
			}
			setTimer();
		}
	}

	public void ReadAnyKey(bool anykey = false, bool stopMesskip = false)
	{
		InputRequest inputRequest = new InputRequest();
		if (!anykey)
		{
			inputRequest.InputType = InputType.EnterKey;
		}
		else
		{
			inputRequest.InputType = InputType.AnyKey;
		}
		inputRequest.StopMesskip = stopMesskip;
		inputReq = inputRequest;
		state = ConsoleState.WaitInput;
		emuera.NeedWaitToEventComEnd = false;
	}

	private void setTimer()
	{
		countTime = 0;
		isTimeout = false;
		timerID = inputReq.ID;
		timer.Enabled = true;
		if (inputReq.DisplayTime)
		{
			long num = inputReq.Timelimit / 100;
			string text = "残り ";
			string text2 = ((double)num / 10.0).ToString();
			PrintSingleLine(text + text2);
		}
	}

	private void tickTimer(object sender, ElapsedEventArgs e)
	{
		if (!timer.Enabled)
		{
			return;
		}
		if (state != ConsoleState.WaitInput || inputReq.Timelimit <= 0 || timerID != inputReq.ID)
		{
			stopTimer();
			return;
		}
		countTime += 100;
		if (countTime >= inputReq.Timelimit)
		{
			endTimer();
		}
		else if (inputReq.DisplayTime)
		{
			long num = (inputReq.Timelimit - countTime) / 100;
			string text = "残り ";
			string text2 = ((double)num / 10.0).ToString();
			changeLastLine(text + text2);
		}
	}

	private void stopTimer()
	{
		timer.Enabled = false;
	}

	private void endTimer()
	{
		if (!wait_timeout)
		{
			stopTimer();
			isTimeout = true;
			if (inputReq.DisplayTime)
			{
				changeLastLine(inputReq.TimeUpMes);
			}
			else if (inputReq.TimeUpMes != null)
			{
				PrintSingleLine(inputReq.TimeUpMes);
			}
			callEmueraProgram("");
			if (state == ConsoleState.WaitInput && inputReq.NeedValue)
			{
				System.Drawing.Point lastTouchPoint = GlobalStatic.FrontEnd.GetLastTouchPoint();
				MoveMouse(lastTouchPoint);
			}
			RefreshStrings(force_Paint: true);
		}
	}

	public void forceStopTimer()
	{
		if (timer.Enabled)
		{
			timer.Enabled = false;
		}
	}

	internal void callEmueraProgram(string str)
	{
		if (doInputToEmueraProgram(str) && state != ConsoleState.Error)
		{
			state = ConsoleState.Running;
			emuera.DoScript();
			if (state == ConsoleState.Running)
			{
				state = ConsoleState.Error;
				PrintError("emueraのエラー：プログラムの状態を特定できません");
			}
			if (state == ConsoleState.Error && !noOutputLog)
			{
				OutputLog(Program.ExeDir + "emuera.log");
			}
			PrintFlush(force: false);
			newGeneration();
		}
	}

	private bool doInputToEmueraProgram(string str)
	{
		if (state == ConsoleState.WaitInput)
		{
			switch (inputReq.InputType)
			{
			case InputType.IntValue:
			{
				long result;
				if (string.IsNullOrEmpty(str) && inputReq.HasDefValue && !IsRunningTimer)
				{
					result = inputReq.DefIntValue;
					str = result.ToString();
				}
				else if (!long.TryParse(str, out result))
				{
					return false;
				}
				if (inputReq.IsSystemInput)
				{
					emuera.InputSystemInteger(result);
				}
				else
				{
					emuera.InputInteger(result);
				}
				break;
			}
			case InputType.StrValue:
				if (string.IsNullOrEmpty(str) && inputReq.HasDefValue && !IsRunningTimer)
				{
					str = inputReq.DefStrValue;
				}
				if (str == null)
				{
					str = "";
				}
				emuera.InputString(str);
				break;
			}
			stopTimer();
		}
		Print(str);
		PrintFlush(force: false);
		return true;
	}

	public void PressEnterKey(bool keySkip, string str, bool changedByMouse)
	{
		MesSkip = keySkip;
		if (state == ConsoleState.Running || state == ConsoleState.Initializing)
		{
			return;
		}
		if (state == ConsoleState.Quit)
		{
			GlobalStatic.FrontEnd.Close();
			return;
		}
		if (state == ConsoleState.Error)
		{
			if (str == "__openFileWithDebug__" && selectingButton != null && selectingButton.ErrPos != null)
			{
				openErrorFile(selectingButton.ErrPos);
			}
			else
			{
				GlobalStatic.FrontEnd.Close();
			}
			return;
		}
		KillMacro = false;
		try
		{
			if (str.StartsWith("@") && !inputReq.OneInput)
			{
				doSystemCommand(str);
				return;
			}
			if (inputReq.InputType == InputType.Void)
			{
				return;
			}
			if (timer.Enabled && (inputReq.InputType == InputType.AnyKey || inputReq.InputType == InputType.EnterKey))
			{
				stopTimer();
			}
			if (str.Contains("("))
			{
				str = parseInput(new StringStream(str), isNest: false);
			}
			string[] array = str.Split(spliter, StringSplitOptions.None);
			inProcess = true;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (text.IndexOf("\\e") >= 0)
				{
					text = text.Replace("\\e", "");
					MesSkip = true;
				}
				if (inputReq.OneInput && (!Config.AllowLongInputByMouse || !changedByMouse) && text.Length > 1)
				{
					text = text.Remove(1);
				}
				if (inputReq.InputType == InputType.Void)
				{
					i--;
					text = "";
				}
				callEmueraProgram(text);
				RefreshStrings(force_Paint: false);
				while (MesSkip && state == ConsoleState.WaitInput && !inputReq.NeedValue && !inputReq.StopMesskip)
				{
					callEmueraProgram("");
					RefreshStrings(force_Paint: false);
				}
				MesSkip = false;
				if (state != ConsoleState.WaitInput || KillMacro)
				{
					break;
				}
			}
		}
		finally
		{
			inProcess = false;
		}
		if (state == ConsoleState.WaitInput && inputReq.NeedValue)
		{
			System.Drawing.Point lastTouchPoint = GlobalStatic.FrontEnd.GetLastTouchPoint();
			MoveMouse(lastTouchPoint);
		}
		RefreshStrings(force_Paint: true);
		GlobalStatic.FrontEnd.ScrollBar.MoveToEnd();
	}

	private void openErrorFile(ScriptPosition pos)
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = Config.TextEditor;
		string text = pos.Filename.ToUpper();
		if (text.EndsWith(".CSV"))
		{
			if (text.Contains(Program.CsvDir.ToUpper()))
			{
				text = text.Replace(Program.CsvDir.ToUpper(), "");
			}
			text = Program.CsvDir + text;
		}
		else if (!Program.AnalysisMode)
		{
			if (text.Contains(Program.ErbDir.ToUpper()))
			{
				text = text.Replace(Program.ErbDir.ToUpper(), "");
			}
			text = Program.ErbDir + text;
		}
		switch (Config.EditorType)
		{
		case TextEditorType.SAKURA:
			processStartInfo.Arguments = "-Y=" + pos.LineNo + " \"" + text + "\"";
			break;
		case TextEditorType.TERAPAD:
			processStartInfo.Arguments = "/jl=" + pos.LineNo + " \"" + text + "\"";
			break;
		case TextEditorType.EMEDITOR:
			processStartInfo.Arguments = "/l " + pos.LineNo + " \"" + text + "\"";
			break;
		case TextEditorType.USER_SETTING:
			if (Config.EditorArg != "" && Config.EditorArg != null)
			{
				processStartInfo.Arguments = Config.EditorArg + pos.LineNo + " \"" + text + "\"";
			}
			else
			{
				processStartInfo.Arguments = text;
			}
			break;
		}
		try
		{
			System.Diagnostics.Process.Start(processStartInfo);
		}
		catch (Win32Exception)
		{
			PrintError("エディタを開くことができませんでした");
			forceUpdateGeneration();
		}
	}

	private string parseInput(StringStream st, bool isNest)
	{
		StringBuilder stringBuilder = new StringBuilder(20);
		StringBuilder stringBuilder2 = new StringBuilder(20);
		bool flag = false;
		int result = 0;
		while (!st.EOS && (!isNest || st.Current != ')'))
		{
			if (st.Current == '(')
			{
				st.ShiftNext();
				string value = parseInput(st, isNest: true);
				if (!st.EOS)
				{
					st.ShiftNext();
					if (st.Current == '*')
					{
						st.ShiftNext();
						while (char.IsNumber(st.Current))
						{
							stringBuilder2.Append(st.Current);
							st.ShiftNext();
						}
						if (stringBuilder2.ToString() != "" && stringBuilder2.ToString() != null)
						{
							int.TryParse(stringBuilder2.ToString(), out result);
							for (int i = 0; i < result; i++)
							{
								stringBuilder.Append(value);
							}
							stringBuilder2.Remove(0, stringBuilder2.Length);
						}
					}
					else
					{
						stringBuilder.Append(value);
					}
					continue;
				}
				stringBuilder.Append(value);
				break;
			}
			if (st.Current == '\\')
			{
				st.ShiftNext();
				switch (st.Current)
				{
				case 'n':
					if (!flag)
					{
						stringBuilder.Append('\n');
					}
					else
					{
						flag = false;
					}
					break;
				case 'r':
					stringBuilder.Append('\r');
					break;
				case 'e':
					stringBuilder.Append("\\e\n");
					flag = true;
					break;
				default:
					stringBuilder.Append(st.Current);
					break;
				case '\n':
					break;
				}
			}
			else
			{
				stringBuilder.Append(st.Current);
			}
			st.ShiftNext();
		}
		return stringBuilder.ToString();
	}

	private void doSystemCommand(string command)
	{
		if (timer.Enabled)
		{
			PrintError("タイマー系命令の待ち時間中はコマンドを入力できません");
			PrintError("");
			RefreshStrings(force_Paint: true);
			return;
		}
		if (IsInProcess)
		{
			PrintError("スクリプト実行中はコマンドを入力できません");
			RefreshStrings(force_Paint: true);
			return;
		}
		StringComparison sCVariable = Config.SCVariable;
		Print(command);
		PrintFlush(force: false);
		RefreshStrings(force_Paint: true);
		string text = command.Substring(1);
		if (text.Length == 0 || text.Equals("REBOOT", sCVariable))
		{
			return;
		}
		if (text.Equals("OUTPUT", sCVariable) || text.Equals("OUTPUTLOG", sCVariable))
		{
			OutputLog(Program.ExeDir + "emuera.log");
		}
		else
		{
			if (text.Equals("QUIT", sCVariable) || text.Equals("EXIT", sCVariable) || text.Equals("CONFIG", sCVariable))
			{
				return;
			}
			if (text.Equals("DEBUG", sCVariable))
			{
				if (!Program.DebugMode)
				{
					PrintError("デバッグウインドウは-Debug引数付きで起動したときのみ使えます");
					RefreshStrings(force_Paint: true);
					return;
				}
			}
			else
			{
				if (!Config.UseDebugCommand)
				{
					PrintError("デバッグコマンドを使用できない設定になっています");
					RefreshStrings(force_Paint: true);
					return;
				}
				DebugCommand(text, Config.ChangeMasterNameIfDebug, outputDebugConsole: false);
				PrintFlush(force: false);
			}
			RefreshStrings(force_Paint: true);
		}
	}

	public void SetRedraw(long i)
	{
		if ((i & 1) == 0L)
		{
			redraw = ConsoleRedraw.None;
		}
		else
		{
			redraw = ConsoleRedraw.Normal;
		}
		if ((i & 2) != 0L)
		{
			RefreshStrings(force_Paint: true);
		}
	}

	public void SetWindowTitle(string str)
	{
	}

	public void SetEmueraVersionInfo(string str)
	{
	}

	public string GetWindowTitle()
	{
		return "EmueraCS";
	}

	public void RefreshStrings(bool force_Paint)
	{
		bool flag = true;
		if (redraw == ConsoleRedraw.None && !force_Paint && !flag)
		{
			return;
		}
		if (selectingButton != null)
		{
			if (state != ConsoleState.Error && state != ConsoleState.WaitInput)
			{
				selectingButton = null;
			}
			else if (state == ConsoleState.WaitInput && !inputReq.NeedValue)
			{
				selectingButton = null;
			}
			else if (selectingButton.Generation != lastButtonGeneration)
			{
				selectingButton = null;
			}
		}
		if (!force_Paint && ((!flag && lastDrawnLineNo == lineNo && lastSelectingButton == selectingButton) || (WinmmTimer.TickCount - lastUpdate < msPerFrame && (state == ConsoleState.Running || state == ConsoleState.Initializing))))
		{
			return;
		}
        if (forceTextBoxColor)
        {
            GlobalStatic.FrontEnd.TextBoxBackColor = bgColor;
            lastBgColorChange = WinmmTimer.TickCount;
        }
        verticalScrollBarUpdate();
		GlobalStatic.FrontEnd.Refresh();
	}

	public void OnPaint(Canvas graph)
	{
		if (Enabled)
		{
			lastUpdate = WinmmTimer.TickCount;
			bool isBackLog = GlobalStatic.FrontEnd.ScrollBar.IsBackLog;
			int topLineIndex = TopLineIndex;
			int bottomLineIndex = BottomLineIndex;
			int num = 0;
			if (Config.TextDrawingMode == TextDrawingMode.WINAPI)
			{
				throw new NotImplementedException();
			}
			for (int i = topLineIndex; i <= bottomLineIndex; i++)
			{
				displayLineList[i].DrawTo(graph, num, isBackLog, force: true, Config.TextDrawingMode);
				num += Config.LineHeight;
			}
			if (lastPointingString != pointingString)
			{
				lastPointingString = pointingString;
			}
			if (isBackLog)
			{
				lastDrawnLineNo = -1;
			}
			else
			{
				lastDrawnLineNo = lineNo;
			}
			lastSelectingButton = selectingButton;
			forceTextBoxColor = false;
		}
	}

	public void SetToolTipColor(System.Drawing.Color foreColor, System.Drawing.Color backColor)
	{
	}

	public void SetToolTipDelay(int delay)
	{
	}

	public string GetDebugTraceLog(bool force)
	{
		StringBuilder stringBuilder = new StringBuilder("");
		LogicalLine scaningLine = emuera.GetScaningLine();
		stringBuilder.AppendLine("*実行中の行");
		if (scaningLine == null || scaningLine.Position == null)
		{
			stringBuilder.AppendLine("ファイル名:なし");
			stringBuilder.AppendLine("行番号:なし 関数名:なし");
			stringBuilder.AppendLine("");
		}
		else
		{
			stringBuilder.AppendLine("ファイル名:" + scaningLine.Position.Filename);
			stringBuilder.AppendLine("行番号:" + scaningLine.Position.LineNo + " 関数名:" + scaningLine.ParentLabelLine.LabelName);
			stringBuilder.AppendLine("");
		}
		stringBuilder.AppendLine("*スタックトレース");
		for (int num = dTraceLogList.Count - 1; num >= 0; num--)
		{
			stringBuilder.AppendLine(dTraceLogList[num]);
		}
		return stringBuilder.ToString();
	}

	public void DebugPrint(string str)
	{
		if (Program.DebugMode)
		{
			dConsoleLog.Append(str);
		}
	}

	public void DebugClear()
	{
		dConsoleLog.Remove(0, dConsoleLog.Length);
	}

	public void DebugNewLine()
	{
		if (Program.DebugMode)
		{
			dConsoleLog.Append(Environment.NewLine);
		}
	}

	public void DebugAddTraceLog(string str)
	{
		if (Program.DebugMode && !runningERBfromMemory)
		{
			dTraceLogChanged = true;
			dTraceLogList.Add(str);
		}
	}

	public void DebugRemoveTraceLog()
	{
		if (Program.DebugMode && !runningERBfromMemory)
		{
			dTraceLogChanged = true;
			if (dTraceLogList.Count > 0)
			{
				dTraceLogList.RemoveAt(dTraceLogList.Count - 1);
			}
		}
	}

	public void DebugClearTraceLog()
	{
		if (Program.DebugMode && !runningERBfromMemory)
		{
			dTraceLogChanged = true;
			dTraceLogList.Clear();
		}
	}

	public void DebugCommand(string com, bool munchkin, bool outputDebugConsole)
	{
		ConsoleState consoleState = state;
		runningERBfromMemory = true;
		GlobalStatic.Process.saveCurrentState(single: false);
		try
		{
			LogicalLine logicalLine = null;
			if (!com.StartsWith("@") && !com.StartsWith("\"") && !com.StartsWith("\\"))
			{
				logicalLine = LogicalLineParser.ParseLine(com, null);
			}
			if (logicalLine == null || logicalLine is InvalidLine)
			{
				com = (((ExpressionParser.ReduceExpressionTerm(LexicalAnalyzer.Analyse(new StringStream(com), LexEndWith.EoL, LexAnalyzeFlag.None), TermEndWith.EoL) ?? throw new CodeEE("解釈不能なコードです")).GetOperandType() == typeof(long)) ? ((!outputDebugConsole) ? ("PRINTVL " + com) : ("DEBUGPRINTFORML {" + com + "}")) : ((!outputDebugConsole) ? ("PRINTFORMSL " + com) : ("DEBUGPRINTFORML %" + com + "%")));
				logicalLine = LogicalLineParser.ParseLine(com, null);
			}
			if (logicalLine == null)
			{
				throw new CodeEE("解釈不能なコードです");
			}
			if (logicalLine is InvalidLine)
			{
				throw new CodeEE(logicalLine.ErrMes);
			}
			if (!(logicalLine is InstructionLine))
			{
				throw new CodeEE("デバッグコマンドで使用できるのは代入文か命令文だけです");
			}
			InstructionLine instructionLine = (InstructionLine)logicalLine;
			if (instructionLine.Function.IsFlowContorol())
			{
				throw new CodeEE("フロー制御命令は使用できません");
			}
			if (instructionLine.Function.IsWaitInput())
			{
				throw new CodeEE(instructionLine.Function.Name + "命令は使用できません");
			}
			if (!instructionLine.Function.IsMethodSafe())
			{
				throw new CodeEE(instructionLine.Function.Name + "命令は使用できません");
			}
			if (instructionLine.Function.IsPartial())
			{
				throw new CodeEE(instructionLine.Function.Name + "命令は使用できません");
			}
			FunctionCode functionCode = instructionLine.FunctionCode;
			if ((uint)(functionCode - 43) <= 1u || functionCode == FunctionCode.PUTFORM || functionCode == FunctionCode.SAVEDATA)
			{
				throw new CodeEE(instructionLine.Function.Name + "命令は使用できません");
			}
			ArgumentParser.SetArgumentTo(instructionLine);
			if (instructionLine.IsError)
			{
				throw new CodeEE(instructionLine.ErrMes);
			}
			emuera.DoDebugNormalFunction(instructionLine, munchkin);
			if (instructionLine.FunctionCode == FunctionCode.SET && !outputDebugConsole)
			{
				PrintSingleLine(com);
			}
		}
		catch (Exception ex)
		{
			if (outputDebugConsole)
			{
				DebugPrint(ex.Message);
				DebugNewLine();
			}
			else
			{
				PrintError(ex.Message);
			}
			emuera.clearMethodStack();
		}
		finally
		{
			GlobalStatic.Process.loadPrevState();
			runningERBfromMemory = false;
			state = consoleState;
		}
	}

	private ConsoleDisplayLine GetLineFromPointY(int pointY)
	{
		int topLineIndex = TopLineIndex;
		int bottomLineIndex = BottomLineIndex;
		int num = GlobalStatic.FrontEnd.Height - (bottomLineIndex - topLineIndex + 1) * Config.LineHeight;
		if (pointY < num)
		{
			return null;
		}
		if (pointY == 0)
		{
			pointY = 1;
		}
		int index = bottomLineIndex - (GlobalStatic.FrontEnd.Height - pointY) / Config.LineHeight;
		return displayLineList[index];
	}

	public bool MoveMouse(System.Drawing.Point point)
	{
		ConsoleButtonString consoleButtonString = null;
		ConsoleButtonString consoleButtonString2 = null;
		bool flag = false;
		if (state == ConsoleState.Error)
		{
			flag = true;
		}
		else if (state == ConsoleState.WaitInput && inputReq.NeedValue)
		{
			flag = true;
		}
		if (!IsInProcess)
		{
			int x = point.X;
			int y = point.Y;
			Log.Debug("GetTouchEvent", $"Point:({x},{y})");
			if (y < 0)
			{
				return false;
			}
			if (y > GlobalStatic.FrontEnd.Height)
			{
				return false;
			}
			ConsoleDisplayLine lineFromPointY = GetLineFromPointY(y);
			if (lineFromPointY == null)
			{
				return false;
			}
			for (int i = 0; i < lineFromPointY.Buttons.Length; i++)
			{
				ConsoleButtonString consoleButtonString3 = lineFromPointY.Buttons[lineFromPointY.Buttons.Length - i - 1];
				if (consoleButtonString3 == null || consoleButtonString3.StrArray == null)
				{
					continue;
				}
				AConsoleDisplayPart[] strArray = consoleButtonString3.StrArray;
				foreach (AConsoleDisplayPart aConsoleDisplayPart in strArray)
				{
					if (aConsoleDisplayPart != null && aConsoleDisplayPart.PointX <= x && aConsoleDisplayPart.PointX + aConsoleDisplayPart.Width >= x)
					{
						consoleButtonString2 = consoleButtonString3;
						if (consoleButtonString2.IsButton)
						{
							goto end_IL_010f;
						}
					}
				}
				continue;
				end_IL_010f:
				break;
			}
			if (consoleButtonString2 == null || consoleButtonString2.Generation != lastButtonGeneration)
			{
				flag = false;
			}
			else if (!consoleButtonString2.IsButton)
			{
				flag = false;
			}
			else if (state == ConsoleState.WaitInput && inputReq.InputType == InputType.IntValue && !consoleButtonString2.IsInteger)
			{
				flag = false;
			}
		}
		if (flag)
		{
			consoleButtonString = consoleButtonString2;
		}
		bool result = consoleButtonString != selectingButton || consoleButtonString2 != pointingString;
		pointingString = consoleButtonString2;
		selectingButton = consoleButtonString;
		return result;
	}

	public void LeaveMouse()
	{
		bool num = selectingButton != null || pointingString != null;
		selectingButton = null;
		pointingString = null;
		if (num)
		{
			RefreshStrings(force_Paint: true);
		}
	}

	private void verticalScrollBarUpdate()
	{
	}

	public void GotoTitle()
	{
		forceStopTimer();
		ClearDisplay();
		redraw = ConsoleRedraw.Normal;
		UseUserStyle = false;
		userStyle = new StringStyle(Config.ForeColor, TypefaceStyle.Normal, null);
		emuera.BeginTitle();
		ReadAnyKey();
		callEmueraProgram("");
		RefreshStrings(force_Paint: true);
	}

	public void ReloadErb()
	{
		if (state == ConsoleState.Error)
		{
			MessageBox.Show("エラー発生時はこの機能は使えません");
			return;
		}
		if (state == ConsoleState.Initializing)
		{
			MessageBox.Show("初期化中はこの機能は使えません");
			return;
		}
		bool flag = false;
		if (redraw == ConsoleRedraw.None)
		{
			flag = true;
			redraw = ConsoleRedraw.Normal;
		}
		if (timer.Enabled)
		{
			timer.Enabled = false;
			timer_suspended = true;
		}
		prevState = state;
		prevReq = inputReq;
		state = ConsoleState.Initializing;
		PrintSingleLine("ERB再読み込み中……", temporary: true);
		force_temporary = true;
		emuera.ReloadErb();
		force_temporary = false;
		PrintSingleLine("再読み込み完了", temporary: true);
		RefreshStrings(force_Paint: true);
		updatedGeneration = true;
		if (flag)
		{
			redraw = ConsoleRedraw.None;
		}
	}

	public void ReloadErbFinished()
	{
		state = prevState;
		inputReq = prevReq;
		PrintSingleLine(" ");
		if (timer_suspended)
		{
			timer_suspended = false;
			timer.Enabled = true;
		}
	}

	public void ReloadPartialErb(List<string> path)
	{
		if (state == ConsoleState.Error)
		{
			MessageBox.Show("エラー発生時はこの機能は使えません");
			return;
		}
		if (state == ConsoleState.Initializing)
		{
			MessageBox.Show("初期化中はこの機能は使えません");
			return;
		}
		bool flag = false;
		if (redraw == ConsoleRedraw.None)
		{
			flag = true;
			redraw = ConsoleRedraw.Normal;
		}
		if (timer.Enabled)
		{
			timer.Enabled = false;
			timer_suspended = true;
		}
		prevState = state;
		prevReq = inputReq;
		state = ConsoleState.Initializing;
		PrintSingleLine("ERB再読み込み中……", temporary: true);
		force_temporary = true;
		emuera.ReloadPartialErb(path);
		force_temporary = false;
		PrintSingleLine("再読み込み完了", temporary: true);
		RefreshStrings(force_Paint: true);
		updatedGeneration = true;
		if (flag)
		{
			redraw = ConsoleRedraw.None;
		}
	}

	public void ReloadFolder(string erbPath)
	{
		if (state == ConsoleState.Error)
		{
			MessageBox.Show("エラー発生時はこの機能は使えません");
			return;
		}
		if (state == ConsoleState.Initializing)
		{
			MessageBox.Show("初期化中はこの機能は使えません");
			return;
		}
		if (timer.Enabled)
		{
			timer.Enabled = false;
			timer_suspended = true;
		}
		List<string> list = new List<string>();
		SearchOption searchOption = SearchOption.AllDirectories;
		if (!Config.SearchSubdirectory)
		{
			searchOption = SearchOption.TopDirectoryOnly;
		}
		string[] files = Directory.GetFiles(erbPath, "*.ERB", searchOption);
		for (int i = 0; i < files.Length; i++)
		{
			if (System.IO.Path.GetExtension(files[i]).ToUpper() == ".ERB")
			{
				list.Add(files[i]);
			}
		}
		bool flag = false;
		if (redraw == ConsoleRedraw.None)
		{
			flag = true;
			redraw = ConsoleRedraw.Normal;
		}
		prevState = state;
		prevReq = inputReq;
		state = ConsoleState.Initializing;
		PrintSingleLine("ERB再読み込み中……", temporary: true);
		force_temporary = true;
		emuera.ReloadPartialErb(list);
		force_temporary = false;
		PrintSingleLine("再読み込み完了", temporary: true);
		RefreshStrings(force_Paint: true);
		updatedGeneration = true;
		if (flag)
		{
			redraw = ConsoleRedraw.None;
		}
	}

	public void Dispose()
	{
		if (timer != null)
		{
			timer.Dispose();
		}
		timer = null;
	}

	public void ClearDisplay()
	{
		displayLineList.Clear();
		logicalLineCount = 0L;
		lineNo = 0;
		lastDrawnLineNo = -1;
		verticalScrollBarUpdate();
		GlobalStatic.FrontEnd.Refresh();
	}

	public void SetStringStyle(Android.Graphics.Color color)
	{
		userStyle.Color = color;
		userStyle.ColorChanged = color != Config.ForeColor;
	}

	public void SetFont(string fontname)
	{
		if (!string.IsNullOrEmpty(fontname))
		{
			userStyle.Fontname = fontname;
		}
		else
		{
			userStyle.Fontname = Config.FontName;
		}
	}

	public void ResetStyle()
	{
		userStyle = defaultStyle;
		alignment = DisplayLineAlignment.LEFT;
	}

	public void SetBgColor(Android.Graphics.Color color)
	{
		bgColor = color;
		forceTextBoxColor = true;
        if (redraw != ConsoleRedraw.None || !GlobalStatic.FrontEnd.ScrollBar.IsBackLog)
        {
            RefreshStrings(force_Paint: true);
            lastBgColorChange = WinmmTimer.TickCount;
        }
    }

	private void addRangeDisplayLine(ConsoleDisplayLine[] lineList)
	{
		for (int i = 0; i < lineList.Length; i++)
		{
			addDisplayLine(lineList[i], force_LEFT: false);
		}
	}

	private void addDisplayLine(ConsoleDisplayLine line, bool force_LEFT)
	{
		if (LastLineIsTemporary)
		{
			deleteLine(1);
		}
		AConsoleDisplayPart aConsoleDisplayPart = null;
		ConsoleButtonString[] buttons = line.Buttons;
		for (int i = 0; i < buttons.Length; i++)
		{
			AConsoleDisplayPart[] strArray = buttons[i].StrArray;
			foreach (AConsoleDisplayPart aConsoleDisplayPart2 in strArray)
			{
				if (aConsoleDisplayPart2.Error)
				{
					aConsoleDisplayPart = aConsoleDisplayPart2;
					break;
				}
			}
		}
		if (aConsoleDisplayPart != null)
		{
			Quit();
			return;
		}
		if (force_LEFT)
		{
			line.SetAlignment(DisplayLineAlignment.LEFT);
		}
		else
		{
			line.SetAlignment(alignment);
		}
		line.LineNo = lineNo;
		displayLineList.Add(line);
		lineNo++;
		if (line.IsLogicalLine)
		{
			logicalLineCount++;
		}
		if (lineNo == int.MaxValue)
		{
			lastDrawnLineNo = -1;
			lineNo = 0;
		}
		if (logicalLineCount == long.MaxValue)
		{
			logicalLineCount = 0L;
		}
		if (displayLineList.Count > Config.MaxLog)
		{
			displayLineList.RemoveAt(0);
		}
		GlobalStatic.FrontEnd.ScrollBar.MoveToEnd();
	}

	public void deleteLine(int argNum)
	{
		int num = 0;
		while (num < argNum && displayLineList.Count != 0)
		{
			ConsoleDisplayLine consoleDisplayLine = displayLineList[displayLineList.Count - 1];
			displayLineList.RemoveAt(displayLineList.Count - 1);
			lineNo--;
			if (consoleDisplayLine.IsLogicalLine)
			{
				num++;
				logicalLineCount--;
			}
		}
		if (lineNo < 0)
		{
			lineNo += int.MaxValue;
		}
		lastDrawnLineNo = -1;
	}

	public void PrintTemporaryLine(string str)
	{
		PrintSingleLine(str, temporary: true);
	}

	private void changeLastLine(string str)
	{
		deleteLine(1);
		PrintSingleLine(str, temporary: false);
	}

	internal void PrintWarning(string str, ScriptPosition position, int level)
	{
		if (level < Config.DisplayWarningLevel && !Program.AnalysisMode)
		{
			return;
		}
		bool flag = force_temporary;
		force_temporary = false;
		if (position != null)
		{
			if (position.LineNo >= 0)
			{
				PrintErrorButton($"警告Lv{level}:{position.Filename}:{position.LineNo}行目:{str}", position);
				if (position.RowLine != null)
				{
					PrintError(position.RowLine);
				}
			}
			else
			{
				PrintErrorButton($"警告Lv{level}:{position.Filename}:{str}", position);
			}
		}
		else
		{
			PrintError($"警告Lv{level}:{str}");
		}
		force_temporary = flag;
	}

	public void PrintSystemLine(string str)
	{
		PrintFlush(force: false);
		UseUserStyle = false;
		PrintSingleLine(str, temporary: false);
	}

	public void PrintError(string str)
	{
		if (!string.IsNullOrEmpty(str))
		{
			if (Program.DebugMode)
			{
				DebugPrint(str);
				DebugNewLine();
			}
			PrintFlush(force: false);
			UseUserStyle = false;
			ConsoleDisplayLine consoleDisplayLine = PrintPlainwithSingleLine(str);
			if (consoleDisplayLine != null)
			{
				addDisplayLine(consoleDisplayLine, force_LEFT: true);
				RefreshStrings(force_Paint: false);
			}
		}
	}

	internal void PrintErrorButton(string str, ScriptPosition pos)
	{
		if (!string.IsNullOrEmpty(str))
		{
			if (Program.DebugMode)
			{
				DebugPrint(str);
				DebugNewLine();
			}
			UseUserStyle = false;
			ConsoleDisplayLine consoleDisplayLine = printBuffer.AppendAndFlushErrButton(str, Style, "__openFileWithDebug__", pos, stringMeasure);
			if (consoleDisplayLine != null)
			{
				addDisplayLine(consoleDisplayLine, force_LEFT: true);
				RefreshStrings(force_Paint: false);
			}
		}
	}

	public void PrintSingleLine(string str)
	{
		PrintSingleLine(str, temporary: false);
	}

	public void PrintSingleLine(string str, bool temporary)
	{
		if (!string.IsNullOrEmpty(str))
		{
			PrintFlush(force: false);
			printBuffer.Append(str, Style);
			ConsoleDisplayLine consoleDisplayLine = BufferToSingleLine(force: true, temporary);
			if (consoleDisplayLine != null)
			{
				addDisplayLine(consoleDisplayLine, force_LEFT: false);
				RefreshStrings(force_Paint: false);
			}
		}
	}

	public void Print(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return;
		}
		if (str.Contains("\n"))
		{
			int num = str.IndexOf('\n');
			string str2 = str.Substring(0, num);
			printBuffer.Append(str2, Style);
			NewLine();
			if (num < str.Length - 1)
			{
				string str3 = str.Substring(num + 1);
				Print(str3);
			}
		}
		else
		{
			printBuffer.Append(str, Style);
		}
	}

	public void PrintImg(string str)
	{
		printBuffer.Append(new ConsoleImagePart(str, null, 0, 0, 0));
	}

	public void PrintShape(string type, int[] param)
	{
		ConsoleShapePart part = ConsoleShapePart.CreateShape(type, param, userStyle.Color, userStyle.ButtonColor, colorchanged: false);
		printBuffer.Append(part);
	}

	public void PrintHtml(string str)
	{
		if (!string.IsNullOrEmpty(str) && Enabled)
		{
			if (!printBuffer.IsEmpty)
			{
				ConsoleDisplayLine[] lineList = printBuffer.Flush(stringMeasure, force_temporary);
				addRangeDisplayLine(lineList);
			}
			addRangeDisplayLine(HtmlManager.Html2DisplayLine(str, stringMeasure, this));
			RefreshStrings(force_Paint: false);
		}
	}

	public void PrintC(string str, bool alignmentRight)
	{
		if (!string.IsNullOrEmpty(str))
		{
			printBuffer.Append(CreateTypeCString(str, alignmentRight), Style, force_button: true);
		}
	}

	private void calcPrintCWidth(StringMeasure stringMeasure)
	{
		string text = new string(' ', Config.PrintCLength);
		int fontSize = Config.FontSize;
		printCWidth = stringMeasure.GetDisplayLength(text, fontSize);
		text += " ";
		printCWidthL = stringMeasure.GetDisplayLength(text, fontSize);
		text += " ";
		printCWidthL2 = stringMeasure.GetDisplayLength(text, fontSize);
	}

	private string CreateTypeCString(string str, bool alignmentRight)
	{
		if (printCWidth == -1)
		{
			calcPrintCWidth(stringMeasure);
		}
		int num = 0;
		int num2 = 0;
		if (str != null)
		{
			num = Config.Encode.GetByteCount(str);
		}
		int printCLength = Config.PrintCLength;
		if (alignmentRight && num < printCLength)
		{
			str = new string(' ', printCLength - num) + str;
			num2 = stringMeasure.GetDisplayLength(str, Config.FontSize);
			while (num2 > printCWidth && str[0] == ' ')
			{
				str = str.Remove(0, 1);
				num2 = stringMeasure.GetDisplayLength(str, Config.FontSize);
			}
		}
		else if (!alignmentRight && num < printCLength + 1)
		{
			str += new string(' ', printCLength + 1 - num);
			num2 = stringMeasure.GetDisplayLength(str, Config.FontSize);
			while (num2 > printCWidthL && str[str.Length - 1] == ' ')
			{
				str = str.Remove(str.Length - 1, 1);
				num2 = stringMeasure.GetDisplayLength(str, Config.FontSize);
			}
		}
		return str;
	}

	internal void PrintButton(string str, string p)
	{
		if (!string.IsNullOrEmpty(str))
		{
			printBuffer.AppendButton(str, Style, p);
		}
	}

	internal void PrintButton(string str, long p)
	{
		if (!string.IsNullOrEmpty(str))
		{
			printBuffer.AppendButton(str, Style, p);
		}
	}

	internal void PrintButtonC(string str, string p, bool isRight)
	{
		if (!string.IsNullOrEmpty(str))
		{
			printBuffer.AppendButton(CreateTypeCString(str, isRight), Style, p);
		}
	}

	internal void PrintButtonC(string str, long p, bool isRight)
	{
		if (!string.IsNullOrEmpty(str))
		{
			printBuffer.AppendButton(CreateTypeCString(str, isRight), Style, p);
		}
	}

	internal void PrintPlain(string str)
	{
		if (!string.IsNullOrEmpty(str))
		{
			printBuffer.AppendPlainText(str, Style);
		}
	}

	public void NewLine()
	{
		PrintFlush(force: true);
		RefreshStrings(force_Paint: false);
	}

	internal ConsoleDisplayLine BufferToSingleLine(bool force, bool temporary)
	{
		if (!Enabled)
		{
			return null;
		}
		if (!force && printBuffer.IsEmpty)
		{
			return null;
		}
		if (force && printBuffer.IsEmpty)
		{
			printBuffer.Append(" ", Style);
		}
		return printBuffer.FlushSingleLine(stringMeasure, temporary | force_temporary);
	}

	internal ConsoleDisplayLine PrintPlainwithSingleLine(string str)
	{
		if (!Enabled)
		{
			return null;
		}
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}
		printBuffer.AppendPlainText(str, Style);
		return printBuffer.FlushSingleLine(stringMeasure, temporary: false);
	}

	public void PrintFlush(bool force)
	{
		if (Enabled && (force || !printBuffer.IsEmpty))
		{
			if (force && printBuffer.IsEmpty)
			{
				printBuffer.Append(" ", Style);
			}
			ConsoleDisplayLine[] lineList = printBuffer.Flush(stringMeasure, force_temporary);
			addRangeDisplayLine(lineList);
		}
	}

	public void PrintBar()
	{
		StringStyle stringStyle = userStyle;
		Print(stBar);
		userStyle = stringStyle;
	}

	public void printCustomBar(string barStr)
	{
		if (string.IsNullOrEmpty(barStr))
		{
			throw new CodeEE("空文字列によるDRAWLINEが行われました");
		}
		StringStyle stringStyle = userStyle;
		Print(getStBar(barStr));
		userStyle = stringStyle;
	}

	public string getDefStBar()
	{
		return stBar;
	}

	public string getStBar(string barStr)
	{
		int displayLength = stringMeasure.GetDisplayLength(barStr, Config.FontSize);
		int num = Config.DrawableWidth / displayLength;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < num; i++)
		{
			stringBuilder.Append(barStr);
		}
		return stringBuilder.ToString();
	}

	public void setStBar(string barStr)
	{
		stBar = getStBar(barStr);
	}

	private bool outputLog(string fullpath)
	{
		StreamWriter streamWriter = null;
		try
		{
			streamWriter = new StreamWriter(fullpath, append: false, Encoding.Unicode);
			foreach (ConsoleDisplayLine displayLine in displayLineList)
			{
				streamWriter.WriteLine(displayLine.ToString());
			}
		}
		catch (Exception)
		{
			MessageBox.Show("ログの出力に失敗しました", "ログ出力失敗");
			return false;
		}
		finally
		{
			streamWriter?.Close();
		}
		return true;
	}

	public bool OutputLog(string filename)
	{
		if (filename == null)
		{
			filename = Program.ExeDir + "emuera.log";
		}
		if (!filename.StartsWith(Program.ExeDir, StringComparison.CurrentCultureIgnoreCase))
		{
			return false;
		}
		if (outputLog(filename))
		{
			if (GlobalStatic.FrontEnd.Created)
			{
				PrintSystemLine("※※※ログファイルを" + filename + "に出力しました※※※");
				RefreshStrings(force_Paint: true);
			}
			return true;
		}
		return false;
	}

	public void GetDisplayStrings(StringBuilder builder)
	{
		if (displayLineList.Count != 0)
		{
			for (int i = 0; i < displayLineList.Count; i++)
			{
				builder.AppendLine(displayLineList[i].ToString());
			}
		}
	}

	internal ConsoleDisplayLine[] GetDisplayLines(long lineNo)
	{
		if (lineNo < 0 || lineNo > displayLineList.Count)
		{
			return null;
		}
		int num = 0;
		List<ConsoleDisplayLine> list = new List<ConsoleDisplayLine>();
		for (int num2 = displayLineList.Count - 1; num2 >= 0; num2--)
		{
			if (num == lineNo)
			{
				list.Insert(0, displayLineList[num2]);
			}
			if (displayLineList[num2].IsLogicalLine)
			{
				num++;
			}
			if (num > lineNo)
			{
				break;
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		ConsoleDisplayLine[] array = new ConsoleDisplayLine[list.Count];
		list.CopyTo(array);
		return array;
	}

	internal ConsoleDisplayLine[] PopDisplayingLines()
	{
		if (!Enabled)
		{
			return null;
		}
		if (printBuffer.IsEmpty)
		{
			return null;
		}
		return printBuffer.Flush(stringMeasure, force_temporary);
	}
}
