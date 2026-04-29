using System;
using System.Drawing;
using System.Reflection;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using EmueraFramework;
using MinorShift.Emuera;
using Config = MinorShift.Emuera.Config;

namespace EraAndroid.FrontEnd;

[Register("EraAndroid.FrontEnd.EmueraFrontEnd")]
public class EmueraFrontEnd : View, IFrontEnd
{
	private class EmueraScrollbar : IScrollBar
	{
		private EmueraFrontEnd _console;

		public bool Enabled
		{
			get
			{
				return _console.VerticalScrollBarEnabled;
			}
			set
			{
				_console.VerticalScrollBarEnabled = value;
			}
		}

		public bool IsBackLog => GameData.ScrollView.ScrollY + 6 < _console.ConsoleHeight - GameData.ScrollView.Height;

		public EmueraScrollbar(EmueraFrontEnd console)
		{
			_console = console;
		}

		public void MoveToEnd()
		{
			if (!GameData.MainActivity.EmueraInitializing)
			{
				GameData.ScrollView.Handler.PostDelayed(delegate
				{
					GameData.ScrollView.FullScroll(FocusSearchDirection.Down);
				}, 50L);
				GameData.ScrollView.Handler.PostDelayed(delegate
				{
					GameData.ScrollView.FullScroll(FocusSearchDirection.Down);
				}, 300L);
			}
		}
	}

	private static Paint textPaint = new Paint
	{
		AntiAlias = true,
		Color = Android.Graphics.Color.LightGray,
		Hinting = PaintHinting.On,
		TextAlign = Paint.Align.Center,
		TextSize = 40f
	};

	private const int MB_BYTE = 1048576;

	private DateTime preTouchTime = DateTime.Now;

	private readonly TimeSpan ClickInterval = new TimeSpan(0, 0, 0, 0, 150);

	private System.Drawing.Point preTouchPoint;

	private Context context;

	private string lastInput = "";

	private EmueraScrollbar scrollbar;

	private bool MesSkip { get; set; }

	private bool IsBackLog { get; set; }

	private int ConsoleHeight
	{
		get
		{
			if (GameData.MainActivity.EmueraInitializing)
			{
				return ShowHeight;
			}
			return Math.Min(ShowHeight * 10, GlobalStatic.Console.DisplayLineCount * Config.LineHeight);
		}
	}

	private int ShowHeight => (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 590f, Resources.DisplayMetrics);

	public string Input
	{
		set
		{
			lastInput = value;
			GlobalStatic.Console.PressEnterKey(MesSkip, value, changedByMouse: true);
		}
	}

	public string MacroInput
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				lastInput = value;
				GlobalStatic.Console.PressEnterKey(MesSkip, value, changedByMouse: false);
			}
		}
	}

	IScrollBar IFrontEnd.ScrollBar
	{
		get
		{
			return scrollbar;
		}
		set
		{
			scrollbar = value as EmueraScrollbar;
		}
	}

	int IFrontEnd.Width => Resources.DisplayMetrics.WidthPixels;

	int IFrontEnd.Height => ConsoleHeight;

	public bool Created => GameData.Enable;

	public string InternalEmueraVer => Assembly.GetExecutingAssembly().GetName().Version.ToString();

	public Android.Graphics.Color TextBoxBackColor
	{
		get
		{
			return GameData.InputText.DrawingCacheBackgroundColor;
		}
		set
		{
			GameData.InputText.DrawingCacheBackgroundColor = value;
		}
	}

	public Android.Graphics.Color TextBoxForeColor
	{
		get
		{
			return new Android.Graphics.Color(GameData.InputText.CurrentTextColor);
		}
		set
		{
			GameData.InputText.SetTextColor(value);
		}
	}

	protected override void OnDraw(Canvas canvas)
	{
		if (GameData.MainActivity.EmueraInitializing)
		{
			int num = canvas.Width / 2;
			int num2 = canvas.Height / 2;
			long totalMemory = GC.GetTotalMemory(forceFullCollection: false);
            canvas.DrawText("초기화 중...", num, num2, textPaint);
            num2 += (int)(textPaint.TextSize * 2f);
            canvas.DrawText("메모리: " + totalMemory / 1048576 + "MB", num, num2, textPaint);
        }
		else
		{
			canvas.DrawColor(Config.BackColor);
			GlobalStatic.Console.OnPaint(canvas);
		}
	}

	protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
	{
		SetMeasuredDimension(Resources.DisplayMetrics.WidthPixels, ConsoleHeight);
	}

	public override bool OnTouchEvent(MotionEvent e)
	{
		if (GameData.MainActivity.EmueraInitializing)
		{
			return false;
		}
		preTouchPoint = new System.Drawing.Point((int)e.GetX(), (int)e.GetY());
		switch (e.Action)
		{
		case MotionEventActions.Down:
			if (GlobalStatic.Console.MoveMouse(preTouchPoint))
			{
				GlobalStatic.Console.RefreshStrings(force_Paint: true);
			}
			preTouchTime = DateTime.Now;
			return true;
		case MotionEventActions.Move:
			if (GlobalStatic.Console.MoveMouse(preTouchPoint))
			{
				GlobalStatic.Console.RefreshStrings(force_Paint: true);
			}
			return true;
		case MotionEventActions.Up:
		{
			DateTime now = DateTime.Now;
			if (GlobalStatic.Console.MoveMouse(preTouchPoint))
			{
				GlobalStatic.Console.RefreshStrings(force_Paint: true);
			}
			if (now - preTouchTime < ClickInterval)
			{
				bool isBackLog = scrollbar.IsBackLog;
				string selectedString = GlobalStatic.Console.SelectedString;
				if (isBackLog)
				{
					GlobalStatic.Console.RefreshStrings(force_Paint: true);
				}
				if (GlobalStatic.Console.IsWaitingEnterKey && !GlobalStatic.Console.IsError && selectedString == null && !isBackLog)
				{
					GlobalStatic.Console.PressEnterKey(MesSkip, "", changedByMouse: true);
				}
				if (selectedString != null)
				{
					if (GlobalStatic.Console.IsWaintingOnePhrase)
					{
						lastInput = "";
					}
					FileLog.Info("GetButton", string.Format("Input Button {0}:{1}", "str", selectedString));
					GlobalStatic.Console.PressEnterKey(MesSkip, selectedString, changedByMouse: true);
				}
				scrollbar.MoveToEnd();
			}
			GlobalStatic.Console.LeaveMouse();
			return true;
		}
		default:
			return base.OnTouchEvent(e);
		}
	}

	public EmueraFrontEnd(Context context)
		: base(context)
	{
		init(context);
	}

	public EmueraFrontEnd(Context context, IAttributeSet attrs)
		: base(context, attrs)
	{
		init(context);
	}

	public EmueraFrontEnd(Context context, IAttributeSet attrs, int defStyle)
		: base(context, attrs, defStyle)
	{
		init(context);
	}

	private void init(Context context)
	{
		this.context = context;
		scrollbar = new EmueraScrollbar(this);
	}

	public System.Drawing.Point GetLastTouchPoint()
	{
		FileLog.Info("Function Call", "GetLastTouchPoint");
		return preTouchPoint;
	}

	public void Close()
	{
		FileLog.Info("Function Call", "EmueraConsoleClose");
		GameData.MainActivity.Close();
	}

	public void Refresh()
	{
		if (!GameData.MainActivity.EmueraInitializing)
		{
			RequestLayout();
		}
	}

	public void update_lastinput()
	{
		GameData.InputText.Text = lastInput;
	}

	public void clear_richText()
	{
		GameData.InputText.Text = "";
	}
}
