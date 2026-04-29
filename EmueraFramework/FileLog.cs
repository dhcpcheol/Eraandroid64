using System;
using System.IO;
using Android.Util;

namespace EmueraFramework;

public static class FileLog
{
	private static StreamWriter logWriter;

	private static string currentTime => DateTime.Now.ToString("yyyy/MM/dd hh:mm.sss");

	public static void Init(string eraPath)
	{
		logWriter = new StreamWriter(new FileStream(eraPath + "/AndroidLog.txt", File.Exists(eraPath + "/AndroidLog.txt") ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
	}

	public static void Debug(string tag, string msg)
	{
		Log.Debug(tag, msg);
		logWriter.WriteLine("[Debug]");
		WriteLog(tag, msg);
	}

	public static void Info(string tag, string msg)
	{
		Log.Info(tag, msg);
		logWriter.WriteLine("[Info]");
		WriteLog(tag, msg);
	}

	public static void Warn(string tag, string msg)
	{
		Log.Warn(tag, msg);
		logWriter.WriteLine("[Warn]");
		WriteLog(tag, msg);
	}

	public static void Error(string tag, string msg)
	{
		Log.Error(tag, msg);
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
		logWriter.Dispose();
	}
}
