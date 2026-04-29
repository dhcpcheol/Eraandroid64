using System;
using System.Drawing;
using System.Text;
using Android.Graphics;
using MinorShift.Emuera.Content;

namespace MinorShift.Emuera.GameView;

internal class ConsoleImagePart : AConsoleDisplayPart
{
	private readonly CroppedImage cImage;

	private readonly CroppedImage cImageB;

	private readonly int top;

	private readonly int bottom;

	private readonly Rectangle destRect;

	public readonly string ResourceName;

	public readonly string ButtonResourceName;

	public override int Top => top;

	public override int Bottom => bottom;

	public override bool CanDivide => false;

	public ConsoleImagePart(string resName, string resNameb, int raw_height, int raw_width, int raw_ypos)
	{
		top = 0;
		bottom = Config.FontSize;
		base.Str = "";
		ResourceName = resName ?? "";
		ButtonResourceName = resNameb;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<img src='");
		stringBuilder.Append(ResourceName);
		if (ButtonResourceName != null)
		{
			stringBuilder.Append("' srcb='");
			stringBuilder.Append(ButtonResourceName);
		}
		if (raw_height != 0)
		{
			stringBuilder.Append("' height='");
			stringBuilder.Append(raw_height.ToString());
		}
		if (raw_width != 0)
		{
			stringBuilder.Append("' width='");
			stringBuilder.Append(raw_height.ToString());
		}
		if (raw_ypos != 0)
		{
			stringBuilder.Append("' ypos='");
			stringBuilder.Append(raw_height.ToString());
		}
		stringBuilder.Append("'>");
		base.AltText = stringBuilder.ToString();
		cImage = AppContents.GetContent<CroppedImage>(ResourceName);
		if (cImage != null && !cImage.Enabled)
		{
			cImage = null;
		}
		if (cImage == null)
		{
			base.Str = base.AltText;
			return;
		}
		int num = 0;
		if (cImage.NoResize)
		{
			num = cImage.Rectangle.Height;
			base.Width = cImage.Rectangle.Width;
		}
		else
		{
			num = ((raw_height != 0) ? (Config.FontSize * raw_height / 100) : Config.FontSize);
			if (raw_width == 0)
			{
				base.Width = cImage.Rectangle.Width * num / cImage.Rectangle.Height;
				base.XsubPixel = (float)cImage.Rectangle.Width * (float)num / (float)cImage.Rectangle.Height - (float)base.Width;
			}
			else
			{
				base.Width = Config.FontSize * raw_width / 100;
				base.XsubPixel = (float)Config.FontSize * (float)raw_width / 100f - (float)base.Width;
			}
		}
		top = raw_ypos * Config.FontSize / 100;
		destRect = new Rectangle(0, top, base.Width, num);
		if (destRect.Width < 0)
		{
			destRect.X = -destRect.Width;
			base.Width = -destRect.Width;
		}
		if (destRect.Height < 0)
		{
			destRect.Y -= destRect.Height;
			num = -destRect.Height;
		}
		bottom = top + num;
		if (ButtonResourceName != null)
		{
			cImageB = AppContents.GetContent<CroppedImage>(ButtonResourceName);
			if (cImageB != null && !cImageB.Enabled)
			{
				cImageB = null;
			}
		}
	}

	public override void SetWidth(StringMeasure sm, float subPixel)
	{
		if (base.Error)
		{
			base.Width = 0;
		}
		else if (cImage == null)
		{
			base.Width = sm.GetDisplayLength(base.Str, Config.FontSize);
			base.XsubPixel = subPixel;
		}
	}

	public override string ToString()
	{
		if (base.AltText == null)
		{
			return "";
		}
		return base.AltText;
	}

	public override void DrawTo(Canvas graph, int pointY, bool isSelecting, bool isBackLog, TextDrawingMode mode)
	{
		if (!base.Error)
		{
			CroppedImage croppedImage = cImage;
			if (isSelecting && cImageB != null)
			{
				croppedImage = cImageB;
			}
			Rectangle rectangle = destRect;
			rectangle.X = destRect.X + base.PointX + Config.DrawingParam_ShapePositionShift;
			rectangle.Y = destRect.Y + pointY;
			if (croppedImage != null)
			{
				graph.DrawBitmap(croppedImage.BaseImage.Bitmap, rectangle.ToRect(), croppedImage.Rectangle.ToRect(), new Paint());
			}
			else
			{
				graph.DrawString(base.Str, base.PointX, pointY, Config.FontName);
			}
		}
	}

	public override void GDIDrawTo(int pointY, bool isSelecting, bool isBackLog)
	{
		if (base.Error)
		{
			return;
		}
		throw new NotImplementedException();
	}
}
