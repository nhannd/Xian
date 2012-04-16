#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;


namespace ClearCanvas.Desktop
{
	public class AlertNotificationArgs
	{
		public AlertNotificationArgs(AlertLevel level, string message)
			:this(level, message, null, null)
		{
			
		}

		public AlertNotificationArgs(AlertLevel level, string message, string linkText, Action<DesktopWindow> linkAction)
		{
			Level = level;
			Message = message;
			LinkText = linkText;
			LinkAction = linkAction;

			AutoDismiss = level == AlertLevel.Info;	// by default, only Info alerts are auto-dismissed
		}

		public AlertLevel Level { get; set; }
		public string Message { get; set; }
		public string LinkText { get; set; }
		public Action<DesktopWindow> LinkAction { get; set; }
		public string Title { get; set; }
		public bool AutoDismiss { get; set; }
	}
}
