using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	internal class TimeSpanDisplayHelper
	{
		public static string CalculateTimeSpanDisplay(DateTime baseTime)
		{
			TimeSpan timeSpan = Platform.Time.Subtract(baseTime);
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
