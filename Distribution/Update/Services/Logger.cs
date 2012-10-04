#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using log4net;

namespace ClearCanvas.Distribution.Update.Services
{
	internal static class Logger
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (UpdateInformationService));

		//generated with resharper
		public static bool IsDebugEnabled
		{
			get { return _log.IsDebugEnabled; }
		}

		public static bool IsInfoEnabled
		{
			get { return _log.IsInfoEnabled; }
		}

		public static bool IsWarnEnabled
		{
			get { return _log.IsWarnEnabled; }
		}

		public static bool IsErrorEnabled
		{
			get { return _log.IsErrorEnabled; }
		}

		public static bool IsFatalEnabled
		{
			get { return _log.IsFatalEnabled; }
		}

		public static void Debug(object message)
		{
			_log.Debug(message);
		}

		public static void Debug(object message, Exception exception)
		{
			_log.Debug(message, exception);
		}

		public static void DebugFormat(string format, params object[] args)
		{
			_log.DebugFormat(format, args);
		}

		public static void DebugFormat(string format, object arg0)
		{
			_log.DebugFormat(format, arg0);
		}

		public static void DebugFormat(string format, object arg0, object arg1)
		{
			_log.DebugFormat(format, arg0, arg1);
		}

		public static void DebugFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.DebugFormat(format, arg0, arg1, arg2);
		}

		public static void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.DebugFormat(provider, format, args);
		}

		public static void Info(object message)
		{
			_log.Info(message);
		}

		public static void Info(object message, Exception exception)
		{
			_log.Info(message, exception);
		}

		public static void InfoFormat(string format, params object[] args)
		{
			_log.InfoFormat(format, args);
		}

		public static void InfoFormat(string format, object arg0)
		{
			_log.InfoFormat(format, arg0);
		}

		public static void InfoFormat(string format, object arg0, object arg1)
		{
			_log.InfoFormat(format, arg0, arg1);
		}

		public static void InfoFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.InfoFormat(format, arg0, arg1, arg2);
		}

		public static void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.InfoFormat(provider, format, args);
		}

		public static void Warn(object message)
		{
			_log.Warn(message);
		}

		public static void Warn(object message, Exception exception)
		{
			_log.Warn(message, exception);
		}

		public static void WarnFormat(string format, params object[] args)
		{
			_log.WarnFormat(format, args);
		}

		public static void WarnFormat(string format, object arg0)
		{
			_log.WarnFormat(format, arg0);
		}

		public static void WarnFormat(string format, object arg0, object arg1)
		{
			_log.WarnFormat(format, arg0, arg1);
		}

		public static void WarnFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.WarnFormat(format, arg0, arg1, arg2);
		}

		public static void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.WarnFormat(provider, format, args);
		}

		public static void Error(object message)
		{
			_log.Error(message);
		}

		public static void Error(object message, Exception exception)
		{
			_log.Error(message, exception);
		}

		public static void ErrorFormat(string format, params object[] args)
		{
			_log.ErrorFormat(format, args);
		}

		public static void ErrorFormat(string format, object arg0)
		{
			_log.ErrorFormat(format, arg0);
		}

		public static void ErrorFormat(string format, object arg0, object arg1)
		{
			_log.ErrorFormat(format, arg0, arg1);
		}

		public static void ErrorFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.ErrorFormat(format, arg0, arg1, arg2);
		}

		public static void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.ErrorFormat(provider, format, args);
		}

		public static void Fatal(object message)
		{
			_log.Fatal(message);
		}

		public static void Fatal(object message, Exception exception)
		{
			_log.Fatal(message, exception);
		}

		public static void FatalFormat(string format, params object[] args)
		{
			_log.FatalFormat(format, args);
		}

		public static void FatalFormat(string format, object arg0)
		{
			_log.FatalFormat(format, arg0);
		}

		public static void FatalFormat(string format, object arg0, object arg1)
		{
			_log.FatalFormat(format, arg0, arg1);
		}

		public static void FatalFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.FatalFormat(format, arg0, arg1, arg2);
		}

		public static void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.FatalFormat(provider, format, args);
		}
	}
}