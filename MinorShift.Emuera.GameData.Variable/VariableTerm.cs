using System;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Variable;

internal class VariableTerm : IOperandTerm
{
	public VariableToken Identifier;

	private readonly IOperandTerm[] arguments;

	protected long[] transporter;

	protected bool allArgIsConst;

	public bool isAllConst => allArgIsConst;

	public int getEl1forArg => (int)transporter[0];

	protected VariableTerm(VariableToken token)
		: base(token.VariableType)
	{
	}

	public VariableTerm(VariableToken token, IOperandTerm[] args)
		: base(token.VariableType)
	{
		Identifier = token;
		arguments = args;
		transporter = new long[arguments.Length];
		allArgIsConst = false;
		for (int i = 0; i < arguments.Length; i++)
		{
			if (!(arguments[i] is SingleTerm))
			{
				return;
			}
			transporter[i] = ((SingleTerm)arguments[i]).Int;
		}
		allArgIsConst = true;
	}

	public long GetElementInt(int i, ExpressionMediator exm)
	{
		if (allArgIsConst)
		{
			return transporter[i];
		}
		return arguments[i].GetIntValue(exm);
	}

	public override long GetIntValue(ExpressionMediator exm)
	{
		try
		{
			if (!allArgIsConst)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					transporter[i] = arguments[i].GetIntValue(exm);
				}
			}
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
			if (!allArgIsConst)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					transporter[i] = arguments[i].GetIntValue(exm);
				}
			}
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

	public virtual void SetValue(long value, ExpressionMediator exm)
	{
		try
		{
			if (!allArgIsConst)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					transporter[i] = arguments[i].GetIntValue(exm);
				}
			}
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

	public virtual void SetValue(string value, ExpressionMediator exm)
	{
		try
		{
			if (!allArgIsConst)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					transporter[i] = arguments[i].GetIntValue(exm);
				}
			}
			Identifier.SetValue(value, transporter);
		}
		catch (Exception ex)
		{
			if (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException || ex is OverflowException)
			{
				Identifier.CheckElement(transporter);
			}
			throw ex;
		}
	}

	public virtual void SetValue(long[] array, ExpressionMediator exm)
	{
		try
		{
			if (!allArgIsConst)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					transporter[i] = arguments[i].GetIntValue(exm);
				}
			}
			Identifier.SetValue(array, transporter);
		}
		catch (Exception ex)
		{
			if (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException || ex is OverflowException)
			{
				Identifier.CheckElement(transporter);
				throw new CodeEE("配列変数" + Identifier.Name + "の要素数を超えて代入しようとしました");
			}
			throw;
		}
	}

	public virtual void SetValue(string[] array, ExpressionMediator exm)
	{
		try
		{
			if (!allArgIsConst)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					transporter[i] = arguments[i].GetIntValue(exm);
				}
			}
			Identifier.SetValue(array, transporter);
		}
		catch (Exception ex)
		{
			if (ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException || ex is OverflowException)
			{
				Identifier.CheckElement(transporter);
				throw new CodeEE("配列変数" + Identifier.Name + "の要素数を超えて代入しようとしました");
			}
			throw;
		}
	}

	public virtual long PlusValue(long value, ExpressionMediator exm)
	{
		try
		{
			if (!allArgIsConst)
			{
				for (int i = 0; i < arguments.Length; i++)
				{
					transporter[i] = arguments[i].GetIntValue(exm);
				}
			}
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

	public override SingleTerm GetValue(ExpressionMediator exm)
	{
		if (Identifier.VariableType == typeof(long))
		{
			return new SingleTerm(GetIntValue(exm));
		}
		return new SingleTerm(GetStrValue(exm));
	}

	public virtual void SetValue(SingleTerm value, ExpressionMediator exm)
	{
		if (Identifier.VariableType == typeof(long))
		{
			SetValue(value.Int, exm);
		}
		else
		{
			SetValue(value.Str, exm);
		}
	}

	public virtual void SetValue(IOperandTerm value, ExpressionMediator exm)
	{
		if (Identifier.VariableType == typeof(long))
		{
			SetValue(value.GetIntValue(exm), exm);
		}
		else
		{
			SetValue(value.GetStrValue(exm), exm);
		}
	}

	public int GetLength()
	{
		return Identifier.GetLength();
	}

	public int GetLength(int dimension)
	{
		return Identifier.GetLength(dimension);
	}

	public int GetLastLength()
	{
		if (Identifier.IsArray1D)
		{
			return Identifier.GetLength();
		}
		if (Identifier.IsArray2D)
		{
			return Identifier.GetLength(1);
		}
		if (Identifier.IsArray3D)
		{
			return Identifier.GetLength(2);
		}
		return 0;
	}

	public virtual FixedVariableTerm GetFixedVariableTerm(ExpressionMediator exm)
	{
		if (!allArgIsConst)
		{
			for (int i = 0; i < arguments.Length; i++)
			{
				transporter[i] = arguments[i].GetIntValue(exm);
			}
		}
		FixedVariableTerm fixedVariableTerm = new FixedVariableTerm(Identifier);
		if (transporter.Length >= 1)
		{
			fixedVariableTerm.Index1 = transporter[0];
		}
		if (transporter.Length >= 2)
		{
			fixedVariableTerm.Index2 = transporter[1];
		}
		if (transporter.Length >= 3)
		{
			fixedVariableTerm.Index3 = transporter[2];
		}
		return fixedVariableTerm;
	}

	public override IOperandTerm Restructure(ExpressionMediator exm)
	{
		bool[] array = new bool[arguments.Length];
		allArgIsConst = true;
		for (int i = 0; i < arguments.Length; i++)
		{
			arguments[i] = arguments[i].Restructure(exm);
			if (!(arguments[i] is SingleTerm))
			{
				allArgIsConst = false;
				array[i] = false;
				continue;
			}
			if ((i == 0 && Identifier.IsCharacterData) || Identifier.Name == "ARG" || Identifier.Name == "ARGS")
			{
				array[i] = false;
			}
			else
			{
				array[i] = true;
			}
			transporter[i] = arguments[i].GetIntValue(exm);
		}
		if (!Identifier.IsReference)
		{
			Identifier.CheckElement(transporter, array);
		}
		if (Identifier.CanRestructure && allArgIsConst)
		{
			return GetValue(exm);
		}
		if (allArgIsConst)
		{
			return new FixedVariableTerm(Identifier, transporter);
		}
		return this;
	}

	public bool checkSameTerm(VariableTerm term)
	{
		if (!allArgIsConst)
		{
			return false;
		}
		if (Identifier.Name != term.Identifier.Name)
		{
			return false;
		}
		for (int i = 0; i < transporter.Length; i++)
		{
			if (transporter[i] != term.transporter[i])
			{
				return false;
			}
		}
		return true;
	}

	public string GetFullString()
	{
		if (!allArgIsConst)
		{
			return "";
		}
		if (Identifier.IsArray1D)
		{
			return Identifier.Name + ":" + transporter[0];
		}
		if (Identifier.IsArray2D)
		{
			return Identifier.Name + ":" + transporter[0] + ":" + transporter[1];
		}
		if (Identifier.IsArray3D)
		{
			return Identifier.Name + ":" + transporter[0] + ":" + transporter[1] + ":" + transporter[2];
		}
		return Identifier.Name;
	}
}
