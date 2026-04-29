using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class OneInputsArgument : Argument
{
	public readonly IOperandTerm Term;

	public readonly IOperandTerm Flag;

	public OneInputsArgument(IOperandTerm term, IOperandTerm flag)
	{
		Term = term;
		Flag = flag;
	}
}
