using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
    /// <summary>
    /// Defines a common base interface for worklist item brokers.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IWorklistItemBroker<TItem>: IPersistenceBroker
    {
        /// <summary>
        /// Gets the set of items in the worklist.
        /// </summary>
        /// <param name="worklist"></param>
        /// <param name="wqc"></param>
        /// <returns></returns>
        IList<TItem> GetWorklistItems(Worklist worklist, IWorklistQueryContext wqc);

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
		/// <remarks>
		/// The <paramref name="includeDegenerate"/> flag specifies whether to include results for patients
		/// and procedures that meet the criteria but do not have an active procedure step.
		/// </remarks>
		/// <param name="where"></param>
		/// <returns></returns>
		/// <param name="includeDegenerate"></param>
		IList<TItem> GetSearchResults(WorklistItemSearchCriteria[] where, bool includeDegenerate);

    	/// <summary>
    	/// Gets an approximate count of the results that a search using the specified criteria would return.
    	/// </summary>
    	/// <remarks>
    	/// <para>
		/// The <paramref name="includeDegenerate"/> flag specifies whether to include results for patients
		/// and procedures that meet the criteria but do not have an active procedure step.
		/// </para>
		/// <para>
		/// This method will be invoked prior to calling <see cref="GetSearchResults"/>, in order to determine
		/// if the search criteria are specific enough to yield an acceptably sized result set.  The method may
		/// be implemented such that it does not need to complete the computation, in the case where it determines
		/// that the result will exceed the specified threshold.  In this case, the method should simply return false,
		/// and the count parameter will be ignored. 
		/// </para>
    	/// </remarks>
    	/// <param name="where"></param>
    	/// <param name="threshold"></param>
    	/// <param name="includeDegenerate"></param>
    	/// <param name="count"></param>
		/// <returns></returns>
		bool EstimateSearchResultsCount(WorklistItemSearchCriteria[] where, int threshold, bool includeDegenerate, out int count);
	}
}
