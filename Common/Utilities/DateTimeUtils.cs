using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Provides convenient utilities for working with <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeUtils
    {
        /// <summary>
        /// Parses an ISO 8601 formatted date string, without milliseconds or timezone.
        /// </summary>
        /// <param name="isoDateString"></param>
        /// <returns></returns>
        public static DateTime? ParseISO(string isoDateString)
        {
            if (string.IsNullOrEmpty(isoDateString))
                return null;

            return DateTime.ParseExact(isoDateString, "s", null);
        }

        /// <summary>
        /// Formats the specified <see cref="DateTime"/> as ISO 8601, without milliseconds or timezone.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string FormatISO(DateTime dt)
        {
            return dt.ToString("s");
        }
    }
}
