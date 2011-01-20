#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Statistics to store the number of bytes
    /// </summary>
    /// <remarks>
    /// <see cref="IStatistics.FormattedValue"/> of the <see cref="ByteCountStatistics"/> has unit of "GB", 'MB" or "KB"
    /// depending on the number of bytes being set.
    /// </remarks>
    public class ByteCountStatistics : Statistics<ulong>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="ByteCountStatistics"/>
        /// </summary>
        /// <param name="name"></param>
        public ByteCountStatistics(string name)
            : this(name, 0)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="ByteCountStatistics"/> with specified name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ByteCountStatistics(string name, ulong value)
            : base(name, value)
        {
            ValueFormatter = ByteCountFormatter.Format;
        }

        /// <summary>
        /// Creates a copy of the original <see cref="ByteCountStatistics"/> object.
        /// </summary>
        /// <param name="source">The original <see cref="ByteCountStatistics"/> to copy</param>
        public ByteCountStatistics(ByteCountStatistics source)
            : base(source)
        {
        }

        #endregion

        #region Overridden Public Methods

        /// <summary>
        /// Creates a copy of the current statistics
        /// </summary>
        /// <returns>A copy of the current <see cref="RateStatistics"/> object</returns>
        public override object Clone()
        {
            return new ByteCountStatistics(this);
        }

        /// <summary>
        /// Returns a new average statistics object corresponding to the current statistics
        /// </summary>
        /// <returns>A <see cref="AverageRateStatistics"/> object</returns>
        public override IAverageStatistics NewAverageStatistics()
        {
            return new AverageByteCountStatistics(this);
        }

        #endregion
    }
}