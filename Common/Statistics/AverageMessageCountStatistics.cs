#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Diagnostics;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Average message count statistics.
    /// </summary>
    public class AverageMessageCountStatistics : AverageStatistics<ulong>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="AverageMessageCountStatistics"/>
        /// </summary>
        public AverageMessageCountStatistics()
            : this("AverageMessageCountStatistics")
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageMessageCountStatistics"/> with a specified name.
        /// </summary>
        /// <param name="name">Name of the <see cref="AverageMessageCountStatistics"/> to be created</param>
        public AverageMessageCountStatistics(string name)
            : base(name)
        {
            Unit = "msg";
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageMessageCountStatistics"/> for a specified <see cref="MessageCountStatistics"/> object
        /// </summary>
        /// <param name="source">The <see cref="MessageCountStatistics"/> for which the <see cref="AverageMessageCountStatistics"/> to be created is based on</param>
        public AverageMessageCountStatistics(MessageCountStatistics source)
            : base(source)
        {
        }

        #endregion

        #region Overridden Public Methods

        /// <summary>
        /// Adds a sample to the <see cref="AverageStatistics{T}.Samples"/> list.
        /// </summary>
        /// <typeparam name="TSample"></typeparam>
        /// <param name="sample"></param>
        public override void AddSample<TSample>(TSample sample)
        {
            if (sample is ulong)
            {
                Samples.Add((ulong) (object) sample);
                NewSamepleAdded = true;
            }
            else if (sample is long)
            {
                Samples.Add((ulong) (object) sample);
                NewSamepleAdded = true;
            }
            else if (sample is int)
            {
                Samples.Add((ulong) (object) sample);
                NewSamepleAdded = true;
            }
            else if (sample is uint)
            {
                Samples.Add((ulong) (object) sample);
                NewSamepleAdded = true;
            }
            else if (sample is MessageCountStatistics)
            {
                Samples.Add(((MessageCountStatistics) (object) sample).Value);
                NewSamepleAdded = true;
            }
            else
            {
                base.AddSample(sample);
            }
        }

        #endregion

        #region Overridden Protected Methods

        /// <summary>
        /// Computes the average for the samples in <see cref="AverageStatistics{T}.Samples"/> list.
        /// </summary>
        protected override void ComputeAverage()
        {
            if (NewSamepleAdded)
            {
                Debug.Assert(Samples.Count > 0);

                double sum = 0;
                foreach (ulong sample in Samples)
                {
                    sum += sample;
                }
                Value = (ulong) (sum/Samples.Count);
                NewSamepleAdded = false;
            }
        }

        #endregion
    }
}