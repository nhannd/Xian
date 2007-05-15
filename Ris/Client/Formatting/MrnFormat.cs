using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Formatting
{
    public static class MrnFormat
    {
        /// <summary>
        /// Formats the MRN according to the default format as specified in <see cref="FormatSettings"/>
        /// </summary>
        /// <param name="hc"></param>
        /// <returns></returns>
        public static string Format(MrnDetail mrn)
        {
            return Format(mrn, FormatSettings.Default.MrnDefaultFormat);
        }

        /// <summary>
        /// Formats the MRN number according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %N - number
        ///     %A - assigning authority
        /// </remarks>
        /// <param name="pn"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(MrnDetail mrn, string format)
        {
            string result = format;
            result = result.Replace("%N", mrn.Id == null ? "" : mrn.Id);
            result = result.Replace("%A", mrn.AssigningAuthority == null ? "" : mrn.AssigningAuthority);
            return result.Trim();
        }
    }
}
