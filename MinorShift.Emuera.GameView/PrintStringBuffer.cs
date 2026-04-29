using System;
using System.Collections.Generic;
using System.Text;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameView;

internal sealed class PrintStringBuffer
{
	private readonly EmueraConsole parent;

	private StringBuilder builder = new StringBuilder();

	private List<AConsoleDisplayPart> m_stringList = new List<AConsoleDisplayPart>();

	private StringStyle lastStringStyle;

	private List<ConsoleButtonString> m_buttonList = new List<ConsoleButtonString>();

	public int BufferStrLength
	{
		get
		{
			int num = 0;
			foreach (AConsoleDisplayPart @string in m_stringList)
			{
				num = ((!(@string is ConsoleStyledString)) ? (num + 1) : (num + @string.Str.Length));
			}
			return num;
		}
	}

	public bool IsEmpty
	{
		get
		{
			if (m_buttonList.Count == 0 && builder.Length == 0)
			{
				return m_stringList.Count == 0;
			}
			return false;
		}
	}

	public PrintStringBuffer(EmueraConsole parent)
	{
		this.parent = parent;
	}

	public void Append(AConsoleDisplayPart part)
	{
		if (builder.Length != 0)
		{
			m_stringList.Add(new ConsoleStyledString(builder.ToString(), lastStringStyle));
			builder.Remove(0, builder.Length);
		}
		m_stringList.Add(part);
	}

	public void Append(string str, StringStyle style)
	{
		Append(str, style, force_button: false);
	}

	public void Append(string str, StringStyle style, bool force_button)
	{
		if (BufferStrLength > 2000)
		{
			return;
		}
		if (force_button)
		{
			fromCssToButton();
		}
		if (builder.Length == 0 || lastStringStyle == style)
		{
			if (builder.Length > 2000)
			{
				return;
			}
			if (builder.Length + str.Length > 2000)
			{
				str = str.Substring(0, 2000 - builder.Length) + "※※※バッファーの文字数が2000字(全角1000字)を超えています。これ以降は表示できません※※※";
			}
			builder.Append(str);
			lastStringStyle = style;
		}
		else
		{
			m_stringList.Add(new ConsoleStyledString(builder.ToString(), lastStringStyle));
			builder.Remove(0, builder.Length);
			builder.Append(str);
			lastStringStyle = style;
		}
		if (force_button)
		{
			fromCssToButton();
		}
	}

	public void AppendButton(string str, StringStyle style, string input)
	{
		fromCssToButton();
		m_stringList.Add(new ConsoleStyledString(str, style));
		if (m_stringList.Count != 0)
		{
			m_buttonList.Add(createButton(m_stringList, input));
			m_stringList.Clear();
		}
	}

	public void AppendButton(string str, StringStyle style, long input)
	{
		fromCssToButton();
		m_stringList.Add(new ConsoleStyledString(str, style));
		if (m_stringList.Count != 0)
		{
			m_buttonList.Add(createButton(m_stringList, input));
			m_stringList.Clear();
		}
	}

	public void AppendPlainText(string str, StringStyle style)
	{
		fromCssToButton();
		m_stringList.Add(new ConsoleStyledString(str, style));
		if (m_stringList.Count != 0)
		{
			m_buttonList.Add(createPlainButton(m_stringList));
			m_stringList.Clear();
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ConsoleButtonString button in m_buttonList)
		{
			stringBuilder.Append(button.ToString());
		}
		foreach (AConsoleDisplayPart @string in m_stringList)
		{
			stringBuilder.Append(@string.Str);
		}
		stringBuilder.Append(builder);
		return stringBuilder.ToString();
	}

	public ConsoleDisplayLine AppendAndFlushErrButton(string str, StringStyle style, string input, ScriptPosition pos, StringMeasure sm)
	{
		fromCssToButton();
		m_stringList.Add(new ConsoleStyledString(str, style));
		if (m_stringList.Count == 0)
		{
			return null;
		}
		m_buttonList.Add(createButton(m_stringList, input, pos));
		m_stringList.Clear();
		return FlushSingleLine(sm, temporary: false);
	}

	public ConsoleDisplayLine FlushSingleLine(StringMeasure stringMeasure, bool temporary)
	{
		fromCssToButton();
		setWidthToButtonList(m_buttonList, stringMeasure, nobr: true);
		ConsoleButtonString[] array = new ConsoleButtonString[m_buttonList.Count];
		m_buttonList.CopyTo(array);
		ConsoleDisplayLine result = new ConsoleDisplayLine(array, isLogical: true, temporary);
		clearBuffer();
		return result;
	}

	public ConsoleDisplayLine[] Flush(StringMeasure stringMeasure, bool temporary)
	{
		fromCssToButton();
		ConsoleDisplayLine[] result = ButtonsToDisplayLines(m_buttonList, stringMeasure, nobr: false, temporary);
		clearBuffer();
		return result;
	}

	private static ConsoleDisplayLine m_buttonsToDisplayLine(List<ConsoleButtonString> lineButtonList, bool firstLine, bool temporary)
	{
		ConsoleButtonString[] array = new ConsoleButtonString[lineButtonList.Count];
		lineButtonList.CopyTo(array);
		lineButtonList.Clear();
		return new ConsoleDisplayLine(array, firstLine, temporary);
	}

	public static ConsoleDisplayLine[] ButtonsToDisplayLines(List<ConsoleButtonString> buttonList, StringMeasure stringMeasure, bool nobr, bool temporary)
	{
		if (buttonList.Count == 0)
		{
			return new ConsoleDisplayLine[0];
		}
		setWidthToButtonList(buttonList, stringMeasure, nobr);
		List<ConsoleDisplayLine> list = new List<ConsoleDisplayLine>();
		List<ConsoleButtonString> list2 = new List<ConsoleButtonString>();
		int drawableWidth = Config.DrawableWidth;
		bool firstLine = true;
		for (int i = 0; i < buttonList.Count; i++)
		{
			if (buttonList[i] == null)
			{
				list.Add(m_buttonsToDisplayLine(list2, firstLine, temporary));
				firstLine = false;
				buttonList.RemoveAt(i);
				i--;
				continue;
			}
			if (nobr || buttonList[i].PointX + buttonList[i].Width <= drawableWidth)
			{
				list2.Add(buttonList[i]);
				continue;
			}
			if (!Config.ButtonWrap || list2.Count == 0 || (!buttonList[i].IsButton && !Config.CompatiLinefeedAs1739))
			{
				int divideIndex = getDivideIndex(buttonList[i], stringMeasure);
				if (divideIndex > 0)
				{
					ConsoleButtonString item = buttonList[i].DivideAt(divideIndex, stringMeasure);
					buttonList.Insert(i + 1, item);
					list2.Add(buttonList[i]);
					i++;
				}
				else if (divideIndex != 0 || list2.Count <= 0)
				{
					list2.Add(buttonList[i]);
					continue;
				}
			}
			list.Add(m_buttonsToDisplayLine(list2, firstLine, temporary));
			firstLine = false;
			int num = 0;
			for (int j = i; j < buttonList.Count && buttonList[j] != null; j++)
			{
				buttonList[j].CalcPointX(num);
				num += buttonList[j].Width;
			}
			i--;
		}
		if (list2.Count > 0)
		{
			list.Add(m_buttonsToDisplayLine(list2, firstLine, temporary));
		}
		ConsoleDisplayLine[] array = new ConsoleDisplayLine[list.Count];
		list.CopyTo(array);
		return array;
	}

	public ConsoleDisplayLine[] PrintHtml(string str, StringMeasure stringMeasure)
	{
		throw new NotImplementedException();
	}

	private void clearBuffer()
	{
		builder.Remove(0, builder.Length);
		m_stringList.Clear();
		m_buttonList.Clear();
	}

	private void fromCssToButton()
	{
		if (builder.Length != 0)
		{
			m_stringList.Add(new ConsoleStyledString(builder.ToString(), lastStringStyle));
			builder.Remove(0, builder.Length);
		}
		if (m_stringList.Count != 0)
		{
			m_buttonList.AddRange(createButtons(m_stringList));
			m_stringList.Clear();
		}
	}

	private ConsoleButtonString createButton(List<AConsoleDisplayPart> cssList, string input)
	{
		AConsoleDisplayPart[] array = new AConsoleDisplayPart[cssList.Count];
		cssList.CopyTo(array);
		cssList.Clear();
		return new ConsoleButtonString(parent, array, input);
	}

	private ConsoleButtonString createButton(List<AConsoleDisplayPart> cssList, string input, ScriptPosition pos)
	{
		AConsoleDisplayPart[] array = new AConsoleDisplayPart[cssList.Count];
		cssList.CopyTo(array);
		cssList.Clear();
		return new ConsoleButtonString(parent, array, input, pos);
	}

	private ConsoleButtonString createButton(List<AConsoleDisplayPart> cssList, long input)
	{
		AConsoleDisplayPart[] array = new AConsoleDisplayPart[cssList.Count];
		cssList.CopyTo(array);
		cssList.Clear();
		return new ConsoleButtonString(parent, array, input);
	}

	private ConsoleButtonString createPlainButton(List<AConsoleDisplayPart> cssList)
	{
		AConsoleDisplayPart[] array = new AConsoleDisplayPart[cssList.Count];
		cssList.CopyTo(array);
		cssList.Clear();
		return new ConsoleButtonString(parent, array);
	}

	private ConsoleButtonString[] createButtons(List<AConsoleDisplayPart> cssList)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < cssList.Count; i++)
		{
			stringBuilder.Append(cssList[i].Str);
		}
		List<ButtonPrimitive> list = ButtonStringCreator.SplitButton(stringBuilder.ToString());
		ConsoleButtonString[] array = new ConsoleButtonString[list.Count];
		AConsoleDisplayPart[] array2 = null;
		if (array.Length == 1)
		{
			array2 = new AConsoleDisplayPart[cssList.Count];
			cssList.CopyTo(array2);
			if (list[0].CanSelect)
			{
				array[0] = new ConsoleButtonString(parent, array2, list[0].Input);
			}
			else
			{
				array[0] = new ConsoleButtonString(parent, array2);
			}
			return array;
		}
		int num = 0;
		int num2 = 0;
		int j = 0;
		List<AConsoleDisplayPart> list2 = new List<AConsoleDisplayPart>();
		for (int k = 0; k < array.Length; k++)
		{
			ButtonPrimitive buttonPrimitive = list[k];
			num2 += buttonPrimitive.Str.Length;
			for (; j < cssList.Count; j++)
			{
				AConsoleDisplayPart aConsoleDisplayPart = cssList[j];
				if (num + aConsoleDisplayPart.Str.Length >= num2)
				{
					int num3 = num2 - num;
					if (num3 > 0 && aConsoleDisplayPart.CanDivide)
					{
						ConsoleStyledString consoleStyledString = ((ConsoleStyledString)aConsoleDisplayPart).DivideAt(num3);
						if (consoleStyledString != null)
						{
							cssList.Insert(j + 1, consoleStyledString);
							consoleStyledString.PointX = aConsoleDisplayPart.PointX + aConsoleDisplayPart.Width;
						}
					}
					list2.Add(aConsoleDisplayPart);
					num += aConsoleDisplayPart.Str.Length;
					j++;
					break;
				}
				list2.Add(aConsoleDisplayPart);
				num += aConsoleDisplayPart.Str.Length;
			}
			array2 = new AConsoleDisplayPart[list2.Count];
			list2.CopyTo(array2);
			if (buttonPrimitive.CanSelect)
			{
				array[k] = new ConsoleButtonString(parent, array2, buttonPrimitive.Input);
			}
			else
			{
				array[k] = new ConsoleButtonString(parent, array2);
			}
			list2.Clear();
		}
		return array;
	}

	private static void setWidthToButtonList(List<ConsoleButtonString> buttonList, StringMeasure stringMeasure, bool nobr)
	{
		int pointx = 0;
		_ = buttonList.Count;
		float subpixel = 0f;
		for (int i = 0; i < buttonList.Count; i++)
		{
			ConsoleButtonString consoleButtonString = buttonList[i];
			if (consoleButtonString == null)
			{
				pointx = 0;
				continue;
			}
			consoleButtonString.CalcWidth(stringMeasure, subpixel);
			consoleButtonString.CalcPointX(pointx);
			pointx = consoleButtonString.PointX + consoleButtonString.Width;
			if (consoleButtonString.PointXisLocked)
			{
				subpixel = 0f;
			}
			subpixel = consoleButtonString.XsubPixel;
		}
	}

	private static int getDivideIndex(ConsoleButtonString button, StringMeasure sm)
	{
		AConsoleDisplayPart aConsoleDisplayPart = null;
		int num = button.PointX;
		int num2 = 0;
		int num3 = 0;
		AConsoleDisplayPart[] strArray = button.StrArray;
		foreach (AConsoleDisplayPart aConsoleDisplayPart2 in strArray)
		{
			if (num + aConsoleDisplayPart2.Width > Config.DrawableWidth)
			{
				if (num3 != 0 || aConsoleDisplayPart2.CanDivide)
				{
					aConsoleDisplayPart = aConsoleDisplayPart2;
					break;
				}
			}
			else
			{
				num3++;
				num2 += aConsoleDisplayPart2.Str.Length;
				num += aConsoleDisplayPart2.Width;
			}
		}
		if (aConsoleDisplayPart != null)
		{
			int divideIndex = getDivideIndex(aConsoleDisplayPart, sm);
			if (divideIndex > 0)
			{
				num2 += divideIndex;
			}
		}
		return num2;
	}

	private static int getDivideIndex(AConsoleDisplayPart part, StringMeasure sm)
	{
		if (!part.CanDivide)
		{
			return -1;
		}
		ConsoleStyledString consoleStyledString = part as ConsoleStyledString;
		if (part == null)
		{
			return -1;
		}
		int num = Config.DrawableWidth - consoleStyledString.PointX;
		string str = consoleStyledString.Str;
		int num2 = str.Length;
		int num3 = 0;
		int num4 = num3;
		string text = null;
		while (num2 - num3 > 1)
		{
			text = str.Substring(0, num4);
			if (sm.GetDisplayLength(text, Config.FontSize) <= num)
			{
				num3 = num4;
				num4++;
			}
			else
			{
				num2 = num4;
				num4--;
			}
		}
		return num3;
	}
}
