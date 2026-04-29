using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Android.Graphics;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameView;

internal static class HtmlManager
{
	private sealed class HtmlAnalzeStateFontTag
	{
		public int Color = -1;

		public int BColor = -1;

		public string FontName;
	}

	private sealed class HtmlAnalzeStateButtonTag
	{
		public bool IsButton = true;

		public bool IsButtonTag = true;

		public int ButtonValueInt;

		public string ButtonValueStr;

		public string ButtonTitle;

		public bool ButtonIsInteger;

		public int PointX;

		public bool PointXisLocked;
	}

	private sealed class HtmlAnalzeState
	{
		public bool LineHead = true;

		public TypefaceStyle FontStyle;

		public List<HtmlAnalzeStateFontTag> FonttagList = new List<HtmlAnalzeStateFontTag>();

		public bool FlagNobr;

		public bool FlagP;

		public bool FlagNobrClosed;

		public bool FlagPClosed;

		public DisplayLineAlignment Alignment;

		public HtmlAnalzeStateButtonTag LastButtonTag;

		public HtmlAnalzeStateButtonTag CurrentButtonTag;

		public bool FlagBr;

		public bool FlagButton;

		public StringStyle GetSS()
		{
			Color color = Config.ForeColor;
			Color buttonColor = Config.FocusColor;
			string fontname = null;
			bool colorChanged = false;
			if (FonttagList.Count > 0)
			{
				HtmlAnalzeStateFontTag htmlAnalzeStateFontTag = FonttagList[FonttagList.Count - 1];
				fontname = htmlAnalzeStateFontTag.FontName;
				if (htmlAnalzeStateFontTag.Color >= 0)
				{
					colorChanged = true;
					color = new Color(htmlAnalzeStateFontTag.Color >> 16, (htmlAnalzeStateFontTag.Color >> 8) & 0xFF, htmlAnalzeStateFontTag.Color & 0xFF);
				}
				if (htmlAnalzeStateFontTag.BColor >= 0)
				{
					buttonColor = new Color(htmlAnalzeStateFontTag.BColor >> 16, (htmlAnalzeStateFontTag.BColor >> 8) & 0xFF, htmlAnalzeStateFontTag.BColor & 0xFF);
				}
			}
			return new StringStyle(color, colorChanged, buttonColor, FontStyle, fontname);
		}
	}

	private static readonly char[] rep;

	private static readonly Dictionary<char, string> repDic;

	static HtmlManager()
	{
		rep = new char[5] { '&', '>', '<', '"', '\'' };
		repDic = new Dictionary<char, string>();
		repDic.Add('&', "&amp;");
		repDic.Add('>', "&gt;");
		repDic.Add('<', "&lt;");
		repDic.Add('"', "&quot;");
		repDic.Add('\'', "&apos;");
	}

	public static string DisplayLine2Html(ConsoleDisplayLine[] lines, bool needPandN)
	{
		if (lines == null || lines.Length == 0)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (needPandN)
		{
			switch (lines[0].Align)
			{
			case DisplayLineAlignment.LEFT:
				stringBuilder.Append("<p align='left'>");
				break;
			case DisplayLineAlignment.CENTER:
				stringBuilder.Append("<p align='center'>");
				break;
			case DisplayLineAlignment.RIGHT:
				stringBuilder.Append("<p align='right'>");
				break;
			}
			stringBuilder.Append("<nobr>");
		}
		for (int i = 0; i < lines.Length; i++)
		{
			if (i != 0)
			{
				stringBuilder.Append("<br>");
			}
			ConsoleButtonString[] buttons = lines[i].Buttons;
			for (int j = 0; j < buttons.Length; j++)
			{
				string text = null;
				if (!string.IsNullOrEmpty(buttons[j].Title))
				{
					text = Escape(buttons[j].Title);
				}
				bool flag = buttons[j].IsButton || text != null || buttons[j].PointXisLocked;
				if (flag)
				{
					if (buttons[j].IsButton)
					{
						string value = Escape(buttons[j].Inputs);
						stringBuilder.Append("<button value='");
						stringBuilder.Append(value);
						stringBuilder.Append("'");
					}
					else
					{
						stringBuilder.Append("<nonbutton");
					}
					if (text != null)
					{
						stringBuilder.Append(" title='");
						stringBuilder.Append(text);
						stringBuilder.Append("'");
					}
					if (buttons[j].PointXisLocked)
					{
						stringBuilder.Append(" pos='");
						stringBuilder.Append(buttons[j].RelativePointX.ToString());
						stringBuilder.Append("'");
					}
					stringBuilder.Append(">");
				}
				AConsoleDisplayPart[] strArray = buttons[j].StrArray;
				for (int k = 0; k < strArray.Length; k++)
				{
					if (strArray[k] is ConsoleStyledString)
					{
						ConsoleStyledString consoleStyledString = strArray[k] as ConsoleStyledString;
						stringBuilder.Append(getStringStyleStartingTag(consoleStyledString.StringStyle));
						stringBuilder.Append(Escape(consoleStyledString.Str));
						stringBuilder.Append(getClosingStyleStartingTag(consoleStyledString.StringStyle));
					}
					else if (strArray[k] is ConsoleImagePart)
					{
						stringBuilder.Append(strArray[k].AltText);
					}
					else if (strArray[k] is ConsoleShapePart)
					{
						stringBuilder.Append(strArray[k].AltText);
					}
				}
				if (flag)
				{
					if (buttons[j].IsButton)
					{
						stringBuilder.Append("</button>");
					}
					else
					{
						stringBuilder.Append("</nonbutton>");
					}
				}
			}
		}
		if (needPandN)
		{
			stringBuilder.Append("</nobr>");
			stringBuilder.Append("</p>");
		}
		return stringBuilder.ToString();
	}

	public static string[] HtmlTagSplit(string str)
	{
		List<string> list = new List<string>();
		StringStream stringStream = new StringStream(str);
		int num = -1;
		while (!stringStream.EOS)
		{
			num = stringStream.Find('<');
			if (num < 0)
			{
				list.Add(stringStream.Substring());
				break;
			}
			if (num > 0)
			{
				list.Add(stringStream.Substring(stringStream.CurrentPosition, num));
				stringStream.CurrentPosition += num;
			}
			num = stringStream.Find('>');
			if (num < 0)
			{
				return null;
			}
			num++;
			list.Add(stringStream.Substring(stringStream.CurrentPosition, num));
			stringStream.CurrentPosition += num;
		}
		string[] array = new string[list.Count];
		list.CopyTo(array);
		return array;
	}

	public static ConsoleDisplayLine[] Html2DisplayLine(string str, StringMeasure sm, EmueraConsole console)
	{
		List<AConsoleDisplayPart> list = new List<AConsoleDisplayPart>();
		List<ConsoleButtonString> list2 = new List<ConsoleButtonString>();
		StringStream stringStream = new StringStream(str);
		bool flag = str.IndexOf("<!--") >= 0;
		bool flag2 = str.IndexOf('\n') >= 0;
		HtmlAnalzeState htmlAnalzeState = new HtmlAnalzeState();
		while (!stringStream.EOS)
		{
			int num = stringStream.Find('<');
			if (flag2)
			{
				int num2 = stringStream.Find('\n');
				if (num2 >= 0 && (num > num2 || num < 0))
				{
					num = num2;
				}
			}
			if (num < 0)
			{
				string str2 = Unescape(stringStream.Substring());
				list.Add(new ConsoleStyledString(str2, htmlAnalzeState.GetSS()));
				if (htmlAnalzeState.FlagPClosed)
				{
					throw new CodeEE("</p>の後にテキストがあります");
				}
				if (!htmlAnalzeState.FlagNobrClosed)
				{
					break;
				}
				throw new CodeEE("</nobr>の後にテキストがあります");
			}
			if (num > 0)
			{
				string str3 = Unescape(stringStream.Substring(stringStream.CurrentPosition, num));
				list.Add(new ConsoleStyledString(str3, htmlAnalzeState.GetSS()));
				htmlAnalzeState.LineHead = false;
				stringStream.CurrentPosition += num;
			}
			if (flag && stringStream.CurrentEqualTo("<!--"))
			{
				stringStream.CurrentPosition += 4;
				num = stringStream.Find("-->");
				if (num < 0)
				{
					throw new CodeEE("コメンdト終了タグ\"-->\"がみつかりません");
				}
				stringStream.CurrentPosition += num + 3;
				continue;
			}
			if (flag2 && stringStream.Current == '\n')
			{
				htmlAnalzeState.FlagBr = true;
				stringStream.ShiftNext();
			}
			else
			{
				stringStream.ShiftNext();
				AConsoleDisplayPart aConsoleDisplayPart = tagAnalyze(htmlAnalzeState, stringStream);
				if (stringStream.Current != '>')
				{
					throw new CodeEE("タグ終端'>'が見つかりません");
				}
				if (aConsoleDisplayPart != null)
				{
					list.Add(aConsoleDisplayPart);
				}
				stringStream.ShiftNext();
			}
			if (htmlAnalzeState.FlagBr)
			{
				htmlAnalzeState.LastButtonTag = htmlAnalzeState.CurrentButtonTag;
				if (list.Count > 0)
				{
					list2.Add(cssToButton(list, htmlAnalzeState, console));
				}
				list2.Add(null);
			}
			if (htmlAnalzeState.FlagButton && list.Count > 0)
			{
				list2.Add(cssToButton(list, htmlAnalzeState, console));
			}
			htmlAnalzeState.FlagBr = false;
			htmlAnalzeState.FlagButton = false;
			htmlAnalzeState.LastButtonTag = htmlAnalzeState.CurrentButtonTag;
		}
		if (htmlAnalzeState.CurrentButtonTag != null || htmlAnalzeState.FonttagList.Count > 0)
		{
			throw new CodeEE("閉じられていないタグがあります");
		}
		if (list.Count > 0)
		{
			list2.Add(cssToButton(list, htmlAnalzeState, console));
		}
		foreach (ConsoleButtonString item in list2)
		{
			if (item != null && item.PointXisLocked)
			{
				if (!htmlAnalzeState.FlagNobr)
				{
					throw new CodeEE("<nobr>が設定されていない行ではpos属性は使用できません");
				}
				if (htmlAnalzeState.Alignment != DisplayLineAlignment.LEFT)
				{
					throw new CodeEE("alignがleftでない行ではpos属性は使用できません");
				}
				break;
			}
		}
		ConsoleDisplayLine[] array = PrintStringBuffer.ButtonsToDisplayLines(list2, sm, htmlAnalzeState.FlagNobr, temporary: false);
		ConsoleDisplayLine[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SetAlignment(htmlAnalzeState.Alignment);
		}
		return array;
	}

	public static string Html2PlainText(string str)
	{
		return Unescape(Regex.Replace(str, "\\<[^<]*\\>", ""));
	}

	public static string Escape(string str)
	{
		int num = 0;
		int num2 = 0;
		StringBuilder stringBuilder = new StringBuilder();
		while (num < str.Length)
		{
			num2 = str.IndexOfAny(rep, num);
			if (num2 < 0)
			{
				stringBuilder.Append(str.Substring(num));
				break;
			}
			if (num2 > num)
			{
				stringBuilder.Append(str.Substring(num, num2 - num));
			}
			string value = repDic[str[num2]];
			stringBuilder.Append(value);
			num = num2 + 1;
		}
		return stringBuilder.ToString();
	}

	public static string Unescape(string str)
	{
		int num = 0;
		int num2 = str.IndexOf('&', num);
		if (num2 < 0)
		{
			return str;
		}
		StringBuilder stringBuilder = new StringBuilder();
		while (num < str.Length)
		{
			num2 = str.IndexOf('&', num);
			if (num2 < 0)
			{
				stringBuilder.Append(str.Substring(num));
				break;
			}
			if (num2 > num)
			{
				stringBuilder.Append(str.Substring(num, num2 - num));
			}
			num = num2;
			num2 = str.IndexOf(';', num);
			if (num2 <= num + 1)
			{
				if (num2 < 0)
				{
					throw new CodeEE("'&'に対応する';'がみつかりません");
				}
				throw new CodeEE("'&'と';'が連続しています");
			}
			string text = str.Substring(num + 1, num2 - num - 1);
			num = num2 + 1;
			string text2 = text.ToLower();
			int num3 = 0;
			switch (text2)
			{
			case "nbsp":
				stringBuilder.Append(" ");
				continue;
			case "amp":
				stringBuilder.Append("&");
				continue;
			case "gt":
				stringBuilder.Append(">");
				continue;
			case "lt":
				stringBuilder.Append("<");
				continue;
			case "quot":
				stringBuilder.Append("\"");
				continue;
			case "apos":
				stringBuilder.Append("'");
				continue;
			}
			int fromBase = 10;
			if (text2[0] != '#')
			{
				throw new CodeEE("\"&" + text + ";\"は適切な文字参照ではありません");
			}
			if (text2.Length > 1 && text2[1] == 'x')
			{
				fromBase = 16;
				text2 = text2.Substring(2);
			}
			else
			{
				text2 = text2.Substring(1);
			}
			try
			{
				num3 = Convert.ToInt32(text2, fromBase);
			}
			catch
			{
				throw new CodeEE("\"&" + text + ";\"は適切な文字参照ではありません");
			}
			if (num3 < 0 || num3 > 65535)
			{
				throw new CodeEE("\"&" + text + ";\"はUnicodeの範囲外です(サロゲートペアは使えません)");
			}
			stringBuilder.Append((char)num3);
		}
		return stringBuilder.ToString();
	}

	private static ConsoleButtonString cssToButton(List<AConsoleDisplayPart> cssList, HtmlAnalzeState state, EmueraConsole console)
	{
		AConsoleDisplayPart[] array = new AConsoleDisplayPart[cssList.Count];
		cssList.CopyTo(array);
		cssList.Clear();
		ConsoleButtonString consoleButtonString = null;
		if (state.LastButtonTag != null && state.LastButtonTag.IsButton)
		{
			consoleButtonString = ((!state.LastButtonTag.ButtonIsInteger) ? new ConsoleButtonString(console, array, state.LastButtonTag.ButtonValueStr) : new ConsoleButtonString(console, array, state.LastButtonTag.ButtonValueInt, state.LastButtonTag.ButtonValueStr));
		}
		else
		{
			consoleButtonString = new ConsoleButtonString(console, array);
			consoleButtonString.Title = null;
		}
		if (state.LastButtonTag != null)
		{
			consoleButtonString.Title = state.LastButtonTag.ButtonTitle;
			if (state.LastButtonTag.PointXisLocked)
			{
				consoleButtonString.LockPointX(state.LastButtonTag.PointX);
			}
		}
		return consoleButtonString;
	}

	public static string GetColorToString(Color color)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("#");
		stringBuilder.Append((color.R * 65536 + color.G * 256 + color.B).ToString("X6"));
		return stringBuilder.ToString();
	}

	private static string getStringStyleStartingTag(StringStyle style)
	{
		bool flag = (style.Fontname != null && !(style.Fontname == Config.FontName)) || style.ColorChanged || !(style.ButtonColor == Config.FocusColor);
		if (!flag)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (flag)
		{
			stringBuilder.Append("<font");
			if (style.Fontname != null && style.Fontname != Config.FontName)
			{
				stringBuilder.Append(" face='");
				stringBuilder.Append(Escape(style.Fontname));
				stringBuilder.Append("'");
			}
			if (style.ColorChanged)
			{
				stringBuilder.Append(" color='#");
				stringBuilder.Append((style.Color.R * 65536 + style.Color.G * 256 + style.Color.B).ToString("X6"));
				stringBuilder.Append("'");
			}
			if (style.ButtonColor != Config.FocusColor)
			{
				stringBuilder.Append(" bcolor='#");
				stringBuilder.Append((style.ButtonColor.R * 65536 + style.ButtonColor.G * 256 + style.ButtonColor.B).ToString("X6"));
				stringBuilder.Append("'");
			}
			stringBuilder.Append(">");
		}
		return stringBuilder.ToString();
	}

	private static string getClosingStyleStartingTag(StringStyle style)
	{
		bool flag = (style.Fontname != null && !(style.Fontname == Config.FontName)) || style.ColorChanged || !(style.ButtonColor == Config.FocusColor);
		if (!flag)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (flag)
		{
			stringBuilder.Append("</font>");
		}
		return stringBuilder.ToString();
	}

	private static AConsoleDisplayPart tagAnalyze(HtmlAnalzeState state, StringStream st)
	{
		string text;
		if (st.Current == '/')
		{
			st.ShiftNext();
			int num = st.Find('>');
			if (num < 0)
			{
				st.CurrentPosition = st.RowString.Length;
				return null;
			}
			text = st.Substring(st.CurrentPosition, num).Trim();
			st.CurrentPosition += num;
		}
		bool useMacro = LexicalAnalyzer.UseMacro;
		WordCollection wordCollection = null;
		try
		{
			LexicalAnalyzer.UseMacro = false;
			text = LexicalAnalyzer.ReadSingleIdentifier(st);
			LexicalAnalyzer.SkipWhiteSpace(st);
			if (st.Current != '>')
			{
				wordCollection = LexicalAnalyzer.Analyse(st, LexEndWith.GreaterThan, (LexAnalyzeFlag)6);
			}
		}
		finally
		{
			LexicalAnalyzer.UseMacro = useMacro;
		}
		if (!string.IsNullOrEmpty(text))
		{
			switch (text.ToLower())
			{
			case "br":
				if (wordCollection != null)
				{
					throw new CodeEE("<" + text + ">タグにに属性が設定されています");
				}
				state.FlagBr = true;
				return null;
			case "nobr":
				if (wordCollection != null)
				{
					throw new CodeEE("<" + text + ">タグに属性が設定されています");
				}
				if (!state.LineHead)
				{
					throw new CodeEE("<nobr>が行頭以外で使われています");
				}
				if (state.FlagNobr)
				{
					throw new CodeEE("<nobr>が2度以上使われています");
				}
				state.FlagNobr = true;
				return null;
			case "p":
			{
				if (wordCollection == null)
				{
					throw new CodeEE("<" + text + ">タグに属性が設定されていません");
				}
				if (!state.LineHead)
				{
					throw new CodeEE("<p>が行頭以外で使われています");
				}
				if (state.FlagNobr)
				{
					throw new CodeEE("<p>が2度以上使われています");
				}
				IdentifierWord identifierWord = wordCollection.Current as IdentifierWord;
				wordCollection.ShiftNext();
				OperatorWord operatorWord4 = wordCollection.Current as OperatorWord;
				wordCollection.ShiftNext();
				LiteralStringWord literalStringWord4 = wordCollection.Current as LiteralStringWord;
				wordCollection.ShiftNext();
				if (wordCollection.EOL && identifierWord != null && operatorWord4 != null && operatorWord4.Code == OperatorCode.Assignment && literalStringWord4 != null)
				{
					if (!identifierWord.Code.Equals("align", StringComparison.OrdinalIgnoreCase))
					{
						throw new CodeEE("<p>タグの属性名" + identifierWord.Code + "は解釈できません");
					}
					switch (Unescape(literalStringWord4.Str).ToLower())
					{
					case "left":
						state.Alignment = DisplayLineAlignment.LEFT;
						break;
					case "center":
						state.Alignment = DisplayLineAlignment.CENTER;
						break;
					case "right":
						state.Alignment = DisplayLineAlignment.RIGHT;
						break;
					default:
						throw new CodeEE("属性値" + literalStringWord4.Str + "は解釈できません");
					}
					state.FlagP = true;
					return null;
				}
				break;
			}
			case "img":
			{
				if (wordCollection == null)
				{
					throw new CodeEE("<" + text + ">タグに属性が設定されていません");
				}
				string text3 = null;
				string text4 = null;
				string text5 = null;
				int result = 0;
				int result2 = 0;
				int result3 = 0;
				while (true)
				{
					if (wordCollection != null && !wordCollection.EOL)
					{
						IdentifierWord identifierWord = wordCollection.Current as IdentifierWord;
						wordCollection.ShiftNext();
						OperatorWord operatorWord2 = wordCollection.Current as OperatorWord;
						wordCollection.ShiftNext();
						LiteralStringWord literalStringWord2 = wordCollection.Current as LiteralStringWord;
						wordCollection.ShiftNext();
						if (identifierWord == null || operatorWord2 == null || operatorWord2.Code != OperatorCode.Assignment || literalStringWord2 == null)
						{
							break;
						}
						text3 = Unescape(literalStringWord2.Str);
						if (identifierWord.Code.Equals("src", StringComparison.OrdinalIgnoreCase))
						{
							if (text4 != null)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							text4 = text3;
							continue;
						}
						if (identifierWord.Code.Equals("srcb", StringComparison.OrdinalIgnoreCase))
						{
							if (text5 != null)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							text5 = text3;
							continue;
						}
						if (identifierWord.Code.Equals("height", StringComparison.OrdinalIgnoreCase))
						{
							if (result != 0)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							if (int.TryParse(text3, out result))
							{
								continue;
							}
							throw new CodeEE("<" + text + ">タグのheight属性の属性値が数値として解釈できません");
						}
						if (identifierWord.Code.Equals("width", StringComparison.OrdinalIgnoreCase))
						{
							if (result2 != 0)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							if (int.TryParse(text3, out result2))
							{
								continue;
							}
							throw new CodeEE("<" + text + ">タグのwidth属性の属性値が数値として解釈できません");
						}
						if (identifierWord.Code.Equals("ypos", StringComparison.OrdinalIgnoreCase))
						{
							if (result3 != 0)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							if (int.TryParse(text3, out result3))
							{
								continue;
							}
							throw new CodeEE("<" + text + ">タグのypos属性の属性値が数値として解釈できません");
						}
						throw new CodeEE("<" + text + ">タグの属性名" + identifierWord.Code + "は解釈できません");
					}
					if (text4 == null)
					{
						throw new CodeEE("<" + text + ">タグにsrc属性が設定されていません");
					}
					return new ConsoleImagePart(text4, text5, result, result2, result3);
				}
				break;
			}
			case "shape":
			{
				if (wordCollection == null)
				{
					throw new CodeEE("<" + text + ">タグに属性が設定されていません");
				}
				int[] array = null;
				string text6 = null;
				int num2 = -1;
				int num3 = -1;
				while (true)
				{
					if (!wordCollection.EOL)
					{
						IdentifierWord identifierWord = wordCollection.Current as IdentifierWord;
						wordCollection.ShiftNext();
						OperatorWord operatorWord3 = wordCollection.Current as OperatorWord;
						wordCollection.ShiftNext();
						LiteralStringWord literalStringWord3 = wordCollection.Current as LiteralStringWord;
						wordCollection.ShiftNext();
						if (identifierWord == null || operatorWord3 == null || operatorWord3.Code != OperatorCode.Assignment || literalStringWord3 == null)
						{
							break;
						}
						string text7 = Unescape(literalStringWord3.Str);
						switch (identifierWord.Code.ToLower())
						{
						case "color":
							if (num2 >= 0)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							num2 = stringToColorInt32(text7);
							break;
						case "bcolor":
							if (num3 >= 0)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							num3 = stringToColorInt32(text7);
							break;
						case "type":
							if (text6 != null)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							text6 = text7;
							break;
						case "param":
						{
							if (array != null)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							string[] array2 = text7.Split(',');
							array = new int[array2.Length];
							for (int i = 0; i < array2.Length; i++)
							{
								if (!int.TryParse(array2[i], out array[i]))
								{
									throw new CodeEE("<" + text + ">タグの" + identifierWord.Code + "属性の属性値が数値として解釈できません");
								}
							}
							break;
						}
						default:
							throw new CodeEE("<" + text + ">タグの属性名" + identifierWord.Code + "は解釈できません");
						}
						continue;
					}
					if (array == null)
					{
						throw new CodeEE("<" + text + ">タグにparam属性が設定されていません");
					}
					if (text6 == null)
					{
						throw new CodeEE("<" + text + ">タグにtype属性が設定されていません");
					}
					Color color = Config.ForeColor;
					Color bcolor = Config.FocusColor;
					if (num2 >= 0)
					{
						color = new Color(num2 >> 16, (num2 >> 8) & 0xFF, num2 & 0xFF);
					}
					if (num3 >= 0)
					{
						bcolor = new Color(num3 >> 16, (num3 >> 8) & 0xFF, num3 & 0xFF);
					}
					return ConsoleShapePart.CreateShape(text6, array, color, bcolor, num2 >= 0);
				}
				break;
			}
			case "button":
			case "nonbutton":
			{
				if (state.CurrentButtonTag != null)
				{
					throw new CodeEE("<button>又は<nonbutton>が入れ子にされています");
				}
				HtmlAnalzeStateButtonTag htmlAnalzeStateButtonTag = new HtmlAnalzeStateButtonTag();
				bool flag = text.ToLower() == "button";
				string text8 = null;
				string text9 = null;
				while (true)
				{
					if (wordCollection != null && !wordCollection.EOL)
					{
						IdentifierWord identifierWord = wordCollection.Current as IdentifierWord;
						wordCollection.ShiftNext();
						OperatorWord operatorWord5 = wordCollection.Current as OperatorWord;
						wordCollection.ShiftNext();
						LiteralStringWord literalStringWord5 = wordCollection.Current as LiteralStringWord;
						wordCollection.ShiftNext();
						if (identifierWord == null || operatorWord5 == null || operatorWord5.Code != OperatorCode.Assignment || literalStringWord5 == null)
						{
							break;
						}
						text8 = Unescape(literalStringWord5.Str);
						if (identifierWord.Code.Equals("value", StringComparison.OrdinalIgnoreCase))
						{
							if (!flag)
							{
								throw new CodeEE("<" + text + ">タグにvalue属性が設定されています");
							}
							if (text9 != null)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							text9 = text8;
							continue;
						}
						if (identifierWord.Code.Equals("title", StringComparison.OrdinalIgnoreCase))
						{
							if (htmlAnalzeStateButtonTag.ButtonTitle != null)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							htmlAnalzeStateButtonTag.ButtonTitle = text8;
							continue;
						}
						if (identifierWord.Code.Equals("pos", StringComparison.OrdinalIgnoreCase))
						{
							int result4 = 0;
							if (htmlAnalzeStateButtonTag.PointXisLocked)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							if (!int.TryParse(text8, out result4))
							{
								throw new CodeEE("<" + text + ">タグのpos属性の属性値が数値として解釈できません");
							}
							htmlAnalzeStateButtonTag.PointX = result4;
							htmlAnalzeStateButtonTag.PointXisLocked = true;
							continue;
						}
						throw new CodeEE("<" + text + ">タグの属性名" + identifierWord.Code + "は解釈できません");
					}
					if (flag)
					{
						int result5 = 0;
						htmlAnalzeStateButtonTag.ButtonIsInteger = int.TryParse(text9, out result5);
						htmlAnalzeStateButtonTag.ButtonValueInt = result5;
						htmlAnalzeStateButtonTag.ButtonValueStr = text9;
					}
					htmlAnalzeStateButtonTag.IsButton = text9 != null;
					htmlAnalzeStateButtonTag.IsButtonTag = flag;
					state.CurrentButtonTag = htmlAnalzeStateButtonTag;
					state.FlagButton = true;
					return null;
				}
				break;
			}
			case "font":
			{
				if (wordCollection == null)
				{
					throw new CodeEE("<" + text + ">タグに属性が設定されていません");
				}
				HtmlAnalzeStateFontTag htmlAnalzeStateFontTag = new HtmlAnalzeStateFontTag();
				while (true)
				{
					if (!wordCollection.EOL)
					{
						IdentifierWord identifierWord = wordCollection.Current as IdentifierWord;
						wordCollection.ShiftNext();
						OperatorWord operatorWord = wordCollection.Current as OperatorWord;
						wordCollection.ShiftNext();
						LiteralStringWord literalStringWord = wordCollection.Current as LiteralStringWord;
						wordCollection.ShiftNext();
						if (identifierWord == null || operatorWord == null || operatorWord.Code != OperatorCode.Assignment || literalStringWord == null)
						{
							break;
						}
						string text2 = Unescape(literalStringWord.Str);
						switch (identifierWord.Code.ToLower())
						{
						case "color":
							if (htmlAnalzeStateFontTag.Color >= 0)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							htmlAnalzeStateFontTag.Color = stringToColorInt32(text2);
							break;
						case "bcolor":
							if (htmlAnalzeStateFontTag.BColor >= 0)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							htmlAnalzeStateFontTag.BColor = stringToColorInt32(text2);
							break;
						case "face":
							if (htmlAnalzeStateFontTag.FontName != null)
							{
								throw new CodeEE("<" + text + ">タグに" + identifierWord.Code + "属性が2度以上指定されています");
							}
							htmlAnalzeStateFontTag.FontName = text2;
							break;
						default:
							throw new CodeEE("<" + text + ">タグの属性名" + identifierWord.Code + "は解釈できません");
						}
						continue;
					}
					if (state.FonttagList.Count > 0)
					{
						HtmlAnalzeStateFontTag htmlAnalzeStateFontTag2 = state.FonttagList[state.FonttagList.Count - 1];
						if (htmlAnalzeStateFontTag.Color < 0)
						{
							htmlAnalzeStateFontTag.Color = htmlAnalzeStateFontTag2.Color;
						}
						if (htmlAnalzeStateFontTag.BColor < 0)
						{
							htmlAnalzeStateFontTag.BColor = htmlAnalzeStateFontTag2.BColor;
						}
						if (htmlAnalzeStateFontTag.FontName == null)
						{
							htmlAnalzeStateFontTag.FontName = htmlAnalzeStateFontTag2.FontName;
						}
					}
					state.FonttagList.Add(htmlAnalzeStateFontTag);
					return null;
				}
				break;
			}
			}
		}
		throw new CodeEE("html文字列\"" + st.RowString + "\"のタグ解析中にエラーが発生しました");
	}

	private static int stringToColorInt32(string str)
	{
		if (str.Length == 0)
		{
			throw new CodeEE("色を表す単語又は#RRGGBB値が必要です");
		}
		int num = 0;
		if (str[0] == '#')
		{
			string text = str.Substring(1);
			try
			{
				num = Convert.ToInt32(text, 16);
				if (num < 0 || num > 16777215)
				{
					throw new CodeEE(text + "は適切な色指定の範囲外です");
				}
			}
			catch
			{
				throw new CodeEE(text + "は数値として解釈できません");
			}
		}
		else
		{
			Color color = Color.ParseColor(str);
			if (color.A == 0)
			{
				if (str.Equals("transparent", StringComparison.OrdinalIgnoreCase))
				{
					throw new CodeEE("無色透明(Transparent)は色として指定できません");
				}
				try
				{
					num = Convert.ToInt32(str, 16);
				}
				catch
				{
					throw new CodeEE("指定された色名\"" + str + "\"は無効な色名です");
				}
				throw new CodeEE("指定された色名\"" + str + "\"は無効な色名です(16進数で色を指定する場合には数値の前に#が必要です)");
			}
			num = color.R * 65536 + color.G * 256 + color.B;
		}
		return num;
	}
}
