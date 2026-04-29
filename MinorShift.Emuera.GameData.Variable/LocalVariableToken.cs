using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal abstract class LocalVariableToken : VariableToken
{
	protected string subID;

	protected int size;

	public LocalVariableToken(VariableCode varCode, VariableData varData, string subId, int size)
		: base(varCode, varData)
	{
		base.CanRestructure = false;
		subID = subId;
		this.size = size;
	}

	public abstract void SetDefault();

	public abstract void resize(int newSize);

	public override int GetLength()
	{
		return size;
	}

	public override int GetLength(int dimension)
	{
		if (dimension == 0)
		{
			return size;
		}
		throw new CodeEE("配列型変数" + varName + "の存在しない次元の長さを取得しようとしました");
	}

	public override void CheckElement(long[] arguments, bool[] doCheck)
	{
		if (doCheck[0] && (arguments[0] < 0 || arguments[0] >= size))
		{
			throw new CodeEE("配列変数" + varName + "の第１引数(" + arguments[0] + ")は配列の範囲外です");
		}
	}

	public override void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
	{
		CheckElement(arguments);
		if (index1 < 0 || index1 > size)
		{
			throw new CodeEE(funcName + "命令の第" + i1 + "引数(" + index1 + ")は配列" + varName + "の範囲外です");
		}
		if (index2 < 0 || index2 > size)
		{
			throw new CodeEE(funcName + "命令の第" + i2 + "引数(" + index2 + ")は配列" + varName + "の範囲外です");
		}
	}
}
