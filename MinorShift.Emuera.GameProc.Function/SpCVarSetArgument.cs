using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpCVarSetArgument : Argument
{
	public readonly VariableTerm VariableDest;

	public readonly IOperandTerm Index;

	public readonly IOperandTerm Term;

	public readonly IOperandTerm Start;

	public readonly IOperandTerm End;

	public SpCVarSetArgument(VariableTerm var, IOperandTerm indexTerm, IOperandTerm termSrc, IOperandTerm start, IOperandTerm end)
	{
		VariableDest = var;
		Index = indexTerm;
		Term = termSrc;
		Start = start;
		End = end;
	}
}
