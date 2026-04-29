using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class SpSortcharaArgument : Argument
{
	public readonly VariableTerm SortKey;

	public readonly SortOrder SortOrder;

	public SpSortcharaArgument(VariableTerm var, SortOrder order)
	{
		SortKey = var;
		SortOrder = order;
	}
}
