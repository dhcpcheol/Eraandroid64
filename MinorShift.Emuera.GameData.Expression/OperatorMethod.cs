using MinorShift.Emuera.GameData.Function;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Expression;

internal abstract class OperatorMethod : FunctionMethod
{
	public OperatorMethod()
	{
		argumentTypeArray = null;
	}

	public override string CheckArgumentType(string name, IOperandTerm[] arguments)
	{
		throw new ExeEE("型チェックは呼び出し元が行うこと");
	}
}
