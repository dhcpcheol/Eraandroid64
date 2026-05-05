using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Android.Graphics;

namespace EraAndroid;

[Activity(Label = "길게 눌러서 Emuera 폴더를 선택해주세요")]
public class SelectFolderActivity : Activity
{
    // 뒤로가기를 두 번 눌렀는지 판단하기 위한 플래그이다.
    private bool pressBackKey;

    private const int RequestOpenDocumentTree = 1001;

    private ListView lv;

    private string preSelected = "";

	private readonly string GotoParent = "..";

	private TextView tempTV;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        RequestedOrientation = ScreenOrientation.Portrait;

        SetContentView(global::EraAndroid64.Resource.Layout.selectfolder);

        lv = FindViewById<ListView>(global::EraAndroid64.Resource.Id.selectFolderLV);
        lv.ItemClick += ItemClick;

		// 디버그 모드를 활성화, 비활성화 할 수 있도록 체크박스를 추가한다
        AddDebugModeCheckBox();

        ArrayAdapter<string> arrayAdapter = new ArrayAdapter<string>(this, 17367043);
        lv.Adapter = arrayAdapter;

        arrayAdapter.Add("SAF로 Emuera 폴더 선택");
        arrayAdapter.Add("선택 후 앱 전용 폴더로 복사하여 실행합니다.");
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
        OpenDocumentTree();
    }

    // Android 저장소 접근 정책에 맞추어 SAF 폴더 선택 화면을 연다.
    private void OpenDocumentTree()
    {
        Intent intent = new Intent(Intent.ActionOpenDocumentTree);

        intent.AddFlags(ActivityFlags.GrantReadUriPermission);
        intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
        intent.AddFlags(ActivityFlags.GrantPersistableUriPermission);
        intent.AddFlags(ActivityFlags.GrantPrefixUriPermission);

        StartActivityForResult(intent, RequestOpenDocumentTree);
    }

    // 선택한 폴더 URI를 저장하고 MainActivity로 전달한다.
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        if (requestCode != RequestOpenDocumentTree || resultCode != Result.Ok || data?.Data == null)
        {
            return;
        }

        Android.Net.Uri selectedUri = data.Data;

        ContentResolver.TakePersistableUriPermission(
            selectedUri,
            ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission
        );

        string selectedUriText = selectedUri.ToString();

        DB.Save("selectedUri", selectedUriText);

        Intent.PutExtra("Selected Uri", selectedUriText);
        SetResult(Result.Ok, Intent);
        Finish();
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
        catch (System.Exception ex2)
        {
            Toast.MakeText(this, ex2.Message, ToastLength.Long).Show();
        }
    }
    public override void OnBackPressed()
    {
        if (pressBackKey)
        {
            // 두 번째 뒤로가기에서 앱을 완전히 종료한다.
            FinishAffinity();
            Java.Lang.JavaSystem.Exit(0);
            return;
        }

        // 첫 번째 뒤로가기에서는 종료하지 않고 사용자에게 한 번 더 눌러야 함을 안내한다.
        Toast.MakeText(this, "한번 더 누르면 종료됩니다", ToastLength.Short).Show();
        pressBackKey = true;
    }
}

