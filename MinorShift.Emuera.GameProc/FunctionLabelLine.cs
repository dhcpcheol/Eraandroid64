using System;
using System.Collections.Generic;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc;

internal class FunctionLabelLine : LogicalLine, IComparable<FunctionLabelLine>
{
	private WordCollection wc;

	private Dictionary<string, UserDefinedVariableToken> privateVar = new Dictionary<string, UserDefinedVariableToken>();

	public string LabelName { get; protected set; }

	public bool IsEvent { get; set; }

	public bool IsSystem { get; set; }

	public bool IsSingle { get; set; }

	public bool IsPri { get; set; }

	public bool IsLater { get; set; }

	public bool IsOnly { get; set; }

	public bool hasPrivDynamicVar { get; set; }

	public int LocalLength { get; set; }

	public int LocalsLength { get; set; }

	public int ArgLength { get; set; }

	public int ArgsLength { get; set; }

	public bool IsMethod { get; set; }

	public Type MethodType { get; set; }

	public VariableTerm[] Arg { get; set; }

	public SingleTerm[] Def { get; set; }

	public int Depth { get; set; }

	public int Index { get; set; }

	public int FileIndex { get; set; }

	protected FunctionLabelLine()
	{
	}

	public FunctionLabelLine(ScriptPosition thePosition, string labelname, WordCollection wc)
	{
		position = thePosition;
		LabelName = labelname;
		IsSingle = false;
		hasPrivDynamicVar = false;
		Index = -1;
		Depth = -1;
		LocalLength = 0;
		LocalsLength = 0;
		ArgLength = 0;
		ArgsLength = 0;
		IsMethod = false;
		MethodType = typeof(void);
		this.wc = wc;
	}

	public WordCollection PopRowArgs()
	{
		WordCollection result = wc;
		wc = null;
		return result;
	}

	public int CompareTo(FunctionLabelLine other)
	{
		if (FileIndex != other.FileIndex)
		{
			return FileIndex.CompareTo(other.FileIndex);
		}
		if (position.LineNo != other.position.LineNo)
		{
			return position.LineNo.CompareTo(other.position.LineNo);
		}
		return Index.CompareTo(other.Index);
	}

	internal bool AddPrivateVariable(UserDefinedVariableData data)
	{
		if (privateVar.ContainsKey(data.Name))
		{
			return false;
		}
		UserDefinedVariableToken value = GlobalStatic.VariableData.CreatePrivateVariable(data);
		privateVar.Add(data.Name, value);
		if (!data.Static)
		{
			hasPrivDynamicVar = true;
		}
		return true;
	}

	internal UserDefinedVariableToken GetPrivateVariable(string key)
	{
		UserDefinedVariableToken value = null;
		privateVar.TryGetValue(key, out value);
		return value;
	}

	internal void In()
	{
		foreach (UserDefinedVariableToken value in privateVar.Values)
		{
			if (!value.IsStatic)
			{
				value.In();
			}
		}
	}

	internal void Out()
	{
		foreach (UserDefinedVariableToken value in privateVar.Values)
		{
			if (!value.IsStatic)
			{
				value.Out();
			}
		}
	}
}
