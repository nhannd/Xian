#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Utility class that assists with formatting objects for display.
    /// </summary>
    public static class Format
    {
        /// <summary>
        /// Gets or sets the default date format string.
        /// </summary>
        public static string DateFormat
        {
            get { return FormatSettings.Default.DateFormat; }
			set 
			{
				FormatSettings.Default.DateFormat = value;
				FormatSettings.Default.Save();
			}
        }

        /// <summary>
        /// Gets or sets the default time format string.
        /// </summary>
        public static string TimeFormat
        { 
            get { return FormatSettings.Default.TimeFormat; }
			set
			{
				FormatSettings.Default.TimeFormat = value;
				FormatSettings.Default.Save();
			}
        }

        /// <summary>
        /// Gets or sets the default date-time format string.
        /// </summary>
        public static string DateTimeFormat
        {
            get { return FormatSettings.Default.DateTimeFormat; }
			set
			{ 
				FormatSettings.Default.DateTimeFormat = value;
				FormatSettings.Default.Save();
			}
        }

        /// <summary>
		/// Formats the specified <see cref="System.DateTime"/> as a date.
        /// </summary>
        public static string Date(DateTime dt)
        {
            return dt.ToString(DateFormat);
        }

        /// <summary>
		/// Formats the specified <see cref="System.DateTime"/> as a date, returning an empty string if null.
        /// </summary>
        public static string Date(DateTime? dt)
        {
            return Date(dt, false);
        }

        /// <summary>
        /// Formats the specific <see cref="System.DateTime"/> as a date, descriptive if set by condition, returns an empty string if input date is null.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="descriptive"></param>
        /// <returns></returns>
        public static string Date(DateTime? dt, bool descriptive)
        {
            if(descriptive)
            {
                DateTime? today = System.DateTime.Today;
                DateTime? yesterday = today.Value.AddDays(-1);
                DateTime? tomorrow = today.Value.AddDays(1);
                DateTime? afterTomorrow = tomorrow.Value.AddDays(1);

                if (dt < yesterday)
                {
                    int dayDiff = (int)Math.Ceiling(((today.Value - dt.Value).TotalDays));

                    if (dayDiff < FormatSettings.Default.DescriptiveDateThresholdInDays)
                    {
                        return dayDiff + " days ago";
                    }
                    else
                    {
                        return DateTime(dt);
                    }
                }
                else if (dt >= yesterday && dt < today)
                {
                    return "Yesterday " + Time(dt);
                }
                else if (dt >= today && dt < tomorrow)
                {
                    return "Today " + Time(dt);
                }
                else if (dt >= tomorrow && dt < afterTomorrow)
                {
                    return "Tomorrow " + Time(dt);
                }
                else if (dt > afterTomorrow)
                {
                    int dayDiff = (dt - today).Value.Days;

                    if (dayDiff < FormatSettings.Default.DescriptiveDateThresholdInDays)
                    {
                        return dayDiff + " days from now";
                    }
                    else
                    {
                        return DateTime(dt);
                    }
                }
                else
                {
                    return DateTime(dt);
                }
            }
            else
            {
                return dt == null ? "" : dt.Value.ToString(DateFormat);
            }
        }

        /// <summary>
		/// Formats the specified <see cref="System.DateTime"/> as a time.
        /// </summary>
        public static string Time(DateTime dt)
        {
            return dt.ToString(TimeFormat);
        }

        /// <summary>
		/// Formats the specified <see cref="System.DateTime"/> as a time, returning an empty string if null.
        /// </summary>
        public static string Time(DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToString(TimeFormat);
        }

        /// <summary>
		/// Formats the specified <see cref="System.DateTime"/> as a date + time.
        /// </summary>
        public static string DateTime(DateTime dt)
        {
            return dt.ToString(DateTimeFormat);
        }

        /// <summary>
		/// Formats the specified <see cref="System.DateTime"/> as a date + time.
        /// </summary>
        public static string DateTime(DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToString(DateTimeFormat);
        }
    }
}
