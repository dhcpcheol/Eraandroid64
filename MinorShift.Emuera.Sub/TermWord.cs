using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.Sub;

internal sealed class TermWord : Word
{
	private readonly IOperandTerm term;

	public IOperandTerm Term => term;

	public override char Type => 'T';

	public TermWord(IOperandTerm term)
	{
		this.term = term;
	}
}
