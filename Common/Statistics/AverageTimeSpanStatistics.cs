#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Diagnostics;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Average time span statistics.
    /// </summary>
    public class AverageTimeSpanStatistics : AverageStatistics<TimeSpan>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="AverageTimeSpanStatistics"/>
        /// </summary>
        public AverageTimeSpanStatistics()
            : this("AverageTimeSpanStatistics")
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageTimeSpanStatistics"/> with specified name.
        /// </summary>
        /// <param name="name"></param>
        public AverageTimeSpanStatistics(string name)
            : base(name)
        {
            Value = new TimeSpan();
            ValueFormatter = TimeSpanFormatter.Format;
        }

        /// <summary>
        /// Creates a copy of the original <see cref="AverageTimeSpanStatistics"/> object.
        /// </summary>
        /// <param name="source"></param>
        public AverageTimeSpanStatistics(TimeSpanStatistics source)
            : base(source)
        {
        }

        #endregion

        #region Overridden Public Methods

        /// <summary>
        /// Adds a sample to the <see cref="AverageStatistics{T}.Samples"/> list.
        /// </summary>
        /// <typeparam name="TSample">Type of the sample value to be inserted</typeparam>
        /// <param name="sample"></param>
        public override void AddSample<TSample>(TSample sample)
        {
            if (sample is TimeSpan)
            {
                TimeSpan ts = (TimeSpan) (object) sample;
                Samples.Add(new TimeSpan(ts.Ticks));
                NewSamepleAdded = true;
            }
            else if (sample is TimeSpanStatistics)
            {
                TimeSpanStatistics stat = (TimeSpanStatistics) (object) sample;
                Samples.Add(stat.Value);
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
                foreach (TimeSpan sample in Samples)
                {
                    sum += sample.Ticks;
                }
                Value = new TimeSpan((long) sum/Samples.Count);
                NewSamepleAdded = false;
            }
        }

        #endregion
    }
}