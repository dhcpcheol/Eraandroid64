using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameProc;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Function;

internal sealed class UserDefinedRefMethodTerm : SuperUserDefinedMethodTerm
{
	private IOperandTerm[] srcArgs;

	private readonly UserDefinedRefMethod reffunc;

	public override UserDefinedFunctionArgument Argument
	{
		get
		{
			if (reffunc.CalledFunction == null)
			{
				throw new CodeEE("何も参照していない関数参照" + reffunc.Name + "を呼び出しました");
			}
			string errMes;
			return reffunc.CalledFunction.ConvertArg(srcArgs, out errMes) ?? throw new CodeEE(errMes);
		}
	}

	public override CalledFunction Call
	{
		get
		{
			if (reffunc.CalledFunction == null)
			{
				throw new CodeEE("何も参照していない関数参照" + reffunc.Name + "を呼び出しました");
			}
			return reffunc.CalledFunction;
		}
	}

	public UserDefinedRefMethodTerm(UserDefinedRefMethod reffunc, IOperandTerm[] srcArgs)
		: base(reffunc.RetType)
	{
		this.srcArgs = srcArgs;
		this.reffunc = reffunc;
	}

	public override IOperandTerm Restructure(ExpressionMediator exm)
	{
		for (int i = 0; i < srcArgs.Length; i++)
		{
			if ((reffunc.ArgTypeList[i] & UserDifinedFunctionDataArgType.__Ref) == UserDifinedFunctionDataArgType.__Ref)
			{
				srcArgs[i].Restructure(exm);
			}
			else
			{
				srcArgs[i] = srcArgs[i].Restructure(exm);
			}
		}
		return this;
	}
}
