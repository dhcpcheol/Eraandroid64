using Android.Widget;
using EraAndroid.FrontEnd;

namespace EraAndroid;

public static class GameData
{
	public static EmueraFrontEnd FrontEnd { get; set; }

	public static MainActivity MainActivity { get; set; }

	public static EditText InputText { get; set; }

	public static bool Enable { get; internal set; }

	public static ScrollView ScrollView { get; internal set; }
}
