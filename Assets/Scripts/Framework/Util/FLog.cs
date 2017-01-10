using UnityEngine;
using System.Diagnostics;
using System.Threading;
using System;
using System.Text;
using System.IO;

namespace Framework
{
    /// <summary>
    /// 日志系统
    /// </summary>
    public class FLog
    {
        private static bool bStackTrack = true;

        private static int mMaxLogFrameCount = 3;

        private static int mMaxLogFileSize = 5 * 1024 * 1024;

        private static StringBuilder sStackSB = new StringBuilder();

        private static object writeLock = new object();

        public static int maxLogFrameCount
        {
            get { return mMaxLogFrameCount; }
            set { mMaxLogFrameCount = value; }
        }

        //默认保留最好60K的日志
        public static int lMaxLastLogSize = 60 * 1024;
        public static int lMaxLogLineSize = 3000;
        public static StringBuilder lastLogs = new StringBuilder();

        public static string logFilePath
        {
            get
            {
                string path = PathConfig.Debug + "TikiAL.log";
                return path;
            }
        }

        public delegate void BreakFunc();
        public static BreakFunc Break = UnityEngine.Debug.Break;

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
            PrintLine(LogType.Error, string.Format(formatStr, args));
        }

        public static void DebugFormat(string formatStr, params object[] args)
        {
            PrintLine(LogType.Log, string.Format(formatStr, args));
        }

        public static void HandleException(string logString, string stackTrace, LogType logType)
        {
            string sLogLine = string.Concat(logType.ToString(), " ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), " ", logString, "\n", stackTrace);
            AppendToLastLogs(sLogLine);
            WriteLogToFile(sLogLine);
        }

        private static void PrintLine(LogType logType, string str)
        {
            //http://bobondevelopment.com/2007/06/11/three-ways-to-clear-a-stringbuilder/
            sStackSB.Length = 0;

            if (bStackTrack)
            {
                StackTrace st = new StackTrace(true);

                int frameIndex = 2;
                for (int printIndex = 0; printIndex < mMaxLogFrameCount && frameIndex < st.FrameCount - 1; printIndex++, frameIndex++)
                {
                    StackFrame sf = st.GetFrame(frameIndex);
                    string sFileName = sf.GetFileName();
                    if (sFileName != null)
                    {
                        string[] sFileNameArr = sFileName.Split(new char[2] { '\\', '/' });
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
                        mb != null ? mb.ToString() : "unknow"
                    ));
                }
            }

            string sLogLine = string.Concat(
                (logType == LogType.Log ? "" : (logType.ToString() + " ")),
                DateTime.Now.ToString("yy-MM-dd HH:mm:ss.fff"),
                " (", Thread.CurrentThread.ManagedThreadId, ")",
                " [", sStackSB, "] ",
                str,
                "\n"
            );

            AppendToLastLogs(sLogLine);

            if (GameConfig.IsLogEnable)
            {
                WriteLogToFile(sLogLine);
                WriteToConsole(sLogLine);
            }
        }

        private static void AppendToLastLogs(string sLogLine)
        {
            if (lMaxLastLogSize > 0)
            {
                lock (writeLock)
                {
                    if (sLogLine.Length > lMaxLogLineSize)
                    {//每一行有最大的长度限制
                        sLogLine = sLogLine.Substring(0, lMaxLogLineSize);
                    }
                    lastLogs.Append(sLogLine);
                    if (lastLogs.Length > lMaxLastLogSize)
                    {
                        lastLogs.Remove(0, lMaxLastLogSize / 2);
                    }
                }
            }
        }

        private static void WriteToConsole(string sLogLine)
        {
            UnityEngine.Debug.Log(sLogLine);
        }

        private static void CheckFileLength(string logFilePath)
        {
            FileInfo f = new FileInfo(logFilePath);
            if (f.Length > mMaxLogFileSize)
            {
                BinaryReader br = new BinaryReader(File.Open(logFilePath, FileMode.Open, FileAccess.ReadWrite));
                int pos = mMaxLogFileSize / 2;
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
            if (logFilePath.Length > 0)
            {
                lock (writeLock)
                {
                    try
                    {
                        if (!File.Exists(logFilePath))
                        {

                            File.WriteAllText(logFilePath, sLogLine);
                        }
                        else
                        {
                            CheckFileLength(logFilePath);
                            File.AppendAllText(logFilePath, sLogLine);
                        }
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log(e.ToString());
                    }
                }
            }
        }

        #region Debug With string.Concat
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