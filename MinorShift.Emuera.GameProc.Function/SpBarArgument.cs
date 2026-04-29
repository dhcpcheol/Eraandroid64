using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpBarArgument : Argument
{
	public readonly IOperandTerm[] Terms = new IOperandTerm[3];

	public SpBarArgument(IOperandTerm value, IOperandTerm max, IOperandTerm length)
	{
		Terms[0] = value;
		Terms[1] = max;
		Terms[2] = length;
	}
}
