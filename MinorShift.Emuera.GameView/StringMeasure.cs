using System;
using Android.Graphics;

namespace MinorShift.Emuera.GameView;

internal sealed class StringMeasure : IDisposable
{
	private static Paint p = new Paint();

	private bool disposed;

	public int GetDisplayLength(string s, int textSize)
	{
		if (string.IsNullOrEmpty(s))
		{
			return 0;
		}
		p.TextSize = textSize;
		return (int)p.MeasureText(s);
	}

	public void Dispose()
	{
		if (!disposed)
		{
			disposed = true;
		}
	}
}
