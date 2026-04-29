using System.Drawing;
using Android.Graphics;

namespace MinorShift.Emuera.GameView;

internal sealed class ConsoleSpacePart : ConsoleShapePart
{
	public ConsoleSpacePart(RectangleF theRect)
	{
		base.Str = "";
		base.WidthF = theRect.Width;
	}

	public override void DrawTo(Canvas graph, int pointY, bool isSelecting, bool isBackLog, TextDrawingMode mode)
	{
	}

	public override void GDIDrawTo(int pointY, bool isSelecting, bool isBackLog)
	{
	}

	public override void SetWidth(StringMeasure sm, float subPixel)
	{
		float num = subPixel + base.WidthF;
		base.Width = (int)num;
		base.XsubPixel = num - (float)base.Width;
	}
}
