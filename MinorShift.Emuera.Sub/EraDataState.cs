using System.Reflection;

namespace MinorShift.Emuera.Sub;

[Obfuscation(Exclude = false)]
internal enum EraDataState
{
	OK,
	FILENOTFOUND,
	GAME_ERROR,
	VIRSION_ERROR,
	ETC_ERROR
}
