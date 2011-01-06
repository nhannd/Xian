#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Provide methods to format a number of bytes in different units
    /// </summary>
    public static class ByteCountFormatter
    {
        #region Constants

        private const double GIGABYTES = 1024*MEGABYTES;
        private const double KILOBYTES = 1024;
        private const double MEGABYTES = 1024*KILOBYTES;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Formats a byte number in different units
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Format(ulong bytes)
        {
            if (bytes > GIGABYTES)
                return String.Format("{0:0.00} GB", bytes/GIGABYTES);
            if (bytes > MEGABYTES)
                return String.Format("{0:0.00} MB", bytes/MEGABYTES);
            if (bytes > KILOBYTES)
                return String.Format("{0:0.00} KB", bytes/KILOBYTES);

            return String.Format("{0} bytes", bytes);
        }

        #endregion
    }
}