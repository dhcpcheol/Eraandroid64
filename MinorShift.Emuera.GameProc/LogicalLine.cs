using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal abstract class LogicalLine
{
	protected ScriptPosition position;

	private LogicalLine nextLine;

	protected bool isError;

	protected string errMes = "";

	public ScriptPosition Position => position;

	public FunctionLabelLine ParentLabelLine { get; set; }

	public LogicalLine NextLine
	{
		get
		{
			return nextLine;
		}
		set
		{
			nextLine = value;
		}
	}

	public virtual string ErrMes
	{
		get
		{
			return errMes;
		}
		set
		{
			errMes = value;
		}
	}

	public virtual bool IsError
	{
		get
		{
			return isError;
		}
		set
		{
			isError = value;
		}
	}

	public override string ToString()
	{
		if (position == null)
		{
			return base.ToString();
		}
		return $"{position.Filename}:{position.LineNo}:{position.RowLine}";
	}
}
