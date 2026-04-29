using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpArrayControlArgument : Argument
{
	public readonly VariableTerm VarToken;

	public readonly IOperandTerm Num1;

	public readonly IOperandTerm Num2;

	public SpArrayControlArgument(VariableTerm var, IOperandTerm num1, IOperandTerm num2)
	{
		VarToken = var;
		Num1 = num1;
		Num2 = num2;
	}
}
