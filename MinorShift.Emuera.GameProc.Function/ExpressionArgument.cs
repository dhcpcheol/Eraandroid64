using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class ExpressionArgument : Argument
{
	public readonly IOperandTerm Term;

	public ExpressionArgument(IOperandTerm termSrc)
	{
		Term = termSrc;
	}
}
