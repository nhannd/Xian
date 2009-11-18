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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities
{
	public abstract class FilterPredicate
	{
		private event EventHandler _changed;

		protected FilterPredicate() {}

		public event EventHandler Changed
		{
			add { _changed += value; }
			remove { _changed -= value; }
		}

		public abstract bool Evaluate(StudyItem item);

		protected virtual void OnChanged()
		{
			EventsHelper.Fire(_changed, this, EventArgs.Empty);
		}
	}

	public sealed class NotFilterPredicate : FilterPredicate
	{
		private FilterPredicate _predicate;

		public NotFilterPredicate() : base() {}

		public NotFilterPredicate(FilterPredicate predicate)
			: base()
		{
			_predicate = predicate;
		}

		public FilterPredicate Predicate
		{
			get { return _predicate; }
			set { _predicate = value; }
		}

		public override bool Evaluate(StudyItem item)
		{
			if (_predicate == null)
				return true;
			return !_predicate.Evaluate(item);
		}
	}

	public abstract class CompositeFilterPredicate : FilterPredicate
	{
		private readonly ObservableList<FilterPredicate> _predicates;

		protected CompositeFilterPredicate()
			: base()
		{
			_predicates = new ObservableList<FilterPredicate>();
			_predicates.ItemAdded += Predicates_Added;
			_predicates.ItemChanged += Predicates_Changed;
			_predicates.ItemChanging += Predicates_Changing;
			_predicates.ItemRemoved += Predicates_Removed;
			_predicates.EnableEvents = true;
		}

		protected CompositeFilterPredicate(IEnumerable<FilterPredicate> predicates)
			: base()
		{
			_predicates = new ObservableList<FilterPredicate>(predicates);
		}

		public IList<FilterPredicate> Predicates
		{
			get { return _predicates; }
		}

		public virtual bool IsActive
		{
			get
			{
				foreach (FilterPredicate predicate in _predicates)
				{
					if (predicate is CompositeFilterPredicate && !((CompositeFilterPredicate)predicate).IsActive)
						continue;
					return true;
				}
				return false;
			}
		}

		private void Predicates_Added(object sender, ListEventArgs<FilterPredicate> e)
		{
			e.Item.Changed += FilterPredicate_Changed;
			base.OnChanged();
		}

		private void Predicates_Changing(object sender, ListEventArgs<FilterPredicate> e)
		{
			e.Item.Changed -= FilterPredicate_Changed;
		}

		private void Predicates_Changed(object sender, ListEventArgs<FilterPredicate> e)
		{
			e.Item.Changed += FilterPredicate_Changed;
			base.OnChanged();
		}

		private void Predicates_Removed(object sender, ListEventArgs<FilterPredicate> e)
		{
			e.Item.Changed -= FilterPredicate_Changed;
			base.OnChanged();
		}

		private void FilterPredicate_Changed(object sender, EventArgs e)
		{
			base.OnChanged();
		}
	}

	public sealed class AndFilterPredicate : CompositeFilterPredicate
	{
		public AndFilterPredicate() : base() {}

		public AndFilterPredicate(IEnumerable<FilterPredicate> predicates) : base(predicates) {}

		public override bool Evaluate(StudyItem item)
		{
			foreach (FilterPredicate predicate in this.Predicates)
			{
				if (!predicate.Evaluate(item))
					return false;
			}
			return true;
		}
	}

	public sealed class OrFilterPredicate : CompositeFilterPredicate
	{
		public OrFilterPredicate() : base() {}

		public OrFilterPredicate(IEnumerable<FilterPredicate> predicates) : base(predicates) {}

		public override bool Evaluate(StudyItem item)
		{
			foreach (FilterPredicate predicate in this.Predicates)
			{
				if (predicate.Evaluate(item))
					return true;
			}
			return false;
		}
	}
}