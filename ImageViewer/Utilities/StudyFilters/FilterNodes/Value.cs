namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.FilterNodes
{
	public abstract class Value : FilterNodeBase
	{
		protected readonly StudyFilterColumn Column;

		protected Value(StudyFilterColumn column)
		{
			this.Column = column;
		}
	}

	public sealed class ValueEquals : Value
	{
		private readonly string _compare;

		public ValueEquals(StudyFilterColumn column, string compare) : base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetValue(item).Equals(_compare);
		}

		public override string ToString()
		{
			return string.Format("{0} = {1}", Column.ToString(), _compare);
		}
	}

	public sealed class ValueGreater : Value
	{
		private readonly string _compare;

		public ValueGreater(StudyFilterColumn column, string compare)
			: base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetValue(item).CompareTo(_compare) > 0;
		}

		public override string ToString()
		{
			return string.Format("{0} > {1}", Column.ToString(), _compare);
		}
	}

	public sealed class ValueLesser : Value
	{
		private readonly string _compare;

		public ValueLesser(StudyFilterColumn column, string compare)
			: base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetValue(item).CompareTo(_compare) < 0;
		}

		public override string ToString()
		{
			return string.Format("{0} < {1}", Column.ToString(), _compare);
		}
	}

	public sealed class ValueGreaterOrEqual : Value
	{
		private readonly string _compare;

		public ValueGreaterOrEqual(StudyFilterColumn column, string compare)
			: base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetValue(item).CompareTo(_compare) >= 0;
		}

		public override string ToString()
		{
			return string.Format("{0} >= {1}", Column.ToString(), _compare);
		}
	}

	public sealed class ValueLesserOrEqual : Value
	{
		private readonly string _compare;

		public ValueLesserOrEqual(StudyFilterColumn column, string compare)
			: base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetValue(item).CompareTo(_compare) <= 0;
		}

		public override string ToString()
		{
			return string.Format("{0} <= {1}", Column.ToString(), _compare);
		}
	}

	public sealed class ValueStartsWith : Value
	{
		private readonly string _compare;

		public ValueStartsWith(StudyFilterColumn column, string compare)
			: base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetValue(item).StartsWith(_compare);
		}

		public override string ToString()
		{
			return string.Format("{0} STARTS WITH {1}", Column.ToString(), _compare);
		}
	}

	public sealed class ValueEndsWith : Value
	{
		private readonly string _compare;

		public ValueEndsWith(StudyFilterColumn column, string compare)
			: base(column)
		{
			_compare = compare;
		}

		public override bool Evaluate(StudyItem item)
		{
			return base.Column.GetValue(item).EndsWith(_compare);
		}

		public override string ToString()
		{
			return string.Format("{0} ENDS WITH {1}", Column.ToString(), _compare);
		}
	}
}