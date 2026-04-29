using System.Collections.Generic;
using MinorShift.Emuera.GameProc;

namespace MinorShift.Emuera.GameData.Variable;

internal sealed class VariableLocal
{
	private readonly int size;

	private VariableCode varCode;

	private CreateLocalVariableToken creater;

	private Dictionary<string, LocalVariableToken> localVarTokens = new Dictionary<string, LocalVariableToken>();

	public bool IsForbid => size == 0;

	public VariableLocal(VariableCode varCode, int size, CreateLocalVariableToken creater)
	{
		this.size = size;
		this.varCode = varCode;
		this.creater = creater;
	}

	public LocalVariableToken GetExistLocalVariableToken(string subKey)
	{
		LocalVariableToken value = null;
		localVarTokens.TryGetValue(subKey, out value);
		return value;
	}

	public int GetDefaultSize()
	{
		return size;
	}

	public LocalVariableToken GetNewLocalVariableToken(string subKey, FunctionLabelLine func)
	{
		LocalVariableToken localVariableToken = null;
		int num = 0;
		if (varCode == VariableCode.LOCAL)
		{
			num = func.LocalLength;
		}
		else if (varCode == VariableCode.LOCALS)
		{
			num = func.LocalsLength;
		}
		else if (varCode == VariableCode.ARG)
		{
			num = func.ArgLength;
		}
		else if (varCode == VariableCode.ARGS)
		{
			num = func.ArgsLength;
		}
		if (num > 0)
		{
			if (num < size && (varCode == VariableCode.ARG || varCode == VariableCode.ARGS))
			{
				num = size;
			}
			localVariableToken = creater(varCode, subKey, num);
		}
		else if (num == 0)
		{
			localVariableToken = creater(varCode, subKey, size);
		}
		else
		{
			localVariableToken = creater(varCode, subKey, size);
			LogicalLine scaningLine = GlobalStatic.Process.GetScaningLine();
			if (scaningLine != null)
			{
				if (!func.IsSystem)
				{
					ParserMediator.Warn(string.Concat("関数宣言に引数変数\"", varCode, "\"が使われていない関数中で\"", varCode, "\"が使われています(関数の引数以外の用途に使うことは推奨されません。代わりに#DIMの使用を検討してください)"), scaningLine, 1, isError: false, isBackComp: false);
				}
				else
				{
					ParserMediator.Warn(string.Concat("システム関数", func.LabelName, "中で\"", varCode, "\"が使われています(関数の引数以外の用途に使うことは推奨されません。代わりに#DIMの使用を検討してください)"), scaningLine, 1, isError: false, isBackComp: false);
				}
			}
		}
		localVarTokens.Add(subKey, localVariableToken);
		return localVariableToken;
	}

	public void ResizeLocalVariableToken(string subKey, int newSize)
	{
		LocalVariableToken value = null;
		if (localVarTokens.TryGetValue(subKey, out value))
		{
			if (size < newSize)
			{
				value.resize(newSize);
			}
			else
			{
				value.resize(size);
			}
			return;
		}
		if (newSize > size)
		{
			value = creater(varCode, subKey, newSize);
		}
		else
		{
			if (newSize != 0)
			{
				return;
			}
			value = creater(varCode, subKey, size);
		}
		localVarTokens.Add(subKey, value);
	}

	public void Clear()
	{
		localVarTokens.Clear();
	}

	public void SetDefault()
	{
		foreach (KeyValuePair<string, LocalVariableToken> localVarToken in localVarTokens)
		{
			localVarToken.Value.SetDefault();
		}
	}
}
