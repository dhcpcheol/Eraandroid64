using System.Collections.Generic;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal sealed class GotoLabelLine : LogicalLine, IEqualityComparer<GotoLabelLine>
{
	private readonly string labelname = "";

	public string LabelName => labelname;

	public GotoLabelLine(ScriptPosition thePosition, string labelname)
	{
		position = thePosition;
		this.labelname = labelname;
	}

	public bool Equals(GotoLabelLine x, GotoLabelLine y)
	{
		if (x == null || y == null)
		{
			return false;
		}
		if (x.ParentLabelLine == y.ParentLabelLine)
		{
			return x.labelname == y.labelname;
		}
		return false;
	}

	public int GetHashCode(GotoLabelLine obj)
	{
		return labelname.GetHashCode() ^ base.ParentLabelLine.GetHashCode();
	}
}
