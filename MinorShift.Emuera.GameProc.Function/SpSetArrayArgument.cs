using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpSetArrayArgument : Argument
{
	public readonly VariableTerm VariableDest;

	public readonly IOperandTerm[] TermList;

	public readonly long[] ConstIntList;

	public readonly string[] ConstStrList;

	public SpSetArrayArgument(VariableTerm var, IOperandTerm[] termList, long[] constList)
	{
		VariableDest = var;
		TermList = termList;
		ConstIntList = constList;
	}

	public SpSetArrayArgument(VariableTerm var, IOperandTerm[] termList, string[] constList)
	{
		VariableDest = var;
		TermList = termList;
		ConstStrList = constList;
	}
}
