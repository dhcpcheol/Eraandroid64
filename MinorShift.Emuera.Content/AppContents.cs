using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.Content;

internal static class AppContents
{
	private static Dictionary<string, AContentFile> resourceDic = new Dictionary<string, AContentFile>();

	private static Dictionary<string, AContentItem> itemDic = new Dictionary<string, AContentItem>();

	public static T GetContent<T>(string name) where T : AContentItem
	{
		if (name == null)
		{
			return null;
		}
		name = name.ToUpper();
		if (!itemDic.ContainsKey(name))
		{
			return null;
		}
		return itemDic[name] as T;
	}

	public static void LoadContents()
	{
		if (!Directory.Exists(Program.ContentDir))
		{
			return;
		}
		try
		{
			List<string> list = new List<string>();
			list.AddRange(Directory.GetFiles(Program.ContentDir, "*.png", SearchOption.TopDirectoryOnly));
			list.AddRange(Directory.GetFiles(Program.ContentDir, "*.bmp", SearchOption.TopDirectoryOnly));
			list.AddRange(Directory.GetFiles(Program.ContentDir, "*.jpg", SearchOption.TopDirectoryOnly));
			list.AddRange(Directory.GetFiles(Program.ContentDir, "*.gif", SearchOption.TopDirectoryOnly));
			foreach (string item in list)
			{
				string text = Path.GetFileName(item).ToUpper();
				resourceDic.Add(text, new BaseImage(text, item));
			}
			string[] files = Directory.GetFiles(Program.ContentDir, "*.csv", SearchOption.TopDirectoryOnly);
			for (int i = 0; i < files.Length; i++)
			{
				string[] array = File.ReadAllLines(files[i], Config.Encode);
				foreach (string text2 in array)
				{
					if (text2.Length == 0)
					{
						continue;
					}
					string text3 = text2.Trim();
					if (text3.Length != 0 && !text3.StartsWith(";"))
					{
						AContentItem aContentItem = CreateFromCsv(text3.Split(','));
						if (aContentItem != null && !itemDic.ContainsKey(aContentItem.Name))
						{
							itemDic.Add(aContentItem.Name, aContentItem);
						}
					}
				}
			}
		}
		catch
		{
			throw new CodeEE("リソースファイルのロード中にエラーが発生しました");
		}
	}

	public static void UnloadContents()
	{
		foreach (AContentFile value in resourceDic.Values)
		{
			value.Dispose();
		}
		resourceDic.Clear();
		itemDic.Clear();
	}

	private static AContentItem CreateFromCsv(string[] tokens)
	{
		if (tokens.Length < 2)
		{
			return null;
		}
		string text = tokens[0].Trim().ToUpper();
		string text2 = tokens[1].ToUpper();
		if (text.Length == 0 || text2.Length == 0)
		{
			return null;
		}
		if (!resourceDic.ContainsKey(text2))
		{
			return null;
		}
		AContentFile aContentFile = resourceDic[text2];
		if (aContentFile is BaseImage)
		{
			BaseImage baseImage = aContentFile as BaseImage;
			baseImage.Load(Config.TextDrawingMode == TextDrawingMode.WINAPI);
			if (!baseImage.Enabled)
			{
				return null;
			}
			Rectangle rect = new Rectangle(0, 0, baseImage.Bitmap.Width, baseImage.Bitmap.Height);
			bool noresize = false;
			if (tokens.Length >= 6)
			{
				int[] array = new int[4];
				bool flag = true;
				for (int i = 0; i < 4; i++)
				{
					flag &= int.TryParse(tokens[i + 2], out array[i]);
				}
				if (flag)
				{
					rect = new Rectangle(array[0], array[1], array[2], array[3]);
				}
				if (tokens.Length >= 7)
				{
					string[] array2 = tokens[6].Split('|');
					for (int j = 0; j < array2.Length; j++)
					{
						string text3 = array2[j].Trim().ToUpper();
						if (text3 == "NORESIZE")
						{
							throw new NotImplCodeEE();
						}
					}
				}
			}
			return new CroppedImage(text, baseImage, rect, noresize);
		}
		return null;
	}
}
