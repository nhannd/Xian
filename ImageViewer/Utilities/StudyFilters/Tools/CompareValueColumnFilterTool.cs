using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[ExtensionOf(typeof (StudyFilterColumnToolExtensionPoint))]
	public class CompareValueColumnFilterTool : StudyFilterColumnTool
	{
		protected override bool IsColumnSupported()
		{
			return typeof (IComparable).IsAssignableFrom(base.Column.GetValueType());
		}

		private ValueRangePredicateFilter ValueRangeFilter
		{
			get
			{
				ValueRangePredicateFilter filter = ValueRangePredicateFilter.Find(base.ColumnFilterRoot.Predicates, base.Column);
				if (filter == null)
					base.ColumnFilterRoot.Predicates.Add(filter = new ValueRangePredicateFilter(base.Column));
				return filter;
			}
		}

		private void ClearValueRangeFilter()
		{
			ValueRangePredicateFilter filter = ValueRangePredicateFilter.Find(base.ColumnFilterRoot.Predicates, base.Column);
			if (filter != null)
				base.ColumnFilterRoot.Predicates.Remove(filter);
		}

		public override IActionSet Actions
		{
			get
			{
				List<IAction> list = new List<IAction>();

				if (this.IsColumnSupported())
				{
					ResourceResolver resourceResolver = new ResourceResolver(this.GetType(), false);

					CompareFilterMenuAction equalsAction = CompareFilterMenuAction.CreateAction(
						this.GetType(), "equals",
						"studyfilters-columnfilters/MenuValueFilters/MenuEquals",
						CompareFilterMode.Equals | CompareFilterMode.NotEquals, resourceResolver);
					equalsAction.RefreshRequested += RefreshRequested;
					equalsAction.CurrentModeChanged += ResyncEqualsFilter;
					equalsAction.ValueChanged += ResyncEqualsFilter;
					list.Add(equalsAction);

					CompareFilterMenuAction upperBoundAction = CompareFilterMenuAction.CreateAction(
						this.GetType(), "upperBound",
						"studyfilters-columnfilters/MenuValueFilters/MenuUpperBound",
						CompareFilterMode.LessThan | CompareFilterMode.LessThenOrEquals, resourceResolver);
					upperBoundAction.RefreshRequested += RefreshRequested;
					upperBoundAction.CurrentModeChanged += ResyncUpperBoundFilter;
					upperBoundAction.ValueChanged += ResyncUpperBoundFilter;
					list.Add(upperBoundAction);

					CompareFilterMenuAction lowerBoundAction = CompareFilterMenuAction.CreateAction(
						this.GetType(), "lowerBound",
						"studyfilters-columnfilters/MenuValueFilters/MenuLowerBound",
						CompareFilterMode.GreaterThan | CompareFilterMode.GreaterThanOrEquals, resourceResolver);
					lowerBoundAction.RefreshRequested += RefreshRequested;
					lowerBoundAction.CurrentModeChanged += ResyncLowerBoundFilter;
					lowerBoundAction.ValueChanged += ResyncLowerBoundFilter;
					list.Add(lowerBoundAction);
				}

				return base.Actions.Union(new ActionSet(list));
			}
		}

		protected override void Dispose(bool disposing)
		{
			ClearValueRangeFilter();

			base.Dispose(disposing);
		}

		private void RefreshRequested(object sender, EventArgs e)
		{
			if (!base.Column.IsColumnFiltered || ValueRangePredicateFilter.Find(base.ColumnFilterRoot.Predicates, base.Column) == null)
			{
				CompareFilterMenuAction action = (CompareFilterMenuAction) sender;
				action.Value = string.Empty;
			}
		}

		private void ResyncEqualsFilter(object sender, EventArgs e)
		{
			CompareFilterMenuAction action = (CompareFilterMenuAction) sender;

			object value;
			if (!string.IsNullOrEmpty(action.Value) && base.Column.Parse(action.Value, out value))
				this.ValueRangeFilter.InstallEquals(value, action.CurrentMode == CompareFilterMode.Equals);
			else
				this.ValueRangeFilter.UninstallEquals();
		}

		private void ResyncUpperBoundFilter(object sender, EventArgs e)
		{
			CompareFilterMenuAction action = (CompareFilterMenuAction) sender;

			object value;
			if (!string.IsNullOrEmpty(action.Value) && base.Column.Parse(action.Value, out value))
				this.ValueRangeFilter.InstallUpperBound(value, action.CurrentMode == CompareFilterMode.GreaterThanOrEquals || action.CurrentMode == CompareFilterMode.LessThenOrEquals);
			else
				this.ValueRangeFilter.UninstallUpperBound();
		}

		private void ResyncLowerBoundFilter(object sender, EventArgs e)
		{
			CompareFilterMenuAction action = (CompareFilterMenuAction) sender;

			object value;
			if (!string.IsNullOrEmpty(action.Value) && base.Column.Parse(action.Value, out value))
				this.ValueRangeFilter.InstallLowerBound(value, action.CurrentMode == CompareFilterMode.GreaterThanOrEquals || action.CurrentMode == CompareFilterMode.LessThenOrEquals);
			else
				this.ValueRangeFilter.UninstallLowerBound();
		}

		private class ValueRangePredicateFilter : FilterPredicate
		{
			private readonly StudyFilterColumn _column;
			private bool _hasEquals = false;
			private bool _equalsResult = false;
			private object _equalsValue = null;
			private bool _hasLowerBound = false;
			private bool _lowerBoundInclusive = false;
			private object _lowerBoundValue = null;
			private bool _hasUpperBound = false;
			private bool _upperBoundInclusive = false;
			private object _upperBoundValue = null;

			public ValueRangePredicateFilter(StudyFilterColumn column)
				: base()
			{
				_column = column;
			}

			public void InstallEquals(object value, bool result)
			{
				_hasEquals = true;
				_equalsValue = value;
				_equalsResult = result;
				base.OnChanged();
			}

			public void UninstallEquals()
			{
				_hasEquals = false;
				_equalsValue = null;

				if (_hasEquals || _hasLowerBound || _hasUpperBound)
					base.OnChanged();
				else
					_column.ColumnFilterRoot.Predicates.Remove(this);
			}

			public void InstallUpperBound(object value, bool inclusive)
			{
				_hasUpperBound = true;
				_upperBoundValue = value;
				_upperBoundInclusive = inclusive;
				base.OnChanged();
			}

			public void UninstallUpperBound()
			{
				_hasUpperBound = false;
				_upperBoundValue = null;

				if (_hasEquals || _hasLowerBound || _hasUpperBound)
					base.OnChanged();
				else
					_column.ColumnFilterRoot.Predicates.Remove(this);
			}

			public void InstallLowerBound(object value, bool inclusive)
			{
				_hasLowerBound = true;
				_lowerBoundValue = value;
				_lowerBoundInclusive = inclusive;
				base.OnChanged();
			}

			public void UninstallLowerBound()
			{
				_hasLowerBound = false;
				_lowerBoundValue = null;

				if (_hasEquals || _hasLowerBound || _hasUpperBound)
					base.OnChanged();
				else
					_column.ColumnFilterRoot.Predicates.Remove(this);
			}

			public override bool Evaluate(StudyItem item)
			{
				bool result = true;

				if (_hasEquals)
				{
					result = ((Compare(_equalsValue, _column.GetValue(item)) == 0) == _equalsResult);
				}

				if (_hasUpperBound & result)
				{
					if (_upperBoundInclusive)
						result = (Compare(_upperBoundValue, _column.GetValue(item)) >= 0);
					else
						result = (Compare(_upperBoundValue, _column.GetValue(item)) > 0);
				}

				if (_hasLowerBound & result)
				{
					if (_lowerBoundInclusive)
						result = (Compare(_lowerBoundValue, _column.GetValue(item)) <= 0);
					else
						result = (Compare(_lowerBoundValue, _column.GetValue(item)) < 0);
				}

				return result;
			}

			public int Compare(object x, object y)
			{
				if (x is IComparable)
					return ((IComparable) x).CompareTo(y);
				else if (y is IComparable)
					return -((IComparable) y).CompareTo(x);
				return 0;
			}

			public static ValueRangePredicateFilter Find(IEnumerable<FilterPredicate> collection, StudyFilterColumn column)
			{
				return (ValueRangePredicateFilter) CollectionUtils.SelectFirst(collection,
				                                   	delegate(FilterPredicate x) { return x is ValueRangePredicateFilter && ((ValueRangePredicateFilter) x)._column == column; });
			}
		}
	}
}