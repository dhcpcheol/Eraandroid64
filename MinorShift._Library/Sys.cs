using System.Diagnostics;
using System.IO;
using MinorShift.Emuera;

namespace MinorShift._Library;

public static class Sys
{
	public static readonly string ExePath;

	public static readonly string ExeDir;

	public static readonly string ExeName;

	static Sys()
	{
		ExePath = Program.ExeDir + "/EmueraCS.exe";
		ExeDir = Path.GetDirectoryName(ExePath) + "/";
		ExeName = Path.GetFileName(ExePath);
	}

	public static bool PrevInstance()
	{
		if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
		{
			return true;
		}
		return false;
	}
}
