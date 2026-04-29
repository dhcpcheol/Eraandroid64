using System.Text;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.GameProc;
using MinorShift.Emuera.GameProc.Function;
using MinorShift.Emuera.GameView;
using MinorShift.Emuera.Sub;

namespace MinorShift.Emuera.GameData.Expression;

internal sealed class ExpressionMediator
{
	public readonly VariableEvaluator VEvaluator;

	public readonly Process Process;

	public readonly EmueraConsole Console;

	private bool forceHiragana;

	private bool forceKatakana;

	private bool halftoFull;

	public ExpressionMediator(Process proc, VariableEvaluator vev, EmueraConsole console)
	{
		VEvaluator = vev;
		Process = proc;
		Console = console;
	}

	public void ForceKana(long flag)
	{
		if (flag < 0 || flag > 3)
		{
			throw new CodeEE("命令FORCEKANAの引数が指定可能な範囲(0～3)を超えています");
		}
		forceKatakana = ((flag == 1) ? true : false);
		forceHiragana = ((flag > 1) ? true : false);
		halftoFull = ((flag == 3) ? true : false);
	}

	public bool ForceKana()
	{
		return forceHiragana | forceKatakana | halftoFull;
	}

	public void OutputToConsole(string str, FunctionIdentifier func)
	{
		if (func.IsPrintSingle())
		{
			Console.PrintSingleLine(str, temporary: false);
		}
		else
		{
			Console.Print(str);
			if (func.IsNewLine() || func.IsWaitInput())
			{
				Console.NewLine();
				if (func.IsWaitInput())
				{
					Console.ReadAnyKey();
				}
			}
		}
		Console.UseSetColorStyle = true;
	}

	public string ConvertStringType(string str)
	{
		return str;
	}

	public string CheckEscape(string str)
	{
		StringStream stringStream = new StringStream(str);
		StringBuilder stringBuilder = new StringBuilder();
		while (!stringStream.EOS)
		{
			if (stringStream.Current == '\\')
			{
				stringStream.ShiftNext();
				switch (stringStream.Current)
				{
				case '\\':
					stringBuilder.Append('\\');
					stringBuilder.Append('\\');
					break;
				case '%':
				case '@':
				case '{':
				case '}':
					stringBuilder.Append('\\');
					stringBuilder.Append(stringStream.Current);
					break;
				default:
					stringBuilder.Append("\\\\");
					stringBuilder.Append(stringStream.Current);
					break;
				}
				stringStream.ShiftNext();
			}
			else
			{
				stringBuilder.Append(stringStream.Current);
				stringStream.ShiftNext();
			}
		}
		return stringBuilder.ToString();
	}

	public string CreateBar(long var, long max, long length)
	{
		if (max <= 0)
		{
			throw new CodeEE("BARの最大値が正の値ではありません");
		}
		if (length <= 0)
		{
			throw new CodeEE("BARの長さが正の値ではありません");
		}
		if (length >= 100)
		{
			throw new CodeEE("BARが長すぎます");
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		int num = (int)(var * length / max);
		if (num < 0)
		{
			num = 0;
		}
		if (num > length)
		{
			num = (int)length;
		}
		stringBuilder.Append(Config.BarChar1, num);
		stringBuilder.Append(Config.BarChar2, (int)length - num);
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}
}
