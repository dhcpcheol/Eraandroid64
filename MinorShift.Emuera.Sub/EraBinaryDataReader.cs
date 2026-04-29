using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MinorShift.Emuera.Sub;

internal abstract class EraBinaryDataReader : IDisposable
{
	private sealed class EraBinaryDataReader1808 : EraBinaryDataReader
	{
		public override int ReaderVersion => 1808;

		public EraBinaryDataReader1808(BinaryReader stream, int ver, uint[] buf)
			: base(stream, ver, buf)
		{
		}

		public override EraSaveFileType ReadFileType()
		{
			byte b = reader.ReadByte();
			if (b >= 0 && b <= 3)
			{
				return (EraSaveFileType)b;
			}
			throw new FileEE("ファイルデータ型異常");
		}

		private long m_ReadInt()
		{
			byte b = reader.ReadByte();
			if (b <= 207)
			{
				return b;
			}
			return b switch
			{
				208 => reader.ReadInt16(), 
				209 => reader.ReadInt32(), 
				210 => reader.ReadInt64(), 
				_ => throw new FileEE("バイナリデータの異常"), 
			};
		}

		public override long ReadInt64()
		{
			return reader.ReadInt64();
		}

		public override KeyValuePair<string, EraSaveDataType> ReadVariableCode()
		{
			EraSaveDataType eraSaveDataType = (EraSaveDataType)reader.ReadByte();
			if (eraSaveDataType == EraSaveDataType.EOC || eraSaveDataType == EraSaveDataType.EOF || eraSaveDataType == EraSaveDataType.Separator)
			{
				return new KeyValuePair<string, EraSaveDataType>(null, eraSaveDataType);
			}
			return new KeyValuePair<string, EraSaveDataType>(reader.ReadString(), eraSaveDataType);
		}

		public override long ReadInt()
		{
			return m_ReadInt();
		}

		public override string ReadString()
		{
			return reader.ReadString();
		}

		public override void ReadIntArray(long[] refArray, bool needInit)
		{
			long[] array = null;
			int i = 0;
			int num = reader.ReadInt32();
			if (refArray == null)
			{
				refArray = new long[num];
			}
			int num2 = refArray.Length;
			if (num2 < num)
			{
				array = refArray;
				refArray = new long[Math.Max(num2, num)];
				num2 = Math.Min(num2, num);
			}
			while (true)
			{
				byte b = reader.ReadByte();
				if (b == byte.MaxValue)
				{
					break;
				}
				if (b == 240)
				{
					int num3 = (int)m_ReadInt();
					if (needInit)
					{
						for (int j = 0; j < num3; j++)
						{
							refArray[i + j] = 0L;
						}
					}
					i += num3;
					continue;
				}
				if (b <= 207)
				{
					refArray[i] = b;
				}
				else
				{
					switch (b)
					{
					case 208:
						refArray[i] = reader.ReadInt16();
						break;
					case 209:
						refArray[i] = reader.ReadInt32();
						break;
					case 210:
						refArray[i] = reader.ReadInt64();
						break;
					default:
						throw new FileEE("バイナリデータの異常");
					}
				}
				i++;
			}
			if (needInit)
			{
				for (; i < num2; i++)
				{
					refArray[i] = 0L;
				}
			}
			if (array != null)
			{
				for (i = 0; i < num2; i++)
				{
					array[i] = refArray[i];
				}
			}
		}

		public override void ReadIntArray2D(long[,] refArray, bool needInit)
		{
			long[,] array = null;
			int i = 0;
			int j = 0;
			int num = reader.ReadInt32();
			int num2 = reader.ReadInt32();
			if (refArray == null)
			{
				refArray = new long[num, num2];
			}
			int num3 = refArray.GetLength(0);
			int num4 = refArray.GetLength(1);
			if (num3 < num || num4 < num2)
			{
				array = refArray;
				refArray = new long[Math.Max(num3, num), Math.Max(num4, num2)];
				num3 = Math.Min(num3, num);
				num4 = Math.Min(num4, num2);
			}
			while (true)
			{
				byte b = reader.ReadByte();
				if (b == byte.MaxValue)
				{
					break;
				}
				if (b == 241)
				{
					int num5 = (int)m_ReadInt();
					if (needInit)
					{
						for (int k = 0; k < num5; k++)
						{
							for (j = 0; j < num4; j++)
							{
								refArray[i + k, j] = 0L;
							}
						}
					}
					i += num5;
					j = 0;
					continue;
				}
				if (b == 224)
				{
					if (needInit)
					{
						for (; j < num4; j++)
						{
							refArray[i, j] = 0L;
						}
					}
					i++;
					j = 0;
					continue;
				}
				if (b == 240)
				{
					int num6 = (int)m_ReadInt();
					if (needInit)
					{
						for (int l = 0; l < num6; l++)
						{
							refArray[i, j + l] = 0L;
						}
					}
					j += num6;
					continue;
				}
				if (b <= 207)
				{
					refArray[i, j] = b;
				}
				else
				{
					switch (b)
					{
					case 208:
						refArray[i, j] = reader.ReadInt16();
						break;
					case 209:
						refArray[i, j] = reader.ReadInt32();
						break;
					case 210:
						refArray[i, j] = reader.ReadInt64();
						break;
					default:
						throw new FileEE("バイナリデータの異常");
					}
				}
				j++;
			}
			if (needInit)
			{
				for (; i < num3; i++)
				{
					for (; j < num4; j++)
					{
						refArray[i, j] = 0L;
					}
					j = 0;
				}
			}
			if (array == null)
			{
				return;
			}
			for (i = 0; i < num3; i++)
			{
				for (j = 0; j < num4; j++)
				{
					array[i, j] = refArray[i, j];
				}
			}
		}

		public override void ReadIntArray3D(long[,,] refArray, bool needInit)
		{
			long[,,] array = null;
			int i = 0;
			int j = 0;
			int k = 0;
			int num = reader.ReadInt32();
			int num2 = reader.ReadInt32();
			int num3 = reader.ReadInt32();
			if (refArray == null)
			{
				refArray = new long[num, num2, num3];
			}
			int num4 = refArray.GetLength(0);
			int num5 = refArray.GetLength(1);
			int num6 = refArray.GetLength(2);
			if (num4 < num || num5 < num2 || num6 < num3)
			{
				array = refArray;
				refArray = new long[Math.Max(num4, num), Math.Max(num5, num2), Math.Max(num6, num3)];
				num4 = Math.Min(num4, num);
				num5 = Math.Min(num5, num2);
				num6 = Math.Min(num6, num3);
			}
			while (true)
			{
				byte b = reader.ReadByte();
				if (b == byte.MaxValue)
				{
					break;
				}
				if (b == 242)
				{
					int num7 = (int)m_ReadInt();
					if (needInit)
					{
						for (int l = 0; l < num7; l++)
						{
							for (j = 0; j < num5; j++)
							{
								for (k = 0; k < num6; k++)
								{
									refArray[i + l, j, k] = 0L;
								}
							}
						}
					}
					i += num7;
					j = 0;
					k = 0;
					continue;
				}
				if (b == 225)
				{
					if (needInit)
					{
						for (; j < num5; j++)
						{
							for (; k < num6; k++)
							{
								refArray[i, j, k] = 0L;
							}
							k = 0;
						}
					}
					i++;
					j = 0;
					k = 0;
					continue;
				}
				if (b == 241)
				{
					int num8 = (int)m_ReadInt();
					if (needInit)
					{
						for (int m = 0; m < num8; m++)
						{
							for (k = 0; k < num6; k++)
							{
								refArray[i, j + m, k] = 0L;
							}
						}
					}
					j += num8;
					k = 0;
					continue;
				}
				if (b == 224)
				{
					if (needInit)
					{
						for (; k < num6; k++)
						{
							refArray[i, j, k] = 0L;
						}
					}
					j++;
					k = 0;
					continue;
				}
				if (b == 240)
				{
					int num9 = (int)m_ReadInt();
					if (needInit)
					{
						for (int n = 0; n < num9; n++)
						{
							refArray[i, j, k + n] = 0L;
						}
					}
					k += num9;
					continue;
				}
				if (b <= 207)
				{
					refArray[i, j, k] = b;
				}
				else
				{
					switch (b)
					{
					case 208:
						refArray[i, j, k] = reader.ReadInt16();
						break;
					case 209:
						refArray[i, j, k] = reader.ReadInt32();
						break;
					case 210:
						refArray[i, j, k] = reader.ReadInt64();
						break;
					default:
						throw new FileEE("バイナリデータの異常");
					}
				}
				k++;
			}
			if (needInit)
			{
				for (; i < num4; i++)
				{
					for (; j < num5; j++)
					{
						for (; k < num6; k++)
						{
							refArray[i, j, k] = 0L;
						}
						k = 0;
					}
					j = 0;
				}
			}
			if (array == null)
			{
				return;
			}
			for (i = 0; i < num4; i++)
			{
				for (j = 0; j < num5; j++)
				{
					for (k = 0; k < num6; k++)
					{
						array[i, j, k] = refArray[i, j, k];
					}
				}
			}
		}

		public override void ReadStrArray(string[] refArray, bool needInit)
		{
			string[] array = null;
			int i = 0;
			int num = reader.ReadInt32();
			if (refArray == null)
			{
				refArray = new string[num];
			}
			int num2 = refArray.Length;
			if (num2 < num)
			{
				array = refArray;
				refArray = new string[Math.Max(num2, num)];
				num2 = Math.Min(num2, num);
			}
			while (true)
			{
				switch (reader.ReadByte())
				{
				case 240:
				{
					int num3 = (int)m_ReadInt();
					if (needInit)
					{
						for (int j = 0; j < num3; j++)
						{
							refArray[i + j] = null;
						}
					}
					i += num3;
					break;
				}
				case 216:
					refArray[i] = ReadString();
					i++;
					break;
				default:
					throw new FileEE("バイナリデータの異常");
				case byte.MaxValue:
					if (needInit)
					{
						for (; i < num2; i++)
						{
							refArray[i] = null;
						}
					}
					if (array != null)
					{
						for (i = 0; i < num2; i++)
						{
							array[i] = refArray[i];
						}
					}
					return;
				}
			}
		}

		public override void ReadStrArray2D(string[,] refArray, bool needInit)
		{
			string[,] array = null;
			int i = 0;
			int j = 0;
			int num = reader.ReadInt32();
			int num2 = reader.ReadInt32();
			if (refArray == null)
			{
				refArray = new string[num, num2];
			}
			int num3 = refArray.GetLength(0);
			int num4 = refArray.GetLength(1);
			if (num3 < num || num4 < num2)
			{
				array = refArray;
				refArray = new string[Math.Max(num3, num), Math.Max(num4, num2)];
				num3 = Math.Min(num3, num);
				num4 = Math.Min(num4, num2);
			}
			while (true)
			{
				switch (reader.ReadByte())
				{
				case 241:
				{
					int num6 = (int)m_ReadInt();
					if (needInit)
					{
						for (int l = 0; l < num6; l++)
						{
							for (j = 0; j < num4; j++)
							{
								refArray[i + l, j] = null;
							}
						}
					}
					i += num6;
					j = 0;
					break;
				}
				case 224:
					if (needInit)
					{
						for (; j < num4; j++)
						{
							refArray[i, j] = null;
						}
					}
					i++;
					j = 0;
					break;
				case 240:
				{
					int num5 = (int)m_ReadInt();
					if (needInit)
					{
						for (int k = 0; k < num5; k++)
						{
							refArray[i, j + k] = null;
						}
					}
					j += num5;
					break;
				}
				case 216:
					refArray[i, j] = ReadString();
					j++;
					break;
				default:
					throw new FileEE("バイナリデータの異常");
				case byte.MaxValue:
					if (needInit)
					{
						for (; i < num3; i++)
						{
							for (; j < num4; j++)
							{
								refArray[i, j] = null;
							}
							j = 0;
						}
					}
					if (array == null)
					{
						return;
					}
					for (i = 0; i < num3; i++)
					{
						for (j = 0; j < num4; j++)
						{
							array[i, j] = refArray[i, j];
						}
					}
					return;
				}
			}
		}

		public override void ReadStrArray3D(string[,,] refArray, bool needInit)
		{
			string[,,] array = null;
			int i = 0;
			int j = 0;
			int k = 0;
			int num = reader.ReadInt32();
			int num2 = reader.ReadInt32();
			int num3 = reader.ReadInt32();
			if (refArray == null)
			{
				refArray = new string[num, num2, num3];
			}
			int num4 = refArray.GetLength(0);
			int num5 = refArray.GetLength(1);
			int num6 = refArray.GetLength(2);
			if (num4 < num || num5 < num2 || num6 < num3)
			{
				array = refArray;
				refArray = new string[Math.Max(num4, num), Math.Max(num5, num2), Math.Max(num6, num3)];
				num4 = Math.Min(num4, num);
				num5 = Math.Min(num5, num2);
				num6 = Math.Min(num6, num3);
			}
			while (true)
			{
				switch (reader.ReadByte())
				{
				case 242:
				{
					int num8 = (int)m_ReadInt();
					if (needInit)
					{
						for (int m = 0; m < num8; m++)
						{
							for (j = 0; j < num5; j++)
							{
								for (k = 0; k < num6; k++)
								{
									refArray[i + m, j, k] = null;
								}
							}
						}
					}
					i += num8;
					j = 0;
					k = 0;
					break;
				}
				case 225:
					if (needInit)
					{
						for (; j < num5; j++)
						{
							for (; k < num6; k++)
							{
								refArray[i, j, k] = null;
							}
							k = 0;
						}
					}
					i++;
					j = 0;
					k = 0;
					break;
				case 241:
				{
					int num7 = (int)m_ReadInt();
					if (needInit)
					{
						for (int l = 0; l < num7; l++)
						{
							for (k = 0; k < num6; k++)
							{
								refArray[i, j + l, k] = null;
							}
						}
					}
					j += num7;
					k = 0;
					break;
				}
				case 224:
					if (needInit)
					{
						for (; k < num6; k++)
						{
							refArray[i, j, k] = null;
						}
					}
					j++;
					k = 0;
					break;
				case 240:
				{
					int num9 = (int)m_ReadInt();
					if (needInit)
					{
						for (int n = 0; n < num9; n++)
						{
							refArray[i, j, k + n] = null;
						}
					}
					k += num9;
					break;
				}
				case 216:
					refArray[i, j, k] = ReadString();
					k++;
					break;
				default:
					throw new FileEE("バイナリデータの異常");
				case byte.MaxValue:
					if (needInit)
					{
						for (; i < num4; i++)
						{
							for (; j < num5; j++)
							{
								for (; k < num6; k++)
								{
									refArray[i, j, k] = null;
								}
								k = 0;
							}
							j = 0;
						}
					}
					if (array == null)
					{
						return;
					}
					for (i = 0; i < num4; i++)
					{
						for (j = 0; j < num5; j++)
						{
							for (k = 0; k < num6; k++)
							{
								array[i, j, k] = refArray[i, j, k];
							}
						}
					}
					return;
				}
			}
		}
	}

	protected BinaryReader reader;

	protected readonly int version;

	protected readonly uint[] data;

	public abstract int ReaderVersion { get; }

	private EraBinaryDataReader()
	{
	}

	protected EraBinaryDataReader(BinaryReader stream, int ver, uint[] buf)
	{
		reader = stream;
		version = ver;
		data = buf;
	}

	public static EraBinaryDataReader CreateReader(FileStream fs)
	{
		try
		{
			if (fs == null || fs.Length < 16)
			{
				return null;
			}
			BinaryReader binaryReader = new BinaryReader(fs, Encoding.Unicode);
			if (binaryReader.ReadUInt64() != 727905341820519817L)
			{
				return null;
			}
			int num = (int)binaryReader.ReadUInt32();
			int num2 = (int)binaryReader.ReadUInt32();
			uint[] array = new uint[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = binaryReader.ReadUInt32();
			}
			if ((long)num == 1808)
			{
				return new EraBinaryDataReader1808(binaryReader, num, array);
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public abstract EraSaveFileType ReadFileType();

	public abstract long ReadInt64();

	public abstract string ReadString();

	public abstract long ReadInt();

	public abstract void ReadIntArray(long[] refArray, bool needInit);

	public abstract void ReadIntArray2D(long[,] refArray, bool needInit);

	public abstract void ReadIntArray3D(long[,,] refArray, bool needInit);

	public abstract void ReadStrArray(string[] refArray, bool needInit);

	public abstract void ReadStrArray2D(string[,] refArray, bool needInit);

	public abstract void ReadStrArray3D(string[,,] refArray, bool needInit);

	public abstract KeyValuePair<string, EraSaveDataType> ReadVariableCode();

	public void Dispose()
	{
		if (reader != null)
		{
			reader.Close();
		}
		reader = null;
	}

	public void Close()
	{
		Dispose();
	}
}
