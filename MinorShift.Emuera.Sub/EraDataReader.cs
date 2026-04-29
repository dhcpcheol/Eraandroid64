using System;
using System.Collections.Generic;
using System.IO;

namespace MinorShift.Emuera.Sub;

internal sealed class EraDataReader : IDisposable
{
	private FileStream file;

	private StreamReader reader;

	public const string FINISHER = "__FINISHED";

	public const string EMU_1700_START = "__EMUERA_STRAT__";

	public const string EMU_1708_START = "__EMUERA_1708_STRAT__";

	public const string EMU_1729_START = "__EMUERA_1729_STRAT__";

	public const string EMU_1803_START = "__EMUERA_1803_STRAT__";

	public const string EMU_1808_START = "__EMUERA_1808_STRAT__";

	public const string EMU_SEPARATOR = "__EMU_SEPARATOR__";

	private int emu_version = -1;

	public int DataVersion => emu_version;

	public EraDataReader(FileStream file)
	{
		this.file = file;
		file.Seek(0L, SeekOrigin.Begin);
		reader = new StreamReader(file, Config.Encode);
	}

	public string ReadString()
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		return reader.ReadLine() ?? throw new FileEE("読み取るべき文字列がありません");
	}

	public long ReadInt64()
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		long result = 0L;
		if (!long.TryParse(reader.ReadLine() ?? throw new FileEE("読み取るべき数値がありません"), out result))
		{
			throw new FileEE("数値として認識できません");
		}
		return result;
	}

	public void ReadInt64Array(long[] array)
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		if (array == null)
		{
			throw new FileEE("無効な配列が渡されました");
		}
		int num = -1;
		string text = null;
		long result = 0L;
		num = -1;
		while (true)
		{
			num++;
			text = reader.ReadLine();
			if (text == null)
			{
				throw new FileEE("予期しないセーブデータの終端です");
			}
			if (text.Equals("__FINISHED", StringComparison.Ordinal))
			{
				break;
			}
			if (num < array.Length)
			{
				if (!long.TryParse(text, out result))
				{
					throw new FileEE("数値として認識できません");
				}
				array[num] = result;
			}
		}
		for (; num < array.Length; num++)
		{
			array[num] = 0L;
		}
	}

	public void ReadStringArray(string[] array)
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		if (array == null)
		{
			throw new FileEE("無効な配列が渡されました");
		}
		int num = -1;
		string text = null;
		num = -1;
		while (true)
		{
			num++;
			text = reader.ReadLine();
			if (text == null)
			{
				throw new FileEE("予期しないセーブデータの終端です");
			}
			if (text.Equals("__FINISHED", StringComparison.Ordinal))
			{
				break;
			}
			if (num < array.Length)
			{
				array[num] = text;
			}
		}
		for (; num < array.Length; num++)
		{
			array[num] = "";
		}
	}

	public bool SeekEmuStart()
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		if (reader.EndOfStream)
		{
			return false;
		}
		string text;
		do
		{
			text = reader.ReadLine();
			if (text == null)
			{
				return false;
			}
			if (text.Equals("__EMUERA_STRAT__", StringComparison.Ordinal))
			{
				emu_version = 1700;
				return true;
			}
			if (text.Equals("__EMUERA_1708_STRAT__", StringComparison.Ordinal))
			{
				emu_version = 1708;
				return true;
			}
			if (text.Equals("__EMUERA_1729_STRAT__", StringComparison.Ordinal))
			{
				emu_version = 1729;
				return true;
			}
			if (text.Equals("__EMUERA_1803_STRAT__", StringComparison.Ordinal))
			{
				emu_version = 1803;
				return true;
			}
		}
		while (!text.Equals("__EMUERA_1808_STRAT__", StringComparison.Ordinal));
		emu_version = 1808;
		return true;
	}

	public Dictionary<string, string> ReadStringExtended()
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string text = null;
		while (true)
		{
			text = reader.ReadLine();
			if (text == null)
			{
				throw new FileEE("予期しないセーブデータの終端です");
			}
			if (text.Equals("__FINISHED", StringComparison.Ordinal))
			{
				throw new FileEE("セーブデータの形式が不正です");
			}
			if (text.Equals("__EMU_SEPARATOR__", StringComparison.Ordinal))
			{
				break;
			}
			int num = text.IndexOf(':');
			if (num < 0)
			{
				throw new FileEE("セーブデータの形式が不正です");
			}
			string key = text.Substring(0, num);
			string value = text.Substring(num + 1, text.Length - num - 1);
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, value);
			}
		}
		return dictionary;
	}

	public Dictionary<string, long> ReadInt64Extended()
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		Dictionary<string, long> dictionary = new Dictionary<string, long>();
		string text = null;
		while (true)
		{
			text = reader.ReadLine();
			if (text == null)
			{
				throw new FileEE("予期しないセーブデータの終端です");
			}
			if (text.Equals("__FINISHED", StringComparison.Ordinal))
			{
				throw new FileEE("セーブデータの形式が不正です");
			}
			if (text.Equals("__EMU_SEPARATOR__", StringComparison.Ordinal))
			{
				break;
			}
			int num = text.IndexOf(':');
			if (num < 0)
			{
				throw new FileEE("セーブデータの形式が不正です");
			}
			string key = text.Substring(0, num);
			string s = text.Substring(num + 1, text.Length - num - 1);
			long result = 0L;
			if (!long.TryParse(s, out result))
			{
				throw new FileEE("数値として認識できません");
			}
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, result);
			}
		}
		return dictionary;
	}

	public Dictionary<string, List<long>> ReadInt64ArrayExtended()
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		Dictionary<string, List<long>> dictionary = new Dictionary<string, List<long>>();
		string text = null;
		while (true)
		{
			text = reader.ReadLine();
			if (text == null)
			{
				throw new FileEE("予期しないセーブデータの終端です");
			}
			if (text.Equals("__FINISHED", StringComparison.Ordinal))
			{
				throw new FileEE("セーブデータの形式が不正です");
			}
			if (text.Equals("__EMU_SEPARATOR__", StringComparison.Ordinal))
			{
				break;
			}
			string key = text;
			List<long> list = new List<long>();
			while (true)
			{
				text = reader.ReadLine();
				if (text == null)
				{
					throw new FileEE("予期しないセーブデータの終端です");
				}
				if (text.Equals("__EMU_SEPARATOR__", StringComparison.Ordinal))
				{
					throw new FileEE("セーブデータの形式が不正です");
				}
				if (text.Equals("__FINISHED", StringComparison.Ordinal))
				{
					break;
				}
				long result = 0L;
				if (!long.TryParse(text, out result))
				{
					throw new FileEE("数値として認識できません");
				}
				list.Add(result);
			}
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, list);
			}
		}
		return dictionary;
	}

	public Dictionary<string, List<string>> ReadStringArrayExtended()
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
		string text = null;
		while (true)
		{
			text = reader.ReadLine();
			if (text == null)
			{
				throw new FileEE("予期しないセーブデータの終端です");
			}
			if (text.Equals("__FINISHED", StringComparison.Ordinal))
			{
				throw new FileEE("セーブデータの形式が不正です");
			}
			if (text.Equals("__EMU_SEPARATOR__", StringComparison.Ordinal))
			{
				break;
			}
			string key = text;
			List<string> list = new List<string>();
			while (true)
			{
				text = reader.ReadLine();
				if (text == null)
				{
					throw new FileEE("予期しないセーブデータの終端です");
				}
				if (text.Equals("__EMU_SEPARATOR__", StringComparison.Ordinal))
				{
					throw new FileEE("セーブデータの形式が不正です");
				}
				if (text.Equals("__FINISHED", StringComparison.Ordinal))
				{
					break;
				}
				list.Add(text);
			}
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, list);
			}
		}
		return dictionary;
	}

	public Dictionary<string, List<long[]>> ReadInt64Array2DExtended()
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		Dictionary<string, List<long[]>> dictionary = new Dictionary<string, List<long[]>>();
		if (emu_version < 1708)
		{
			return dictionary;
		}
		string text = null;
		while (true)
		{
			text = reader.ReadLine();
			if (text == null)
			{
				throw new FileEE("予期しないセーブデータの終端です");
			}
			if (text.Equals("__FINISHED", StringComparison.Ordinal))
			{
				throw new FileEE("セーブデータの形式が不正です");
			}
			if (text.Equals("__EMU_SEPARATOR__", StringComparison.Ordinal))
			{
				break;
			}
			string key = text;
			List<long[]> list = new List<long[]>();
			while (true)
			{
				text = reader.ReadLine();
				if (text == null)
				{
					throw new FileEE("予期しないセーブデータの終端です");
				}
				if (text.Equals("__EMU_SEPARATOR__", StringComparison.Ordinal))
				{
					throw new FileEE("セーブデータの形式が不正です");
				}
				if (text.Equals("__FINISHED", StringComparison.Ordinal))
				{
					break;
				}
				if (text.Length == 0)
				{
					list.Add(new long[0]);
					continue;
				}
				string[] array = text.Split(',');
				long[] array2 = new long[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					if (!long.TryParse(array[i], out array2[i]))
					{
						throw new FileEE(array[i] + "は数値として認識できません");
					}
				}
				list.Add(array2);
			}
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, list);
			}
		}
		return dictionary;
	}

	public Dictionary<string, List<string[]>> ReadStringArray2DExtended()
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		Dictionary<string, List<string[]>> result = new Dictionary<string, List<string[]>>();
		if (emu_version < 1708)
		{
			return result;
		}
		string obj = reader.ReadLine() ?? throw new FileEE("予期しないセーブデータの終端です");
		if (obj.Equals("__FINISHED", StringComparison.Ordinal))
		{
			throw new FileEE("セーブデータの形式が不正です");
		}
		if (!obj.Equals("__EMU_SEPARATOR__", StringComparison.Ordinal))
		{
			throw new FileEE("StringArray2Dのロードには対応していません");
		}
		return result;
	}

	public Dictionary<string, List<List<long[]>>> ReadInt64Array3DExtended()
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		Dictionary<string, List<List<long[]>>> dictionary = new Dictionary<string, List<List<long[]>>>();
		if (emu_version < 1729)
		{
			return dictionary;
		}
		string text = null;
		while (true)
		{
			text = reader.ReadLine();
			if (text == null)
			{
				throw new FileEE("予期しないセーブデータの終端です");
			}
			if (text.Equals("__FINISHED", StringComparison.Ordinal))
			{
				throw new FileEE("セーブデータの形式が不正です");
			}
			if (text.Equals("__EMU_SEPARATOR__", StringComparison.Ordinal))
			{
				break;
			}
			string key = text;
			List<List<long[]>> list = new List<List<long[]>>();
			while (true)
			{
				text = reader.ReadLine();
				if (text == null)
				{
					throw new FileEE("予期しないセーブデータの終端です");
				}
				if (text.Equals("__EMU_SEPARATOR__", StringComparison.Ordinal))
				{
					throw new FileEE("セーブデータの形式が不正です");
				}
				if (text.Equals("__FINISHED", StringComparison.Ordinal))
				{
					break;
				}
				if (!text.Contains("{"))
				{
					continue;
				}
				List<long[]> list2 = new List<long[]>();
				while (true)
				{
					text = reader.ReadLine();
					if (text == "}")
					{
						break;
					}
					if (text.Length == 0)
					{
						list2.Add(new long[0]);
						continue;
					}
					string[] array = text.Split(',');
					long[] array2 = new long[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						if (!long.TryParse(array[i], out array2[i]))
						{
							throw new FileEE(array[i] + "は数値として認識できません");
						}
					}
					list2.Add(array2);
				}
				list.Add(list2);
			}
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, list);
			}
		}
		return dictionary;
	}

	public Dictionary<string, List<List<string[]>>> ReadStringArray3DExtended()
	{
		if (reader == null)
		{
			throw new FileEE("無効なストリームです");
		}
		Dictionary<string, List<List<string[]>>> result = new Dictionary<string, List<List<string[]>>>();
		if (emu_version < 1729)
		{
			return result;
		}
		string obj = reader.ReadLine() ?? throw new FileEE("予期しないセーブデータの終端です");
		if (obj.Equals("__FINISHED", StringComparison.Ordinal))
		{
			throw new FileEE("セーブデータの形式が不正です");
		}
		if (!obj.Equals("__EMU_SEPARATOR__", StringComparison.Ordinal))
		{
			throw new FileEE("StringArray2Dのロードには対応していません");
		}
		return result;
	}

	public void Dispose()
	{
		if (reader != null)
		{
			reader.Close();
		}
		else if (file != null)
		{
			file.Close();
		}
		file = null;
		reader = null;
	}

	public void Close()
	{
		Dispose();
	}
}
