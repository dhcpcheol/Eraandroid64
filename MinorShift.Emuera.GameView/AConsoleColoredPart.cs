using Android.Graphics;

namespace MinorShift.Emuera.GameView;

internal abstract class AConsoleColoredPart : AConsoleDisplayPart
{
	protected bool colorChanged;

	protected Color Color { get; set; }

	protected Color ButtonColor { get; set; }
}
