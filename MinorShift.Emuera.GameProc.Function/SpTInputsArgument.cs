using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpTInputsArgument : Argument
{
	public readonly IOperandTerm Time;

	public readonly IOperandTerm Def;

	public readonly IOperandTerm Disp;

	public readonly IOperandTerm Timeout;

	public SpTInputsArgument(IOperandTerm time, IOperandTerm def, IOperandTerm disp, IOperandTerm timeout)
	{
		Time = time;
		Def = def;
		Disp = disp;
		Timeout = timeout;
	}
}
