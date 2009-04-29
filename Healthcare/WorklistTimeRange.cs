using System;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Represents a time-range by which a worklist may be filtered.
    /// </summary>
    public class WorklistTimeRange : ValueObject, IEquatable<WorklistTimeRange>
    {
        /// <summary>
        /// Defines a time-range that originates at midnight this morning and ends at midnight tonight.
        /// </summary>
        public static readonly WorklistTimeRange Today = new WorklistTimeRange(WorklistTimePoint.Today, WorklistTimePoint.Today);

        private WorklistTimePoint _start;
        private WorklistTimePoint _end;

        /// <summary>
        /// Constructor.
        /// </summary>
        public WorklistTimeRange()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public WorklistTimeRange(WorklistTimePoint start, WorklistTimePoint end)
        {
            _start = start;
            _end = end;
        }

        #region Public members

        /// <summary>
        /// Gets or sets the beginning of the time range.
        /// </summary>
        public WorklistTimePoint Start
        {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary>
        /// Gets or sets the end of the time range.
        /// </summary>
        public WorklistTimePoint End
        {
            get { return _end; }
            set { _end = value; }
        }

        /// <summary>
        /// Applies this time range to the specified <see cref="ISearchCondition{T}"/>, using the specified current time.
        /// </summary>
        /// <remarks>
        /// T must be either a <see cref="DateTime"/> or a nullable <see cref="DateTime"/>.
        /// </remarks>
        /// <param name="condition"></param>
        /// <param name="currentTime"></param>
        public void Apply(ISearchCondition condition, DateTime currentTime)
        {
            DateTime startTime, endTime;
            Resolve(currentTime, out startTime, out endTime);

            ApplyRange(condition, _start != null, startTime, _end != null, endTime);

        }

        #endregion

        #region Helpers

        private void Resolve(DateTime currentTime, out DateTime startTime, out DateTime endTime)
        {
            startTime = _start != null ? _start.ResolveDown(currentTime) : DateTime.MinValue;
            endTime = _end != null ? _end.ResolveUp(currentTime) : DateTime.MaxValue;
        }

        private static void ApplyRange(ISearchCondition condition, bool hasLower, DateTime lower, bool hasUpper, DateTime upper)
        {
			// if both upper and lower bounded, use the between operator, otherwise use the >= and < operators
			// note that in SQL server, BETWEEN a AND b means a <= x < b (it is asymmetrical), however
			// this is not necessarily the case in other database servers... not much we can do about this.
			if (hasLower && hasUpper)
            {
                condition.Between(lower, upper);
            }
            else if (hasLower)
            {
                condition.MoreThanOrEqualTo(lower);
            }
            else if (hasUpper)
            {
                condition.LessThan(upper);
            }
        }

        #endregion

        #region overrides

        public override object Clone()
        {
            return new WorklistTimeRange(
                _start != null ? (WorklistTimePoint) _start.Clone() : null,
                _end != null ? (WorklistTimePoint)_end.Clone() : null);
        }

        public bool Equals(WorklistTimeRange worklistTimeRange)
        {
            if (worklistTimeRange == null) return false;
            return Equals(_start, worklistTimeRange._start) && Equals(_end, worklistTimeRange._end);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as WorklistTimeRange);
        }

        public override int GetHashCode()
        {
            return (_start != null ? _start.GetHashCode() : 0) + 29*(_end != null ? _end.GetHashCode() : 0);
        }

        #endregion
    }
}
