using System.Collections.Generic;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.GameProc.Function;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal sealed class InstructionLine : LogicalLine
{
	private readonly FunctionIdentifier func;

	private StringStream argprimitive;

	private WordCollection assigndest;

	private long subData;

	private VariableTerm cnt;

	private long step;

	private LogicalLine jumpto;

	private LogicalLine jumptoendcatch;

	public List<InstructionLine> IfCaseList;

	public List<List<InstructionLine>> dataList;

	public List<InstructionLine> callList;

	public OperatorCode AssignOperator { get; private set; }

	public FunctionCode FunctionCode => func.Code;

	public FunctionIdentifier Function => func;

	public Argument Argument { get; set; }

	public long LoopEnd
	{
		get
		{
			return subData;
		}
		set
		{
			subData = value;
		}
	}

	public VariableTerm LoopCounter
	{
		get
		{
			return cnt;
		}
		set
		{
			cnt = value;
		}
	}

	public long LoopStep
	{
		get
		{
			return step;
		}
		set
		{
			step = value;
		}
	}

	public LogicalLine JumpTo
	{
		get
		{
			return jumpto;
		}
		set
		{
			jumpto = value;
		}
	}

	public LogicalLine JumpToEndCatch
	{
		get
		{
			return jumptoendcatch;
		}
		set
		{
			jumptoendcatch = value;
		}
	}

	public InstructionLine(ScriptPosition thePosition, FunctionIdentifier theFunc, StringStream theArgPrimitive)
	{
		position = thePosition;
		func = theFunc;
		argprimitive = theArgPrimitive;
	}

	public InstructionLine(ScriptPosition thePosition, FunctionIdentifier functionIdentifier, OperatorCode assignOP, WordCollection dest, StringStream theArgPrimitive)
	{
		position = thePosition;
		func = functionIdentifier;
		AssignOperator = assignOP;
		assigndest = dest;
		argprimitive = theArgPrimitive;
	}

	public StringStream PopArgumentPrimitive()
	{
		StringStream result = argprimitive;
		argprimitive = null;
		return result;
	}

	public WordCollection PopAssignmentDestStr()
	{
		WordCollection result = assigndest;
		assigndest = null;
		return result;
	}
}
