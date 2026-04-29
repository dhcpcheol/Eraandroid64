using System.Collections.Generic;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Function;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal sealed class CalledFunction
{
	private static FunctionMethod tostrMethod;

	private List<FunctionLabelLine>[] eventLabelList;

	private int counter = -1;

	private int group;

	private LogicalLine returnAddress;

	public readonly string FunctionName = "";

	public FunctionLabelLine CurrentLabel { get; private set; }

	public FunctionLabelLine TopLabel { get; private set; }

	public bool IsJump { get; set; }

	public bool Finished { get; private set; }

	public LogicalLine ReturnAddress => returnAddress;

	public bool IsEvent { get; private set; }

	public bool HasSingleFlag
	{
		get
		{
			if (CurrentLabel == null)
			{
				return false;
			}
			return CurrentLabel.IsSingle;
		}
	}

	public bool IsOnly => CurrentLabel.IsOnly;

	private CalledFunction(string label)
	{
		FunctionName = label;
	}

	public static CalledFunction CallEventFunction(Process parent, string label, LogicalLine retAddress)
	{
		CalledFunction calledFunction = new CalledFunction(label);
		new List<FunctionLabelLine>();
		calledFunction.Finished = false;
		calledFunction.eventLabelList = parent.LabelDictionary.GetEventLabels(label);
		if (calledFunction.eventLabelList == null)
		{
			FunctionLabelLine nonEventLabel = parent.LabelDictionary.GetNonEventLabel(label);
			if (parent.LabelDictionary.GetNonEventLabel(label) != null)
			{
				throw new CodeEE("イベント関数でない関数@" + label + "(" + nonEventLabel.Position.Filename + ":" + nonEventLabel.Position.LineNo + "行目)に対しEVENT呼び出しが行われました");
			}
			return null;
		}
		calledFunction.counter = -1;
		calledFunction.group = 0;
		calledFunction.ShiftNext();
		calledFunction.TopLabel = calledFunction.CurrentLabel;
		calledFunction.returnAddress = retAddress;
		calledFunction.IsEvent = true;
		return calledFunction;
	}

	public static CalledFunction CallFunction(Process parent, string label, LogicalLine retAddress)
	{
		CalledFunction calledFunction = new CalledFunction(label);
		calledFunction.Finished = false;
		FunctionLabelLine nonEventLabel = parent.LabelDictionary.GetNonEventLabel(label);
		if (nonEventLabel == null)
		{
			if (parent.LabelDictionary.GetEventLabels(label) != null)
			{
				throw new CodeEE("イベント関数@" + label + "に対し通常のCALLが行われました(このエラーは互換性オプション「" + Config.GetConfigName(ConfigCode.CompatiCallEvent) + "」により無視できます)");
			}
			return null;
		}
		if (nonEventLabel.IsMethod)
		{
			throw new CodeEE("#FUCNTION(S)が定義された関数@" + nonEventLabel.LabelName + "(" + nonEventLabel.Position.Filename + ":" + nonEventLabel.Position.LineNo + "行目)に対し通常のCALLが行われました");
		}
		calledFunction.TopLabel = nonEventLabel;
		calledFunction.CurrentLabel = nonEventLabel;
		calledFunction.returnAddress = retAddress;
		calledFunction.IsEvent = false;
		return calledFunction;
	}

	public static CalledFunction CreateCalledFunctionMethod(FunctionLabelLine labelline, string label)
	{
		return new CalledFunction(label)
		{
			TopLabel = labelline,
			CurrentLabel = labelline,
			returnAddress = null,
			IsEvent = false
		};
	}

	public UserDefinedFunctionArgument ConvertArg(IOperandTerm[] srcArgs, out string errMes)
	{
		errMes = null;
		if (TopLabel.IsError)
		{
			errMes = TopLabel.ErrMes;
			return null;
		}
		FunctionLabelLine topLabel = TopLabel;
		IOperandTerm[] array = new IOperandTerm[topLabel.Arg.Length];
		if (array.Length < srcArgs.Length)
		{
			errMes = "引数の数が関数\"@" + topLabel.LabelName + "\"に設定された数を超えています";
			return null;
		}
		IOperandTerm operandTerm = null;
		VariableTerm variableTerm = null;
		for (int i = 0; i < topLabel.Arg.Length; i++)
		{
			operandTerm = ((i < srcArgs.Length) ? srcArgs[i] : null);
			variableTerm = topLabel.Arg[i];
			_ = variableTerm.IsString;
			if (variableTerm.Identifier.IsReference)
			{
				if (operandTerm == null)
				{
					errMes = "\"@" + topLabel.LabelName + "\"の" + (i + 1) + "番目の引数は参照渡しのため省略できません";
					return null;
				}
				if (!(operandTerm is VariableTerm variableTerm2) || variableTerm2.Identifier.Dimension == 0)
				{
					errMes = "\"@" + topLabel.LabelName + "\"の" + (i + 1) + "番目の引数は参照渡しのための配列変数でなければなりません";
					return null;
				}
				if (!((ReferenceToken)variableTerm.Identifier).MatchType(variableTerm2.Identifier, allowChara: false, out errMes))
				{
					errMes = "\"@" + topLabel.LabelName + "\"の" + (i + 1) + "番目の引数:" + errMes;
					return null;
				}
			}
			else if (operandTerm == null)
			{
				operandTerm = topLabel.Def[i];
				if (operandTerm == null && !Config.CompatiFuncArgOptional)
				{
					errMes = "\"@" + topLabel.LabelName + "\"の" + (i + 1) + "番目の引数は省略できません(この警告は互換性オプション「" + Config.GetConfigName(ConfigCode.CompatiFuncArgOptional) + "」により無視できます)";
					return null;
				}
			}
			else if (operandTerm.GetOperandType() != variableTerm.GetOperandType())
			{
				if (operandTerm.GetOperandType() == typeof(string))
				{
					errMes = "\"@" + topLabel.LabelName + "\"の" + (i + 1) + "番目の引数を文字列型から整数型に変換できません";
					return null;
				}
				if (!Config.CompatiFuncArgAutoConvert)
				{
					errMes = "\"@" + topLabel.LabelName + "\"の" + (i + 1) + "番目の引数を整数型から文字列型に変換できません(この警告は互換性オプション「" + Config.GetConfigName(ConfigCode.CompatiFuncArgAutoConvert) + "」により無視できます)";
					return null;
				}
				if (tostrMethod == null)
				{
					tostrMethod = FunctionMethodCreator.GetMethodList()["TOSTR"];
				}
				operandTerm = new FunctionMethodTerm(tostrMethod, new IOperandTerm[1] { operandTerm });
			}
			array[i] = operandTerm;
		}
		return new UserDefinedFunctionArgument(array, topLabel.Arg);
	}

	public LogicalLine CallLabel(Process parent, string label)
	{
		return parent.LabelDictionary.GetLabelDollar(label, CurrentLabel);
	}

	public void updateRetAddress(LogicalLine line)
	{
		returnAddress = line;
	}

	public CalledFunction Clone()
	{
		return new CalledFunction(FunctionName)
		{
			eventLabelList = eventLabelList,
			CurrentLabel = CurrentLabel,
			TopLabel = TopLabel,
			group = group,
			IsEvent = IsEvent,
			counter = counter,
			returnAddress = returnAddress
		};
	}

	public void ShiftNext()
	{
		do
		{
			counter++;
			if (eventLabelList[group].Count > counter)
			{
				CurrentLabel = eventLabelList[group][counter];
				return;
			}
			group++;
			counter = -1;
		}
		while (group < 4);
		CurrentLabel = null;
	}

	public void ShiftNextGroup()
	{
		counter = -1;
		group++;
		if (group >= 4)
		{
			CurrentLabel = null;
		}
		else
		{
			ShiftNext();
		}
	}

	public void FinishEvent()
	{
		group = 4;
		counter = -1;
		CurrentLabel = null;
	}
}
