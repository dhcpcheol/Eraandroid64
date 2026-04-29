using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class MethodArgument : Argument
{
	public readonly IOperandTerm MethodTerm;

	public MethodArgument(IOperandTerm method)
	{
		MethodTerm = method;
	}
}
