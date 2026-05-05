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

    // 뒤로가기를 두 번 눌렀는지 판단하기 위한 플래그이다.
    private bool pressBackKey;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        RequestedOrientation = ScreenOrientation.Portrait;
        SetContentView(global::EraAndroid64.Resource.Layout.setfont);

        tv = FindViewById<TextView>(global::EraAndroid64.Resource.Id.fontPreviewTV);
        tv.Paint.AntiAlias = true;
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

            pressBackKey = false;
            return;
        }

        if (e.KeyCode == Keycode.Back)
        {
            SaveAndFinishAfterSecondBack();
            return;
        }

        input.OnKeyDown(e.KeyCode, e.Event);
        pressBackKey = false;
    }

    public override void OnBackPressed()
    {
        // 기기 뒤로가기 버튼도 EditText의 키 입력과 동일하게 처리한다.
        SaveAndFinishAfterSecondBack();
    }

    private void SaveAndFinishAfterSecondBack()
    {
        if (pressBackKey)
        {
            // 두 번째 뒤로가기에서 현재 미리보기 글자 크기를 저장하고 다음 단계로 진행한다.
            Intent.PutExtra("TextSize", (int)tv.TextSize);
            Intent.PutExtra("LineHeight", tv.LineHeight);
            Intent.PutExtra("OriginalTextSize", ((int)tv.TextSize).ToString());
            SetResult(Result.Ok, Intent);
            Finish();
            return;
        }

        // 첫 번째 뒤로가기에서는 저장하지 않고 사용자에게 한 번 더 눌러야 함을 안내한다.
        Toast.MakeText(this, "한번 더 누르면 저장됩니다", ToastLength.Short).Show();
        pressBackKey = true;
    }
}