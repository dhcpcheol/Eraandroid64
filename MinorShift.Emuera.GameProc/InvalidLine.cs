using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal sealed class InvalidLine : LogicalLine
{
	public override bool IsError => true;

	public InvalidLine(ScriptPosition thePosition, string err)
	{
		position = thePosition;
		errMes = err;
	}
}
