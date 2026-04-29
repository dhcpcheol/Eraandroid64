using System;
using System.IO;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera;

internal static class KeyMacro
{
	private static readonly string macroPath;

	public const string gID = "グループ";

	public const int MaxGroup = 10;

	public const int MaxFkey = 12;

	public const int MaxMacro = 120;

	private static string[] macro;

	private static string[] macroName;

	private static string[] groupName;

	private static bool isMacroChanged;

	static KeyMacro()
	{
		macroPath = Program.ExeDir + "macro.txt";
		macro = new string[120];
		macroName = new string[120];
		groupName = new string[10];
		isMacroChanged = false;
		for (int i = 0; i < 10; i++)
		{
			groupName[i] = "マクログループ" + i + "に設定";
			for (int j = 0; j < 12; j++)
			{
				int num = j + i * 12;
				macro[num] = "";
				if (i == 0)
				{
					macroName[num] = "マクロキーF" + (j + 1) + ":";
					continue;
				}
				macroName[num] = "G" + i + ":マクロキーF" + (j + 1) + ":";
			}
		}
	}

	public static bool SaveMacro()
	{
		if (!isMacroChanged)
		{
			return true;
		}
		StreamWriter streamWriter = null;
		try
		{
			streamWriter = new StreamWriter(macroPath, append: false, Config.Encode);
			for (int i = 0; i < 10; i++)
			{
				streamWriter.WriteLine("グループ" + i + ":" + groupName[i]);
			}
			for (int j = 0; j < 120; j++)
			{
				streamWriter.WriteLine(macroName[j] + macro[j]);
			}
		}
		catch (Exception)
		{
			return false;
		}
		finally
		{
			streamWriter?.Close();
		}
		return true;
	}

	public static void LoadMacroFile(string filename)
	{
		EraStreamReader eraStreamReader = new EraStreamReader(useRename: false);
		if (!eraStreamReader.Open(filename))
		{
			return;
		}
		try
		{
			string text = null;
			while ((text = eraStreamReader.ReadLine()) != null)
			{
				if (text.Length == 0 || text[0] == ';')
				{
					continue;
				}
				if (text.StartsWith("グループ"))
				{
					if (text.Length < "グループ".Length + 4)
					{
						continue;
					}
					int num = text["グループ".Length] - 48;
					if (num < 0 || num > 9 || text["グループ".Length + 1] != ':')
					{
						continue;
					}
					groupName[num] = text.Substring("グループ".Length + 2);
				}
				for (int i = 0; i < 120; i++)
				{
					if (text.StartsWith(macroName[i]))
					{
						macro[i] = text.Substring(macroName[i].Length);
						break;
					}
				}
			}
		}
		catch
		{
		}
		finally
		{
			eraStreamReader.Dispose();
		}
	}

	public static void SetMacro(int FkeyNum, int groupNum, string macroStr)
	{
		isMacroChanged = true;
		macro[FkeyNum + groupNum * 12] = macroStr;
	}

	public static string GetMacro(int FkeyNum, int groupNum)
	{
		return macro[FkeyNum + groupNum * 12];
	}

	public static string GetGroupName(int groupNum)
	{
		return groupName[groupNum];
	}
}
