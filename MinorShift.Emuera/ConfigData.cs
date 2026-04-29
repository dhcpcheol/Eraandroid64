using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Android.Graphics;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera;

internal sealed class ConfigData
{
	private static readonly string configPath;

	private static readonly string configdebugPath;

	private static ConfigData instance;

	private AConfigItem[] configArray = new AConfigItem[70];

	private AConfigItem[] replaceArray = new AConfigItem[50];

	private AConfigItem[] debugArray = new AConfigItem[20];

	public static ConfigData Instance => instance;

	static ConfigData()
	{
		configPath = Program.ExeDir + "emuera.config";
		configdebugPath = Program.DebugDir + "debug.config";
		instance = new ConfigData();
	}

	private ConfigData()
	{
		setDefault();
	}

	private void setDefault()
	{
		int num = 0;
		configArray[num++] = new ConfigItem<bool>(ConfigCode.IgnoreCase, "大文字小文字の違いを無視する", t: true);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.UseRenameFile, "_Rename.csvを利用する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.UseReplaceFile, "_Replace.csvを利用する", t: true);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.UseMouse, "マウスを使用する", t: true);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.UseMenu, "メニューを使用する", t: true);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.UseDebugCommand, "デバッグコマンドを使用する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.AllowMultipleInstances, "多重起動を許可する", t: true);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.AutoSave, "オートセーブを行なう", t: true);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.UseKeyMacro, "キーボードマクロを使用する", t: true);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.SizableWindow, "ウィンドウの高さを可変にする", t: true);
		configArray[num++] = new ConfigItem<TextDrawingMode>(ConfigCode.TextDrawingMode, "描画インターフェース", TextDrawingMode.GRAPHICS);
		configArray[num++] = new ConfigItem<int>(ConfigCode.WindowX, "ウィンドウ幅", 760);
		configArray[num++] = new ConfigItem<int>(ConfigCode.WindowY, "ウィンドウ高さ", 480);
		configArray[num++] = new ConfigItem<int>(ConfigCode.WindowPosX, "ウィンドウ位置X", 0);
		configArray[num++] = new ConfigItem<int>(ConfigCode.WindowPosY, "ウィンドウ位置Y", 0);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.SetWindowPos, "起動時のウィンドウ位置を指定する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.WindowMaximixed, "起動時にウィンドウを最大化する", t: false);
		configArray[num++] = new ConfigItem<int>(ConfigCode.MaxLog, "履歴ログの行数", 5000);
		configArray[num++] = new ConfigItem<int>(ConfigCode.PrintCPerLine, "PRINTCを並べる数", 3);
		configArray[num++] = new ConfigItem<int>(ConfigCode.PrintCLength, "PRINTCの文字数", 25);
		configArray[num++] = new ConfigItem<string>(ConfigCode.FontName, "フォント名", "ＭＳ ゴシック");
		configArray[num++] = new ConfigItem<int>(ConfigCode.FontSize, "フォントサイズ", 30);
		configArray[num++] = new ConfigItem<int>(ConfigCode.LineHeight, "一行の高さ", 20);
		configArray[num++] = new ConfigItem<Android.Graphics.Color>(ConfigCode.ForeColor, "文字色", Android.Graphics.Color.Argb(255, 192, 192, 192));
		configArray[num++] = new ConfigItem<Android.Graphics.Color>(ConfigCode.BackColor, "背景色", Android.Graphics.Color.Argb(255, 0, 0, 0));
		configArray[num++] = new ConfigItem<Android.Graphics.Color>(ConfigCode.FocusColor, "選択中文字色", Android.Graphics.Color.Argb(255, 255, 255, 0));
		configArray[num++] = new ConfigItem<Android.Graphics.Color>(ConfigCode.LogColor, "履歴文字色", Android.Graphics.Color.Argb(255, 192, 192, 192));
		configArray[num++] = new ConfigItem<int>(ConfigCode.FPS, "フレーム毎秒", 5);
		configArray[num++] = new ConfigItem<int>(ConfigCode.SkipFrame, "最大スキップフレーム数", 3);
		configArray[num++] = new ConfigItem<int>(ConfigCode.ScrollHeight, "スクロール行数", 1);
		configArray[num++] = new ConfigItem<int>(ConfigCode.InfiniteLoopAlertTime, "無限ループ警告までのミリ秒数", 5000);
		configArray[num++] = new ConfigItem<int>(ConfigCode.DisplayWarningLevel, "表示する最低警告レベル", 1);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.DisplayReport, "ロード時にレポートを表示する", t: false);
		configArray[num++] = new ConfigItem<ReduceArgumentOnLoadFlag>(ConfigCode.ReduceArgumentOnLoad, "ロード時に引数を解析する", ReduceArgumentOnLoadFlag.NO);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.IgnoreUncalledFunction, "呼び出されなかった関数を無視する", t: true);
		configArray[num++] = new ConfigItem<DisplayWarningFlag>(ConfigCode.FunctionNotFoundWarning, "関数が見つからない警告の扱い", DisplayWarningFlag.IGNORE);
		configArray[num++] = new ConfigItem<DisplayWarningFlag>(ConfigCode.FunctionNotCalledWarning, "関数が呼び出されなかった警告の扱い", DisplayWarningFlag.IGNORE);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.ChangeMasterNameIfDebug, "デバッグコマンドを使用した時にMASTERの名前を変更する", t: true);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.ButtonWrap, "ボタンの途中で行を折りかえさない", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.SearchSubdirectory, "サブディレクトリを検索する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.SortWithFilename, "読み込み順をファイル名順にソートする", t: false);
		configArray[num++] = new ConfigItem<long>(ConfigCode.LastKey, "最終更新コード", 0L);
		configArray[num++] = new ConfigItem<int>(ConfigCode.SaveDataNos, "表示するセーブデータ数", 20);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.WarnBackCompatibility, "eramaker互換性に関する警告を表示する", t: true);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.AllowFunctionOverloading, "システム関数の上書きを許可する", t: true);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.WarnFunctionOverloading, "システム関数が上書きされたとき警告を表示する", t: true);
		configArray[num++] = new ConfigItem<string>(ConfigCode.TextEditor, "関連づけるテキストエディタ", "notepad");
		configArray[num++] = new ConfigItem<TextEditorType>(ConfigCode.EditorType, "テキストエディタコマンドライン指定", TextEditorType.USER_SETTING);
		configArray[num++] = new ConfigItem<string>(ConfigCode.EditorArgument, "エディタに渡す行指定引数", "");
		configArray[num++] = new ConfigItem<bool>(ConfigCode.WarnNormalFunctionOverloading, "同名の非イベント関数が複数定義されたとき警告する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.CompatiErrorLine, "解釈不可能な行があっても実行する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.CompatiCALLNAME, "CALLNAMEが空文字列の時にNAMEを代入する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.UseSaveFolder, "セーブデータをsavフォルダ内に作成する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.CompatiRAND, "擬似変数RANDの仕様をeramakerに合わせる", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.CompatiDRAWLINE, "DRAWLINEを常に新しい行で行う", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.CompatiFunctionNoignoreCase, "関数・属性については大文字小文字を無視しない", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.SystemAllowFullSpace, "全角スペースをホワイトスペースに含める", t: true);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.SystemSaveInUTF8, "セーブデータをUTF-8で保存する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.CompatiLinefeedAs1739, "ver1739以前の非ボタン折り返しを再現する", t: false);
		configArray[num++] = new ConfigItem<UseLanguage>(ConfigCode.useLanguage, "内部で使用する東アジア言語", UseLanguage.JAPANESE);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.AllowLongInputByMouse, "ONEINPUT系命令でマウスによる2文字以上の入力を許可する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.CompatiCallEvent, "イベント関数のCALLを許可する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.CompatiSPChara, "SPキャラを使用する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.SystemSaveInBinary, "セーブデータをバイナリ形式で保存する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.CompatiFuncArgOptional, "ユーザー関数の全ての引数の省略を許可する", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.CompatiFuncArgAutoConvert, "ユーザー関数の引数に自動的にTOSTRを補完する", t: true);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.SystemIgnoreTripleSymbol, "FORM中の三連記号を展開しない", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.TimesNotRigorousCalculation, "TIMESの計算をeramakerにあわせる", t: false);
		configArray[num++] = new ConfigItem<bool>(ConfigCode.SystemNoTarget, "キャラクタ変数の引数を補完しない", t: false);
		num = 0;
		debugArray[num++] = new ConfigItem<bool>(ConfigCode.DebugShowWindow, "起動時にデバッグウインドウを表示する", t: true);
		debugArray[num++] = new ConfigItem<bool>(ConfigCode.DebugWindowTopMost, "デバッグウインドウを最前面に表示する", t: true);
		debugArray[num++] = new ConfigItem<int>(ConfigCode.DebugWindowWidth, "デバッグウィンドウ幅", 400);
		debugArray[num++] = new ConfigItem<int>(ConfigCode.DebugWindowHeight, "デバッグウィンドウ高さ", 300);
		debugArray[num++] = new ConfigItem<bool>(ConfigCode.DebugSetWindowPos, "デバッグウィンドウ位置を指定する", t: false);
		debugArray[num++] = new ConfigItem<int>(ConfigCode.DebugWindowPosX, "デバッグウィンドウ位置X", 0);
		debugArray[num++] = new ConfigItem<int>(ConfigCode.DebugWindowPosY, "デバッグウィンドウ位置Y", 0);
		num = 0;
		replaceArray[num++] = new ConfigItem<string>(ConfigCode.MoneyLabel, "お金の単位", "$");
		replaceArray[num++] = new ConfigItem<bool>(ConfigCode.MoneyFirst, "単位の位置", t: true);
		replaceArray[num++] = new ConfigItem<string>(ConfigCode.LoadLabel, "起動時簡略表示", "Now Loading...");
		replaceArray[num++] = new ConfigItem<int>(ConfigCode.MaxShopItem, "販売アイテム数", 100);
		replaceArray[num++] = new ConfigItem<string>(ConfigCode.DrawLineString, "DRAWLINE文字", "-");
		replaceArray[num++] = new ConfigItem<char>(ConfigCode.BarChar1, "BAR文字1", '*');
		replaceArray[num++] = new ConfigItem<char>(ConfigCode.BarChar2, "BAR文字2", '.');
		replaceArray[num++] = new ConfigItem<string>(ConfigCode.TitleMenuString0, "システムメニュー0", "最初からはじめる");
		replaceArray[num++] = new ConfigItem<string>(ConfigCode.TitleMenuString1, "システムメニュー1", "ロードしてはじめる");
		replaceArray[num++] = new ConfigItem<int>(ConfigCode.ComAbleDefault, "COM_ABLE初期値", 1);
		replaceArray[num++] = new ConfigItem<List<long>>(ConfigCode.StainDefault, "汚れの初期値", new List<long>(new long[5] { 0L, 0L, 2L, 1L, 8L }));
		replaceArray[num++] = new ConfigItem<string>(ConfigCode.TimeupLabel, "時間切れ表示", "時間切れ");
		replaceArray[num++] = new ConfigItem<List<long>>(ConfigCode.ExpLvDef, "EXPLVの初期値", new List<long>(new long[6] { 0L, 1L, 4L, 20L, 50L, 200L }));
		replaceArray[num++] = new ConfigItem<List<long>>(ConfigCode.PalamLvDef, "PALAMLVの初期値", new List<long>(new long[10] { 0L, 100L, 500L, 3000L, 10000L, 30000L, 60000L, 100000L, 150000L, 250000L }));
		replaceArray[num++] = new ConfigItem<long>(ConfigCode.pbandDef, "PBANDの初期値", 4L);
		replaceArray[num++] = new ConfigItem<long>(ConfigCode.RelationDef, "RELATIONの初期値", 0L);
	}

	public ConfigData Copy()
	{
		ConfigData configData = new ConfigData();
		for (int i = 0; i < configArray.Length; i++)
		{
			if (configArray[i] != null && configData.configArray[i] != null)
			{
				configArray[i].CopyTo(configData.configArray[i]);
			}
		}
		for (int j = 0; j < configArray.Length; j++)
		{
			if (configArray[j] != null && configData.configArray[j] != null)
			{
				configArray[j].CopyTo(configData.configArray[j]);
			}
		}
		for (int k = 0; k < replaceArray.Length; k++)
		{
			if (replaceArray[k] != null && configData.replaceArray[k] != null)
			{
				replaceArray[k].CopyTo(configData.replaceArray[k]);
			}
		}
		return configData;
	}

	public Dictionary<ConfigCode, string> GetConfigNameDic()
	{
		Dictionary<ConfigCode, string> dictionary = new Dictionary<ConfigCode, string>();
		AConfigItem[] array = configArray;
		foreach (AConfigItem aConfigItem in array)
		{
			if (aConfigItem != null)
			{
				dictionary.Add(aConfigItem.Code, aConfigItem.Text);
			}
		}
		return dictionary;
	}

	public T GetConfigValue<T>(ConfigCode code)
	{
		return ((ConfigItem<T>)GetItem(code)).Value;
	}

	public AConfigItem GetItem(ConfigCode code)
	{
		AConfigItem aConfigItem = GetConfigItem(code);
		if (aConfigItem == null)
		{
			aConfigItem = GetReplaceItem(code);
			if (aConfigItem == null)
			{
				aConfigItem = GetDebugItem(code);
			}
		}
		return aConfigItem;
	}

	public AConfigItem GetItem(string key)
	{
		AConfigItem aConfigItem = GetConfigItem(key);
		if (aConfigItem == null)
		{
			aConfigItem = GetReplaceItem(key);
			if (aConfigItem == null)
			{
				aConfigItem = GetDebugItem(key);
			}
		}
		return aConfigItem;
	}

	public AConfigItem GetConfigItem(ConfigCode code)
	{
		AConfigItem[] array = configArray;
		foreach (AConfigItem aConfigItem in array)
		{
			if (aConfigItem != null && aConfigItem.Code == code)
			{
				return aConfigItem;
			}
		}
		return null;
	}

	public AConfigItem GetConfigItem(string key)
	{
		AConfigItem[] array = configArray;
		foreach (AConfigItem aConfigItem in array)
		{
			if (aConfigItem != null)
			{
				if (aConfigItem.Name == key)
				{
					return aConfigItem;
				}
				if (aConfigItem.Text == key)
				{
					return aConfigItem;
				}
			}
		}
		return null;
	}

	public AConfigItem GetReplaceItem(ConfigCode code)
	{
		AConfigItem[] array = replaceArray;
		foreach (AConfigItem aConfigItem in array)
		{
			if (aConfigItem != null && aConfigItem.Code == code)
			{
				return aConfigItem;
			}
		}
		return null;
	}

	public AConfigItem GetReplaceItem(string key)
	{
		AConfigItem[] array = replaceArray;
		foreach (AConfigItem aConfigItem in array)
		{
			if (aConfigItem != null)
			{
				if (aConfigItem.Name == key)
				{
					return aConfigItem;
				}
				if (aConfigItem.Text == key)
				{
					return aConfigItem;
				}
			}
		}
		return null;
	}

	public AConfigItem GetDebugItem(ConfigCode code)
	{
		AConfigItem[] array = debugArray;
		foreach (AConfigItem aConfigItem in array)
		{
			if (aConfigItem != null && aConfigItem.Code == code)
			{
				return aConfigItem;
			}
		}
		return null;
	}

	public AConfigItem GetDebugItem(string key)
	{
		AConfigItem[] array = debugArray;
		foreach (AConfigItem aConfigItem in array)
		{
			if (aConfigItem != null)
			{
				if (aConfigItem.Name == key)
				{
					return aConfigItem;
				}
				if (aConfigItem.Text == key)
				{
					return aConfigItem;
				}
			}
		}
		return null;
	}

	public SingleTerm GetConfigValueInERB(string text, ref string errMes)
	{
		AConfigItem item = Instance.GetItem(text);
		if (item == null)
		{
			errMes = "文字列\"" + text + "\"は適切なコンフィグ名ではありません";
			return null;
		}
		switch (item.Code)
		{
		case ConfigCode.AutoSave:
		case ConfigCode.MoneyFirst:
			if (item.GetValue<bool>())
			{
				return new SingleTerm(1L);
			}
			return new SingleTerm(0L);
		case ConfigCode.WindowX:
		case ConfigCode.PrintCPerLine:
		case ConfigCode.PrintCLength:
		case ConfigCode.FontSize:
		case ConfigCode.LineHeight:
		case ConfigCode.SaveDataNos:
		case ConfigCode.MaxShopItem:
		case ConfigCode.ComAbleDefault:
			return new SingleTerm(item.GetValue<int>());
		case ConfigCode.ForeColor:
		case ConfigCode.BackColor:
		case ConfigCode.FocusColor:
		case ConfigCode.LogColor:
		{
			System.Drawing.Color value = item.GetValue<System.Drawing.Color>();
			return new SingleTerm((value.R * 256 + value.G) * 256 + value.B);
		}
		case ConfigCode.pbandDef:
		case ConfigCode.RelationDef:
			return new SingleTerm(item.GetValue<long>());
		case ConfigCode.FontName:
		case ConfigCode.MoneyLabel:
		case ConfigCode.LoadLabel:
		case ConfigCode.DrawLineString:
		case ConfigCode.TitleMenuString0:
		case ConfigCode.TitleMenuString1:
		case ConfigCode.TimeupLabel:
			return new SingleTerm(item.GetValue<string>());
		case ConfigCode.BarChar1:
		case ConfigCode.BarChar2:
			return new SingleTerm(item.GetValue<char>().ToString());
		case ConfigCode.TextDrawingMode:
			return new SingleTerm(item.GetValue<TextDrawingMode>().ToString());
		default:
			errMes = "コンフィグ文字列\"" + text + "\"の値の取得は許可されていません";
			return null;
		}
	}

	public bool SaveConfig()
	{
		StreamWriter streamWriter = null;
		try
		{
			streamWriter = new StreamWriter(configPath, append: false, Config.Encode);
			for (int i = 0; i < configArray.Length; i++)
			{
				AConfigItem aConfigItem = configArray[i];
				if (aConfigItem != null && aConfigItem.Code != ConfigCode.CompatiDRAWLINE && (aConfigItem.Code != ConfigCode.ChangeMasterNameIfDebug || !aConfigItem.GetValue<bool>()) && (aConfigItem.Code != ConfigCode.LastKey || aConfigItem.GetValue<long>() != 0L))
				{
					streamWriter.WriteLine(aConfigItem.ToString());
				}
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

	public bool LoadConfig()
	{
		string text = Program.CsvDir + "_default.config";
		string text2 = Program.CsvDir + "_fixed.config";
		if (!File.Exists(text))
		{
			text = Program.CsvDir + "default.config";
		}
		if (!File.Exists(text2))
		{
			text2 = Program.CsvDir + "fixed.config";
		}
		loadConfig(text, fix: false);
		loadConfig(configPath, fix: false);
		loadConfig(text2, fix: true);
		Config.SetConfig(this);
		bool flag = false;
		if (!File.Exists(configPath))
		{
			flag = true;
		}
		if (Config.CheckUpdate())
		{
			GetItem(ConfigCode.LastKey).SetValue(Config.LastKey);
			flag = true;
		}
		if (flag)
		{
			SaveConfig();
		}
		return true;
	}

	private bool loadConfig(string confPath, bool fix)
	{
		if (!File.Exists(confPath))
		{
			return false;
		}
		EraStreamReader eraStreamReader = new EraStreamReader(useRename: false);
		if (!eraStreamReader.Open(confPath))
		{
			return false;
		}
		ScriptPosition pos = null;
		try
		{
			string text = null;
			while ((text = eraStreamReader.ReadLine()) != null)
			{
				if (text.Length == 0 || text[0] == ';')
				{
					continue;
				}
				pos = new ScriptPosition(eraStreamReader.Filename, eraStreamReader.LineNo, text);
				string[] array = text.Split(':');
				if (array.Length < 2)
				{
					continue;
				}
				AConfigItem configItem = GetConfigItem(array[0].Trim());
				if (configItem == null)
				{
					continue;
				}
				if (configItem.Code == ConfigCode.CompatiDRAWLINE)
				{
					configItem = GetConfigItem(ConfigCode.CompatiLinefeedAs1739);
				}
				if (configItem.Code == ConfigCode.TextEditor && array.Length > 2)
				{
					if (array[2].StartsWith("\\"))
					{
						ref string reference = ref array[1];
						reference = reference + ":" + array[2];
					}
					if (array.Length > 3)
					{
						for (int i = 3; i < array.Length; i++)
						{
							ref string reference2 = ref array[1];
							reference2 = reference2 + ":" + array[i];
						}
					}
				}
				if (configItem.Code == ConfigCode.EditorArgument)
				{
					((ConfigItem<string>)configItem).Value = array[1];
				}
				else if (configItem.TryParse(array[1]) && fix)
				{
					configItem.Fixed = true;
				}
			}
		}
		catch (EmueraException ex)
		{
			ParserMediator.ConfigWarn(ex.Message, pos, 1, null);
		}
		catch (Exception ex2)
		{
			ParserMediator.ConfigWarn(ex2.GetType().ToString() + ":" + ex2.Message, pos, 1, ex2.StackTrace);
		}
		finally
		{
			eraStreamReader.Dispose();
		}
		return true;
	}

	public void LoadReplaceFile(string filename)
	{
		EraStreamReader eraStreamReader = new EraStreamReader(useRename: false);
		if (!eraStreamReader.Open(filename))
		{
			return;
		}
		ScriptPosition pos = null;
		try
		{
			string text = null;
			while ((text = eraStreamReader.ReadLine()) != null)
			{
				if (text.Length == 0 || text[0] == ';')
				{
					continue;
				}
				pos = new ScriptPosition(eraStreamReader.Filename, eraStreamReader.LineNo, text);
				string[] array = text.Split(',', ':');
				if (array.Length >= 2)
				{
					string key = array[0].Trim();
					array[1] = text.Substring(array[0].Length + 1);
					if (!string.IsNullOrEmpty(array[1].Trim()))
					{
						GetReplaceItem(key)?.TryParse(array[1]);
					}
				}
			}
		}
		catch (EmueraException ex)
		{
			ParserMediator.Warn(ex.Message, pos, 1);
		}
		catch (Exception ex2)
		{
			ParserMediator.Warn(ex2.GetType().ToString() + ":" + ex2.Message, pos, 1, ex2.StackTrace);
		}
		finally
		{
			eraStreamReader.Dispose();
		}
	}

	public bool SaveDebugConfig()
	{
		StreamWriter streamWriter = null;
		try
		{
			streamWriter = new StreamWriter(configdebugPath, append: false, Config.Encode);
			for (int i = 0; i < debugArray.Length; i++)
			{
				AConfigItem aConfigItem = debugArray[i];
				if (aConfigItem != null)
				{
					streamWriter.WriteLine(aConfigItem.ToString());
				}
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

	public bool LoadDebugConfig()
	{
		if (File.Exists(configdebugPath))
		{
			EraStreamReader eraStreamReader = new EraStreamReader(useRename: false);
			if (eraStreamReader.Open(configdebugPath))
			{
				ScriptPosition pos = null;
				try
				{
					string text = null;
					while ((text = eraStreamReader.ReadLine()) != null)
					{
						if (text.Length != 0 && text[0] != ';')
						{
							pos = new ScriptPosition(eraStreamReader.Filename, eraStreamReader.LineNo, text);
							string[] array = text.Split(':');
							if (array.Length >= 2)
							{
								GetDebugItem(array[0].Trim())?.TryParse(array[1]);
							}
						}
					}
				}
				catch (EmueraException ex)
				{
					ParserMediator.ConfigWarn(ex.Message, pos, 1, null);
					goto IL_00e2;
				}
				catch (Exception ex2)
				{
					ParserMediator.ConfigWarn(ex2.GetType().ToString() + ":" + ex2.Message, pos, 1, ex2.StackTrace);
					goto IL_00e2;
				}
				finally
				{
					eraStreamReader.Dispose();
				}
				Config.SetDebugConfig(this);
				return true;
			}
		}
		goto IL_00e2;
		IL_00e2:
		Config.SetDebugConfig(this);
		return false;
	}
}
