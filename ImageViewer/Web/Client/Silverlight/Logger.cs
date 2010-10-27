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

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public class Logger
    {
        private static WriteDelegate _writeDelegate;
        private static ErrorDelegate _errorDelegate;

        public delegate void WriteDelegate(String msg);
        public delegate void ErrorDelegate(String msg);

        public static void SetWriteMethod(WriteDelegate del)
        {
            _writeDelegate = del;
        }

        public static void SetErrorMethod(ErrorDelegate del)
        {
            _errorDelegate = del;
        }

        public static void Write(String message, params object[] args)
        {
            if (_writeDelegate != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(message, args);
                _writeDelegate(sb.ToString());
            }
        }

        public static void Error(String message, params object[] args)
        {
            if (_errorDelegate != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(message, args);
                _errorDelegate(sb.ToString());
            }
        }

        public static void Error(Exception e, String message, params object[] args)
        {
            if (_errorDelegate != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(message, args);
                sb.AppendLine(e.Message);
                sb.AppendFormat(e.StackTrace);
                sb.AppendLine();
                _errorDelegate(sb.ToString());
            }
        }

        public static void Error(Exception exception)
        {
            if (_errorDelegate != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(exception.Message);
                sb.AppendFormat(exception.StackTrace);
                sb.AppendLine();
                _errorDelegate(sb.ToString());
            }
        }

    }
}
