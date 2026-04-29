using System.Collections.Generic;
using Android.Graphics;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameView;

internal sealed class ConsoleButtonString
{
	private AConsoleDisplayPart[] strArray;

	private EmueraConsole parent;

	public AConsoleDisplayPart[] StrArray => strArray;

	public ConsoleDisplayLine ParentLine { get; set; }

	public bool IsButton { get; private set; }

	public bool IsInteger { get; private set; }

	public long Input { get; private set; }

	public string Inputs { get; private set; }

	public int PointX { get; set; }

	public bool PointXisLocked { get; set; }

	public int Width { get; set; }

	public float XsubPixel { get; set; }

	public long Generation { get; private set; }

	public ScriptPosition ErrPos { get; set; }

	public string Title { get; set; }

	public int RelativePointX { get; private set; }

	public ConsoleButtonString(EmueraConsole console, AConsoleDisplayPart[] strs)
	{
		parent = console;
		strArray = strs;
		IsButton = false;
		PointX = -1;
		Width = -1;
		ErrPos = null;
	}

	public ConsoleButtonString(EmueraConsole console, AConsoleDisplayPart[] strs, long input)
		: this(console, strs)
	{
		Input = input;
		Inputs = input.ToString();
		IsButton = true;
		IsInteger = true;
		if (console != null)
		{
			Generation = parent.NewButtonGeneration;
			console.UpdateGeneration();
		}
		ErrPos = null;
	}

	public ConsoleButtonString(EmueraConsole console, AConsoleDisplayPart[] strs, string inputs)
		: this(console, strs)
	{
		Inputs = inputs;
		IsButton = true;
		IsInteger = false;
		if (console != null)
		{
			Generation = parent.NewButtonGeneration;
			console.UpdateGeneration();
		}
		ErrPos = null;
	}

	public ConsoleButtonString(EmueraConsole console, AConsoleDisplayPart[] strs, long input, string inputs)
		: this(console, strs)
	{
		Input = input;
		Inputs = inputs;
		IsButton = true;
		IsInteger = true;
		if (console != null)
		{
			Generation = parent.NewButtonGeneration;
			console.UpdateGeneration();
		}
		ErrPos = null;
	}

	public ConsoleButtonString(EmueraConsole console, AConsoleDisplayPart[] strs, string inputs, ScriptPosition pos)
		: this(console, strs)
	{
		Inputs = inputs;
		IsButton = true;
		IsInteger = false;
		if (console != null)
		{
			Generation = parent.NewButtonGeneration;
			console.UpdateGeneration();
		}
		ErrPos = pos;
	}

	public void LockPointX(int rel_px)
	{
		PointX = rel_px * Config.FontSize / 100;
		XsubPixel = (float)(rel_px * Config.FontSize) / 100f - (float)PointX;
		PointXisLocked = true;
		RelativePointX = rel_px;
	}

	public ConsoleButtonString DivideAt(int divIndex, StringMeasure sm)
	{
		if (divIndex <= 0)
		{
			return null;
		}
		List<AConsoleDisplayPart> list = new List<AConsoleDisplayPart>();
		List<AConsoleDisplayPart> list2 = new List<AConsoleDisplayPart>();
		int num = 0;
		int num2 = 0;
		bool flag = false;
		for (num2 = 0; num2 < strArray.Length; num2++)
		{
			if (flag)
			{
				list2.Add(strArray[num2]);
				continue;
			}
			int length = strArray[num2].Str.Length;
			if (divIndex < num + length)
			{
				if (!(strArray[num2] is ConsoleStyledString { CanDivide: not false } consoleStyledString))
				{
					throw new ExeEE("文字列分割異常");
				}
				ConsoleStyledString consoleStyledString2 = consoleStyledString.DivideAt(divIndex - num, sm);
				list.Add(consoleStyledString);
				if (consoleStyledString2 != null)
				{
					list2.Add(consoleStyledString2);
				}
				flag = true;
			}
			else if (divIndex == num + length)
			{
				list.Add(strArray[num2]);
				flag = true;
			}
			else
			{
				num += length;
				list.Add(strArray[num2]);
			}
		}
		if (num2 >= strArray.Length && list2.Count == 0)
		{
			return null;
		}
		AConsoleDisplayPart[] array = new AConsoleDisplayPart[list.Count];
		AConsoleDisplayPart[] array2 = new AConsoleDisplayPart[list2.Count];
		list.CopyTo(array);
		list2.CopyTo(array2);
		strArray = array;
		ConsoleButtonString consoleButtonString = new ConsoleButtonString(null, array2);
		CalcWidth(sm, XsubPixel);
		consoleButtonString.CalcWidth(sm, 0f);
		CalcPointX(PointX);
		consoleButtonString.CalcPointX(PointX + Width);
		consoleButtonString.parent = parent;
		consoleButtonString.ParentLine = ParentLine;
		consoleButtonString.IsButton = IsButton;
		consoleButtonString.IsInteger = IsInteger;
		consoleButtonString.Input = Input;
		consoleButtonString.Inputs = Inputs;
		consoleButtonString.Generation = Generation;
		consoleButtonString.ErrPos = ErrPos;
		consoleButtonString.Title = Title;
		return consoleButtonString;
	}

	public void CalcWidth(StringMeasure sm, float subpixel)
	{
		Width = -1;
		if (strArray != null && strArray.Length != 0)
		{
			Width = 0;
			AConsoleDisplayPart[] array = strArray;
			foreach (AConsoleDisplayPart aConsoleDisplayPart in array)
			{
				if (aConsoleDisplayPart.Width <= 0)
				{
					aConsoleDisplayPart.SetWidth(sm, subpixel);
				}
				Width += aConsoleDisplayPart.Width;
				subpixel = aConsoleDisplayPart.XsubPixel;
			}
			if (Width <= 0)
			{
				Width = -1;
			}
		}
		XsubPixel = subpixel;
	}

	public void CalcPointX(int pointx)
	{
		int num = pointx;
		if (!PointXisLocked)
		{
			PointX = num;
		}
		else
		{
			num = PointX;
		}
		for (int i = 0; i < strArray.Length; i++)
		{
			strArray[i].PointX = num;
			num += strArray[i].Width;
		}
		if (strArray.Length != 0)
		{
			PointX = strArray[0].PointX;
			Width = strArray[strArray.Length - 1].PointX + strArray[strArray.Length - 1].Width - PointX;
		}
	}

	internal void ShiftPositionX(int shiftX)
	{
		PointX += shiftX;
		AConsoleDisplayPart[] array = strArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].PointX += shiftX;
		}
	}

	public void DrawTo(Canvas graph, int pointY, bool isBackLog, TextDrawingMode mode)
	{
		bool isSelecting = IsButton && parent.ButtonIsSelected(this);
		AConsoleDisplayPart[] array = strArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DrawTo(graph, pointY, isSelecting, isBackLog, mode);
		}
	}

	public void GDIDrawTo(int pointY, bool isBackLog)
	{
		bool isSelecting = IsButton && parent.ButtonIsSelected(this);
		AConsoleDisplayPart[] array = strArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GDIDrawTo(pointY, isSelecting, isBackLog);
		}
	}

	public override string ToString()
	{
		if (strArray == null)
		{
			return "";
		}
		string text = "";
		AConsoleDisplayPart[] array = strArray;
		foreach (AConsoleDisplayPart aConsoleDisplayPart in array)
		{
			text += aConsoleDisplayPart.ToString();
		}
		return text;
	}
}
