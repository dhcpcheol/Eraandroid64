using System;
using Android.Graphics;

namespace MinorShift.Emuera.GameView;

internal struct StringStyle
{
	public Color Color;

	public Color ButtonColor;

	public bool ColorChanged;

	public TypefaceStyle FontStyle;

	public string Fontname;

	public StringStyle(Color color, TypefaceStyle fontStyle, string fontname)
	{
		Color = color;
		ButtonColor = Config.FocusColor;
		ColorChanged = false;
		FontStyle = fontStyle;
		if (string.IsNullOrEmpty(fontname))
		{
			Fontname = Config.FontName;
		}
		else
		{
			Fontname = fontname;
		}
	}

	public StringStyle(Color color, bool colorChanged, Color buttonColor, TypefaceStyle fontStyle, string fontname)
	{
		Color = color;
		ButtonColor = buttonColor;
		ColorChanged = colorChanged;
		FontStyle = fontStyle;
		if (string.IsNullOrEmpty(fontname))
		{
			Fontname = Config.FontName;
		}
		else
		{
			Fontname = fontname;
		}
	}

	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is StringStyle stringStyle))
		{
			return false;
		}
		if (Color == stringStyle.Color && ButtonColor == stringStyle.ButtonColor && ColorChanged == stringStyle.ColorChanged && FontStyle == stringStyle.FontStyle)
		{
			return Fontname.Equals(stringStyle.Fontname, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Color.GetHashCode() ^ ButtonColor.GetHashCode() ^ ColorChanged.GetHashCode() ^ Fontname.GetHashCode();
	}

	public static bool operator ==(StringStyle x, StringStyle y)
	{
		if (x.Color == y.Color && x.ButtonColor == y.ButtonColor && x.ColorChanged == y.ColorChanged)
		{
			return x.Fontname.Equals(y.Fontname, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public static bool operator !=(StringStyle x, StringStyle y)
	{
		return !(x == y);
	}
}
