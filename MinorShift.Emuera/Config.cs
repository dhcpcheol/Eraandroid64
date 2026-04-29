using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.Graphics;
using MinorShift._Library;

namespace MinorShift.Emuera;

public static class Config
{
	private sealed class StrIgnoreCaseComparer : IComparer<string>
	{
		public int Compare(string x, string y)
		{
			return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
		}
	}

	public static Encoding Encode = Encoding.UTF8;

    public static Encoding SaveEncode = Encoding.UTF8;
    private static Dictionary<ConfigCode, string> nameDic = null;

	private static StrIgnoreCaseComparer ignoreCaseComparer = new StrIgnoreCaseComparer();

	public const StringComparison SCIgnoreCase = StringComparison.OrdinalIgnoreCase;

	public const StringComparison SCExpression = StringComparison.Ordinal;

	private static bool IgnoreCase { get; set; }

	private static bool CompatiFunctionNoignoreCase { get; set; }

	public static bool ICFunction { get; private set; }

	public static bool ICVariable { get; private set; }

	public static StringComparison SCFunction { get; private set; }

	public static StringComparison SCVariable { get; private set; }

	public static int DrawingParam_ShapePositionShift { get; private set; }

	public static bool UseRenameFile { get; private set; }

	public static bool UseReplaceFile { get; private set; }

	public static bool UseMouse { get; private set; }

	public static bool UseMenu { get; private set; }

	public static bool UseDebugCommand { get; private set; }

	public static bool AllowMultipleInstances { get; private set; }

	public static bool AutoSave { get; private set; }

	public static bool UseKeyMacro { get; private set; }

	public static bool SizableWindow { get; private set; }

	internal static TextDrawingMode TextDrawingMode => TextDrawingMode.GRAPHICS;

	public static int WindowX => GlobalStatic.FrontEnd.Width;

	public static int DrawableWidth { get; private set; }

	public static int WindowPosX { get; private set; }

	public static int WindowPosY { get; private set; }

	public static bool SetWindowPos { get; private set; }

	public static int MaxLog { get; private set; }

	public static int PrintCPerLine { get; private set; }

	public static int PrintCLength { get; private set; }

	public static Color ForeColor { get; private set; }

	public static Color BackColor { get; private set; }

	public static Color FocusColor { get; private set; }

	public static Color LogColor { get; private set; }

	public static int FontSize { get; set; }

	public static string FontName { get; private set; }

	public static int LineHeight { get; set; }

	public static int FPS { get; private set; }

	public static int ScrollHeight { get; private set; }

	public static int InfiniteLoopAlertTime { get; private set; }

	public static int SaveDataNos { get; private set; }

	public static bool WarnBackCompatibility { get; private set; }

	public static bool WindowMaximixed { get; private set; }

	public static bool WarnNormalFunctionOverloading { get; private set; }

	public static bool SearchSubdirectory { get; private set; }

	public static bool SortWithFilename { get; private set; }

	public static bool AllowFunctionOverloading { get; private set; }

	public static bool WarnFunctionOverloading { get; private set; }

	public static int DisplayWarningLevel { get; private set; }

	public static bool DisplayReport { get; private set; }

	internal static ReduceArgumentOnLoadFlag ReduceArgumentOnLoad { get; private set; }

	public static bool IgnoreUncalledFunction { get; private set; }

	internal static DisplayWarningFlag FunctionNotFoundWarning { get; private set; }

	internal static DisplayWarningFlag FunctionNotCalledWarning { get; private set; }

	public static bool ChangeMasterNameIfDebug { get; private set; }

	public static long LastKey { get; private set; }

	public static bool ButtonWrap { get; private set; }

	public static string TextEditor { get; private set; }

	internal static TextEditorType EditorType { get; private set; }

	public static string EditorArg { get; private set; }

	public static bool CompatiErrorLine { get; private set; }

	public static bool CompatiCALLNAME { get; private set; }

	public static bool UseSaveFolder { get; private set; }

	public static bool CompatiRAND { get; private set; }

	public static bool CompatiLinefeedAs1739 { get; private set; }

	public static bool SystemAllowFullSpace { get; private set; }

	public static bool SystemSaveInUTF8 { get; private set; }

	public static bool SystemSaveInBinary { get; private set; }

	public static bool CompatiFuncArgAutoConvert { get; private set; }

	public static bool CompatiFuncArgOptional { get; private set; }

	public static bool CompatiCallEvent { get; private set; }

	public static bool CompatiSPChara { get; private set; }

	public static bool SystemIgnoreTripleSymbol { get; private set; }

	public static bool SystemNoTarget { get; private set; }

	public static int Language { get; private set; }

	public static string SavDir { get; private set; }

	public static bool NeedReduceArgumentOnLoad { get; private set; }

	public static bool AllowLongInputByMouse { get; private set; }

	public static bool TimesNotRigorousCalculation { get; private set; }

	public static bool DebugShowWindow { get; private set; }

	public static bool DebugWindowTopMost { get; private set; }

	public static int DebugWindowWidth { get; private set; }

	public static int DebugWindowHeight { get; private set; }

	public static bool DebugSetWindowPos { get; private set; }

	public static int DebugWindowPosX { get; private set; }

	public static int DebugWindowPosY { get; private set; }

	public static string MoneyLabel { get; private set; }

	public static bool MoneyFirst { get; private set; }

	public static string LoadLabel { get; private set; }

	public static int MaxShopItem { get; private set; }

	public static string DrawLineString { get; private set; }

	public static char BarChar1 { get; private set; }

	public static char BarChar2 { get; private set; }

	public static string TitleMenuString0 { get; private set; }

	public static string TitleMenuString1 { get; private set; }

	public static int ComAbleDefault { get; private set; }

	public static List<long> StainDefault { get; private set; }

	public static string TimeupLabel { get; private set; }

	public static List<long> ExpLvDef { get; private set; }

	public static List<long> PalamLvDef { get; private set; }

	public static long PbandDef { get; private set; }

	public static long RelationDef { get; private set; }

	internal static string GetConfigName(ConfigCode code)
	{
		return nameDic[code];
	}

	internal static void SetConfig(ConfigData instance)
	{
		nameDic = instance.GetConfigNameDic();
		IgnoreCase = instance.GetConfigValue<bool>(ConfigCode.IgnoreCase);
		CompatiFunctionNoignoreCase = instance.GetConfigValue<bool>(ConfigCode.CompatiFunctionNoignoreCase);
		ICFunction = IgnoreCase && !CompatiFunctionNoignoreCase;
		ICVariable = IgnoreCase;
		if (IgnoreCase)
		{
			if (CompatiFunctionNoignoreCase)
			{
				SCFunction = StringComparison.Ordinal;
			}
			else
			{
				SCFunction = StringComparison.OrdinalIgnoreCase;
			}
			SCVariable = StringComparison.OrdinalIgnoreCase;
		}
		else
		{
			SCFunction = StringComparison.Ordinal;
			SCVariable = StringComparison.Ordinal;
		}
		UseRenameFile = instance.GetConfigValue<bool>(ConfigCode.UseRenameFile);
		UseReplaceFile = instance.GetConfigValue<bool>(ConfigCode.UseReplaceFile);
		UseMouse = instance.GetConfigValue<bool>(ConfigCode.UseMouse);
		UseMenu = instance.GetConfigValue<bool>(ConfigCode.UseMenu);
		UseDebugCommand = instance.GetConfigValue<bool>(ConfigCode.UseDebugCommand);
		AllowMultipleInstances = instance.GetConfigValue<bool>(ConfigCode.AllowMultipleInstances);
		AutoSave = instance.GetConfigValue<bool>(ConfigCode.AutoSave);
		UseKeyMacro = instance.GetConfigValue<bool>(ConfigCode.UseKeyMacro);
		SizableWindow = instance.GetConfigValue<bool>(ConfigCode.SizableWindow);
		WindowPosX = 0;
		WindowPosY = 0;
		SetWindowPos = instance.GetConfigValue<bool>(ConfigCode.SetWindowPos);
		MaxLog = instance.GetConfigValue<int>(ConfigCode.MaxLog);
		PrintCPerLine = instance.GetConfigValue<int>(ConfigCode.PrintCPerLine);
		PrintCLength = instance.GetConfigValue<int>(ConfigCode.PrintCLength);
		ForeColor = instance.GetConfigValue<Color>(ConfigCode.ForeColor);
		BackColor = instance.GetConfigValue<Color>(ConfigCode.BackColor);
		FocusColor = instance.GetConfigValue<Color>(ConfigCode.FocusColor);
		LogColor = instance.GetConfigValue<Color>(ConfigCode.LogColor);
		FontName = instance.GetConfigValue<string>(ConfigCode.FontName);
		FPS = instance.GetConfigValue<int>(ConfigCode.FPS);
		ScrollHeight = instance.GetConfigValue<int>(ConfigCode.ScrollHeight);
		InfiniteLoopAlertTime = instance.GetConfigValue<int>(ConfigCode.InfiniteLoopAlertTime);
		SaveDataNos = instance.GetConfigValue<int>(ConfigCode.SaveDataNos);
		WarnBackCompatibility = instance.GetConfigValue<bool>(ConfigCode.WarnBackCompatibility);
		WindowMaximixed = instance.GetConfigValue<bool>(ConfigCode.WindowMaximixed);
		WarnNormalFunctionOverloading = instance.GetConfigValue<bool>(ConfigCode.WarnNormalFunctionOverloading);
		SearchSubdirectory = instance.GetConfigValue<bool>(ConfigCode.SearchSubdirectory);
		SortWithFilename = instance.GetConfigValue<bool>(ConfigCode.SortWithFilename);
		AllowFunctionOverloading = instance.GetConfigValue<bool>(ConfigCode.AllowFunctionOverloading);
		if (!AllowFunctionOverloading)
		{
			WarnFunctionOverloading = true;
		}
		else
		{
			WarnFunctionOverloading = instance.GetConfigValue<bool>(ConfigCode.WarnFunctionOverloading);
		}
		DisplayWarningLevel = instance.GetConfigValue<int>(ConfigCode.DisplayWarningLevel);
		DisplayReport = instance.GetConfigValue<bool>(ConfigCode.DisplayReport);
		ReduceArgumentOnLoad = instance.GetConfigValue<ReduceArgumentOnLoadFlag>(ConfigCode.ReduceArgumentOnLoad);
		IgnoreUncalledFunction = instance.GetConfigValue<bool>(ConfigCode.IgnoreUncalledFunction);
		FunctionNotFoundWarning = instance.GetConfigValue<DisplayWarningFlag>(ConfigCode.FunctionNotFoundWarning);
		FunctionNotCalledWarning = instance.GetConfigValue<DisplayWarningFlag>(ConfigCode.FunctionNotCalledWarning);
		ChangeMasterNameIfDebug = instance.GetConfigValue<bool>(ConfigCode.ChangeMasterNameIfDebug);
		LastKey = instance.GetConfigValue<long>(ConfigCode.LastKey);
		ButtonWrap = instance.GetConfigValue<bool>(ConfigCode.ButtonWrap);
		TextEditor = instance.GetConfigValue<string>(ConfigCode.TextEditor);
		EditorType = instance.GetConfigValue<TextEditorType>(ConfigCode.EditorType);
		EditorArg = instance.GetConfigValue<string>(ConfigCode.EditorArgument);
		CompatiErrorLine = instance.GetConfigValue<bool>(ConfigCode.CompatiErrorLine);
		CompatiCALLNAME = instance.GetConfigValue<bool>(ConfigCode.CompatiCALLNAME);
		UseSaveFolder = instance.GetConfigValue<bool>(ConfigCode.UseSaveFolder);
		CompatiRAND = instance.GetConfigValue<bool>(ConfigCode.CompatiRAND);
		CompatiLinefeedAs1739 = instance.GetConfigValue<bool>(ConfigCode.CompatiLinefeedAs1739);
		SystemAllowFullSpace = instance.GetConfigValue<bool>(ConfigCode.SystemAllowFullSpace);
		SystemSaveInUTF8 = instance.GetConfigValue<bool>(ConfigCode.SystemSaveInUTF8);
		if (SystemSaveInUTF8)
		{
			SaveEncode = Encoding.GetEncoding("UTF-8");
		}
		SystemSaveInBinary = instance.GetConfigValue<bool>(ConfigCode.SystemSaveInBinary);
		SystemIgnoreTripleSymbol = instance.GetConfigValue<bool>(ConfigCode.SystemIgnoreTripleSymbol);
		CompatiFuncArgAutoConvert = instance.GetConfigValue<bool>(ConfigCode.CompatiFuncArgAutoConvert);
		CompatiFuncArgOptional = instance.GetConfigValue<bool>(ConfigCode.CompatiFuncArgOptional);
		CompatiCallEvent = instance.GetConfigValue<bool>(ConfigCode.CompatiCallEvent);
		CompatiSPChara = instance.GetConfigValue<bool>(ConfigCode.CompatiSPChara);
		AllowLongInputByMouse = instance.GetConfigValue<bool>(ConfigCode.AllowLongInputByMouse);
		TimesNotRigorousCalculation = instance.GetConfigValue<bool>(ConfigCode.TimesNotRigorousCalculation);
		SystemNoTarget = instance.GetConfigValue<bool>(ConfigCode.SystemNoTarget);
		switch (instance.GetConfigValue<UseLanguage>(ConfigCode.useLanguage))
		{
		case UseLanguage.JAPANESE:
			Language = 1041;
			LangManager.setEncode(932);
			break;
		case UseLanguage.KOREAN:
			Language = 1042;
			LangManager.setEncode(949);
			break;
		case UseLanguage.CHINESE_HANS:
			Language = 2052;
			LangManager.setEncode(936);
			break;
		case UseLanguage.CHINESE_HANT:
			Language = 1028;
			LangManager.setEncode(950);
			break;
		}
		if (FontSize < 8)
		{
			FontSize = 8;
		}
		if (LineHeight < FontSize)
		{
			LineHeight = FontSize;
		}
		if (SaveDataNos < 20)
		{
			SaveDataNos = 20;
		}
		if (SaveDataNos > 80)
		{
			SaveDataNos = 80;
		}
		if (MaxLog < 500)
		{
			MaxLog = 500;
		}
		DrawingParam_ShapePositionShift = 0;
		if (TextDrawingMode != TextDrawingMode.WINAPI)
		{
			DrawingParam_ShapePositionShift = Math.Max(2, FontSize / 6);
		}
		DrawableWidth = WindowX;
		if (UseSaveFolder)
		{
			SavDir = Program.ExeDir + "sav/";
		}
		else
		{
			SavDir = Program.ExeDir;
		}
		if (UseSaveFolder && !Directory.Exists(SavDir))
		{
			createSavDirAndMoveFiles();
		}
	}

	public static void CreateSavDir()
	{
		if (UseSaveFolder && !Directory.Exists(SavDir))
		{
			Directory.CreateDirectory(SavDir);
		}
	}

	private static void createSavDirAndMoveFiles()
	{
		try
		{
			Directory.CreateDirectory(SavDir);
		}
		catch
		{
			return;
		}
		bool num = File.Exists(Program.ExeDir + "global.sav");
		string[] files = Directory.GetFiles(Program.ExeDir, "save*.sav", SearchOption.TopDirectoryOnly);
		if ((!num && files.Length == 0) || !Directory.Exists(SavDir))
		{
			return;
		}
		try
		{
			if (File.Exists(Program.ExeDir + "global.sav"))
			{
				File.Move(Program.ExeDir + "global.sav", SavDir + "global.sav");
			}
			files = Directory.GetFiles(Program.ExeDir, "save*.sav", SearchOption.TopDirectoryOnly);
			string[] array = files;
			foreach (string text in array)
			{
				File.Move(text, SavDir + System.IO.Path.GetFileName(text));
			}
		}
		catch
		{
		}
	}

	public static bool CheckUpdate()
	{
		if (ReduceArgumentOnLoad != ReduceArgumentOnLoadFlag.ONCE)
		{
			if (ReduceArgumentOnLoad == ReduceArgumentOnLoadFlag.YES)
			{
				NeedReduceArgumentOnLoad = true;
			}
			else if (ReduceArgumentOnLoad == ReduceArgumentOnLoadFlag.NO)
			{
				NeedReduceArgumentOnLoad = false;
			}
			return false;
		}
		long updateKey = getUpdateKey();
		bool result = LastKey != updateKey;
		LastKey = updateKey;
		return result;
	}

	private static long getUpdateKey()
	{
		SearchOption searchOption = SearchOption.TopDirectoryOnly;
		if (SearchSubdirectory)
		{
			searchOption = SearchOption.AllDirectories;
		}
		string[] files = Directory.GetFiles(Program.ErbDir, "*.ERB", searchOption);
		string[] files2 = Directory.GetFiles(Program.CsvDir, "*.CSV", searchOption);
		long[] array = new long[files.Length + files2.Length];
		for (int i = 0; i < files.Length; i++)
		{
			if (System.IO.Path.GetExtension(files[i]).Equals(".ERB", StringComparison.OrdinalIgnoreCase))
			{
				array[i] = File.GetLastWriteTime(files[i]).ToBinary();
			}
		}
		for (int j = 0; j < files2.Length; j++)
		{
			if (System.IO.Path.GetExtension(files2[j]).Equals(".CSV", StringComparison.OrdinalIgnoreCase))
			{
				array[j + files.Length] = File.GetLastWriteTime(files2[j]).ToBinary();
			}
		}
		long num = 0L;
		for (int k = 0; k < array.Length; k++)
		{
			num ^= array[k] * 1103515245 + 12345;
		}
		return num;
	}

	public static List<KeyValuePair<string, string>> GetFiles(string rootdir, string pattern)
	{
		return getFiles(rootdir, rootdir, pattern, !SearchSubdirectory, SortWithFilename);
	}

	private static List<KeyValuePair<string, string>> getFiles(string dir, string rootdir, string pattern, bool toponly, bool sort)
	{
		if (!dir.EndsWith("/"))
		{
			dir += "/";
		}
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		DirectoryInfo directoryInfo = new DirectoryInfo(dir);
		FileInfo[] array = null;
		array = (toponly ? (from file in directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly)
			where file.Name.MatchWithWildcard(pattern)
			select file).ToArray() : (from file in directoryInfo.GetFiles("*", SearchOption.AllDirectories)
			where file.Name.MatchWithWildcard(pattern)
			select file).ToArray());
		if (sort)
		{
			list.AddRange(from file in array
				orderby file.FullName
				select new KeyValuePair<string, string>(file.FullName.Substring(rootdir.Length), file.FullName));
		}
		else
		{
			list.AddRange(array.Select((FileInfo file) => new KeyValuePair<string, string>(file.FullName.Substring(rootdir.Length), file.FullName)));
		}
		return list;
	}

	internal static void SetDebugConfig(ConfigData instance)
	{
		DebugShowWindow = instance.GetConfigValue<bool>(ConfigCode.DebugShowWindow);
		DebugWindowTopMost = instance.GetConfigValue<bool>(ConfigCode.DebugWindowTopMost);
		DebugWindowWidth = instance.GetConfigValue<int>(ConfigCode.DebugWindowWidth);
		DebugWindowHeight = instance.GetConfigValue<int>(ConfigCode.DebugWindowHeight);
		DebugSetWindowPos = instance.GetConfigValue<bool>(ConfigCode.DebugSetWindowPos);
		DebugWindowPosX = instance.GetConfigValue<int>(ConfigCode.DebugWindowPosX);
		DebugWindowPosY = instance.GetConfigValue<int>(ConfigCode.DebugWindowPosY);
	}

	internal static void SetReplace(ConfigData instance)
	{
		MoneyLabel = instance.GetConfigValue<string>(ConfigCode.MoneyLabel);
		MoneyFirst = instance.GetConfigValue<bool>(ConfigCode.MoneyFirst);
		LoadLabel = instance.GetConfigValue<string>(ConfigCode.LoadLabel);
		MaxShopItem = instance.GetConfigValue<int>(ConfigCode.MaxShopItem);
		DrawLineString = instance.GetConfigValue<string>(ConfigCode.DrawLineString);
		if (string.IsNullOrEmpty(DrawLineString))
		{
			DrawLineString = "-";
		}
		BarChar1 = instance.GetConfigValue<char>(ConfigCode.BarChar1);
		BarChar2 = instance.GetConfigValue<char>(ConfigCode.BarChar2);
		TitleMenuString0 = instance.GetConfigValue<string>(ConfigCode.TitleMenuString0);
		TitleMenuString1 = instance.GetConfigValue<string>(ConfigCode.TitleMenuString1);
		ComAbleDefault = instance.GetConfigValue<int>(ConfigCode.ComAbleDefault);
		StainDefault = instance.GetConfigValue<List<long>>(ConfigCode.StainDefault);
		TimeupLabel = instance.GetConfigValue<string>(ConfigCode.TimeupLabel);
		ExpLvDef = instance.GetConfigValue<List<long>>(ConfigCode.ExpLvDef);
		PalamLvDef = instance.GetConfigValue<List<long>>(ConfigCode.PalamLvDef);
		PbandDef = instance.GetConfigValue<long>(ConfigCode.pbandDef);
		RelationDef = instance.GetConfigValue<long>(ConfigCode.RelationDef);
	}
}
