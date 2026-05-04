using System;
using System.IO;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace EraAndroid;

[Activity(Label = "길게 눌러서 Emuera 폴더를 선택해주세요")]
public class SelectFolderActivity : Activity
{
	private ListView lv;

	private string preSelected = "";

	private readonly string GotoParent = "..";

	private TextView tempTV;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		RequestedOrientation = ScreenOrientation.Portrait;
        SetContentView(global::EraAndroid64.Resource.Layout.selectfolder); tempTV = new TextView(this);
        lv = FindViewById<ListView>(global::EraAndroid64.Resource.Id.selectFolderLV); lv.ItemClick += ItemClick;
		lv.ItemLongClick += ItemLongClick;
		lv.KeyPress += Lv_KeyPress;
        preSelected = DB.Load("selectedPath");

        preSelected = DB.Load("selectedPath");

        // 경로 선택 화면에서 디버그 모드를 켜고 끌 수 있도록 체크박스를 추가한다.
        AddDebugModeCheckBox();

        if (string.IsNullOrEmpty(preSelected) || !Directory.Exists(preSelected))
        {
            string[] candidates =
            {
        Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
        "/storage/emulated/150",
        "/storage/emulated/0"
    };

            preSelected = candidates.FirstOrDefault(Directory.Exists)
                ?? Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        }

        UpdateDirectories(preSelected);
    }

    // dp 단위를 실제 픽셀로 변환한다.
    // 화면 밀도에 따라 체크박스 위치와 크기가 달라지는 것을 방지하기 위해 사용한다.
    private int Dp(float value)
    {
        return (int)Android.Util.TypedValue.ApplyDimension(
            Android.Util.ComplexUnitType.Dip,
            value,
            Resources.DisplayMetrics
        );
    }

    // 경로 선택 화면에 디버그 모드 체크박스를 추가한다.
    // 체크 상태는 DB에 저장하여 앱 재실행 후에도 유지한다.
    private void AddDebugModeCheckBox()
    {
        CheckBox debugCheckBox = new CheckBox(this);

        debugCheckBox.Text = "디버그 모드";
        debugCheckBox.TextSize = 14;
        debugCheckBox.SetTextColor(Color.White);
        debugCheckBox.SetBackgroundColor(Color.Argb(120, 0, 0, 0));

        debugCheckBox.Checked = DB.Load("debugMode") == "true";

        debugCheckBox.CheckedChange += (sender, e) =>
        {
            DB.Save("debugMode", e.IsChecked ? "true" : "false");
        };

        FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(
            Dp(150),
            Dp(48)
        );

        layoutParams.Gravity = GravityFlags.Bottom | GravityFlags.Right;
        layoutParams.BottomMargin = Dp(12);
        layoutParams.RightMargin = Dp(12);

        AddContentView(debugCheckBox, layoutParams);
    }

    private void Lv_KeyPress(object sender, View.KeyEventArgs e)
    {
        if (e.Event.Action == KeyEventActions.Down)
		{
			if (e.KeyCode == Keycode.Back)
			{
				UpdateDirectories(preSelected);
			}
			else
			{
				lv.OnKeyDown(e.KeyCode, e.Event);
			}
		}
	}

	private void ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
	{
		string value = (string)lv.Adapter.GetItem(e.Position);
		Intent.PutExtra("Selected Path", value);
		SetResult(Result.Ok, Intent);
		Finish();
	}

	private void ItemClick(object sender, AdapterView.ItemClickEventArgs e)
	{
		if (e.Position == 0)
		{
			try
			{
				preSelected = Directory.GetParent(preSelected).FullName;
			}
			catch (UnauthorizedAccessException)
			{
				return;
			}
			catch (Exception ex2)
			{
				Toast.MakeText(this, ex2.Message, ToastLength.Long).Show();
				return;
			}
		}
		else
		{
			preSelected = (string)lv.Adapter.GetItem(e.Position);
		}
		UpdateDirectories(preSelected);
	}

	private void UpdateDirectories(string root)
	{
		if (!Directory.Exists(root))
		{
			return;
		}
		try
		{
			ArrayAdapter<string> arrayAdapter = new ArrayAdapter<string>(this, 17367043);
			lv.Adapter = arrayAdapter;
			arrayAdapter.Add(GotoParent);
			string[] directories = Directory.GetDirectories(root);
			foreach (string text in directories)
			{
				arrayAdapter.Add(text);
			}
		}
		catch (UnauthorizedAccessException)
		{
		}
		catch (Exception ex2)
		{
			Toast.MakeText(this, ex2.Message, ToastLength.Long);
		}
	}
}
