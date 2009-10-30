using System;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public interface IStudyFilter
	{
		IList<StudyItem> Items { get; }
		StudyItemSelection Selection { get; }

		event EventHandler ItemAdded;
		event EventHandler ItemRemoved;

		bool IsStale { get; }
		event EventHandler IsStaleChanged;

		bool FilterPredicatesEnabled { get; set; }
		event EventHandler FilterPredicatesEnabledChanged;

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
		/// If the displayed data is stale, reapplies the predicates to the dataset and updates the display.
		/// </summary>
		void Refresh();

		/// <summary>
		/// Reapplies the predicates to the dataset and updates the display.
		/// </summary>
		/// <param name="force">A value indicating whether or not to perform the refresh even if the data is not stale.</param>
		void Refresh(bool force);
		event EventHandler FilterPredicatesChanged;
		event EventHandler SortPredicatesChanged;
		
	}
}