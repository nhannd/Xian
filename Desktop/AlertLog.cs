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
	/// <summary>
	/// Represents a record of an alert.
	/// </summary>
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
		public bool Acknowledged { get; set; }
	}

	/// <summary>
	/// Maintains a log of alerts that have occured during process execution.
	/// </summary>
	internal class AlertLog
	{

		private static readonly AlertLog _instance = new AlertLog();

		/// <summary>
		/// Gets the singleton instance of the alert log.
		/// </summary>
		internal static AlertLog Instance
		{
			get { return _instance; }
		}

		private const int MaxLogSize = 500;
		private readonly Queue<Alert> _alerts = new Queue<Alert>();

		private AlertLog()
		{
		}

		/// <summary>
		/// Occurs when a new alert is logged.
		/// </summary>
		public event EventHandler<ItemEventArgs<Alert>>  AlertLogged;

		/// <summary>
		/// Logs a new alert.
		/// </summary>
		/// <param name="args"></param>
		public void Log(AlertNotificationArgs args)
		{
			var alert = new Alert(args.Level, Platform.Time, args.Message)
			            	{
			            		Acknowledged = args.Level == AlertLevel.Info	// info alerts are "pre-acknowledged" (do not require acknowledgement)
			            	};

			_alerts.Enqueue(alert);
			while (_alerts.Count > MaxLogSize)
				_alerts.Dequeue();

			EventsHelper.Fire(AlertLogged, this, new ItemEventArgs<Alert>(alert));
		}

		/// <summary>
		/// Marks any unacknowledged alerts as being acknowledged.
		/// </summary>
		public void AcknowledgeAll()
		{
			foreach (var alert in _alerts)
			{
				alert.Acknowledged = true;
			}
		}

		/// <summary>
		/// Returns the alert log entries in chronological order.
		/// </summary>
		public IEnumerable<Alert> Entries
		{
			get { return _alerts; }
		}

	}
}
