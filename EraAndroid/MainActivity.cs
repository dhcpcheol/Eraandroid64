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
using AndroidX.DocumentFile.Provider;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EraAndroid;

[Activity(Label = "EraAndroid", MainLauncher = true, Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
public class MainActivity : Activity
{
	private const int SELECT_FOLDER_ACTIVITY = 1;

	private const int SET_FONTSIZE_ACTIVITY = 2;

    private EditText inputEditText;

    private Task EmueraInitializeTask;

    // 실제 구상 화면 레이아웃이 로드되었는지 확인하기 위한 플래그이다.
    private bool mainLayoutLoaded;

    private bool pressBackKey;

    // 파일 확인, 파일 복사, 초기화 상태를 화면에 표시하기 위한 텍스트이다.
    private TextView loadingStatusTextView;

    // SAF 전체 복사 중 복사한 파일 수를 기록한다.
    private int startupCopiedFileCount;

    // 로딩 상태 UI가 마지막으로 갱신된 시간을 기록한다.
    // 파일을 하나 읽을 때마다 화면과 로그를 갱신하면 초기화 속도가 느려질 수 있으므로 갱신 빈도를 제한한다.
    private long lastLoadingStatusUpdateTime;

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
                    string selectedUri = data.GetStringExtra("Selected Uri");

                    Log.Info("Folder Selected", "selectedUri: " + selectedUri);

                    if (string.IsNullOrEmpty(selectedUri))
                    {
                        Toast.MakeText(this, "선택된 폴더 URI가 없습니다.", ToastLength.Long).Show();
                        RunSelectFolderActivity();
                        return;
                    }

                    DB.Save("selectedUri", selectedUri);

                    // SAF 폴더 선택이 끝난 뒤에는 복사 진행 중에도 메인 화면이 보이도록 한다.
                    // 이 시점에 main.xml을 로드해야 복사 및 초기화 상태를 화면에 표시할 수 있다.
                    EnsureMainLayoutLoaded();

                    // 복사와 초기화 상태를 화면에 표시할 수 있도록 콜백을 먼저 연결한다.
                    EraAndroidFileProvider.StatusCallback = ShowLoadingStatus;

                    string copiedPathCandidate = GetCopiedPathFromSelectedUri(selectedUri);

                    if (IsValidCopiedFolder(copiedPathCandidate))
                    {
                        new AlertDialog.Builder(this)
                            .SetTitle("기존 Era 복사본 발견")
                            .SetMessage("기존에 복사된 Era 폴더가 있습니다.\n\n기존 복사본을 사용하면 빠르게 실행할 수 있습니다.\nEra 파일을 수정했다면 다시 불러오기를 선택하세요.")
                            .SetPositiveButton("기존 복사본 사용", (sender, args) =>
                            {
                                StartCopyAndInitialize(selectedUri, false);
                            })
                            .SetNegativeButton("다시 불러오기", (sender, args) =>
                            {
                                StartCopyAndInitialize(selectedUri, true);
                            })
                            .Show();

                        return;
                    }

                    StartCopyAndInitialize(selectedUri, false);
                }
                else
                {
                    RunSelectFolderActivity();
                }
                break;
            case 2:
                if (resultCode == Result.Ok)
                {
                    int textSize = data.GetIntExtra("TextSize", 42);
                    int lineHeight = data.GetIntExtra("LineHeight", 54);

                    Config.FontSize = textSize;
                    Config.LineHeight = lineHeight;

                    // 실제 적용된 글자 크기 값을 저장한다.
                    DB.Save("fontSize", textSize.ToString());
                    DB.Save("lineHeight", lineHeight.ToString());

                    FileLog.Info("Font", "Saved FontSize: " + textSize + ", LineHeight: " + lineHeight);

                    RunSelectFolderActivity();
                }
                else
                {
                    // 글자 크기 설정을 취소한 경우에도 기본값 또는 기존 저장값으로 진행한다.
                    RunSelectFolderActivity();
                }
                break;
        }
    }

    private string GetCopiedPathFromSelectedUri(string selectedUri)
    {
        // SAF URI 문자열을 기준으로 기존 복사 폴더 경로를 계산한다.
        string appRootPath = GetExternalFilesDir(null).AbsolutePath;
        string cacheFolderName = GetStableCacheFolderName(selectedUri);

        return Path.Combine(appRootPath, cacheFolderName);
    }

    private bool IsValidCopiedFolder(string copiedPath)
    {
        // 기존 복사 폴더가 정상 Era 폴더인지 확인한다.
        if (string.IsNullOrEmpty(copiedPath))
        {
            return false;
        }

        if (!Directory.Exists(copiedPath))
        {
            return false;
        }

        bool csvExists = Directory.Exists(Path.Combine(copiedPath, "CSV"));
        bool erbExists = Directory.GetFiles(copiedPath, "*.ERB", SearchOption.AllDirectories).Length > 0;

        return csvExists && erbExists;
    }

    private void StartCopyAndInitialize(string selectedUri, bool forceRecopy)
    {
        ShowLoadingStatus("Era 폴더 복사를 준비합니다.");
        Toast.MakeText(this, "SAF 폴더 선택 완료. Era 폴더를 복사합니다.", ToastLength.Long).Show();

        FileLog.Info("SAF", "Copy Start: " + selectedUri);
        FileLog.Info("SAF", "Force Recopy: " + forceRecopy);

        Task.Run(() =>
        {
            try
            {
                // SAF URI는 기존 엔진에서 직접 읽을 수 없으므로 앱 전용 폴더로 복사한 뒤 실행한다.
                // forceRecopy가 false이면 정상 캐시가 있을 때 기존 복사본을 재사용한다.
                // forceRecopy가 true이면 기존 복사본을 삭제하고 SAF 원본에서 다시 복사한다.
                string copiedPath = CopySelectedTreeToAppFolder(selectedUri, forceRecopy);

                FileLog.Info("SAF", "Copied Path: " + copiedPath);

                RunOnUiThread(() =>
                {
                    if (string.IsNullOrEmpty(copiedPath) || !Directory.Exists(copiedPath))
                    {
                        Toast.MakeText(this, "Era 폴더 복사에 실패했습니다.", ToastLength.Long).Show();
                        RunSelectFolderActivity();
                        return;
                    }

                    DB.Save("selectedPath", copiedPath);

                    // 필요한 파일만 나중에 SAF 원본에서 가져올 수 있도록 Provider에 현재 경로 정보를 전달한다.
                    EraAndroidFileProvider.SelectedUri = Android.Net.Uri.Parse(selectedUri);
                    EraAndroidFileProvider.LocalRootPath = copiedPath;
                    EraAndroidFileProvider.AppContext = this;

                    Toast.MakeText(this, "Era 폴더 복사 완료. 초기화를 시작합니다.", ToastLength.Long).Show();
                    ShowLoadingStatus("Era 초기화 준비 중입니다.");

                    Initialize(copiedPath);
                });
            }
            catch (System.Exception ex)
            {
                FileLog.Error("SAF Copy Error", ex.ToString());

                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "Era 폴더 복사 중 오류가 발생했습니다: " + ex.Message, ToastLength.Long).Show();
                    RunSelectFolderActivity();
                });
            }
        });
    }

    private string CopySelectedTreeToAppFolder(string selectedUri, bool forceRecopy)
    {
        // SAF로 선택된 폴더를 앱 전용 외부 디렉터리로 복사한다.
        string appRootPath = GetExternalFilesDir(null).AbsolutePath;

        // 선택한 SAF URI마다 서로 다른 복사 폴더를 사용한다.
        // GetHashCode는 실행마다 달라질 수 있으므로 안정적인 SHA256 기반 폴더명을 사용한다.
        string cacheFolderName = GetStableCacheFolderName(selectedUri);

        string copiedPath = Path.Combine(appRootPath, cacheFolderName);

        FileLog.Info("SAF", "Selected Uri: " + selectedUri);
        FileLog.Info("SAF", "App Root Path: " + appRootPath);
        FileLog.Info("SAF", "Target Copy Path: " + copiedPath);

        Android.Net.Uri treeUri = Android.Net.Uri.Parse(selectedUri);
        DocumentFile rootDocument = DocumentFile.FromTreeUri(this, treeUri);

        if (rootDocument == null || !rootDocument.Exists() || !rootDocument.IsDirectory)
        {
            FileLog.Error("SAF", "선택된 SAF 폴더를 열 수 없습니다.");
            return "";
        }

        // 이미 앱 전용 폴더에 복사된 Era 데이터가 존재하는지 확인한다.
        if (Directory.Exists(copiedPath))
        {
            // CSV 폴더 존재 여부를 확인한다.
            // Era 실행에 필수적인 데이터이므로 존재 여부를 기준으로 판단한다.
            bool csvExists = Directory.Exists(Path.Combine(copiedPath, "CSV"));

            // ERB 파일 존재 여부를 확인한다.
            // ERB는 Era 스크립트 파일이므로 최소 1개 이상 존재해야 정상이다.
            bool erbExists = Directory.GetFiles(copiedPath, "*.ERB", SearchOption.AllDirectories).Length > 0;

            // CSV 폴더와 ERB 파일이 모두 존재하는 경우,
            // 이미 정상적으로 복사가 완료된 것으로 판단한다.
            if (csvExists && erbExists)
            {
                // 강제 재복사가 필요하지 않으면 기존 복사 폴더를 재사용한다.
                // 앱 실행 중 같은 Era을 다시 사용할 때 전체 복사를 반복하지 않기 위한 처리이다.
                if (!forceRecopy)
                {
                    FileLog.Info("SAF", "기존 복사 폴더를 재사용한다: " + copiedPath);
                    return copiedPath;
                }

                // 사용자가 폴더를 다시 선택한 경우 최신 원본을 반영하기 위해 기존 복사 폴더를 삭제한다.
                FileLog.Info("SAF", "폴더가 다시 선택되어 기존 복사 폴더를 삭제하고 재복사한다: " + copiedPath);
                Directory.Delete(copiedPath, true);
            }
            else
            {
                // 일부 파일이 누락된 경우, 이전 복사가 불완전하다고 판단한다.
                // 이 경우 기존 폴더를 삭제하고 다시 복사를 수행한다.
                FileLog.Info("SAF", "기존 복사 폴더가 불완전하여 재복사를 수행한다: " + copiedPath);
                Directory.Delete(copiedPath, true);
            }
        }

        // 복사 대상 디렉터리를 생성한다.
        Directory.CreateDirectory(copiedPath);

        // SAF로 선택된 폴더의 전체 구조를 재귀적으로 복사한다.
        startupCopiedFileCount = 0;
        ShowLoadingStatus("Era 폴더 복사 중입니다.");
        FileLog.Info("SAF", "Era 폴더 전체 복사 시작: " + copiedPath);

        CopyDocumentTreeRecursive(rootDocument, copiedPath);

        FileLog.Info("SAF", "Era 폴더 전체 복사 완료: " + copiedPath);

        FileLog.Info("SAF", "Copy Completed: " + copiedPath);
        FileLog.Info("SAF", "CSV Exists: " + Directory.Exists(Path.Combine(copiedPath, "CSV")));
        FileLog.Info("SAF", "ERB Count: " + Directory.GetFiles(copiedPath, "*.ERB", SearchOption.AllDirectories).Length);
        FileLog.Info("SAF", "CSV Count: " + Directory.GetFiles(copiedPath, "*.CSV", SearchOption.AllDirectories).Length);

        return copiedPath;
    }

    private void CopyDocumentTreeRecursive(DocumentFile sourceDirectory, string targetDirectory)
    {
        // SAF 폴더 내부의 모든 파일과 하위 폴더를 기존 파일 경로 구조로 복사한다.
        foreach (DocumentFile document in sourceDirectory.ListFiles())
        {
            string safeName = GetSafeFileName(document.Name);

            if (string.IsNullOrEmpty(safeName))
            {
                continue;
            }

            string targetPath = Path.Combine(targetDirectory, safeName);

            if (document.IsDirectory)
            {
                Directory.CreateDirectory(targetPath);
                CopyDocumentTreeRecursive(document, targetPath);
            }
            else if (document.IsFile)
            {
                // 초기 실행에 필요한 파일만 먼저 복사한다.
                // 이미지, 사운드, 동영상 등은 실제로 요청될 때 EraAndroidFileProvider에서 가져오도록 한다.
                if (ShouldCopyAtStartup(targetPath))
                {
                    CopyDocumentFile(document, targetPath);
                }
            }
        }
    }

    private bool ShouldCopyAtStartup(string targetPath)
    {
        string extension = Path.GetExtension(targetPath).ToUpperInvariant();

        // Emuera 초기화와 스크립트 파싱에 필요한 텍스트 계열 파일이다.
        if (extension == ".ERB")
        {
            return true;
        }

        if (extension == ".CSV")
        {
            return true;
        }

        if (extension == ".ERH")
        {
            return true;
        }

        if (extension == ".CONFIG")
        {
            return true;
        }

        if (extension == ".TXT")
        {
            return true;
        }

        return false;
    }

    private void CopyDocumentFile(DocumentFile sourceFile, string targetPath)
    {
        // SAF 파일 스트림을 열어 앱 전용 디렉터리의 실제 파일로 저장한다.
        using Stream inputStream = ContentResolver.OpenInputStream(sourceFile.Uri);
        using FileStream outputStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024);
        inputStream.CopyTo(outputStream, 1024 * 1024);

        startupCopiedFileCount++;

        if (startupCopiedFileCount == 1 || startupCopiedFileCount % 25 == 0)
        {
            ShowLoadingStatus("Era 파일 복사 중: " + startupCopiedFileCount + "개 복사");
        }
    }

    private string GetStableCacheFolderName(string selectedUri)
    {
        // SAF URI 문자열을 기준으로 항상 같은 캐시 폴더명을 생성한다.
        using SHA256 sha256 = SHA256.Create();

        byte[] inputBytes = Encoding.UTF8.GetBytes(selectedUri);
        byte[] hashBytes = sha256.ComputeHash(inputBytes);

        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        for (int i = 0; i < 8; i++)
        {
            builder.Append(hashBytes[i].ToString("x2"));
        }

        return "Emuera_" + builder.ToString();
    }

    private string GetSafeFileName(string fileName)
    {
        // Android 문서 이름에 포함될 수 있는 잘못된 문자를 파일 시스템에 맞게 정리한다.
        if (string.IsNullOrEmpty(fileName))
        {
            return "";
        }

        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidChar, '_');
        }

        return fileName;
    }

    private void ApplySavedFontConfig()
    {
        // 저장된 폰트 크기를 불러와 실제 Era 화면 설정에 적용한다.
        string savedFontSize = DB.Load("fontSize");
        string savedLineHeight = DB.Load("lineHeight");

        if (int.TryParse(savedFontSize, out int fontSize))
        {
            Config.FontSize = fontSize;
        }

        if (int.TryParse(savedLineHeight, out int lineHeight))
        {
            Config.LineHeight = lineHeight;
        }
        else if (int.TryParse(savedFontSize, out int fallbackFontSize))
        {
            // 줄 높이가 저장되어 있지 않은 경우 글자 크기를 기준으로 계산한다.
            Config.LineHeight = (int)(fallbackFontSize * 1.3f);
        }

        FileLog.Info("Font", "Applied FontSize: " + Config.FontSize + ", LineHeight: " + Config.LineHeight);
    }

    private void RunSelectFolderActivity()
    {
        StartActivityForResult(new Intent(this, typeof(SelectFolderActivity)), 1);

        // 경로 선택 화면으로 전환할 때 MainActivity가 순간적으로 보이지 않도록 전환 애니메이션을 제거한다.
        OverridePendingTransition(0, 0);
    }

    private void RunSetFontSizeActivity()
    {
        StartActivityForResult(new Intent(this, typeof(SetFontActivity)), 2);

        // 폰트 설정 화면으로 전환할 때 MainActivity가 순간적으로 보이지 않도록 전환 애니메이션을 제거한다.
        OverridePendingTransition(0, 0);
    }

    private void ShowLoadingStatus(string message)
    {
        long currentTime = JavaSystem.CurrentTimeMillis();

        bool importantMessage =
            message.Contains("준비") ||
            message.Contains("완료") ||
            message.Contains("복사 중") ||
            message.Contains("초기화");

        // 중요한 상태 메시지는 즉시 표시한다.
        // 그 외의 파일 단위 로딩 메시지는 250ms에 한 번만 표시하여 UI 갱신과 로그 기록 부담을 줄인다.
        if (!importantMessage && currentTime - lastLoadingStatusUpdateTime < 250)
        {
            return;
        }

        lastLoadingStatusUpdateTime = currentTime;

        RunOnUiThread(() =>
        {
            FileLog.Info("LoadingStatus", message);

            if (loadingStatusTextView == null)
            {
                loadingStatusTextView = new TextView(this);
                loadingStatusTextView.TextSize = 16;
                loadingStatusTextView.SetTextColor(Android.Graphics.Color.White);
                loadingStatusTextView.SetBackgroundColor(Android.Graphics.Color.Argb(220, 0, 0, 0));
                loadingStatusTextView.Gravity = GravityFlags.Center;
                loadingStatusTextView.SetPadding(Dp(12), Dp(8), Dp(12), Dp(8));

                FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(
                    FrameLayout.LayoutParams.MatchParent,
                    Dp(96)
                );

                layoutParams.Gravity = GravityFlags.Bottom;

                AddContentView(loadingStatusTextView, layoutParams);
            }

            loadingStatusTextView.Text = message;
            loadingStatusTextView.Visibility = ViewStates.Visible;

            // 화면 위에 상태 표시창이 보이도록 맨 앞으로 올린다.
            loadingStatusTextView.BringToFront();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                loadingStatusTextView.Elevation = Dp(100);
            }

            loadingStatusTextView.Invalidate();
            Window.DecorView.Invalidate();
        });
    }

    private void HideLoadingStatus()
    {
        RunOnUiThread(() =>
        {
            if (loadingStatusTextView != null)
            {
                loadingStatusTextView.Visibility = ViewStates.Gone;
            }
        });
    }

    private void EnsureMainLayoutLoaded()
    {
        // Era 실행 직전에만 실제 메인 화면 레이아웃을 로드한다.
        if (mainLayoutLoaded)
        {
            return;
        }

        SetContentView(global::EraAndroid64.Resource.Layout.main);

        GameData.MainActivity = this;
        GameData.FrontEnd = FindViewById<EmueraFrontEnd>(global::EraAndroid64.Resource.Id.emueraConsole);
        GameData.ScrollView = FindViewById<ScrollView>(global::EraAndroid64.Resource.Id.emueraScrollView);

        // 디버그 모드가 켜져 있으면 Era 화면 위에 DEBUG 버튼을 표시한다.
        if (DB.Load("debugMode") == "true")
        {
            AddDebugButton();
        }

        mainLayoutLoaded = true;
    }

    private void Initialize(string eraPath)
    {
        // 실제 Era 초기화가 시작될 때 메인 화면 레이아웃을 로드한다.
        EnsureMainLayoutLoaded();

        // Era 초기화 시작 지점을 기록한다.
        FileLog.Info("Initialize", "Start: " + eraPath);

        // 저장된 폰트 크기를 불러와 실제 Era 화면 설정에 적용한다.
        string savedFontSize = DB.Load("fontSize");

        if (int.TryParse(savedFontSize, out int fontSize))
        {
            Config.FontSize = fontSize;

            // 줄 높이는 글자 크기에 맞추어 자동으로 계산한다.
            Config.LineHeight = (int)(fontSize * 1.3f);

            FileLog.Info("Font", "Applied FontSize: " + Config.FontSize + ", LineHeight: " + Config.LineHeight);
        }
        else
        {
            FileLog.Info("Font", "저장된 폰트 크기가 없어 기존 설정값을 사용한다.");
        }

        inputEditText = FindViewById<EditText>(global::EraAndroid64.Resource.Id.inputEditText);
        inputEditText.KeyPress += InputEditText_KeyPress;
        GameData.InputText = inputEditText;

        // Program.Main 실행 전에 저장된 폰트 설정을 먼저 적용한다.
        ApplySavedFontConfig();

        // 초기화 상태 표시창이 정상적으로 표시되는지 확인하기 위한 문구이다.
        ShowLoadingStatus("Era 파일 읽기 준비 중입니다.");

        MinorShift.Emuera.Program.Main(this, GameData.FrontEnd, eraPath);

        // Program.Main 내부에서 Config 값이 다시 변경될 수 있으므로 실행 후에도 다시 적용한다.
        ApplySavedFontConfig();

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
                FileLog.Error("Initialize Error", ex.ToString());
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                Task.Delay(1500).Wait();
                Finish();
            }
        }).ContinueWith(delegate (Task task)
        {
            if (!task.IsFaulted)
            {
                FileLog.Info("Initialize", "Completed. Memory: " + (GC.GetTotalMemory(false) / 1024 / 1024) + " MB");

                HideLoadingStatus();

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
        base.OnCreate(bundle);

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

        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            RequestPermissions(new[]
            {
                Android.Manifest.Permission.ReadExternalStorage,
                Android.Manifest.Permission.WriteExternalStorage
            }, 100);
        }

        RequestedOrientation = ScreenOrientation.Portrait;

        // 폰트 설정과 경로 선택이 끝나기 전에는 Era 화면 레이아웃을 로드하지 않는다.
        // Activity 전환 사이에 "메모리 초기화중" 화면이 잠깐 보이는 현상을 막기 위한 처리이다.
        RunSetFontSizeActivity();
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
                    string fullLogText = File.ReadAllText(FileLog.LogFilePath);

                    // 공유 인텐트에 너무 큰 텍스트를 넣으면 공유 창이 뜨지 않을 수 있으므로,
                    // SAF 복사와 초기화 확인에 필요한 로그만 추출한다.
                    string[] logLines = fullLogText.Split(new[]
                    {
                        "\r\n",
                        "\n"
                    }, StringSplitOptions.None);

                    List<string> filteredLogLines = new List<string>();

                    foreach (string logLine in logLines)
                    {
                        if (logLine.Contains("SAF") ||
                            logLine.Contains("복사한 파일 수") ||
                            logLine.Contains("건너뛴 파일 수") ||
                            logLine.Contains("Initialize") ||
                            logLine.Contains("ReadFile") ||
                            logLine.Contains("ERB Load") ||
                            logLine.Contains("LoadingStatus") ||
                            logLine.Contains("Font") ||
                            logLine.Contains("DebugInfo"))
                        {
                            filteredLogLines.Add(logLine);
                        }
                    }

                    logText = string.Join("\n", filteredLogLines);

                    // 그래도 로그가 너무 길면 마지막 50000자만 공유한다.
                    if (logText.Length > 50000)
                    {
                        logText = logText.Substring(logText.Length - 50000);
                    }

                    if (string.IsNullOrEmpty(logText))
                    {
                        logText = "필터링된 로그가 없습니다.";
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

            Toast.MakeText(this, "한번 더 누르면 종료됩니다", ToastLength.Short).Show();
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
