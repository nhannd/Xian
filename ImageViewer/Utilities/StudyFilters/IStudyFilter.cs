using System;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public interface IStudyFilter
	{
		IList<StudyItem> Items { get; }

		event EventHandler ItemAdded;
		event EventHandler ItemRemoved;

		bool IsStale { get; }
		event EventHandler IsStaleChanged;

		/// <summary>
		/// Gets the collection of columns available on the Study Filter.
		/// </summary>
		IStudyFilterColumnCollection Columns { get; }

		/// <summary>
		/// Gets the current sort predicates (i.e. relational predicates) applied to the Study Filter.
		/// </summary>
		IList<SortPredicate> SortPredicates { get; }

		/// <summary>
		/// Gets the current filter predicates (i.e. functional predicates) applied to the Study Filter.
		/// </summary>
		IList<FilterPredicate> FilterPredicates { get; }

		/// <summary>
		/// Forces the data to be updated and any predicates to be reapplied.
		/// </summary>
		void Refresh();
		void Refresh(bool force);
		event EventHandler FilterPredicatesChanged;
		event EventHandler SortPredicatesChanged;
	}
}