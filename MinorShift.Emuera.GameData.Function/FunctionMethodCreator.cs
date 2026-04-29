using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MinorShift._Library;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.GameView;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Function;

internal static class FunctionMethodCreator
{
	private sealed class GetcharaMethod : FunctionMethod
	{
		public GetcharaMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = false;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 2)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (arguments[0].GetOperandType() != typeof(long))
			{
				return name + "関数の1番目の引数の型が正しくありません";
			}
			if (arguments.Length == 2 && arguments[1] != null && arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の引数の型が正しくありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			long num = -1L;
			if (!Config.CompatiSPChara)
			{
				return exm.VEvaluator.GetChara(intValue);
			}
			bool flag = false;
			if (arguments.Length > 1 && arguments[1] != null && arguments[1].GetIntValue(exm) != 0L)
			{
				flag = true;
			}
			if (flag)
			{
				num = exm.VEvaluator.GetChara_UseSp(intValue, getSp: false);
				if (num != -1)
				{
					return num;
				}
				return exm.VEvaluator.GetChara_UseSp(intValue, getSp: true);
			}
			return exm.VEvaluator.GetChara_UseSp(intValue, getSp: false);
		}
	}

	private sealed class GetspcharaMethod : FunctionMethod
	{
		public GetspcharaMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(long) };
			base.CanRestructure = false;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (!Config.CompatiSPChara)
			{
				throw new CodeEE("SPキャラ関係の機能は標準では使用できません(互換性オプション「SPキャラを使用する」をONにしてください)");
			}
			long intValue = arguments[0].GetIntValue(exm);
			return exm.VEvaluator.GetChara_UseSp(intValue, getSp: true);
		}
	}

	private sealed class CsvStrDataMethod : FunctionMethod
	{
		private CharacterStrData charaStr;

		public CsvStrDataMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = null;
			charaStr = CharacterStrData.NAME;
			base.CanRestructure = true;
		}

		public CsvStrDataMethod(CharacterStrData cStr)
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = null;
			charaStr = cStr;
			base.CanRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 2)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!arguments[0].IsInteger)
			{
				return name + "関数の1番目の引数が数値ではありません";
			}
			if (arguments.Length == 1)
			{
				return null;
			}
			if (arguments[1] != null && arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の変数が数値ではありません";
			}
			return null;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			long num = ((arguments.Length > 1 && arguments[1] != null) ? arguments[1].GetIntValue(exm) : 0);
			if (!Config.CompatiSPChara && num != 0L)
			{
				throw new CodeEE("SPキャラ関係の機能は標準では使用できません(互換性オプション「SPキャラを使用する」をONにしてください)");
			}
			return exm.VEvaluator.GetCharacterStrfromCSVData(intValue, charaStr, num != 0, 0L);
		}
	}

	private sealed class CsvcstrMethod : FunctionMethod
	{
		public CsvcstrMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = null;
			base.CanRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 2)
			{
				return name + "関数には少なくとも2つの引数が必要です";
			}
			if (arguments.Length > 3)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!arguments[0].IsInteger)
			{
				return name + "関数の1番目の引数が数値ではありません";
			}
			if (arguments[1] == null)
			{
				return name + "関数の2番目の引数は省略できません";
			}
			if (arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の変数が数値ではありません";
			}
			if (arguments.Length == 2)
			{
				return null;
			}
			if (arguments[2] != null && arguments[2].GetOperandType() != typeof(long))
			{
				return name + "関数の3番目の変数が数値ではありません";
			}
			return null;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			long intValue2 = arguments[1].GetIntValue(exm);
			long num = ((arguments.Length == 3 && arguments[2] != null) ? arguments[2].GetIntValue(exm) : 0);
			if (!Config.CompatiSPChara && num != 0L)
			{
				throw new CodeEE("SPキャラ関係の機能は標準では使用できません(互換性オプション「SPキャラを使用する」をONにしてください)");
			}
			return exm.VEvaluator.GetCharacterStrfromCSVData(intValue, CharacterStrData.CSTR, num != 0, intValue2);
		}
	}

	private sealed class CsvDataMethod : FunctionMethod
	{
		private CharacterIntData charaInt;

		public CsvDataMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			charaInt = CharacterIntData.BASE;
			base.CanRestructure = true;
		}

		public CsvDataMethod(CharacterIntData cInt)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			charaInt = cInt;
			base.CanRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 2)
			{
				return name + "関数には少なくとも2つの引数が必要です";
			}
			if (arguments.Length > 3)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!arguments[0].IsInteger)
			{
				return name + "関数の1番目の引数が数値ではありません";
			}
			if (arguments[1] == null)
			{
				return name + "関数の2番目の引数は省略できません";
			}
			if (arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の変数が数値ではありません";
			}
			if (arguments.Length == 2)
			{
				return null;
			}
			if (arguments[2] != null && arguments[2].GetOperandType() != typeof(long))
			{
				return name + "関数の3番目の変数が数値ではありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			long intValue2 = arguments[1].GetIntValue(exm);
			long num = ((arguments.Length == 3 && arguments[2] != null) ? arguments[2].GetIntValue(exm) : 0);
			if (!Config.CompatiSPChara && num != 0L)
			{
				throw new CodeEE("SPキャラ関係の機能は標準では使用できません(互換性オプション「SPキャラを使用する」をONにしてください)");
			}
			return exm.VEvaluator.GetCharacterIntfromCSVData(intValue, charaInt, num != 0, intValue2);
		}
	}

	private sealed class FindcharaMethod : FunctionMethod
	{
		private bool isLast;

		public FindcharaMethod(bool last)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = false;
			isLast = last;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 2)
			{
				return name + "関数には少なくとも2つの引数が必要です";
			}
			if (arguments.Length > 4)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!(arguments[0] is VariableTerm))
			{
				return name + "関数の1番目の引数の型が正しくありません";
			}
			if (!((VariableTerm)arguments[0]).Identifier.IsCharacterData)
			{
				return name + "関数の1番目の引数の変数がキャラクタ変数ではありません";
			}
			if (arguments[1] == null)
			{
				return name + "関数の2番目の引数は省略できません";
			}
			if (arguments[1].GetOperandType() != arguments[0].GetOperandType())
			{
				return name + "関数の2番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 3 && arguments[2] != null && arguments[2].GetOperandType() != typeof(long))
			{
				return name + "関数の3番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 4 && arguments[3] != null && arguments[3].GetOperandType() != typeof(long))
			{
				return name + "関数の4番目の引数の型が正しくありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			VariableTerm variableTerm = (VariableTerm)arguments[0];
			VariableToken identifier = variableTerm.Identifier;
			long elem = 0L;
			if (variableTerm.Identifier.IsArray1D)
			{
				elem = variableTerm.GetElementInt(1, exm);
			}
			else if (variableTerm.Identifier.IsArray2D)
			{
				elem = variableTerm.GetElementInt(1, exm) << 32;
				elem += variableTerm.GetElementInt(2, exm);
			}
			long num = 0L;
			long num2 = exm.VEvaluator.CHARANUM;
			if (arguments.Length >= 3 && arguments[2] != null)
			{
				num = arguments[2].GetIntValue(exm);
			}
			if (arguments.Length >= 4 && arguments[3] != null)
			{
				num2 = arguments[3].GetIntValue(exm);
			}
			long num3 = -1L;
			if (num < 0 || num >= exm.VEvaluator.CHARANUM)
			{
				throw new CodeEE((isLast ? "" : "") + "関数の第3引数(" + num + ")はキャラクタ位置の範囲外です");
			}
			if (num2 < 0 || num2 > exm.VEvaluator.CHARANUM)
			{
				throw new CodeEE((isLast ? "" : "") + "関数の第4引数(" + num2 + ")はキャラクタ位置の範囲外です");
			}
			if (identifier.IsString)
			{
				string strValue = arguments[1].GetStrValue(exm);
				return exm.VEvaluator.FindChara(identifier, elem, strValue, num, num2, isLast);
			}
			long intValue = arguments[1].GetIntValue(exm);
			return exm.VEvaluator.FindChara(identifier, elem, intValue, num, num2, isLast);
		}
	}

	private sealed class ExistCsvMethod : FunctionMethod
	{
		public ExistCsvMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 2)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!arguments[0].IsInteger)
			{
				return name + "関数の1番目の引数が数値ではありません";
			}
			if (arguments.Length == 1)
			{
				return null;
			}
			if (arguments[1] != null && arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の変数が数値ではありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			bool flag = arguments.Length == 2 && arguments[1] != null && arguments[1].GetIntValue(exm) != 0;
			if (!Config.CompatiSPChara && flag)
			{
				throw new CodeEE("SPキャラ関係の機能は標準では使用できません(互換性オプション「SPキャラを使用する」をONにしてください)");
			}
			return exm.VEvaluator.ExistCsv(intValue, flag);
		}
	}

	private sealed class VarsizeMethod : FunctionMethod
	{
		public VarsizeMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = true;
			base.HasUniqueRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 2)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!arguments[0].IsString)
			{
				return name + "関数の1番目の引数が文字列ではありません";
			}
			if (arguments[0] is SingleTerm)
			{
				string str = ((SingleTerm)arguments[0]).Str;
				if (GlobalStatic.IdentifierDictionary.GetVariableToken(str, null, allowPrivate: true) == null)
				{
					return name + "関数の1番目の引数が変数名ではありません";
				}
			}
			if (arguments.Length == 1)
			{
				return null;
			}
			if (arguments[1] != null && arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の変数が数値ではありません";
			}
			_ = arguments.Length;
			_ = 2;
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			VariableToken obj = GlobalStatic.IdentifierDictionary.GetVariableToken(arguments[0].GetStrValue(exm), null, allowPrivate: true) ?? throw new CodeEE("VARSIZEの1番目の引数(\"" + arguments[0].GetStrValue(exm) + "\")が変数名ではありません");
			int dimension = 0;
			if (arguments.Length == 2 && arguments[1] != null)
			{
				dimension = (int)arguments[1].GetIntValue(exm);
			}
			return obj.GetLength(dimension);
		}

		public override bool UniqueRestructure(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			arguments[0].Restructure(exm);
			if (arguments.Length > 1)
			{
				arguments[1].Restructure(exm);
			}
			if (arguments[0] is SingleTerm && (arguments.Length == 1 || arguments[1] is SingleTerm))
			{
				VariableToken variableToken = GlobalStatic.IdentifierDictionary.GetVariableToken(arguments[0].GetStrValue(exm), null, allowPrivate: true);
				if (variableToken == null || variableToken.IsReference)
				{
					return false;
				}
				return true;
			}
			return false;
		}
	}

	private sealed class CheckfontMethod : FunctionMethod
	{
		public CheckfontMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return 1L;
		}
	}

	private sealed class CheckdataMethod : FunctionMethod
	{
		private string name;

		private EraSaveFileType type;

		public CheckdataMethod(string name, EraSaveFileType type)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(long) };
			base.CanRestructure = false;
			this.name = name;
			this.type = type;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			if (intValue < 0)
			{
				throw new CodeEE(name + "の引数に負の値(" + intValue + ")が指定されました");
			}
			if (intValue > int.MaxValue)
			{
				throw new CodeEE(name + "の引数(" + intValue + ")が大きすぎます");
			}
			EraDataResult eraDataResult = null;
			eraDataResult = exm.VEvaluator.CheckData((int)intValue, type);
			exm.VEvaluator.RESULTS = eraDataResult.DataMes;
			return (long)eraDataResult.State;
		}
	}

	private sealed class CheckdataStrMethod : FunctionMethod
	{
		private string name;

		private EraSaveFileType type;

		public CheckdataStrMethod(string name, EraSaveFileType type)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = false;
			this.name = name;
			this.type = type;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			EraDataResult eraDataResult = null;
			eraDataResult = exm.VEvaluator.CheckData(strValue, type);
			exm.VEvaluator.RESULTS = eraDataResult.DataMes;
			return (long)eraDataResult.State;
		}
	}

	private sealed class FindFilesMethod : FunctionMethod
	{
		private string name;

		private EraSaveFileType type;

		public FindFilesMethod(string name, EraSaveFileType type)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = false;
			this.name = name;
			this.type = type;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length > 1)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments.Length == 0 || arguments[0] == null)
			{
				return null;
			}
			if (!arguments[0].IsString)
			{
				return name + "関数の1番目の引数が文字列ではありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string pattern = "*";
			if (arguments.Length != 0 && arguments[0] != null)
			{
				pattern = arguments[0].GetStrValue(exm);
			}
			List<string> list = null;
			list = exm.VEvaluator.GetDatFiles(type == EraSaveFileType.CharVar, pattern);
			string[] array = exm.VEvaluator.VariableData.DataStringArray[2];
			if (list.Count <= array.Length)
			{
				list.CopyTo(array);
			}
			else
			{
				list.CopyTo(0, array, 0, array.Length);
			}
			return list.Count;
		}
	}

	private sealed class IsSkipMethod : FunctionMethod
	{
		public IsSkipMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = false;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (!exm.Process.SkipPrint)
			{
				return 0L;
			}
			return 1L;
		}
	}

	private sealed class MesSkipMethod : FunctionMethod
	{
		private bool warn;

		public MesSkipMethod(bool warn)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = false;
			this.warn = warn;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length != 0)
			{
				return name + "関数の引数が多すぎます";
			}
			if (warn)
			{
				ParserMediator.Warn("関数MOUSESKIP()は推奨されません。代わりに関数MESSKIP()を使用してください", GlobalStatic.Process.GetScaningLine(), 1, isError: false, isBackComp: false, null);
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (!GlobalStatic.Console.MesSkip)
			{
				return 0L;
			}
			return 1L;
		}
	}

	private sealed class GetColorMethod : FunctionMethod
	{
		private bool defaultColor;

		public GetColorMethod(bool isDef)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = isDef;
			defaultColor = isDef;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return (defaultColor ? Config.ForeColor : GlobalStatic.Console.StringStyle.Color).ToArgb() & 0xFFFFFF;
		}
	}

	private sealed class GetFocusColorMethod : FunctionMethod
	{
		public GetFocusColorMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return Config.FocusColor.ToArgb() & 0xFFFFFF;
		}
	}

	private sealed class GetBGColorMethod : FunctionMethod
	{
		private bool defaultColor;

		public GetBGColorMethod(bool isDef)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = isDef;
			defaultColor = isDef;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return (defaultColor ? Config.BackColor : GlobalStatic.Console.bgColor).ToArgb() & 0xFFFFFF;
		}
	}

	private sealed class GetStyleMethod : FunctionMethod
	{
		public GetStyleMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = false;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return 0L;
		}
	}

	private sealed class GetFontMethod : FunctionMethod
	{
		public GetFontMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[0];
			base.CanRestructure = false;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return GlobalStatic.Console.StringStyle.Fontname;
		}
	}

	private sealed class BarStringMethod : FunctionMethod
	{
		public BarStringMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[3]
			{
				typeof(long),
				typeof(long),
				typeof(long)
			};
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			long intValue2 = arguments[1].GetIntValue(exm);
			long intValue3 = arguments[2].GetIntValue(exm);
			return exm.CreateBar(intValue, intValue2, intValue3);
		}
	}

	private sealed class CurrentAlignMethod : FunctionMethod
	{
		public CurrentAlignMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[0];
			base.CanRestructure = false;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (exm.Console.Alignment == DisplayLineAlignment.LEFT)
			{
				return "LEFT";
			}
			if (exm.Console.Alignment == DisplayLineAlignment.CENTER)
			{
				return "CENTER";
			}
			return "RIGHT";
		}
	}

	private sealed class CurrentRedrawMethod : FunctionMethod
	{
		public CurrentRedrawMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = false;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (exm.Console.Redraw != ConsoleRedraw.None)
			{
				return 1L;
			}
			return 0L;
		}
	}

	private sealed class ColorFromNameMethod : FunctionMethod
	{
		public ColorFromNameMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			Color color = Color.FromName(strValue);
			int num = 0;
			if (color.A > 0)
			{
				num = (color.R << 16) + (color.G << 8) + color.B;
			}
			else
			{
				if (strValue.Equals("transparent", StringComparison.OrdinalIgnoreCase))
				{
					throw new CodeEE("無色透明(Transparent)は色として指定できません");
				}
				num = -1;
			}
			return num;
		}
	}

	private sealed class ColorFromRGBMethod : FunctionMethod
	{
		public ColorFromRGBMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[3]
			{
				typeof(long),
				typeof(long),
				typeof(long)
			};
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			if (intValue < 0 || intValue > 255)
			{
				throw new CodeEE("第１引数が0から255の範囲外です");
			}
			long intValue2 = arguments[1].GetIntValue(exm);
			if (intValue2 < 0 || intValue2 > 255)
			{
				throw new CodeEE("第２引数が0から255の範囲外です");
			}
			long intValue3 = arguments[2].GetIntValue(exm);
			if (intValue3 < 0 || intValue3 > 255)
			{
				throw new CodeEE("第３引数が0から255の範囲外です");
			}
			return (intValue << 16) + (intValue2 << 8) + intValue3;
		}
	}

	private sealed class GetRefMethod : FunctionMethod
	{
		public GetRefMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = null;
			base.CanRestructure = false;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 1)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!(arguments[0] is UserDefinedRefMethodNoArgTerm))
			{
				return name + "関数の1番目の引数が関数参照ではありません";
			}
			return null;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return ((UserDefinedRefMethodNoArgTerm)arguments[0]).GetRefName();
		}
	}

	private sealed class MoneyStrMethod : FunctionMethod
	{
		public MoneyStrMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = null;
			base.CanRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 2)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (arguments[0].GetOperandType() != typeof(long))
			{
				return name + "関数の1番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 2 && arguments[1] != null && arguments[1].GetOperandType() != typeof(string))
			{
				return name + "関数の2番目の引数の型が正しくありません";
			}
			return null;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			if (arguments.Length < 2 || arguments[1] == null)
			{
				if (!Config.MoneyFirst)
				{
					return intValue + Config.MoneyLabel;
				}
				return Config.MoneyLabel + intValue;
			}
			string strValue = arguments[1].GetStrValue(exm);
			string text;
			try
			{
				text = intValue.ToString(strValue);
			}
			catch (FormatException)
			{
				throw new CodeEE("MONEYSTR関数の第2引数の書式指定が間違っています");
			}
			if (!Config.MoneyFirst)
			{
				return text + Config.MoneyLabel;
			}
			return Config.MoneyLabel + text;
		}
	}

	private sealed class GetPrintCPerLineMethod : FunctionMethod
	{
		public GetPrintCPerLineMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return Config.PrintCPerLine;
		}
	}

	private sealed class PrintCLengthMethod : FunctionMethod
	{
		public PrintCLengthMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return Config.PrintCLength;
		}
	}

	private sealed class GetSaveNosMethod : FunctionMethod
	{
		public GetSaveNosMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return Config.SaveDataNos;
		}
	}

	private sealed class GettimeMethod : FunctionMethod
	{
		public GettimeMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = false;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return ((((((long)DateTime.Now.Year * 100L + DateTime.Now.Month) * 100 + DateTime.Now.Day) * 100 + DateTime.Now.Hour) * 100 + DateTime.Now.Minute) * 100 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond;
		}
	}

	private sealed class GettimesMethod : FunctionMethod
	{
		public GettimesMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[0];
			base.CanRestructure = false;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
		}
	}

	private sealed class GetmsMethod : FunctionMethod
	{
		public GetmsMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = false;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return DateTime.Now.Ticks / 10000;
		}
	}

	private sealed class GetSecondMethod : FunctionMethod
	{
		public GetSecondMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = false;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return DateTime.Now.Ticks / 10000000;
		}
	}

	private sealed class RandMethod : FunctionMethod
	{
		public RandMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = false;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 2)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments.Length == 1)
			{
				if (arguments[0] == null)
				{
					return name + "関数には少なくとも1つの引数が必要です";
				}
				if (arguments[0].GetOperandType() != typeof(long))
				{
					return name + "関数の1番目の引数の型が正しくありません";
				}
				return null;
			}
			if (arguments[0] != null && arguments[0].GetOperandType() != typeof(long))
			{
				return name + "関数の1番目の引数の型が正しくありません";
			}
			if (arguments[1] != null && arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の引数の型が正しくありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long num = 0L;
			long num2 = 0L;
			if (arguments.Length == 1)
			{
				num = arguments[0].GetIntValue(exm);
			}
			else
			{
				if (arguments[0] != null)
				{
					num2 = arguments[0].GetIntValue(exm);
				}
				num = arguments[1].GetIntValue(exm);
			}
			if (num <= num2)
			{
				if (num2 == 0L)
				{
					throw new CodeEE("RANDの最大値に0以下の値(" + num + ")が指定されました");
				}
				throw new CodeEE("RANDの最大値に最小値以下の値(" + num + ")が指定されました");
			}
			return exm.VEvaluator.GetNextRand(num - num2) + num2;
		}
	}

	private sealed class MaxMethod : FunctionMethod
	{
		private bool isMax;

		public MaxMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			isMax = true;
			base.CanRestructure = true;
		}

		public MaxMethod(bool max)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			isMax = max;
			base.CanRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			for (int i = 0; i < arguments.Length; i++)
			{
				if (arguments[i] == null)
				{
					return name + "関数の" + (i + 1) + "番目の引数は省略できません";
				}
				if (arguments[i].GetOperandType() != typeof(long))
				{
					return name + "関数の" + (i + 1) + "番目の引数の型が正しくありません";
				}
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long num = arguments[0].GetIntValue(exm);
			for (int i = 1; i < arguments.Length; i++)
			{
				long intValue = arguments[i].GetIntValue(exm);
				if (isMax)
				{
					if (num < intValue)
					{
						num = intValue;
					}
				}
				else if (num > intValue)
				{
					num = intValue;
				}
			}
			return num;
		}
	}

	private sealed class AbsMethod : FunctionMethod
	{
		public AbsMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(long) };
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return Math.Abs(arguments[0].GetIntValue(exm));
		}
	}

	private sealed class PowerMethod : FunctionMethod
	{
		public PowerMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[2]
			{
				typeof(long),
				typeof(long)
			};
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			long intValue2 = arguments[1].GetIntValue(exm);
			double num = Math.Pow(intValue, intValue2);
			if (double.IsNaN(num))
			{
				throw new CodeEE("累乗結果が非数値です");
			}
			if (double.IsInfinity(num))
			{
				throw new CodeEE("累乗結果が無限大です");
			}
			if (num >= 9.223372036854776E+18 || num <= -9.223372036854776E+18)
			{
				throw new CodeEE("累乗結果(" + num + ")が64ビット符号付き整数の範囲外です");
			}
			return (long)num;
		}
	}

	private sealed class SqrtMethod : FunctionMethod
	{
		public SqrtMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(long) };
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			if (intValue < 0)
			{
				throw new CodeEE("SQRT関数の引数に負の値が指定されました");
			}
			return (long)Math.Sqrt(intValue);
		}
	}

	private sealed class CbrtMethod : FunctionMethod
	{
		public CbrtMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(long) };
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			if (intValue < 0)
			{
				throw new CodeEE("CBRT関数の引数に負の値が指定されました");
			}
			return (long)Math.Pow(intValue, 1.0 / 3.0);
		}
	}

	private sealed class LogMethod : FunctionMethod
	{
		private double Base;

		public LogMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(long) };
			Base = Math.E;
			base.CanRestructure = true;
		}

		public LogMethod(double b)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(long) };
			Base = b;
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			if (intValue <= 0)
			{
				throw new CodeEE("対数関数の引数に0以下の値が指定されました");
			}
			if (Base <= 0.0)
			{
				throw new CodeEE("対数関数の底に0以下の値が指定されました");
			}
			double d = intValue;
			d = ((Base != Math.E) ? Math.Log10(d) : Math.Log(d));
			if (double.IsNaN(d))
			{
				throw new CodeEE("計算値が非数値です");
			}
			if (double.IsInfinity(d))
			{
				throw new CodeEE("計算値が無限大です");
			}
			if (d >= 9.223372036854776E+18 || d <= -9.223372036854776E+18)
			{
				throw new CodeEE("計算結果(" + d + ")が64ビット符号付き整数の範囲外です");
			}
			return (long)d;
		}
	}

	private sealed class ExpMethod : FunctionMethod
	{
		public ExpMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(long) };
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			double num = Math.Exp(arguments[0].GetIntValue(exm));
			if (double.IsNaN(num))
			{
				throw new CodeEE("計算値が非数値です");
			}
			if (double.IsInfinity(num))
			{
				throw new CodeEE("計算値が無限大です");
			}
			if (num >= 9.223372036854776E+18 || num <= -9.223372036854776E+18)
			{
				throw new CodeEE("計算結果(" + num + ")が64ビット符号付き整数の範囲外です");
			}
			return (long)num;
		}
	}

	private sealed class SignMethod : FunctionMethod
	{
		public SignMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(long) };
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return Math.Sign(arguments[0].GetIntValue(exm));
		}
	}

	private sealed class GetLimitMethod : FunctionMethod
	{
		public GetLimitMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[3]
			{
				typeof(long),
				typeof(long),
				typeof(long)
			};
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			long intValue2 = arguments[1].GetIntValue(exm);
			long intValue3 = arguments[2].GetIntValue(exm);
			long num = 0L;
			if (intValue < intValue2)
			{
				return intValue2;
			}
			if (intValue > intValue3)
			{
				return intValue3;
			}
			return intValue;
		}
	}

	private sealed class SumArrayMethod : FunctionMethod
	{
		private bool isCharaRange;

		public SumArrayMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			isCharaRange = false;
			base.CanRestructure = false;
		}

		public SumArrayMethod(bool isChara)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			isCharaRange = isChara;
			base.CanRestructure = false;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 3)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!(arguments[0] is VariableTerm))
			{
				return name + "関数の1番目の引数が変数ではありません";
			}
			VariableTerm variableTerm = (VariableTerm)arguments[0];
			if (variableTerm.IsString)
			{
				return name + "関数の1番目の引数が数値変数ではありません";
			}
			if (isCharaRange && !variableTerm.Identifier.IsCharacterData)
			{
				return name + "関数の1番目の引数がキャラクタ変数ではありません";
			}
			if (!isCharaRange && !variableTerm.Identifier.IsArray1D && !variableTerm.Identifier.IsArray2D && !variableTerm.Identifier.IsArray3D)
			{
				return name + "関数の1番目の引数が配列変数ではありません";
			}
			if (arguments.Length == 1)
			{
				return null;
			}
			if (arguments[1] != null && arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の変数が数値ではありません";
			}
			if (arguments.Length == 2)
			{
				return null;
			}
			if (arguments[2] != null && arguments[2].GetOperandType() != typeof(long))
			{
				return name + "関数の3番目の変数が数値ではありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			VariableTerm variableTerm = (VariableTerm)arguments[0];
			long num = ((arguments.Length >= 2 && arguments[1] != null) ? arguments[1].GetIntValue(exm) : 0);
			long num2 = ((arguments.Length == 3 && arguments[2] != null) ? arguments[2].GetIntValue(exm) : (isCharaRange ? exm.VEvaluator.CHARANUM : variableTerm.GetLastLength()));
			FixedVariableTerm fixedVariableTerm = variableTerm.GetFixedVariableTerm(exm);
			if (!isCharaRange)
			{
				fixedVariableTerm.IsArrayRangeValid(num, num2, "SUMARRAY", 2L, 3L);
				return exm.VEvaluator.GetArraySum(fixedVariableTerm, num, num2);
			}
			long cHARANUM = exm.VEvaluator.CHARANUM;
			if (num >= cHARANUM || num < 0 || num2 > cHARANUM || num2 < 0)
			{
				throw new CodeEE("SUMCARRAY関数の範囲指定がキャラクタ配列の範囲を超えています(" + num + "～" + num2 + ")");
			}
			return exm.VEvaluator.GetArraySumChara(fixedVariableTerm, num, num2);
		}
	}

	private sealed class MatchMethod : FunctionMethod
	{
		private bool isCharaRange;

		public MatchMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			isCharaRange = false;
			base.CanRestructure = false;
			base.HasUniqueRestructure = true;
		}

		public MatchMethod(bool isChara)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			isCharaRange = isChara;
			base.CanRestructure = false;
			base.HasUniqueRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 2)
			{
				return name + "関数には少なくとも2つの引数が必要です";
			}
			if (arguments.Length > 4)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!(arguments[0] is VariableTerm))
			{
				return name + "関数の1番目の引数が変数ではありません";
			}
			VariableTerm variableTerm = (VariableTerm)arguments[0];
			if (isCharaRange && !variableTerm.Identifier.IsCharacterData)
			{
				return name + "関数の1番目の引数がキャラクタ変数ではありません";
			}
			if (!isCharaRange && (variableTerm.Identifier.IsArray2D || variableTerm.Identifier.IsArray3D))
			{
				return name + "関数は二重配列・三重配列には対応していません";
			}
			if (!isCharaRange && !variableTerm.Identifier.IsArray1D)
			{
				return name + "関数の1番目の引数が配列変数ではありません";
			}
			if (arguments[1] == null)
			{
				return name + "関数の2番目の引数は省略できません";
			}
			if (arguments[1].GetOperandType() != arguments[0].GetOperandType())
			{
				return name + "関数の1番目の引数と2番目の引数の型が異なります";
			}
			if (arguments.Length >= 3 && arguments[2] != null && arguments[2].GetOperandType() != typeof(long))
			{
				return name + "関数の3番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 4 && arguments[3] != null && arguments[3].GetOperandType() != typeof(long))
			{
				return name + "関数の4番目の引数の型が正しくありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			VariableTerm variableTerm = arguments[0] as VariableTerm;
			long num = ((arguments.Length > 2 && arguments[2] != null) ? arguments[2].GetIntValue(exm) : 0);
			long num2 = ((arguments.Length > 3 && arguments[3] != null) ? arguments[3].GetIntValue(exm) : (isCharaRange ? exm.VEvaluator.CHARANUM : variableTerm.GetLength()));
			FixedVariableTerm fixedVariableTerm = variableTerm.GetFixedVariableTerm(exm);
			if (!isCharaRange)
			{
				fixedVariableTerm.IsArrayRangeValid(num, num2, "MATCH", 3L, 4L);
				if (arguments[0].GetOperandType() == typeof(long))
				{
					long intValue = arguments[1].GetIntValue(exm);
					return exm.VEvaluator.GetMatch(fixedVariableTerm, intValue, num, num2);
				}
				string strValue = arguments[1].GetStrValue(exm);
				return exm.VEvaluator.GetMatch(fixedVariableTerm, strValue, num, num2);
			}
			long cHARANUM = exm.VEvaluator.CHARANUM;
			if (num >= cHARANUM || num < 0 || num2 > cHARANUM || num2 < 0)
			{
				throw new CodeEE("CMATCH関数の範囲指定がキャラクタ配列の範囲を超えています(" + num + "～" + num2 + ")");
			}
			if (arguments[0].GetOperandType() == typeof(long))
			{
				long intValue2 = arguments[1].GetIntValue(exm);
				return exm.VEvaluator.GetMatchChara(fixedVariableTerm, intValue2, num, num2);
			}
			string strValue2 = arguments[1].GetStrValue(exm);
			return exm.VEvaluator.GetMatchChara(fixedVariableTerm, strValue2, num, num2);
		}

		public override bool UniqueRestructure(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			arguments[0].Restructure(exm);
			for (int i = 1; i < arguments.Length; i++)
			{
				if (arguments[i] != null)
				{
					arguments[i] = arguments[i].Restructure(exm);
				}
			}
			return false;
		}
	}

	private sealed class GroupMatchMethod : FunctionMethod
	{
		public GroupMatchMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = false;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 2)
			{
				return name + "関数には少なくとも2つの引数が必要です";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			Type operandType = arguments[0].GetOperandType();
			for (int i = 1; i < arguments.Length; i++)
			{
				if (arguments[i] == null)
				{
					return name + "関数の" + (i + 1) + "番目の引数は省略できません";
				}
				if (arguments[i].GetOperandType() != operandType)
				{
					return name + "関数の" + (i + 1) + "番目の引数の型が正しくありません";
				}
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long num = 0L;
			if (arguments[0].GetOperandType() == typeof(long))
			{
				long intValue = arguments[0].GetIntValue(exm);
				for (int i = 1; i < arguments.Length; i++)
				{
					if (intValue == arguments[i].GetIntValue(exm))
					{
						num++;
					}
				}
			}
			else
			{
				string strValue = arguments[0].GetStrValue(exm);
				for (int j = 1; j < arguments.Length; j++)
				{
					if (strValue == arguments[j].GetStrValue(exm))
					{
						num++;
					}
				}
			}
			return num;
		}
	}

	private sealed class NosamesMethod : FunctionMethod
	{
		public NosamesMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = false;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 2)
			{
				return name + "関数には少なくとも2つの引数が必要です";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			Type operandType = arguments[0].GetOperandType();
			for (int i = 1; i < arguments.Length; i++)
			{
				if (arguments[i] == null)
				{
					return name + "関数の" + (i + 1) + "番目の引数は省略できません";
				}
				if (arguments[i].GetOperandType() != operandType)
				{
					return name + "関数の" + (i + 1) + "番目の引数の型が正しくありません";
				}
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetOperandType() == typeof(long))
			{
				long intValue = arguments[0].GetIntValue(exm);
				for (int i = 1; i < arguments.Length; i++)
				{
					if (intValue == arguments[i].GetIntValue(exm))
					{
						return 0L;
					}
				}
			}
			else
			{
				string strValue = arguments[0].GetStrValue(exm);
				for (int j = 1; j < arguments.Length; j++)
				{
					if (strValue == arguments[j].GetStrValue(exm))
					{
						return 0L;
					}
				}
			}
			return 1L;
		}
	}

	private sealed class AllsamesMethod : FunctionMethod
	{
		public AllsamesMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = false;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 2)
			{
				return name + "関数には少なくとも2つの引数が必要です";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			Type operandType = arguments[0].GetOperandType();
			for (int i = 1; i < arguments.Length; i++)
			{
				if (arguments[i] == null)
				{
					return name + "関数の" + (i + 1) + "番目の引数は省略できません";
				}
				if (arguments[i].GetOperandType() != operandType)
				{
					return name + "関数の" + (i + 1) + "番目の引数の型が正しくありません";
				}
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (arguments[0].GetOperandType() == typeof(long))
			{
				long intValue = arguments[0].GetIntValue(exm);
				for (int i = 1; i < arguments.Length; i++)
				{
					if (intValue != arguments[i].GetIntValue(exm))
					{
						return 0L;
					}
				}
			}
			else
			{
				string strValue = arguments[0].GetStrValue(exm);
				for (int j = 1; j < arguments.Length; j++)
				{
					if (strValue != arguments[j].GetStrValue(exm))
					{
						return 0L;
					}
				}
			}
			return 1L;
		}
	}

	private sealed class MaxArrayMethod : FunctionMethod
	{
		private bool isCharaRange;

		private bool isMax;

		private string funcName;

		public MaxArrayMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			isCharaRange = false;
			isMax = true;
			funcName = "MAXARRAY";
			base.CanRestructure = false;
		}

		public MaxArrayMethod(bool isChara)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			isCharaRange = isChara;
			isMax = true;
			if (isCharaRange)
			{
				funcName = "MAXCARRAY";
			}
			else
			{
				funcName = "MAXARRAY";
			}
			base.CanRestructure = false;
		}

		public MaxArrayMethod(bool isChara, bool isMaxFunc)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			isCharaRange = isChara;
			isMax = isMaxFunc;
			funcName = (isMax ? "MAX" : "MIN") + (isCharaRange ? "C" : "") + "ARRAY";
			base.CanRestructure = false;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 3)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!(arguments[0] is VariableTerm))
			{
				return name + "関数の1番目の引数が変数ではありません";
			}
			VariableTerm variableTerm = (VariableTerm)arguments[0];
			if (isCharaRange && !variableTerm.Identifier.IsCharacterData)
			{
				return name + "関数の1番目の引数がキャラクタ変数ではありません";
			}
			if (!variableTerm.IsInteger)
			{
				return name + "関数の1番目の引数が数値変数ではありません";
			}
			if (!isCharaRange && (variableTerm.Identifier.IsArray2D || variableTerm.Identifier.IsArray3D))
			{
				return name + "関数は二重配列・三重配列には対応していません";
			}
			if (!variableTerm.Identifier.IsArray1D)
			{
				return name + "関数の1番目の引数が配列変数ではありません";
			}
			if (arguments.Length >= 2 && arguments[1] != null && arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 3 && arguments[2] != null && arguments[2].GetOperandType() != typeof(long))
			{
				return name + "関数の3番目の引数の型が正しくありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			VariableTerm variableTerm = (VariableTerm)arguments[0];
			long num = ((arguments.Length > 1 && arguments[1] != null) ? arguments[1].GetIntValue(exm) : 0);
			long num2 = ((arguments.Length <= 2 || arguments[2] == null) ? (isCharaRange ? exm.VEvaluator.CHARANUM : variableTerm.GetLength()) : (num2 = arguments[2].GetIntValue(exm)));
			FixedVariableTerm fixedVariableTerm = variableTerm.GetFixedVariableTerm(exm);
			if (!isCharaRange)
			{
				fixedVariableTerm.IsArrayRangeValid(num, num2, funcName, 2L, 3L);
				return exm.VEvaluator.GetMaxArray(fixedVariableTerm, num, num2, isMax);
			}
			long cHARANUM = exm.VEvaluator.CHARANUM;
			if (num >= cHARANUM || num < 0 || num2 > cHARANUM || num2 < 0)
			{
				throw new CodeEE(funcName + "関数の範囲指定がキャラクタ配列の範囲を超えています(" + num + "～" + num2 + ")");
			}
			return exm.VEvaluator.GetMaxArrayChara(fixedVariableTerm, num, num2, isMax);
		}
	}

	private sealed class GetbitMethod : FunctionMethod
	{
		public GetbitMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[2]
			{
				typeof(long),
				typeof(long)
			};
			base.CanRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			string text = base.CheckArgumentType(name, arguments);
			if (text != null)
			{
				return text;
			}
			if (arguments[1] is SingleTerm)
			{
				long num = ((SingleTerm)arguments[1]).Int;
				if (num < 0 || num > 63)
				{
					return "GETBIT関数の第２引数(" + num + ")が範囲(０～６３)を超えています";
				}
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			long intValue2 = arguments[1].GetIntValue(exm);
			if (intValue2 < 0 || intValue2 > 63)
			{
				throw new CodeEE("GETBIT関数の第２引数(" + intValue2 + ")が範囲(０～６３)を超えています");
			}
			int num = (int)intValue2;
			return (intValue >> num) & 1;
		}
	}

	private sealed class GetnumMethod : FunctionMethod
	{
		public GetnumMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = true;
			base.HasUniqueRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length != 2)
			{
				return name + "関数には2つの引数が必要です";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!(arguments[0] is VariableTerm))
			{
				return name + "関数の1番目の引数の型が正しくありません";
			}
			if (arguments[1] == null)
			{
				return name + "関数の2番目の引数は省略できません";
			}
			if (arguments[1].GetOperandType() != typeof(string))
			{
				return name + "関数の2番目の引数の型が正しくありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			VariableCode code = ((VariableTerm)arguments[0]).Identifier.Code;
			string strValue = arguments[1].GetStrValue(exm);
			int ret = 0;
			if (exm.VEvaluator.Constant.TryKeywordToInteger(out ret, code, strValue, -1))
			{
				return ret;
			}
			return -1L;
		}

		public override bool UniqueRestructure(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			arguments[1] = arguments[1].Restructure(exm);
			return arguments[1] is SingleTerm;
		}
	}

	private sealed class GetnumBMethod : FunctionMethod
	{
		public GetnumBMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[2]
			{
				typeof(string),
				typeof(string)
			};
			base.CanRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			string text = base.CheckArgumentType(name, arguments);
			if (text != null)
			{
				return text;
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (arguments[0] is SingleTerm)
			{
				string str = ((SingleTerm)arguments[0]).Str;
				if (GlobalStatic.IdentifierDictionary.GetVariableToken(str, null, allowPrivate: true) == null)
				{
					return name + "関数の1番目の引数が変数名ではありません";
				}
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			VariableToken variableToken = GlobalStatic.IdentifierDictionary.GetVariableToken(arguments[0].GetStrValue(exm), null, allowPrivate: true);
			if (variableToken == null)
			{
				throw new CodeEE("GETNUMBの1番目の引数(\"" + arguments[0].GetStrValue(exm) + "\")が変数名ではありません");
			}
			string strValue = arguments[1].GetStrValue(exm);
			int ret = 0;
			if (exm.VEvaluator.Constant.TryKeywordToInteger(out ret, variableToken.Code, strValue, -1))
			{
				return ret;
			}
			return -1L;
		}
	}

	private sealed class GetPalamLVMethod : FunctionMethod
	{
		public GetPalamLVMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[2]
			{
				typeof(long),
				typeof(long)
			};
			base.CanRestructure = false;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			string text = base.CheckArgumentType(name, arguments);
			if (text != null)
			{
				return text;
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			long intValue2 = arguments[1].GetIntValue(exm);
			return exm.VEvaluator.getPalamLv(intValue, intValue2);
		}
	}

	private sealed class GetExpLVMethod : FunctionMethod
	{
		public GetExpLVMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[2]
			{
				typeof(long),
				typeof(long)
			};
			base.CanRestructure = false;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			string text = base.CheckArgumentType(name, arguments);
			if (text != null)
			{
				return text;
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			long intValue2 = arguments[1].GetIntValue(exm);
			return exm.VEvaluator.getExpLv(intValue, intValue2);
		}
	}

	private sealed class FindElementMethod : FunctionMethod
	{
		private bool isLast;

		private string funcName;

		public FindElementMethod(bool last)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = true;
			base.HasUniqueRestructure = true;
			isLast = last;
			funcName = (isLast ? "FINDLASTELEMENT" : "FINDELEMENT");
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 2)
			{
				return name + "関数には少なくとも2つの引数が必要です";
			}
			if (arguments.Length > 5)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!(arguments[0] is VariableTerm variableTerm))
			{
				return name + "関数の1番目の引数が変数ではありません";
			}
			if (variableTerm.Identifier.IsArray2D || variableTerm.Identifier.IsArray3D)
			{
				return name + "関数は二重配列・三重配列には対応していません";
			}
			if (!variableTerm.Identifier.IsArray1D)
			{
				return name + "関数の1番目の引数が配列変数ではありません";
			}
			Type operandType = arguments[0].GetOperandType();
			if (arguments[1] == null)
			{
				return name + "関数の2番目の引数は省略できません";
			}
			if (arguments[1].GetOperandType() != operandType)
			{
				return name + "関数の2番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 3 && arguments[2] != null && arguments[2].GetOperandType() != typeof(long))
			{
				return name + "関数の3番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 4 && arguments[3] != null && arguments[3].GetOperandType() != typeof(long))
			{
				return name + "関数の4番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 5 && arguments[4] != null && arguments[4].GetOperandType() != typeof(long))
			{
				return name + "関数の5番目の引数の型が正しくありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			bool isExact = false;
			VariableTerm variableTerm = (VariableTerm)arguments[0];
			long num = ((arguments.Length > 2 && arguments[2] != null) ? arguments[2].GetIntValue(exm) : 0);
			long num2 = ((arguments.Length > 3 && arguments[3] != null) ? arguments[3].GetIntValue(exm) : variableTerm.GetLength());
			if (arguments.Length > 4 && arguments[4] != null)
			{
				isExact = arguments[4].GetIntValue(exm) != 0;
			}
			FixedVariableTerm fixedVariableTerm = variableTerm.GetFixedVariableTerm(exm);
			fixedVariableTerm.IsArrayRangeValid(num, num2, funcName, 3L, 4L);
			if (arguments[0].GetOperandType() == typeof(long))
			{
				long intValue = arguments[1].GetIntValue(exm);
				return exm.VEvaluator.FindElement(fixedVariableTerm, intValue, num, num2, isExact, isLast);
			}
			Regex regex = null;
			try
			{
				regex = new Regex(arguments[1].GetStrValue(exm));
			}
			catch (ArgumentException)
			{
				throw new CodeEE("第2引数が正規表現として不正です");
			}
			return exm.VEvaluator.FindElement(fixedVariableTerm, regex, num, num2, isExact, isLast);
		}

		public override bool UniqueRestructure(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			bool flag = true;
			arguments[0].Restructure(exm);
			flag = (arguments[0] as VariableTerm).Identifier.IsConst;
			for (int i = 1; i < arguments.Length; i++)
			{
				if (arguments[i] != null)
				{
					arguments[i] = arguments[i].Restructure(exm);
					if (flag && !(arguments[i] is SingleTerm))
					{
						flag = false;
					}
				}
			}
			return flag;
		}
	}

	private sealed class InRangeMethod : FunctionMethod
	{
		public InRangeMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[3]
			{
				typeof(long),
				typeof(long),
				typeof(long)
			};
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			long intValue2 = arguments[1].GetIntValue(exm);
			long intValue3 = arguments[2].GetIntValue(exm);
			if (intValue < intValue2 || intValue > intValue3)
			{
				return 0L;
			}
			return 1L;
		}
	}

	private sealed class InRangeArrayMethod : FunctionMethod
	{
		private bool isCharaRange;

		public InRangeArrayMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = false;
		}

		public InRangeArrayMethod(bool isChara)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			isCharaRange = isChara;
			base.CanRestructure = false;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 2)
			{
				return name + "関数には少なくとも2つの引数が必要です";
			}
			if (arguments.Length > 6)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (!(arguments[0] is VariableTerm))
			{
				return name + "関数の1番目の引数が変数ではありません";
			}
			VariableTerm variableTerm = (VariableTerm)arguments[0];
			if (isCharaRange && !variableTerm.Identifier.IsCharacterData)
			{
				return name + "関数の1番目の引数がキャラクタ変数ではありません";
			}
			if (!isCharaRange && (variableTerm.Identifier.IsArray2D || variableTerm.Identifier.IsArray3D))
			{
				return name + "関数は二重配列・三重配列には対応していません";
			}
			if (!isCharaRange && !variableTerm.Identifier.IsArray1D)
			{
				return name + "関数の1番目の引数が配列変数ではありません";
			}
			if (!variableTerm.IsInteger)
			{
				return name + "関数の1番目の引数が数値型変数ではありません";
			}
			if (arguments[1] == null)
			{
				return name + "関数の2番目の引数は省略できません";
			}
			if (arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の引数が数値型ではありません";
			}
			if (arguments[2] == null)
			{
				return name + "関数の3番目の引数は省略できません";
			}
			if (arguments[2].GetOperandType() != typeof(long))
			{
				return name + "関数の3番目の引数が数値型ではありません";
			}
			if (arguments.Length >= 4 && arguments[3] != null && arguments[3].GetOperandType() != typeof(long))
			{
				return name + "関数の4番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 5 && arguments[4] != null && arguments[4].GetOperandType() != typeof(long))
			{
				return name + "関数の5番目の引数の型が正しくありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[1].GetIntValue(exm);
			long intValue2 = arguments[2].GetIntValue(exm);
			VariableTerm variableTerm = arguments[0] as VariableTerm;
			long num = ((arguments.Length > 3 && arguments[3] != null) ? arguments[3].GetIntValue(exm) : 0);
			long num2 = ((arguments.Length > 4 && arguments[4] != null) ? arguments[4].GetIntValue(exm) : (isCharaRange ? exm.VEvaluator.CHARANUM : variableTerm.GetLength()));
			FixedVariableTerm fixedVariableTerm = variableTerm.GetFixedVariableTerm(exm);
			if (!isCharaRange)
			{
				fixedVariableTerm.IsArrayRangeValid(num, num2, "INRANGEARRAY", 4L, 5L);
				return exm.VEvaluator.GetInRangeArray(fixedVariableTerm, intValue, intValue2, num, num2);
			}
			long cHARANUM = exm.VEvaluator.CHARANUM;
			if (num >= cHARANUM || num < 0 || num2 > cHARANUM || num2 < 0)
			{
				throw new CodeEE("INRANGECARRAY関数の範囲指定がキャラクタ配列の範囲を超えています(" + num + "～" + num2 + ")");
			}
			return exm.VEvaluator.GetInRangeArrayChara(fixedVariableTerm, intValue, intValue2, num, num2);
		}
	}

	private sealed class StrlenMethod : FunctionMethod
	{
		public StrlenMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return LangManager.GetStrlenLang(arguments[0].GetStrValue(exm));
		}
	}

	private sealed class StrlenuMethod : FunctionMethod
	{
		public StrlenuMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return arguments[0].GetStrValue(exm).Length;
		}
	}

	private sealed class SubstringMethod : FunctionMethod
	{
		public SubstringMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = null;
			base.CanRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 3)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (arguments[0].GetOperandType() != typeof(string))
			{
				return name + "関数の1番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 2 && arguments[1] != null && arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 3 && arguments[2] != null && arguments[2].GetOperandType() != typeof(long))
			{
				return name + "関数の3番目の引数の型が正しくありません";
			}
			return null;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			int startindex = 0;
			int length = -1;
			if (arguments.Length >= 2 && arguments[1] != null)
			{
				startindex = (int)arguments[1].GetIntValue(exm);
			}
			if (arguments.Length >= 3 && arguments[2] != null)
			{
				length = (int)arguments[2].GetIntValue(exm);
			}
			return LangManager.GetSubStringLang(strValue, startindex, length);
		}
	}

	private sealed class SubstringuMethod : FunctionMethod
	{
		public SubstringuMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = null;
			base.CanRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 3)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (arguments[0].GetOperandType() != typeof(string))
			{
				return name + "関数の1番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 2 && arguments[1] != null && arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 3 && arguments[2] != null && arguments[2].GetOperandType() != typeof(long))
			{
				return name + "関数の3番目の引数の型が正しくありません";
			}
			return null;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			int num = 0;
			int num2 = -1;
			if (arguments.Length >= 2 && arguments[1] != null)
			{
				num = (int)arguments[1].GetIntValue(exm);
			}
			if (arguments.Length >= 3 && arguments[2] != null)
			{
				num2 = (int)arguments[2].GetIntValue(exm);
			}
			if (num >= strValue.Length || num2 == 0)
			{
				return "";
			}
			if (num2 < 0 || num2 > strValue.Length)
			{
				num2 = strValue.Length;
			}
			if (num <= 0)
			{
				if (num2 == strValue.Length)
				{
					return strValue;
				}
				num = 0;
			}
			if (num + num2 > strValue.Length)
			{
				num2 = strValue.Length - num;
			}
			return strValue.Substring(num, num2);
		}
	}

	private sealed class StrfindMethod : FunctionMethod
	{
		private bool unicode;

		public StrfindMethod(bool unicode)
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = null;
			base.CanRestructure = true;
			this.unicode = unicode;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 2)
			{
				return name + "関数には少なくとも2つの引数が必要です";
			}
			if (arguments.Length > 3)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (arguments[0].GetOperandType() != typeof(string))
			{
				return name + "関数の1番目の引数の型が正しくありません";
			}
			if (arguments[1] == null)
			{
				return name + "関数の2番目の引数は省略できません";
			}
			if (arguments[1].GetOperandType() != typeof(string))
			{
				return name + "関数の2番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 3 && arguments[2] != null && arguments[2].GetOperandType() != typeof(long))
			{
				return name + "関数の3番目の引数の型が正しくありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			string strValue2 = arguments[1].GetStrValue(exm);
			int num = 0;
			int num2 = 0;
			if (arguments.Length >= 3 && arguments[2] != null)
			{
				if (unicode)
				{
					num2 = (int)arguments[2].GetIntValue(exm);
				}
				else
				{
					num = (int)arguments[2].GetIntValue(exm);
					num2 = LangManager.GetUFTIndex(strValue, num);
				}
			}
			if (num2 < 0 || num2 >= strValue.Length)
			{
				return -1L;
			}
			int num3 = strValue.IndexOf(strValue2, num2);
			if (num3 > 0 && !unicode)
			{
				num3 = LangManager.GetStrlenLang(strValue.Substring(0, num3));
			}
			return num3;
		}
	}

	private sealed class StrCountMethod : FunctionMethod
	{
		public StrCountMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[2]
			{
				typeof(string),
				typeof(string)
			};
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			Regex regex = null;
			try
			{
				regex = new Regex(arguments[1].GetStrValue(exm));
			}
			catch (ArgumentException ex)
			{
				throw new CodeEE("第2引数が正規表現として不正です：" + ex.Message);
			}
			return regex.Matches(arguments[0].GetStrValue(exm)).Count;
		}
	}

	private sealed class ToStrMethod : FunctionMethod
	{
		public ToStrMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = null;
			base.CanRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 2)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (arguments[0].GetOperandType() != typeof(long))
			{
				return name + "関数の1番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 2 && arguments[1] != null && arguments[1].GetOperandType() != typeof(string))
			{
				return name + "関数の2番目の引数の型が正しくありません";
			}
			return null;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			if (arguments.Length < 2 || arguments[1] == null)
			{
				return intValue.ToString();
			}
			string strValue = arguments[1].GetStrValue(exm);
			try
			{
				return intValue.ToString(strValue);
			}
			catch (FormatException)
			{
				throw new CodeEE("TOSTR関数の書式指定が間違っています");
			}
		}
	}

	private sealed class ToIntMethod : FunctionMethod
	{
		public ToIntMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			if (strValue == null || strValue == "")
			{
				return 0L;
			}
			if (strValue.Length < LangManager.GetStrlenLang(strValue))
			{
				return 0L;
			}
			StringStream stringStream = new StringStream(strValue);
			if (!char.IsDigit(stringStream.Current) && stringStream.Current != '+' && stringStream.Current != '-')
			{
				return 0L;
			}
			if ((stringStream.Current == '+' || stringStream.Current == '-') && !char.IsDigit(stringStream.Next))
			{
				return 0L;
			}
			long result = LexicalAnalyzer.ReadInt64(stringStream, retZero: true);
			if (!stringStream.EOS)
			{
				if (stringStream.Current != '.')
				{
					return 0L;
				}
				stringStream.ShiftNext();
				while (!stringStream.EOS)
				{
					if (!char.IsDigit(stringStream.Current))
					{
						return 0L;
					}
					stringStream.ShiftNext();
				}
			}
			return result;
		}
	}

	[Obfuscation(Exclude = false)]
	private enum StrFormType
	{
		Upper,
		Lower,
		Half,
		Full
	}

	private sealed class StrChangeStyleMethod : FunctionMethod
	{
		private StrFormType strType;

		public StrChangeStyleMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[1] { typeof(string) };
			strType = StrFormType.Upper;
			base.CanRestructure = true;
		}

		public StrChangeStyleMethod(StrFormType type)
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[1] { typeof(string) };
			strType = type;
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			if (strValue == null || strValue == "")
			{
				return "";
			}
			return strType switch
			{
				StrFormType.Upper => strValue.ToUpper(), 
				StrFormType.Lower => strValue.ToLower(), 
				StrFormType.Half => strValue.ToHalf(), 
				StrFormType.Full => strValue.ToWide(), 
				_ => "", 
			};
		}
	}

	private sealed class LineIsEmptyMethod : FunctionMethod
	{
		public LineIsEmptyMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[0];
			base.CanRestructure = false;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (!GlobalStatic.Console.EmptyLine)
			{
				return 0L;
			}
			return 1L;
		}
	}

	private sealed class ReplaceMethod : FunctionMethod
	{
		public ReplaceMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[3]
			{
				typeof(string),
				typeof(string),
				typeof(string)
			};
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			Regex regex;
			try
			{
				regex = new Regex(arguments[1].GetStrValue(exm));
			}
			catch (ArgumentException ex)
			{
				throw new CodeEE("第２引数が正規表現として不正です：" + ex.Message);
			}
			return regex.Replace(strValue, arguments[2].GetStrValue(exm));
		}
	}

	private sealed class UnicodeMethod : FunctionMethod
	{
		public UnicodeMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[1] { typeof(long) };
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[0].GetIntValue(exm);
			if (intValue < 0 || intValue > 65535)
			{
				throw new CodeEE("UNICODE関数に範囲外の値(" + intValue + ")が渡されました");
			}
			return new string(new char[1] { (char)intValue });
		}
	}

	private sealed class UnicodeByteMethod : FunctionMethod
	{
		public UnicodeByteMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			byte[] array = new byte[Encoding.UTF32.GetEncoder().GetByteCount(strValue.ToCharArray(), 0, strValue.Length, flush: false)];
			Encoding.UTF32.GetEncoder().GetBytes(strValue.ToCharArray(), 0, strValue.Length, array, 0, flush: false);
			return BitConverter.ToInt32(array, 0);
		}
	}

	private sealed class ConvertIntMethod : FunctionMethod
	{
		public ConvertIntMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[2]
			{
				typeof(long),
				typeof(long)
			};
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long intValue = arguments[1].GetIntValue(exm);
			if (intValue != 2 && intValue != 8 && intValue != 10 && intValue != 16)
			{
				throw new CodeEE("CONVERT関数の第２引数は2, 8, 10, 16のいずれかでなければなりません");
			}
			return Convert.ToString(arguments[0].GetIntValue(exm), (int)intValue);
		}
	}

	private sealed class IsNumericMethod : FunctionMethod
	{
		public IsNumericMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = true;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			if (strValue.Length < LangManager.GetStrlenLang(strValue))
			{
				return 0L;
			}
			StringStream stringStream = new StringStream(strValue);
			if (!char.IsDigit(stringStream.Current) && stringStream.Current != '+' && stringStream.Current != '-')
			{
				return 0L;
			}
			if ((stringStream.Current == '+' || stringStream.Current == '-') && !char.IsDigit(stringStream.Next))
			{
				return 0L;
			}
			LexicalAnalyzer.ReadInt64(stringStream, retZero: true);
			if (!stringStream.EOS)
			{
				if (stringStream.Current != '.')
				{
					return 0L;
				}
				stringStream.ShiftNext();
				while (!stringStream.EOS)
				{
					if (!char.IsDigit(stringStream.Current))
					{
						return 0L;
					}
					stringStream.ShiftNext();
				}
			}
			return 1L;
		}
	}

	private sealed class EscapeMethod : FunctionMethod
	{
		public EscapeMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return Regex.Escape(arguments[0].GetStrValue(exm));
		}
	}

	private sealed class EncodeToUniMethod : FunctionMethod
	{
		public EncodeToUniMethod()
		{
			base.ReturnType = typeof(long);
			argumentTypeArray = new Type[1];
			base.CanRestructure = true;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length < 1)
			{
				return name + "関数には少なくとも1つの引数が必要です";
			}
			if (arguments.Length > 2)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments[0] == null)
			{
				return name + "関数の1番目の引数は省略できません";
			}
			if (arguments[0].GetOperandType() != typeof(string))
			{
				return name + "関数の1番目の引数の型が正しくありません";
			}
			if (arguments.Length >= 2 && arguments[1] != null && arguments[1].GetOperandType() != typeof(long))
			{
				return name + "関数の2番目の引数の型が正しくありません";
			}
			return null;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			if (strValue.Length == 0)
			{
				return -1L;
			}
			long num = ((arguments.Length > 1 && arguments[1] != null) ? arguments[1].GetIntValue(exm) : 0);
			if (num < 0)
			{
				throw new CodeEE("ENCOIDETOUNI関数の第２引数(" + num + ")が負の値です");
			}
			if (num >= strValue.Length)
			{
				throw new CodeEE("ENCOIDETOUNI関数の第２引数(" + num + ")が第１引数の文字列(" + strValue + ")の文字数を超えています");
			}
			return char.ConvertToUtf32(strValue, (int)num);
		}
	}

	public sealed class CharAtMethod : FunctionMethod
	{
		public CharAtMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[2]
			{
				typeof(string),
				typeof(long)
			};
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			long intValue = arguments[1].GetIntValue(exm);
			if (intValue < 0 || intValue >= strValue.Length)
			{
				return "";
			}
			return strValue[(int)intValue].ToString();
		}
	}

	public sealed class GetLineStrMethod : FunctionMethod
	{
		public GetLineStrMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			if (string.IsNullOrEmpty(strValue))
			{
				throw new CodeEE("GETLINESTR関数の引数が空文字列です");
			}
			return exm.Console.getStBar(strValue);
		}
	}

	public sealed class StrFormMethod : FunctionMethod
	{
		public StrFormMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = true;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			string text = strValue;
			try
			{
				return StrForm.FromWordToken(LexicalAnalyzer.AnalyseFormattedString(new StringStream(strValue), FormStrEndWith.EoL, trim: false)).GetString(exm);
			}
			catch (CodeEE codeEE)
			{
				throw new CodeEE("STRFORM関数:文字列\"" + strValue + "\"の展開エラー:" + codeEE.Message);
			}
			catch
			{
				throw new CodeEE("STRFORM関数:文字列\"" + strValue + "\"の展開処理中にエラーが発生しました");
			}
		}
	}

	public sealed class GetConfigMethod : FunctionMethod
	{
		private readonly string funcname;

		public GetConfigMethod(bool typeisInt)
		{
			if (typeisInt)
			{
				funcname = "GETCONFIG";
				base.ReturnType = typeof(long);
			}
			else
			{
				funcname = "GETCONFIGS";
				base.ReturnType = typeof(string);
			}
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = true;
		}

		private SingleTerm getSingleTerm(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			string strValue = arguments[0].GetStrValue(exm);
			if (strValue == null || strValue.Length == 0)
			{
				throw new CodeEE(funcname + "関数に空文字列が渡されました");
			}
			string errMes = null;
			SingleTerm configValueInERB = ConfigData.Instance.GetConfigValueInERB(strValue, ref errMes);
			if (errMes != null)
			{
				throw new CodeEE(funcname + "関数:" + errMes);
			}
			return configValueInERB;
		}

		public override long GetIntValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (base.ReturnType != typeof(long))
			{
				throw new ExeEE(funcname + "関数:不正な呼び出し");
			}
			SingleTerm singleTerm = getSingleTerm(exm, arguments);
			if (singleTerm.GetOperandType() != typeof(long))
			{
				throw new CodeEE(funcname + "関数:型が違います（GETCONFIGS関数を使用してください）");
			}
			return singleTerm.Int;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			if (base.ReturnType != typeof(string))
			{
				throw new ExeEE(funcname + "関数:不正な呼び出し");
			}
			SingleTerm singleTerm = getSingleTerm(exm, arguments);
			if (singleTerm.GetOperandType() != typeof(string))
			{
				throw new CodeEE(funcname + "関数:型が違います（GETCONFIG関数を使用してください）");
			}
			return singleTerm.Str;
		}
	}

	private sealed class HtmlGetPrintedStrMethod : FunctionMethod
	{
		public HtmlGetPrintedStrMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = null;
			base.CanRestructure = false;
		}

		public override string CheckArgumentType(string name, IOperandTerm[] arguments)
		{
			if (arguments.Length > 1)
			{
				return name + "関数の引数が多すぎます";
			}
			if (arguments.Length == 0 || arguments[0] == null)
			{
				return null;
			}
			if (arguments[0].GetOperandType() != typeof(long))
			{
				return name + "関数の1番目の引数の型が正しくありません";
			}
			return null;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			long num = 0L;
			if (arguments.Length != 0)
			{
				num = arguments[0].GetIntValue(exm);
			}
			if (num < 0)
			{
				throw new CodeEE("引数を0未満にできません");
			}
			ConsoleDisplayLine[] displayLines = exm.Console.GetDisplayLines(num);
			if (displayLines == null)
			{
				return "";
			}
			return HtmlManager.DisplayLine2Html(displayLines, needPandN: true);
		}
	}

	private sealed class HtmlPopPrintingStrMethod : FunctionMethod
	{
		public HtmlPopPrintingStrMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[0];
			base.CanRestructure = false;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			ConsoleDisplayLine[] array = exm.Console.PopDisplayingLines();
			if (array == null)
			{
				return "";
			}
			return HtmlManager.DisplayLine2Html(array, needPandN: false);
		}
	}

	private sealed class HtmlToPlainTextMethod : FunctionMethod
	{
		public HtmlToPlainTextMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = false;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return HtmlManager.Html2PlainText(arguments[0].GetStrValue(exm));
		}
	}

	private sealed class HtmlEscapeMethod : FunctionMethod
	{
		public HtmlEscapeMethod()
		{
			base.ReturnType = typeof(string);
			argumentTypeArray = new Type[1] { typeof(string) };
			base.CanRestructure = false;
		}

		public override string GetStrValue(ExpressionMediator exm, IOperandTerm[] arguments)
		{
			return HtmlManager.Escape(arguments[0].GetStrValue(exm));
		}
	}

	private static Dictionary<string, FunctionMethod> methodList;

	static FunctionMethodCreator()
	{
		methodList = new Dictionary<string, FunctionMethod>();
		methodList["GETCHARA"] = new GetcharaMethod();
		methodList["GETSPCHARA"] = new GetspcharaMethod();
		methodList["CSVNAME"] = new CsvStrDataMethod(CharacterStrData.NAME);
		methodList["CSVCALLNAME"] = new CsvStrDataMethod(CharacterStrData.CALLNAME);
		methodList["CSVNICKNAME"] = new CsvStrDataMethod(CharacterStrData.NICKNAME);
		methodList["CSVMASTERNAME"] = new CsvStrDataMethod(CharacterStrData.MASTERNAME);
		methodList["CSVCSTR"] = new CsvcstrMethod();
		methodList["CSVBASE"] = new CsvDataMethod(CharacterIntData.BASE);
		methodList["CSVABL"] = new CsvDataMethod(CharacterIntData.ABL);
		methodList["CSVMARK"] = new CsvDataMethod(CharacterIntData.MARK);
		methodList["CSVEXP"] = new CsvDataMethod(CharacterIntData.EXP);
		methodList["CSVRELATION"] = new CsvDataMethod(CharacterIntData.RELATION);
		methodList["CSVTALENT"] = new CsvDataMethod(CharacterIntData.TALENT);
		methodList["CSVCFLAG"] = new CsvDataMethod(CharacterIntData.CFLAG);
		methodList["CSVEQUIP"] = new CsvDataMethod(CharacterIntData.EQUIP);
		methodList["CSVJUEL"] = new CsvDataMethod(CharacterIntData.JUEL);
		methodList["FINDCHARA"] = new FindcharaMethod(last: false);
		methodList["FINDLASTCHARA"] = new FindcharaMethod(last: true);
		methodList["EXISTCSV"] = new ExistCsvMethod();
		methodList["VARSIZE"] = new VarsizeMethod();
		methodList["CHKFONT"] = new CheckfontMethod();
		methodList["CHKDATA"] = new CheckdataMethod("CHKDATA", EraSaveFileType.Normal);
		methodList["ISSKIP"] = new IsSkipMethod();
		methodList["MOUSESKIP"] = new MesSkipMethod(warn: true);
		methodList["MESSKIP"] = new MesSkipMethod(warn: false);
		methodList["GETCOLOR"] = new GetColorMethod(isDef: false);
		methodList["GETDEFCOLOR"] = new GetColorMethod(isDef: true);
		methodList["GETFOCUSCOLOR"] = new GetFocusColorMethod();
		methodList["GETBGCOLOR"] = new GetBGColorMethod(isDef: false);
		methodList["GETDEFBGCOLOR"] = new GetBGColorMethod(isDef: true);
		methodList["GETSTYLE"] = new GetStyleMethod();
		methodList["GETFONT"] = new GetFontMethod();
		methodList["BARSTR"] = new BarStringMethod();
		methodList["CURRENTALIGN"] = new CurrentAlignMethod();
		methodList["CURRENTREDRAW"] = new CurrentRedrawMethod();
		methodList["COLOR_FROMNAME"] = new ColorFromNameMethod();
		methodList["COLOR_FROMRGB"] = new ColorFromRGBMethod();
		methodList["CHKCHARADATA"] = new CheckdataStrMethod("CHKCHARADATA", EraSaveFileType.CharVar);
		methodList["FIND_CHARADATA"] = new FindFilesMethod("FIND_CHARADATA", EraSaveFileType.CharVar);
		methodList["MONEYSTR"] = new MoneyStrMethod();
		methodList["PRINTCPERLINE"] = new GetPrintCPerLineMethod();
		methodList["PRINTCLENGTH"] = new PrintCLengthMethod();
		methodList["SAVENOS"] = new GetSaveNosMethod();
		methodList["GETTIME"] = new GettimeMethod();
		methodList["GETTIMES"] = new GettimesMethod();
		methodList["GETMILLISECOND"] = new GetmsMethod();
		methodList["GETSECOND"] = new GetSecondMethod();
		methodList["RAND"] = new RandMethod();
		methodList["MIN"] = new MaxMethod(max: false);
		methodList["MAX"] = new MaxMethod(max: true);
		methodList["ABS"] = new AbsMethod();
		methodList["POWER"] = new PowerMethod();
		methodList["SQRT"] = new SqrtMethod();
		methodList["CBRT"] = new CbrtMethod();
		methodList["LOG"] = new LogMethod();
		methodList["LOG10"] = new LogMethod(10.0);
		methodList["EXPONENT"] = new ExpMethod();
		methodList["SIGN"] = new SignMethod();
		methodList["LIMIT"] = new GetLimitMethod();
		methodList["SUMARRAY"] = new SumArrayMethod();
		methodList["SUMCARRAY"] = new SumArrayMethod(isChara: true);
		methodList["MATCH"] = new MatchMethod();
		methodList["CMATCH"] = new MatchMethod(isChara: true);
		methodList["GROUPMATCH"] = new GroupMatchMethod();
		methodList["NOSAMES"] = new NosamesMethod();
		methodList["ALLSAMES"] = new AllsamesMethod();
		methodList["MAXARRAY"] = new MaxArrayMethod();
		methodList["MAXCARRAY"] = new MaxArrayMethod(isChara: true);
		methodList["MINARRAY"] = new MaxArrayMethod(isChara: false, isMaxFunc: false);
		methodList["MINCARRAY"] = new MaxArrayMethod(isChara: true, isMaxFunc: false);
		methodList["GETBIT"] = new GetbitMethod();
		methodList["GETNUM"] = new GetnumMethod();
		methodList["GETPALAMLV"] = new GetPalamLVMethod();
		methodList["GETEXPLV"] = new GetExpLVMethod();
		methodList["FINDELEMENT"] = new FindElementMethod(last: false);
		methodList["FINDLASTELEMENT"] = new FindElementMethod(last: true);
		methodList["INRANGE"] = new InRangeMethod();
		methodList["INRANGEARRAY"] = new InRangeArrayMethod();
		methodList["INRANGECARRAY"] = new InRangeArrayMethod(isChara: true);
		methodList["GETNUMB"] = new GetnumMethod();
		methodList["STRLENS"] = new StrlenMethod();
		methodList["STRLENSU"] = new StrlenuMethod();
		methodList["SUBSTRING"] = new SubstringMethod();
		methodList["SUBSTRINGU"] = new SubstringuMethod();
		methodList["STRFIND"] = new StrfindMethod(unicode: false);
		methodList["STRFINDU"] = new StrfindMethod(unicode: true);
		methodList["STRCOUNT"] = new StrCountMethod();
		methodList["TOSTR"] = new ToStrMethod();
		methodList["TOINT"] = new ToIntMethod();
		methodList["TOUPPER"] = new StrChangeStyleMethod(StrFormType.Upper);
		methodList["TOLOWER"] = new StrChangeStyleMethod(StrFormType.Lower);
		methodList["TOHALF"] = new StrChangeStyleMethod(StrFormType.Half);
		methodList["TOFULL"] = new StrChangeStyleMethod(StrFormType.Full);
		methodList["LINEISEMPTY"] = new LineIsEmptyMethod();
		methodList["REPLACE"] = new ReplaceMethod();
		methodList["UNICODE"] = new UnicodeMethod();
		methodList["UNICODEBYTE"] = new UnicodeByteMethod();
		methodList["CONVERT"] = new ConvertIntMethod();
		methodList["ISNUMERIC"] = new IsNumericMethod();
		methodList["ESCAPE"] = new EscapeMethod();
		methodList["ENCODETOUNI"] = new EncodeToUniMethod();
		methodList["CHARATU"] = new CharAtMethod();
		methodList["GETLINESTR"] = new GetLineStrMethod();
		methodList["STRFORM"] = new StrFormMethod();
		methodList["GETCONFIG"] = new GetConfigMethod(typeisInt: true);
		methodList["GETCONFIGS"] = new GetConfigMethod(typeisInt: false);
		methodList["HTML_GETPRINTEDSTR"] = new HtmlGetPrintedStrMethod();
		methodList["HTML_POPPRINTINGSTR"] = new HtmlPopPrintingStrMethod();
		methodList["HTML_TOPLAINTEXT"] = new HtmlToPlainTextMethod();
		methodList["HTML_ESCAPE"] = new HtmlEscapeMethod();
	}

	public static Dictionary<string, FunctionMethod> GetMethodList()
	{
		return methodList;
	}
}
