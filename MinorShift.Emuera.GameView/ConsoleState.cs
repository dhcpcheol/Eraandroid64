using System.Reflection;

namespace MinorShift.Emuera.GameView;

[Obfuscation(Exclude = false)]
internal enum ConsoleState
{
	Initializing = 0,
	Quit = 5,
	Error = 6,
	Running = 7,
	WaitInput = 20
}
