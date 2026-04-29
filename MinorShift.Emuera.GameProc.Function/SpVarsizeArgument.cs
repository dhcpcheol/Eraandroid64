using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpVarsizeArgument : Argument
{
	public readonly VariableToken VariableID;

	public SpVarsizeArgument(VariableToken var)
	{
		VariableID = var;
	}
}
