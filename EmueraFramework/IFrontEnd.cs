using System.Drawing;
using Android.Graphics;

namespace EmueraFramework;

public interface IFrontEnd
{
	IScrollBar ScrollBar { get; set; }

	bool Created { get; }

	int Height { get; }

	int Width { get; }

	string InternalEmueraVer { get; }

	Android.Graphics.Color TextBoxBackColor { get; set; }

	Android.Graphics.Color TextBoxForeColor { get; set; }

	System.Drawing.Point GetLastTouchPoint();

	void Close();

	void Refresh();

	void update_lastinput();

	void clear_richText();
}
