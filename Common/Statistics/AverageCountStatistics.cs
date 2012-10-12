#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
    public class AverageCountStatistics : AverageStatistics<uint>
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="AverageCountStatistics"/>
        /// </summary>
        public AverageCountStatistics()
            : this("AverageCountStatistics")
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageCountStatistics"/> with a specified name.
        /// </summary>
        /// <param name="name">Name of the <see cref="AverageCountStatistics"/> to be created</param>
        public AverageCountStatistics(string name)
            : base(name)
        {
            Unit = "";
        }

        /// <summary>
        /// Creates an instance of <see cref="AverageCountStatistics"/> for a specified <see cref="CountStatistics"/> object
        /// </summary>
        /// <param name="source">The <see cref="CountStatistics"/> for which the <see cref="AverageCountStatistics"/> to be created is based on</param>
        public AverageCountStatistics(CountStatistics source)
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
            if (sample is uint)
            {
                Samples.Add((uint)(object)sample);
                NewSamepleAdded = true;
            }
            if (sample is int)
            {
                Samples.Add((uint)(object)sample);
                NewSamepleAdded = true;
            }
            else if (sample is CountStatistics)
            {
                Samples.Add(((CountStatistics)(object)sample).Value);
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
                foreach (int sample in Samples)
                {
                    sum += sample;
                }
                Value = (uint)(sum / Samples.Count);
                NewSamepleAdded = false;
            }
        }

        #endregion
    }
}