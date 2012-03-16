
using ClearCanvas.Common.Statistics;

namespace ClearCanvas.Dicom.Utilities.Rules
{
    /// <summary>
    /// Stores the engine statistics of a rule engine.
    /// </summary>
    public class RulesEngineStatistics : StatisticsSet
    {
        #region Private members

        #endregion Private members

        public void Reset()
        {
            LoadTime.Reset();
            ExecutionTime.Reset();
        }

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
    }
}
