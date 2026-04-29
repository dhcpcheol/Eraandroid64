using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpColorArgument : Argument
{
	public readonly IOperandTerm R;

	public readonly IOperandTerm G;

	public readonly IOperandTerm B;

	public readonly IOperandTerm RGB;

	public SpColorArgument(IOperandTerm r, IOperandTerm g, IOperandTerm b)
	{
		R = r;
		G = g;
		B = b;
	}

	public SpColorArgument(IOperandTerm rgb)
	{
		RGB = rgb;
	}
}
