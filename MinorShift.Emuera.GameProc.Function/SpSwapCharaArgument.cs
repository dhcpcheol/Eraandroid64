using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpSwapCharaArgument : Argument
{
	public readonly IOperandTerm X;

	public readonly IOperandTerm Y;

	public SpSwapCharaArgument(IOperandTerm x, IOperandTerm y)
	{
		X = x;
		Y = y;
	}
}
