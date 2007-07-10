using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Structure containing log information passed to delegates that receive log information.
    /// </summary>
    public struct DicomLogInfo
    {
        public DateTime Time;
        public String Message;
        public String Level;
    }

    public delegate void DicomLog(DicomLogInfo info);

    /// <summary>
    /// Generic logging routine for the DICOM tool kit.
    /// </summary>
    public class DicomLogger
    {
        public static DicomLog LogDelegates = DicomFileLogger.Log;

        private static String logFile = "DicomLog.txt";

        /// <summary>
        /// Static property containing the name of the log file.
        /// The default value is 'PortalServiceLog.txt'.
        /// </summary>
        public static String LogFile
        {
            set
            {
                logFile = value;
            }
        }

        /// <summary>
        /// Log an error message to the log file.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="args">Arguments</param>
        public static void LogError(String msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(msg, args);

            WriteLog(sb.ToString(), "Error");
        }

        /// <summary>
        /// Log an informational message to the log file.
        /// </summary>
        /// <param name="msg">The informational message to log.</param>
        /// <param name="args">Arguments</param>
        public static void LogInfo(String msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(msg, args);

            WriteLog(sb.ToString(), "Info");
        }


        /// <summary>
        /// Variable used for locking access to the log file between threads.
        /// </summary>
        private static System.Object lockThis = new System.Object();

        /// <summary>
        /// Internal routine for writing all log messages to the assigned delegates.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="type">A string describing the type of log message.</param>
        private static void WriteLog(String msg, String type)
        {
            if (LogDelegates != null)
            {
                DicomLogInfo info = new DicomLogInfo();
                info.Time = DateTime.Now;
                info.Message = msg;
                info.Level = type;

                LogDelegates(info);
            }
        }
    }

    public class DicomFileLogger
    {
        private static String logFile = "DicomLog.txt";

        /// <summary>
        /// Static property containing the name of the log file.
        /// The default value is 'DicomLog.txt'.
        /// </summary>
        public static String LogFile
        {
            set
            {
                logFile = value;
            }
        }

        /// <summary>
        /// Variable used for locking access to the log file between threads.
        /// </summary>
        private static System.Object lockThis = new System.Object();

        /// <summary>
        /// Delegate routine for writing all log messages to a log file.
        /// </summary>
        /// <param name="info">The log information</param>
        public static void Log(DicomLogInfo info)
        {
            lock (lockThis)
            {
                StreamWriter writer;
                try
                {
                    String logFileDate = logFile;

                    logFileDate = logFileDate.Insert(logFileDate.IndexOf(".log"), "_" + info.Time.ToString("MM-dd-yyyy"));

                    writer = File.AppendText(logFileDate);

                    string header = "(" + Thread.CurrentThread.ManagedThreadId + ") " + info.Time.ToShortDateString() + " " + info.Time.ToLongTimeString();

                    writer.WriteLine(header + " (" + info.Level + ") " + info.Message);

                    writer.Close();
                }
                catch (Exception)
                {
                    // Circular reference?

                }
            }
        }
    }
}
