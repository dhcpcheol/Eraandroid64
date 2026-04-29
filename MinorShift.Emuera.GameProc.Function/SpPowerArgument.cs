using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpPowerArgument : Argument
{
	public readonly VariableTerm VariableDest;

	public readonly IOperandTerm X;

	public readonly IOperandTerm Y;

	public SpPowerArgument(VariableTerm var, IOperandTerm x, IOperandTerm y)
	{
		VariableDest = var;
		X = x;
		Y = y;
	}
}
