using System.Collections.Generic;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Expression;

internal static class ExpressionParser
{
	private class TermStack
	{
		private int state;

		private bool hasBefore;

		private bool hasAfter;

		private bool waitAfter;

		private Stack<object> stack = new Stack<object>();

		public void Add(OperatorCode op)
		{
			if (state == 2 || state == 3)
			{
				throw new CodeEE("式が異常です");
			}
			if (state == 0)
			{
				if (!OperatorManager.IsUnary(op))
				{
					throw new CodeEE("式が異常です");
				}
				stack.Push(op);
				if (op == OperatorCode.Plus || op == OperatorCode.Minus || op == OperatorCode.BitNot)
				{
					state = 2;
				}
				else
				{
					state = 3;
				}
				return;
			}
			if (state == 1)
			{
				if (OperatorManager.IsUnaryAfter(op))
				{
					if (hasAfter)
					{
						hasAfter = false;
						throw new CodeEE("後置の単項演算子が複数存在しています");
					}
					if (hasBefore)
					{
						hasBefore = false;
						throw new CodeEE("インクリメント・デクリメントを前置・後置両方同時に使うことはできません");
					}
					stack.Push(op);
					reduceUnaryAfter();
					if (waitAfter)
					{
						reduceUnary();
					}
					hasBefore = false;
					hasAfter = true;
					waitAfter = false;
				}
				else
				{
					if (!OperatorManager.IsBinary(op) && !OperatorManager.IsTernary(op))
					{
						throw new CodeEE("式が異常です");
					}
					if (waitAfter)
					{
						reduceUnary();
					}
					int priority = OperatorManager.GetPriority(op);
					while (lastPriority() >= priority)
					{
						reduceLastThree();
					}
					stack.Push(op);
					state = 0;
					waitAfter = false;
					hasBefore = false;
					hasAfter = false;
				}
				return;
			}
			throw new CodeEE("式が異常です");
		}

		public void Add(long i)
		{
			Add(new SingleTerm(i));
		}

		public void Add(string s)
		{
			Add(new SingleTerm(s));
		}

		public void Add(IOperandTerm term)
		{
			stack.Push(term);
			if (state == 1)
			{
				throw new CodeEE("式が異常です");
			}
			if (state == 2)
			{
				waitAfter = true;
			}
			if (state == 3)
			{
				reduceUnary();
				hasBefore = true;
			}
			state = 1;
		}

		private int lastPriority()
		{
			if (stack.Count < 3)
			{
				return -1;
			}
			object item = stack.Pop();
			int priority = OperatorManager.GetPriority((OperatorCode)stack.Peek());
			stack.Push(item);
			return priority;
		}

		public IOperandTerm ReduceAll()
		{
			if (stack.Count == 0)
			{
				return null;
			}
			if (state != 1)
			{
				throw new CodeEE("式が異常です");
			}
			if (waitAfter)
			{
				reduceUnary();
			}
			waitAfter = false;
			hasBefore = false;
			hasAfter = false;
			while (stack.Count > 1)
			{
				reduceLastThree();
			}
			return (IOperandTerm)stack.Pop();
		}

		private void reduceUnary()
		{
			IOperandTerm o = (IOperandTerm)stack.Pop();
			IOperandTerm item = OperatorMethodManager.ReduceUnaryTerm((OperatorCode)stack.Pop(), o);
			stack.Push(item);
		}

		private void reduceUnaryAfter()
		{
			OperatorCode op = (OperatorCode)stack.Pop();
			IOperandTerm o = (IOperandTerm)stack.Pop();
			IOperandTerm item = OperatorMethodManager.ReduceUnaryAfterTerm(op, o);
			stack.Push(item);
		}

		private void reduceLastThree()
		{
			IOperandTerm right = (IOperandTerm)stack.Pop();
			OperatorCode operatorCode = (OperatorCode)stack.Pop();
			IOperandTerm left = (IOperandTerm)stack.Pop();
			if (OperatorManager.IsTernary(operatorCode))
			{
				if (stack.Count <= 1)
				{
					throw new CodeEE("式の数が不足しています");
				}
				reduceTernary(left, right, operatorCode);
			}
			else
			{
				IOperandTerm item = OperatorMethodManager.ReduceBinaryTerm(operatorCode, left, right);
				stack.Push(item);
			}
		}

		private void reduceTernary(IOperandTerm left, IOperandTerm right, OperatorCode op)
		{
			OperatorCode op2 = (OperatorCode)stack.Pop();
			IOperandTerm o = (IOperandTerm)stack.Pop();
			IOperandTerm item = OperatorMethodManager.ReduceTernaryTerm(op2, o, left, right);
			stack.Push(item);
		}

		private SingleTerm GetSingle(IOperandTerm oprand)
		{
			return (SingleTerm)oprand;
		}
	}

	public static IOperandTerm[] ReduceArguments(WordCollection wc, ArgsEndWith endWith, bool isDefine)
	{
		if (wc == null)
		{
			throw new ExeEE("空のストリームを渡された");
		}
		List<IOperandTerm> list = new List<IOperandTerm>();
		TermEndWith termEndWith = TermEndWith.EoL;
		switch (endWith)
		{
		case ArgsEndWith.EoL:
			termEndWith = TermEndWith.Comma;
			break;
		case ArgsEndWith.RightParenthesis:
			termEndWith = TermEndWith.RightParenthesis_Comma;
			break;
		}
		TermEndWith endWith2 = termEndWith | TermEndWith.Assignment;
		while (true)
		{
			switch (wc.Current.Type)
			{
			case '\0':
				switch (endWith)
				{
				case ArgsEndWith.RightBracket:
					throw new CodeEE("'['に対応する']'が見つかりません");
				case ArgsEndWith.RightParenthesis:
					throw new CodeEE("'('に対応する')'が見つかりません");
				}
				break;
			case ')':
				if (endWith == ArgsEndWith.RightParenthesis)
				{
					wc.ShiftNext();
					break;
				}
				throw new CodeEE("構文解析中に予期しない')'を発見しました");
			case ']':
				if (endWith == ArgsEndWith.RightBracket)
				{
					wc.ShiftNext();
					break;
				}
				throw new CodeEE("構文解析中に予期しない']'を発見しました");
			default:
				if (!isDefine)
				{
					list.Add(ReduceExpressionTerm(wc, termEndWith));
				}
				else
				{
					list.Add(ReduceExpressionTerm(wc, endWith2));
					if (list[list.Count - 1] == null)
					{
						throw new CodeEE("関数定義の引数は省略できません");
					}
					if (wc.Current is OperatorWord)
					{
						wc.ShiftNext();
						IOperandTerm operandTerm = reduceTerm(wc, allowKeywordTo: false, termEndWith, VariableCode.__NULL__);
						if (operandTerm == null)
						{
							throw new CodeEE("'='の後に式がありません");
						}
						if (operandTerm.GetOperandType() != list[list.Count - 1].GetOperandType())
						{
							throw new CodeEE("'='の前後で型が一致しません");
						}
						list.Add(operandTerm);
					}
					else if (list[list.Count - 1].GetOperandType() == typeof(long))
					{
						list.Add(new NullTerm(0L));
					}
					else
					{
						list.Add(new NullTerm(""));
					}
				}
				if (wc.Current.Type == ',')
				{
					wc.ShiftNext();
				}
				continue;
			}
			break;
		}
		IOperandTerm[] array = new IOperandTerm[list.Count];
		list.CopyTo(array);
		return array;
	}

	public static IOperandTerm ReduceExpressionTerm(WordCollection wc, TermEndWith endWith)
	{
		return reduceTerm(wc, allowKeywordTo: false, endWith, VariableCode.__NULL__);
	}

	public static IOperandTerm ReduceIntegerTerm(WordCollection wc, TermEndWith endwith)
	{
		IOperandTerm obj = reduceTerm(wc, allowKeywordTo: false, endwith, VariableCode.__NULL__) ?? throw new CodeEE("構文を式として解釈できません");
		if (obj.GetOperandType() != typeof(long))
		{
			throw new CodeEE("式の結果が数値ではありません");
		}
		return obj;
	}

	public static IOperandTerm ToStrFormTerm(StrFormWord sfw)
	{
		StrForm strForm = StrForm.FromWordToken(sfw);
		if (strForm.IsConst)
		{
			return new SingleTerm(strForm.GetString(null));
		}
		return new StrFormTerm(strForm);
	}

	public static CaseExpression[] ReduceCaseExpressions(WordCollection wc)
	{
		List<CaseExpression> list = new List<CaseExpression>();
		while (!wc.EOL)
		{
			list.Add(reduceCaseExpression(wc));
			wc.ShiftNext();
		}
		CaseExpression[] array = new CaseExpression[list.Count];
		list.CopyTo(array);
		return array;
	}

	public static IOperandTerm ReduceVariableArgument(WordCollection wc, VariableCode varCode)
	{
		return reduceTerm(wc, allowKeywordTo: false, TermEndWith.EoL, varCode) ?? throw new CodeEE("変数の:の後に引数がありません");
	}

	public static VariableToken ReduceVariableIdentifier(WordCollection wc, string idStr)
	{
		string subKey = null;
		if (wc.Current.Type == '@')
		{
			wc.ShiftNext();
			IdentifierWord obj = (wc.Current as IdentifierWord) ?? throw new CodeEE("@の使い方が不正です");
			wc.ShiftNext();
			subKey = obj.Code;
		}
		return GlobalStatic.IdentifierDictionary.GetVariableToken(idStr, subKey, allowPrivate: true);
	}

	private static IOperandTerm reduceIdentifier(WordCollection wc, string idStr, VariableCode varCode)
	{
		wc.ShiftNext();
		SymbolWord symbolWord = wc.Current as SymbolWord;
		if (symbolWord != null && symbolWord.Type == '.')
		{
			throw new NotImplCodeEE();
		}
		if (symbolWord != null && (symbolWord.Type == '(' || symbolWord.Type == '['))
		{
			wc.ShiftNext();
			if (symbolWord.Type == '[')
			{
				throw new CodeEE("[]を使った機能はまだ実装されていません");
			}
			IOperandTerm[] arguments = ReduceArguments(wc, ArgsEndWith.RightParenthesis, isDefine: false);
			IOperandTerm functionMethod = GlobalStatic.IdentifierDictionary.GetFunctionMethod(GlobalStatic.LabelDictionary, idStr, arguments, userDefinedOnly: false);
			if (functionMethod == null)
			{
				if (Program.AnalysisMode)
				{
					if (GlobalStatic.tempDic.ContainsKey(idStr))
					{
						GlobalStatic.tempDic[idStr]++;
					}
					else
					{
						GlobalStatic.tempDic.Add(idStr, 1L);
					}
					return new NullTerm(0L);
				}
				GlobalStatic.IdentifierDictionary.ThrowException(idStr, isFunc: true);
			}
			return functionMethod;
		}
		VariableToken variableToken = ReduceVariableIdentifier(wc, idStr);
		if (variableToken != null)
		{
			if (varCode != VariableCode.__NULL__)
			{
				return VariableParser.ReduceVariable(variableToken, null, null, null);
			}
			return VariableParser.ReduceVariable(variableToken, wc);
		}
		IOperandTerm functionMethod2 = GlobalStatic.IdentifierDictionary.GetFunctionMethod(GlobalStatic.LabelDictionary, idStr, null, userDefinedOnly: false);
		if (functionMethod2 != null)
		{
			return functionMethod2;
		}
		if (varCode != VariableCode.__NULL__ && GlobalStatic.ConstantData.isDefined(varCode, idStr))
		{
			return new SingleTerm(idStr);
		}
		GlobalStatic.IdentifierDictionary.ThrowException(idStr, isFunc: false);
		throw new ExeEE("エラー投げ損ねた");
	}

	private static CaseExpression reduceCaseExpression(WordCollection wc)
	{
		CaseExpression caseExpression = new CaseExpression();
		if (wc.Current is IdentifierWord identifierWord && identifierWord.Code.Equals("IS", Config.SCVariable))
		{
			wc.ShiftNext();
			caseExpression.CaseType = CaseExpressionType.Is;
			OperatorCode code = ((wc.Current as OperatorWord) ?? throw new CodeEE("ISキーワードの後に演算子がありません")).Code;
			if (!OperatorManager.IsBinary(code))
			{
				throw new CodeEE("ISキーワードの後の演算子が2項演算子ではありません");
			}
			wc.ShiftNext();
			caseExpression.Operator = code;
			caseExpression.LeftTerm = reduceTerm(wc, allowKeywordTo: false, TermEndWith.Comma, VariableCode.__NULL__);
			if (caseExpression.LeftTerm == null)
			{
				throw new CodeEE("ISキーワードの後に式がありません");
			}
			caseExpression.LeftTerm.GetOperandType();
			return caseExpression;
		}
		caseExpression.LeftTerm = reduceTerm(wc, allowKeywordTo: true, TermEndWith.Comma, VariableCode.__NULL__);
		if (caseExpression.LeftTerm == null)
		{
			throw new CodeEE("CASEの引数は省略できません");
		}
		if (wc.Current is IdentifierWord identifierWord2 && identifierWord2.Code.Equals("TO", Config.SCVariable))
		{
			caseExpression.CaseType = CaseExpressionType.To;
			wc.ShiftNext();
			caseExpression.RightTerm = reduceTerm(wc, allowKeywordTo: true, TermEndWith.Comma, VariableCode.__NULL__);
			if (caseExpression.RightTerm == null)
			{
				throw new CodeEE("TOキーワードの後に式がありません");
			}
			if (wc.Current is IdentifierWord identifierWord3 && identifierWord3.Code.Equals("TO", Config.SCVariable))
			{
				throw new CodeEE("TOキーワードが2度使われています");
			}
			if (caseExpression.LeftTerm.GetOperandType() != caseExpression.RightTerm.GetOperandType())
			{
				throw new CodeEE("TOキーワードの前後の型が一致していません");
			}
			return caseExpression;
		}
		caseExpression.CaseType = CaseExpressionType.Normal;
		return caseExpression;
	}

	private static IOperandTerm reduceTerm(WordCollection wc, bool allowKeywordTo, TermEndWith endWith, VariableCode varCode)
	{
		TermStack termStack = new TermStack();
		int num = 0;
		OperatorCode operatorCode = OperatorCode.NULL;
		bool flag = varCode != VariableCode.__NULL__;
		do
		{
			Word current = wc.Current;
			switch (current.Type)
			{
			case '"':
				termStack.Add(((LiteralStringWord)current).Str);
				goto IL_0344;
			case '0':
				termStack.Add(((LiteralIntegerWord)current).Int);
				goto IL_0344;
			case 'F':
				termStack.Add(ToStrFormTerm((StrFormWord)current));
				goto IL_0344;
			case 'A':
			{
				string code2 = ((IdentifierWord)current).Code;
				if (code2.Equals("TO", Config.SCVariable))
				{
					if (!allowKeywordTo)
					{
						throw new CodeEE("TOキーワードはここでは使用できません");
					}
					break;
				}
				if (code2.Equals("IS", Config.SCVariable))
				{
					throw new CodeEE("ISキーワードはここでは使用できません");
				}
				termStack.Add(reduceIdentifier(wc, code2, varCode));
				continue;
			}
			case '=':
			{
				if (flag)
				{
					throw new CodeEE("変数の引数の読み取り中に予期しない演算子を発見しました");
				}
				OperatorCode code = ((OperatorWord)current).Code;
				if (code == OperatorCode.Assignment)
				{
					if ((endWith & TermEndWith.Assignment) != TermEndWith.Assignment)
					{
						throw new CodeEE("式中で代入演算子'='が使われています(等価比較には'=='を使用してください)");
					}
					break;
				}
				if ((operatorCode == OperatorCode.Equal || operatorCode == OperatorCode.Greater || operatorCode == OperatorCode.Less || operatorCode == OperatorCode.GreaterEqual || operatorCode == OperatorCode.LessEqual || operatorCode == OperatorCode.NotEqual) && (code == OperatorCode.Equal || code == OperatorCode.Greater || code == OperatorCode.Less || code == OperatorCode.GreaterEqual || code == OperatorCode.LessEqual || code == OperatorCode.NotEqual))
				{
					ParserMediator.Warn("（構文上の注意）比較演算子が連続しています。", GlobalStatic.Process.GetScaningLine(), 0, isError: false, isBackComp: false);
				}
				termStack.Add(code);
				operatorCode = code;
				switch (code)
				{
				case OperatorCode.Ternary_a:
					num++;
					break;
				case OperatorCode.Ternary_b:
					if (num > 0)
					{
						num--;
						break;
					}
					throw new CodeEE("対応する'?'のない'#'です");
				}
				goto IL_0344;
			}
			case '(':
			{
				wc.ShiftNext();
				IOperandTerm operandTerm = reduceTerm(wc, allowKeywordTo: false, TermEndWith.RightParenthesis, VariableCode.__NULL__);
				if (operandTerm == null)
				{
					throw new CodeEE("かっこ\"(\"～\")\"の中に式が含まれていません");
				}
				termStack.Add(operandTerm);
				if (wc.Current.Type != ')')
				{
					throw new CodeEE("対応する')'のない'('です");
				}
				wc.ShiftNext();
				continue;
			}
			case ')':
				if ((endWith & TermEndWith.RightParenthesis) != TermEndWith.RightParenthesis)
				{
					throw new CodeEE("構文解釈中に予期しない記号'" + current.Type + "'を発見しました");
				}
				break;
			case ']':
				if ((endWith & TermEndWith.RightBracket) != TermEndWith.RightBracket)
				{
					throw new CodeEE("構文解釈中に予期しない記号'" + current.Type + "'を発見しました");
				}
				break;
			case ',':
				if ((endWith & TermEndWith.Comma) != TermEndWith.Comma)
				{
					throw new CodeEE("構文解釈中に予期しない記号'" + current.Type + "'を発見しました");
				}
				break;
			case 'M':
				throw new ExeEE("マクロ解決失敗");
			default:
				throw new CodeEE("構文解釈中に予期しない記号'" + current.Type + "'を発見しました");
			case '\0':
				break;
				IL_0344:
				wc.ShiftNext();
				continue;
			}
			break;
		}
		while (!flag);
		if (num > 0)
		{
			throw new CodeEE("'?'と'#'の数が正しく対応していません");
		}
		return termStack.ReduceAll();
	}
}
