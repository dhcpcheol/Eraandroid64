using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class PrintDataArgument : Argument
{
	public readonly VariableTerm Var;

	public PrintDataArgument(VariableTerm var)
	{
		Var = var;
	}
}
