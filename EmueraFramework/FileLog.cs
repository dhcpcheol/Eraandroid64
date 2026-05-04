using System;
using System.IO;
using Android.Util;

namespace EmueraFramework;

public static class FileLog
{
    private static StreamWriter logWriter;

    // 로그 파일 경로를 외부에서 확인하기 위한 변수
    public static string LogFilePath { get; private set; }

    private static string currentTime => DateTime.Now.ToString("yyyy/MM/dd hh:mm.sss");

    public static void Init(string eraPath)
    {
        // 로그 파일 경로 설정
        LogFilePath = eraPath + "/AndroidLog.txt";

        // 로그 파일 생성 또는 이어쓰기
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