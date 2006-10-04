using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace ClearCanvas.Dicom
{
	public static class DateTimeParser
	{
		private static readonly char[] _plusMinus = { '+', '-' };

		/// <summary>
		/// Parses a dicom Date/Time string using the DateParser and TimeParser
		/// (TryParseExact) functions.  The Hour/Minute adjustment factor (as
		/// specified in Dicom for universal time adjustment) is accounted for 
		/// (and parsed) by this function.
		/// </summary>
		/// <param name="dicomDateTime">the dicom date/time string</param>
		/// <param name="dateTime">the date/time as a DateTime object</param>
		/// <returns>true on success, false otherwise</returns>
		public static bool Parse(string dicomDateTime, out DateTime dateTime)
		{
			dateTime = new DateTime(); 
			
			if (String.IsNullOrEmpty(dicomDateTime))
				return false;

			int plusMinusIndex = dicomDateTime.IndexOfAny(_plusMinus);

			string dateTimeString = dicomDateTime;

			string offsetString = String.Empty;
			if (plusMinusIndex > 0)
			{
				offsetString = dateTimeString.Substring(plusMinusIndex);
				dateTimeString = dateTimeString.Remove(plusMinusIndex);
			}

			string dateString;
			if (dateTimeString.Length >= 8)
				dateString = dateTimeString.Substring(0, 8);
			else
				return false;

			string timeString = String.Empty;
			if (dateTimeString.Length > 8)
				timeString = dateTimeString.Substring(8);

			int hourOffset = 0;
			int minuteOffset = 0;
			if (!String.IsNullOrEmpty(offsetString))
			{
				if (offsetString.Length > 3)
				{
#if MONO
					try
					{
						minuteOffset = Int32.Parse(offsetString.Substring(3));
						hourOffset = Int32.Parse(offsetString.Remove(3));
					}
					catch
					{
						return false;
					}
#else
					if (!Int32.TryParse(offsetString.Substring(3), out minuteOffset))
						return false;

					if (!Int32.TryParse(offsetString.Remove(3), out hourOffset))
						return false;
#endif
				}
				else
				{
#if MONO
					try
					{
						hourOffset = Int32.Parse(offsetString);
					}
					catch
					{
						return false;
					}
#else
					if (!Int32.TryParse(offsetString, out hourOffset))
						return false;
#endif
				}

				minuteOffset *= Math.Sign(hourOffset);
			}

			DateTime date;
			if (!DateParser.Parse(dateString, out date))
				return false;

			DateTime time = new DateTime(); //zero datetime
			if (!String.IsNullOrEmpty(timeString))
			{
				if (!TimeParser.Parse(timeString, out time))
					return false;
			}

			dateTime = date;
			dateTime = dateTime.AddTicks(time.Ticks);
			dateTime = dateTime.AddHours(hourOffset);
			dateTime = dateTime.AddMinutes(minuteOffset);

			return true;
		}
	}
}
