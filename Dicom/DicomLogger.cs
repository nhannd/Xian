#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace ClearCanvas.Dicom
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
        /// <param name="msg">A message format string to log.</param>
        /// <param name="e">The exception to log informaton about.</param>
        /// <param name="args">Arguments to be used with the <paramref name="msg"/> format string.</param>
        public static void LogErrorException(Exception e, String msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(msg, args);
            sb.AppendLine();
            sb.AppendFormat("Exception: {0} ", e.Message);
            sb.AppendLine();
            sb.AppendLine("Stack Trace:");
            sb.AppendLine(e.StackTrace);

            WriteLog(sb.ToString(), DicomLogLevel.Error);
        }


        /// <summary>
        /// Log an error message to the log file.
        /// </summary>
        /// <param name="msg">The message/format string to log.</param>
        /// <param name="args">Variable arguments used with the format string.</param>
        public static void LogError(String msg,params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(msg,args);

            WriteLog(sb.ToString(), DicomLogLevel.Error);
        }

        /// <summary>
        /// Log an informational message to the log file.
        /// </summary>
        /// <param name="msg">The informational message/format string to log.</param>
        /// <param name="args">Variable arguments used with the format string.</param>
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
