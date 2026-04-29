using System.Reflection;

namespace MinorShift.Emuera.GameProc;

[Obfuscation(Exclude = false)]
internal enum BeginType
{
	NULL = 0,
	SHOP = 2,
	TRAIN = 3,
	AFTERTRAIN = 4,
	ABLUP = 5,
	TURNEND = 6,
	FIRST = 7,
	TITLE = 8
}
