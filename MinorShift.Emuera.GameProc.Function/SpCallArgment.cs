using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpCallArgment : Argument
{
	public readonly IOperandTerm FuncnameTerm;

	public readonly IOperandTerm[] SubNames;

	public readonly IOperandTerm[] RowArgs;

	public UserDefinedFunctionArgument UDFArgument;

	public CalledFunction CallFunc;

	public SpCallArgment(IOperandTerm funcname, IOperandTerm[] subNames, IOperandTerm[] args)
	{
		FuncnameTerm = funcname;
		SubNames = subNames;
		RowArgs = args;
	}
}
