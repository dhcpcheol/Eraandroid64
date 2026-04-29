using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpHtmlSplitArgument : Argument
{
	public readonly IOperandTerm TargetStr;

	public readonly VariableToken Var;

	public readonly VariableTerm Num;

	public SpHtmlSplitArgument(IOperandTerm s1, VariableToken varId, VariableTerm num)
	{
		TargetStr = s1;
		Var = varId;
		Num = num;
	}
}
