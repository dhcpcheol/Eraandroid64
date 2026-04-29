using System;
using System.IO;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

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
