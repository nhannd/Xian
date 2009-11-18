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