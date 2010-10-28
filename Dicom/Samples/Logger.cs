#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Samples
{
    public struct LogInfo
    {
    	public DateTime Time;
		public LogLevel Level;
    	public string Message;
    	public int ThreadId;
    }

	public static class Logger
    {
        private static TextBox _tb;

		public static void Log(LogInfo info)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendFormat("({0}) {1} {2} ({3}) {4}", info.ThreadId,
                         info.Time.ToShortDateString(), info.Time.ToLongTimeString(),
                         info.Level, info.Message);

            AppendToTextBox(sb.ToString());
        }

        private static void AppendToTextBox(string info)
        {
            if (_tb == null)
                return;

            if (!_tb.InvokeRequired)
            {
                _tb.AppendText(info);
            }
            else
            {
                _tb.BeginInvoke(new Action<string>(AppendToTextBox), new object[] { info });
            }
        }

		private static void Log(LogLevel level, Exception e, string message, params object[] formatArgs)
		{
			if (String.IsNullOrEmpty(message))
				return;

			StringBuilder builder = new StringBuilder();
			builder.AppendFormat(message, formatArgs);
			if (e != null)
			{
				builder.AppendLine();
				builder.Append(e);
			}

			LogInfo info = new LogInfo();
			info.Level = level;
			info.Message = builder.ToString();
			info.ThreadId = Thread.CurrentThread.ManagedThreadId;
			info.Time = DateTime.Now;

			Log(info);
		}

		public static void RegisterLogHandler(TextBox tb)
        {
            _tb = tb;
        }
		
		public static void LogError(string message, params object[] formatArgs)
		{
			Log(LogLevel.Error, null, message, formatArgs);
            Platform.Log(LogLevel.Error, message, formatArgs);
		}

		public static void LogErrorException(Exception e, string message, params object[] formatArgs)
		{
			Log(LogLevel.Error, e, message, formatArgs);
            Platform.Log(LogLevel.Error, e, message, formatArgs);
        }

		public static void LogInfo(string message, params object[] formatArgs)
		{
			Log(LogLevel.Info, null, message, formatArgs);
            Platform.Log(LogLevel.Info, message, formatArgs);
        }

		public static void LogWarn(string message, params object[] formatArgs)
		{
			Log(LogLevel.Warn, null, message, formatArgs);
            Platform.Log(LogLevel.Warn, message, formatArgs);
        }

		public static void LogDebug(string message, params object[] formatArgs)
		{
			Log(LogLevel.Debug, null, message, formatArgs);
		}
	}
}
