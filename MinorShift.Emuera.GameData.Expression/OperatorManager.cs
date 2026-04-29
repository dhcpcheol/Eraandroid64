using System.Collections.Generic;

namespace MinorShift.Emuera.GameData.Expression;

internal static class OperatorManager
{
	private static readonly Dictionary<string, OperatorCode> opDictionary;

	static OperatorManager()
	{
		opDictionary = new Dictionary<string, OperatorCode>();
		opDictionary.Add("+", OperatorCode.Plus);
		opDictionary.Add("-", OperatorCode.Minus);
		opDictionary.Add("*", OperatorCode.Mult);
		opDictionary.Add("/", OperatorCode.Div);
		opDictionary.Add("%", OperatorCode.Mod);
		opDictionary.Add("==", OperatorCode.Equal);
		opDictionary.Add(">", OperatorCode.Greater);
		opDictionary.Add("<", OperatorCode.Less);
		opDictionary.Add(">=", OperatorCode.GreaterEqual);
		opDictionary.Add("<=", OperatorCode.LessEqual);
		opDictionary.Add("!=", OperatorCode.NotEqual);
		opDictionary.Add("&&", OperatorCode.And);
		opDictionary.Add("||", OperatorCode.Or);
		opDictionary.Add("^^", OperatorCode.Xor);
		opDictionary.Add("!&", OperatorCode.Nand);
		opDictionary.Add("!|", OperatorCode.Nor);
		opDictionary.Add("&", OperatorCode.BitAnd);
		opDictionary.Add("|", OperatorCode.BitOr);
		opDictionary.Add("!", OperatorCode.Not);
		opDictionary.Add("^", OperatorCode.BitXor);
		opDictionary.Add("~", OperatorCode.BitNot);
		opDictionary.Add("?", OperatorCode.Ternary_a);
		opDictionary.Add("#", OperatorCode.Ternary_b);
		opDictionary.Add(">>", OperatorCode.RightShift);
		opDictionary.Add("<<", OperatorCode.LeftShift);
		opDictionary.Add("++", OperatorCode.Increment);
		opDictionary.Add("--", OperatorCode.Decrement);
		opDictionary.Add("=", OperatorCode.Assignment);
		opDictionary.Add("'=", OperatorCode.AssignmentStr);
	}

	public static string ToOperatorString(OperatorCode op)
	{
		if (op == OperatorCode.NULL)
		{
			return "";
		}
		foreach (KeyValuePair<string, OperatorCode> item in opDictionary)
		{
			if (op == item.Value)
			{
				return item.Key;
			}
		}
		return "";
	}

	public static bool IsUnary(OperatorCode type)
	{
		return (type & OperatorCode.__UNARY__) == OperatorCode.__UNARY__;
	}

	public static bool IsUnaryAfter(OperatorCode type)
	{
		return (type & OperatorCode.__UNARY_AFTER__) == OperatorCode.__UNARY_AFTER__;
	}

	public static bool IsBinary(OperatorCode type)
	{
		return (type & OperatorCode.__BINARY__) == OperatorCode.__BINARY__;
	}

	public static bool IsTernary(OperatorCode type)
	{
		return (type & OperatorCode.__TERNARY__) == OperatorCode.__TERNARY__;
	}

	public static int GetPriority(OperatorCode type)
	{
		return (int)(type & OperatorCode.__PRIORITY_MASK__);
	}
}
