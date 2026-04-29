using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpCopyArrayArgument : Argument
{
	public readonly IOperandTerm VarName1;

	public readonly IOperandTerm VarName2;

	public SpCopyArrayArgument(IOperandTerm str1, IOperandTerm str2)
	{
		VarName1 = str1;
		VarName2 = str2;
	}
}
