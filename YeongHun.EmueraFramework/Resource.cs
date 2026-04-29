using System.CodeDom.Compiler;
using Android.Runtime;

namespace YeongHun.EmueraFramework;

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

	public class String
	{
		public static int ApplicationName;

		public static int Hello;

		static String()
		{
			ApplicationName = 2130837505;
			Hello = 2130837504;
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
}
