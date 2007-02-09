using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Utility class that assists with formatting objects for display.  This class is configurable through
    /// the <see cref="FormatSettings"/> class.
    /// </summary>
    public static class Format
    {
        /// <summary>
        /// Gets the default date format string
        /// </summary>
        public static string DateFormat
        {
            get { return FormatSettings.Default.DateFormat; }
        }

        /// <summary>
        /// Gets the default time format string
        /// </summary>
        public static string TimeFormat
        { 
            get { return FormatSettings.Default.TimeFormat; } 
        }

        /// <summary>
        /// Gets the default date-time format string
        /// </summary>
        public static string DateTimeFormat
        {
            get { return FormatSettings.Default.DateTimeFormat; }
        }

        /// <summary>
        /// Formats the specified date-time as a date
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string Date(DateTime dt)
        {
            return dt.ToString(DateFormat);
        }

        /// <summary>
        /// Formats the specified date-time as a date, returning an empty string if null
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string Date(DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToString(DateFormat);
        }

        /// <summary>
        /// Formats the specified date-time as a time
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string Time(DateTime dt)
        {
            return dt.ToString(TimeFormat);
        }

        /// <summary>
        /// Formats the specified date-time as a time, returning an empty string if null
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string Time(DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToString(TimeFormat);
        }

        /// <summary>
        /// Formats the specified date-time as a date + time
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DateTime(DateTime dt)
        {
            return dt.ToString(DateTimeFormat);
        }

        /// <summary>
        /// Formats the specified date-time as a date + time
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DateTime(DateTime? dt)
        {
            return dt == null ? "" : dt.Value.ToString(DateTimeFormat);
        }

        /// <summary>
        /// Formats the specified object.  For now, this method just calls the object's ToString() method.
        /// In future, we may want to add the ability to hook in formatting overrides via extension points.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Custom(object obj)
        {
            if (obj is IFormattable)
            {
                return (obj as IFormattable).ToString(null, null);
            }
            return (obj == null ? "" : obj.ToString());
        }
    }
}
