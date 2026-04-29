using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpSetArgument : Argument
{
	public readonly VariableTerm VariableDest;

	public readonly IOperandTerm Term;

	public bool AddConst;

	public SpSetArgument(VariableTerm var, IOperandTerm termSrc)
	{
		VariableDest = var;
		Term = termSrc;
	}
}
