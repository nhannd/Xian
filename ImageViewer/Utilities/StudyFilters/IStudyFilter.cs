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