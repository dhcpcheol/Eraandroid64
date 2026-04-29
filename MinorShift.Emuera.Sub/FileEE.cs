using System;

namespace MinorShift.Emuera.Sub;

[Serializable]
internal sealed class FileEE : EmueraException
{
	public FileEE(string errormes)
		: base(errormes)
	{
	}
}
