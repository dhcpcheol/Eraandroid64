using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Android.App;
using Android.Widget;
using EmueraFramework;
using MinorShift._Library;
using MinorShift.Emuera.GameView;

namespace MinorShift.Emuera;

public static class Program
{
	public const string ConfigFileName = "Config.txt";

	public static bool Reboot;

	public static int RebootClientY;

	public static Point RebootLocation;

	public static bool AnalysisMode;

	public static List<string> AnalysisFiles;

	public static bool debugMode;

	public static string ExeDir { get; private set; }

	public static string CsvDir { get; private set; }

	public static string ErbDir { get; private set; }

	public static string DebugDir { get; private set; }

	public static string DatDir { get; private set; }

	public static string ContentDir { get; private set; }

	public static string ExeName { get; private set; }

	public static bool DebugMode => debugMode;

	public static uint StartTime { get; private set; }

	public static void Main(Activity parent, IFrontEnd frontEnd, string eraPath, params string[] args)
	{
		if (eraPath.Last() != '/')
		{
			eraPath += "/";
		}
		GlobalStatic.FrontEnd = frontEnd;
		ExeDir = eraPath;
		FileLog.Init(ExeDir);
		CsvDir = ExeDir + "csv/";
		ErbDir = ExeDir + "erb/";
		DebugDir = ExeDir + "debug/";
		DatDir = ExeDir + "dat/";
		ContentDir = ExeDir + "resources/";
		ExeName = Path.GetFileNameWithoutExtension(Sys.ExeName);
		ConfigData.Instance.LoadConfig();
		if (!Directory.Exists(CsvDir))
		{
			Toast.MakeText(parent, "csv폴더가 존재하지 않습니다", ToastLength.Long).Show();
			return;
		}
		if (!Directory.Exists(ErbDir))
		{
			Toast.MakeText(parent, "erb폴더가 존재하지 않습니다", ToastLength.Long).Show();
			return;
		}
		int num = 0;
		if (args.Length != 0 && args[0].Equals("-DEBUG", StringComparison.CurrentCultureIgnoreCase))
		{
			num = 1;
			debugMode = true;
		}
		if (debugMode)
		{
			ConfigData.Instance.LoadDebugConfig();
			if (!Directory.Exists(DebugDir))
			{
				try
				{
					Directory.CreateDirectory(DebugDir);
				}
				catch
				{
					Toast.MakeText(parent, "debugフォルダの作成に失敗しました", ToastLength.Long).Show();
					return;
				}
			}
		}
		if (args.Length > num)
		{
			AnalysisFiles = new List<string>();
			for (int i = num; i < args.Length; i++)
			{
				if (!File.Exists(args[i]) && !Directory.Exists(args[i]))
				{
					return;
				}
				if ((File.GetAttributes(args[i]) & FileAttributes.Directory) == FileAttributes.Directory)
				{
					List<KeyValuePair<string, string>> files = Config.GetFiles(args[i] + "/", "*.ERB");
					for (int j = 0; j < files.Count; j++)
					{
						AnalysisFiles.Add(files[j].Value);
					}
				}
				else
				{
					if (Path.GetExtension(args[i]).ToUpper() != ".ERB")
					{
						return;
					}
					AnalysisFiles.Add(args[i]);
				}
			}
			AnalysisMode = true;
		}
		StartTime = WinmmTimer.TickCount;
		GlobalStatic.Console = new EmueraConsole(parent);
	}
}
