#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	public enum AlertLevel
	{
		Info,
		Warning,
		Error
	}

	internal class Alert
	{
		public Alert(AlertLevel level, DateTime time, string message)
		{
			Level = level;
			Time = time;
			Message = message;
		}

		public AlertLevel Level { get; private set; }
		public DateTime Time { get; private set; }
		public string Message { get; private set; }
	}

	internal class AlertLog
	{

		private static readonly AlertLog _instance = new AlertLog();

		public static AlertLog Instance
		{
			get { return _instance; }
		}

		private const int MaxLogSize = 500;
		private readonly Queue<Alert> _alerts = new Queue<Alert>();

		private AlertLog()
		{
			
		}

		public event EventHandler<ItemEventArgs<Alert>>  AlertLogged;

		public void Log(AlertNotificationArgs args)
		{
			var alert = new Alert(args.Level, Platform.Time, args.Message);

			_alerts.Enqueue(alert);
			while (_alerts.Count > MaxLogSize)
				_alerts.Dequeue();

			EventsHelper.Fire(AlertLogged, this, new ItemEventArgs<Alert>(alert));
		}

		public IEnumerable<Alert> Entries
		{
			get { return _alerts; }
		}

	}
}
