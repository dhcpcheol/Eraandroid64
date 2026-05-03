using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using Android.Graphics;
using MinorShift.Emuera;

namespace MinorShift;

internal static class ConvertingTools
{
	private static Rect r = new Rect();

	private static Paint paint = new Paint
	{
		Hinting = PaintHinting.On,
		AntiAlias = true
	};

	public static Rect ToRect(this Rectangle rectangle)
	{
		return new Rect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
	}

	public static void FillRect(this Canvas canvas, Rect rect, Paint paint)
	{
		canvas.DrawPath(rect.ToPath(), paint);
	}

    public static void DrawString(this Canvas canvas, string str, float x, float y, string fontName, TypefaceStyle style = TypefaceStyle.Normal)
    {
        paint.Color = Config.ForeColor;
		paint.TextSize = Config.FontSize;
        canvas.DrawText(str, x, y - paint.Ascent(), paint);
    }

    public static void DrawString(this Canvas canvas, string str, float x, float y, Android.Graphics.Color textColor, string fontName, TypefaceStyle style = TypefaceStyle.Normal)
    {
        paint.Color = textColor;
        paint.TextSize = Config.FontSize;
        canvas.DrawText(str, x, y - paint.Ascent(), paint);
    }

    public static string ToHalf(this string WideStr)
	{
		if (WideStr == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		char c = '\0';
		for (int i = 0; i < WideStr.Length; i++)
		{
			c = WideStr[i];
			if (c >= '！' && c <= '～')
			{
				c = (char)(c - 65248);
			}
			else if (c == '\u3000')
			{
				c = ' ';
			}
			stringBuilder.Append(c);
		}
		return stringBuilder.ToString();
	}

	public static string ToWide(this string HalfStr)
	{
		if (HalfStr == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		char c = '\0';
		for (int i = 0; i < HalfStr.Length; i++)
		{
			c = HalfStr[i];
			if (c >= '!' && c <= '~')
			{
				c = (char)(c + 65248);
			}
			else if (c == ' ')
			{
				c = '\u3000';
			}
			stringBuilder.Append(c);
		}
		return stringBuilder.ToString();
	}

    public static Android.Graphics.Path ToPath(this Rect rect)
    {
        Android.Graphics.Path path = new Android.Graphics.Path();
        path.MoveTo(rect.Left, rect.Top);
		path.MoveTo(rect.Left, rect.Bottom);
		path.MoveTo(rect.Right, rect.Bottom);
		path.MoveTo(rect.Right, rect.Top);
		path.MoveTo(rect.Left, rect.Top);
		return path;
	}

	private static string WildcardToRegex(string pattern)
	{
		return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
	}

	public static bool MatchWithWildcard(this string str, string pattern)
	{
		return new Regex(WildcardToRegex(pattern), RegexOptions.IgnoreCase).IsMatch(str);
	}
}
