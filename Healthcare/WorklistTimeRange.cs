using System;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public class WorklistTimeRange : ValueObject, IEquatable<WorklistTimeRange>
    {
        public static readonly WorklistTimeRange Today = new WorklistTimeRange(WorklistTimePoint.Today, WorklistTimePoint.Today);

        private WorklistTimePoint _start;
        private WorklistTimePoint _end;

        public WorklistTimeRange()
        {

        }

        public WorklistTimeRange(WorklistTimePoint start, WorklistTimePoint end)
        {
            _start = start;
            _end = end;
        }

        public WorklistTimePoint Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public WorklistTimePoint End
        {
            get { return _end; }
            set { _end = value; }
        }

        public void Apply(ISearchCondition<DateTime> condition, DateTime currentTime)
        {
            DateTime startTime, endTime;
            Resolve(currentTime, out startTime, out endTime);

            ApplyRange(condition, _start != null, startTime, _end != null, endTime);
        }

        public void Apply(ISearchCondition<DateTime?> condition, DateTime currentTime)
        {
            DateTime startTime, endTime;
            Resolve(currentTime, out startTime, out endTime);

            ApplyRange<DateTime?>(condition, _start != null, startTime, _end != null, endTime);

        }

        private void Resolve(DateTime currentTime, out DateTime startTime, out DateTime endTime)
        {
            startTime = _start != null ? _start.ResolveDown(currentTime) : DateTime.MinValue;
            endTime = _end != null ? _end.ResolveUp(currentTime) : DateTime.MaxValue;
        }

        private static void ApplyRange<T>(ISearchCondition<T> condition, bool hasLower, T lower, bool hasUpper, T upper)
        {
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
                condition.LessThanOrEqualTo(upper);
            }
        }

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
