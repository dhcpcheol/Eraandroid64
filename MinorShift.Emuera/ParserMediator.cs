using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MinorShift.Emuera.GameProc;
using MinorShift.Emuera.GameView;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera;

internal static class ParserMediator
{
	private class ParserWarning
	{
		public string WarningMes;

		public ScriptPosition WarningPos;

		public int WarningLevel;

		public string StackTrace;

		public ParserWarning(string mes, ScriptPosition pos, int level, string stackTrace)
		{
			WarningMes = mes;
			WarningPos = pos;
			WarningLevel = level;
			StackTrace = stackTrace;
		}
	}

	private static EmueraConsole console;

	private static List<ParserWarning> warningList = new List<ParserWarning>();

	public static Dictionary<string, string> RenameDic { get; private set; }

	public static bool HasWarning => warningList.Count > 0;

	public static void ConfigWarn(string str, ScriptPosition pos, int level, string stack)
	{
		if (level >= Config.DisplayWarningLevel || Program.AnalysisMode)
		{
			warningList.Add(new ParserWarning(str, pos, level, stack));
		}
	}

	public static void Initialize(EmueraConsole console)
	{
		ParserMediator.console = console;
	}

	public static void LoadEraExRenameFile(string filepath)
	{
		if (RenameDic != null)
		{
			RenameDic.Clear();
		}
		RenameDic = new Dictionary<string, string>();
		EraStreamReader eraStreamReader = new EraStreamReader(useRename: false);
		if (!File.Exists(filepath) || !eraStreamReader.Open(filepath))
		{
			return;
		}
		string text = null;
		ScriptPosition scriptPosition = null;
		Regex regex = new Regex("\\\\,", RegexOptions.Compiled);
		try
		{
			while ((text = eraStreamReader.ReadLine()) != null)
			{
				if (text.Length != 0 && !text.StartsWith(";"))
				{
					string[] array = regex.Split(text);
					if (array[array.Length - 1].Contains(","))
					{
						string[] array2 = array[array.Length - 1].Split(',');
						array[array.Length - 1] = array2[0];
						string[] array3 = new string[2]
						{
							string.Join(",", array),
							array2[1]
						};
						scriptPosition = new ScriptPosition(eraStreamReader.Filename, eraStreamReader.LineNo, text);
						string value = array3[0].Trim();
						string key = $"[[{array3[1].Trim()}]]";
						RenameDic[key] = value;
						scriptPosition = null;
					}
				}
			}
		}
		catch (Exception ex)
		{
			if (scriptPosition != null)
			{
				throw new CodeEE(ex.Message, scriptPosition);
			}
			throw new CodeEE(ex.Message);
		}
		finally
		{
			eraStreamReader.Close();
		}
	}

	public static void Warn(string str, ScriptPosition pos, int level)
	{
		Warn(str, pos, level, null);
	}

	public static void Warn(string str, ScriptPosition pos, int level, string stack)
	{
		if ((level >= Config.DisplayWarningLevel || Program.AnalysisMode) && console != null && !console.RunERBFromMemory)
		{
			warningList.Add(new ParserWarning(str, pos, level, stack));
		}
	}

	public static void Warn(string str, LogicalLine line, int level, bool isError, bool isBackComp)
	{
		Warn(str, line, level, isError, isBackComp, null);
	}

	public static void Warn(string str, LogicalLine line, int level, bool isError, bool isBackComp, string stack)
	{
		if (isError)
		{
			line.IsError = true;
			line.ErrMes = str;
		}
		if ((level >= Config.DisplayWarningLevel || Program.AnalysisMode) && (!isBackComp || Config.WarnBackCompatibility) && console != null && !console.RunERBFromMemory)
		{
			warningList.Add(new ParserWarning(str, line.Position, level, stack));
		}
	}

	public static void ClearWarningList()
	{
		warningList.Clear();
	}

	public static void FlushWarningList()
	{
		for (int i = 0; i < warningList.Count; i++)
		{
			ParserWarning parserWarning = warningList[i];
			console.PrintWarning(parserWarning.WarningMes, parserWarning.WarningPos, parserWarning.WarningLevel);
			if (parserWarning.StackTrace != null)
			{
				string[] array = parserWarning.StackTrace.Split('\n');
				for (int j = 0; j < array.Length; j++)
				{
					console.PrintSystemLine(array[j]);
				}
			}
		}
		warningList.Clear();
	}
}
