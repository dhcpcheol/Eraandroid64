using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpSaveVarArgument : Argument
{
	public readonly IOperandTerm Term;

	public readonly IOperandTerm SavMes;

	public readonly VariableToken[] VarTokens;

	public SpSaveVarArgument(IOperandTerm term, IOperandTerm mes, VariableToken[] varTokens)
	{
		Term = term;
		SavMes = mes;
		VarTokens = varTokens;
	}
}
