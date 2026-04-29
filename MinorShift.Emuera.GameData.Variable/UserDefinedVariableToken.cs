using MinorShift.Emuera.GameProc;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal abstract class UserDefinedVariableToken : VariableToken
{
	protected bool isConst;

	protected int[] sizes;

	protected int totalSize;

	public override bool IsConst => isConst;

	public bool IsStatic { get; protected set; }

	protected UserDefinedVariableToken(VariableCode varCode, UserDefinedVariableData data)
		: base(varCode, null)
	{
		varName = data.Name;
		base.IsPrivate = data.Private;
		isConst = data.Const;
		sizes = data.Lengths;
		base.IsGlobal = data.Global;
		base.IsSavedata = data.Save;
		totalSize = 1;
		for (int i = 0; i < sizes.Length; i++)
		{
			totalSize *= sizes[i];
		}
		base.IsForbid = totalSize == 0;
		base.CanRestructure = isConst;
	}

	public abstract void SetDefault();

	public override int GetLength()
	{
		if (base.Dimension == 1)
		{
			return sizes[0];
		}
		throw new CodeEE(base.Dimension + "次元配列型変数" + varName + "の長さを取得しようとしました");
	}

	public override int GetLength(int dimension)
	{
		if (dimension < base.Dimension)
		{
			return sizes[dimension];
		}
		throw new CodeEE("配列型変数" + varName + "の存在しない次元の長さを取得しようとしました");
	}

	public override void CheckElement(long[] arguments, bool[] doCheck)
	{
		if (doCheck[0] && (arguments[0] < 0 || arguments[0] >= sizes[0]))
		{
			throw new CodeEE("配列型変数" + varName + "の第１引数(" + arguments[0] + ")は配列の範囲外です");
		}
		if (sizes.Length >= 2 && (arguments[1] < 0 || arguments[1] >= sizes[1]))
		{
			throw new CodeEE("配列型変数" + varName + "の第２引数(" + arguments[1] + ")は配列の範囲外です");
		}
		if (sizes.Length >= 3 && (arguments[2] < 0 || arguments[2] >= sizes[2]))
		{
			throw new CodeEE("配列型変数" + varName + "の第３引数(" + arguments[2] + ")は配列の範囲外です");
		}
	}

	public override void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
	{
		CheckElement(arguments);
		if (index1 < 0 || index1 > sizes[base.Dimension - 1])
		{
			throw new CodeEE(funcName + "命令の第" + i1 + "引数(" + index1 + ")は配列" + varName + "の範囲外です");
		}
		if (index2 < 0 || index2 > sizes[base.Dimension - 1])
		{
			throw new CodeEE(funcName + "命令の第" + i2 + "引数(" + index2 + ")は配列" + varName + "の範囲外です");
		}
	}

	public abstract void In();

	public abstract void Out();
}
