#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Globalization;

namespace ClearCanvas.Dicom.Utilities
{
	/// <summary>
	/// The TimeParser class parses times that are formatted correctly according to Dicom.
	/// 
	/// We use the TryParseExact function to parse the times because it is far more efficient
	/// than ParseExact since it does not throw exceptions.
	/// 
	/// See http://blogs.msdn.com/ianhu/archive/2005/12/19/505702.aspx for a good profile
	/// comparision of the different Parse/Convert methods.
	/// </summary>
	public static class TimeParser
	{
		public static readonly string DicomFullTimeFormat = "HHmmss.FFFFFF";

		private static readonly string[] _timeFormats = { "HHmmss", "HHmmss.FFFFFF", "HHmm", "HH" };

		/// <summary>
		/// Attempts to parse the time string exactly, according to accepted Dicom time format(s).
		/// Will *not* throw an exception if the format is invalid (better for when performance is needed).
		/// </summary>
		/// <param name="timeString">the dicom time string</param>
		/// <returns>a nullable DateTime</returns>
		public static DateTime? Parse(string timeString)
		{
			DateTime time;
			if (!Parse(timeString, out time))
				return null;

			return time;
		}

		/// <summary>
		/// Attempts to parse the time string exactly, according to accepted Dicom time format(s).
		/// Will *not* throw an exception if the format is invalid (better for when performance is needed).
		/// </summary>
		/// <param name="timeString">the dicom time string</param>
		/// <param name="time">returns the time as a DateTime object</param>
		/// <returns>true on success, false otherwise</returns>
		public static bool Parse(string timeString, out DateTime time)
		{
			// This method is used in DicomAttribute Get/TryGet,
			// which allow leading/trailing spaces in the string
			// They are considered valid DICOM date/time.
			if (timeString!=null)
				timeString = timeString.Trim();

			if (!DateTime.TryParseExact(timeString, _timeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
				return false;

			//exclude the date.
			time = new DateTime(time.TimeOfDay.Ticks);
			return true;
		}

		/// <summary>
		/// Convert a DateTime object into a TM string
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns>The DICOM formatted string</returns>
		public static string ToDicomString(DateTime datetime)
		{
			return datetime.ToString(DicomFullTimeFormat, CultureInfo.InvariantCulture);
		}

	}
}