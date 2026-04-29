using System.Reflection;

namespace MinorShift.Emuera;

[Obfuscation(Exclude = true)]
internal enum TextDrawingMode
{
	GRAPHICS,
	TEXTRENDERER,
	WINAPI
}
