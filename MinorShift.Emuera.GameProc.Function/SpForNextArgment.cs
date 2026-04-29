using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpForNextArgment : Argument
{
	public readonly VariableTerm Cnt;

	public readonly IOperandTerm Start;

	public readonly IOperandTerm End;

	public readonly IOperandTerm Step;

	public SpForNextArgment(VariableTerm var, IOperandTerm start, IOperandTerm end, IOperandTerm step)
	{
		Cnt = var;
		Start = start;
		End = end;
		Step = step;
	}
}
