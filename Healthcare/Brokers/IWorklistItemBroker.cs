#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

		/// <summary>
		/// Gets the set of items matching the specified criteria, returned as tuples shaped by the specified projection.
		/// </summary>
		IList<object[]> GetWorklistItems(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection, SearchResultPage page);

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

		/// <summary>
		/// Gets the HQL for debugging purposes only.
		/// </summary>
		/// <param name="procedureStepClasses"></param>
		/// <param name="criteria"></param>
		/// <param name="projection"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		string GetWorklistItemsHql(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection, SearchResultPage page);
		
		/// <summary>
		/// Gets the HQL for debugging purposes only.
		/// </summary>
		/// <param name="worklist"></param>
		/// <param name="wqc"></param>
		/// <returns></returns>
		string GetWorklistItemsHql(Worklist worklist, IWorklistQueryContext wqc);
	}
}
