#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
    /// Transmission rate formatter class.
    /// </summary>
    public static class TransmissionRateFormatter
    {
        #region Constants

        private const double GIGABYTES = 1024*MEGABYTES;
        private const double KILOBYTES = 1024;
        private const double MEGABYTES = 1024*KILOBYTES;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Formats a transmission rate in appropriate units
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static string Format(double rate)
        {
            if (rate > GIGABYTES)
                return String.Format("{0:0.00} GB/s", rate/GIGABYTES);
            if (rate > MEGABYTES)
                return String.Format("{0:0.00} MB/s", rate/MEGABYTES);
            if (rate > KILOBYTES)
                return String.Format("{0:0.00} KB/s", rate/KILOBYTES);

            return String.Format("{0:0} bytes/s", rate);
        }

        #endregion
    }
}