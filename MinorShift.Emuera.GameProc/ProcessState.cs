using System.Collections.Generic;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.GameView;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal sealed class ProcessState
{
	private readonly EmueraConsole console;

	private readonly List<CalledFunction> functionList = new List<CalledFunction>();

	private LogicalLine currentLine;

	public int lineCount;

	public int currentMin;

	private SystemStateCode sysStateCode;

	private BeginType begintype;

	public bool calledWhenNormal = true;

	public SingleTerm MethodReturnValue;

	private bool isClone;

	public bool ScriptEnd => functionList.Count == currentMin;

	public int functionCount => functionList.Count;

	public bool isBegun
	{
		get
		{
			if (begintype == BeginType.NULL)
			{
				return false;
			}
			return true;
		}
	}

	public LogicalLine CurrentLine
	{
		get
		{
			return currentLine;
		}
		set
		{
			currentLine = value;
		}
	}

	public LogicalLine ErrorLine => currentLine;

	public CalledFunction CurrentCalled => functionList[functionList.Count - 1];

	public SystemStateCode SystemState
	{
		get
		{
			return sysStateCode;
		}
		set
		{
			sysStateCode = value;
		}
	}

	public LogicalLine GetCurrentReturnAddress
	{
		get
		{
			if (functionList.Count == currentMin)
			{
				return null;
			}
			return functionList[functionList.Count - 1].ReturnAddress;
		}
	}

	public string Scope
	{
		get
		{
			if (functionList.Count == 0)
			{
				return null;
			}
			return functionList[functionList.Count - 1].FunctionName;
		}
	}

	public bool IsFunctionMethod => functionList[currentMin].TopLabel.IsMethod;

	public bool IsClone
	{
		get
		{
			return isClone;
		}
		set
		{
			isClone = value;
		}
	}

	public ProcessState(EmueraConsole console)
	{
		if (Program.DebugMode)
		{
			this.console = console;
		}
	}

	public void ShiftNextLine()
	{
		currentLine = currentLine.NextLine;
		lineCount++;
	}

	public void JumpTo(LogicalLine line)
	{
		currentLine = line;
		lineCount++;
	}

	public void SetBegin(string keyword)
	{
		switch (keyword)
		{
		case "SHOP":
			SetBegin(BeginType.SHOP);
			break;
		case "TRAIN":
			SetBegin(BeginType.TRAIN);
			break;
		case "AFTERTRAIN":
			SetBegin(BeginType.AFTERTRAIN);
			break;
		case "ABLUP":
			SetBegin(BeginType.ABLUP);
			break;
		case "TURNEND":
			SetBegin(BeginType.TURNEND);
			break;
		case "FIRST":
			SetBegin(BeginType.FIRST);
			break;
		case "TITLE":
			SetBegin(BeginType.TITLE);
			break;
		default:
			throw new CodeEE("BEGINのキーワード\"" + keyword + "\"は未定義です");
		}
	}

	public void SetBegin(BeginType type)
	{
		string text = "";
		switch (type)
		{
		case BeginType.SHOP:
		case BeginType.TRAIN:
		case BeginType.AFTERTRAIN:
		case BeginType.ABLUP:
		case BeginType.TURNEND:
		case BeginType.FIRST:
			if ((sysStateCode & SystemStateCode.__CAN_BEGIN__) != SystemStateCode.__CAN_BEGIN__)
			{
				text = "BEGIN";
				string functionName = functionList[0].FunctionName;
				throw new CodeEE("@" + functionName + "中で" + text + "命令を実行することはできません");
			}
			break;
		}
		begintype = type;
	}

	public void SaveLoadData(bool saveData)
	{
		if (saveData)
		{
			sysStateCode = SystemStateCode.SaveGame_Begin;
		}
		else
		{
			sysStateCode = SystemStateCode.LoadGame_Begin;
		}
	}

	public void ClearFunctionList()
	{
		if (Program.DebugMode && !isClone && GlobalStatic.Process.MethodStack() == 0)
		{
			console.DebugClearTraceLog();
		}
		foreach (CalledFunction function in functionList)
		{
			if (function.CurrentLabel.hasPrivDynamicVar)
			{
				function.CurrentLabel.Out();
			}
		}
		functionList.Clear();
		begintype = BeginType.NULL;
	}

	public void Begin()
	{
		if (sysStateCode == SystemStateCode.Shop_CallEventShop)
		{
			return;
		}
		switch (begintype)
		{
		case BeginType.SHOP:
			if (sysStateCode == SystemStateCode.Normal)
			{
				calledWhenNormal = true;
			}
			else
			{
				calledWhenNormal = false;
			}
			sysStateCode = SystemStateCode.Shop_Begin;
			break;
		case BeginType.TRAIN:
			sysStateCode = SystemStateCode.Train_Begin;
			break;
		case BeginType.AFTERTRAIN:
			sysStateCode = SystemStateCode.AfterTrain_Begin;
			break;
		case BeginType.ABLUP:
			sysStateCode = SystemStateCode.Ablup_Begin;
			break;
		case BeginType.TURNEND:
			sysStateCode = SystemStateCode.Turnend_Begin;
			break;
		case BeginType.FIRST:
			sysStateCode = SystemStateCode.First_Begin;
			break;
		case BeginType.TITLE:
			sysStateCode = SystemStateCode.Title_Begin;
			break;
		}
		if (Program.DebugMode)
		{
			console.DebugClearTraceLog();
			console.DebugAddTraceLog("BEGIN:" + begintype);
		}
		foreach (CalledFunction function in functionList)
		{
			if (function.CurrentLabel.hasPrivDynamicVar)
			{
				function.CurrentLabel.Out();
			}
		}
		functionList.Clear();
		begintype = BeginType.NULL;
	}

	public void Begin(BeginType type)
	{
		begintype = type;
		sysStateCode = SystemStateCode.Title_Begin;
		Begin();
	}

	public LogicalLine GetReturnAddressSequensial(int curerntDepth)
	{
		if (functionList.Count == currentMin)
		{
			return null;
		}
		return functionList[functionList.Count - curerntDepth - 1].ReturnAddress;
	}

	public void Return(long ret)
	{
		if (IsFunctionMethod)
		{
			ReturnF(null);
			return;
		}
		CalledFunction calledFunction = functionList[functionList.Count - 1];
		if (calledFunction.IsJump)
		{
			if (calledFunction.TopLabel.hasPrivDynamicVar)
			{
				calledFunction.TopLabel.Out();
			}
			functionList.Remove(calledFunction);
			if (Program.DebugMode)
			{
				console.DebugRemoveTraceLog();
			}
			Return(ret);
			return;
		}
		if (!calledFunction.IsEvent)
		{
			if (calledFunction.TopLabel.hasPrivDynamicVar)
			{
				calledFunction.TopLabel.Out();
			}
			currentLine = null;
		}
		else
		{
			if (calledFunction.CurrentLabel.hasPrivDynamicVar)
			{
				calledFunction.CurrentLabel.Out();
			}
			if (calledFunction.IsOnly)
			{
				calledFunction.FinishEvent();
			}
			else if (calledFunction.HasSingleFlag && ret == 1)
			{
				calledFunction.ShiftNextGroup();
			}
			else
			{
				calledFunction.ShiftNext();
			}
			currentLine = calledFunction.CurrentLabel;
			if (calledFunction.CurrentLabel != null)
			{
				lineCount++;
				if (calledFunction.CurrentLabel.hasPrivDynamicVar)
				{
					calledFunction.CurrentLabel.In();
				}
			}
		}
		if (Program.DebugMode)
		{
			console.DebugRemoveTraceLog();
		}
		if (currentLine == null)
		{
			currentLine = calledFunction.ReturnAddress;
			functionList.RemoveAt(functionList.Count - 1);
			if (currentLine == null)
			{
				if (begintype != BeginType.NULL)
				{
					Begin();
				}
			}
			else
			{
				lineCount++;
			}
			return;
		}
		if (Program.DebugMode)
		{
			FunctionLabelLine currentLabel = calledFunction.CurrentLabel;
			console.DebugAddTraceLog("CALL :@" + currentLabel.LabelName + ":" + currentLabel.Position.ToString() + "行目");
		}
		lineCount++;
	}

	public void IntoFunction(CalledFunction call, UserDefinedFunctionArgument srcArgs, ExpressionMediator exm)
	{
		if (call.IsEvent)
		{
			foreach (CalledFunction function in functionList)
			{
				if (function.IsEvent)
				{
					throw new CodeEE("EVENT関数の解決前にCALLEVENT命令が行われました");
				}
			}
		}
		if (Program.DebugMode)
		{
			FunctionLabelLine currentLabel = call.CurrentLabel;
			if (call.IsJump)
			{
				console.DebugAddTraceLog("JUMP :@" + currentLabel.LabelName + ":" + currentLabel.Position.ToString() + "行目");
			}
			else
			{
				console.DebugAddTraceLog("CALL :@" + currentLabel.LabelName + ":" + currentLabel.Position.ToString() + "行目");
			}
		}
		if (srcArgs != null)
		{
			srcArgs.SetTransporter(exm);
			if (call.TopLabel.hasPrivDynamicVar)
			{
				call.TopLabel.In();
			}
			for (int i = 0; i < call.TopLabel.Arg.Length; i++)
			{
				if (srcArgs.Arguments[i] != null)
				{
					if (call.TopLabel.Arg[i].Identifier.IsReference)
					{
						((ReferenceToken)call.TopLabel.Arg[i].Identifier).SetRef(srcArgs.TransporterRef[i]);
					}
					else if (srcArgs.Arguments[i].GetOperandType() == typeof(long))
					{
						call.TopLabel.Arg[i].SetValue(srcArgs.TransporterInt[i], exm);
					}
					else
					{
						call.TopLabel.Arg[i].SetValue(srcArgs.TransporterStr[i], exm);
					}
				}
			}
		}
		else if (call.TopLabel.hasPrivDynamicVar)
		{
			call.TopLabel.In();
		}
		functionList.Add(call);
		currentLine = call.CurrentLabel;
		lineCount++;
	}

	public void ReturnF(SingleTerm ret)
	{
		if (Program.DebugMode)
		{
			console.DebugRemoveTraceLog();
		}
		currentLine = functionList[functionList.Count - 1].ReturnAddress;
		functionList.RemoveAt(functionList.Count - 1);
		MethodReturnValue = ret;
	}

	public ProcessState Clone()
	{
		return new ProcessState(console)
		{
			isClone = true,
			currentLine = currentLine,
			sysStateCode = sysStateCode,
			begintype = begintype
		};
	}
}
