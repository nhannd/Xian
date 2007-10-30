using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Used to store the elapsed time between two "events"
    /// </summary>
    /// <remarks>
    /// <para>
    /// Users call <seealso cref="Begin()"/> and <seealso cref="End()"/> to signal
    /// the beginning and at of the events. The elapsed time can be determinted using <seealso cref="ElapsedTimeInMs"/>
    /// </para>
    /// </remarks>
    /// 
    public class TimeSpanStatistics : BaseStatistics
    {
        #region Protected Variables
        protected long _elapsedTime;
        
        #endregion

        #region Public Properties
        /// <summary>
        /// Elapsed time between two "events" (In 100 nanoseconds)
        /// </summary>
        /// <remarks>
        /// "Events" are marked by calling <seealso cref="Begin()"/> and <seealso cref="End()"/>
        /// </remarks>
        public long ElapsedTimeInMs
        {
            get
            {
                return _elapsedTime;
            }
            set
            {
                _elapsedTime = value;
                _statsValuesCollection["@ElapsedTimeInMs"] = value;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <seealso cref="TimeSpanStatistics"/>.
        /// </summary>
        /// <param name="desc"></param>
        public TimeSpanStatistics(string desc)
            : base(desc)
        {
        }

        #endregion

        #region Protected Overridden Methods
        
        protected override void OnBegin()
        {
            // NOOP
        }

        protected override void OnEnd()
        {
            ElapsedTimeInMs = (_endTick - _beginTick) / 10000; // convert 100 ns to ms
        }

        #endregion

       
    }
}
