using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal sealed class InvalidLabelLine : FunctionLabelLine
{
	public override bool IsError => true;

	public InvalidLabelLine(ScriptPosition thePosition, string labelname, string err)
	{
		position = thePosition;
		base.LabelName = labelname;
		errMes = err;
		base.IsSingle = false;
		base.Index = -1;
		base.Depth = -1;
		base.IsMethod = false;
		base.MethodType = typeof(void);
	}
}
