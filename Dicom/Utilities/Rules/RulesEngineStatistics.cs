#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.Dicom.Utilities.Rules
{
    /// <summary>
    /// Stores the engine statistics of a rule engine.
    /// </summary>
    public class RulesEngineStatistics : StatisticsSet
    {    
        #region Constructors

        public RulesEngineStatistics()
        {
        }

        public RulesEngineStatistics(string name, string description)
            : base(name, description)
        {
            Context = new StatisticsContext(name);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the execution time of the rule engine in miliseconds.
        /// </summary>
        public TimeSpanStatistics ExecutionTime
        {
            get
            {
                if (this["ExecutionTime"] == null)
                {
                    this["ExecutionTime"] = new TimeSpanStatistics("ExecutionTime");
                }

                return (this["ExecutionTime"] as TimeSpanStatistics);
            }
        }

        /// <summary>
        /// Gets or sets the load time of the rule engine in miliseconds.
        /// </summary>
        public TimeSpanStatistics LoadTime
        {
            get
            {
                if (this["LoadTime"] == null)
                    this["LoadTime"] = new TimeSpanStatistics("LoadTime");
                return (this["LoadTime"] as TimeSpanStatistics);
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Reset the timer.
        /// </summary>
        public void Reset()
        {
            LoadTime.Reset();
            ExecutionTime.Reset();
        }

        #endregion
    }
}
