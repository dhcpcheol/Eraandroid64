using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpPrintVArgument : Argument
{
	public readonly IOperandTerm[] Terms;

	public SpPrintVArgument(IOperandTerm[] list)
	{
		Terms = list;
	}
}
