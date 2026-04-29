using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal sealed class VariableNoArgTerm : VariableTerm
{
	public VariableNoArgTerm(VariableToken token)
		: base(token)
	{
		Identifier = token;
		allArgIsConst = true;
	}

	public override long GetIntValue(ExpressionMediator exm)
	{
		throw new CodeEE("変数" + Identifier.Name + "に必要な引数が不足しています");
	}

	public override string GetStrValue(ExpressionMediator exm)
	{
		throw new CodeEE("変数" + Identifier.Name + "に必要な引数が不足しています");
	}

	public override void SetValue(long value, ExpressionMediator exm)
	{
		throw new CodeEE("変数" + Identifier.Name + "に必要な引数が不足しています");
	}

	public override void SetValue(string value, ExpressionMediator exm)
	{
		throw new CodeEE("変数" + Identifier.Name + "に必要な引数が不足しています");
	}

	public override void SetValue(long[] array, ExpressionMediator exm)
	{
		throw new CodeEE("変数" + Identifier.Name + "に必要な引数が不足しています");
	}

	public override void SetValue(string[] array, ExpressionMediator exm)
	{
		throw new CodeEE("変数" + Identifier.Name + "に必要な引数が不足しています");
	}

	public override long PlusValue(long value, ExpressionMediator exm)
	{
		throw new CodeEE("変数" + Identifier.Name + "に必要な引数が不足しています");
	}

	public override SingleTerm GetValue(ExpressionMediator exm)
	{
		throw new CodeEE("変数" + Identifier.Name + "に必要な引数が不足しています");
	}

	public override void SetValue(SingleTerm value, ExpressionMediator exm)
	{
		throw new CodeEE("変数" + Identifier.Name + "に必要な引数が不足しています");
	}

	public override void SetValue(IOperandTerm value, ExpressionMediator exm)
	{
		throw new CodeEE("変数" + Identifier.Name + "に必要な引数が不足しています");
	}

	public override FixedVariableTerm GetFixedVariableTerm(ExpressionMediator exm)
	{
		throw new CodeEE("変数" + Identifier.Name + "に必要な引数が不足しています");
	}

	public override IOperandTerm Restructure(ExpressionMediator exm)
	{
		return this;
	}
}
