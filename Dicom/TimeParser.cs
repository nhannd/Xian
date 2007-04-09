using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace ClearCanvas.Dicom
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
#if MONO
			try
			{
				time = DateTime.ParseExact(timeString, _timeFormats, new CultureInfo(""), DateTimeStyles.None);
				time = new DateTime(time.TimeOfDay.Ticks);
				return true;
			}
			catch
			{
				time = new DateTime();
				return false;
			}
#else
			if (!DateTime.TryParseExact(timeString, _timeFormats, new CultureInfo(""), DateTimeStyles.None, out time))
				return false;

			//exclude the date.
			time = new DateTime(time.TimeOfDay.Ticks);
			return true;
#endif
		}

		//*** This code also works, but the TryParseExact function works just fine.

		//public static DateTime Parse(string dicomString, out bool success)
		//{
		//    success = false;

		//    int hour, minute, second;
		//    double fraction;
		//    if (!Parse(dicomString, out hour, out minute, out second, out fraction))
		//        return new DateTime();

		//    success = true; 
		//    DateTime time = new DateTime(1, 1, 1, hour, minute, second);
		//    fraction *= 10000000; //ticks are in 100 nano-second units (100 * 1e-9 = 1e-7)
		//    time = time.AddTicks((long)fraction);

		//    return time;
		//}
		
		//public static bool Parse(string timeString, out int hour, out int minute, out int second, out double fraction)
		//{
		//    hour = 0;
		//    minute = 0;
		//    second = 0;
		//    fraction = 0;

		//    int stringLength = timeString.Length; 
			
		//    if (stringLength >= 2)
		//    {
		//        if (!Int32.TryParse(timeString.Substring(0, 2), out hour))
		//            return false;

		//        if (stringLength >= 4)
		//        {
		//            if (!Int32.TryParse(timeString.Substring(2, 2), out minute))
		//                return false;

		//            if (stringLength >= 6)
		//            {
		//                if (!Int32.TryParse(timeString.Substring(4, 2), out second))
		//                    return false;

		//                if (stringLength >= 8)
		//                {
		//                    if (!double.TryParse(timeString.Substring(6), out fraction))
		//                        return false;
		//                }
		//            }
		//        }

		//        if (ValidateTime(hour, minute, second, fraction))
		//            return true;
		//    }

		//    return false;
		//}

		//public static bool ValidateTime(int hour, int minute, int second, double fraction)
		//{
		//    if (hour < 0 || hour > 23)
		//        return false;

		//    if (minute < 0 || minute > 59)
		//        return false;

		//    if (second < 0 || second > 59)
		//        return false;

		//    if (fraction < 0 || fraction > 0.999999)
		//        return false;

		//    return true;
		//}
	}
}
