using System;
using System.IO;

namespace MinorShift.Emuera.Sub;

internal sealed class StringStream
{
	private string source;

	public const char EndOfString = '\0';

	private int pointer;

	public string RowString => source;

	public int CurrentPosition
	{
		get
		{
			return pointer;
		}
		set
		{
			pointer = value;
		}
	}

	public char Current
	{
		get
		{
			if (pointer >= source.Length)
			{
				return '\0';
			}
			return source[pointer];
		}
	}

	public bool EOS => pointer >= source.Length;

	public char Next
	{
		get
		{
			if (pointer + 1 >= source.Length)
			{
				return '\0';
			}
			return source[pointer + 1];
		}
	}

	public StringStream(string s)
	{
		source = s;
		if (source == null)
		{
			source = "";
		}
		pointer = 0;
	}

	public void AppendString(string str)
	{
		if (pointer > source.Length)
		{
			pointer = source.Length;
		}
		source = source + " " + str;
	}

	public string Substring()
	{
		if (pointer >= source.Length)
		{
			return "";
		}
		if (pointer == 0)
		{
			return source;
		}
		return source.Substring(pointer);
	}

	public string Substring(int start, int length)
	{
		if (start >= source.Length || length == 0)
		{
			return "";
		}
		if (start + length > source.Length)
		{
			length = source.Length - start;
		}
		return source.Substring(start, length);
	}

	internal void Replace(int start, int count, string src)
	{
		source = source.Remove(start, count).Insert(start, src);
		pointer = start;
	}

	public void ShiftNext()
	{
		pointer++;
	}

	public void Jump(int skip)
	{
		pointer += skip;
	}

	public int Find(string str)
	{
		return source.IndexOf(str, pointer) - pointer;
	}

	public int Find(char c)
	{
		return source.IndexOf(c, pointer) - pointer;
	}

	public override string ToString()
	{
		if (source == null)
		{
			return "";
		}
		return source;
	}

	public bool CurrentEqualTo(string rother)
	{
		if (pointer + rother.Length > source.Length)
		{
			return false;
		}
		for (int i = 0; i < rother.Length; i++)
		{
			if (source[pointer + i] != rother[i])
			{
				return false;
			}
		}
		return true;
	}

	public bool TripleSymbol()
	{
		if (pointer + 3 > source.Length)
		{
			return false;
		}
		if (source[pointer] == source[pointer + 1])
		{
			return source[pointer] == source[pointer + 2];
		}
		return false;
	}

	public bool CurrentEqualTo(string rother, StringComparison comp)
	{
		if (pointer + rother.Length > source.Length)
		{
			return false;
		}
		return source.Substring(pointer, rother.Length).Equals(rother, comp);
	}

	public void Seek(int offset, SeekOrigin origin)
	{
		switch (origin)
		{
		case SeekOrigin.Begin:
			pointer = offset;
			break;
		case SeekOrigin.Current:
			pointer += offset;
			break;
		case SeekOrigin.End:
			pointer = source.Length + offset;
			break;
		}
		if (pointer < 0)
		{
			pointer = 0;
		}
	}
}
