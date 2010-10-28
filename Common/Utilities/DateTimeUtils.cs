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
