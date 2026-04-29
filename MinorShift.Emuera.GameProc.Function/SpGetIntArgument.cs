using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpGetIntArgument : Argument
{
	public readonly VariableTerm VarToken;

	public SpGetIntArgument(VariableTerm var)
	{
		VarToken = var;
	}
}
