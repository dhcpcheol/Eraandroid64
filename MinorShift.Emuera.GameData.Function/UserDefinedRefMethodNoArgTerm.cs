using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameProc;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Function;

internal sealed class UserDefinedRefMethodNoArgTerm : SuperUserDefinedMethodTerm
{
	private readonly UserDefinedRefMethod reffunc;

	public override UserDefinedFunctionArgument Argument
	{
		get
		{
			throw new CodeEE("引数のない関数参照" + reffunc.Name + "を呼び出しました");
		}
	}

	public override CalledFunction Call
	{
		get
		{
			throw new CodeEE("引数のない関数参照" + reffunc.Name + "を呼び出しました");
		}
	}

	public UserDefinedRefMethodNoArgTerm(UserDefinedRefMethod reffunc)
		: base(reffunc.RetType)
	{
		this.reffunc = reffunc;
	}

	public string GetRefName()
	{
		if (reffunc.CalledFunction == null)
		{
			return "";
		}
		return reffunc.CalledFunction.TopLabel.LabelName;
	}

	public override long GetIntValue(ExpressionMediator exm)
	{
		throw new CodeEE("引数のない関数参照" + reffunc.Name + "を呼び出しました");
	}

	public override string GetStrValue(ExpressionMediator exm)
	{
		throw new CodeEE("引数のない関数参照" + reffunc.Name + "を呼び出しました");
	}

	public override SingleTerm GetValue(ExpressionMediator exm)
	{
		throw new CodeEE("引数のない関数参照" + reffunc.Name + "を呼び出しました");
	}

	public override IOperandTerm Restructure(ExpressionMediator exm)
	{
		return this;
	}
}
