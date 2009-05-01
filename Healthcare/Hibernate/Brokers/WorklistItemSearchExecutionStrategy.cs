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
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Defines an interface to an object that encapsulates information about a search to be executed.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
    interface IWorklistItemSearchContext<TItem>
        where TItem : WorklistItemBase
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
		/// Obtains the HQL query to search for full (non-degenerate) worklist items.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="countQuery"></param>
		/// <returns></returns>
        HqlProjectionQuery BuildWorklistItemSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery);

		/// <summary>
		/// Obtains the HQL query to search for degenerate procedure worklist items.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="countQuery"></param>
		/// <returns></returns>
        HqlProjectionQuery BuildProcedureSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery);

		/// <summary>
		/// Obtains the HQL query to search for degenerate patient worklist items.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="countQuery"></param>
		/// <returns></returns>
        HqlProjectionQuery BuildPatientSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery);

		/// <summary>
		/// Executes the specified query.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
        List<TItem> DoQuery(HqlQuery query);

		/// <summary>
		/// Executes the specified count query.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
        int DoQueryCount(HqlQuery query);
    }

	/// <summary>
	/// Defines an interface to a class that encapsulates a strategy for executing a worklist search.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
    interface IWorklistItemSearchExecutionStrategy<TItem>
        where TItem : WorklistItemBase
    {
		/// <summary>
		/// Executes a search, returning a list of hits.
		/// </summary>
		/// <param name="wisc"></param>
		/// <returns></returns>
        IList<TItem> GetSearchResults(IWorklistItemSearchContext<TItem> wisc);

		/// <summary>
		/// Estimates the hit count for the specified search, unless the count exceeds a specified
		/// threshold, in which case the method returns false and no count is obtained.
		/// </summary>
		/// <param name="wisc"></param>
		/// <param name="count"></param>
		/// <returns></returns>
        bool EstimateSearchResultsCount(IWorklistItemSearchContext<TItem> wisc, out int count);
    }

	/// <summary>
	/// Abstract base implementation of <see cref="IWorklistItemSearchExecutionStrategy{TItem}"/>.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
    abstract class WorklistItemSearchExecutionStrategy<TItem> : IWorklistItemSearchExecutionStrategy<TItem>
        where TItem : WorklistItemBase
    {
        #region IWorklistItemSearchExecutionStrategy<TItem> Members

		/// <summary>
		/// Executes a search, returning a list of hits.
		/// </summary>
		/// <param name="wisc"></param>
		/// <returns></returns>
		public abstract IList<TItem> GetSearchResults(IWorklistItemSearchContext<TItem> wisc);

		/// <summary>
		/// Estimates the hit count for the specified search, unless the count exceeds a specified
		/// threshold, in which case the method returns false and no count is obtained.
		/// </summary>
		/// <param name="wisc"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public abstract bool EstimateSearchResultsCount(IWorklistItemSearchContext<TItem> wisc, out int count);

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
        protected static List<TItem> UnionMerge(List<TItem> primary, List<TItem> secondary,
            Converter<TItem, EntityRef> identityProvider)
        {
            // note that we do not modify the arguments
            List<TItem> merged = new List<TItem>(primary);
            foreach (TItem s in secondary)
            {
                if (!CollectionUtils.Contains(primary,
                     delegate(TItem p) { return identityProvider(s).Equals(identityProvider(p), true); }))
                {
                    merged.Add(s);
                }
            }
            return merged;
        }
    }
}
