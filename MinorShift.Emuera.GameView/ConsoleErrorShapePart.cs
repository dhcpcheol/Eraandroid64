using System;
using Android.Graphics;

namespace MinorShift.Emuera.GameView;

internal sealed class ConsoleErrorShapePart : ConsoleShapePart
{
	public ConsoleErrorShapePart(string errMes)
	{
		base.Str = errMes;
		base.AltText = errMes;
	}

	public override void DrawTo(Canvas graph, int pointY, bool isSelecting, bool isBackLog, TextDrawingMode mode)
	{
		graph.DrawString(base.Str, base.PointX, pointY, Config.FontName);
	}

	public override void GDIDrawTo(int pointY, bool isSelecting, bool isBackLog)
	{
		throw new NotImplementedException();
	}

	public override void SetWidth(StringMeasure sm, float subPixel)
	{
		if (base.Error)
		{
			base.Width = 0;
			return;
		}
		base.Width = sm.GetDisplayLength(base.Str, Config.FontSize);
		base.XsubPixel = subPixel;
	}
}
