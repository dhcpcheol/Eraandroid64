using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpTimesArgument : Argument
{
	public readonly VariableTerm VariableDest;

	public readonly double DoubleValue;

	public SpTimesArgument(VariableTerm var, double d)
	{
		VariableDest = var;
		DoubleValue = d;
	}
}
