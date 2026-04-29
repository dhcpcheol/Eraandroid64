using System;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.GameProc;

namespace MinorShift.Emuera.GameData.Function;

internal sealed class UserDefinedRefMethod
{
	public CalledFunction CalledFunction { get; private set; }

	public string Name { get; private set; }

	public Type RetType { get; private set; }

	public UserDifinedFunctionDataArgType[] ArgTypeList { get; private set; }

	internal static UserDefinedRefMethod Create(UserDefinedFunctionData funcData)
	{
		UserDefinedRefMethod userDefinedRefMethod = new UserDefinedRefMethod();
		userDefinedRefMethod.Name = funcData.Name;
		if (funcData.TypeIsStr)
		{
			userDefinedRefMethod.RetType = typeof(string);
		}
		else
		{
			userDefinedRefMethod.RetType = typeof(long);
		}
		userDefinedRefMethod.ArgTypeList = funcData.ArgList;
		return userDefinedRefMethod;
	}

	internal bool MatchType(CalledFunction call)
	{
		FunctionLabelLine topLabel = call.TopLabel;
		if (topLabel.IsError)
		{
			return false;
		}
		if (RetType != topLabel.MethodType)
		{
			return false;
		}
		if (ArgTypeList.Length != topLabel.Arg.Length)
		{
			return false;
		}
		for (int i = 0; i < ArgTypeList.Length; i++)
		{
			VariableToken identifier = topLabel.Arg[i].Identifier;
			if (identifier.IsReference)
			{
				UserDifinedFunctionDataArgType userDifinedFunctionDataArgType = UserDifinedFunctionDataArgType.__Ref;
				userDifinedFunctionDataArgType += identifier.Dimension;
				userDifinedFunctionDataArgType = ((!identifier.IsInteger) ? (userDifinedFunctionDataArgType | UserDifinedFunctionDataArgType.Str) : (userDifinedFunctionDataArgType | UserDifinedFunctionDataArgType.Int));
				if (ArgTypeList[i] != userDifinedFunctionDataArgType)
				{
					return false;
				}
			}
			else
			{
				if (identifier.IsInteger && ArgTypeList[i] != UserDifinedFunctionDataArgType.Int)
				{
					return false;
				}
				if (identifier.IsString && ArgTypeList[i] != UserDifinedFunctionDataArgType.Str)
				{
					return false;
				}
			}
		}
		return true;
	}

	internal bool MatchType(UserDefinedRefMethod rother)
	{
		if (RetType != rother.RetType)
		{
			return false;
		}
		if (ArgTypeList.Length != rother.ArgTypeList.Length)
		{
			return false;
		}
		for (int i = 0; i < ArgTypeList.Length; i++)
		{
			if (ArgTypeList[i] != rother.ArgTypeList[i])
			{
				return false;
			}
		}
		return true;
	}

	internal void SetReference(CalledFunction call)
	{
		CalledFunction = call;
	}
}
