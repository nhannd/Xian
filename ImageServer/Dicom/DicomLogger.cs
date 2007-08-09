using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace ClearCanvas.ImageServer.Dicom
{
    /// <summary>
    /// Enumerated value for DICOM log levels
    /// </summary>
    public enum DicomLogLevel
    {
        Info,
        Error,
        Warning
    }

    /// <summary>
    /// Structure containing log information passed to delegates that receive log information.
    /// </summary>
    public struct DicomLogInfo
    {
        public DateTime Time;
        public String Message;
        public DicomLogLevel Level;
        public int ThreadId;
    }

    public delegate void DicomLogDelegate(DicomLogInfo info);

    /// <summary>
    /// Generic logging routine for the DICOM tool kit.
    /// </summary>
    public class DicomLogger
    {
        public static DicomLogDelegate LogDelegates = DicomFileLogger.Log;

        /// <summary>
        /// Log an error message related to an exception to the log file.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        public static void LogErrorException(Exception e, String msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(msg, args);

            sb.AppendFormat("Exception: {0} ", e.Message);
            sb.AppendLine();
            sb.AppendLine("Stack Trace:");
            sb.AppendLine(e.StackTrace);

            WriteLog(sb.ToString(), DicomLogLevel.Error);
        }


        /// <summary>
        /// Log an error message to the log file.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        public static void LogError(String msg,params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(msg,args);

            WriteLog(sb.ToString(), DicomLogLevel.Error);
        }

        /// <summary>
        /// Log an informational message to the log file.
        /// </summary>
        /// <param name="msg">The informational message to log.</param>
        public static void LogInfo(String msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(msg, args);

            WriteLog(sb.ToString(), DicomLogLevel.Info);
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
        private static void WriteLog(String msg, DicomLogLevel type)
        {
            if (LogDelegates != null)
            {
                DicomLogInfo info = new DicomLogInfo();
                info.Time = DateTime.Now;
                info.Message = msg;
                info.Level = type;
                info.ThreadId = Thread.CurrentThread.ManagedThreadId;

                try
                {
                    LogDelegates(info);
                }
                catch (Exception) { }
            }
        }
    }

    /// <summary>
    /// Default implementation of a logger that logs the DICOM log to a file.
    /// </summary>
    public class DicomFileLogger
    {
        private static String _logFile = "DicomLog.log";

        /// <summary>
        /// Static property containing the name of the log file.
        /// The default value is 'DicomLog.txt'.
        /// </summary>
        public static String LogFile
        {
            set { _logFile = value; }
            get { return _logFile; }
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
                    String logFileDate = _logFile;
                    int index = logFileDate.IndexOf(".log");
                    if (index < 0)
                        logFileDate = logFileDate + "_" + info.Time.ToString("MM-dd-yyyy") + ".log";
                    else
                        logFileDate = logFileDate.Insert(index, "_" + info.Time.ToString("MM-dd-yyyy"));

                    writer = File.AppendText(logFileDate);
                    String level = "";
                    switch (info.Level)
                    {
                        case DicomLogLevel.Error: level = "Error"; break;
                        case DicomLogLevel.Info: level = "Info"; break;
                        case DicomLogLevel.Warning: level = "Warn"; break;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb = sb.AppendFormat("({0}) {1} {2} ({3}) {4}",info.ThreadId,info.Time.ToShortDateString(),info.Time.ToLongTimeString(),level,
                        info.Message);


                    writer.WriteLine(sb.ToString());

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
