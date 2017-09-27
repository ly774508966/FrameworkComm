using UnityEngine;
using System.Diagnostics;
using System.Threading;
using System;
using System.Text;
using System.IO;

/// <summary>
/// 日志系统
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public class Log
    {
        private static bool bStackTrack = true;

        private static int iMaxLogFrameCount = 1;

        private static int iMaxLogFileSize = 5 * 1024 * 1024;

        private static string sLogFilePath = "";

        private static StringBuilder sStackSB = new StringBuilder();

        private static object writeLock = new object();

        public static int iMaxLastLogSize = 60 * 1024;
        public static int iMaxLogLineSize = 3000;

        public static StringBuilder sLastLogSB = new StringBuilder();

        public static bool bLogDebugEnabled = true;

        public static Action Break = UnityEngine.Debug.Break;

        public static string logFileName
        {
            get
            {
                return sLogFilePath;
            }
            set
            {
#if UNITY_EDITOR
                sLogFilePath = Application.dataPath + "/../Debug/";
                if (!Directory.Exists(sLogFilePath))
                {
                    Directory.CreateDirectory(sLogFilePath);
                }
                sLogFilePath += value;
#elif UNITY_ANDROID
                logFilePath = "/sdcard/" + value;
#elif UNITY_IPHONE
                if (BuildConfig.isReleaseBuild)
                    logFilePath = Application.temporaryCachePath + "/" + value;
                else
                    logFilePath = Application.persistentDataPath + "/" + value;
#endif
            }
        }

        public static void Error(string str)
        {
            PrintLine(LogType.Error, str);
        }

        public static void Debug(string str)
        {
            PrintLine(LogType.Log, str);
        }

        public static void Error(string formatStr, params object[] args)
        {
            PrintLine(LogType.Error, String.Format(formatStr, args));
        }

        public static void DebugFormat(String formatStr, params object[] args)
        {
            PrintLine(LogType.Log, String.Format(formatStr, args));
        }

        public static void HandleException(string logString, string stackTrace, LogType logType)
        {
            string sLogLine = string.Concat(logType.ToString(), " ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), " ", logString, "\n", stackTrace);
            AppendToLastLogs(sLogLine);
            WriteLogToFile(sLogLine);
        }

        private static void PrintLine(LogType logType, string str)
        {
            string sLogLine = "";

            lock (sStackSB)
            {
                sStackSB.Length = 0;

                if (bStackTrack)
                {
                    StackTrace st = new StackTrace(true);

                    int frameIndex = (Application.platform == RuntimePlatform.IPhonePlayer ? 1 : 2);
                    for (int printIndex = 0; printIndex < iMaxLogFrameCount && frameIndex < st.FrameCount - 1; printIndex++, frameIndex++)
                    {
                        StackFrame sf = st.GetFrame(frameIndex);
                        string sFullFileName = sf.GetFileName();
                        string sFileName = "";
                        if (!string.IsNullOrEmpty(sFullFileName))
                        {
                            string[] sFileNameArr = sFullFileName.Split(new char[2] { '\\', '/' });
                            if (sFileNameArr.Length > 0)
                            {
                                sFileName = sFileNameArr[sFileNameArr.Length - 1];
                            }
                        }
                        var mb = sf.GetMethod();
                        sStackSB.Append(string.Concat(
                            printIndex > 0 ? "<-" : "",
                            sFileName, ":",
                            sf.GetFileLineNumber(),
                            ",",
                            mb != null ? mb.ToString() : "Unknown"
                        ));
                    }
                }

                sLogLine = string.Concat(
                    (logType == LogType.Log ? "" : (logType.ToString() + " ")),
                    DateTime.Now.ToString("yy-MM-dd HH:mm:ss.fff"),
                    " (", Thread.CurrentThread.ManagedThreadId.ToString(), ")",
                    " [", sStackSB.ToString(), "] ",
                    str,
                    "\n"
                );
            }

            AppendToLastLogs(sLogLine);

            if (bLogDebugEnabled)
            {
                WriteLogToFile(sLogLine);
#if UNITY_EDITOR
                WriteToConsole(logType, sLogLine);
#endif
            }
        }

        private static void AppendToLastLogs(string sLogLine)
        {
            if (iMaxLastLogSize > 0)
            {
                lock (writeLock)
                {
                    if (sLogLine.Length > iMaxLogLineSize)
                    {
                        sLogLine = sLogLine.Substring(0, iMaxLogLineSize);//每一行有最大的长度限制
                    }

                    sLastLogSB.Append(sLogLine);
                    if (sLastLogSB.Length > iMaxLastLogSize)
                    {
                        sLastLogSB.Remove(0, iMaxLastLogSize / 2);
                    }
                }
            }
        }

        private static void WriteToConsole(LogType logType, string sLogLine)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
			Console.Write(sLogLine);
#else
            if (logType == LogType.Error)
                UnityEngine.Debug.LogError(sLogLine);
            else
                UnityEngine.Debug.Log(sLogLine);
#endif
        }

        private static void CheckFileLength(string logFilePath)
        {
            FileInfo f = new FileInfo(logFilePath);
            if (f.Length > iMaxLogFileSize)
            {
                BinaryReader br = new BinaryReader(File.Open(logFilePath, FileMode.Open, FileAccess.ReadWrite));
                int pos = iMaxLogFileSize / 2;
                br.BaseStream.Seek(pos, SeekOrigin.Begin);
                byte[] by = br.ReadBytes(pos + 1);
                br.Close();

                BinaryWriter bw = new BinaryWriter(File.Open(logFilePath, FileMode.Create, FileAccess.ReadWrite));
                bw.Write(by);
                bw.Close();
            }
        }

        private static void WriteLogToFile(string sLogLine)
        {
            if (sLogFilePath.Length > 0)
            {
                lock (writeLock)
                {
                    try
                    {
                        if (!File.Exists(sLogFilePath))
                        {

                            File.WriteAllText(sLogFilePath, sLogLine);
                        }
                        else
                        {
                            CheckFileLength(sLogFilePath);
                            File.AppendAllText(sLogFilePath, sLogLine);
                        }
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log(e.ToString());
                    }
                    finally { }
                }
            }
        }

        #region Debug with string.Concat
        public static void Debug(string str1, string str2)
        {
            PrintLine(LogType.Log, string.Concat(str1, str2));
        }

        public static void Debug(string str1, string str2, string str3)
        {
            PrintLine(LogType.Log, string.Concat(str1, str2, str3));
        }

        public static void Debug(string str1, string str2, string str3, string str4)
        {
            PrintLine(LogType.Log, string.Concat(str1, str2, str3, str4));
        }

        public static void Debug(string str1, string str2, string str3, string str4, string str5)
        {
            PrintLine(LogType.Log, string.Concat(str1, str2, str3, str4, str5));
        }

        public static void Debug(string str1, string str2, string str3, string str4, string str5, string str6)
        {
            PrintLine(LogType.Log, string.Concat(str1, str2, str3, str4, str5, str6));
        }

        public static void Debug(string str1, string str2, string str3, string str4, string str5, string str6, string str7)
        {
            PrintLine(LogType.Log, string.Concat(str1, str2, str3, str4, str5, str6, str7));
        }

        public static void Debug(string str1, string str2, string str3, string str4, string str5, string str6, string str7, string str8)
        {
            PrintLine(LogType.Log, string.Concat(str1, str2, str3, str4, str5, str6, str7, str8));
        }

        public static void Debug(string str1, string str2, string str3, string str4, string str5, string str6, string str7, string str8, string str9)
        {
            PrintLine(LogType.Log, string.Concat(str1, str2, str3, str4, str5, str6, str7, str8, str9));
        }

        public static void Debug(string str1, string str2, string str3, string str4, string str5, string str6, string str7, string str8, string str9, string str10)
        {
            PrintLine(LogType.Log, string.Concat(str1, str2, str3, str4, str5, str6, str7, str8, str9, str10));
        }

        public static void Debug(string str1, string str2, string str3, string str4, string str5, string str6, string str7, string str8, string str9, string str10, string str11)
        {
            PrintLine(LogType.Log, string.Concat(str1, str2, str3, str4, str5, str6, str7, str8, str9, str10, str11));
        }

        public static void Debug(string str1, string str2, string str3, string str4, string str5, string str6, string str7, string str8, string str9, string str10, string str11, string str12)
        {
            PrintLine(LogType.Log, string.Concat(str1, str2, str3, str4, str5, str6, str7, str8, str9, str10, str11, str12));
        }
        #endregion
    }
}
