using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using EmueraFramework;
using EraAndroid.FrontEnd;
using Java.Lang;
using MinorShift.Emuera;
using Config = MinorShift.Emuera.Config;
using System.Linq;

namespace EraAndroid;

[Activity(Label = "EraAndroid", MainLauncher = true, Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
public class MainActivity : Activity
{
	private const int SELECT_FOLDER_ACTIVITY = 1;

	private const int SET_FONTSIZE_ACTIVITY = 2;

	private EditText inputEditText;

	private Task EmueraInitializeTask;

	private bool pressBackKey;

	public bool EmueraInitializing { get; private set; } = true;

	public void Close()
	{
		GlobalStatic.Console?.Dispose();
		FileLog.Info("Close", "Activity Closed");
		FileLog.Dispose();
		Finish();
	}

	protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
	{
		switch (requestCode)
		{
            case 1:
                if (resultCode == Result.Ok)
                {
                    string stringExtra = data.GetStringExtra("Selected Path");
                    Log.Info("Folder Selected", "selectedPath:" + stringExtra);
                    DB.Save("selectedPath", Directory.GetParent(stringExtra).FullName);
                    Initialize(stringExtra);
                }
                else
                {
                    RunSelectFolderActivity();
                }
                break;
            case 2:
			if (resultCode == Result.Ok)
			{
				Config.FontSize = data.GetIntExtra("TextSize", 42);
				Config.LineHeight = data.GetIntExtra("LineHeight", 54);
                    DB.Save("fontSize", data.GetStringExtra("OriginalTextSize"));
                    RunSelectFolderActivity();
			}
			else
			{
				RunSetFontSizeActivity();
			}
			break;
		}
	}


    private void RunSelectFolderActivity()
    {
        StartActivityForResult(new Intent(this, typeof(SelectFolderActivity)), 1);
    }

    private void RunSetFontSizeActivity()
    {
        StartActivityForResult(new Intent(this, typeof(SetFontActivity)), 2);
    }

    private void Initialize(string eraPath)
    {
        // 구상 초기화 시작 지점을 기록한다.
        FileLog.Info("Initialize", "Start: " + eraPath);

        inputEditText = FindViewById<EditText>(global::EraAndroid64.Resource.Id.inputEditText);
        inputEditText.KeyPress += InputEditText_KeyPress;
		GameData.InputText = inputEditText;
		Program.Main(this, GameData.FrontEnd, eraPath);
		inputEditText.SetBackgroundColor(Config.BackColor);
		inputEditText.SetTextColor(Config.ForeColor);
		EmueraInitializeTask = Task.Run(delegate
		{
			try
			{
				GlobalStatic.Console.Initialize();
			}
            catch (System.Exception ex)
            {
                // 예외 메시지만 저장하면 원인 추적이 어렵기 때문에 전체 스택트레이스를 기록한다.
                FileLog.Error("Initialize Error", ex.ToString());

                // 사용자에게는 간단한 메시지만 표시한다.
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();

                Task.Delay(1500).Wait();
                Finish();
            }
        }).ContinueWith(delegate(Task task)
		{
            if (!task.IsFaulted)
            {
                // 구상 초기화 완료 시점과 메모리 상태를 기록한다.
                FileLog.Info("Initialize", "Completed. Memory: " + (GC.GetTotalMemory(false) / 1024 / 1024) + " MB");

                EmueraInitializing = false;
                GameData.FrontEnd.Handler.Post(GameData.FrontEnd.RequestLayout);
                GC.Collect();
            }
        });
		Task.Run(async delegate
		{
			while (EmueraInitializing)
			{
				GameData.FrontEnd.PostInvalidate();
				await Task.Delay(300);
			}
		});
	}

    protected override void OnCreate(Bundle bundle)
    {
        // 앱 전체에서 처리되지 않은 예외를 로그로 남긴다.
        // 일반 catch에서 잡히지 않는 크래시 원인 추적용이다.
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            FileLog.Error("UnhandledException", e.ExceptionObject.ToString());
        };

        // 비동기 Task 내부에서 발생한 예외를 로그로 남긴다.
        // 초기화나 백그라운드 작업 중 발생하는 예외 추적용이다.
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            FileLog.Error("UnobservedTaskException", e.Exception.ToString());
            e.SetObserved();
        };

        RunSetFontSizeActivity();
        base.OnCreate(bundle);
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            RequestPermissions(new[]
            {
            Android.Manifest.Permission.ReadExternalStorage,
            Android.Manifest.Permission.WriteExternalStorage
        }, 100);
        }
        RequestedOrientation = ScreenOrientation.Portrait;
        SetContentView(global::EraAndroid64.Resource.Layout.main); GameData.MainActivity = this;
        GameData.FrontEnd = FindViewById<EmueraFrontEnd>(global::EraAndroid64.Resource.Id.emueraConsole);
        GameData.ScrollView = FindViewById<ScrollView>(global::EraAndroid64.Resource.Id.emueraScrollView);

        // 디버그 모드가 켜져 있을 때만 게임 화면에 DEBUG 버튼을 표시한다.
        if (DB.Load("debugMode") == "true")
        {
            AddDebugButton();
        }
    }

    // dp 단위를 실제 픽셀로 변환하는 함수
    // 다양한 해상도에서 동일한 크기로 표시되도록 하기 위함
    private int Dp(float value)
    {
        return (int)TypedValue.ApplyDimension(
            ComplexUnitType.Dip,
            value,
            Resources.DisplayMetrics
        );
    }

    // 화면 우측 상단에 디버그 버튼을 추가하는 메서드
    // 게임 입력 처리와 분리되어 있어 기존 로직에 영향을 주지 않는다
    private void AddDebugButton()
    {
        Button debugButton = new Button(this);

        // 버튼 텍스트
        debugButton.Text = "DEBUG";

        // 글자 색상 (흰색)

        debugButton.SetTextColor(Android.Graphics.Color.White);

        // 배경 투명
        debugButton.SetBackgroundColor(Android.Graphics.Color.Transparent);

        // 버튼 클릭 시 현재 상태 정보를 표시
        debugButton.Click += (sender, e) =>
        {
            long memoryMb = GC.GetTotalMemory(false) / 1024 / 1024;

            string selectedPath = DB.Load("selectedPath");

            string appVersion = PackageManager
                .GetPackageInfo(PackageName, 0)
                .VersionName;

            // 현재 상태 출력
            string debugInfo =
                $"Version: {appVersion}\n" +
                $"Memory: {memoryMb} MB\n" +
                $"SDK: {Build.VERSION.SdkInt}\n" +
                $"Path: {selectedPath}\n" +
                $"Emuera: {GlobalStatic.FrontEnd?.InternalEmueraVer}\n" +
                $"Log: {FileLog.LogFilePath}";

            // 사용자가 DEBUG 버튼을 누른 시점의 상태를 로그에도 남긴다.
            // 버그 제보 시 해당 시점의 메모리, 경로, 버전 정보를 확인하기 위함이다.
            FileLog.Info("DebugInfo", debugInfo);

            Toast.MakeText(
                this,
                debugInfo,
                ToastLength.Long
            ).Show();

            // 로그 파일 공유
            try
            {
                string logText = "";

                // 로그 파일이 있으면 내용을 읽어온다.
                if (File.Exists(FileLog.LogFilePath))
                {
                    logText = File.ReadAllText(FileLog.LogFilePath);

                    // 로그가 너무 길면 공유 앱이 멈출 수 있으므로 마지막 20000자만 공유한다.
                    if (logText.Length > 20000)
                    {
                        logText = logText.Substring(logText.Length - 20000);
                    }
                }
                else
                {
                    logText = "로그 파일이 존재하지 않습니다.";
                }

                // 로그 내용을 텍스트로 공유한다.
                var intent = new Intent(Intent.ActionSend);
                intent.SetType("text/plain");
                intent.PutExtra(Intent.ExtraSubject, "EraAndroid Debug Log");
                intent.PutExtra(Intent.ExtraText, logText);

                StartActivity(Intent.CreateChooser(intent, "로그 내용 공유"));
            }

            catch (System.Exception ex)
            {
                Toast.MakeText(this, $"공유 실패: {ex.Message}", ToastLength.Long).Show();
            }
        };

        // 버튼 위치 및 크기 설정
        FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(
            Dp(64),   // 너비
            Dp(32)    // 높이
        );
        debugButton.TextSize = 8;

        // 화면 우측 상단에 배치
        layoutParams.Gravity = GravityFlags.Top | GravityFlags.Right;

        // 여백 설정
        layoutParams.TopMargin = Dp(8);
        layoutParams.RightMargin = Dp(8);

        // 현재 화면 위에 버튼을 오버레이로 추가
        AddContentView(debugButton, layoutParams);
    }

    protected override void OnDestroy()
	{
		GlobalStatic.Console?.Dispose();
		base.OnDestroy();
		FileLog.Dispose();
	}

	private void InputEditText_KeyPress(object sender, View.KeyEventArgs e)
	{
		if (e.Event.Action != KeyEventActions.Down)
		{
			return;
		}
		FileLog.Info("KeyInput", string.Format("InputEditText OnKeyPress {0}:{1}", "KeyCode", e.KeyCode.ToString()));
		Keycode keyCode = e.KeyCode;
		if (e.KeyCode == Keycode.Back)
		{
			if (pressBackKey)
			{
				FinishAffinity();
				JavaSystem.Exit(0);
			}
			Toast.MakeText(this, "한번 더 누르면 종료됩니다", ToastLength.Short);
			pressBackKey = true;
			return;
		}
		if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.NumpadEnter)
		{
			GameData.FrontEnd.Input = inputEditText.Text;
			inputEditText.Text = "";
		}
		else if (keyCode == Keycode.Menu)
		{
			OpenOptionsMenu();
		}
		inputEditText.OnKeyDown(e.KeyCode, e.Event);
		pressBackKey = false;
	}
}
