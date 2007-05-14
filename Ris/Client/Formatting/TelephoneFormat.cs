using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Formatting
{
    public static class TelephoneFormat
    {
        /// <summary>
        /// Formats the telephone number according to the default format as specified in <see cref="FormatSettings"/>
        /// </summary>
        /// <param name="hc"></param>
        /// <returns></returns>
        public static string Format(TelephoneDetail tn)
        {
            return Format(tn, FormatSettings.Default.TelephoneNumberDefaultFormat);
        }

        /// <summary>
        /// Formats the address according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %C - country code, preceded by +
        ///     %c - country code if different from default country code specified in <see cref="FormatSettings"/>
        ///     %A - area code
        ///     %N - phone number in form XXX-XXXX 
        ///     %X - extension, preceded by x
        /// </remarks>
        /// <param name="pn"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(TelephoneDetail tn, string format)
        {
            string result = format;
            result = result.Replace("%C", tn.CountryCode == null ? "" : string.Format("+{0}", tn.CountryCode));

            result = result.Replace("%c",
                (tn.CountryCode == null || tn.CountryCode == FormatSettings.Default.TelephoneNumberSuppressCountryCode) ? ""
                : string.Format("+{0}", tn.CountryCode));

            result = result.Replace("%A", tn.AreaCode == null ? "" : tn.AreaCode);
            result = result.Replace("%N", tn.Number == null ? "" : string.Format("{0}-{1}", tn.Number.Substring(0, 3), tn.Number.Substring(3)));
            result = result.Replace("%X", string.IsNullOrEmpty(tn.Extension) ? "" : string.Format("x{0}", tn.Extension));

            return result.Trim();
        }

    }
}
