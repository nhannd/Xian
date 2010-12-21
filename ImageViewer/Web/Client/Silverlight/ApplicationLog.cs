#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion


using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public class ApplicationLogAppendEventArgs : EventArgs
    {
        public String Message;
    }

    public class ApplicationLog
    {
        static StringBuilder _logs = new StringBuilder();

        public static event EventHandler<ApplicationLogAppendEventArgs> LogAppended;
        

        public static String GetLogContent()
        {
            return _logs.ToString();
        }

        public static void Clear()
        {
            _logs.Clear();
        }

        static public void LogError(string msg)
        {
            _logs.Append(msg);
            if (LogAppended != null)
                LogAppended(null, new ApplicationLogAppendEventArgs { Message = msg });
        }

        static public void LogMessage(string msg)
        {

#if DEBUG
            _logs.Append(msg);
            if (LogAppended != null)
                LogAppended(null, new ApplicationLogAppendEventArgs { Message = msg });
#endif

        }

        static public void LogError(Exception exception)
        {
            
            if (LogAppended != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(exception.Message);
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(exception.StackTrace);

                string msg = sb.ToString();
                _logs.Append(msg);

                LogAppended(null, new ApplicationLogAppendEventArgs { Message = msg });
            }
        }

        internal static void Initialize()
        {
            Logger.SetWriteMethod(LogMessage);
        }
    }
}
