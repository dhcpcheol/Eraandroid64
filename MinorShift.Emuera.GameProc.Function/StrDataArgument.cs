using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class StrDataArgument : Argument
{
	public readonly VariableTerm Var;

	public StrDataArgument(VariableTerm var)
	{
		Var = var;
	}
}
