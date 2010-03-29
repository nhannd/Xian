#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Defines an interface to an object that encapsulates information about a search to be executed.
	/// </summary>
	interface IWorklistItemSearchContext
	{
		/// <summary>
		/// Gets a value indicating whether degenerate procedure worklist items should be included
		/// in the search results.
		/// </summary>
		bool IncludeDegenerateProcedureItems { get; }

		/// <summary>
		/// Gets a value indicating whether degenerate patient worklist items should be included
		/// in the search results.
		/// </summary>
		bool IncludeDegeneratePatientItems { get; }

		/// <summary>
		/// Gets the search criteria.
		/// </summary>
		WorklistItemSearchCriteria[] SearchCriteria { get; }

		/// <summary>
		/// Gets the maximum number of allowed hits.  A search that would return more hits should not execute.
		/// </summary>
		int Threshold { get; }

		/// <summary>
		/// Find matching worklist items.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		IList<WorklistItem> FindWorklistItems(WorklistItemSearchCriteria[] where);

		/// <summary>
		/// Count matching worklist items.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		int CountWorklistItems(WorklistItemSearchCriteria[] where);

		/// <summary>
		/// Find matching procedures.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		IList<WorklistItem> FindProcedures(WorklistItemSearchCriteria[] where);

		/// <summary>
		/// Count matching procedures.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		int CountProcedures(WorklistItemSearchCriteria[] where);

		/// <summary>
		/// Find matching patients.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		IList<WorklistItem> FindPatients(WorklistItemSearchCriteria[] where);

		/// <summary>
		/// Count matching patients.
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		int CountPatients(WorklistItemSearchCriteria[] where);

	}

	/// <summary>
	/// Defines an interface to a class that encapsulates a strategy for executing a worklist search.
	/// </summary>
	interface IWorklistItemSearchExecutionStrategy
	{
		/// <summary>
		/// Executes a search, returning a list of hits.
		/// </summary>
		/// <param name="wisc"></param>
		/// <returns></returns>
		IList<WorklistItem> GetSearchResults(IWorklistItemSearchContext wisc);

		/// <summary>
		/// Estimates the hit count for the specified search, unless the count exceeds a specified
		/// threshold, in which case the method returns false and no count is obtained.
		/// </summary>
		/// <param name="wisc"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		bool EstimateSearchResultsCount(IWorklistItemSearchContext wisc, out int count);
	}

	/// <summary>
	/// Abstract base implementation of <see cref="IWorklistItemSearchExecutionStrategy"/>.
	/// </summary>
	abstract class WorklistItemSearchExecutionStrategy : IWorklistItemSearchExecutionStrategy
	{
		#region IWorklistItemSearchExecutionStrategy Members

		/// <summary>
		/// Executes a search, returning a list of hits.
		/// </summary>
		/// <param name="wisc"></param>
		/// <returns></returns>
		public abstract IList<WorklistItem> GetSearchResults(IWorklistItemSearchContext wisc);

		/// <summary>
		/// Estimates the hit count for the specified search, unless the count exceeds a specified
		/// threshold, in which case the method returns false and no count is obtained.
		/// </summary>
		/// <param name="wisc"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public abstract bool EstimateSearchResultsCount(IWorklistItemSearchContext wisc, out int count);

		#endregion

		/// <summary>
		/// Performs a union of the primary and secondary sets.
		/// </summary>
		/// <remarks>
		/// The specified identity-provider is used to determine identity of two items, hence whether the same logical
		/// item appears in both sets.  If the same logical item appears in both sets, the actual item from the primary set will
		/// always be chosen.  The union is returned as a new list (the arguments are not modified).
		/// </remarks>
		/// <param name="primary"></param>
		/// <param name="secondary"></param>
		/// <param name="identityProvider"></param>
		/// <returns></returns>
		protected static List<WorklistItem> UnionMerge(IList<WorklistItem> primary, IList<WorklistItem> secondary,
			Converter<WorklistItem, EntityRef> identityProvider)
		{
			// note that we do not modify the arguments
			var merged = new List<WorklistItem>(primary);
			foreach (var s in secondary)
			{
				if (!CollectionUtils.Contains(primary,
						p => identityProvider(s).Equals(identityProvider(p), true)))
				{
					merged.Add(s);
				}
			}
			return merged;
		}
	}
}
