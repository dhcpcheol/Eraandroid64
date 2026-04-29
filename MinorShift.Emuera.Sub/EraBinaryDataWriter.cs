using System;
using System.IO;
using System.Text;

namespace MinorShift.Emuera.Sub;

internal sealed class EraBinaryDataWriter : IDisposable
{
	private BinaryWriter writer;

	public EraBinaryDataWriter(FileStream fs)
	{
		writer = new BinaryWriter(fs, Encoding.Unicode);
	}

	public void WriteHeader()
	{
		writer.Write(727905341820519817uL);
		writer.Write(1808u);
		writer.Write(0u);
		for (int i = 0; (long)i < 0L; i++)
		{
			writer.Write(0u);
		}
	}

	public void WriteFileType(EraSaveFileType type)
	{
		writer.Write((byte)type);
	}

	public void WriteInt64(long v)
	{
		writer.Write(v);
	}

	public void WriteString(string s)
	{
		writer.Write(s);
	}

	public void WriteSeparator()
	{
		writer.Write((byte)253);
	}

	public void WriteEOC()
	{
		writer.Write((byte)254);
	}

	public void WriteEOF()
	{
		writer.Write(byte.MaxValue);
	}

	public void WriteWithKey(string key, object v)
	{
		if (v is long)
		{
			writer.Write((byte)0);
			writer.Write(key);
			writeData((long)v);
		}
		else if (v is long[])
		{
			writer.Write((byte)1);
			writer.Write(key);
			writeData((long[])v);
		}
		else if (v is long[,])
		{
			writer.Write((byte)2);
			writer.Write(key);
			writeData((long[,])v);
		}
		else if (v is long[,,])
		{
			writer.Write((byte)3);
			writer.Write(key);
			writeData((long[,,])v);
		}
		else if (v is string)
		{
			writer.Write((byte)16);
			writer.Write(key);
			writeData((string)v);
		}
		else if (v is string[])
		{
			writer.Write((byte)17);
			writer.Write(key);
			writeData((string[])v);
		}
		else if (v is string[,])
		{
			writer.Write((byte)18);
			writer.Write(key);
			writeData((string[,])v);
		}
		else if (v is string[,,])
		{
			writer.Write((byte)19);
			writer.Write(key);
			writeData((string[,,])v);
		}
	}

	private void m_WriteInt(long v)
	{
		if (v >= 0 && v <= 207)
		{
			writer.Write((byte)v);
		}
		else if (v >= -32768 && v <= 32767)
		{
			writer.Write((byte)208);
			writer.Write((short)v);
		}
		else if (v >= int.MinValue && v <= int.MaxValue)
		{
			writer.Write((byte)209);
			writer.Write((int)v);
		}
		else
		{
			writer.Write((byte)210);
			writer.Write(v);
		}
	}

	private void writeData(long v)
	{
		m_WriteInt(v);
	}

	private void writeData(long[] array)
	{
		writer.Write(array.Length);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == 0L)
			{
				num++;
				continue;
			}
			if (num > 0)
			{
				writer.Write((byte)240);
				m_WriteInt(num);
				num = 0;
			}
			m_WriteInt(array[i]);
		}
		writer.Write(byte.MaxValue);
	}

	private void writeData(long[,] array)
	{
		int num = 0;
		int num2 = 0;
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		writer.Write(length);
		writer.Write(length2);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				if (array[i, j] == 0L)
				{
					num++;
					continue;
				}
				if (num2 > 0)
				{
					writer.Write((byte)241);
					m_WriteInt(num2);
					num2 = 0;
				}
				if (num > 0)
				{
					writer.Write((byte)240);
					m_WriteInt(num);
					num = 0;
				}
				m_WriteInt(array[i, j]);
			}
			if (num == length2)
			{
				num2++;
			}
			else
			{
				writer.Write((byte)224);
			}
			num = 0;
		}
		writer.Write(byte.MaxValue);
	}

	private void writeData(long[,,] array)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		int length3 = array.GetLength(2);
		writer.Write(length);
		writer.Write(length2);
		writer.Write(length3);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				for (int k = 0; k < length3; k++)
				{
					if (array[i, j, k] == 0L)
					{
						num++;
						continue;
					}
					if (num3 > 0)
					{
						writer.Write((byte)242);
						m_WriteInt(num3);
						num3 = 0;
					}
					if (num2 > 0)
					{
						writer.Write((byte)241);
						m_WriteInt(num2);
						num2 = 0;
					}
					if (num > 0)
					{
						writer.Write((byte)240);
						m_WriteInt(num);
						num = 0;
					}
					m_WriteInt(array[i, j, k]);
				}
				if (num == length3)
				{
					num2++;
				}
				else
				{
					writer.Write((byte)224);
				}
				num = 0;
			}
			if (num2 == length2)
			{
				num3++;
			}
			else
			{
				writer.Write((byte)225);
			}
			num2 = 0;
		}
		writer.Write(byte.MaxValue);
	}

	private void writeData(string v)
	{
		if (v != null)
		{
			writer.Write(v);
		}
		else
		{
			writer.Write("");
		}
	}

	private void writeData(string[] array)
	{
		int num = 0;
		writer.Write(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == null || array[i].Length == 0)
			{
				num++;
				continue;
			}
			if (num > 0)
			{
				writer.Write((byte)240);
				m_WriteInt(num);
				num = 0;
			}
			writer.Write((byte)216);
			writer.Write(array[i]);
		}
		writer.Write(byte.MaxValue);
	}

	private void writeData(string[,] array)
	{
		int num = 0;
		int num2 = 0;
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		writer.Write(length);
		writer.Write(length2);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				if (array[i, j] == null || array[i, j].Length == 0)
				{
					num++;
					continue;
				}
				if (num2 > 0)
				{
					writer.Write((byte)241);
					m_WriteInt(num2);
					num2 = 0;
				}
				if (num > 0)
				{
					writer.Write((byte)240);
					m_WriteInt(num);
					num = 0;
				}
				writer.Write((byte)216);
				writer.Write(array[i, j]);
			}
			if (num == length2)
			{
				num2++;
			}
			else
			{
				writer.Write((byte)224);
			}
			num = 0;
		}
		writer.Write(byte.MaxValue);
	}

	private void writeData(string[,,] array)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int length = array.GetLength(0);
		int length2 = array.GetLength(1);
		int length3 = array.GetLength(2);
		writer.Write(length);
		writer.Write(length2);
		writer.Write(length3);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				for (int k = 0; k < length3; k++)
				{
					if (array[i, j, k] == null || array[i, j, k].Length == 0)
					{
						num++;
						continue;
					}
					if (num3 > 0)
					{
						writer.Write((byte)242);
						m_WriteInt(num3);
						num3 = 0;
					}
					if (num2 > 0)
					{
						writer.Write((byte)241);
						m_WriteInt(num2);
						num2 = 0;
					}
					if (num > 0)
					{
						writer.Write((byte)240);
						m_WriteInt(num);
						num = 0;
					}
					writer.Write((byte)216);
					writer.Write(array[i, j, k]);
				}
				if (num == length3)
				{
					num2++;
				}
				else
				{
					writer.Write((byte)224);
				}
				num = 0;
			}
			if (num2 == length2)
			{
				num3++;
			}
			else
			{
				writer.Write((byte)225);
			}
			num2 = 0;
		}
		writer.Write(byte.MaxValue);
	}

	public void Dispose()
	{
		if (writer != null)
		{
			writer.Close();
		}
		writer = null;
	}

	public void Close()
	{
		Dispose();
	}
}
