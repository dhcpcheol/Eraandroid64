using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using EmueraFramework;
using MinorShift.Emuera.GameData;
using MinorShift.Emuera.GameData.Expression;
using MinorShift.Emuera.GameData.Variable;
using MinorShift.Emuera.GameProc;
using MinorShift.Emuera.GameView;

namespace MinorShift.Emuera;

public static class GlobalStatic
{
	public static IFrontEnd FrontEnd;

	public static Activity MainWindow;

	public static EmueraConsole Console;

	internal static Process Process;

	internal static Paint GlobalPaint = new Paint();

	internal static GameBase GameBaseData;

	internal static ConstantData ConstantData;

	internal static VariableData VariableData;

	internal static VariableEvaluator VEvaluator;

	internal static IdentifierDictionary IdentifierDictionary;

	internal static ExpressionMediator EMediator;

	internal static LabelDictionary LabelDictionary;

	public static Dictionary<string, long> tempDic = new Dictionary<string, long>();

	public static void Reset()
	{
		Process = null;
		ConstantData = null;
		GameBaseData = null;
		EMediator = null;
		VEvaluator = null;
		VariableData = null;
		Console = null;
		MainWindow = null;
		LabelDictionary = null;
		IdentifierDictionary = null;
		tempDic.Clear();
	}
}
