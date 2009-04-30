#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Represents one end of a time range by which a worklist may be filtered.
    /// </summary>
    /// <remarks>
    /// Instances of this class are immutable.
    /// </remarks>
    public class WorklistTimePoint : ValueObject, IEquatable<WorklistTimePoint>
    {
        #region Constants

        /// <summary>
        /// Defines a time point that represents "now".
        /// </summary>
        public static readonly WorklistTimePoint Now = new WorklistTimePoint(TimeSpan.FromDays(0), 0);

        /// <summary>
        /// Defines a time point that represent "today".
        /// </summary>
        public static readonly WorklistTimePoint Today = new WorklistTimePoint(TimeSpan.FromDays(0), 1440);
        
        /// <summary>
        /// Defines a set of constants representing legal values for the <see cref="WorklistTimePoint.Resolution"/> property.
        /// </summary>
        public static class Resolutions
        {
            // each value is the number of minutes in that period of time, except RealTime which is a special case
            
            /// <summary>
            /// Represents real-time resolution.
            /// </summary>
            public const long RealTime = 0;

            /// <summary>
            /// Represents a resolution of 1 minute.
            /// </summary>
            public const long Minute = 1;

            /// <summary>
            /// Represents a resolution of 1 hour.
            /// </summary>
            public const long Hour = 60;

            /// <summary>
            /// Represents a resolution of 1 day.
            /// </summary>
            public const long Day = 1440;
        }

        #endregion


        private DateTime? _fixedValue;
        private TimeSpan? _relativeValue;
        private long _resolution;

        #region Constructors

        /// <summary>
        /// Constructor for creating a fixed time point.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="resolution"></param>
        public WorklistTimePoint(DateTime date, long resolution)
            : this(date.Date, null, resolution)
        {
        }

        /// <summary>
        /// Constructor for creating a relative time point.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="resolution"></param>
        public WorklistTimePoint(TimeSpan offset, long resolution)
            :this(null, offset, resolution)
        {
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="offset"></param>
        /// <param name="resolution"></param>
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

        #endregion

        #region Public members

        /// <summary>
        /// Gets a value indicating whether this instance represents a fixed point in time.
        /// </summary>
        public bool IsFixed
        {
            get { return _fixedValue != null;}
        }

        /// <summary>
        /// Gets a value indicating whether this instance represents a relative point in time.
        /// </summary>
        public bool IsRelative
        {
            get { return !IsFixed; }
        }

        /// <summary>
        /// Gets the fixed time value of this instance, or null if this instance does not represent a fixed point in time.
        /// </summary>
        public DateTime? FixedValue
        {
            get { return _fixedValue; }
            private set { _fixedValue = value; }
        }

        /// <summary>
        /// Gets the relative time value of this instance, or null if this instance does not represent a relative point in time.
        /// </summary>
        public TimeSpan? RelativeValue
        {
            get { return _relativeValue; }
            private set { _relativeValue = value; }
        }

        /// <summary>
        /// Gets the resolution of this instance, in minutes.
        /// </summary>
        public long Resolution
        {
            get { return _resolution; }
            private set { _resolution = value; }
        }

        /// <summary>
        /// Resolves this time point with respect to the specified current time, rounding up according
        /// to the value of the <see cref="Resolution"/> property.
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public DateTime ResolveUp(DateTime currentTime)
        {
            return Resolve(currentTime, true);
        }

        /// <summary>
        /// Resolves this time point with respect to the specified current time, rounding down according
        /// to the value of the <see cref="Resolution"/> property.
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public DateTime ResolveDown(DateTime currentTime)
        {
            return Resolve(currentTime, false);
        }

        #endregion

        #region Helpers

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

        #endregion

        #region Overrides

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

        #endregion
    }
}
