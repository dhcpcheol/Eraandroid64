using System;
using Android.Graphics;

namespace MinorShift.Emuera.Content;

internal sealed class BaseImage : AContentFile
{
	public Bitmap Bitmap;

	private Canvas g;

	private IntPtr hBitmap;

	private IntPtr hDefaultImg;

	public IntPtr GDIhDC { get; private set; }

	public BaseImage(string name, string path)
		: base(name, path)
	{
	}

	public void Load(bool useGDI)
	{
		if (Loaded)
		{
			return;
		}
		try
		{
			Bitmap = BitmapFactory.DecodeFile(Filepath);
			g = new Canvas(Bitmap);
			Loaded = true;
			base.Enabled = true;
		}
		catch
		{
		}
	}

	public override void Dispose()
	{
		if (Bitmap != null)
		{
			if (g != null)
			{
				g.Dispose();
				g = null;
			}
			Bitmap.Dispose();
			Bitmap = null;
		}
	}
}
