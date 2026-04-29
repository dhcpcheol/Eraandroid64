using System;

namespace MinorShift.Emuera.Sub;

[Serializable]
internal class CodeEE : EmueraException
{
	public CodeEE(string errormes, ScriptPosition position)
		: base(errormes, position)
	{
	}

	public CodeEE(string errormes)
		: base(errormes)
	{
	}
}
