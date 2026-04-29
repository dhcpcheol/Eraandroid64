using System;
using System.Collections.Generic;

namespace MinorShift.Emuera.GameProc;

internal sealed class LabelDictionary
{
	private Dictionary<string, List<FunctionLabelLine>> labelAtDic = new Dictionary<string, List<FunctionLabelLine>>();

	private List<FunctionLabelLine> invalidList = new List<FunctionLabelLine>();

	private List<GotoLabelLine> labelDollarList = new List<GotoLabelLine>();

	private int count;

	private Dictionary<string, int> loadedFileDic = new Dictionary<string, int>();

	private int currentFileCount;

	private int totalFileCount;

	private Dictionary<string, List<FunctionLabelLine>[]> eventLabelDic = new Dictionary<string, List<FunctionLabelLine>[]>();

	private Dictionary<string, FunctionLabelLine> noneventLabelDic = new Dictionary<string, FunctionLabelLine>();

	public int Count => count;

	public bool Initialized { get; set; }

	public LabelDictionary()
	{
		Initialized = false;
	}

	public FunctionLabelLine GetSameNameLabel(FunctionLabelLine point)
	{
		string labelName = point.LabelName;
		if (!labelAtDic.ContainsKey(labelName))
		{
			return null;
		}
		if (point.IsError)
		{
			return null;
		}
		List<FunctionLabelLine> list = labelAtDic[labelName];
		if (list.Count <= 1)
		{
			return null;
		}
		return list[0];
	}

	public void SortLabels()
	{
		foreach (KeyValuePair<string, List<FunctionLabelLine>[]> item in eventLabelDic)
		{
			List<FunctionLabelLine>[] value = item.Value;
			for (int i = 0; i < value.Length; i++)
			{
				value[i].Clear();
			}
		}
		eventLabelDic.Clear();
		noneventLabelDic.Clear();
		foreach (KeyValuePair<string, List<FunctionLabelLine>> item2 in labelAtDic)
		{
			string key = item2.Key;
			List<FunctionLabelLine> value2 = item2.Value;
			if (value2.Count > 1)
			{
				value2.Sort();
			}
			if (!value2[0].IsEvent)
			{
				noneventLabelDic.Add(key, value2[0]);
				GlobalStatic.IdentifierDictionary.resizeLocalVars("ARG", value2[0].LabelName, value2[0].ArgLength);
				GlobalStatic.IdentifierDictionary.resizeLocalVars("ARGS", value2[0].LabelName, value2[0].ArgsLength);
				continue;
			}
			if (Config.CompatiCallEvent)
			{
				noneventLabelDic.Add(key, value2[0]);
			}
			List<FunctionLabelLine>[] array = new List<FunctionLabelLine>[4];
			List<FunctionLabelLine> list = new List<FunctionLabelLine>();
			List<FunctionLabelLine> list2 = new List<FunctionLabelLine>();
			List<FunctionLabelLine> list3 = new List<FunctionLabelLine>();
			List<FunctionLabelLine> list4 = new List<FunctionLabelLine>();
			int num = 0;
			int num2 = 0;
			for (int j = 0; j < value2.Count; j++)
			{
				if (value2[j].LocalLength > num)
				{
					num = value2[j].LocalLength;
				}
				if (value2[j].LocalsLength > num2)
				{
					num2 = value2[j].LocalsLength;
				}
				if (value2[j].IsOnly)
				{
					list.Add(value2[j]);
				}
				if (value2[j].IsPri)
				{
					list2.Add(value2[j]);
				}
				if (value2[j].IsLater)
				{
					list4.Add(value2[j]);
				}
				if (!value2[j].IsPri && !value2[j].IsLater)
				{
					list3.Add(value2[j]);
				}
			}
			if (num < GlobalStatic.IdentifierDictionary.getLocalDefaultSize("LOCAL"))
			{
				num = GlobalStatic.IdentifierDictionary.getLocalDefaultSize("LOCAL");
			}
			if (num2 < GlobalStatic.IdentifierDictionary.getLocalDefaultSize("LOCALS"))
			{
				num2 = GlobalStatic.IdentifierDictionary.getLocalDefaultSize("LOCALS");
			}
			array[0] = list;
			array[1] = list2;
			array[2] = list3;
			array[3] = list4;
			for (int k = 0; k < 4; k++)
			{
				for (int l = 0; l < array[k].Count; l++)
				{
					array[k][l].LocalLength = num;
					array[k][l].LocalsLength = num2;
				}
			}
			eventLabelDic.Add(key, array);
		}
	}

	public void RemoveAll()
	{
		Initialized = false;
		count = 0;
		foreach (KeyValuePair<string, List<FunctionLabelLine>[]> item in eventLabelDic)
		{
			List<FunctionLabelLine>[] value = item.Value;
			for (int i = 0; i < value.Length; i++)
			{
				value[i].Clear();
			}
		}
		eventLabelDic.Clear();
		noneventLabelDic.Clear();
		foreach (KeyValuePair<string, List<FunctionLabelLine>> item2 in labelAtDic)
		{
			item2.Value.Clear();
		}
		labelAtDic.Clear();
		labelDollarList.Clear();
		loadedFileDic.Clear();
		invalidList.Clear();
		currentFileCount = 0;
		totalFileCount = 0;
	}

	public void RemoveLabelWithPath(string fname)
	{
		List<FunctionLabelLine> list = new List<FunctionLabelLine>();
		List<string> list2 = new List<string>();
		foreach (KeyValuePair<string, List<FunctionLabelLine>> item in labelAtDic)
		{
			string key = item.Key;
			List<FunctionLabelLine> value = item.Value;
			foreach (FunctionLabelLine item2 in value)
			{
				if (string.Equals(item2.Position.Filename, fname, StringComparison.OrdinalIgnoreCase))
				{
					list.Add(item2);
				}
			}
			foreach (FunctionLabelLine item3 in list)
			{
				value.Remove(item3);
				if (value.Count == 0)
				{
					list2.Add(key);
				}
			}
			list.Clear();
		}
		foreach (string item4 in list2)
		{
			labelAtDic.Remove(item4);
		}
		for (int i = 0; i < invalidList.Count; i++)
		{
			if (string.Equals(invalidList[i].Position.Filename, fname, StringComparison.OrdinalIgnoreCase))
			{
				invalidList.RemoveAt(i);
				i--;
			}
		}
	}

	public void AddFilename(string filename)
	{
		lock (this)
		{
			int value = 0;
			if (loadedFileDic.TryGetValue(filename, out value))
			{
				currentFileCount = value;
				RemoveLabelWithPath(filename);
			}
			else
			{
				totalFileCount++;
				currentFileCount = totalFileCount;
				loadedFileDic.Add(filename, totalFileCount);
			}
		}
	}

	public void AddLabel(FunctionLabelLine point)
	{
		lock (this)
		{
			point.Index = count;
			point.FileIndex = currentFileCount;
			count++;
			string labelName = point.LabelName;
			if (labelAtDic.ContainsKey(labelName))
			{
				labelAtDic[labelName].Add(point);
				return;
			}
			List<FunctionLabelLine> list = new List<FunctionLabelLine>();
			list.Add(point);
			labelAtDic.Add(labelName, list);
		}
	}

	public bool AddLabelDollar(GotoLabelLine point)
	{
		lock (this)
		{
			string labelName = point.LabelName;
			foreach (GotoLabelLine labelDollar in labelDollarList)
			{
				if (labelDollar.LabelName == labelName && labelDollar.ParentLabelLine == point.ParentLabelLine)
				{
					return false;
				}
			}
			labelDollarList.Add(point);
			return true;
		}
	}

	public List<FunctionLabelLine>[] GetEventLabels(string key)
	{
		List<FunctionLabelLine>[] value = null;
		eventLabelDic.TryGetValue(key, out value);
		return value;
	}

	public FunctionLabelLine GetNonEventLabel(string key)
	{
		FunctionLabelLine value = null;
		noneventLabelDic.TryGetValue(key, out value);
		return value;
	}

	public List<FunctionLabelLine> GetAllLabels(bool getInvalidList)
	{
		List<FunctionLabelLine> list = new List<FunctionLabelLine>();
		foreach (List<FunctionLabelLine> value in labelAtDic.Values)
		{
			list.AddRange(value);
		}
		if (getInvalidList)
		{
			list.AddRange(invalidList);
		}
		return list;
	}

	internal IEnumerable<string> GetAllLabelName()
	{
		return labelAtDic.Keys;
	}

	public GotoLabelLine GetLabelDollar(string key, FunctionLabelLine labelAtLine)
	{
		foreach (GotoLabelLine labelDollar in labelDollarList)
		{
			if (labelDollar.LabelName == key && labelDollar.ParentLabelLine == labelAtLine)
			{
				return labelDollar;
			}
		}
		return null;
	}

	internal void AddInvalidLabel(FunctionLabelLine invalidLabelLine)
	{
		invalidList.Add(invalidLabelLine);
	}
}
