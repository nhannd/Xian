using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// The DateParser class parses dates that are formatted correctly according to Dicom.
	/// 
	/// We use the TryParseExact function to parse the dates because it is far more efficient
	/// than ParseExact since it does not throw exceptions.
	/// 
	/// See http://blogs.msdn.com/ianhu/archive/2005/12/19/505702.aspx for a good profile
	/// comparision of the different Parse/Convert methods.
	/// </summary>
	public static class DateParser
	{
		public const string DicomDateFormat = "yyyyMMdd";

		// Dicom recommends we still support the old date format (#2) although it is deprecated.
		// See PS 3.5, table 6.2-1 - 'Dicom Value Representations' under VR DA.
		private static readonly string[] _dateFormats = { "yyyyMMdd", "yyyy.MM.dd" };

		/// <summary>
		/// Attempts to parse the date string exactly, according to accepted Dicom format(s).
		/// Will *not* throw an exception if the format is invalid.
		/// </summary>
		/// <param name="dicomDate">the dicom date string</param>
		/// <returns>a nullable DateTime</returns>
		public static DateTime? Parse(string dicomDate)
		{
			DateTime date;
			if (!Parse(dicomDate, out date))
				return null;

			return date;
		}

		/// <summary>
		/// Attempts to parse the date string exactly, according to accepted Dicom format(s).
		/// Will *not* throw an exception if the format is invalid.
		/// </summary>
		/// <param name="dicomDate">the dicom date string</param>
		/// <param name="date">returns the date as a DateTime object</param>
		/// <returns>true on success, false otherwise</returns>
		public static bool Parse(string dicomDate, out DateTime date)
		{
#if MONO
			try
			{
				date = DateTime.ParseExact(dicomDate, _dateFormats, new CultureInfo(""), DateTimeStyles.None);
				return true;
			}
			catch
			{
				date = new DateTime();
				return false;
			}
#else
			return DateTime.TryParseExact(dicomDate, _dateFormats, new CultureInfo(""), DateTimeStyles.None, out date);
#endif
		}

		//*** This code also works, but the TryParseExact function works just fine.

		//public static DateTime Parse(string dicomDate, out bool success)
		//{
		//    success = false;

		//    int year, month, day;
		//    if (!Parse(dicomDate, out year, out month, out day))
		//        return new DateTime();

		//    success = true;
		//    return new DateTime(year, month, day);
		//}

		//public static bool Parse(string dicomDate, out int year, out int month, out int day)
		//{
		//    year = 0;
		//    month = 0;
		//    day = 0;

		//    int yearStartIndex = 0;
		//    int monthStartIndex = 4;
		//    int dayStartIndex = 6;

		//    if (dicomDate.Length == 8)
		//    {
		//    }
		//    else if (dicomDate.Length == 10)
		//    {
		//        ++monthStartIndex;
		//        dayStartIndex += 2;
		//    }
		//    else
		//    {
		//        return false;
		//    }

		//    if (!Int32.TryParse(dicomDate.Substring(yearStartIndex, 4), out year))
		//        return false;

		//    if (!Int32.TryParse(dicomDate.Substring(monthStartIndex, 2), out month))
		//        return false;

		//    if (!Int32.TryParse(dicomDate.Substring(dayStartIndex, 2), out day))
		//        return false;

		//    if (!ValidateDate(year, month, day))
		//        return false;

		//    return true;
		//}

		//public static bool ValidateDate(int year, int month, int day)
		//{
		//    if (year < 1 || year > 9999)
		//        return false;

		//    if (month < 1 || month > 12)
		//        return false;

		//    if (day < 1 || day > DateTime.DaysInMonth(year, month))
		//        return false;

		//    return true;
		//}
 	}
}
