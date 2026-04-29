using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.Sub;

internal sealed class OperatorWord : Word
{
	private readonly OperatorCode code;

	public OperatorCode Code => code;

	public override char Type => '=';

	public OperatorWord(OperatorCode op)
	{
		code = op;
	}

	public override string ToString()
	{
		return code.ToString();
	}
}
