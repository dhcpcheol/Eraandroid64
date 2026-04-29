using System.Collections.Generic;
using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class ExpressionArrayArgument : Argument
{
	public readonly IOperandTerm[] TermList;

	public ExpressionArrayArgument(List<IOperandTerm> termList)
	{
		TermList = new IOperandTerm[termList.Count];
		termList.CopyTo(TermList);
	}
}
