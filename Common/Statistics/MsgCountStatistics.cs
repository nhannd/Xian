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
    public class MessageCountStatistics : Statistics<ulong>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="MessageCountStatistics"/> with unit "msg"
        /// </summary>
        /// <param name="name"></param>
        public MessageCountStatistics(string name)
            : base(name)
        {
            Unit = "msg";
        }

        /// <summary>
        /// Creates an instance of <see cref="MessageCountStatistics"/> with specified name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public MessageCountStatistics(string name, ulong value)
            : this(name)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a copy of the original <see cref="MessageCountStatistics"/>
        /// </summary>
        /// <param name="copy"></param>
        public MessageCountStatistics(MessageCountStatistics copy)
            : base(copy)
        {
        }

        #endregion Constructors

        #region Overridden Public Methods

        public override object Clone()
        {
            return new MessageCountStatistics(this);
        }

        public override IAverageStatistics NewAverageStatistics()
        {
            return new AverageMessageCountStatistics(this);
        }

        #endregion
    }
}