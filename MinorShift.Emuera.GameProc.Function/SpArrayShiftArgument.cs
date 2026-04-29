using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpArrayShiftArgument : Argument
{
	public readonly VariableTerm VarToken;

	public readonly IOperandTerm Num1;

	public readonly IOperandTerm Num2;

	public readonly IOperandTerm Num3;

	public readonly IOperandTerm Num4;

	public SpArrayShiftArgument(VariableTerm var, IOperandTerm num1, IOperandTerm num2, IOperandTerm num3, IOperandTerm num4)
	{
		VarToken = var;
		Num1 = num1;
		Num2 = num2;
		Num3 = num3;
		Num4 = num4;
	}
}
