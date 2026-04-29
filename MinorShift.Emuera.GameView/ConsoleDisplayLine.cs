using System.Drawing;
using System.Text;
using Android.Graphics;

namespace MinorShift.Emuera.GameView;

internal sealed class ConsoleDisplayLine
{
	public int LineNo = -1;

	public readonly bool IsLogicalLine = true;

	public readonly bool IsTemporary;

	private ConsoleButtonString[] buttons;

	private DisplayLineAlignment align;

	private bool aligned;

	public ConsoleButtonString[] Buttons => buttons;

	public DisplayLineAlignment Align => align;

	public ConsoleDisplayLine(ConsoleButtonString[] buttons, bool isLogical, bool temporary)
	{
		if (buttons == null)
		{
			buttons = new ConsoleButtonString[0];
			return;
		}
		this.buttons = buttons;
		ConsoleButtonString[] array = buttons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ParentLine = this;
		}
		IsLogicalLine = isLogical;
		IsTemporary = temporary;
	}

	public void SetAlignment(DisplayLineAlignment align)
	{
		if (aligned)
		{
			return;
		}
		aligned = true;
		this.align = align;
		if (buttons.Length == 0)
		{
			return;
		}
		int num = 0;
		ConsoleButtonString[] array = buttons;
		foreach (ConsoleButtonString consoleButtonString in array)
		{
			num += consoleButtonString.Width;
		}
		int pointX = buttons[0].PointX;
		int num2 = 0;
		switch (align)
		{
		case DisplayLineAlignment.LEFT:
			if (IsLogicalLine)
			{
				return;
			}
			num2 = 0;
			break;
		case DisplayLineAlignment.CENTER:
			num2 = Config.WindowX / 2 - num / 2;
			break;
		case DisplayLineAlignment.RIGHT:
			num2 = Config.WindowX - num;
			break;
		}
		int num3 = num2 - pointX;
		if (num3 != 0)
		{
			ShiftPositionX(num3);
		}
	}

	public void ShiftPositionX(int shiftX)
	{
		ConsoleButtonString[] array = buttons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ShiftPositionX(shiftX);
		}
	}

	public void ChangeStr(ConsoleButtonString[] newButtons)
	{
		buttons = null;
		for (int i = 0; i < newButtons.Length; i++)
		{
			newButtons[i].ParentLine = this;
		}
		buttons = newButtons;
	}

	public void Clear(Paint paint, Canvas graph, int pointY)
	{
		Rectangle rectangle = new Rectangle(0, pointY, Config.WindowX, Config.LineHeight);
		graph.FillRect(rectangle.ToRect(), paint);
	}

	public void DrawTo(Canvas graph, int pointY, bool isBackLog, bool force, TextDrawingMode mode)
	{
		ConsoleButtonString[] array = buttons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DrawTo(graph, pointY, isBackLog, mode);
		}
	}

	public void GDIDrawTo(int pointY, bool isBackLog)
	{
		ConsoleButtonString[] array = buttons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GDIDrawTo(pointY, isBackLog);
		}
	}

	public override string ToString()
	{
		if (buttons == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		ConsoleButtonString[] array = buttons;
		foreach (ConsoleButtonString consoleButtonString in array)
		{
			stringBuilder.Append(consoleButtonString.ToString());
		}
		return stringBuilder.ToString();
	}
}
