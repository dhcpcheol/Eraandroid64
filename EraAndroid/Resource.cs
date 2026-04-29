using System.CodeDom.Compiler;
using Android.Runtime;
using YeongHun.EmueraFramework;

namespace EraAndroid;

[GeneratedCode("Xamarin.Android.Build.Tasks", "1.0.0.0")]
public class Resource
{
	public class Attribute
	{
		static Attribute()
		{
			ResourceIdManager.UpdateIdValues();
		}

		private Attribute()
		{
		}
	}

	public class Drawable
	{
		public const int Icon = 2130837504;

		static Drawable()
		{
			ResourceIdManager.UpdateIdValues();
		}

		private Drawable()
		{
		}
	}

	public class Id
	{
		public const int emueraConsole = 2131034113;

		public const int emueraScrollView = 2131034112;

		public const int fontPreviewTV = 2131034117;

		public const int fontSizeET = 2131034118;

		public const int inputEditText = 2131034115;

		public const int linearLayout1 = 2131034114;

		public const int selectFolderLV = 2131034116;

		static Id()
		{
			ResourceIdManager.UpdateIdValues();
		}

		private Id()
		{
		}
	}

	public class Layout
	{
		public const int Main = 2130903040;

		public const int SelectFolder = 2130903041;

		public const int SetFont = 2130903042;

		static Layout()
		{
			ResourceIdManager.UpdateIdValues();
		}

		private Layout()
		{
		}
	}

	public class String
	{
		public const int ApplicationName = 2130968577;

		public const int FontPreviewString = 2130968582;

		public const int Hello = 2130968576;

		public const int InitializingEmuera = 2130968578;

		public const int PreviousFontSizeSettingName = 2130968583;

		public const int PreviousMemoryUsageSettingName = 2130968581;

		public const int PreviousSelectedFolderSettingName = 2130968584;

		public const int ShowMemory = 2130968579;

		public const int ShowProgress = 2130968580;

		static String()
		{
			ResourceIdManager.UpdateIdValues();
		}

		private String()
		{
		}
	}

	static Resource()
	{
		ResourceIdManager.UpdateIdValues();
	}

	public static void UpdateIdValues()
	{
		YeongHun.EmueraFramework.Resource.String.ApplicationName = 2130968577;
		YeongHun.EmueraFramework.Resource.String.Hello = 2130968576;
	}
}
