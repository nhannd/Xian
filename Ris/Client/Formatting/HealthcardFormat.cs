#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Formatting
{
    public static class HealthcardFormat
    {
        /// <summary>
        /// Formats the healthcard according to the default format as specified in <see cref="FormatSettings"/>
        /// </summary>
        /// <param name="hc"></param>
        /// <returns></returns>
        public static string Format(HealthcardDetail hc)
        {
            return Format(hc, FormatSettings.Default.HealthcardDefaultFormat);
        }

        /// <summary>
        /// Formats the healthcard number according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %N - number
        ///     %A - assigning authority
        ///     %V - version code
        ///     %X - expiry date
        /// </remarks>
        /// <param name="hc"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(HealthcardDetail hc, string format)
        {
            string result = format;
            result = result.Replace("%N", hc.Id ?? "");
            result = result.Replace("%A", hc.AssigningAuthority == null ? "" : hc.AssigningAuthority.Code);
            result = result.Replace("%V", hc.VersionCode ?? "");
            result = result.Replace("%X", ClearCanvas.Desktop.Format.Date(hc.ExpiryDate));

            return result.Trim();
        }
    }
}
