using Android.Util;
using EraAndroid;
using System;
using System.IO;

namespace EmueraFramework;

public static class FileLog
{
    private static StreamWriter logWriter;

    // 로그 파일 경로를 외부에서 확인하기 위한 변수
    public static string LogFilePath { get; private set; }

    private static string currentTime => DateTime.Now.ToString("yyyy/MM/dd hh:mm.sss");

    public static void Init(string eraPath)
    {
        // targetSdkVersion 34에서는 일반 외부 저장소 경로에 직접 쓰기가 제한된다.
        // 로그는 앱 전용 외부 저장소에 저장하여 권한 문제를 피한다. 차후 SAF 채택시 변경
        string logDir = GameData.MainActivity.GetExternalFilesDir(null).AbsolutePath;

        LogFilePath = Path.Combine(logDir, "AndroidLog.txt");

        logWriter = new StreamWriter(new FileStream(
            LogFilePath,
            File.Exists(LogFilePath) ? FileMode.Append : FileMode.Create,
            FileAccess.Write,
            FileShare.ReadWrite
        ));
    }

    public static void Debug(string tag, string msg)
    {
        Log.Debug(tag, msg);

        // 초기화 전 호출 방지
        if (logWriter == null)
        {
            return;
        }

        logWriter.WriteLine("[Debug]");
        WriteLog(tag, msg);
    }

    public static void Info(string tag, string msg)
    {
        Log.Info(tag, msg);

        if (logWriter == null)
        {
            return;
        }

        logWriter.WriteLine("[Info]");
        WriteLog(tag, msg);
    }

    public static void Warn(string tag, string msg)
    {
        Log.Warn(tag, msg);

        if (logWriter == null)
        {
            return;
        }

        logWriter.WriteLine("[Warn]");
        WriteLog(tag, msg);
    }

    public static void Error(string tag, string msg)
    {
        Log.Error(tag, msg);

        // 로그 파일이 아직 초기화되지 않은 상태에서도 앱이 종료되는 현상을 방지
        if (logWriter == null)
        {
            return;
        }

        logWriter.WriteLine("[Error]");
        WriteLog(tag, msg);
    }

    private static void WriteLog(string tag, string msg)
    {
        logWriter.WriteLine(currentTime);
        logWriter.Write(tag + " : ");
        logWriter.WriteLine(msg);
        logWriter.Flush();
    }

    public static void Dispose()
    {
        if (logWriter != null)
        {
            logWriter.Dispose();
        }
    }
}