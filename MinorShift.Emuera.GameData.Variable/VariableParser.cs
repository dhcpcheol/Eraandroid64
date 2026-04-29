using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal static class VariableParser
{
	public static SingleTerm ZeroTerm { get; private set; }

	public static VariableTerm TARGET { get; private set; }

	public static void Initialize()
	{
		ZeroTerm = new SingleTerm(0L);
		IOperandTerm[] args = new IOperandTerm[1] { ZeroTerm };
		TARGET = new VariableTerm(GlobalStatic.VariableData.GetSystemVariableToken("TARGET"), args);
	}

	public static bool IsVariable(string ids)
	{
		if (string.IsNullOrEmpty(ids))
		{
			return false;
		}
		string[] array = ids.Split(':');
		return GlobalStatic.IdentifierDictionary.GetVariableToken(array[0], null, allowPrivate: false) != null;
	}

	public static VariableTerm ReduceVariable(VariableToken id, WordCollection wc)
	{
		IOperandTerm operandTerm = null;
		IOperandTerm p = null;
		IOperandTerm p2 = null;
		IOperandTerm p3 = null;
		int num = 0;
		while (wc.Current.Type == ':')
		{
			if (num >= 3)
			{
				throw new CodeEE(id.Code.ToString() + "の引数が多すぎます");
			}
			wc.ShiftNext();
			operandTerm = ExpressionParser.ReduceVariableArgument(wc, id.Code);
			switch (num)
			{
			case 0:
				p = operandTerm;
				break;
			case 1:
				p2 = operandTerm;
				break;
			case 2:
				p3 = operandTerm;
				break;
			}
			num++;
		}
		return ReduceVariable(id, p, p2, p3);
	}

	public static VariableTerm ReduceVariable(VariableToken id, IOperandTerm p1, IOperandTerm p2, IOperandTerm p3)
	{
		IOperandTerm[] array = null;
		IOperandTerm operandTerm = p1;
		IOperandTerm operandTerm2 = p2;
		if (id.IsCharacterData)
		{
			if (id.IsArray2D)
			{
				if (operandTerm == null && operandTerm2 == null && p3 == null)
				{
					return new VariableNoArgTerm(id);
				}
				if (operandTerm == null || operandTerm2 == null || p3 == null)
				{
					throw new CodeEE("キャラクタ二次元配列変数" + id.Name + "の引数は省略できません");
				}
				array = new IOperandTerm[3] { operandTerm, operandTerm2, p3 };
			}
			else if (id.IsArray1D)
			{
				if (p3 != null)
				{
					throw new CodeEE("キャラクタ変数" + id.Name + "の引数が多すぎます");
				}
				if (operandTerm == null && operandTerm2 == null && p3 == null && Config.SystemNoTarget)
				{
					return new VariableNoArgTerm(id);
				}
				if (operandTerm2 == null)
				{
					if (Config.SystemNoTarget)
					{
						throw new CodeEE("キャラクタ配列変数" + id.Name + "の引数は省略できません(コンフィグにより禁止が選択されています)");
					}
					operandTerm2 = ((operandTerm != null) ? operandTerm : ZeroTerm);
					operandTerm = TARGET;
				}
				array = new IOperandTerm[2] { operandTerm, operandTerm2 };
			}
			else
			{
				if (operandTerm2 != null)
				{
					throw new CodeEE("キャラクタ変数" + id.Name + "の引数が多すぎます");
				}
				if (operandTerm == null && operandTerm2 == null && p3 == null && Config.SystemNoTarget)
				{
					return new VariableNoArgTerm(id);
				}
				if (operandTerm == null)
				{
					if (Config.SystemNoTarget)
					{
						throw new CodeEE("キャラクタ変数" + id.Name + "の引数は省略できません(コンフィグにより禁止が選択されています)");
					}
					operandTerm = TARGET;
				}
				array = new IOperandTerm[1] { operandTerm };
			}
		}
		else if (id.IsArray3D)
		{
			if (operandTerm == null && operandTerm2 == null && p3 == null)
			{
				return new VariableNoArgTerm(id);
			}
			if (operandTerm == null || operandTerm2 == null || p3 == null)
			{
				throw new CodeEE("三次元配列変数" + id.Name + "の引数は省略できません");
			}
			array = new IOperandTerm[3] { operandTerm, operandTerm2, p3 };
		}
		else if (id.IsArray2D)
		{
			if (operandTerm == null && operandTerm2 == null && p3 == null)
			{
				return new VariableNoArgTerm(id);
			}
			if (operandTerm == null || operandTerm2 == null)
			{
				throw new CodeEE("二次元配列変数" + id.Name + "の引数は省略できません");
			}
			if (p3 != null)
			{
				throw new CodeEE("二次元配列" + id.Name + "の引数が多すぎます");
			}
			array = new IOperandTerm[2] { operandTerm, operandTerm2 };
		}
		else if (id.IsArray1D)
		{
			if (operandTerm2 != null)
			{
				throw new CodeEE("一次元配列変数" + id.Name + "の引数が多すぎます");
			}
			if (operandTerm == null)
			{
				operandTerm = ZeroTerm;
				if (!Config.CompatiRAND && id.Code == VariableCode.RAND)
				{
					throw new CodeEE("RANDの引数が省略されています");
				}
			}
			if (!Config.CompatiRAND && operandTerm is SingleTerm && id.Code == VariableCode.RAND && ((SingleTerm)operandTerm).Int == 0L)
			{
				throw new CodeEE("RANDの引数に0が与えられています");
			}
			array = new IOperandTerm[1] { operandTerm };
		}
		else
		{
			if (operandTerm != null)
			{
				throw new CodeEE("配列でない変数" + id.Name + "を引数付きで呼び出しています");
			}
			array = new IOperandTerm[0];
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsString)
			{
				array[i] = new VariableStrArgTerm(id.Code, array[i], i);
			}
		}
		return new VariableTerm(id, array);
	}
}
