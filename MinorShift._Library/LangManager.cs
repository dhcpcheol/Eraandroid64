using System.Text;

namespace MinorShift._Library;

internal static class LangManager
{
	private static Encoding lang;

	public static void setEncode(int code)
	{
        lang = Encoding.UTF8;
    }

	public static int GetStrlenLang(string str)
	{
		return lang.GetByteCount(str);
	}

	public static int GetUFTIndex(string str, int LangIndex)
	{
		if (LangIndex <= 0)
		{
			return 0;
		}
		int strlenLang = GetStrlenLang(str);
		if (LangIndex >= strlenLang)
		{
			return str.Length;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < str.Length; i++)
		{
			num2 += lang.GetByteCount(str[num].ToString());
			num++;
			if (num2 >= LangIndex)
			{
				break;
			}
		}
		return num;
	}

	public static string GetSubStringLang(string str, int startindex, int length)
	{
		int strlenLang = GetStrlenLang(str);
		if (startindex >= strlenLang || length == 0)
		{
			return "";
		}
		if (length < 0 || length > strlenLang)
		{
			length = strlenLang;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		int num2 = 0;
		if (startindex <= 0)
		{
			if (length == strlenLang)
			{
				return str;
			}
		}
		else
		{
			for (int i = 0; i < str.Length; i++)
			{
				num2 += lang.GetByteCount(str[num].ToString());
				num++;
				if (num2 >= startindex)
				{
					break;
				}
			}
			if (num >= str.Length)
			{
				return "";
			}
		}
		num2 = 0;
		do
		{
			stringBuilder.Append(str[num]);
			num2 += lang.GetByteCount(str[num].ToString());
			num++;
		}
		while (num2 < length && num < str.Length);
		return stringBuilder.ToString();
	}
}
