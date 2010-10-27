#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Ris.Client.Formatting
{
	public static class TimeSpanFormat
	{
		/// <summary>
		/// This is a specialized method to Format the timeSpan in to a descriptive text.
		/// The formatted text may be "HH:mm hours", "HH hours" or "mm minutes".   All units below minutes are ignored.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		public static string FormatDescriptive(TimeSpan timeSpan)
		{
			if (timeSpan.Days == 0 && timeSpan.Hours == 0)
				return string.Format("{0} minutes", timeSpan.Minutes);

			var totalHours = Math.Floor(timeSpan.TotalHours);
			return timeSpan.Minutes == 0 
				? string.Format("{0} hours", totalHours) 
				: string.Format("{0}:{1} hours", totalHours, timeSpan.Minutes);
		}
	}
}
