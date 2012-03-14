#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	internal class TimeSpanDisplayHelper
	{
		public static string CalculateTimeSpanDisplay(DateTime baseTime)
		{
			//TODO (Time Review): Change this back to use Platform.Time once we've resolved
			//the exception throwing issue.
			TimeSpan timeSpan = DateTime.Now.Subtract(baseTime);
			return CalculateTimeSpanDisplay(timeSpan);
		}

		public static string CalculateTimeSpanDisplay(TimeSpan timeSpan)
		{
			string timeSpanDisplay;

			if (timeSpan.Days > 0)
			{
				if (timeSpan.Days == 1)
					timeSpanDisplay = SR.MessageOneDayAgo;
				else
					timeSpanDisplay = String.Format(SR.FormatXDaysAgo, timeSpan.Days);
			}
			else if (timeSpan.Hours > 0)
			{
				if (timeSpan.Hours == 1)
					timeSpanDisplay = SR.MessageOneHourAgo;
				else
					timeSpanDisplay = String.Format(SR.FormatXHoursAgo, timeSpan.Hours);

				if (timeSpan.Minutes == 1)
					timeSpanDisplay += SR.MessageOneMinuteAgo;
				else
					timeSpanDisplay += String.Format(SR.FormatXMinutesAgo, timeSpan.Minutes);
			}
			else
			{
				if (timeSpan.Minutes == 1)
				{
					timeSpanDisplay = SR.MessageOneMinuteAgo;
				}
				else
				{
					int minutes = 0;
					if (timeSpan.Minutes > 0)
						minutes = timeSpan.Minutes;

					timeSpanDisplay = String.Format(SR.FormatXMinutesAgo, minutes);
				}
			}

			return timeSpanDisplay;
		}
	}
}
