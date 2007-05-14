using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Formatting
{
    public static class AddressFormat
    {
        /// <summary>
        /// Formats the address according to the default format as specified in <see cref="FormatSettings"/>
        /// </summary>
        /// <param name="hc"></param>
        /// <returns></returns>
        public static string Format(AddressDetail addr)
        {
            return Format(addr, FormatSettings.Default.AddressDefaultFormat);
        }

        /// <summary>
        /// Formats the address according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %S - street address, including number and unit/apartment number
        ///     %V - city
        ///     %P - Province
        ///     %Z - Postal/Zip Code
        ///     %C - country
        /// </remarks>
        /// <param name="pn"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(AddressDetail a, string format)
        {
            string result = format;
            result = result.Replace("%S", a.Street == null ? "" : FormatStreet(a));
            result = result.Replace("%V", a.City == null ? "" : a.City);
            result = result.Replace("%P", a.Province == null ? "" : a.Province);
            result = result.Replace("%Z", a.PostalCode == null ? "" : a.PostalCode);
            result = result.Replace("%C", a.Country == null ? "" : a.Country);

            return result.Trim();
        }

        private static string FormatStreet(AddressDetail a)
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(a.Unit))
            {
                sb.Append(a.Unit);
                sb.Append("-");
            }
            sb.Append(a.Street);
            return sb.ToString();
        }

    }
}
