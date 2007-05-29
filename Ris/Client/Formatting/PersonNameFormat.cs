using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Formatting
{
    public static class PersonNameFormat
    {
        /// <summary>
        /// Formats the person name according to the default person name format as specified in <see cref="FormatSettings"/>
        /// </summary>
        /// <param name="pn"></param>
        /// <returns></returns>
        public static string Format(PersonNameDetail pn)
        {
            return Format(pn, FormatSettings.Default.PersonNameDefaultFormat);
        }

        /// <summary>
        /// Formats the specified person name according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %F - full family name
        ///     %f - family name initial
        ///     %G - full given name
        ///     %g - given name initial
        ///     %M - full middle name
        ///     %m - middle initial
        /// </remarks>
        /// <param name="pn"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(PersonNameDetail pn, string format)
        {
            // G g F f M m
            string result = format;
            result = result.Replace("%G", pn.GivenName == null ? "" : pn.GivenName);
            result = result.Replace("%g", pn.GivenName == null || pn.GivenName.Length == 0 ? "" : pn.GivenName.Substring(0, 1));
            result = result.Replace("%F", pn.FamilyName == null ? "" : pn.FamilyName);
            result = result.Replace("%f", pn.FamilyName == null || pn.FamilyName.Length == 0 ? "" : pn.FamilyName.Substring(0, 1));
            result = result.Replace("%M", pn.MiddleName == null ? "" : pn.MiddleName);
            result = result.Replace("%m", pn.MiddleName == null || pn.MiddleName.Length == 0 ? "" : pn.MiddleName.Substring(0, 1));

            return result.Trim();
        }
    }
}
