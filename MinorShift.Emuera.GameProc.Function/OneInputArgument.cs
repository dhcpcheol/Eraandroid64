using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class OneInputArgument : Argument
{
	public readonly IOperandTerm Term;

	public readonly IOperandTerm Flag;

	public OneInputArgument(IOperandTerm term, IOperandTerm flag)
	{
		Term = term;
		Flag = flag;
	}
}
