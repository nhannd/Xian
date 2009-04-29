#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;


namespace ClearCanvas.Healthcare {

    /// <summary>
    /// Represents a date/time range.  Either the From or Until property may be null, in which
    /// case the range extends infinitely in that direction.
    /// </summary>
    [Serializable]
	public class DateTimeRange : IEquatable<DateTimeRange>, ICloneable, IAuditFormattable
	{
        private DateTime? _from;
        private DateTime? _until;

        /// <summary>
        /// Constructor
        /// </summary>
        public DateTimeRange()
        {
        }

        /// <summary>
        /// Constructs a new date/time range from the specified times
        /// </summary>
        /// <param name="from"></param>
        /// <param name="until"></param>
        public DateTimeRange(DateTime? from, DateTime? until)
        {
            _from = from;
            _until = until;
        }
       
        /// <summary>
        /// Gets a value indicating whether this range is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return (_from ?? DateTime.MinValue) > (_until ?? DateTime.MaxValue); }
        }

        public bool IsLowerBounded
        {
            get { return _from.HasValue; }
        }

        public bool IsUpperBounded
        {
            get { return _until.HasValue; }
        }

        /// <summary>
        /// Tests if this range includes the specified time
        /// </summary>
        /// <param name="time">The time to test for</param>
        /// <returns>True if this range includes the specified time</returns>
        public bool Includes(DateTime time)
        {
            DateTime start = _from ?? DateTime.MinValue;
            DateTime end = _until ?? DateTime.MaxValue;

            return (time <= end) && (time >= start);
        }

        /// <summary>
        /// Tests if this range excludes the specified time
        /// </summary>
        /// <param name="time">The time to test for</param>
        /// <returns>True if this range excludes the specified time</returns>
        public bool Excludes(DateTime time)
        {
            return !Includes(time);
        }

        /// <summary>
        /// Returns a new instance of <see cref="DateTimeRange"/> representing the bounding range
        /// of this and other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public DateTimeRange GetBoundingRange(DateTimeRange other)
        {
            Platform.CheckForNullReference(other, "other");

            DateTime?[] lowers = new DateTime?[] { this.From, other.From };
            DateTime?[] uppers = new DateTime?[] { this.Until, other.Until };

            DateTime? lower = CollectionUtils.Min(lowers, null, CompareLower);
            DateTime? upper = CollectionUtils.Max(uppers, null, CompareUpper);

            return new DateTimeRange(lower, upper);
        }

        /// <summary>
        /// Returns a new instance of <see cref="DateTimeRange"/> representing the intersection
        /// of this and other.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public DateTimeRange Intersect(DateTimeRange other)
        {
            Platform.CheckForNullReference(other, "other");

            DateTime?[] lowers = new DateTime?[] { this.From, other.From };
            DateTime?[] uppers = new DateTime?[] { this.Until, other.Until };

            DateTime? lower = CollectionUtils.Max(lowers, null, CompareLower);
            DateTime? upper = CollectionUtils.Min(uppers, null, CompareUpper);

            return new DateTimeRange(lower, upper);
        }

        /// <summary>
        /// The beginning of the range
        /// </summary>
        public DateTime? From
        {
            get { return _from; }
            set { _from = value; }
        }

        /// <summary>
        /// The end of the range
        /// </summary>
        public DateTime? Until
        {
            get { return _until; }
            set { _until = value; }
        }

        /// <summary>
        /// Tests for value equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as DateTimeRange);
        }

        public override int GetHashCode()
        {
            return (_from == null ? 0 : _from.GetHashCode()) ^ (_until == null ? 0 : _until.GetHashCode());
        }

        #region IEquatable<DateTimeRange> Members

        /// <summary>
        /// Tests for value equality
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public bool Equals(DateTimeRange that)
        {
            return that != null && that._from == this._from && that._until == this._until;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new DateTimeRange(this.From, this.Until);
        }

        #endregion

        private static int CompareUpper(DateTime? x, DateTime? y)
        {
            DateTime x1 = x ?? DateTime.MaxValue;
            DateTime y1 = y ?? DateTime.MaxValue;
            return x1.CompareTo(y1);
        }

        private static int CompareLower(DateTime? x, DateTime? y)
        {
            DateTime x1 = x ?? DateTime.MinValue;
            DateTime y1 = y ?? DateTime.MinValue;
            return x1.CompareTo(y1);
        }

		#region IAuditFormattable Members

		void IAuditFormattable.Write(IObjectWriter writer)
		{
			writer.WriteProperty("From", _from);
			writer.WriteProperty("Until", _until);
		}

		#endregion
	}
}