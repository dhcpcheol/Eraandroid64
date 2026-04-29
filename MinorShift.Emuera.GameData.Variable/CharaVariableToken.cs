using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal abstract class CharaVariableToken : VariableToken
{
	protected int[] sizes;

	protected int totalSize;

	protected CharaVariableToken(VariableCode varCode, VariableData varData)
		: base(varCode, varData)
	{
		sizes = CharacterData.CharacterVarLength(varCode, varData.Constant);
		if (sizes != null)
		{
			totalSize = 1;
			for (int i = 0; i < sizes.Length; i++)
			{
				totalSize *= sizes[i];
			}
			base.IsForbid = totalSize == 0;
		}
		base.IsPrivate = false;
		base.CanRestructure = false;
	}

	public override int GetLength()
	{
		if (sizes.Length == 1)
		{
			return sizes[0];
		}
		if (sizes.Length == 0)
		{
			throw new CodeEE("非配列型のキャラ変数" + varName + "の長さを取得しようとしました");
		}
		throw new CodeEE(base.Dimension + "次元配列型のキャラ変数" + varName + "の長さを次元を指定せずに取得しようとしました");
	}

	public override int GetLength(int dimension)
	{
		if (sizes.Length == 0)
		{
			throw new CodeEE("非配列型のキャラ変数" + varName + "の長さを取得しようとしました");
		}
		if (dimension < sizes.Length)
		{
			return sizes[dimension];
		}
		throw new CodeEE("配列型変数のキャラ変数" + varName + "の存在しない次元の長さを取得しようとしました");
	}

	public override void CheckElement(long[] arguments, bool[] doCheck)
	{
		if (doCheck[0] && (arguments[0] < 0 || arguments[0] >= varData.CharacterList.Count))
		{
			throw new CodeEE("キャラクタ配列変数" + varName + "の第１引数(" + arguments[0] + ")はキャラ登録番号の範囲外です");
		}
		if (doCheck.Length > 1 && sizes.Length != 0 && doCheck[1] && (arguments[1] < 0 || arguments[1] >= sizes[0]))
		{
			throw new CodeEE("キャラクタ配列変数" + varName + "の第２引数(" + arguments[1] + ")は配列の範囲外です");
		}
		if (doCheck.Length > 2 && sizes.Length > 1 && doCheck[2] && (arguments[2] < 0 || arguments[2] >= sizes[1]))
		{
			throw new CodeEE("キャラクタ配列変数" + varName + "の第３引数(" + arguments[2] + ")は配列の範囲外です");
		}
	}

	public override void IsArrayRangeValid(long[] arguments, long index1, long index2, string funcName, long i1, long i2)
	{
		CheckElement(arguments);
		_ = varData.CharacterList[(int)arguments[0]];
		if (index1 < 0 || index1 > sizes[0])
		{
			throw new CodeEE(funcName + "命令の第" + i1 + "引数(" + index1 + ")は配列" + varName + "の範囲外です");
		}
		if (index2 < 0 || index2 > sizes[0])
		{
			throw new CodeEE(funcName + "命令の第" + i2 + "引数(" + index2 + ")は配列" + varName + "の範囲外です");
		}
	}
}
