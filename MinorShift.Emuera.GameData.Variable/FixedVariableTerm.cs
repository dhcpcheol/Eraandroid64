using System;
using MinorShift.Emuera.GameData.Expression;

namespace MinorShift.Emuera.GameData.Variable;

internal sealed class FixedVariableTerm : VariableTerm
{
	public long Index1
	{
		get
		{
			return transporter[0];
		}
		set
		{
			transporter[0] = value;
		}
	}

	public long Index2
	{
		get
		{
			return transporter[1];
		}
		set
		{
			transporter[1] = value;
		}
	}

	public long Index3
	{
		get
		{
			return transporter[2];
		}
		set
		{
			transporter[2] = value;
		}
	}

	public FixedVariableTerm(VariableToken token)
		: base(token)
	{
		Identifier = token;
		transporter = new long[3];
		allArgIsConst = true;
	}

	public FixedVariableTerm(VariableToken token, long[] args)
		: base(token)
	{
		allArgIsConst = true;
		Identifier = token;
		transporter = new long[3];
		for (int i = 0; i < args.Length; i++)
		{
			transporter[i] = args[i];
		}
	}

	public override long GetIntValue(ExpressionMediator exm)
	{
		try
		{
			return Identifier.GetIntValue(exm, transporter);
		}
		catch (Exception ex)
		{
			if (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException || ex is OverflowException)
			{
				Identifier.CheckElement(transporter);
			}
			throw;
		}
	}

	public override string GetStrValue(ExpressionMediator exm)
	{
		try
		{
			string strValue = Identifier.GetStrValue(exm, transporter);
			if (strValue == null)
			{
				return "";
			}
			return strValue;
		}
		catch (Exception ex)
		{
			if (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException || ex is OverflowException)
			{
				Identifier.CheckElement(transporter);
			}
			throw;
		}
	}

	public override void SetValue(long value, ExpressionMediator exm)
	{
		try
		{
			Identifier.SetValue(value, transporter);
		}
		catch (Exception ex)
		{
			if (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException || ex is OverflowException)
			{
				Identifier.CheckElement(transporter);
			}
			throw;
		}
	}

	public override void SetValue(string value, ExpressionMediator exm)
	{
		try
		{
			Identifier.SetValue(value, transporter);
		}
		catch (Exception ex)
		{
			if (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException || ex is OverflowException)
			{
				Identifier.CheckElement(transporter);
			}
			throw;
		}
	}

	public override long PlusValue(long value, ExpressionMediator exm)
	{
		try
		{
			return Identifier.PlusValue(value, transporter);
		}
		catch (Exception ex)
		{
			if (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException || ex is OverflowException)
			{
				Identifier.CheckElement(transporter);
			}
			throw;
		}
	}

	public override IOperandTerm Restructure(ExpressionMediator exm)
	{
		if (Identifier.CanRestructure)
		{
			return GetValue(exm);
		}
		return this;
	}

	public void IsArrayRangeValid(long index1, long index2, string funcName, long i1, long i2)
	{
		Identifier.IsArrayRangeValid(transporter, index1, index2, funcName, i1, i2);
	}
}
