using Android.Graphics;

namespace MinorShift.Emuera.GameView;

internal abstract class AConsoleDisplayPart
{
	public bool Error { get; protected set; }

	public string Str { get; protected set; }

	public string AltText { get; protected set; }

	public int PointX { get; set; }

	public float XsubPixel { get; set; }

	public float WidthF { get; set; }

	public int Width { get; set; }

	public virtual int Top => 0;

	public virtual int Bottom => Config.LineHeight;

	public abstract bool CanDivide { get; }

	public abstract void DrawTo(Canvas graph, int pointY, bool isSelecting, bool isBackLog, TextDrawingMode mode);

	public abstract void GDIDrawTo(int pointY, bool isSelecting, bool isBackLog);

	public abstract void SetWidth(StringMeasure sm, float subPixel);

	public override string ToString()
	{
		if (Str == null)
		{
			return "";
		}
		return Str;
	}
}
