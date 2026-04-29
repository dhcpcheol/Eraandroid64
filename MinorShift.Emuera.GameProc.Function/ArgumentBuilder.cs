using System;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc.Function;

internal abstract class ArgumentBuilder
{
	protected Type[] argumentTypeArray;

	protected int minArg = -1;

	protected bool argAny;

	protected void assignwarn(string mes, InstructionLine line, int level, bool isBackComp)
	{
		bool flag = level >= 2;
		if (flag)
		{
			line.IsError = true;
			line.ErrMes = mes;
		}
		ParserMediator.Warn(mes, line, level, flag, isBackComp);
	}

	protected void warn(string mes, InstructionLine line, int level, bool isBackComp)
	{
		mes = line.Function.Name + "命令:" + mes;
		bool flag = level >= 2;
		if (flag)
		{
			line.IsError = true;
			line.ErrMes = mes;
		}
		ParserMediator.Warn(mes, line, level, flag, isBackComp);
	}

	protected bool checkArgumentType(InstructionLine line, ExpressionMediator exm, IOperandTerm[] arguments)
	{
		if (arguments == null)
		{
			warn("引数がありません", line, 2, isBackComp: false);
			return false;
		}
		if (arguments.Length < minArg || (arguments.Length < argumentTypeArray.Length && minArg < 0))
		{
			warn("引数が足りません", line, 2, isBackComp: false);
			return false;
		}
		int num = arguments.Length;
		if (arguments.Length > argumentTypeArray.Length && !argAny)
		{
			warn("引数が多すぎます", line, 1, isBackComp: false);
			num = argumentTypeArray.Length;
		}
		for (int i = 0; i < num; i++)
		{
			if (!argAny && argumentTypeArray[i] == null)
			{
				continue;
			}
			Type type = ((!argAny || i < argumentTypeArray.Length) ? argumentTypeArray[i] : argumentTypeArray[argumentTypeArray.Length - 1]);
			if (arguments[i] == null)
			{
				if (!(type == null))
				{
					warn("第" + (i + 1) + "引数を認識できません", line, 2, isBackComp: false);
					return false;
				}
			}
			else if (type != typeof(void) && type != arguments[i].GetOperandType())
			{
				warn("第" + (i + 1) + "引数の型が正しくありません", line, 2, isBackComp: false);
				return false;
			}
		}
		num = arguments.Length;
		for (int j = 0; j < num; j++)
		{
			if (arguments[j] != null)
			{
				arguments[j] = arguments[j].Restructure(exm);
			}
		}
		return true;
	}

	protected VariableTerm getChangeableVariable(IOperandTerm[] terms, int i, InstructionLine line)
	{
		if (!(terms[i - 1] is VariableTerm variableTerm))
		{
			warn("第" + i + "引数に変数以外を指定することはできません", line, 2, isBackComp: false);
			return null;
		}
		if (variableTerm.Identifier.IsConst)
		{
			warn("第" + i + "引数に変更できない変数を指定することはできません", line, 2, isBackComp: false);
			return null;
		}
		return variableTerm;
	}

	protected WordCollection popWords(InstructionLine line)
	{
		return LexicalAnalyzer.Analyse(line.PopArgumentPrimitive(), LexEndWith.EoL, LexAnalyzeFlag.None);
	}

	protected IOperandTerm[] popTerms(InstructionLine line)
	{
		return ExpressionParser.ReduceArguments(LexicalAnalyzer.Analyse(line.PopArgumentPrimitive(), LexEndWith.EoL, LexAnalyzeFlag.None), ArgsEndWith.EoL, isDefine: false);
	}

	public abstract Argument CreateArgument(InstructionLine line, ExpressionMediator exm);
}
