using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters
{
	[ExtensionOf(typeof (AutoFilterToolExtensionPoint))]
	public class CompareValueColumnFilterTool : AutoFilterTool, IFilterMenuActionOwner
	{
		private IActionSet _actions;

		protected override bool IsColumnSupported()
		{
			return typeof (IComparable).IsAssignableFrom(base.Column.GetValueType());
		}

		public CompositeFilterPredicate ParentFilterPredicate
		{
			get
			{
				CompareValueCompositePredicateFilter filter = CompareValueCompositePredicateFilter.Find(base.AutoFilterRoot.Predicates, base.Column);
				if (filter == null)
					base.AutoFilterRoot.Predicates.Add(filter = new CompareValueCompositePredicateFilter(base.Column));
				return filter;
			}
		}

		protected override void Dispose(bool disposing)
		{
			CompareValueCompositePredicateFilter filter = CompareValueCompositePredicateFilter.Find(base.AutoFilterRoot.Predicates, base.Column);
			if (filter != null)
				base.AutoFilterRoot.Predicates.Remove(filter);

			base.Dispose(disposing);
		}

		public override IActionSet Actions
		{
			get
			{
				if (_actions == null)
				{
					List<IAction> list = new List<IAction>();

					if (this.IsColumnSupported())
					{
						ResourceResolver resourceResolver = new ResourceResolver(this.GetType(), false);

						CompareFilterMenuAction equalsAction = CompareFilterMenuAction.CreateAction(
							this.GetType(), "equals",
							"studyfilters-columnfilters/MenuValueFilters/MenuEquals",
							this, new CompareFilterMode[] {CompareFilterMode.Equals, CompareFilterMode.NotEquals},
							resourceResolver);
						list.Add(equalsAction);

						CompareFilterMenuAction upperBoundAction = CompareFilterMenuAction.CreateAction(
							this.GetType(), "upperBound",
							"studyfilters-columnfilters/MenuValueFilters/MenuUpperBound",
							this, new CompareFilterMode[] {CompareFilterMode.LessThan, CompareFilterMode.LessThenOrEquals},
							resourceResolver);
						list.Add(upperBoundAction);

						CompareFilterMenuAction lowerBoundAction = CompareFilterMenuAction.CreateAction(
							this.GetType(), "lowerBound",
							"studyfilters-columnfilters/MenuValueFilters/MenuLowerBound",
							this, new CompareFilterMode[] {CompareFilterMode.GreaterThan, CompareFilterMode.GreaterThanOrEquals},
							resourceResolver);
						list.Add(lowerBoundAction);
					}

					_actions = base.Actions.Union(new ActionSet(list));
				}
				return _actions;
			}
		}

		private class CompareValueCompositePredicateFilter : CompositeFilterPredicate
		{
			private readonly StudyFilterColumn _column;

			public CompareValueCompositePredicateFilter(StudyFilterColumn column)
				: base()
			{
				_column = column;
			}

			public override bool Evaluate(StudyItem item)
			{
				foreach (FilterPredicate predicate in base.Predicates)
				{
					if (!predicate.Evaluate(item))
						return false;
				}
				return true;
			}

			public override bool IsActive
			{
				get
				{
					foreach (CompareFilterMenuAction.Predicate predicate in base.Predicates)
						if (predicate.IsActive)
							return true;

					return false;
				}
			}

			public static CompareValueCompositePredicateFilter Find(IEnumerable<FilterPredicate> collection, StudyFilterColumn column)
			{
				return (CompareValueCompositePredicateFilter)
				       CollectionUtils.SelectFirst(collection,
				                                   delegate(FilterPredicate x) { return x is CompareValueCompositePredicateFilter && ((CompareValueCompositePredicateFilter) x)._column == column; });
			}
		}
	}
}