using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Function;
using MinorShift.Emuera.GameData.Variable;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class RefArgument : Argument
{
	public readonly UserDefinedRefMethod RefMethodToken;

	public readonly UserDefinedRefMethod SrcRefMethodToken;

	public readonly CalledFunction SrcCalledFunction;

	public readonly ReferenceToken RefVarToken;

	public readonly VariableToken SrcVarToken;

	public readonly IOperandTerm SrcTerm;

	public RefArgument(UserDefinedRefMethod udrm, UserDefinedRefMethod src)
	{
		RefMethodToken = udrm;
		SrcRefMethodToken = src;
	}

	public RefArgument(UserDefinedRefMethod udrm, CalledFunction src)
	{
		RefMethodToken = udrm;
		SrcCalledFunction = src;
	}

	public RefArgument(UserDefinedRefMethod udrm, IOperandTerm src)
	{
		RefMethodToken = udrm;
		SrcTerm = src;
	}

	public RefArgument(ReferenceToken vt, VariableToken src)
	{
		RefVarToken = vt;
		SrcVarToken = src;
	}

	public RefArgument(ReferenceToken vt, IOperandTerm src)
	{
		RefVarToken = vt;
		SrcTerm = src;
	}
}
