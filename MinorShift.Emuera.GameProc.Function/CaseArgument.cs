using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class CaseArgument : Argument
{
	public readonly CaseExpression[] CaseExps;

	public CaseArgument(CaseExpression[] args)
	{
		CaseExps = args;
	}
}
