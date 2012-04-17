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
using System.Linq;
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

	public static class AlertLevelExtensions
	{
		public static IconSet GetIcon(this AlertLevel level)
		{
			switch (level)
			{
				case AlertLevel.Info:
					return new IconSet("InfoMini.png", "InfoSmall.png", "InfoMedium.png");
				case AlertLevel.Warning:
					return new IconSet("WarningMini.png", "WarningSmall.png", "WarningMedium.png");
				case AlertLevel.Error:
					return new IconSet("ErrorMini.png", "ErrorSmall.png", "ErrorMedium.png");
			}
			throw new ArgumentOutOfRangeException();
		}
	}

	public class Alert
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

	public class AlertLog
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

		public void AcknowledgeAll()
		{
			foreach (var alert in _alerts)
			{
				alert.Acknowledged = true;
			}
		}

		/// <summary>
		/// Returns the entries in chronological order.
		/// </summary>
		public IEnumerable<Alert> Entries
		{
			get { return _alerts; }
		}

	}
}
