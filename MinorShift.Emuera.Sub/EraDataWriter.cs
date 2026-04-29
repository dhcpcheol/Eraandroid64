using System;
using System.IO;
using System.Text;

namespace MinorShift.Emuera.Sub;

internal sealed class EraDataWriter : IDisposable
{
	public const string FINISHER = "__FINISHED";

	public const string EMU_START = "__EMUERA_1808_STRAT__";

	public const string EMU_SEPARATOR = "__EMU_SEPARATOR__";

	private FileStream file;

	private StreamWriter writer;

	public EraDataWriter(FileStream file)
	{
		this.file = file;
		writer = new StreamWriter(file, Config.SaveEncode);
	}

	public void Write(long integer)
	{
		if (writer == null)
		{
			throw new FileEE("無効なストリームです");
		}
		writer.WriteLine(integer.ToString());
	}

	public void Write(string str)
	{
		if (writer == null)
		{
			throw new FileEE("無効なストリームです");
		}
		if (str == null)
		{
			writer.WriteLine("");
		}
		else
		{
			writer.WriteLine(str);
		}
	}

	public void Write(long[] array)
	{
		if (writer == null)
		{
			throw new FileEE("無効なストリームです");
		}
		if (array == null)
		{
			throw new FileEE("無効な配列が渡されました");
		}
		int num = -1;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != 0L)
			{
				num = i;
			}
		}
		num++;
		for (int j = 0; j < num; j++)
		{
			writer.WriteLine(array[j].ToString());
		}
		writer.WriteLine("__FINISHED");
	}

	public void Write(string[] array)
	{
		if (writer == null)
		{
			throw new FileEE("無効なストリームです");
		}
		if (array == null)
		{
			throw new FileEE("無効な配列が渡されました");
		}
		int num = -1;
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				num = i;
			}
		}
		num++;
		for (int j = 0; j < num; j++)
		{
			if (array[j] == null)
			{
				writer.WriteLine("");
			}
			else
			{
				writer.WriteLine(array[j]);
			}
		}
		writer.WriteLine("__FINISHED");
	}

	public void EmuStart()
	{
		if (writer == null)
		{
			throw new FileEE("無効なストリームです");
		}
		writer.WriteLine("__EMUERA_1808_STRAT__");
	}

	public void EmuSeparete()
	{
		if (writer == null)
		{
			throw new FileEE("無効なストリームです");
		}
		writer.WriteLine("__EMU_SEPARATOR__");
	}

	public void WriteExtended(string key, long value)
	{
		if (writer == null)
		{
			throw new FileEE("無効なストリームです");
		}
		if (value != 0L)
		{
			writer.WriteLine($"{key}:{value}");
		}
	}

	public void WriteExtended(string key, string value)
	{
		if (writer == null)
		{
			throw new FileEE("無効なストリームです");
		}
		if (!string.IsNullOrEmpty(value))
		{
			writer.WriteLine($"{key}:{value}");
		}
	}

	public void WriteExtended(string key, long[] array)
	{
		if (writer == null)
		{
			throw new FileEE("無効なストリームです");
		}
		if (array == null)
		{
			throw new FileEE("無効な配列が渡されました");
		}
		int num = -1;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != 0L)
			{
				num = i;
			}
		}
		num++;
		if (num != 0)
		{
			writer.WriteLine(key);
			for (int j = 0; j < num; j++)
			{
				writer.WriteLine(array[j].ToString());
			}
			writer.WriteLine("__FINISHED");
		}
	}

	public void WriteExtended(string key, string[] array)
	{
		if (writer == null)
		{
			throw new FileEE("無効なストリームです");
		}
		if (array == null)
		{
			throw new FileEE("無効な配列が渡されました");
		}
		int num = -1;
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				num = i;
			}
		}
		num++;
		if (num == 0)
		{
			return;
		}
		writer.WriteLine(key);
		for (int j = 0; j < num; j++)
		{
			if (array[j] == null)
			{
				writer.WriteLine("");
			}
			else
			{
				writer.WriteLine(array[j]);
			}
		}
		writer.WriteLine("__FINISHED");
	}

	public void WriteExtended(string key, long[,] array2D)
	{
		if (writer == null)
		{
			throw new FileEE("無効なストリームです");
		}
		if (array2D == null)
		{
			throw new FileEE("無効な配列が渡されました");
		}
		int num = 0;
		int length = array2D.GetLength(0);
		int length2 = array2D.GetLength(1);
		int[] array = new int[length];
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				if (array2D[i, j] != 0L)
				{
					num = i + 1;
					array[i] = j + 1;
				}
			}
		}
		if (num == 0)
		{
			return;
		}
		writer.WriteLine(key);
		for (int k = 0; k < num; k++)
		{
			if (array[k] == 0)
			{
				writer.WriteLine("");
				continue;
			}
			StringBuilder stringBuilder = new StringBuilder("");
			for (int l = 0; l < array[k]; l++)
			{
				stringBuilder.Append(array2D[k, l].ToString());
				if (l != array[k] - 1)
				{
					stringBuilder.Append(",");
				}
			}
			writer.WriteLine(stringBuilder.ToString());
		}
		writer.WriteLine("__FINISHED");
	}

	public void WriteExtended(string key, string[,] array2D)
	{
		throw new NotImplementedException("まだ実装してないよ");
	}

	public void WriteExtended(string key, long[,,] array3D)
	{
		if (writer == null)
		{
			throw new FileEE("無効なストリームです");
		}
		if (array3D == null)
		{
			throw new FileEE("無効な配列が渡されました");
		}
		int num = 0;
		int length = array3D.GetLength(0);
		int length2 = array3D.GetLength(1);
		int length3 = array3D.GetLength(2);
		int[] array = new int[length];
		int[,] array2 = new int[length, length2];
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				for (int k = 0; k < length3; k++)
				{
					if (array3D[i, j, k] != 0L)
					{
						num = i + 1;
						array[i] = j + 1;
						array2[i, j] = k + 1;
					}
				}
			}
		}
		if (num == 0)
		{
			return;
		}
		writer.WriteLine(key);
		for (int l = 0; l < num; l++)
		{
			writer.WriteLine(l + "{");
			if (array[l] == 0)
			{
				writer.WriteLine("}");
				continue;
			}
			for (int m = 0; m < array[l]; m++)
			{
				StringBuilder stringBuilder = new StringBuilder("");
				if (array2[l, m] == 0)
				{
					writer.WriteLine("");
					continue;
				}
				for (int n = 0; n < array2[l, m]; n++)
				{
					stringBuilder.Append(array3D[l, m, n].ToString());
					if (n != array2[l, m] - 1)
					{
						stringBuilder.Append(",");
					}
				}
				writer.WriteLine(stringBuilder.ToString());
			}
			writer.WriteLine("}");
		}
		writer.WriteLine("__FINISHED");
	}

	public void WriteExtended(string key, string[,,] array2D)
	{
		throw new NotImplementedException("まだ実装してないよ");
	}

	public void Dispose()
	{
		if (writer != null)
		{
			writer.Close();
		}
		else if (file != null)
		{
			file.Close();
		}
		writer = null;
		file = null;
	}

	public void Close()
	{
		Dispose();
	}
}
