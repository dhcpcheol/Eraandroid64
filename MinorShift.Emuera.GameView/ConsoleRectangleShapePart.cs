using System;
using System.Drawing;
using Android.Graphics;

namespace MinorShift.Emuera.GameView;

internal sealed class ConsoleRectangleShapePart : ConsoleShapePart
{
	private readonly int top;

	private readonly int bottom;

	private readonly RectangleF originalRectF;

	private bool visible;

	private Rectangle rect;

	public override int Top => top;

	public override int Bottom => bottom;

	public ConsoleRectangleShapePart(RectangleF theRect)
	{
		base.Str = "";
		originalRectF = theRect;
		base.WidthF = theRect.X + theRect.Width;
		rect.Y = (int)theRect.Y;
		rect.Height = (int)theRect.Height;
		if (rect.Height == 0 && theRect.Height >= 0.001f)
		{
			rect.Height = 1;
		}
		top = Math.Min(0, rect.Y);
		bottom = Math.Max(Config.FontSize, rect.Y + rect.Height);
	}

	public override void DrawTo(Canvas graph, int pointY, bool isSelecting, bool isBackLog, TextDrawingMode mode)
	{
		if (visible)
		{
			Rectangle rectangle = rect;
			rectangle.X += base.PointX;
			rectangle.Y += pointY;
			Android.Graphics.Color color = (isSelecting ? base.ButtonColor : base.Color);
			graph.FillRect(rectangle.ToRect(), new Paint
			{
				Color = color
			});
		}
	}

	public override void GDIDrawTo(int pointY, bool isSelecting, bool isBackLog)
	{
		throw new NotImplementedException();
	}

	public override void SetWidth(StringMeasure sm, float subPixel)
	{
		float num = subPixel + base.WidthF;
		base.Width = (int)num;
		base.XsubPixel = num - (float)base.Width;
		rect.X = (int)(subPixel + originalRectF.X);
		rect.Width = base.Width - rect.X;
		rect.X += Config.DrawingParam_ShapePositionShift;
		visible = rect.X >= 0 && rect.Width > 0;
	}
}
