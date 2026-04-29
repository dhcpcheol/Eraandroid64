using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class BitArgument : Argument
{
	public readonly VariableTerm VariableDest;

	public readonly IOperandTerm[] Term;

	public BitArgument(VariableTerm var, IOperandTerm[] termSrc)
	{
		VariableDest = var;
		Term = termSrc;
	}
}
