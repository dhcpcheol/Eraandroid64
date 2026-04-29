using System.Collections.Generic;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal sealed class VariableStrArgTerm : IOperandTerm
{
	private IOperandTerm strTerm;

	private readonly VariableCode parentCode;

	private readonly int index;

	private Dictionary<string, int> dic;

	private string errPos;

	public VariableStrArgTerm(VariableCode code, IOperandTerm strTerm, int index)
		: base(typeof(long))
	{
		this.strTerm = strTerm;
		parentCode = code;
		this.index = index;
	}

	public override long GetIntValue(ExpressionMediator exm)
	{
		if (dic == null)
		{
			dic = exm.VEvaluator.Constant.GetKeywordDictionary(out errPos, parentCode, index);
		}
		string strValue = strTerm.GetStrValue(exm);
		if (strValue == "")
		{
			throw new CodeEE("キーワードを空には出来ません");
		}
		if (!dic.TryGetValue(strValue, out var value))
		{
			if (errPos == null)
			{
				throw new CodeEE("配列変数" + parentCode.ToString() + "の要素を文字列で指定することはできません");
			}
			throw new CodeEE(errPos + "の中に\"" + strValue + "\"の定義がありません");
		}
		return value;
	}

	public override IOperandTerm Restructure(ExpressionMediator exm)
	{
		if (dic == null)
		{
			dic = exm.VEvaluator.Constant.GetKeywordDictionary(out errPos, parentCode, index);
		}
		strTerm = strTerm.Restructure(exm);
		if (!(strTerm is SingleTerm))
		{
			return this;
		}
		return new SingleTerm(GetIntValue(exm));
	}
}
