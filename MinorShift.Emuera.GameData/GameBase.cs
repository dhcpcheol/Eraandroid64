using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData;

internal sealed class GameBase
{
	public string ScriptAutherName = "";

	public string ScriptDetail = "";

	public string ScriptYear = "";

	public string ScriptTitle = "";

	public long ScriptUniqueCode;

	public long ScriptVersion;

	public bool ScriptVersionDefined;

	public long ScriptCompatibleMinVersion = -1L;

	public string Compatible_EmueraVer = "0.000.0.0";

	public string ScriptWindowTitle;

	public long DefaultCharacter = -1L;

	public long DefaultNoItem;

	public string ScriptVersionText
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append((ScriptVersion / 1000).ToString());
			stringBuilder.Append(".");
			if (ScriptVersion % 10 != 0L)
			{
				stringBuilder.Append((ScriptVersion % 1000).ToString("000"));
			}
			else
			{
				stringBuilder.Append((ScriptVersion % 1000 / 10).ToString("00"));
			}
			return stringBuilder.ToString();
		}
	}

	public bool UniqueCodeEqualTo(long target)
	{
		if (target == 0L)
		{
			return true;
		}
		return target == ScriptUniqueCode;
	}

	public bool CheckVersion(long target)
	{
		if (!ScriptVersionDefined && target != 1000)
		{
			return true;
		}
		if (ScriptCompatibleMinVersion <= target)
		{
			return true;
		}
		return ScriptVersion == target;
	}

	private bool tryatoi(string str, out long i)
	{
		if (long.TryParse(str, out i))
		{
			return true;
		}
		StringStream stringStream = new StringStream(str);
		StringBuilder stringBuilder = new StringBuilder(str.Length);
		while (!stringStream.EOS && char.IsNumber(stringStream.Current))
		{
			stringBuilder.Append(stringStream.Current);
			stringStream.ShiftNext();
		}
		if (stringBuilder.Length > 0 && long.TryParse(stringBuilder.ToString(), out i))
		{
			return true;
		}
		return false;
	}

	public bool LoadGameBaseCsv(string basePath)
	{
		if (!File.Exists(basePath))
		{
			return true;
		}
		ScriptPosition pos = null;
		EraStreamReader eraStreamReader = new EraStreamReader(useRename: false);
		if (!eraStreamReader.Open(basePath))
		{
			return true;
		}
		try
		{
			StringStream stringStream = null;
			while ((stringStream = eraStreamReader.ReadEnabledLine()) != null)
			{
				string[] array = stringStream.Substring().Split(',');
				if (array.Length < 2)
				{
					continue;
				}
				array[1].Trim();
				pos = new ScriptPosition(eraStreamReader.Filename, eraStreamReader.LineNo, stringStream.RowString);
				switch (array[0])
				{
				case "コード":
					if (tryatoi(array[1], out ScriptUniqueCode) && ScriptUniqueCode == 0L)
					{
						ParserMediator.Warn("コード:0のセーブデータはいかなるコードのスクリプトからも読めるデータとして扱われます", pos, 0);
					}
					break;
				case "バージョン":
					ScriptVersionDefined = tryatoi(array[1], out ScriptVersion);
					break;
				case "バージョン違い認める":
					tryatoi(array[1], out ScriptCompatibleMinVersion);
					break;
				case "最初からいるキャラ":
					tryatoi(array[1], out DefaultCharacter);
					break;
				case "アイテムなし":
					tryatoi(array[1], out DefaultNoItem);
					break;
				case "タイトル":
					ScriptTitle = array[1];
					break;
				case "作者":
					ScriptAutherName = array[1];
					break;
				case "製作年":
					ScriptYear = array[1];
					break;
				case "追加情報":
					ScriptDetail = array[1];
					break;
				case "ウィンドウタイトル":
					ScriptWindowTitle = array[1];
					break;
				case "動作に必要なEmueraのバージョン":
				{
					Compatible_EmueraVer = array[1];
					if (!Regex.IsMatch(Compatible_EmueraVer, "^\\d+\\.\\d+\\.\\d+\\.\\d+$"))
					{
						ParserMediator.Warn("バージョン指定を読み取れなかったので処理を省略します", pos, 0);
						break;
					}
					Version version = new Version(GlobalStatic.FrontEnd.InternalEmueraVer);
					Version version2 = new Version(Compatible_EmueraVer);
					if (!(version < version2))
					{
						break;
					}
					ParserMediator.Warn("このバリアント動作させるにはVer. " + GlobalStatic.FrontEnd.InternalEmueraVer + "以降のバージョンのEmueraが必要です", pos, 2);
					return false;
				}
				}
			}
		}
		catch
		{
			ParserMediator.Warn("GAMEBASE.CSVの読み込み中にエラーが発生したため、読みこみを中断します", pos, 1);
			return true;
		}
		finally
		{
			eraStreamReader.Close();
		}
		if (ScriptWindowTitle == null)
		{
			if (string.IsNullOrEmpty(ScriptTitle))
			{
				ScriptWindowTitle = "Emuera";
			}
			else
			{
				ScriptWindowTitle = ScriptTitle + " " + ScriptVersionText;
			}
		}
		return true;
	}
}
