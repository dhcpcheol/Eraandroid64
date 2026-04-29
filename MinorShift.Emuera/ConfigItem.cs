using System;
using System.Collections.Generic;
using System.Drawing;
using Android.Graphics;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera;

internal sealed class ConfigItem<T> : AConfigItem
{
	private T val;

	public T Value
	{
		get
		{
			return val;
		}
		set
		{
			if (!Fixed)
			{
				val = value;
			}
		}
	}

	public ConfigItem(ConfigCode code, string text, T t)
		: base(code, text)
	{
		val = t;
	}

	public override void CopyTo(AConfigItem other)
	{
		ConfigItem<T> obj = (ConfigItem<T>)other;
		obj.Fixed = false;
		obj.Value = Value;
		obj.Fixed = Fixed;
	}

	public override void SetValue<U>(U p)
	{
		((ConfigItem<U>)(object)this).Value = p;
	}

	public override U GetValue<U>()
	{
		return ((ConfigItem<U>)(object)this).Value;
	}

	public override string ValueToString()
	{
		if (this is ConfigItem<bool>)
		{
			if (((ConfigItem<bool>)(object)this).Value)
			{
				return "YES";
			}
			return "NO";
		}
		if (this is ConfigItem<System.Drawing.Color> || this is ConfigItem<Android.Graphics.Color>)
		{
			System.Drawing.Color value = ((ConfigItem<System.Drawing.Color>)(object)this).Value;
			return $"{value.R},{value.G},{value.B}";
		}
		return val.ToString();
	}

	public override string ToString()
	{
		return Text + ":" + ValueToString();
	}

	public override bool TryParse(string param)
	{
		bool flag = false;
		if (param == null || param.Length == 0)
		{
			return false;
		}
		if (Fixed)
		{
			return false;
		}
		string text = param.Trim();
		if (this is ConfigItem<bool>)
		{
			bool p = false;
			flag = tryStringToBool(text, ref p);
			if (flag)
			{
				((ConfigItem<bool>)(object)this).Value = p;
			}
		}
		else if (this is ConfigItem<System.Drawing.Color>)
		{
			flag = tryStringsToColor(text, out System.Drawing.Color c);
			if (!flag)
			{
				throw new CodeEE("値をColor指定子として認識できません");
			}
			((ConfigItem<System.Drawing.Color>)(object)this).Value = c;
		}
		else if (this is ConfigItem<Android.Graphics.Color>)
		{
			flag = tryStringsToColor(text, out Android.Graphics.Color c2);
			if (!flag)
			{
				throw new CodeEE("値をColor指定子として認識できません");
			}
			((ConfigItem<Android.Graphics.Color>)(object)this).Value = c2;
		}
		else if (this is ConfigItem<char>)
		{
			flag = char.TryParse(text, out var result);
			if (flag)
			{
				((ConfigItem<char>)(object)this).Value = result;
			}
		}
		else if (this is ConfigItem<int>)
		{
			flag = int.TryParse(text, out var result2);
			if (!flag)
			{
				throw new CodeEE("数字でない文字が含まれています");
			}
			((ConfigItem<int>)(object)this).Value = result2;
		}
		else if (this is ConfigItem<long>)
		{
			flag = long.TryParse(text, out var result3);
			if (!flag)
			{
				throw new CodeEE("数字でない文字が含まれています");
			}
			((ConfigItem<long>)(object)this).Value = result3;
		}
		else if (this is ConfigItem<List<long>>)
		{
			((ConfigItem<List<long>>)(object)this).Value.Clear();
			string[] array = text.Split('/');
			for (int i = 0; i < array.Length; i++)
			{
				flag = long.TryParse(array[i].Trim(), out var result4);
				if (flag)
				{
					((ConfigItem<List<long>>)(object)this).Value.Add(result4);
					continue;
				}
				throw new CodeEE("数字でない文字が含まれています");
			}
		}
		else if (this is ConfigItem<string>)
		{
			flag = true;
			((ConfigItem<string>)(object)this).Value = text;
		}
		else if (this is ConfigItem<List<string>>)
		{
			flag = true;
			((ConfigItem<List<string>>)(object)this).Value.Add(text);
		}
		else if (this is ConfigItem<TextDrawingMode>)
		{
			text = text.ToUpper();
			flag = Enum.IsDefined(typeof(TextDrawingMode), text);
			if (!flag)
			{
				throw new CodeEE("不正な指定です");
			}
			((ConfigItem<TextDrawingMode>)(object)this).Value = (TextDrawingMode)Enum.Parse(typeof(TextDrawingMode), text);
		}
		else if (this is ConfigItem<ReduceArgumentOnLoadFlag>)
		{
			text = text.ToUpper();
			flag = Enum.IsDefined(typeof(ReduceArgumentOnLoadFlag), text);
			if (!flag)
			{
				throw new CodeEE("不正な指定です");
			}
			((ConfigItem<ReduceArgumentOnLoadFlag>)(object)this).Value = (ReduceArgumentOnLoadFlag)Enum.Parse(typeof(ReduceArgumentOnLoadFlag), text);
		}
		else if (this is ConfigItem<DisplayWarningFlag>)
		{
			text = text.ToUpper();
			flag = Enum.IsDefined(typeof(DisplayWarningFlag), text);
			if (!flag)
			{
				throw new CodeEE("不正な指定です");
			}
			((ConfigItem<DisplayWarningFlag>)(object)this).Value = (DisplayWarningFlag)Enum.Parse(typeof(DisplayWarningFlag), text);
		}
		else if (this is ConfigItem<UseLanguage>)
		{
			text = text.ToUpper();
			flag = Enum.IsDefined(typeof(UseLanguage), text);
			if (!flag)
			{
				throw new CodeEE("不正な指定です");
			}
			((ConfigItem<UseLanguage>)(object)this).Value = (UseLanguage)Enum.Parse(typeof(UseLanguage), text);
		}
		else if (this is ConfigItem<TextEditorType>)
		{
			text = text.ToUpper();
			flag = Enum.IsDefined(typeof(TextEditorType), text);
			if (!flag)
			{
				throw new CodeEE("不正な指定です");
			}
			((ConfigItem<TextEditorType>)(object)this).Value = (TextEditorType)Enum.Parse(typeof(TextEditorType), text);
		}
		return flag;
	}

	private bool tryStringToBool(string arg, ref bool p)
	{
		if (arg == null)
		{
			return false;
		}
		string text = arg.Trim();
		int result = 0;
		if (int.TryParse(text, out result))
		{
			p = result != 0;
			return true;
		}
		if (text.Equals("NO", StringComparison.CurrentCultureIgnoreCase) || text.Equals("FALSE", StringComparison.CurrentCultureIgnoreCase) || text.Equals("後", StringComparison.CurrentCultureIgnoreCase))
		{
			p = false;
			return true;
		}
		if (text.Equals("YES", StringComparison.CurrentCultureIgnoreCase) || text.Equals("TRUE", StringComparison.CurrentCultureIgnoreCase) || text.Equals("前", StringComparison.CurrentCultureIgnoreCase))
		{
			p = true;
			return true;
		}
		throw new CodeEE("不正な指定です");
	}

	private bool tryStringsToColor(string str, out System.Drawing.Color c)
	{
		string[] array = str.Split(',');
		c = System.Drawing.Color.Black;
		if (array.Length < 3)
		{
			return false;
		}
		if (!int.TryParse(array[0].Trim(), out var result) || result < 0 || result > 255)
		{
			return false;
		}
		if (!int.TryParse(array[1].Trim(), out var result2) || result2 < 0 || result2 > 255)
		{
			return false;
		}
		if (!int.TryParse(array[2].Trim(), out var result3) || result3 < 0 || result3 > 255)
		{
			return false;
		}
		c = System.Drawing.Color.FromArgb(result, result2, result3);
		return true;
	}

	private bool tryStringsToColor(string str, out Android.Graphics.Color c)
	{
		c = Android.Graphics.Color.Black;
		if (tryStringsToColor(str, out System.Drawing.Color c2))
		{
			c = new Android.Graphics.Color(c2.R, c2.G, c2.B, c2.A);
			return true;
		}
		return false;
	}
}
