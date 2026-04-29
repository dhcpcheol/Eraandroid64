using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpSplitArgument : Argument
{
	public readonly IOperandTerm TargetStr;

	public readonly IOperandTerm Split;

	public readonly VariableToken Var;

	public readonly VariableTerm Num;

	public SpSplitArgument(IOperandTerm s1, IOperandTerm s2, VariableToken varId, VariableTerm num)
	{
		TargetStr = s1;
		Split = s2;
		Var = varId;
		Num = num;
	}
}
