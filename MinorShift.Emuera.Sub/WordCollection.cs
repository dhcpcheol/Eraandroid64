using System.Collections.Generic;

namespace MinorShift.Emuera.Sub;

internal sealed class WordCollection
{
	public List<Word> Collection = new List<Word>();

	public int Pointer;

	private static Word nullToken = new NullWord();

	public Word Current
	{
		get
		{
			if (Pointer >= Collection.Count)
			{
				return nullToken;
			}
			return Collection[Pointer];
		}
	}

	public bool EOL => Pointer >= Collection.Count;

	public void Add(Word token)
	{
		Collection.Add(token);
	}

	public void Add(WordCollection wc)
	{
		Collection.AddRange(wc.Collection);
	}

	public void Clear()
	{
		Collection.Clear();
	}

	public void ShiftNext()
	{
		Pointer++;
	}

	public void Insert(Word w)
	{
		Collection.Insert(Pointer, w);
	}

	public void InsertRange(WordCollection wc)
	{
		Collection.InsertRange(Pointer, wc.Collection);
	}

	public void Remove()
	{
		Collection.RemoveAt(Pointer);
	}

	public void SetIsMacro()
	{
		foreach (Word item in Collection)
		{
			item.SetIsMacro();
		}
	}

	public WordCollection Clone()
	{
		WordCollection wordCollection = new WordCollection();
		for (int i = 0; i < Collection.Count; i++)
		{
			wordCollection.Collection.Add(Collection[i]);
		}
		return wordCollection;
	}

	public WordCollection Clone(int start, int count)
	{
		WordCollection wordCollection = new WordCollection();
		if (start > Collection.Count)
		{
			return wordCollection;
		}
		int num = start + count;
		if (num > Collection.Count)
		{
			num = Collection.Count;
		}
		for (int i = start; i < num; i++)
		{
			wordCollection.Collection.Add(Collection[i]);
		}
		return wordCollection;
	}
}
