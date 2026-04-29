using System.Diagnostics;

namespace MinorShift._Library;

internal sealed class WinmmTimer
{
	private static volatile Stopwatch instance;

	public static uint TickCount => (uint)instance.ElapsedMilliseconds;

	static WinmmTimer()
	{
		instance = new Stopwatch();
		instance.Start();
	}

	private WinmmTimer()
	{
	}

	~WinmmTimer()
	{
		instance.Stop();
	}
}
