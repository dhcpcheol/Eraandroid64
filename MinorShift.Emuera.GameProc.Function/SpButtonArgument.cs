using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpButtonArgument : Argument
{
	public readonly IOperandTerm PrintStrTerm;

	public readonly IOperandTerm ButtonWord;

	public SpButtonArgument(IOperandTerm p1, IOperandTerm p2)
	{
		PrintStrTerm = p1;
		ButtonWord = p2;
	}
}
