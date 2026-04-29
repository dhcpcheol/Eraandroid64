using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpVarSetArgument : Argument
{
	public readonly VariableTerm VariableDest;

	public readonly IOperandTerm Term;

	public readonly IOperandTerm Start;

	public readonly IOperandTerm End;

	public SpVarSetArgument(VariableTerm var, IOperandTerm termSrc, IOperandTerm start, IOperandTerm end)
	{
		VariableDest = var;
		Term = termSrc;
		Start = start;
		End = end;
	}
}
