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

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities
{
	public class ValueFilterPredicate : FilterPredicate
	{
		public readonly StudyFilterColumn Column;
		public readonly object Value;

		public ValueFilterPredicate(StudyFilterColumn column, object value)
		{
			this.Column = column;
			this.Value = value;
		}

		public override bool Evaluate(StudyItem item)
		{
			return this.Column.GetValue(item).Equals(this.Value);
		}
	}

	public class ValueFilterPredicate<T> : ValueFilterPredicate where T : IEquatable<T>
	{
		public ValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public new StudyFilterColumnBase<T> Column
		{
			get { return (StudyFilterColumnBase<T>) base.Column; }
		}

		public new T Value
		{
			get { return (T) base.Value; }
		}

		public override bool Evaluate(StudyItem item)
		{
			return this.Column.GetTypedValue(item).Equals(this.Value);
		}
	}

	public class GreaterValueFilterPredicate<T> : ValueFilterPredicate<T> where T : IComparable<T>, IEquatable<T>
	{
		public GreaterValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public override bool Evaluate(StudyItem item)
		{
			return this.Column.GetTypedValue(item).CompareTo(this.Value) > 0;
		}
	}

	public class LesserValueFilterPredicate<T> : ValueFilterPredicate<T> where T : IComparable<T>, IEquatable<T>
	{
		public LesserValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public override bool Evaluate(StudyItem item)
		{
			return this.Column.GetTypedValue(item).CompareTo(this.Value) < 0;
		}
	}

	public class GreaterOrEqualValueFilterPredicate<T> : ValueFilterPredicate<T> where T : IComparable<T>, IEquatable<T>
	{
		public GreaterOrEqualValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public override bool Evaluate(StudyItem item)
		{
			return this.Column.GetTypedValue(item).CompareTo(this.Value) >= 0;
		}
	}

	public class LesserOrEqualValueFilterPredicate<T> : ValueFilterPredicate<T> where T : IComparable<T>, IEquatable<T>
	{
		public LesserOrEqualValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public override bool Evaluate(StudyItem item)
		{
			return this.Column.GetTypedValue(item).CompareTo(this.Value) <= 0;
		}
	}
}