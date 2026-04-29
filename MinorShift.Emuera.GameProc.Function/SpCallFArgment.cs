using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpCallFArgment : Argument
{
	public readonly IOperandTerm FuncnameTerm;

	public readonly IOperandTerm[] SubNames;

	public readonly IOperandTerm[] RowArgs;

	public IOperandTerm FuncTerm;

	public SpCallFArgment(IOperandTerm funcname, IOperandTerm[] subNames, IOperandTerm[] args)
	{
		FuncnameTerm = funcname;
		SubNames = subNames;
		RowArgs = args;
	}
}
