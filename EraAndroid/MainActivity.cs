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
				FileLog.Error("Initialize Error", ex.Message);
				Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
				Task.Delay(1500).Wait();
				Finish();
			}
		}).ContinueWith(delegate(Task task)
		{
			if (!task.IsFaulted)
			{
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
