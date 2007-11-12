#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace ClearCanvas.Dicom
{
	public static class DateTimeParser
	{
        public static readonly string DicomFullDateTimeFormatWithTimeZone = "yyyyMMddHHmmss.ffffff&zzzz";
        public static readonly string DicomFullDateTimeFormat = "yyyyMMddHHmmss.ffffff";

		private static readonly char[] _plusMinus = { '+', '-' };

		/// <summary>
		/// Attempts to parse the time string exactly, according to accepted Dicom datetime format(s).
		/// Will *not* throw an exception if the format is invalid (better for when performance is needed).
		/// </summary>
		/// <param name="dateTimeString">the dicom datetime string</param>
		/// <returns>a nullable DateTime</returns>
		public static DateTime? Parse(string dateTimeString)
		{
			DateTime dateTime;
			if (!Parse(dateTimeString, out dateTime))
				return null;

			return dateTime;
		}

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
					if (!Int32.TryParse(offsetString.Substring(3), NumberStyles.Integer, CultureInfo.InvariantCulture, out minuteOffset))
						return false;

					if (!Int32.TryParse(offsetString.Remove(3), NumberStyles.Integer, CultureInfo.InvariantCulture, out hourOffset))
						return false;
				}
				else
				{
					if (!Int32.TryParse(offsetString, NumberStyles.Integer, CultureInfo.InvariantCulture, out hourOffset))
						return false;
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

        /// <summary>
        /// Convert a datetime object into a DT string
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToDicomString(DateTime datetime, bool toUTC)
        {
            if (toUTC)
            {
                DateTime utc = datetime.ToUniversalTime();
				return utc.ToString(DicomFullDateTimeFormatWithTimeZone, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
				return datetime.ToString(DicomFullDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
            }
            
        }
	}
}
