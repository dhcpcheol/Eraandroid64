using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpArraySortArgument : Argument
{
	public readonly VariableTerm VarToken;

	public readonly SortOrder Order;

	public readonly IOperandTerm Num1;

	public readonly IOperandTerm Num2;

	public SpArraySortArgument(VariableTerm var, SortOrder order, IOperandTerm num1, IOperandTerm num2)
	{
		VarToken = var;
		Order = order;
		Num1 = num1;
		Num2 = num2;
	}
}
