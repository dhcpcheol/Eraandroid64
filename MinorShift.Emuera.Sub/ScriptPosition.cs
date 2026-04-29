using System;
using System.Collections.Generic;

namespace MinorShift.Emuera.Sub;

internal sealed class ScriptPosition : IEquatable<ScriptPosition>, IEqualityComparer<ScriptPosition>
{
	public readonly int LineNo;

	public readonly string RowLine;

	public readonly string Filename;

	public ScriptPosition(string srcLine)
	{
		LineNo = -1;
		RowLine = srcLine;
		Filename = "";
	}

	public ScriptPosition(string srcFile, int srcLineNo, string srcLine)
	{
		LineNo = srcLineNo;
		RowLine = srcLine;
		if (srcFile == null)
		{
			Filename = "";
		}
		else
		{
			Filename = srcFile;
		}
	}

	public override string ToString()
	{
		if (LineNo == -1)
		{
			return base.ToString();
		}
		return Filename + ":" + LineNo;
	}

	public bool Equals(ScriptPosition x, ScriptPosition y)
	{
		if (x == null || y == null)
		{
			return false;
		}
		if (x.Filename == y.Filename)
		{
			return x.LineNo == y.LineNo;
		}
		return false;
	}

	public int GetHashCode(ScriptPosition obj)
	{
		return Filename.GetHashCode() ^ LineNo.GetHashCode();
	}

	public bool Equals(ScriptPosition other)
	{
		return Equals(this, other);
	}
}
