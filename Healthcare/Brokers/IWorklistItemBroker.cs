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
		IList<TItem> GetSearchResults(WorklistItemSearchArgs args);

    	/// <summary>
    	/// Gets an approximate count of the results that a search using the specified criteria would return.
    	/// </summary>
    	/// <remarks>
		/// This method will be invoked prior to calling <see cref="GetSearchResults"/>, in order to determine
		/// if the search criteria are specific enough to yield an acceptably sized result set.  The method may
		/// be implemented such that it does not need to complete the computation, in the case where it determines
		/// that the result will exceed the specified threshold.  In this case, the method should simply return false,
		/// and the count parameter will be ignored. 
    	/// </remarks>
		bool EstimateSearchResultsCount(WorklistItemSearchArgs args, out int count);
	}
}
