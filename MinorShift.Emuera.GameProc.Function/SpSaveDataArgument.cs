using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpSaveDataArgument : Argument
{
	public readonly IOperandTerm Target;

	public readonly IOperandTerm StrExpression;

	public SpSaveDataArgument(IOperandTerm target, IOperandTerm var)
	{
		Target = target;
		StrExpression = var;
	}
}
