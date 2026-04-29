namespace MinorShift.Emuera.GameData.Expression;

internal sealed class StrFormTerm : IOperandTerm
{
	private readonly StrForm sfValue;

	public StrForm StrForm => sfValue;

	public StrFormTerm(StrForm sf)
		: base(typeof(string))
	{
		sfValue = sf;
	}

	public override string GetStrValue(ExpressionMediator exm)
	{
		return sfValue.GetString(exm);
	}

	public override SingleTerm GetValue(ExpressionMediator exm)
	{
		return new SingleTerm(sfValue.GetString(exm));
	}

	public override IOperandTerm Restructure(ExpressionMediator exm)
	{
		sfValue.Restructure(exm);
		if (sfValue.IsConst)
		{
			return new SingleTerm(sfValue.GetString(exm));
		}
		IOperandTerm iOperandTerm = sfValue.GetIOperandTerm();
		if (iOperandTerm != null)
		{
			return iOperandTerm;
		}
		return this;
	}
}
