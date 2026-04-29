using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameView;

internal static class ButtonStringCreator
{
	private delegate void VoidMethod();

	private static readonly Regex numReg = new Regex("\\[\\s*([0][xXbB])?[+-]?[0-9]+([eEpP][0-9]+)?\\s*\\]");

	public static List<string> Split(string printBuffer)
	{
		List<ButtonPrimitive> list = syn(printBuffer);
		List<string> list2 = new List<string>();
		foreach (ButtonPrimitive item in list)
		{
			list2.Add(item.Str);
		}
		return list2;
	}

	public static List<ButtonPrimitive> SplitButton(string printBuffer)
	{
		return syn(printBuffer);
	}

	private static List<ButtonPrimitive> syn(string printBuffer)
	{
		string text = printBuffer.ToString();
		List<ButtonPrimitive> ret = new List<ButtonPrimitive>();
		if (text.Length != 0)
		{
			List<string> list = null;
			if (text.Contains("[") && text.Contains("]"))
			{
				list = lex(new StringStream(text));
				if (list != null)
				{
					bool flag = false;
					bool flag2 = false;
					int num = 0;
					long input = 0L;
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].Length == 0 || LexicalAnalyzer.IsWhiteSpace(list[i][0]))
						{
							continue;
						}
						if (isButtonCore(list[i], ref input))
						{
							num++;
							flag2 = false;
							continue;
						}
						flag2 = true;
						if (num == 0)
						{
							flag = true;
						}
					}
					if (num <= 1)
					{
						ButtonPrimitive buttonPrimitive = new ButtonPrimitive();
						buttonPrimitive.Str = printBuffer.ToString();
						buttonPrimitive.CanSelect = num >= 1;
						buttonPrimitive.Input = input;
						ret.Add(buttonPrimitive);
						return ret;
					}
					num = 0;
					bool flag3 = !flag && flag2;
					bool flag4 = flag && !flag2;
					bool flag5 = !flag3 && !flag4;
					bool canSelect = false;
					long input2 = 0L;
					int num2 = 0;
					StringBuilder buffer = new StringBuilder();
					VoidMethod voidMethod = delegate
					{
						if (buffer.Length != 0)
						{
							ButtonPrimitive item = new ButtonPrimitive
							{
								Str = buffer.ToString(),
								CanSelect = canSelect,
								Input = input2
							};
							ret.Add(item);
							buffer.Remove(0, buffer.Length);
							canSelect = false;
							input2 = 0L;
						}
					};
					for (int num3 = 0; num3 < list.Count; num3++)
					{
						if (list[num3].Length == 0)
						{
							continue;
						}
						if (LexicalAnalyzer.IsWhiteSpace(list[num3][0]))
						{
							if ((num2 & 3) == 3 && flag5 && list[num3].Length >= 2)
							{
								voidMethod();
								buffer.Append(list[num3]);
								num2 = 0;
							}
							else
							{
								buffer.Append(list[num3]);
							}
						}
						else if (isButtonCore(list[num3], ref input))
						{
							num++;
							if ((num2 & 1) == 1 || flag3)
							{
								voidMethod();
								buffer.Append(list[num3]);
								input2 = input;
								canSelect = true;
								num2 = 1;
							}
							else if (flag4)
							{
								buffer.Append(list[num3]);
								input2 = input;
								canSelect = true;
								voidMethod();
								num2 = 0;
							}
							else
							{
								buffer.Append(list[num3]);
								input2 = input;
								canSelect = true;
								num2 = 1;
							}
						}
						else
						{
							buffer.Append(list[num3]);
							num2 |= 2;
						}
					}
					voidMethod();
					return ret;
				}
			}
		}
		ret = new List<ButtonPrimitive>();
		ButtonPrimitive buttonPrimitive2 = new ButtonPrimitive();
		buttonPrimitive2.Str = text;
		ret.Add(buttonPrimitive2);
		return ret;
	}

	private static bool isNumericWord(string str)
	{
		return numReg.IsMatch(str);
	}

	private static bool isButtonCore(string str, ref long input)
	{
		if (str == null || str.Length < 3 || str[0] != '[' || str[str.Length - 1] != ']')
		{
			return false;
		}
		if (!isNumericWord(str))
		{
			return false;
		}
		StringStream st = new StringStream(str.Substring(1, str.Length - 2));
		LexicalAnalyzer.SkipAllSpace(st);
		try
		{
			input = LexicalAnalyzer.ReadInt64(st, retZero: false);
		}
		catch
		{
			return false;
		}
		return true;
	}

	private static List<string> lex(StringStream st)
	{
		List<string> strs = new List<string>();
		int num = 0;
		int startIndex = 0;
		VoidMethod voidMethod = delegate
		{
			if (st.CurrentPosition != startIndex)
			{
				int length = st.CurrentPosition - startIndex;
				strs.Add(st.Substring(startIndex, length));
				startIndex = st.CurrentPosition;
			}
		};
		while (true)
		{
			if (!st.EOS)
			{
				if (st.Current == '[')
				{
					if (num == 1)
					{
						break;
					}
					voidMethod();
					num = 1;
					st.ShiftNext();
				}
				else if (st.Current == ']')
				{
					if (num != 1)
					{
						break;
					}
					st.ShiftNext();
					voidMethod();
					num = 0;
				}
				else if (num == 0 && LexicalAnalyzer.IsWhiteSpace(st.Current))
				{
					voidMethod();
					LexicalAnalyzer.SkipAllSpace(st);
					voidMethod();
				}
				else
				{
					st.ShiftNext();
				}
				continue;
			}
			voidMethod();
			return strs;
		}
		return null;
	}
}
