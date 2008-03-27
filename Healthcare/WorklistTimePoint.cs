using System;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public class WorklistTimePoint : ValueObject, IEquatable<WorklistTimePoint>
    {
        public static readonly WorklistTimePoint Now = new WorklistTimePoint(TimeSpan.FromDays(0), 0);
        public static readonly WorklistTimePoint Today = new WorklistTimePoint(TimeSpan.FromDays(0), 1440);
        
        public static class Resolutions
        {
            // each value is the number of minutes in that period of time, except RealTime which is a special case
            public const long RealTime = 0;
            public const long Minute = 1;
            public const long Hour = 60;
            public const long Day = 1440;
        }



        private DateTime? _fixedValue;
        private TimeSpan? _relativeValue;
        private long _resolution;

        public WorklistTimePoint(DateTime date, long resolution)
            : this(date.Date, null, resolution)
        {
        }

        public WorklistTimePoint(TimeSpan offset, long resolution)
            :this(null, offset, resolution)
        {
        }

        private WorklistTimePoint(DateTime? date, TimeSpan? offset, long resolution)
        {
            _fixedValue = date;
            _relativeValue = offset;
            _resolution = resolution;
        }

        /// <summary>
        /// No-args constructor for NHibernate.
        /// </summary>
        private WorklistTimePoint()
        {
        }

        public bool IsFixed
        {
            get { return _fixedValue != null;}
        }

        public bool IsRelative
        {
            get { return !IsFixed; }
        }

        public DateTime? FixedValue
        {
            get { return _fixedValue; }
            private set { _fixedValue = value; }
        }

        public TimeSpan? RelativeValue
        {
            get { return _relativeValue; }
            private set { _relativeValue = value; }
        }

        public long Resolution
        {
            get { return _resolution; }
            private set { _resolution = value; }
        }

        public DateTime ResolveUp(DateTime currentTime)
        {
            return Resolve(currentTime, true);
        }

        public DateTime ResolveDown(DateTime currentTime)
        {
            return Resolve(currentTime, false);
        }

        private DateTime Resolve(DateTime currentTime, bool roundUp)
        {
            DateTime value = (_fixedValue != null) ? _fixedValue.Value : currentTime.Add(_relativeValue.Value);

            if (_resolution > Resolutions.RealTime)
            {
                long resolutionInTicks = _resolution * TimeSpan.TicksPerMinute;
                long roundedValueInTicks = value.Ticks - (value.Ticks % resolutionInTicks) + (roundUp ? resolutionInTicks : 0);

                value = new DateTime(roundedValueInTicks);
            }

            return value;
        }

        public override object Clone()
        {
            return new WorklistTimePoint(_fixedValue, _relativeValue, _resolution);
        }

        public static bool operator !=(WorklistTimePoint worklistTimePoint1, WorklistTimePoint worklistTimePoint2)
        {
            return !Equals(worklistTimePoint1, worklistTimePoint2);
        }

        public static bool operator ==(WorklistTimePoint worklistTimePoint1, WorklistTimePoint worklistTimePoint2)
        {
            return Equals(worklistTimePoint1, worklistTimePoint2);
        }

        public bool Equals(WorklistTimePoint worklistTimePoint)
        {
            if (worklistTimePoint == null) return false;
            if (!Equals(_fixedValue, worklistTimePoint._fixedValue)) return false;
            if (!Equals(_relativeValue, worklistTimePoint._relativeValue)) return false;
            if (!Equals(_resolution, worklistTimePoint._resolution)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as WorklistTimePoint);
        }

        public override int GetHashCode()
        {
            return _resolution.GetHashCode()
                 ^ ((_fixedValue == null) ? 0 : _fixedValue.GetHashCode())
                 ^ ((_relativeValue == null) ? 0 : _relativeValue.GetHashCode());
        }
    }
}
