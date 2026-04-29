using System;

namespace MinorShift.Emuera.Sub;

[Serializable]
internal sealed class ExeEE : EmueraException
{
	public ExeEE(string errormes)
		: base(errormes)
	{
	}

	public ExeEE(string errormes, ScriptPosition position)
		: base(errormes, position)
	{
	}
}
