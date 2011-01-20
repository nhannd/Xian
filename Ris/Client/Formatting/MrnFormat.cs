#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
    public static class MrnFormat
    {
        /// <summary>
        /// Formats the MRN according to the default format as specified in <see cref="FormatSettings"/>
        /// </summary>
        /// <param name="mrn"></param>
        /// <returns></returns>
        public static string Format(CompositeIdentifierDetail mrn)
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
        /// <param name="mrn"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string Format(CompositeIdentifierDetail mrn, string format)
        {
            string result = format;
            result = result.Replace("%N", mrn.Id ?? "");
            result = result.Replace("%A", mrn.AssigningAuthority == null ? "" : mrn.AssigningAuthority.Code);
            return result.Trim();
        }
    }
}
