#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

		public override bool Evaluate(IStudyItem item)
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

		public override bool Evaluate(IStudyItem item)
		{
			return this.Column.GetTypedValue(item).Equals(this.Value);
		}
	}

	public class GreaterValueFilterPredicate<T> : ValueFilterPredicate<T> where T : IComparable<T>, IEquatable<T>
	{
		public GreaterValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public override bool Evaluate(IStudyItem item)
		{
			return this.Column.GetTypedValue(item).CompareTo(this.Value) > 0;
		}
	}

	public class LesserValueFilterPredicate<T> : ValueFilterPredicate<T> where T : IComparable<T>, IEquatable<T>
	{
		public LesserValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public override bool Evaluate(IStudyItem item)
		{
			return this.Column.GetTypedValue(item).CompareTo(this.Value) < 0;
		}
	}

	public class GreaterOrEqualValueFilterPredicate<T> : ValueFilterPredicate<T> where T : IComparable<T>, IEquatable<T>
	{
		public GreaterOrEqualValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public override bool Evaluate(IStudyItem item)
		{
			return this.Column.GetTypedValue(item).CompareTo(this.Value) >= 0;
		}
	}

	public class LesserOrEqualValueFilterPredicate<T> : ValueFilterPredicate<T> where T : IComparable<T>, IEquatable<T>
	{
		public LesserOrEqualValueFilterPredicate(StudyFilterColumnBase<T> column, T value) : base(column, value) {}

		public override bool Evaluate(IStudyItem item)
		{
			return this.Column.GetTypedValue(item).CompareTo(this.Value) <= 0;
		}
	}
}