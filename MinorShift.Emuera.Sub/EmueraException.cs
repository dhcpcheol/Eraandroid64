using System;

namespace MinorShift.Emuera.Sub;

[Serializable]
internal abstract class EmueraException : ApplicationException
{
	public ScriptPosition Position;

	protected EmueraException(string errormes, ScriptPosition position)
		: base(errormes)
	{
		Position = position;
	}

	protected EmueraException(string errormes)
		: base(errormes)
	{
		Position = null;
	}
}
