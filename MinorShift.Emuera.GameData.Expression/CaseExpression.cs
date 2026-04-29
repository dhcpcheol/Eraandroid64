using System;

namespace MinorShift.Emuera.GameData.Expression;

internal sealed class CaseExpression
{
	public CaseExpressionType CaseType = CaseExpressionType.Normal;

	public IOperandTerm LeftTerm;

	public IOperandTerm RightTerm;

	public OperatorCode Operator;

	public Type GetOperandType()
	{
		if (LeftTerm != null)
		{
			return LeftTerm.GetOperandType();
		}
		return typeof(void);
	}

	public void Reduce(ExpressionMediator exm)
	{
		LeftTerm = LeftTerm.Restructure(exm);
		if (CaseType == CaseExpressionType.To)
		{
			RightTerm = RightTerm.Restructure(exm);
		}
	}

	public override string ToString()
	{
		return CaseType switch
		{
			CaseExpressionType.Normal => LeftTerm.ToString(), 
			CaseExpressionType.Is => "Is " + Operator.ToString() + " " + LeftTerm.ToString(), 
			CaseExpressionType.To => LeftTerm.ToString() + " To " + RightTerm.ToString(), 
			_ => base.ToString(), 
		};
	}

	public bool GetBool(long Is, ExpressionMediator exm)
	{
		if (CaseType == CaseExpressionType.To)
		{
			if (LeftTerm.GetIntValue(exm) <= Is)
			{
				return Is <= RightTerm.GetIntValue(exm);
			}
			return false;
		}
		if (CaseType == CaseExpressionType.Is)
		{
			return OperatorMethodManager.ReduceBinaryTerm(Operator, new SingleTerm(Is), LeftTerm).GetIntValue(exm) != 0;
		}
		return LeftTerm.GetIntValue(exm) == Is;
	}

	public bool GetBool(string Is, ExpressionMediator exm)
	{
		if (CaseType == CaseExpressionType.To)
		{
			if (string.Compare(LeftTerm.GetStrValue(exm), Is, StringComparison.Ordinal) <= 0)
			{
				return string.Compare(Is, RightTerm.GetStrValue(exm), StringComparison.Ordinal) <= 0;
			}
			return false;
		}
		if (CaseType == CaseExpressionType.Is)
		{
			return OperatorMethodManager.ReduceBinaryTerm(Operator, new SingleTerm(Is), LeftTerm).GetIntValue(exm) != 0;
		}
		return LeftTerm.GetStrValue(exm) == Is;
	}
}
