using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace EraAndroid;

[Activity(Label = "글자크기를 설정한뒤 뒤로가기를 2번 누르세요")]
public class SetFontActivity : Activity
{
	private TextView tv;

	private EditText input;

	private bool pressBackKey;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		RequestedOrientation = ScreenOrientation.Portrait;
		SetContentView(2130903042);
        tv = FindViewById<TextView>(global::EraAndroid64.Resource.Id.fontPreviewTV); tv.Paint.AntiAlias = true;
		tv.Paint.Hinting = PaintHinting.On;
        if (int.TryParse(DB.Load("fontSize"), out var result))
        {
			tv.TextSize = result;
		}
		else
		{
			tv.TextSize = Math.Min(Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels) / 40;
		}
		for (int i = 0; i < 60; i++)
		{
            tv.Text = tv.Text + "가나다라마바사 ABC abc 123" + System.Environment.NewLine;
        }
        input = FindViewById<EditText>(global::EraAndroid64.Resource.Id.fontSizeET);
        input.KeyPress += Input_KeyPress;
	}

	private void Input_KeyPress(object sender, View.KeyEventArgs e)
	{
		if (e.Event.Action != KeyEventActions.Down)
		{
			return;
		}
		if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.NumpadEnter)
		{
			if (int.TryParse(input.Text, out var result))
			{
				Intent.PutExtra("OriginalTextSize", input.Text);
				tv.TextSize = result;
				tv.PostInvalidate();
			}
		}
		else
		{
			if (e.KeyCode == Keycode.Back)
			{
				if (pressBackKey)
				{
					Intent.PutExtra("TextSize", (int)tv.TextSize);
					Intent.PutExtra("LineHeight", tv.LineHeight);
					SetResult(Result.Ok, Intent);
					Finish();
				}
				else
				{
					Toast.MakeText(this, "한번 더 누르면 저장됩니다", ToastLength.Short).Show();
					pressBackKey = true;
				}
				return;
			}
			input.OnKeyDown(e.KeyCode, e.Event);
		}
		pressBackKey = false;
	}
}
