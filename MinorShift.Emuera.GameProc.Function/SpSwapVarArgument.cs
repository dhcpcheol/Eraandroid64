using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpSwapVarArgument : Argument
{
	public readonly VariableTerm var1;

	public readonly VariableTerm var2;

	public SpSwapVarArgument(VariableTerm v1, VariableTerm v2)
	{
		var1 = v1;
		var2 = v2;
	}
}
