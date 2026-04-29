using System;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameProc;

namespace MinorShift.Emuera.GameData.Function;

internal sealed class UserDefinedMethodTerm : SuperUserDefinedMethodTerm
{
	private readonly UserDefinedFunctionArgument argment;

	private readonly CalledFunction called;

	public override UserDefinedFunctionArgument Argument => argment;

	public override CalledFunction Call => called;

	public static UserDefinedMethodTerm Create(FunctionLabelLine targetLabel, IOperandTerm[] srcArgs, out string errMes)
	{
		CalledFunction calledFunction = CalledFunction.CreateCalledFunctionMethod(targetLabel, targetLabel.LabelName);
		UserDefinedFunctionArgument userDefinedFunctionArgument = calledFunction.ConvertArg(srcArgs, out errMes);
		if (userDefinedFunctionArgument == null)
		{
			return null;
		}
		return new UserDefinedMethodTerm(userDefinedFunctionArgument, calledFunction.TopLabel.MethodType, calledFunction);
	}

	private UserDefinedMethodTerm(UserDefinedFunctionArgument arg, Type returnType, CalledFunction call)
		: base(returnType)
	{
		argment = arg;
		called = call;
	}

	public override IOperandTerm Restructure(ExpressionMediator exm)
	{
		Argument.Restructure(exm);
		return this;
	}
}
