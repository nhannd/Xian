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
    public static class VisitNumberFormat
    {
        /// <summary>
        /// Formats the MRN according to the default format as specified in <see cref="FormatSettings"/>
        /// </summary>
        /// <param name="vn"></param>
        /// <returns></returns>
        public static string Format(CompositeIdentifierDetail vn)
        {
            return Format(vn, FormatSettings.Default.VisitNumberDefaultFormat);
        }

        /// <summary>
        /// Formats the MRN number according to the specified format string.
        /// </summary>
        /// <remarks>
        /// Valid format specifiers are as follows:
        ///     %N - number
        ///     %A - assigning authority
        /// </remarks>
        /// <param name="vn"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(CompositeIdentifierDetail vn, string format)
        {
            string result = format;
            result = result.Replace("%N", vn.Id ?? "");
            result = result.Replace("%A", vn.AssigningAuthority == null ? "" : vn.AssigningAuthority.Code);
            return result.Trim();
        }
    }
}
