using System;
using Android.Graphics;

namespace MinorShift.Emuera.GameView;

internal sealed class ConsoleStyledString : AConsoleColoredPart
{
	public StringStyle StringStyle { get; private set; }

	public override bool CanDivide => true;

	private ConsoleStyledString()
	{
	}

	public ConsoleStyledString(string str, StringStyle style)
	{
		base.Str = str;
		StringStyle = style;
		base.Color = style.Color;
		base.ButtonColor = style.ButtonColor;
		colorChanged = style.ColorChanged;
		if (!colorChanged && base.Color != Config.ForeColor)
		{
			colorChanged = true;
		}
		base.PointX = -1;
		base.Width = -1;
	}

	public ConsoleStyledString DivideAt(int index, StringMeasure sm)
	{
		ConsoleStyledString consoleStyledString = DivideAt(index);
		if (consoleStyledString == null)
		{
			return null;
		}
		SetWidth(sm, base.XsubPixel);
		consoleStyledString.SetWidth(sm, base.XsubPixel);
		return consoleStyledString;
	}

	public ConsoleStyledString DivideAt(int index)
	{
		if (index <= 0 || index > base.Str.Length || base.Error)
		{
			return null;
		}
		string str = base.Str.Substring(index, base.Str.Length - index);
		base.Str = base.Str.Substring(0, index);
		return new ConsoleStyledString
		{
			Str = str,
			Color = base.Color,
			ButtonColor = base.ButtonColor,
			colorChanged = colorChanged,
			StringStyle = StringStyle,
			XsubPixel = base.XsubPixel
		};
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

	public override void DrawTo(Canvas graph, int pointY, bool isSelecting, bool isBackLog, TextDrawingMode mode)
	{
		if (!base.Error)
		{
			Color textColor = base.Color;
			if (isSelecting)
			{
				textColor = base.ButtonColor;
			}
			else if (isBackLog && !colorChanged)
			{
				textColor = Config.LogColor;
			}
			graph.DrawString(base.Str, base.PointX, pointY, textColor, StringStyle.Fontname, StringStyle.FontStyle);
		}
	}

	public override void GDIDrawTo(int pointY, bool isSelecting, bool isBackLog)
	{
		throw new NotImplementedException();
	}
}
