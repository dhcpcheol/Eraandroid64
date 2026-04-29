using System.Reflection;

namespace MinorShift.Emuera.GameView;

[Obfuscation(Exclude = false)]
internal enum DisplayLineLastState
{
	None,
	Normal,
	Selected,
	BackLog
}
