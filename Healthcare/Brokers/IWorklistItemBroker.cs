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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Brokers
{
	/// <summary>
	/// Defines a common base interface for worklist item brokers.
	/// </summary>
	public interface IWorklistItemBroker : IPersistenceBroker
	{
		/// <summary>
		/// Gets the set of items in the worklist.
		/// </summary>
		IList<TItem> GetWorklistItems<TItem>(Worklist worklist, IWorklistQueryContext wqc)
			where TItem : WorklistItem;

		string GetWorklistItemsHql(Worklist worklist, IWorklistQueryContext wqc);

		/// <summary>
		/// Gets the set of items matching the specified criteria, returned as tuples shaped by the specified projection.
		/// </summary>
		IList<object[]> GetWorklistItems(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection, SearchResultPage page);

		/// <summary>
		/// Allow access to the HQL for debugging purposes only.  Obviously it does not make sense to pass HQL through the abstraction layer!
		/// </summary>
		/// <param name="procedureStepClasses"></param>
		/// <param name="criteria"></param>
		/// <param name="projection"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		string GetWorklistItemsHql(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection, SearchResultPage page);

		/// <summary>
		/// Gets a count of the items in the worklist.
		/// </summary>
		/// <param name="worklist"></param>
		/// <param name="wqc"></param>
		/// <returns></returns>
		int CountWorklistItems(Worklist worklist, IWorklistQueryContext wqc);

		/// <summary>
		/// Performs a search using the specified criteria.
		/// </summary>
		IList<TItem> GetSearchResults<TItem>(WorklistItemSearchArgs args)
			where TItem : WorklistItem;

		/// <summary>
		/// Gets an approximate count of the results that a search using the specified criteria would return.
		/// </summary>
		/// <remarks>
		/// This method will be invoked prior to calling <see cref="GetSearchResults{TItem}"/>, in order to determine
		/// if the search criteria are specific enough to yield an acceptably sized result set.  The method may
		/// be implemented such that it does not need to complete the computation, in the case where it determines
		/// that the result will exceed the specified threshold.  In this case, the method should simply return false,
		/// and the count parameter will be ignored. 
		/// </remarks>
		bool EstimateSearchResultsCount(WorklistItemSearchArgs args, out int count);
	}
}
