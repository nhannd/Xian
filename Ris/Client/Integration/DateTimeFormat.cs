using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Integration
{
    public static class DateTimeFormat
    {
        /// <summary>
        /// Formats a date according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     YYYY - year
        ///     MM   - month
        ///     DD   - day
        ///     hh   - hour
        ///     mm   - minute
        ///     ss   - second
        ///     xxx   - millisecond
        /// </remarks>
        /// <param name="pn"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(DateTime dateTime, string format)
        {
            string result = format;
            result = result.Replace("YYYY", dateTime.Year.ToString());
            result = result.Replace("MM", dateTime.Month.ToString("00"));
            result = result.Replace("DD", dateTime.Day.ToString("00"));
            result = result.Replace("hh", dateTime.Hour.ToString("00"));
            result = result.Replace("mm", dateTime.Minute.ToString("00"));
            result = result.Replace("ss", dateTime.Second.ToString("00"));
            result = result.Replace("xxx", dateTime.Millisecond.ToString("000"));
            return result.Trim();
        }
    }
}
