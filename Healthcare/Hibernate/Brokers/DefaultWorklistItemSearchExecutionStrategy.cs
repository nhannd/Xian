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

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	/// <summary>
	/// Encapsulates the default worklist search execution strategy.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The default strategy is the naive implementation.  It sends the minimum possible number of queries
	/// to the database to perform the search, which means it relies on the database entirely to optimize
	/// the execution of those queries.
	/// </para>
	/// <para>
	/// The concept of "degenerate" worklist items requires some explanation.  If a worklist item is thought of
	/// as a tuple containing Patient Profile, Order, Procedure, and Procedure Step information
	/// e.g.
	/// ( pp, o, rp, ps )
	/// the a "degenerate procedure" worklist item is an item of the form
	/// ( pp, o, rp, null )
	/// and a "degenerate patient" worklist item is an item of the form
	/// ( pp, null, null, null )
	/// 
	/// </para>
	/// </remarks>
	/// <typeparam name="TItem"></typeparam>
    class DefaultWorklistItemSearchExecutionStrategy<TItem> : WorklistItemSearchExecutionStrategy<TItem>
        where TItem : WorklistItemBase
    {
		/// <summary>
		/// Executes a search, returning a list of hits.
		/// </summary>
		/// <param name="wisc"></param>
		/// <returns></returns>
		public override IList<TItem> GetSearchResults(IWorklistItemSearchContext<TItem> wisc)
        {
            List<TItem> results = new List<TItem>();

            WorklistItemSearchCriteria[] where = wisc.SearchCriteria;

            // search for worklist items, delegating the task of designing the query to the subclass
            HqlProjectionQuery worklistItemQuery = wisc.BuildWorklistItemSearchQuery(where, false);
            if (worklistItemQuery != null)
            {
                results = UnionMerge(results, wisc.DoQuery(worklistItemQuery),
                    delegate(TItem item) { return item.ProcedureRef; });
            }

			// include degenerate procedure items if requested
            if (wisc.IncludeDegenerateProcedureItems)
            {
            	// search for procedures
            	HqlProjectionQuery procedureQuery = wisc.BuildProcedureSearchQuery(where, false);
            	results = UnionMerge(results, wisc.DoQuery(procedureQuery),
            	                       delegate(TItem item) { return item.ProcedureRef; });
            }

			// include degenerate patient items if requested
			if(wisc.IncludeDegeneratePatientItems)
			{
        		// search for patients
                HqlProjectionQuery patientQuery = wisc.BuildPatientSearchQuery(where, false);

                // add any patients for which there is no result
                results = UnionMerge(results, wisc.DoQuery(patientQuery),
					delegate(TItem item) { return item.PatientRef; });
            }

            return results;
        }

		/// <summary>
		/// Estimates the hit count for the specified search, unless the count exceeds a specified
		/// threshold, in which case the method returns false and no count is obtained.
		/// </summary>
		/// <param name="wisc"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override bool EstimateSearchResultsCount(IWorklistItemSearchContext<TItem> wisc, out int count)
        {
			count = 0;

			// if no degenerate items are included, we need to do exactly one query for worklist items,
			// no estimation is possible
			if(!wisc.IncludeDegeneratePatientItems && !wisc.IncludeDegenerateProcedureItems)
			{
				// search for worklist items, delegating the task of designing the query to the subclass
				HqlProjectionQuery worklistItemCountQuery = wisc.BuildWorklistItemSearchQuery(wisc.SearchCriteria, true);
				count = wisc.DoQueryCount(worklistItemCountQuery);

				// return whether the count exceeded the threshold
				return count <= wisc.Threshold;
			}

			// if some degenerate items are to be included, then we can omit querying for "active worklist items", e.g. ProcedureSteps,
			// because the degenerate set is by definition a superset of the active items
			// Strategy:
			// Valid search fields are:
			// - Patient: Name, MRN, Healthcard
			// - Order/Procedure: Accession, Ordering Prac, Procedure Type, Date Range
			// The approach taken here is to perform a patient count query and a procedure count query.
			// The patient query will count all potential patient matches based on Patient-applicable search fields.
			// The procedure count query will count all potential procedure matches based on both Patient and Order/Procedure search fields.
			// If either count exceeds the threshold, we can bail immediately.
			// Otherwise, the counts must be combined.  Note that each count represents a potentially overlapping
			// set of items, so there is no possible way to determine an 'exact' count (hence the word Estimate).
			// However, we know that the true count is a) greater than or equal to the maximum of either independent count, and
			// b) less than or equal to the sum of both counts.  Therefore, choose the midpoint of this number as a
			// 'good enough' estimate.

        	int numPatients = 0;
			if(wisc.IncludeDegeneratePatientItems)
			{
				// count number of patient matches
				HqlProjectionQuery patientCountQuery = wisc.BuildPatientSearchQuery(wisc.SearchCriteria, true);
				numPatients = wisc.DoQueryCount(patientCountQuery);

				// if this number exceeds threshold, bail
				if (numPatients > wisc.Threshold)
					return false;
			}

			int numProcedures = 0;
			if (wisc.IncludeDegenerateProcedureItems)
			{
				// count number of procedure matches
				HqlProjectionQuery procedureCountQuery = wisc.BuildProcedureSearchQuery(wisc.SearchCriteria, true);
				numProcedures = wisc.DoQueryCount(procedureCountQuery);

				// if this number exceeds threshold, bail
				if (numProcedures > wisc.Threshold)
					return false;
			}

			// combine the two numbers to produce a guess at the actual number of results
			count = (Math.Max(numPatients, numProcedures) + numPatients + numProcedures) / 2;

			// return whether the count exceeded the threshold
			return count <= wisc.Threshold;
		}
    }
}
