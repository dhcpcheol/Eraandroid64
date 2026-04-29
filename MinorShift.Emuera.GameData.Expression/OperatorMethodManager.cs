using System;
using System.Collections.Generic;
using System.Text;
using MinorShift.Emuera.GameData.Function;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Expression;

internal static class OperatorMethodManager
{
	private sealed class PlusIntInt : OperatorMethod
	{
		public PlusIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return arguments[0].GetIntValue(exm) + arguments[1].GetIntValue(exm);
		}
	}

	private sealed class PlusStrStr : OperatorMethod
	{
		public PlusStrStr()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[2]
			{
				typeof(string),
				typeof(string)
			};
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return arguments[0].GetStrValue(exm) + arguments[1].GetStrValue(exm);
		}
	}

	private sealed class MinusIntInt : OperatorMethod
	{
		public MinusIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return arguments[0].GetIntValue(exm) - arguments[1].GetIntValue(exm);
		}
	}

	private sealed class MultIntInt : OperatorMethod
	{
		public MultIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return arguments[0].GetIntValue(exm) * arguments[1].GetIntValue(exm);
		}
	}

	private sealed class MultStrInt : OperatorMethod
	{
		public MultStrInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(string);
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long num = 0L;
			string text = null;
			if (arguments[0].GetOperandType() == typeof(long))
			{
				num = arguments[0].GetIntValue(exm);
				text = arguments[1].GetStrValue(exm);
			}
			else
			{
				text = arguments[0].GetStrValue(exm);
				num = arguments[1].GetIntValue(exm);
			}
			if (num < 0)
			{
				throw new CodeEE("文字列に負の値(" + num + ")を乗算しようとしました");
			}
			if (num >= 10000)
			{
				throw new CodeEE("文字列に10000以上の値(" + num + ")を乗算しようとしました");
			}
			if (text == "" || num == 0L)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = text.Length * (int)num;
			for (int i = 0; i < num; i++)
			{
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}
	}

	private sealed class DivIntInt : OperatorMethod
	{
		public DivIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[1].GetIntValue(exm);
			if (intValue == 0L)
			{
				throw new CodeEE("0による除算が行なわれました");
			}
			return arguments[0].GetIntValue(exm) / intValue;
		}
	}

	private sealed class ModIntInt : OperatorMethod
	{
		public ModIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[1].GetIntValue(exm);
			if (intValue == 0L)
			{
				throw new CodeEE("0による除算が行なわれました");
			}
			return arguments[0].GetIntValue(exm) % intValue;
		}
	}

	private sealed class EqualIntInt : OperatorMethod
	{
		public EqualIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) == arguments[1].GetIntValue(exm))
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class EqualStrStr : OperatorMethod
	{
		public EqualStrStr()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetStrValue(exm) == arguments[1].GetStrValue(exm))
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class NotEqualIntInt : OperatorMethod
	{
		public NotEqualIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) != arguments[1].GetIntValue(exm))
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class NotEqualStrStr : OperatorMethod
	{
		public NotEqualStrStr()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetStrValue(exm) != arguments[1].GetStrValue(exm))
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class GreaterIntInt : OperatorMethod
	{
		public GreaterIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) > arguments[1].GetIntValue(exm))
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class GreaterStrStr : OperatorMethod
	{
		public GreaterStrStr()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (string.Compare(arguments[0].GetStrValue(exm), arguments[1].GetStrValue(exm), StringComparison.Ordinal) > 0)
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class LessIntInt : OperatorMethod
	{
		public LessIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) < arguments[1].GetIntValue(exm))
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class LessStrStr : OperatorMethod
	{
		public LessStrStr()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (string.Compare(arguments[0].GetStrValue(exm), arguments[1].GetStrValue(exm), StringComparison.Ordinal) < 0)
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class GreaterEqualIntInt : OperatorMethod
	{
		public GreaterEqualIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) >= arguments[1].GetIntValue(exm))
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class GreaterEqualStrStr : OperatorMethod
	{
		public GreaterEqualStrStr()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (string.Compare(arguments[0].GetStrValue(exm), arguments[1].GetStrValue(exm), StringComparison.Ordinal) < 0)
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class LessEqualIntInt : OperatorMethod
	{
		public LessEqualIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) <= arguments[1].GetIntValue(exm))
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class LessEqualStrStr : OperatorMethod
	{
		public LessEqualStrStr()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (string.Compare(arguments[0].GetStrValue(exm), arguments[1].GetStrValue(exm), StringComparison.Ordinal) < 0)
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class AndIntInt : OperatorMethod
	{
		public AndIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) != 0L && arguments[1].GetIntValue(exm) != 0L)
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class OrIntInt : OperatorMethod
	{
		public OrIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) != 0L || arguments[1].GetIntValue(exm) != 0L)
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class XorIntInt : OperatorMethod
	{
		public XorIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			long intValue2 = arguments[1].GetIntValue(exm);
			if ((intValue == 0L && intValue2 != 0L) || (intValue != 0L && intValue2 == 0L))
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class NandIntInt : OperatorMethod
	{
		public NandIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) == 0L || arguments[1].GetIntValue(exm) == 0L)
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class NorIntInt : OperatorMethod
	{
		public NorIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) == 0L && arguments[1].GetIntValue(exm) == 0L)
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class BitAndIntInt : OperatorMethod
	{
		public BitAndIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return arguments[0].GetIntValue(exm) & arguments[1].GetIntValue(exm);
		}
	}

	private sealed class BitOrIntInt : OperatorMethod
	{
		public BitOrIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return arguments[0].GetIntValue(exm) | arguments[1].GetIntValue(exm);
		}
	}

	private sealed class BitXorIntInt : OperatorMethod
	{
		public BitXorIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return arguments[0].GetIntValue(exm) ^ arguments[1].GetIntValue(exm);
		}
	}

	private sealed class RightShiftIntInt : OperatorMethod
	{
		public RightShiftIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return arguments[0].GetIntValue(exm) >> (int)arguments[1].GetIntValue(exm);
		}
	}

	private sealed class LeftShiftIntInt : OperatorMethod
	{
		public LeftShiftIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return arguments[0].GetIntValue(exm) << (int)arguments[1].GetIntValue(exm);
		}
	}

	private sealed class PlusInt : OperatorMethod
	{
		public PlusInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return arguments[0].GetIntValue(exm);
		}
	}

	private sealed class MinusInt : OperatorMethod
	{
		public MinusInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return -arguments[0].GetIntValue(exm);
		}
	}

	private sealed class NotInt : OperatorMethod
	{
		public NotInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) == 0L)
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class BitNotInt : OperatorMethod
	{
		public BitNotInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return ~arguments[0].GetIntValue(exm);
		}
	}

	private sealed class IncrementInt : OperatorMethod
	{
		public IncrementInt()
		{
			base.CanRestructure = false;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return ((VariableTerm)arguments[0]).PlusValue(1L, exm);
		}
	}

	private sealed class DecrementInt : OperatorMethod
	{
		public DecrementInt()
		{
			base.CanRestructure = false;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return ((VariableTerm)arguments[0]).PlusValue(-1L, exm);
		}
	}

	private sealed class IncrementAfterInt : OperatorMethod
	{
		public IncrementAfterInt()
		{
			base.CanRestructure = false;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return ((VariableTerm)arguments[0]).PlusValue(1L, exm) - 1;
		}
	}

	private sealed class DecrementAfterInt : OperatorMethod
	{
		public DecrementAfterInt()
		{
			base.CanRestructure = false;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return ((VariableTerm)arguments[0]).PlusValue(-1L, exm) + 1;
		}
	}

	private sealed class TernaryIntIntInt : OperatorMethod
	{
		public TernaryIntIntInt()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(long);
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) == 0L)
			{
				return arguments[2].GetIntValue(exm);
			}
			return arguments[1].GetIntValue(exm);
		}
	}

	private sealed class TernaryIntStrStr : OperatorMethod
	{
		public TernaryIntStrStr()
		{
			base.CanRestructure = true;
			base.ReturnType = typeof(string);
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetIntValue(exm) == 0L)
			{
				return arguments[2].GetStrValue(exm);
			}
			return arguments[1].GetStrValue(exm);
		}
	}

	private static readonly Dictionary<OperatorCode, OperatorMethod> unaryDic;

	private static readonly Dictionary<OperatorCode, OperatorMethod> unaryAfterDic;

	private static readonly Dictionary<OperatorCode, OperatorMethod> binaryIntIntDic;

	private static readonly Dictionary<OperatorCode, OperatorMethod> binaryStrStrDic;

	private static readonly OperatorMethod binaryMultIntStr;

	private static readonly OperatorMethod ternaryIntIntInt;

	private static readonly OperatorMethod ternaryIntStrStr;

	static OperatorMethodManager()
	{
		unaryDic = new Dictionary<OperatorCode, OperatorMethod>();
		unaryAfterDic = new Dictionary<OperatorCode, OperatorMethod>();
		binaryIntIntDic = new Dictionary<OperatorCode, OperatorMethod>();
		binaryStrStrDic = new Dictionary<OperatorCode, OperatorMethod>();
		binaryMultIntStr = null;
		ternaryIntIntInt = null;
		ternaryIntStrStr = null;
		unaryDic[OperatorCode.Plus] = new PlusInt();
		unaryDic[OperatorCode.Minus] = new MinusInt();
		unaryDic[OperatorCode.Not] = new NotInt();
		unaryDic[OperatorCode.BitNot] = new BitNotInt();
		unaryDic[OperatorCode.Increment] = new IncrementInt();
		unaryDic[OperatorCode.Decrement] = new DecrementInt();
		unaryAfterDic[OperatorCode.Increment] = new IncrementAfterInt();
		unaryAfterDic[OperatorCode.Decrement] = new DecrementAfterInt();
		binaryIntIntDic[OperatorCode.Plus] = new PlusIntInt();
		binaryIntIntDic[OperatorCode.Minus] = new MinusIntInt();
		binaryIntIntDic[OperatorCode.Mult] = new MultIntInt();
		binaryIntIntDic[OperatorCode.Div] = new DivIntInt();
		binaryIntIntDic[OperatorCode.Mod] = new ModIntInt();
		binaryIntIntDic[OperatorCode.Equal] = new EqualIntInt();
		binaryIntIntDic[OperatorCode.Greater] = new GreaterIntInt();
		binaryIntIntDic[OperatorCode.Less] = new LessIntInt();
		binaryIntIntDic[OperatorCode.GreaterEqual] = new GreaterEqualIntInt();
		binaryIntIntDic[OperatorCode.LessEqual] = new LessEqualIntInt();
		binaryIntIntDic[OperatorCode.NotEqual] = new NotEqualIntInt();
		binaryIntIntDic[OperatorCode.And] = new AndIntInt();
		binaryIntIntDic[OperatorCode.Or] = new OrIntInt();
		binaryIntIntDic[OperatorCode.Xor] = new XorIntInt();
		binaryIntIntDic[OperatorCode.Nand] = new NandIntInt();
		binaryIntIntDic[OperatorCode.Nor] = new NorIntInt();
		binaryIntIntDic[OperatorCode.BitAnd] = new BitAndIntInt();
		binaryIntIntDic[OperatorCode.BitOr] = new BitOrIntInt();
		binaryIntIntDic[OperatorCode.BitXor] = new BitXorIntInt();
		binaryIntIntDic[OperatorCode.RightShift] = new RightShiftIntInt();
		binaryIntIntDic[OperatorCode.LeftShift] = new LeftShiftIntInt();
		binaryStrStrDic[OperatorCode.Plus] = new PlusStrStr();
		binaryStrStrDic[OperatorCode.Equal] = new EqualStrStr();
		binaryStrStrDic[OperatorCode.Greater] = new GreaterStrStr();
		binaryStrStrDic[OperatorCode.Less] = new LessStrStr();
		binaryStrStrDic[OperatorCode.GreaterEqual] = new GreaterEqualStrStr();
		binaryStrStrDic[OperatorCode.LessEqual] = new LessEqualStrStr();
		binaryStrStrDic[OperatorCode.NotEqual] = new NotEqualStrStr();
		binaryMultIntStr = new MultStrInt();
		ternaryIntIntInt = new TernaryIntIntInt();
		ternaryIntStrStr = new TernaryIntStrStr();
	}

	public static IOperandTerm ReduceUnaryTerm(OperatorCode op, IOperandTerm o1)
	{
		OperatorMethod operatorMethod = null;
		if ((op == OperatorCode.Increment || op == OperatorCode.Decrement) && ((o1 as VariableTerm) ?? throw new CodeEE("変数以外をインクリメントすることはできません")).Identifier.IsConst)
		{
			throw new CodeEE("変更できない変数をインクリメントすることはできません");
		}
		if (o1.GetOperandType() == typeof(long))
		{
			if (op == OperatorCode.Plus)
			{
				return o1;
			}
			if (unaryDic.ContainsKey(op))
			{
				operatorMethod = unaryDic[op];
			}
		}
		if (operatorMethod != null)
		{
			return new FunctionMethodTerm(operatorMethod, new IOperandTerm[1] { o1 });
		}
		string text = "";
		text = ((o1.GetOperandType() == typeof(long)) ? (text + "数値型") : ((!(o1.GetOperandType() == typeof(string))) ? (text + "不定型") : (text + "文字列型")));
		text = text + "に単項演算子'" + OperatorManager.ToOperatorString(op) + "'は適用できません";
		throw new CodeEE(text);
	}

	public static IOperandTerm ReduceUnaryAfterTerm(OperatorCode op, IOperandTerm o1)
	{
		OperatorMethod operatorMethod = null;
		if ((op == OperatorCode.Increment || op == OperatorCode.Decrement) && ((o1 as VariableTerm) ?? throw new CodeEE("変数以外をインクリメントすることはできません")).Identifier.IsConst)
		{
			throw new CodeEE("変更できない変数をインクリメントすることはできません");
		}
		if (o1.GetOperandType() == typeof(long) && unaryAfterDic.ContainsKey(op))
		{
			operatorMethod = unaryAfterDic[op];
		}
		if (operatorMethod != null)
		{
			return new FunctionMethodTerm(operatorMethod, new IOperandTerm[1] { o1 });
		}
		string text = "";
		text = ((o1.GetOperandType() == typeof(long)) ? (text + "数値型") : ((!(o1.GetOperandType() == typeof(string))) ? (text + "不定型") : (text + "文字列型")));
		text = text + "に後置単項演算子'" + OperatorManager.ToOperatorString(op) + "'は適用できません";
		throw new CodeEE(text);
	}

	public static IOperandTerm ReduceBinaryTerm(OperatorCode op, IOperandTerm left, IOperandTerm right)
	{
		OperatorMethod operatorMethod = null;
		if (left.GetOperandType() == typeof(long) && right.GetOperandType() == typeof(long))
		{
			if (binaryIntIntDic.ContainsKey(op))
			{
				operatorMethod = binaryIntIntDic[op];
			}
		}
		else if (left.GetOperandType() == typeof(string) && right.GetOperandType() == typeof(string))
		{
			if (binaryStrStrDic.ContainsKey(op))
			{
				operatorMethod = binaryStrStrDic[op];
			}
		}
		else if (((left.GetOperandType() == typeof(long) && right.GetOperandType() == typeof(string)) || (left.GetOperandType() == typeof(string) && right.GetOperandType() == typeof(long))) && op == OperatorCode.Mult)
		{
			operatorMethod = binaryMultIntStr;
		}
		if (operatorMethod != null)
		{
			return new FunctionMethodTerm(operatorMethod, new IOperandTerm[2] { left, right });
		}
		string text = "";
		text = ((left.GetOperandType() == typeof(long)) ? (text + "数値型と") : ((!(left.GetOperandType() == typeof(string))) ? (text + "不定型と") : (text + "文字列型と")));
		text = ((right.GetOperandType() == typeof(long)) ? (text + "数値型の") : ((!(right.GetOperandType() == typeof(string))) ? (text + "不定型の") : (text + "文字列型の")));
		text = text + "演算に二項演算子'" + OperatorManager.ToOperatorString(op) + "'は適用できません";
		throw new CodeEE(text);
	}

	public static IOperandTerm ReduceTernaryTerm(OperatorCode op, IOperandTerm o1, IOperandTerm o2, IOperandTerm o3)
	{
		OperatorMethod operatorMethod = null;
		if (o1.GetOperandType() == typeof(long) && o2.GetOperandType() == typeof(long) && o3.GetOperandType() == typeof(long))
		{
			operatorMethod = ternaryIntIntInt;
		}
		else if (o1.GetOperandType() == typeof(long) && o2.GetOperandType() == typeof(string) && o3.GetOperandType() == typeof(string))
		{
			operatorMethod = ternaryIntStrStr;
		}
		if (operatorMethod != null)
		{
			return new FunctionMethodTerm(operatorMethod, new IOperandTerm[3] { o1, o2, o3 });
		}
		throw new CodeEE("三項演算子の使用法が不正です");
	}
}
