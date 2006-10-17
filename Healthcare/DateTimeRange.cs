using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {

    /// <summary>
    /// Represents a date/time range.  Either the From or Until property may be null, in which
    /// case the range is open-ended.
    /// </summary>
	public class DateTimeRange : IEquatable<DateTimeRange>
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
    }
}