using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MinorShift._Library;
using MinorShift.Emuera.GameData;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Function;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.GameView;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameProc.Function;

internal sealed class FunctionIdentifier
{
	private sealed class PRINT_Instruction : AbstractInstruction
	{
		private readonly bool isPrintV;

		private readonly bool isLC;

		private readonly bool isC;

		private readonly bool isForms;

		public PRINT_Instruction(string name)
		{
			flag = 8192;
			StringStream stringStream = new StringStream(name);
			stringStream.Jump(5);
			if (stringStream.CurrentEqualTo("SINGLE"))
			{
				flag |= 1026;
				stringStream.Jump(6);
			}
			if (stringStream.CurrentEqualTo("V"))
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_PRINTV);
				isPrintV = true;
				stringStream.Jump(1);
			}
			else if (stringStream.CurrentEqualTo("S"))
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.STR_EXPRESSION);
				stringStream.Jump(1);
			}
			else if (stringStream.CurrentEqualTo("FORMS"))
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.STR_EXPRESSION);
				isForms = true;
				stringStream.Jump(5);
			}
			else if (stringStream.CurrentEqualTo("FORM"))
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.FORM_STR_NULLABLE);
				stringStream.Jump(4);
			}
			else
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.STR_NULLABLE);
			}
			if (stringStream.CurrentEqualTo("LC"))
			{
				flag |= 2;
				isLC = true;
				stringStream.Jump(2);
			}
			else if (stringStream.CurrentEqualTo("C"))
			{
				if (name == "PRINTFORMC")
				{
					flag |= 2;
				}
				isC = true;
				stringStream.Jump(1);
			}
			if (stringStream.CurrentEqualTo("K"))
			{
				flag |= 4098;
				stringStream.Jump(1);
			}
			if (stringStream.CurrentEqualTo("D"))
			{
				flag |= 2050;
				stringStream.Jump(1);
			}
			if (stringStream.CurrentEqualTo("L"))
			{
				flag |= 256;
				flag |= 4;
				stringStream.Jump(1);
			}
			else if (stringStream.CurrentEqualTo("W"))
			{
				flag |= 768;
				stringStream.Jump(1);
			}
			else
			{
				flag |= 4;
			}
			if (base.ArgBuilder == null || !stringStream.EOS)
			{
				throw new ExeEE("PRINT異常");
			}
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.Console.UseUserStyle = true;
			exm.Console.UseSetColorStyle = !func.Function.IsPrintDFunction();
			string text = null;
			if (func.Argument.IsConst)
			{
				text = func.Argument.ConstStr;
			}
			else if (isPrintV)
			{
				StringBuilder stringBuilder = new StringBuilder();
				IOperandTerm[] terms = ((SpPrintVArgument)func.Argument).Terms;
				foreach (IOperandTerm operandTerm in terms)
				{
					if (operandTerm.GetOperandType() == typeof(long))
					{
						stringBuilder.Append(operandTerm.GetIntValue(exm).ToString());
					}
					else
					{
						stringBuilder.Append(operandTerm.GetStrValue(exm));
					}
				}
				text = stringBuilder.ToString();
			}
			else
			{
				text = ((ExpressionArgument)func.Argument).Term.GetStrValue(exm);
				if (isForms)
				{
					text = exm.CheckEscape(text);
					text = StrForm.FromWordToken(LexicalAnalyzer.AnalyseFormattedString(new StringStream(text), FormStrEndWith.EoL, trim: false)).GetString(exm);
				}
			}
			if (func.Function.IsPrintKFunction())
			{
				text = exm.ConvertStringType(text);
			}
			if (isC)
			{
				exm.Console.PrintC(text, alignmentRight: true);
			}
			else if (isLC)
			{
				exm.Console.PrintC(text, alignmentRight: false);
			}
			else
			{
				exm.OutputToConsole(text, func.Function);
			}
			exm.Console.UseSetColorStyle = true;
		}
	}

	private sealed class PRINT_DATA_Instruction : AbstractInstruction
	{
		public PRINT_DATA_Instruction(string name)
		{
			flag = 73746;
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VAR_INT);
			StringStream stringStream = new StringStream(name);
			stringStream.Jump(9);
			if (stringStream.CurrentEqualTo("K"))
			{
				flag |= 4098;
				stringStream.Jump(1);
			}
			if (stringStream.CurrentEqualTo("D"))
			{
				flag |= 2050;
				stringStream.Jump(1);
			}
			if (stringStream.CurrentEqualTo("L"))
			{
				flag |= 256;
				flag |= 4;
				stringStream.Jump(1);
			}
			else if (stringStream.CurrentEqualTo("W"))
			{
				flag |= 768;
				stringStream.Jump(1);
			}
			else
			{
				flag |= 4;
			}
			if (base.ArgBuilder == null || !stringStream.EOS)
			{
				throw new ExeEE("PRINTDATA異常");
			}
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.Console.UseUserStyle = true;
			exm.Console.UseSetColorStyle = !func.Function.IsPrintDFunction();
			if (func.dataList.Count == 0)
			{
				state.JumpTo(func.JumpTo);
				return;
			}
			int count = func.dataList.Count;
			int num = (int)exm.VEvaluator.GetNextRand(count);
			((PrintDataArgument)func.Argument).Var?.SetValue(num, exm);
			List<InstructionLine> list = func.dataList[num];
			int num2 = 0;
			string text = null;
			foreach (InstructionLine item in list)
			{
				InstructionLine instructionLine = (InstructionLine)(state.CurrentLine = item);
				if (instructionLine.Argument == null)
				{
					ArgumentParser.SetArgumentTo(instructionLine);
				}
				text = ((ExpressionArgument)instructionLine.Argument).Term.GetStrValue(exm);
				if (func.Function.IsPrintKFunction())
				{
					text = exm.ConvertStringType(text);
				}
				exm.Console.Print(text);
				if (++num2 < list.Count)
				{
					exm.Console.NewLine();
				}
			}
			if (func.Function.IsNewLine() || func.Function.IsWaitInput())
			{
				exm.Console.NewLine();
				if (func.Function.IsWaitInput())
				{
					exm.Console.ReadAnyKey();
				}
			}
			exm.Console.UseSetColorStyle = true;
			state.JumpTo(func.JumpTo);
		}
	}

	private sealed class HTML_PRINT_Instruction : AbstractInstruction
	{
		public HTML_PRINT_Instruction()
		{
			flag = 6;
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.STR_EXPRESSION);
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			string text = null;
			text = ((!func.Argument.IsConst) ? ((ExpressionArgument)func.Argument).Term.GetStrValue(exm) : func.Argument.ConstStr);
			exm.Console.PrintHtml(text);
		}
	}

	private sealed class HTML_TAGSPLIT_Instruction : AbstractInstruction
	{
		public HTML_TAGSPLIT_Instruction()
		{
			flag = 6;
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_HTMLSPLIT);
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpHtmlSplitArgument spHtmlSplitArgument = (SpHtmlSplitArgument)func.Argument;
			string[] array = HtmlManager.HtmlTagSplit(spHtmlSplitArgument.TargetStr.GetStrValue(exm));
			if (array == null)
			{
				spHtmlSplitArgument.Num.SetValue(-1L, exm);
				return;
			}
			spHtmlSplitArgument.Num.SetValue(array.Length, exm);
			string[] array2 = (string[])spHtmlSplitArgument.Var.GetArray();
			int length = Math.Min(array2.Length, array.Length);
			Array.Copy(array, array2, length);
		}
	}

	private sealed class PRINT_IMG_Instruction : AbstractInstruction
	{
		public PRINT_IMG_Instruction()
		{
			flag = 6;
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.STR_EXPRESSION);
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			string text = null;
			text = ((!func.Argument.IsConst) ? ((ExpressionArgument)func.Argument).Term.GetStrValue(exm) : func.Argument.ConstStr);
			exm.Console.PrintImg(text);
		}
	}

	private sealed class PRINT_RECT_Instruction : AbstractInstruction
	{
		public PRINT_RECT_Instruction()
		{
			flag = 6;
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_ANY);
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			ExpressionArrayArgument expressionArrayArgument = (ExpressionArrayArgument)func.Argument;
			int[] array = new int[expressionArrayArgument.TermList.Length];
			for (int i = 0; i < expressionArrayArgument.TermList.Length; i++)
			{
				array[i] = toUInt32inArg(expressionArrayArgument.TermList[i].GetIntValue(exm), "DELDATA", i + 1);
			}
			exm.Console.PrintShape("rect", array);
		}
	}

	private sealed class PRINT_SPACE_Instruction : AbstractInstruction
	{
		public PRINT_SPACE_Instruction()
		{
			flag = 6;
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_EXPRESSION);
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			long value = ((!func.Argument.IsConst) ? ((ExpressionArgument)func.Argument).Term.GetIntValue(exm) : func.Argument.ConstInt);
			int num = toUInt32inArg(value, "DELDATA", 1);
			exm.Console.PrintShape("space", new int[1] { num });
		}
	}

	private sealed class DEBUGPRINT_Instruction : AbstractInstruction
	{
		public DEBUGPRINT_Instruction(bool form, bool newline)
		{
			if (form)
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.FORM_STR_NULLABLE);
			}
			else
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.STR_NULLABLE);
			}
			flag = 14;
			if (newline)
			{
				flag |= 256;
			}
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			string text = null;
			text = ((!func.Argument.IsConst) ? ((ExpressionArgument)func.Argument).Term.GetStrValue(exm) : func.Argument.ConstStr);
			exm.Console.DebugPrint(text);
			if (func.Function.IsNewLine())
			{
				exm.Console.DebugNewLine();
			}
		}
	}

	private sealed class DEBUGCLEAR_Instruction : AbstractInstruction
	{
		public DEBUGCLEAR_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 14;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.Console.DebugClear();
		}
	}

	private sealed class METHOD_Instruction : AbstractInstruction
	{
		public METHOD_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.METHOD);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			IOperandTerm methodTerm = ((MethodArgument)func.Argument).MethodTerm;
			methodTerm.GetOperandType();
			if (methodTerm.GetOperandType() == typeof(long))
			{
				exm.VEvaluator.RESULT = methodTerm.GetIntValue(exm);
			}
			else
			{
				exm.VEvaluator.RESULTS = methodTerm.GetStrValue(exm);
			}
		}
	}

	private sealed class SET_Instruction : AbstractInstruction
	{
		public SET_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_SET);
			flag = 4;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			if (func.Argument is SpSetArrayArgument)
			{
				SpSetArrayArgument spSetArrayArgument = (SpSetArrayArgument)func.Argument;
				if (spSetArrayArgument.VariableDest.IsInteger)
				{
					if (spSetArrayArgument.IsConst)
					{
						spSetArrayArgument.VariableDest.SetValue(spSetArrayArgument.ConstIntList, exm);
						return;
					}
					long[] array = new long[spSetArrayArgument.TermList.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = spSetArrayArgument.TermList[i].GetIntValue(exm);
					}
					spSetArrayArgument.VariableDest.SetValue(array, exm);
				}
				else if (spSetArrayArgument.IsConst)
				{
					spSetArrayArgument.VariableDest.SetValue(spSetArrayArgument.ConstStrList, exm);
				}
				else
				{
					string[] array2 = new string[spSetArrayArgument.TermList.Length];
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j] = spSetArrayArgument.TermList[j].GetStrValue(exm);
					}
					spSetArrayArgument.VariableDest.SetValue(array2, exm);
				}
				return;
			}
			SpSetArgument spSetArgument = (SpSetArgument)func.Argument;
			if (spSetArgument.VariableDest.IsInteger)
			{
				long value = (spSetArgument.IsConst ? spSetArgument.ConstInt : spSetArgument.Term.GetIntValue(exm));
				if (spSetArgument.AddConst)
				{
					spSetArgument.VariableDest.PlusValue(value, exm);
				}
				else
				{
					spSetArgument.VariableDest.SetValue(value, exm);
				}
			}
			else
			{
				string value2 = (spSetArgument.IsConst ? spSetArgument.ConstStr : spSetArgument.Term.GetStrValue(exm));
				spSetArgument.VariableDest.SetValue(value2, exm);
			}
		}
	}

	private sealed class REUSELASTLINE_Instruction : AbstractInstruction
	{
		public REUSELASTLINE_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.FORM_STR_NULLABLE);
			flag = 8198;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			string strValue = ((ExpressionArgument)func.Argument).Term.GetStrValue(exm);
			exm.Console.PrintTemporaryLine(strValue);
		}
	}

	private sealed class CLEARLINE_Instruction : AbstractInstruction
	{
		public CLEARLINE_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_EXPRESSION);
			flag = 8198;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			int argNum = (int)((ExpressionArgument)func.Argument).Term.GetIntValue(exm);
			exm.Console.deleteLine(argNum);
			exm.Console.RefreshStrings(force_Paint: false);
		}
	}

	private sealed class STRLEN_Instruction : AbstractInstruction
	{
		private bool unicode;

		public STRLEN_Instruction(bool argisform, bool unicode)
		{
			if (argisform)
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.FORM_STR_NULLABLE);
			}
			else
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.STR_NULLABLE);
			}
			flag = 6;
			this.unicode = unicode;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			string text = null;
			text = ((!func.Argument.IsConst) ? ((ExpressionArgument)func.Argument).Term.GetStrValue(exm) : func.Argument.ConstStr);
			if (unicode)
			{
				exm.VEvaluator.RESULT = text.Length;
			}
			else
			{
				exm.VEvaluator.RESULT = LangManager.GetStrlenLang(text);
			}
		}
	}

	private sealed class SETBIT_Instruction : AbstractInstruction
	{
		private int op;

		public SETBIT_Instruction(int op)
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.BIT_ARG);
			flag = 6;
			this.op = op;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			BitArgument obj = (BitArgument)func.Argument;
			VariableTerm variableDest = obj.VariableDest;
			IOperandTerm[] term = obj.Term;
			for (int i = 0; i < term.Length; i++)
			{
				long intValue = term[i].GetIntValue(exm);
				if (intValue < 0 || intValue > 63)
				{
					throw new CodeEE("第2引数がビットのレンジ(0から63)を超えています");
				}
				long intValue2 = variableDest.GetIntValue(exm);
				long num = 1L << (int)intValue;
				intValue2 = ((op != 1) ? ((op != 0) ? (intValue2 ^ num) : (intValue2 & ~num)) : (intValue2 | num));
				variableDest.SetValue(intValue2, exm);
			}
		}
	}

	private sealed class WAIT_Instruction : AbstractInstruction
	{
		private bool isForce;

		public WAIT_Instruction(bool force)
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 8192;
			isForce = force;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			if (isForce)
			{
				exm.Console.ReadAnyKey(anykey: false, stopMesskip: true);
			}
			else
			{
				exm.Console.ReadAnyKey();
			}
		}
	}

	private sealed class WAITANYKEY_Instruction : AbstractInstruction
	{
		public WAITANYKEY_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 8192;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.Console.ReadAnyKey(anykey: true);
		}
	}

	private sealed class TWAIT_Instruction : AbstractInstruction
	{
		public TWAIT_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_SWAP);
			flag = 8194;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.Console.ReadAnyKey();
			SpSwapCharaArgument obj = (SpSwapCharaArgument)func.Argument;
			long intValue = obj.X.GetIntValue(exm);
			long intValue2 = obj.Y.GetIntValue(exm);
			InputRequest inputRequest = new InputRequest
			{
				InputType = InputType.EnterKey
			};
			if (intValue2 != 0L)
			{
				inputRequest.InputType = InputType.Void;
			}
			inputRequest.Timelimit = intValue;
			exm.Console.WaitInput(inputRequest);
		}
	}

	private sealed class INPUT_Instruction : AbstractInstruction
	{
		public INPUT_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_INPUT);
			flag = 24576;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			ExpressionArgument expressionArgument = (ExpressionArgument)func.Argument;
			InputRequest inputRequest = new InputRequest();
			inputRequest.InputType = InputType.IntValue;
			if (expressionArgument.Term != null)
			{
				long defIntValue = ((!expressionArgument.IsConst) ? expressionArgument.Term.GetIntValue(exm) : expressionArgument.ConstInt);
				inputRequest.HasDefValue = true;
				inputRequest.DefIntValue = defIntValue;
			}
			exm.Console.WaitInput(inputRequest);
		}
	}

	private sealed class INPUTS_Instruction : AbstractInstruction
	{
		public INPUTS_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_INPUTS);
			flag = 24576;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			ExpressionArgument expressionArgument = (ExpressionArgument)func.Argument;
			InputRequest inputRequest = new InputRequest();
			inputRequest.InputType = InputType.StrValue;
			if (expressionArgument.Term != null)
			{
				string defStrValue = ((!expressionArgument.IsConst) ? expressionArgument.Term.GetStrValue(exm) : expressionArgument.ConstStr);
				inputRequest.HasDefValue = true;
				inputRequest.DefStrValue = defStrValue;
			}
			exm.Console.WaitInput(inputRequest);
		}
	}

	private sealed class ONEINPUT_Instruction : AbstractInstruction
	{
		public ONEINPUT_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_INPUT);
			flag = 24578;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			ExpressionArgument expressionArgument = (ExpressionArgument)func.Argument;
			InputRequest inputRequest = new InputRequest();
			inputRequest.InputType = InputType.IntValue;
			inputRequest.OneInput = true;
			if (expressionArgument.Term != null)
			{
				long num = ((!expressionArgument.IsConst) ? expressionArgument.Term.GetIntValue(exm) : expressionArgument.ConstInt);
				if (num > 9)
				{
					num = long.Parse(num.ToString().Remove(1));
				}
				if (num >= 0)
				{
					inputRequest.HasDefValue = true;
					inputRequest.DefIntValue = num;
				}
			}
			exm.Console.WaitInput(inputRequest);
		}
	}

	private sealed class ONEINPUTS_Instruction : AbstractInstruction
	{
		public ONEINPUTS_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_INPUTS);
			flag = 24578;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			ExpressionArgument expressionArgument = (ExpressionArgument)func.Argument;
			InputRequest inputRequest = new InputRequest();
			inputRequest.InputType = InputType.StrValue;
			inputRequest.OneInput = true;
			if (expressionArgument.Term != null)
			{
				string text = ((!expressionArgument.IsConst) ? expressionArgument.Term.GetStrValue(exm) : expressionArgument.ConstStr);
				if (text.Length > 1)
				{
					text = text.Remove(1);
				}
				if (text.Length > 0)
				{
					inputRequest.HasDefValue = true;
					inputRequest.DefStrValue = text;
				}
			}
			exm.Console.WaitInput(inputRequest);
		}
	}

	private sealed class TINPUT_Instruction : AbstractInstruction
	{
		private bool isOne;

		public TINPUT_Instruction(bool oneInput)
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_TINPUT);
			flag = 24578;
			isOne = oneInput;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpTInputsArgument spTInputsArgument = (SpTInputsArgument)func.Argument;
			InputRequest inputRequest = new InputRequest();
			inputRequest.InputType = InputType.IntValue;
			inputRequest.HasDefValue = true;
			inputRequest.OneInput = isOne;
			long intValue = spTInputsArgument.Time.GetIntValue(exm);
			long num = spTInputsArgument.Def.GetIntValue(exm);
			if (isOne)
			{
				if (num < 0)
				{
					num = Math.Abs(num);
				}
				if (num >= 10)
				{
					num /= (long)Math.Pow(10.0, Math.Log10(num));
				}
			}
			long num2 = ((spTInputsArgument.Disp != null) ? spTInputsArgument.Disp.GetIntValue(exm) : 1);
			inputRequest.Timelimit = intValue;
			inputRequest.DefIntValue = num;
			inputRequest.DisplayTime = num2 != 0;
			inputRequest.TimeUpMes = ((spTInputsArgument.Timeout != null) ? spTInputsArgument.Timeout.GetStrValue(exm) : Config.TimeupLabel);
			exm.Console.WaitInput(inputRequest);
		}
	}

	private sealed class TINPUTS_Instruction : AbstractInstruction
	{
		private bool isOne;

		public TINPUTS_Instruction(bool oneInput)
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_TINPUTS);
			flag = 24578;
			isOne = oneInput;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpTInputsArgument spTInputsArgument = (SpTInputsArgument)func.Argument;
			InputRequest inputRequest = new InputRequest();
			inputRequest.InputType = InputType.StrValue;
			inputRequest.HasDefValue = true;
			inputRequest.OneInput = isOne;
			long intValue = spTInputsArgument.Time.GetIntValue(exm);
			string text = spTInputsArgument.Def.GetStrValue(exm);
			if (isOne && text.Length > 1)
			{
				text = text.Remove(1);
			}
			long num = ((spTInputsArgument.Disp != null) ? spTInputsArgument.Disp.GetIntValue(exm) : 1);
			inputRequest.Timelimit = intValue;
			inputRequest.DefStrValue = text;
			inputRequest.DisplayTime = num != 0;
			inputRequest.TimeUpMes = ((spTInputsArgument.Timeout != null) ? spTInputsArgument.Timeout.GetStrValue(exm) : Config.TimeupLabel);
			exm.Console.WaitInput(inputRequest);
		}
	}

	private sealed class CALLF_Instruction : AbstractInstruction
	{
		public CALLF_Instruction(bool form)
		{
			if (form)
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_CALLFORMF);
			}
			else
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_CALLF);
			}
			flag = 38;
		}

		public override void SetJumpTo(ref bool useCallForm, InstructionLine func, int currentDepth, ref string FunctionoNotFoundName)
		{
			if (!func.Argument.IsConst)
			{
				useCallForm = true;
				return;
			}
			SpCallFArgment spCallFArgment = (SpCallFArgment)func.Argument;
			if (Config.ICFunction)
			{
				spCallFArgment.ConstStr = spCallFArgment.ConstStr.ToUpper();
			}
			try
			{
				spCallFArgment.FuncTerm = GlobalStatic.IdentifierDictionary.GetFunctionMethod(GlobalStatic.LabelDictionary, spCallFArgment.ConstStr, spCallFArgment.RowArgs, userDefinedOnly: true);
			}
			catch (CodeEE codeEE)
			{
				ParserMediator.Warn(codeEE.Message, func, 2, isError: true, isBackComp: false);
				return;
			}
			if (spCallFArgment.FuncTerm == null)
			{
				if (!Program.AnalysisMode)
				{
					ParserMediator.Warn("指定された関数名\"@" + spCallFArgment.ConstStr + "\"は存在しません", func, 2, isError: true, isBackComp: false);
				}
				else
				{
					ParserMediator.Warn(spCallFArgment.ConstStr, func, 2, isError: true, isBackComp: false);
				}
			}
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			IOperandTerm operandTerm = null;
			string text = null;
			if (!func.Argument.IsConst || exm.Console.RunERBFromMemory)
			{
				SpCallFArgment spCallFArgment = (SpCallFArgment)func.Argument;
				text = spCallFArgment.FuncnameTerm.GetStrValue(exm);
				operandTerm = GlobalStatic.IdentifierDictionary.GetFunctionMethod(GlobalStatic.LabelDictionary, text, spCallFArgment.RowArgs, userDefinedOnly: true);
			}
			else
			{
				text = func.Argument.ConstStr;
				operandTerm = ((SpCallFArgment)func.Argument).FuncTerm;
			}
			if (operandTerm == null)
			{
				throw new CodeEE("式中関数\"@" + text + "\"が見つかりません");
			}
			operandTerm.GetValue(exm);
		}
	}

	private sealed class BAR_Instruction : AbstractInstruction
	{
		private bool newline;

		public BAR_Instruction(bool newline)
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_BAR);
			flag = 8198;
			this.newline = newline;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpBarArgument obj = (SpBarArgument)func.Argument;
			long intValue = obj.Terms[0].GetIntValue(exm);
			long intValue2 = obj.Terms[1].GetIntValue(exm);
			long intValue3 = obj.Terms[2].GetIntValue(exm);
			exm.Console.Print(exm.CreateBar(intValue, intValue2, intValue3));
			if (newline)
			{
				exm.Console.NewLine();
			}
		}
	}

	private sealed class TIMES_Instruction : AbstractInstruction
	{
		public TIMES_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_TIMES);
			flag = 4;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpTimesArgument spTimesArgument = (SpTimesArgument)func.Argument;
			VariableTerm variableDest = spTimesArgument.VariableDest;
			if (Config.TimesNotRigorousCalculation)
			{
				double num = (double)variableDest.GetIntValue(exm) * spTimesArgument.DoubleValue;
				variableDest.SetValue((long)num, exm);
				return;
			}
			decimal num2 = (decimal)variableDest.GetIntValue(exm) * (decimal)spTimesArgument.DoubleValue;
			if (num2 <= 9223372036854775807m && num2 >= -9223372036854775808m)
			{
				variableDest.SetValue((long)num2, exm);
			}
			else
			{
				variableDest.SetValue((long)(double)num2, exm);
			}
		}
	}

	private sealed class ADDCHARA_Instruction : AbstractInstruction
	{
		private bool isDel;

		private bool isSp;

		public ADDCHARA_Instruction(bool flagSp, bool flagDel)
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_ANY);
			flag = 4;
			isDel = flagDel;
			isSp = flagSp;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			if (!Config.CompatiSPChara && isSp)
			{
				throw new CodeEE("SPキャラ関係の機能は標準では使用できません(互換性オプション「SPキャラを使用する」をONにしてください)");
			}
			ExpressionArrayArgument obj = (ExpressionArrayArgument)func.Argument;
			long num = -1L;
			long[] array = new long[obj.TermList.Length];
			int num2 = 0;
			IOperandTerm[] termList = obj.TermList;
			for (int i = 0; i < termList.Length; i++)
			{
				num = termList[i].GetIntValue(exm);
				if (isDel)
				{
					array[num2] = num;
					num2++;
				}
				else if (!Config.CompatiSPChara)
				{
					exm.VEvaluator.AddCharacter_UseSp(num, isSp);
				}
				else
				{
					exm.VEvaluator.AddCharacter(num);
				}
			}
			if (isDel)
			{
				if (array.Length == 1)
				{
					exm.VEvaluator.DelCharacter(array[0]);
				}
				else
				{
					exm.VEvaluator.DelCharacter(array);
				}
			}
		}
	}

	private sealed class ADDVOIDCHARA_Instruction : AbstractInstruction
	{
		public ADDVOIDCHARA_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.VEvaluator.AddPseudoCharacter();
		}
	}

	private sealed class SWAPCHARA_Instruction : AbstractInstruction
	{
		public SWAPCHARA_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_SWAP);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpSwapCharaArgument obj = (SpSwapCharaArgument)func.Argument;
			long intValue = obj.X.GetIntValue(exm);
			long intValue2 = obj.Y.GetIntValue(exm);
			exm.VEvaluator.SwapChara(intValue, intValue2);
		}
	}

	private sealed class COPYCHARA_Instruction : AbstractInstruction
	{
		public COPYCHARA_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_SWAP);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpSwapCharaArgument obj = (SpSwapCharaArgument)func.Argument;
			long intValue = obj.X.GetIntValue(exm);
			long intValue2 = obj.Y.GetIntValue(exm);
			exm.VEvaluator.CopyChara(intValue, intValue2);
		}
	}

	private sealed class ADDCOPYCHARA_Instruction : AbstractInstruction
	{
		public ADDCOPYCHARA_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_ANY);
			flag = 4;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			IOperandTerm[] termList = ((ExpressionArrayArgument)func.Argument).TermList;
			foreach (IOperandTerm operandTerm in termList)
			{
				exm.VEvaluator.AddCopyChara(operandTerm.GetIntValue(exm));
			}
		}
	}

	private sealed class SORTCHARA_Instruction : AbstractInstruction
	{
		public SORTCHARA_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_SORTCHARA);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpSortcharaArgument spSortcharaArgument = (SpSortcharaArgument)func.Argument;
			long elem = 0L;
			VariableTerm sortKey = spSortcharaArgument.SortKey;
			if (sortKey.Identifier.IsArray1D)
			{
				elem = sortKey.GetElementInt(1, exm);
			}
			else if (sortKey.Identifier.IsArray2D)
			{
				elem = sortKey.GetElementInt(1, exm) << 32;
				elem += sortKey.GetElementInt(2, exm);
			}
			exm.VEvaluator.SortChara(sortKey.Identifier, elem, spSortcharaArgument.SortOrder, fixMaster: true);
		}
	}

	private sealed class RESETCOLOR_Instruction : AbstractInstruction
	{
		public RESETCOLOR_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.Console.SetStringStyle(Config.ForeColor);
		}
	}

	private sealed class RESETBGCOLOR_Instruction : AbstractInstruction
	{
		public RESETBGCOLOR_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.Console.SetBgColor(Config.BackColor);
		}
	}

	private sealed class FONTBOLD_Instruction : AbstractInstruction
	{
		public FONTBOLD_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
		}
	}

	private sealed class FONTITALIC_Instruction : AbstractInstruction
	{
		public FONTITALIC_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
		}
	}

	private sealed class FONTREGULAR_Instruction : AbstractInstruction
	{
		public FONTREGULAR_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
		}
	}

	private sealed class VARSET_Instruction : AbstractInstruction
	{
		public VARSET_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_VAR_SET);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpVarSetArgument spVarSetArgument = (SpVarSetArgument)func.Argument;
			VariableTerm variableDest = spVarSetArgument.VariableDest;
			FixedVariableTerm fixedVariableTerm = variableDest.GetFixedVariableTerm(exm);
			int num = 0;
			int num2 = 0;
			if (spVarSetArgument.End != null)
			{
				num2 = (int)spVarSetArgument.End.GetIntValue(exm);
			}
			else if (variableDest.Identifier.IsArray1D)
			{
				num2 = variableDest.GetLength();
			}
			if (spVarSetArgument.Start != null)
			{
				num = (int)spVarSetArgument.Start.GetIntValue(exm);
				if (num > num2)
				{
					num = num2;
					num2 = num;
				}
			}
			if (variableDest.IsString)
			{
				string strValue = spVarSetArgument.Term.GetStrValue(exm);
				exm.VEvaluator.SetValueAll(fixedVariableTerm, strValue, num, num2);
			}
			else
			{
				long intValue = spVarSetArgument.Term.GetIntValue(exm);
				exm.VEvaluator.SetValueAll(fixedVariableTerm, intValue, num, num2);
			}
		}
	}

	private sealed class CVARSET_Instruction : AbstractInstruction
	{
		public CVARSET_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_CVAR_SET);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpCVarSetArgument spCVarSetArgument = (SpCVarSetArgument)func.Argument;
			FixedVariableTerm fixedVariableTerm = spCVarSetArgument.VariableDest.GetFixedVariableTerm(exm);
			SingleTerm value = spCVarSetArgument.Index.GetValue(exm);
			int num = (int)exm.VEvaluator.CHARANUM;
			int num2 = 0;
			if (spCVarSetArgument.Start != null)
			{
				num2 = (int)spCVarSetArgument.Start.GetIntValue(exm);
				if (num2 < 0 || num2 >= num)
				{
					throw new CodeEE("命令CVARSETの第４引数(" + num2 + ")がキャラクタの範囲外です");
				}
			}
			int num3 = 0;
			if (spCVarSetArgument.End != null)
			{
				num3 = (int)spCVarSetArgument.End.GetIntValue(exm);
				if (num3 < 0 || num3 > num)
				{
					throw new CodeEE("命令CVARSETの第５引数(" + num3 + ")がキャラクタの範囲外です");
				}
			}
			else
			{
				num3 = num;
			}
			if (num2 > num3)
			{
				num2 = num3;
				num3 = num2;
			}
			if (!fixedVariableTerm.Identifier.IsCharacterData)
			{
				throw new CodeEE("命令CVARSETにキャラクタ変数でない変数" + fixedVariableTerm.Identifier.Name + "が渡されました");
			}
			if (value.GetOperandType() == typeof(string) && fixedVariableTerm.Identifier.IsArray1D && !GlobalStatic.ConstantData.isDefined(fixedVariableTerm.Identifier.Code, value.Str))
			{
				throw new CodeEE("文字列" + value.Str + "は配列変数" + fixedVariableTerm.Identifier.Name + "の要素ではありません");
			}
			if (fixedVariableTerm.Identifier.IsString)
			{
				string strValue = spCVarSetArgument.Term.GetStrValue(exm);
				exm.VEvaluator.SetValueAllEachChara(fixedVariableTerm, value, strValue, num2, num3);
			}
			else
			{
				long intValue = spCVarSetArgument.Term.GetIntValue(exm);
				exm.VEvaluator.SetValueAllEachChara(fixedVariableTerm, value, intValue, num2, num3);
			}
		}
	}

	private sealed class RANDOMIZE_Instruction : AbstractInstruction
	{
		public RANDOMIZE_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_EXPRESSION_NULLABLE);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			long seed = ((!func.Argument.IsConst) ? ((ExpressionArgument)func.Argument).Term.GetIntValue(exm) : func.Argument.ConstInt);
			exm.VEvaluator.Randomize(seed);
		}
	}

	private sealed class INITRAND_Instruction : AbstractInstruction
	{
		public INITRAND_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.VEvaluator.InitRanddata();
		}
	}

	private sealed class DUMPRAND_Instruction : AbstractInstruction
	{
		public DUMPRAND_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.VEvaluator.DumpRanddata();
		}
	}

	private sealed class SAVEGLOBAL_Instruction : AbstractInstruction
	{
		public SAVEGLOBAL_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.VEvaluator.SaveGlobal();
		}
	}

	private sealed class LOADGLOBAL_Instruction : AbstractInstruction
	{
		public LOADGLOBAL_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			if (exm.VEvaluator.LoadGlobal())
			{
				exm.VEvaluator.RESULT = 1L;
			}
			else
			{
				exm.VEvaluator.RESULT = 0L;
			}
		}
	}

	private sealed class RESETDATA_Instruction : AbstractInstruction
	{
		public RESETDATA_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.VEvaluator.ResetData();
			exm.Console.ResetStyle();
		}
	}

	private sealed class RESETGLOBAL_Instruction : AbstractInstruction
	{
		public RESETGLOBAL_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			exm.VEvaluator.ResetGlobalData();
		}
	}

	private sealed class SAVECHARA_Instruction : AbstractInstruction
	{
		public SAVECHARA_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_SAVECHARA);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			IOperandTerm[] termList = ((ExpressionArrayArgument)func.Argument).TermList;
			string strValue = termList[0].GetStrValue(exm);
			string strValue2 = termList[1].GetStrValue(exm);
			int[] array = new int[termList.Length - 2];
			int num = (int)exm.VEvaluator.CHARANUM;
			for (int i = 0; i < array.Length; i++)
			{
				long intValue = termList[i + 2].GetIntValue(exm);
				array[i] = toUInt32inArg(intValue, "SAVECHARA", i + 3);
				if (array[i] >= num)
				{
					throw new CodeEE("SAVECHARAの第" + (i + 3) + "引数の値がキャラ登録番号の範囲を超えています");
				}
				for (int j = 0; j < i; j++)
				{
					if (array[i] == array[j])
					{
						throw new CodeEE("同一のキャラ登録番号(" + array[i] + ")が複数回指定されました");
					}
				}
			}
			exm.VEvaluator.SaveChara(strValue, strValue2, array);
		}
	}

	private sealed class LOADCHARA_Instruction : AbstractInstruction
	{
		public LOADCHARA_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.STR_EXPRESSION);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			ExpressionArgument expressionArgument = (ExpressionArgument)func.Argument;
			string text = null;
			text = ((!expressionArgument.IsConst) ? expressionArgument.Term.GetStrValue(exm) : expressionArgument.ConstStr);
			exm.VEvaluator.LoadChara(text);
		}
	}

	private sealed class SAVEVAR_Instruction : AbstractInstruction
	{
		public SAVEVAR_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_SAVEVAR);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			throw new NotImplCodeEE();
		}
	}

	private sealed class LOADVAR_Instruction : AbstractInstruction
	{
		public LOADVAR_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.STR_EXPRESSION);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			throw new NotImplCodeEE();
		}
	}

	private sealed class DELDATA_Instruction : AbstractInstruction
	{
		public DELDATA_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_EXPRESSION);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			long value = ((!func.Argument.IsConst) ? ((ExpressionArgument)func.Argument).Term.GetIntValue(exm) : func.Argument.ConstInt);
			int dataIndex = toUInt32inArg(value, "DELDATA", 1);
			exm.VEvaluator.DelData(dataIndex);
		}
	}

	private sealed class DO_NOTHING_Instruction : AbstractInstruction
	{
		public DO_NOTHING_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 22;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
		}
	}

	private sealed class REF_Instruction : AbstractInstruction
	{
		private bool byname;

		public REF_Instruction(bool byname)
		{
			this.byname = byname;
			if (byname)
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_REFBYNAME);
			}
			else
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_REF);
			}
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			throw new NotImplCodeEE();
		}
	}

	private sealed class TOOLTIP_SETCOLOR_Instruction : AbstractInstruction
	{
		public TOOLTIP_SETCOLOR_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_SWAP);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpSwapCharaArgument obj = (SpSwapCharaArgument)func.Argument;
			long intValue = obj.X.GetIntValue(exm);
			long intValue2 = obj.Y.GetIntValue(exm);
			if (intValue < 0 || intValue > 16777215)
			{
				throw new CodeEE("第１引数が色を表す整数の範囲外です");
			}
			if (intValue2 < 0 || intValue2 > 16777215)
			{
				throw new CodeEE("第２引数が色を表す整数の範囲外です");
			}
			Color foreColor = Color.FromArgb((int)intValue >> 16, ((int)intValue >> 8) & 0xFF, (int)intValue & 0xFF);
			Color backColor = Color.FromArgb((int)intValue2 >> 16, ((int)intValue2 >> 8) & 0xFF, (int)intValue2 & 0xFF);
			exm.Console.SetToolTipColor(foreColor, backColor);
		}
	}

	private sealed class TOOLTIP_SETDELAY_Instruction : AbstractInstruction
	{
		public TOOLTIP_SETDELAY_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_EXPRESSION);
			flag = 6;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			ExpressionArgument expressionArgument = (ExpressionArgument)func.Argument;
			long num = 0L;
			num = ((!expressionArgument.IsConst) ? expressionArgument.Term.GetIntValue(exm) : expressionArgument.ConstInt);
			if (num < 0 || num > int.MaxValue)
			{
				throw new CodeEE("引数の値が適切な範囲外です");
			}
			exm.Console.SetToolTipDelay((int)num);
		}
	}

	private sealed class BEGIN_Instruction : AbstractInstruction
	{
		public BEGIN_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.STR);
			flag = 1;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			string text = func.Argument.ConstStr;
			if (Config.ICFunction)
			{
				text = text.ToUpper();
			}
			state.SetBegin(text);
			state.Return(0L);
			exm.Console.ResetStyle();
		}
	}

	private sealed class SAVELOADGAME_Instruction : AbstractInstruction
	{
		private readonly bool isSave;

		public SAVELOADGAME_Instruction(bool isSave)
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 1;
			this.isSave = isSave;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			if ((state.SystemState & SystemStateCode.__CAN_SAVE__) != SystemStateCode.__CAN_SAVE__)
			{
				string text = state.Scope;
				if (text == null)
				{
					text = "";
				}
				throw new CodeEE("@" + text + "中でSAVEGAME/LOADGAME命令を実行することはできません");
			}
			GlobalStatic.Process.saveCurrentState(single: true);
			GlobalStatic.Process.getCurrentState.SaveLoadData(isSave);
		}
	}

	private sealed class REPEAT_Instruction : AbstractInstruction
	{
		public REPEAT_Instruction(bool fornext)
		{
			flag = 21;
			if (fornext)
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_FOR_NEXT);
				flag |= 2;
			}
			else
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_EXPRESSION);
			}
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpForNextArgment spForNextArgment = (SpForNextArgment)func.Argument;
			func.LoopCounter = spForNextArgment.Cnt;
			func.LoopCounter.SetValue(spForNextArgment.Start.GetIntValue(exm), exm);
			func.LoopEnd = spForNextArgment.End.GetIntValue(exm);
			func.LoopStep = spForNextArgment.Step.GetIntValue(exm);
			if ((func.LoopStep <= 0 || func.LoopEnd <= func.LoopCounter.GetIntValue(exm)) && (func.LoopStep >= 0 || func.LoopEnd >= func.LoopCounter.GetIntValue(exm)))
			{
				state.JumpTo(func.JumpTo);
			}
		}
	}

	private sealed class WHILE_Instruction : AbstractInstruction
	{
		public WHILE_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_EXPRESSION);
			flag = 23;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			if (((ExpressionArgument)func.Argument).Term.GetIntValue(exm) == 0L)
			{
				state.JumpTo(func.JumpTo);
			}
		}
	}

	private sealed class SIF_Instruction : AbstractInstruction
	{
		public SIF_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_EXPRESSION);
			flag = 53;
		}

		public override void SetJumpTo(ref bool useCallForm, InstructionLine func, int currentDepth, ref string FunctionoNotFoundName)
		{
			LogicalLine nextLine = func.NextLine;
			if (nextLine == null || nextLine.NextLine == null || nextLine is FunctionLabelLine || nextLine is NullLine)
			{
				ParserMediator.Warn("SIF文の次の行がありません", func, 2, isError: true, isBackComp: false);
				return;
			}
			if (nextLine is InstructionLine)
			{
				InstructionLine instructionLine = (InstructionLine)nextLine;
				if (instructionLine.Function.IsPartial())
				{
					ParserMediator.Warn("SIF文の次の行を" + instructionLine.Function.Name + "文にすることはできません", func, 2, isError: true, isBackComp: false);
				}
				else
				{
					func.JumpTo = func.NextLine.NextLine;
				}
			}
			else if (nextLine is GotoLabelLine)
			{
				ParserMediator.Warn("SIF文の次の行をラベル行にすることはできません", func, 2, isError: true, isBackComp: false);
			}
			else
			{
				func.JumpTo = func.NextLine.NextLine;
			}
			if (func.JumpTo != null && func.Position.LineNo + 1 != func.NextLine.Position.LineNo)
			{
				ParserMediator.Warn("SIF文の次の行が空行またはコメント行です(eramaker:SIF文は意味を失います)", func, 0, isError: false, isBackComp: true);
			}
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			if (((ExpressionArgument)func.Argument).Term.GetIntValue(exm) == 0L)
			{
				state.ShiftNextLine();
			}
		}
	}

	private sealed class ELSEIF_Instruction : AbstractInstruction
	{
		public ELSEIF_Instruction(FunctionArgType argtype)
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(argtype);
			flag = 53;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			state.JumpTo(func.JumpTo);
		}
	}

	private sealed class ENDIF_Instruction : AbstractInstruction
	{
		public ENDIF_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 49;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
		}
	}

	private sealed class IF_Instruction : AbstractInstruction
	{
		public IF_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_EXPRESSION);
			flag = 53;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			LogicalLine logicalLine = func.JumpTo;
			InstructionLine instructionLine = null;
			for (int i = 0; i < func.IfCaseList.Count; i++)
			{
				instructionLine = func.IfCaseList[i];
				if (!instructionLine.IsError)
				{
					if (instructionLine.FunctionCode == FunctionCode.ELSE)
					{
						logicalLine = instructionLine;
						break;
					}
					state.CurrentLine = instructionLine;
					if (((ExpressionArgument)instructionLine.Argument).Term.GetIntValue(exm) != 0L)
					{
						logicalLine = instructionLine;
						break;
					}
				}
			}
			if (logicalLine != func)
			{
				state.JumpTo(logicalLine);
			}
		}
	}

	private sealed class SELECTCASE_Instruction : AbstractInstruction
	{
		public SELECTCASE_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.EXPRESSION);
			flag = 55;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			LogicalLine line = func.JumpTo;
			IOperandTerm term = ((ExpressionArgument)func.Argument).Term;
			string text = null;
			long num = 0L;
			if (term.IsInteger)
			{
				num = term.GetIntValue(exm);
			}
			else
			{
				text = term.GetStrValue(exm);
			}
			InstructionLine instructionLine = null;
			for (int i = 0; i < func.IfCaseList.Count; i++)
			{
				instructionLine = func.IfCaseList[i];
				if (instructionLine.IsError)
				{
					continue;
				}
				if (instructionLine.FunctionCode == FunctionCode.CASEELSE)
				{
					line = instructionLine;
					break;
				}
				CaseArgument caseArgument = (CaseArgument)instructionLine.Argument;
				state.CurrentLine = instructionLine;
				CaseExpression[] caseExps;
				int num3;
				if (term.IsInteger)
				{
					long num2 = num;
					caseExps = caseArgument.CaseExps;
					num3 = 0;
					while (num3 < caseExps.Length)
					{
						if (!caseExps[num3].GetBool(num2, exm))
						{
							num3++;
							continue;
						}
						goto IL_00b1;
					}
					continue;
				}
				string text2 = text;
				caseExps = caseArgument.CaseExps;
				num3 = 0;
				while (num3 < caseExps.Length)
				{
					if (!caseExps[num3].GetBool(text2, exm))
					{
						num3++;
						continue;
					}
					goto IL_00e6;
				}
				continue;
				IL_00e6:
				line = instructionLine;
				break;
				IL_00b1:
				line = instructionLine;
				break;
			}
			state.JumpTo(line);
		}
	}

	private sealed class RETURNFORM_Instruction : AbstractInstruction
	{
		public RETURNFORM_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.FORM_STR);
			flag = 3;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			StringStream stringStream = new StringStream(((ExpressionArgument)func.Argument).Term.GetStrValue(exm));
			List<long> list = new List<long>();
			while (!stringStream.EOS)
			{
				WordCollection wc = LexicalAnalyzer.Analyse(stringStream, LexEndWith.Comma, LexAnalyzeFlag.None);
				list.Add(ExpressionParser.ReduceIntegerTerm(wc, TermEndWith.EoL).GetIntValue(exm));
				stringStream.ShiftNext();
				LexicalAnalyzer.SkipHalfSpace(stringStream);
			}
			if (list.Count == 0)
			{
				list.Add(0L);
			}
			exm.VEvaluator.SetResultX(list);
			state.Return(exm.VEvaluator.RESULT);
			_ = state.ScriptEnd;
		}
	}

	private sealed class RETURN_Instruction : AbstractInstruction
	{
		public RETURN_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_ANY);
			flag = 1;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			ExpressionArrayArgument expressionArrayArgument = (ExpressionArrayArgument)func.Argument;
			if (expressionArrayArgument.TermList.Length == 0)
			{
				exm.VEvaluator.RESULT = 0L;
				state.Return(0L);
				return;
			}
			List<long> list = new List<long>();
			IOperandTerm[] termList = expressionArrayArgument.TermList;
			foreach (IOperandTerm operandTerm in termList)
			{
				list.Add(operandTerm.GetIntValue(exm));
			}
			if (list.Count == 0)
			{
				list.Add(0L);
			}
			exm.VEvaluator.SetResultX(list);
			state.Return(exm.VEvaluator.RESULT);
		}
	}

	private sealed class CATCH_Instruction : AbstractInstruction
	{
		public CATCH_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 23;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			state.JumpTo(func.JumpToEndCatch);
		}
	}

	private sealed class RESTART_Instruction : AbstractInstruction
	{
		public RESTART_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 7;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			state.JumpTo(func.ParentLabelLine);
		}
	}

	private sealed class BREAK_Instruction : AbstractInstruction
	{
		public BREAK_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 5;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			InstructionLine instructionLine = (InstructionLine)func.JumpTo;
			InstructionLine line = (InstructionLine)instructionLine.JumpTo;
			if (instructionLine.FunctionCode != FunctionCode.WHILE && instructionLine.FunctionCode != FunctionCode.DO)
			{
				instructionLine.LoopCounter.PlusValue(instructionLine.LoopStep, exm);
			}
			state.JumpTo(line);
		}
	}

	private sealed class CONTINUE_Instruction : AbstractInstruction
	{
		public CONTINUE_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 5;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			InstructionLine instructionLine = (InstructionLine)func.JumpTo;
			if (instructionLine.FunctionCode == FunctionCode.REPEAT || instructionLine.FunctionCode == FunctionCode.FOR)
			{
				if (instructionLine.LoopCounter == null)
				{
					state.JumpTo(instructionLine.JumpTo);
					return;
				}
				instructionLine.LoopCounter.PlusValue(instructionLine.LoopStep, exm);
				long intValue = instructionLine.LoopCounter.GetIntValue(exm);
				if ((instructionLine.LoopStep > 0 && instructionLine.LoopEnd > intValue) || (instructionLine.LoopStep < 0 && instructionLine.LoopEnd < intValue))
				{
					state.JumpTo(func.JumpTo);
				}
				else
				{
					state.JumpTo(instructionLine.JumpTo);
				}
				return;
			}
			if (instructionLine.FunctionCode == FunctionCode.WHILE)
			{
				if (((ExpressionArgument)instructionLine.Argument).Term.GetIntValue(exm) != 0L)
				{
					state.JumpTo(func.JumpTo);
				}
				else
				{
					state.JumpTo(instructionLine.JumpTo);
				}
				return;
			}
			if (instructionLine.FunctionCode == FunctionCode.DO)
			{
				InstructionLine instructionLine2 = (InstructionLine)((InstructionLine)func.JumpTo).JumpTo;
				if (instructionLine2.IsError)
				{
					throw new CodeEE(instructionLine2.ErrMes, instructionLine2.Position);
				}
				if (((ExpressionArgument)instructionLine2.Argument).Term.GetIntValue(exm) != 0L)
				{
					state.JumpTo(instructionLine);
				}
				else
				{
					state.JumpTo(instructionLine2);
				}
				return;
			}
			throw new ExeEE("異常なCONTINUE");
		}
	}

	private sealed class REND_Instruction : AbstractInstruction
	{
		public REND_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 21;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			InstructionLine instructionLine = (InstructionLine)func.JumpTo;
			if (instructionLine.LoopCounter == null)
			{
				state.JumpTo(instructionLine.JumpTo);
				return;
			}
			instructionLine.LoopCounter.PlusValue(instructionLine.LoopStep, exm);
			long intValue = instructionLine.LoopCounter.GetIntValue(exm);
			if ((instructionLine.LoopStep > 0 && instructionLine.LoopEnd > intValue) || (instructionLine.LoopStep < 0 && instructionLine.LoopEnd < intValue))
			{
				state.JumpTo(func.JumpTo);
			}
		}
	}

	private sealed class WEND_Instruction : AbstractInstruction
	{
		public WEND_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.VOID);
			flag = 23;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			if (((ExpressionArgument)((InstructionLine)func.JumpTo).Argument).Term.GetIntValue(exm) != 0L)
			{
				state.JumpTo(func.JumpTo);
			}
		}
	}

	private sealed class LOOP_Instruction : AbstractInstruction
	{
		public LOOP_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.INT_EXPRESSION);
			flag = 55;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			if (((ExpressionArgument)func.Argument).Term.GetIntValue(exm) != 0L)
			{
				state.JumpTo(func.JumpTo);
			}
		}
	}

	private sealed class RETURNF_Instruction : AbstractInstruction
	{
		public RETURNF_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.EXPRESSION_NULLABLE);
			flag = 7;
		}

		public override void SetJumpTo(ref bool useCallForm, InstructionLine func, int currentDepth, ref string FunctionoNotFoundName)
		{
			FunctionLabelLine parentLabelLine = func.ParentLabelLine;
			if (!parentLabelLine.IsMethod)
			{
				ParserMediator.Warn("RETURNFは#FUNCTION以外では使用できません", func, 2, isError: true, isBackComp: false);
			}
			if (func.Argument == null)
			{
				return;
			}
			IOperandTerm term = ((ExpressionArgument)func.Argument).Term;
			if (term != null && parentLabelLine.MethodType != term.GetOperandType())
			{
				if (parentLabelLine.MethodType == typeof(long))
				{
					ParserMediator.Warn("#FUNCTIONで始まる関数の戻り値に文字列型が指定されました", func, 2, isError: true, isBackComp: false);
				}
				else if (parentLabelLine.MethodType == typeof(string))
				{
					ParserMediator.Warn("#FUCNTIONSで始まる関数の戻り値に数値型が指定されました", func, 2, isError: true, isBackComp: false);
				}
			}
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			IOperandTerm term = ((ExpressionArgument)func.Argument).Term;
			SingleTerm ret = null;
			if (term != null)
			{
				ret = term.GetValue(exm);
			}
			state.ReturnF(ret);
		}
	}

	private sealed class CALL_Instruction : AbstractInstruction
	{
		private readonly bool isJump;

		private readonly bool isTry;

		private string originalName;

		public CALL_Instruction(bool form, bool isJump, bool isTry, bool isTryCatch)
		{
			if (form)
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_CALLFORM);
			}
			else
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_CALL);
			}
			flag = 33;
			if (isJump)
			{
				flag |= 64;
			}
			if (isTry)
			{
				flag |= 128;
			}
			if (isTryCatch)
			{
				flag |= 32784;
			}
			this.isJump = isJump;
			this.isTry = isTry;
		}

		public override void SetJumpTo(ref bool useCallForm, InstructionLine func, int currentDepth, ref string FunctionoNotFoundName)
		{
			if (!func.Argument.IsConst)
			{
				useCallForm = true;
				return;
			}
			SpCallArgment spCallArgment = (SpCallArgment)func.Argument;
			string text = (originalName = spCallArgment.ConstStr);
			if (Config.ICFunction)
			{
				text = text.ToUpper();
			}
			CalledFunction calledFunction = CalledFunction.CallFunction(GlobalStatic.Process, text, func);
			if (calledFunction == null && !func.Function.IsTry())
			{
				FunctionoNotFoundName = text;
				return;
			}
			if (calledFunction != null)
			{
				func.JumpTo = calledFunction.TopLabel;
				if (calledFunction.TopLabel.Depth < 0)
				{
					calledFunction.TopLabel.Depth = currentDepth + 1;
				}
				if (calledFunction.TopLabel.IsError)
				{
					func.IsError = true;
					func.ErrMes = calledFunction.TopLabel.ErrMes;
					return;
				}
				spCallArgment.UDFArgument = calledFunction.ConvertArg(spCallArgment.RowArgs, out var errMes);
				if (spCallArgment.UDFArgument == null)
				{
					ParserMediator.Warn(errMes, func, 2, isError: true, isBackComp: false);
					return;
				}
			}
			spCallArgment.CallFunc = calledFunction;
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			SpCallArgment spCallArgment = (SpCallArgment)func.Argument;
			CalledFunction calledFunction = null;
			string text = null;
			UserDefinedFunctionArgument userDefinedFunctionArgument = null;
			if (spCallArgment.IsConst)
			{
				calledFunction = spCallArgment.CallFunc;
				text = (originalName = spCallArgment.ConstStr);
				userDefinedFunctionArgument = spCallArgment.UDFArgument;
			}
			else
			{
				text = (originalName = spCallArgment.FuncnameTerm.GetStrValue(exm));
				if (Config.ICFunction)
				{
					text = text.ToUpper();
				}
				calledFunction = CalledFunction.CallFunction(GlobalStatic.Process, text, func);
			}
			if (calledFunction == null)
			{
				if (!isTry)
				{
					throw new CodeEE("関数\"@" + text + "\"が見つかりません");
				}
				if (func.JumpToEndCatch != null)
				{
					state.JumpTo(func.JumpToEndCatch);
				}
				return;
			}
			calledFunction.IsJump = isJump;
			if (userDefinedFunctionArgument == null)
			{
				userDefinedFunctionArgument = calledFunction.ConvertArg(spCallArgment.RowArgs, out var errMes);
				if (userDefinedFunctionArgument == null)
				{
					throw new CodeEE(errMes);
				}
			}
			state.IntoFunction(calledFunction, userDefinedFunctionArgument, exm);
		}
	}

	private sealed class CALLEVENT_Instruction : AbstractInstruction
	{
		public CALLEVENT_Instruction()
		{
			base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.STR);
			flag = 3;
		}

		public override void SetJumpTo(ref bool useCallForm, InstructionLine func, int currentDepth, ref string FunctionoNotFoundName)
		{
			if (func.ParentLabelLine.IsEvent)
			{
				ParserMediator.Warn("EVENT関数中にCALLEVENT命令は使用できません", func, 2, isError: true, isBackComp: false);
			}
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			string text = func.Argument.ConstStr;
			if (Config.ICFunction)
			{
				text = text.ToUpper();
			}
			CalledFunction calledFunction = CalledFunction.CallEventFunction(GlobalStatic.Process, text, func);
			if (calledFunction != null)
			{
				state.IntoFunction(calledFunction, null, null);
			}
		}
	}

	private sealed class GOTO_Instruction : AbstractInstruction
	{
		private readonly bool isTry;

		public GOTO_Instruction(bool form, bool isTry, bool isTryCatch)
		{
			if (form)
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_CALLFORM);
			}
			else
			{
				base.ArgBuilder = ArgumentParser.GetArgumentBuilder(FunctionArgType.SP_CALL);
			}
			this.isTry = isTry;
			flag = 37;
			if (isTry)
			{
				flag |= 128;
			}
			if (isTryCatch)
			{
				flag |= 32784;
			}
		}

		public override void SetJumpTo(ref bool useCallForm, InstructionLine func, int currentDepth, ref string FunctionoNotFoundName)
		{
			GotoLabelLine gotoLabelLine = null;
			func.JumpTo = null;
			if (!func.Argument.IsConst)
			{
				return;
			}
			string text = func.Argument.ConstStr;
			if (Config.ICVariable)
			{
				text = text.ToUpper();
			}
			gotoLabelLine = GlobalStatic.LabelDictionary.GetLabelDollar(text, func.ParentLabelLine);
			if (gotoLabelLine == null)
			{
				if (!func.Function.IsTry())
				{
					ParserMediator.Warn("指定されたラベル名\"$" + text + "\"は現在の関数内に存在しません", func, 2, isError: true, isBackComp: false);
				}
			}
			else if (gotoLabelLine.IsError)
			{
				ParserMediator.Warn("指定されたラベル名\"$" + text + "\"は無効な$ラベル行です", func, 2, isError: true, isBackComp: false);
			}
			else if (gotoLabelLine != null)
			{
				func.JumpTo = gotoLabelLine;
			}
		}

		public override void DoInstruction(ExpressionMediator exm, InstructionLine func, ProcessState state)
		{
			string text = null;
			LogicalLine logicalLine = null;
			if (func.Argument.IsConst)
			{
				text = func.Argument.ConstStr;
				if (func.JumpTo == null)
				{
					return;
				}
				logicalLine = func.JumpTo;
			}
			else
			{
				text = ((SpCallArgment)func.Argument).FuncnameTerm.GetStrValue(exm);
				if (Config.ICVariable)
				{
					text = text.ToUpper();
				}
				logicalLine = state.CurrentCalled.CallLabel(GlobalStatic.Process, text);
			}
			if (logicalLine == null)
			{
				if (!func.Function.IsTry())
				{
					throw new CodeEE("指定されたラベル名\"$" + text + "\"は現在の関数内に存在しません");
				}
				if (func.JumpToEndCatch != null)
				{
					state.JumpTo(func.JumpToEndCatch);
				}
			}
			else
			{
				if (logicalLine.IsError)
				{
					throw new CodeEE("指定されたラベル名\"$" + text + "\"は無効な$ラベル行です");
				}
				state.JumpTo(logicalLine);
			}
		}
	}

	public const int FLOW_CONTROL = 1;

	public const int EXTENDED = 2;

	public const int METHOD_SAFE = 4;

	public const int DEBUG_FUNC = 8;

	public const int PARTIAL = 16;

	public const int FORCE_SETARG = 32;

	public const int IS_JUMP = 64;

	public const int IS_TRY = 128;

	public const int IS_TRYC = 32768;

	public const int PRINT_NEWLINE = 256;

	public const int PRINT_WAITINPUT = 512;

	public const int PRINT_SINGLE = 1024;

	public const int ISPRINTDFUNC = 2048;

	public const int ISPRINTKFUNC = 4096;

	public const int IS_PRINT = 8192;

	public const int IS_INPUT = 16384;

	public const int IS_PRINTDATA = 65536;

	private static readonly Dictionary<string, FunctionIdentifier> funcDic;

	private static readonly Dictionary<FunctionCode, string> funcMatch;

	private static readonly Dictionary<FunctionCode, FunctionCode> funcParent;

	private static readonly ArgumentBuilder methodArgumentBuilder;

	private static readonly AbstractInstruction methodInstruction;

	private static FunctionIdentifier setFunc;

	public readonly AbstractInstruction Instruction;

	private FunctionCode code;

	private ArgumentBuilder arg;

	private int flag;

	private FunctionMethod method;

	public static FunctionIdentifier SETFunction => setFunc;

	public FunctionCode Code => code;

	public ArgumentBuilder ArgBuilder => arg;

	public FunctionMethod Method => method;

	public string Name { get; private set; }

	private static void addFunction(FunctionCode code, AbstractInstruction inst)
	{
		addFunction(code, inst, 0);
	}

	private static void addFunction(FunctionCode code, AbstractInstruction inst, int additionalFlag)
	{
		string text = code.ToString();
		if (Config.ICFunction)
		{
			text = text.ToUpper();
		}
		funcDic.Add(text, new FunctionIdentifier(text, code, inst, additionalFlag));
	}

	private static void addFunction(FunctionCode code, ArgumentBuilder arg)
	{
		addFunction(code, arg, 0);
	}

	private static void addFunction(FunctionCode code, ArgumentBuilder arg, int flag)
	{
		string text = code.ToString();
		if (Config.ICFunction)
		{
			text = text.ToUpper();
		}
		funcDic.Add(text, new FunctionIdentifier(text, code, arg, flag));
	}

	public static Dictionary<string, FunctionIdentifier> GetInstructionNameDic()
	{
		return funcDic;
	}

	private static void addPrintFunction(FunctionCode code)
	{
		addFunction(code, new PRINT_Instruction(code.ToString()));
	}

	private static void addPrintDataFunction(FunctionCode code)
	{
		addFunction(code, new PRINT_DATA_Instruction(code.ToString()));
	}

	static FunctionIdentifier()
	{
		funcDic = new Dictionary<string, FunctionIdentifier>();
		funcMatch = new Dictionary<FunctionCode, string>();
		funcParent = new Dictionary<FunctionCode, FunctionCode>();
		methodArgumentBuilder = null;
		methodInstruction = null;
		Dictionary<FunctionArgType, ArgumentBuilder> argumentBuilderDictionary = ArgumentParser.GetArgumentBuilderDictionary();
		methodArgumentBuilder = argumentBuilderDictionary[FunctionArgType.METHOD];
		methodInstruction = new METHOD_Instruction();
		setFunc = new FunctionIdentifier("SET", FunctionCode.SET, new SET_Instruction());
		addPrintFunction(FunctionCode.PRINT);
		addPrintFunction(FunctionCode.PRINTL);
		addPrintFunction(FunctionCode.PRINTW);
		addPrintFunction(FunctionCode.PRINTV);
		addPrintFunction(FunctionCode.PRINTVL);
		addPrintFunction(FunctionCode.PRINTVW);
		addPrintFunction(FunctionCode.PRINTS);
		addPrintFunction(FunctionCode.PRINTSL);
		addPrintFunction(FunctionCode.PRINTSW);
		addPrintFunction(FunctionCode.PRINTFORM);
		addPrintFunction(FunctionCode.PRINTFORML);
		addPrintFunction(FunctionCode.PRINTFORMW);
		addPrintFunction(FunctionCode.PRINTFORMS);
		addPrintFunction(FunctionCode.PRINTFORMSL);
		addPrintFunction(FunctionCode.PRINTFORMSW);
		addPrintFunction(FunctionCode.PRINTK);
		addPrintFunction(FunctionCode.PRINTKL);
		addPrintFunction(FunctionCode.PRINTKW);
		addPrintFunction(FunctionCode.PRINTVK);
		addPrintFunction(FunctionCode.PRINTVKL);
		addPrintFunction(FunctionCode.PRINTVKW);
		addPrintFunction(FunctionCode.PRINTSK);
		addPrintFunction(FunctionCode.PRINTSKL);
		addPrintFunction(FunctionCode.PRINTSKW);
		addPrintFunction(FunctionCode.PRINTFORMK);
		addPrintFunction(FunctionCode.PRINTFORMKL);
		addPrintFunction(FunctionCode.PRINTFORMKW);
		addPrintFunction(FunctionCode.PRINTFORMSK);
		addPrintFunction(FunctionCode.PRINTFORMSKL);
		addPrintFunction(FunctionCode.PRINTFORMSKW);
		addPrintFunction(FunctionCode.PRINTD);
		addPrintFunction(FunctionCode.PRINTDL);
		addPrintFunction(FunctionCode.PRINTDW);
		addPrintFunction(FunctionCode.PRINTVD);
		addPrintFunction(FunctionCode.PRINTVDL);
		addPrintFunction(FunctionCode.PRINTVDW);
		addPrintFunction(FunctionCode.PRINTSD);
		addPrintFunction(FunctionCode.PRINTSDL);
		addPrintFunction(FunctionCode.PRINTSDW);
		addPrintFunction(FunctionCode.PRINTFORMD);
		addPrintFunction(FunctionCode.PRINTFORMDL);
		addPrintFunction(FunctionCode.PRINTFORMDW);
		addPrintFunction(FunctionCode.PRINTFORMSD);
		addPrintFunction(FunctionCode.PRINTFORMSDL);
		addPrintFunction(FunctionCode.PRINTFORMSDW);
		addPrintFunction(FunctionCode.PRINTSINGLE);
		addPrintFunction(FunctionCode.PRINTSINGLEV);
		addPrintFunction(FunctionCode.PRINTSINGLES);
		addPrintFunction(FunctionCode.PRINTSINGLEFORM);
		addPrintFunction(FunctionCode.PRINTSINGLEFORMS);
		addPrintFunction(FunctionCode.PRINTSINGLEK);
		addPrintFunction(FunctionCode.PRINTSINGLEVK);
		addPrintFunction(FunctionCode.PRINTSINGLESK);
		addPrintFunction(FunctionCode.PRINTSINGLEFORMK);
		addPrintFunction(FunctionCode.PRINTSINGLEFORMSK);
		addPrintFunction(FunctionCode.PRINTSINGLED);
		addPrintFunction(FunctionCode.PRINTSINGLEVD);
		addPrintFunction(FunctionCode.PRINTSINGLESD);
		addPrintFunction(FunctionCode.PRINTSINGLEFORMD);
		addPrintFunction(FunctionCode.PRINTSINGLEFORMSD);
		addPrintFunction(FunctionCode.PRINTC);
		addPrintFunction(FunctionCode.PRINTLC);
		addPrintFunction(FunctionCode.PRINTFORMC);
		addPrintFunction(FunctionCode.PRINTFORMLC);
		addPrintFunction(FunctionCode.PRINTCK);
		addPrintFunction(FunctionCode.PRINTLCK);
		addPrintFunction(FunctionCode.PRINTFORMCK);
		addPrintFunction(FunctionCode.PRINTFORMLCK);
		addPrintFunction(FunctionCode.PRINTCD);
		addPrintFunction(FunctionCode.PRINTLCD);
		addPrintFunction(FunctionCode.PRINTFORMCD);
		addPrintFunction(FunctionCode.PRINTFORMLCD);
		addPrintDataFunction(FunctionCode.PRINTDATA);
		addPrintDataFunction(FunctionCode.PRINTDATAL);
		addPrintDataFunction(FunctionCode.PRINTDATAW);
		addPrintDataFunction(FunctionCode.PRINTDATAK);
		addPrintDataFunction(FunctionCode.PRINTDATAKL);
		addPrintDataFunction(FunctionCode.PRINTDATAKW);
		addPrintDataFunction(FunctionCode.PRINTDATAD);
		addPrintDataFunction(FunctionCode.PRINTDATADL);
		addPrintDataFunction(FunctionCode.PRINTDATADW);
		addFunction(FunctionCode.PRINTBUTTON, argumentBuilderDictionary[FunctionArgType.SP_BUTTON], 6);
		addFunction(FunctionCode.PRINTBUTTONC, argumentBuilderDictionary[FunctionArgType.SP_BUTTON], 6);
		addFunction(FunctionCode.PRINTBUTTONLC, argumentBuilderDictionary[FunctionArgType.SP_BUTTON], 6);
		addFunction(FunctionCode.PRINTPLAIN, argumentBuilderDictionary[FunctionArgType.STR_NULLABLE], 6);
		addFunction(FunctionCode.PRINTPLAINFORM, argumentBuilderDictionary[FunctionArgType.FORM_STR_NULLABLE], 6);
		addFunction(FunctionCode.PRINT_ABL, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 4);
		addFunction(FunctionCode.PRINT_TALENT, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 4);
		addFunction(FunctionCode.PRINT_MARK, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 4);
		addFunction(FunctionCode.PRINT_EXP, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 4);
		addFunction(FunctionCode.PRINT_PALAM, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 4);
		addFunction(FunctionCode.PRINT_ITEM, argumentBuilderDictionary[FunctionArgType.VOID], 4);
		addFunction(FunctionCode.PRINT_SHOPITEM, argumentBuilderDictionary[FunctionArgType.VOID], 4);
		addFunction(FunctionCode.DRAWLINE, argumentBuilderDictionary[FunctionArgType.VOID], 4);
		addFunction(FunctionCode.BAR, new BAR_Instruction(newline: false));
		addFunction(FunctionCode.BARL, new BAR_Instruction(newline: true));
		addFunction(FunctionCode.TIMES, new TIMES_Instruction());
		addFunction(FunctionCode.WAIT, new WAIT_Instruction(force: false));
		addFunction(FunctionCode.INPUT, new INPUT_Instruction());
		addFunction(FunctionCode.INPUTS, new INPUTS_Instruction());
		addFunction(FunctionCode.TINPUT, new TINPUT_Instruction(oneInput: false));
		addFunction(FunctionCode.TINPUTS, new TINPUTS_Instruction(oneInput: false));
		addFunction(FunctionCode.TONEINPUT, new TINPUT_Instruction(oneInput: true));
		addFunction(FunctionCode.TONEINPUTS, new TINPUTS_Instruction(oneInput: true));
		addFunction(FunctionCode.TWAIT, new TWAIT_Instruction());
		addFunction(FunctionCode.WAITANYKEY, new WAITANYKEY_Instruction());
		addFunction(FunctionCode.FORCEWAIT, new WAIT_Instruction(force: true));
		addFunction(FunctionCode.ONEINPUT, new ONEINPUT_Instruction());
		addFunction(FunctionCode.ONEINPUTS, new ONEINPUTS_Instruction());
		addFunction(FunctionCode.CLEARLINE, new CLEARLINE_Instruction());
		addFunction(FunctionCode.REUSELASTLINE, new REUSELASTLINE_Instruction());
		addFunction(FunctionCode.UPCHECK, argumentBuilderDictionary[FunctionArgType.VOID], 4);
		addFunction(FunctionCode.CUPCHECK, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 6);
		addFunction(FunctionCode.ADDCHARA, new ADDCHARA_Instruction(flagSp: false, flagDel: false));
		addFunction(FunctionCode.ADDSPCHARA, new ADDCHARA_Instruction(flagSp: true, flagDel: false));
		addFunction(FunctionCode.ADDDEFCHARA, argumentBuilderDictionary[FunctionArgType.VOID], 6);
		addFunction(FunctionCode.ADDVOIDCHARA, new ADDVOIDCHARA_Instruction());
		addFunction(FunctionCode.DELCHARA, new ADDCHARA_Instruction(flagSp: false, flagDel: true));
		addFunction(FunctionCode.PUTFORM, argumentBuilderDictionary[FunctionArgType.FORM_STR_NULLABLE], 4);
		addFunction(FunctionCode.QUIT, argumentBuilderDictionary[FunctionArgType.VOID]);
		addFunction(FunctionCode.OUTPUTLOG, argumentBuilderDictionary[FunctionArgType.VOID]);
		addFunction(FunctionCode.BEGIN, new BEGIN_Instruction());
		addFunction(FunctionCode.SAVEGAME, new SAVELOADGAME_Instruction(isSave: true));
		addFunction(FunctionCode.LOADGAME, new SAVELOADGAME_Instruction(isSave: false));
		addFunction(FunctionCode.SAVEDATA, argumentBuilderDictionary[FunctionArgType.SP_SAVEDATA], 6);
		addFunction(FunctionCode.LOADDATA, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 3);
		addFunction(FunctionCode.DELDATA, new DELDATA_Instruction());
		addFunction(FunctionCode.SAVEGLOBAL, new SAVEGLOBAL_Instruction());
		addFunction(FunctionCode.LOADGLOBAL, new LOADGLOBAL_Instruction());
		addFunction(FunctionCode.RESETDATA, new RESETDATA_Instruction());
		addFunction(FunctionCode.RESETGLOBAL, new RESETGLOBAL_Instruction());
		addFunction(FunctionCode.SIF, new SIF_Instruction());
		addFunction(FunctionCode.IF, new IF_Instruction());
		addFunction(FunctionCode.ELSE, new ELSEIF_Instruction(FunctionArgType.VOID));
		addFunction(FunctionCode.ELSEIF, new ELSEIF_Instruction(FunctionArgType.INT_EXPRESSION));
		addFunction(FunctionCode.ENDIF, new ENDIF_Instruction(), 4);
		addFunction(FunctionCode.SELECTCASE, new SELECTCASE_Instruction());
		addFunction(FunctionCode.CASE, new ELSEIF_Instruction(FunctionArgType.CASE), 2);
		addFunction(FunctionCode.CASEELSE, new ELSEIF_Instruction(FunctionArgType.VOID), 2);
		addFunction(FunctionCode.ENDSELECT, new ENDIF_Instruction(), 6);
		addFunction(FunctionCode.REPEAT, new REPEAT_Instruction(fornext: false));
		addFunction(FunctionCode.REND, new REND_Instruction());
		addFunction(FunctionCode.FOR, new REPEAT_Instruction(fornext: true), 2);
		addFunction(FunctionCode.NEXT, new REND_Instruction(), 2);
		addFunction(FunctionCode.WHILE, new WHILE_Instruction());
		addFunction(FunctionCode.WEND, new WEND_Instruction());
		addFunction(FunctionCode.DO, new ENDIF_Instruction(), 6);
		addFunction(FunctionCode.LOOP, new LOOP_Instruction());
		addFunction(FunctionCode.CONTINUE, new CONTINUE_Instruction());
		addFunction(FunctionCode.BREAK, new BREAK_Instruction());
		addFunction(FunctionCode.RETURN, new RETURN_Instruction());
		addFunction(FunctionCode.RETURNFORM, new RETURNFORM_Instruction());
		addFunction(FunctionCode.RETURNF, new RETURNF_Instruction());
		addFunction(FunctionCode.STRLEN, new STRLEN_Instruction(argisform: false, unicode: false));
		addFunction(FunctionCode.STRLENFORM, new STRLEN_Instruction(argisform: true, unicode: false));
		addFunction(FunctionCode.STRLENU, new STRLEN_Instruction(argisform: false, unicode: true));
		addFunction(FunctionCode.STRLENFORMU, new STRLEN_Instruction(argisform: true, unicode: true));
		addFunction(FunctionCode.SWAPCHARA, new SWAPCHARA_Instruction());
		addFunction(FunctionCode.COPYCHARA, new COPYCHARA_Instruction());
		addFunction(FunctionCode.ADDCOPYCHARA, new ADDCOPYCHARA_Instruction());
		addFunction(FunctionCode.SPLIT, argumentBuilderDictionary[FunctionArgType.SP_SPLIT], 6);
		addFunction(FunctionCode.SETCOLOR, argumentBuilderDictionary[FunctionArgType.SP_COLOR], 6);
		addFunction(FunctionCode.SETCOLORBYNAME, argumentBuilderDictionary[FunctionArgType.STR], 6);
		addFunction(FunctionCode.RESETCOLOR, new RESETCOLOR_Instruction());
		addFunction(FunctionCode.SETBGCOLOR, argumentBuilderDictionary[FunctionArgType.SP_COLOR], 6);
		addFunction(FunctionCode.SETBGCOLORBYNAME, argumentBuilderDictionary[FunctionArgType.STR], 6);
		addFunction(FunctionCode.RESETBGCOLOR, new RESETBGCOLOR_Instruction());
		addFunction(FunctionCode.FONTBOLD, new FONTBOLD_Instruction());
		addFunction(FunctionCode.FONTITALIC, new FONTITALIC_Instruction());
		addFunction(FunctionCode.FONTREGULAR, new FONTREGULAR_Instruction());
		addFunction(FunctionCode.SORTCHARA, new SORTCHARA_Instruction());
		addFunction(FunctionCode.FONTSTYLE, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION_NULLABLE], 6);
		addFunction(FunctionCode.ALIGNMENT, argumentBuilderDictionary[FunctionArgType.STR], 6);
		addFunction(FunctionCode.CUSTOMDRAWLINE, argumentBuilderDictionary[FunctionArgType.STR], 6);
		addFunction(FunctionCode.DRAWLINEFORM, argumentBuilderDictionary[FunctionArgType.FORM_STR], 6);
		addFunction(FunctionCode.CLEARTEXTBOX, argumentBuilderDictionary[FunctionArgType.VOID], 6);
		addFunction(FunctionCode.SETFONT, argumentBuilderDictionary[FunctionArgType.STR_EXPRESSION_NULLABLE], 6);
		addFunction(FunctionCode.SWAP, argumentBuilderDictionary[FunctionArgType.SP_SWAPVAR], 6);
		addFunction(FunctionCode.RANDOMIZE, new RANDOMIZE_Instruction());
		addFunction(FunctionCode.DUMPRAND, new DUMPRAND_Instruction());
		addFunction(FunctionCode.INITRAND, new INITRAND_Instruction());
		addFunction(FunctionCode.REDRAW, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 6);
		addFunction(FunctionCode.CALLTRAIN, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 3);
		addFunction(FunctionCode.STOPCALLTRAIN, argumentBuilderDictionary[FunctionArgType.VOID], 3);
		addFunction(FunctionCode.DOTRAIN, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 3);
		addFunction(FunctionCode.DATA, argumentBuilderDictionary[FunctionArgType.STR_NULLABLE], 22);
		addFunction(FunctionCode.DATAFORM, argumentBuilderDictionary[FunctionArgType.FORM_STR_NULLABLE], 22);
		addFunction(FunctionCode.ENDDATA, new DO_NOTHING_Instruction());
		addFunction(FunctionCode.DATALIST, argumentBuilderDictionary[FunctionArgType.VOID], 22);
		addFunction(FunctionCode.ENDLIST, argumentBuilderDictionary[FunctionArgType.VOID], 22);
		addFunction(FunctionCode.STRDATA, argumentBuilderDictionary[FunctionArgType.VAR_STR], 22);
		addFunction(FunctionCode.SETBIT, new SETBIT_Instruction(1));
		addFunction(FunctionCode.CLEARBIT, new SETBIT_Instruction(0));
		addFunction(FunctionCode.INVERTBIT, new SETBIT_Instruction(-1));
		addFunction(FunctionCode.DELALLCHARA, argumentBuilderDictionary[FunctionArgType.VOID], 6);
		addFunction(FunctionCode.PICKUPCHARA, argumentBuilderDictionary[FunctionArgType.INT_ANY], 6);
		addFunction(FunctionCode.VARSET, new VARSET_Instruction());
		addFunction(FunctionCode.CVARSET, new CVARSET_Instruction());
		addFunction(FunctionCode.RESET_STAIN, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 6);
		addFunction(FunctionCode.FORCEKANA, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 6);
		addFunction(FunctionCode.SKIPDISP, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 6);
		addFunction(FunctionCode.NOSKIP, argumentBuilderDictionary[FunctionArgType.VOID], 22);
		addFunction(FunctionCode.ENDNOSKIP, argumentBuilderDictionary[FunctionArgType.VOID], 22);
		addFunction(FunctionCode.ARRAYSHIFT, argumentBuilderDictionary[FunctionArgType.SP_SHIFT_ARRAY], 6);
		addFunction(FunctionCode.ARRAYREMOVE, argumentBuilderDictionary[FunctionArgType.SP_CONTROL_ARRAY], 6);
		addFunction(FunctionCode.ARRAYSORT, argumentBuilderDictionary[FunctionArgType.SP_SORTARRAY], 6);
		addFunction(FunctionCode.ARRAYCOPY, argumentBuilderDictionary[FunctionArgType.SP_COPY_ARRAY], 6);
		addFunction(FunctionCode.JUMP, new CALL_Instruction(form: false, isJump: true, isTry: false, isTryCatch: false));
		addFunction(FunctionCode.CALL, new CALL_Instruction(form: false, isJump: false, isTry: false, isTryCatch: false));
		addFunction(FunctionCode.TRYJUMP, new CALL_Instruction(form: false, isJump: true, isTry: true, isTryCatch: false), 2);
		addFunction(FunctionCode.TRYCALL, new CALL_Instruction(form: false, isJump: false, isTry: true, isTryCatch: false), 2);
		addFunction(FunctionCode.JUMPFORM, new CALL_Instruction(form: true, isJump: true, isTry: false, isTryCatch: false), 2);
		addFunction(FunctionCode.CALLFORM, new CALL_Instruction(form: true, isJump: false, isTry: false, isTryCatch: false), 2);
		addFunction(FunctionCode.TRYJUMPFORM, new CALL_Instruction(form: true, isJump: true, isTry: true, isTryCatch: false), 2);
		addFunction(FunctionCode.TRYCALLFORM, new CALL_Instruction(form: true, isJump: false, isTry: true, isTryCatch: false), 2);
		addFunction(FunctionCode.TRYCJUMP, new CALL_Instruction(form: false, isJump: true, isTry: true, isTryCatch: true), 2);
		addFunction(FunctionCode.TRYCCALL, new CALL_Instruction(form: false, isJump: false, isTry: true, isTryCatch: true), 2);
		addFunction(FunctionCode.TRYCJUMPFORM, new CALL_Instruction(form: true, isJump: true, isTry: true, isTryCatch: true), 2);
		addFunction(FunctionCode.TRYCCALLFORM, new CALL_Instruction(form: true, isJump: false, isTry: true, isTryCatch: true), 2);
		addFunction(FunctionCode.CALLEVENT, new CALLEVENT_Instruction());
		addFunction(FunctionCode.CALLF, new CALLF_Instruction(form: false));
		addFunction(FunctionCode.CALLFORMF, new CALLF_Instruction(form: true));
		addFunction(FunctionCode.RESTART, new RESTART_Instruction());
		addFunction(FunctionCode.GOTO, new GOTO_Instruction(form: false, isTry: false, isTryCatch: false));
		addFunction(FunctionCode.TRYGOTO, new GOTO_Instruction(form: false, isTry: true, isTryCatch: false), 2);
		addFunction(FunctionCode.GOTOFORM, new GOTO_Instruction(form: true, isTry: false, isTryCatch: false), 2);
		addFunction(FunctionCode.TRYGOTOFORM, new GOTO_Instruction(form: true, isTry: true, isTryCatch: false), 2);
		addFunction(FunctionCode.TRYCGOTO, new GOTO_Instruction(form: false, isTry: true, isTryCatch: true), 2);
		addFunction(FunctionCode.TRYCGOTOFORM, new GOTO_Instruction(form: true, isTry: true, isTryCatch: true), 2);
		addFunction(FunctionCode.CATCH, new CATCH_Instruction());
		addFunction(FunctionCode.ENDCATCH, new ENDIF_Instruction(), 6);
		addFunction(FunctionCode.TRYCALLLIST, argumentBuilderDictionary[FunctionArgType.VOID], 147);
		addFunction(FunctionCode.TRYJUMPLIST, argumentBuilderDictionary[FunctionArgType.VOID], 211);
		addFunction(FunctionCode.TRYGOTOLIST, argumentBuilderDictionary[FunctionArgType.VOID], 147);
		addFunction(FunctionCode.FUNC, argumentBuilderDictionary[FunctionArgType.SP_CALLFORM], 51);
		addFunction(FunctionCode.ENDFUNC, new ENDIF_Instruction(), 2);
		addFunction(FunctionCode.DEBUGPRINT, new DEBUGPRINT_Instruction(form: false, newline: false));
		addFunction(FunctionCode.DEBUGPRINTL, new DEBUGPRINT_Instruction(form: false, newline: true));
		addFunction(FunctionCode.DEBUGPRINTFORM, new DEBUGPRINT_Instruction(form: true, newline: false));
		addFunction(FunctionCode.DEBUGPRINTFORML, new DEBUGPRINT_Instruction(form: true, newline: true));
		addFunction(FunctionCode.DEBUGCLEAR, new DEBUGCLEAR_Instruction());
		addFunction(FunctionCode.ASSERT, argumentBuilderDictionary[FunctionArgType.INT_EXPRESSION], 14);
		addFunction(FunctionCode.THROW, argumentBuilderDictionary[FunctionArgType.FORM_STR_NULLABLE], 6);
		addFunction(FunctionCode.SAVEVAR, new SAVEVAR_Instruction());
		addFunction(FunctionCode.LOADVAR, new LOADVAR_Instruction());
		addFunction(FunctionCode.SAVECHARA, new SAVECHARA_Instruction());
		addFunction(FunctionCode.LOADCHARA, new LOADCHARA_Instruction());
		addFunction(FunctionCode.REF, new REF_Instruction(byname: false));
		addFunction(FunctionCode.REFBYNAME, new REF_Instruction(byname: true));
		addFunction(FunctionCode.HTML_PRINT, new HTML_PRINT_Instruction());
		addFunction(FunctionCode.HTML_TAGSPLIT, new HTML_TAGSPLIT_Instruction());
		addFunction(FunctionCode.PRINT_IMG, new PRINT_IMG_Instruction());
		addFunction(FunctionCode.PRINT_RECT, new PRINT_RECT_Instruction());
		addFunction(FunctionCode.PRINT_SPACE, new PRINT_SPACE_Instruction());
		addFunction(FunctionCode.TOOLTIP_SETCOLOR, new TOOLTIP_SETCOLOR_Instruction());
		addFunction(FunctionCode.TOOLTIP_SETDELAY, new TOOLTIP_SETDELAY_Instruction());
		addFunction(FunctionCode.VARSIZE, argumentBuilderDictionary[FunctionArgType.SP_VAR], 6);
		addFunction(FunctionCode.GETTIME, argumentBuilderDictionary[FunctionArgType.VOID], 6);
		addFunction(FunctionCode.POWER, argumentBuilderDictionary[FunctionArgType.SP_POWER], 6);
		addFunction(FunctionCode.PRINTCPERLINE, argumentBuilderDictionary[FunctionArgType.SP_GETINT], 6);
		addFunction(FunctionCode.SAVENOS, argumentBuilderDictionary[FunctionArgType.SP_GETINT], 6);
		addFunction(FunctionCode.ENCODETOUNI, argumentBuilderDictionary[FunctionArgType.FORM_STR_NULLABLE], 6);
		foreach (KeyValuePair<string, FunctionMethod> method in FunctionMethodCreator.GetMethodList())
		{
			string key = method.Key;
			if (!funcDic.ContainsKey(key))
			{
				funcDic.Add(key, new FunctionIdentifier(key, method.Value, methodInstruction));
			}
		}
		funcMatch[FunctionCode.IF] = "ENDIF";
		funcMatch[FunctionCode.SELECTCASE] = "ENDSELECT";
		funcMatch[FunctionCode.REPEAT] = "REND";
		funcMatch[FunctionCode.FOR] = "NEXT";
		funcMatch[FunctionCode.WHILE] = "WEND";
		funcMatch[FunctionCode.TRYCGOTO] = "CATCH";
		funcMatch[FunctionCode.TRYCJUMP] = "CATCH";
		funcMatch[FunctionCode.TRYCCALL] = "CATCH";
		funcMatch[FunctionCode.TRYCGOTOFORM] = "CATCH";
		funcMatch[FunctionCode.TRYCJUMPFORM] = "CATCH";
		funcMatch[FunctionCode.TRYCCALLFORM] = "CATCH";
		funcMatch[FunctionCode.CATCH] = "ENDCATCH";
		funcMatch[FunctionCode.DO] = "LOOP";
		funcMatch[FunctionCode.PRINTDATA] = "ENDDATA";
		funcMatch[FunctionCode.PRINTDATAL] = "ENDDATA";
		funcMatch[FunctionCode.PRINTDATAW] = "ENDDATA";
		funcMatch[FunctionCode.PRINTDATAK] = "ENDDATA";
		funcMatch[FunctionCode.PRINTDATAKL] = "ENDDATA";
		funcMatch[FunctionCode.PRINTDATAKW] = "ENDDATA";
		funcMatch[FunctionCode.PRINTDATAD] = "ENDDATA";
		funcMatch[FunctionCode.PRINTDATADL] = "ENDDATA";
		funcMatch[FunctionCode.PRINTDATADW] = "ENDDATA";
		funcMatch[FunctionCode.DATALIST] = "ENDLIST";
		funcMatch[FunctionCode.STRDATA] = "ENDDATA";
		funcMatch[FunctionCode.NOSKIP] = "ENDNOSKIP";
		funcMatch[FunctionCode.TRYCALLLIST] = "ENDFUNC";
		funcMatch[FunctionCode.TRYGOTOLIST] = "ENDFUNC";
		funcMatch[FunctionCode.TRYJUMPLIST] = "ENDFUNC";
		funcParent[FunctionCode.REND] = FunctionCode.REPEAT;
		funcParent[FunctionCode.NEXT] = FunctionCode.FOR;
		funcParent[FunctionCode.WEND] = FunctionCode.WHILE;
		funcParent[FunctionCode.LOOP] = FunctionCode.DO;
	}

	internal static string getMatchFunction(FunctionCode func)
	{
		string value = null;
		funcMatch.TryGetValue(func, out value);
		return value;
	}

	internal static FunctionCode getParentFunc(FunctionCode func)
	{
		FunctionCode value = FunctionCode.__NULL__;
		funcParent.TryGetValue(func, out value);
		return value;
	}

	private FunctionIdentifier(string name, FunctionCode code, AbstractInstruction instruction)
		: this(name, code, instruction, 0)
	{
	}

	private FunctionIdentifier(string name, FunctionCode code, AbstractInstruction instruction, int additionalFlag)
	{
		this.code = code;
		arg = instruction.ArgBuilder;
		flag = instruction.Flag | additionalFlag;
		method = null;
		Name = name;
		Instruction = instruction;
	}

	private FunctionIdentifier(string name, FunctionCode code, ArgumentBuilder arg, int flag)
	{
		this.code = code;
		this.arg = arg;
		this.flag = flag;
		method = null;
		Name = name;
		Instruction = null;
	}

	private FunctionIdentifier(string methodName, FunctionMethod method, AbstractInstruction instruction)
	{
		code = FunctionCode.__NULL__;
		arg = instruction.ArgBuilder;
		flag = instruction.Flag;
		this.method = method;
		Name = methodName;
		Instruction = instruction;
	}

	internal bool IsFlowContorol()
	{
		return (flag & 1) == 1;
	}

	internal bool IsExtended()
	{
		return (flag & 2) == 2;
	}

	internal bool IsPrintDFunction()
	{
		return (flag & 0x800) == 2048;
	}

	internal bool IsPrintKFunction()
	{
		return (flag & 0x1000) == 4096;
	}

	internal bool IsNewLine()
	{
		return (flag & 0x100) == 256;
	}

	internal bool IsWaitInput()
	{
		return (flag & 0x200) == 512;
	}

	internal bool IsPrintSingle()
	{
		return (flag & 0x400) == 1024;
	}

	internal bool IsPartial()
	{
		return (flag & 0x10) == 16;
	}

	internal bool IsMethodSafe()
	{
		return (flag & 4) == 4;
	}

	internal bool IsPrint()
	{
		return (flag & 0x2000) == 8192;
	}

	internal bool IsInput()
	{
		return (flag & 0x4000) == 16384;
	}

	internal bool IsPrintData()
	{
		return (flag & 0x10000) == 65536;
	}

	internal bool IsForceSetArg()
	{
		return (flag & 0x20) == 32;
	}

	internal bool IsDebug()
	{
		return (flag & 8) == 8;
	}

	internal bool IsTry()
	{
		return (flag & 0x80) == 128;
	}

	internal bool IsJump()
	{
		return (flag & 0x40) == 64;
	}

	internal bool IsMethod()
	{
		return method != null;
	}

	public override string ToString()
	{
		return Name;
	}

	private static int toUInt32inArg(long value, string funcName, int argnum)
	{
		if (value < 0)
		{
			throw new CodeEE(funcName + "の第" + argnum + "引数に負の値(" + value + ")が指定されました");
		}
		if (value > int.MaxValue)
		{
			throw new CodeEE(funcName + "の第" + argnum + "引数の値(" + value + ")が大きすぎます");
		}
		return (int)value;
	}
}
