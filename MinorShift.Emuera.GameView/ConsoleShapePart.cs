using System.Drawing;
using System.Text;
using Android.Graphics;

namespace MinorShift.Emuera.GameView;

internal abstract class ConsoleShapePart : AConsoleColoredPart
{
	public override bool CanDivide => false;

	public static ConsoleShapePart CreateShape(string shapeType, int[] param, Android.Graphics.Color color, Android.Graphics.Color bcolor, bool colorchanged)
	{
		string text = shapeType.ToLower();
		colorchanged = colorchanged || color != Config.ForeColor;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<shape type='");
		stringBuilder.Append(text);
		stringBuilder.Append("' param='");
		for (int i = 0; i < param.Length; i++)
		{
			stringBuilder.Append(param[i].ToString());
			if (i < param.Length - 1)
			{
				stringBuilder.Append(", ");
			}
		}
		stringBuilder.Append("'");
		if (colorchanged)
		{
			stringBuilder.Append(" color='");
			stringBuilder.Append(HtmlManager.GetColorToString(color));
			stringBuilder.Append("'");
		}
		if (bcolor != Config.FocusColor)
		{
			stringBuilder.Append(" bcolor='");
			stringBuilder.Append(HtmlManager.GetColorToString(bcolor));
			stringBuilder.Append("'");
		}
		stringBuilder.Append(">");
		ConsoleShapePart consoleShapePart = null;
		int fontSize = Config.FontSize;
		float[] array = new float[param.Length];
		for (int j = 0; j < param.Length; j++)
		{
			array[j] = (float)param[j] * (float)fontSize / 100f;
		}
		switch (text)
		{
		case "space":
			if (array.Length == 1 && array[0] >= 0f)
			{
				RectangleF theRect = new RectangleF(0f, 0f, array[0], fontSize);
				consoleShapePart = new ConsoleSpacePart(theRect);
			}
			break;
		case "rect":
			if (array.Length == 1 && array[0] > 0f)
			{
				RectangleF theRect = new RectangleF(0f, 0f, array[0], fontSize);
				consoleShapePart = new ConsoleRectangleShapePart(theRect);
			}
			else if (array.Length == 4)
			{
				RectangleF theRect = new RectangleF(array[0], array[1], array[2], array[3]);
				if (theRect.X >= 0f && theRect.Width > 0f && theRect.Height > 0f)
				{
					consoleShapePart = new ConsoleRectangleShapePart(theRect);
				}
			}
			break;
		}
		if (consoleShapePart == null)
		{
			consoleShapePart = new ConsoleErrorShapePart(stringBuilder.ToString());
		}
		consoleShapePart.AltText = stringBuilder.ToString();
		consoleShapePart.Color = color;
		consoleShapePart.ButtonColor = bcolor;
		consoleShapePart.colorChanged = colorchanged;
		return consoleShapePart;
	}

	public override string ToString()
	{
		if (base.AltText == null)
		{
			return "";
		}
		return base.AltText;
	}
}
