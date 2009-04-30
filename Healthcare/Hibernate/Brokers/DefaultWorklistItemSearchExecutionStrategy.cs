using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    class DefaultWorklistItemSearchExecutionStrategy<TItem> : WorklistItemSearchExecutionStrategy<TItem>
        where TItem : WorklistItemBase
    {
        public override IList<TItem> GetSearchResults(IWorklistItemSearchContext<TItem> wisc)
        {
            List<TItem> results = new List<TItem>();

            WorklistItemSearchCriteria[] where = wisc.SearchCriteria;

            // search for worklist items, delegating the task of designing the query to the subclass
            HqlProjectionQuery worklistItemQuery = wisc.BuildWorklistItemSearchQuery(where, false);
            if (worklistItemQuery != null)
            {
                results = MergeResults(results, wisc.DoQuery(worklistItemQuery),
                    delegate(TItem item) { return item.ProcedureRef; });
            }

			// include degenerate procedure items if requested
            if (wisc.IncludeDegenerateProcedureItems)
            {
            	// search for procedures
            	HqlProjectionQuery procedureQuery = wisc.BuildProcedureSearchQuery(where, false);
            	results = MergeResults(results, wisc.DoQuery(procedureQuery),
            	                       delegate(TItem item) { return item.ProcedureRef; });
            }

			// include degenerate patient items if requested
			if(wisc.IncludeDegeneratePatientItems)
			{
        		// search for patients
                HqlProjectionQuery patientQuery = wisc.BuildPatientSearchQuery(where, false);

                // add any patients for which there is no result
                results = MergeResults(results, wisc.DoQuery(patientQuery),
					delegate(TItem item) { return item.PatientRef; });
            }

            return results;
        }

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
