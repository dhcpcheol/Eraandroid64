using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MinorShift.Emuera.Sub;

internal sealed class EraStreamReader : IDisposable
{
	private string filepath;

	private string filename;

	private bool useRename;

	private int curNo;

	private int nextNo;

	private StreamReader reader;

	private FileStream stream;

	private bool disposed;

	public int LineNo => curNo;

	public string Filename => filename;

	public EraStreamReader(bool useRename)
	{
		this.useRename = useRename;
	}

	public bool Open(string path)
	{
		return Open(path, Path.GetFileName(path));
	}

	public bool Open(string path, string name)
	{
		filepath = path;
		filename = name;
		nextNo = 0;
		curNo = 0;
		try
		{
			stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			reader = new StreamReader(stream, Config.Encode);
		}
		catch
		{
			Dispose();
			return false;
		}
		return true;
	}

	public string ReadLine()
	{
		nextNo++;
		curNo = nextNo;
		return reader.ReadLine();
	}

	public StringStream ReadEnabledLine()
	{
		string text = null;
		StringStream stringStream = null;
		curNo = nextNo;
		while (true)
		{
			text = reader.ReadLine();
			curNo++;
			nextNo++;
			if (text == null)
			{
				return null;
			}
			if (text.Length == 0)
			{
				continue;
			}
			if (useRename && text.IndexOf("[[") >= 0 && text.IndexOf("]]") >= 0)
			{
				foreach (KeyValuePair<string, string> item in ParserMediator.RenameDic)
				{
					text = text.Replace(item.Key, item.Value);
				}
			}
			stringStream = new StringStream(text);
			LexicalAnalyzer.SkipWhiteSpace(stringStream);
			if (!stringStream.EOS)
			{
				break;
			}
		}
		if (stringStream.Current == '}')
		{
			throw new CodeEE("予期しない行連結終端記号'}'が見つかりました");
		}
		if (stringStream.Current == '{')
		{
			if (text.Trim() != "{")
			{
				throw new CodeEE("行連結始端記号'{'の行に'{'以外の文字を含めることはできません");
			}
			StringBuilder stringBuilder = new StringBuilder();
			while (true)
			{
				text = reader.ReadLine();
				nextNo++;
				if (text == null)
				{
					throw new CodeEE("行連結始端記号'{'が使われましたが終端記号'}'が見つかりません");
				}
				if (useRename && text.IndexOf("[[") >= 0 && text.IndexOf("]]") >= 0)
				{
					foreach (KeyValuePair<string, string> item2 in ParserMediator.RenameDic)
					{
						text = text.Replace(item2.Key, item2.Value);
					}
				}
				string text2 = text.TrimStart();
				if (text2.Length > 0)
				{
					if (text2[0] == '}')
					{
						if (!(text2.Trim() != "}"))
						{
							break;
						}
						throw new CodeEE("行連結終端記号'}'の行に'}'以外の文字を含めることはできません");
					}
					if (text2[0] == '{')
					{
						throw new CodeEE("予期しない行連結始端記号'{'が見つかりました");
					}
				}
				stringBuilder.Append(text);
				stringBuilder.Append(" ");
			}
			stringStream = new StringStream(stringBuilder.ToString());
			LexicalAnalyzer.SkipWhiteSpace(stringStream);
			return stringStream;
		}
		return stringStream;
	}

	public void Close()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (!disposed)
		{
			if (reader != null)
			{
				reader.Close();
			}
			else if (stream != null)
			{
				stream.Close();
			}
			filepath = null;
			filename = null;
			reader = null;
			stream = null;
			disposed = true;
		}
	}
}
