#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Statistics to store the number of messages.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CountStatistics : Statistics<uint>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="CountStatistics"/> with unit "?"
        /// </summary>
        /// <param name="name"></param>
        public CountStatistics(string name)
            : base(name)
        {
            Unit = "";
        }

        /// <summary>
        /// Creates an instance of <see cref="CountStatistics"/> with specified name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public CountStatistics(string name, uint value)
            : this(name)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a copy of the original <see cref="CountStatistics"/>
        /// </summary>
        /// <param name="copy"></param>
        public CountStatistics(CountStatistics copy)
            : base(copy)
        {
        }

        #endregion Constructors

        #region Overridden Public Methods

        public override object Clone()
        {
            return new CountStatistics(this);
        }

        public override IAverageStatistics NewAverageStatistics()
        {
            return new AverageCountStatistics(this);
        }

        #endregion
    }
}