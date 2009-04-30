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
            if(Math.Abs(this.Value) == 1)
                return string.Format("1 hour {1}", this.Value > 0 ? "from now" : "ago");

            return string.Format("{0} hours {1}", Math.Abs(this.Value), this.Value > 0 ? "from now" : "ago");
        }
    }
}
