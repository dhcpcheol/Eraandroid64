using System.Reflection;

namespace MinorShift.Emuera;

[Obfuscation(Exclude = true)]
internal enum DisplayWarningFlag
{
	IGNORE,
	LATER,
	ONCE,
	DISPLAY
}
