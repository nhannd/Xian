#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Client
{
    public abstract class RelativeTime : IComparable<RelativeTime>, IComparable
    {
        private int _value;

        protected RelativeTime(int value)
        {
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }

        public override bool Equals(object obj)
        {
            RelativeTime other = obj as RelativeTime;
            if (other == null)
                return false;
            
            return this.GetType() == other.GetType() && _value == other._value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        #region IComparable<RelativeTime> Members

        public int CompareTo(RelativeTime other)
        {
            if (this.GetType() != other.GetType())
                throw new InvalidOperationException("Only instances of the same class can be compared");
            return _value.CompareTo(other._value);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            RelativeTime that = obj as RelativeTime;
            if(that == null)
                throw new InvalidOperationException("Cannot compare to null.");
            return CompareTo(that);
        }

        #endregion
    }

    public class RelativeTimeInDays : RelativeTime
    {
        public RelativeTimeInDays(int days)
            :base(days)
        {
        }

        public override string ToString()
        {
            if (this.Value == 0)
                return "Today";
            if (this.Value == 1)
                return "Tomorrow";
            if (this.Value == -1)
                return "Yesterday";

            return string.Format("{0} days {1}", Math.Abs(this.Value), this.Value > 0 ? "from now" : "ago");
        }
    }

    public class RelativeTimeInHours : RelativeTime
    {
        public RelativeTimeInHours(int hours)
            : base(hours)
        {
        }

        public override string ToString()
        {
            if (this.Value == 0)
                return "Now";

			var absoluteValue = Math.Abs(this.Value);
			var formatting = absoluteValue == 1 ? "{0} hour {1}" : "{0} hours {1}";
			return string.Format(formatting, absoluteValue, this.Value > 0 ? "from now" : "ago");
        }
    }
}
