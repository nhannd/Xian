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
    /// Message rate formatter class
    /// </summary>
    public static class MessageRateFormatter
    {
        /// <summary>
        /// Formats a rate in msg/sec unit
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static string Format(double rate)
        {
            return String.Format("{0:0.00} msg/s", rate);
        }
    }
}