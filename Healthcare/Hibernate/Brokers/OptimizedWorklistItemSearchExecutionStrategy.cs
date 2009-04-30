using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    class OptimizedWorklistItemSearchExecutionStrategy<TItem> : WorklistItemSearchExecutionStrategy<TItem>
        where TItem : WorklistItemBase
    {
        public override IList<TItem> GetSearchResults(IWorklistItemSearchContext<TItem> wisc)
        {
            List<TItem> results = new List<TItem>();

            // the search criteria is broken up to gain performance. See #2416.
            WorklistItemSearchCriteria[] whereSelectPatient = Select(wisc.SearchCriteria, "PatientProfile");
            WorklistItemSearchCriteria[] whereExcludePatient = Exclude(wisc.SearchCriteria, "PatientProfile");

            // search for worklist items, delegating the task of designing the query to the subclass
            if (whereSelectPatient.Length > 0)
            {
                HqlProjectionQuery patientOnlyWorklistItemQuery = wisc.BuildWorklistItemSearchQuery(whereSelectPatient, false);
                if (patientOnlyWorklistItemQuery != null)
                {
                    results = MergeResults(results, wisc.DoQuery(patientOnlyWorklistItemQuery),
                        delegate(TItem item) { return item.ProcedureRef; });
                }
            }

            if (whereExcludePatient.Length > 0)
            {
                HqlProjectionQuery patientExcludedWorklistItemQuery = wisc.BuildWorklistItemSearchQuery(whereExcludePatient, false);
                if (patientExcludedWorklistItemQuery != null)
                {
                    results = MergeResults(results, wisc.DoQuery(patientExcludedWorklistItemQuery),
                        delegate(TItem item) { return item.ProcedureRef; });
                }
            }

			// include procedure degenerate items if requested
            if (wisc.IncludeDegenerateProcedureItems)
            {
            	// search for procedures
            	if (whereSelectPatient.Length > 0)
            	{
            		HqlProjectionQuery patientBasedProcedureQuery = wisc.BuildProcedureSearchQuery(whereSelectPatient, false);
            		results = MergeResults(results, wisc.DoQuery(patientBasedProcedureQuery),
            		                       delegate(TItem item) { return item.ProcedureRef; });
            	}

            	if (whereExcludePatient.Length > 0)
            	{
            		HqlProjectionQuery patientExcludedProcedureQuery =
            			wisc.BuildProcedureSearchQuery(whereExcludePatient, false);
            		results = MergeResults(results, wisc.DoQuery(patientExcludedProcedureQuery),
            		                       delegate(TItem item) { return item.ProcedureRef; });
            	}
            }

			// include patient degenerate items if requested
			if (wisc.IncludeDegeneratePatientItems)
			{
        		// search for patients
                if (whereSelectPatient.Length > 0)
                {
                    HqlProjectionQuery patientQuery = wisc.BuildPatientSearchQuery(whereSelectPatient, false);
                    List<TItem> patients = wisc.DoQuery(patientQuery);

                    // add any patients for which there is no result
                    results = MergeResults(results, patients, delegate(TItem item) { return item.PatientRef; });
                }
            }

            return results;
        }

		public override bool EstimateSearchResultsCount(IWorklistItemSearchContext<TItem> wisc, out int count)
		{
			count = 0;

			// if no degenerate items are included, we need to do exactly one query for worklist items,
			// no estimation is possible
			if (!wisc.IncludeDegeneratePatientItems && !wisc.IncludeDegenerateProcedureItems)
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
			// - Order/Procedure: Accession
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
			if (wisc.IncludeDegeneratePatientItems)
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
				// find procedures based on the patient search fields
				WorklistItemSearchCriteria[] whereSelectPatient = Select(wisc.SearchCriteria, "PatientProfile");
				if (whereSelectPatient.Length > 0)
				{
					HqlProjectionQuery procedureCountQuery = wisc.BuildProcedureSearchQuery(whereSelectPatient, true);
					numProcedures += wisc.DoQueryCount(procedureCountQuery);

					// if this number exceeds threshold, bail
					if (numProcedures > wisc.Threshold)
						return false;
				}

				// find procedures based on the order search fields
				WorklistItemSearchCriteria[] whereExcludePatient = Exclude(wisc.SearchCriteria, "PatientProfile");
				if (whereExcludePatient.Length > 0)
				{
					HqlProjectionQuery procedureCountQuery = wisc.BuildProcedureSearchQuery(whereExcludePatient, true);
					numProcedures += wisc.DoQueryCount(procedureCountQuery);

					// if this number exceeds threshold, bail
					if (numProcedures > wisc.Threshold)
						return false;
				}
			}

			// combine the two numbers to produce a guess at the actual number of results
			count = (Math.Max(numPatients, numProcedures) + numPatients + numProcedures) / 2;

			// return whether the count exceeded the threshold
			return count <= wisc.Threshold;
		}

        /// <summary>
        /// Returns all criteria with only the filter key, as none of the others are relevant
        /// </summary>
        /// <param name="where"></param>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public static WorklistItemSearchCriteria[] Filter(IEnumerable<WorklistItemSearchCriteria> where, string filterKey)
        {
            return CollectionUtils.Map<WorklistItemSearchCriteria, WorklistItemSearchCriteria>(
				where,
                delegate(WorklistItemSearchCriteria criteria)
                {
                	return (WorklistItemSearchCriteria) criteria.Clone(
                	                                    	delegate(SearchCriteria subCriteria)
                	                                    	{
                	                                    		return subCriteria.GetKey() == filterKey;
                	                                    	}, false);
                }).ToArray();
        }

        /// <summary>
        /// Returns all criteria without the filter key
        /// </summary>
        /// <param name="where"></param>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public static WorklistItemSearchCriteria[] FilterOut(IEnumerable<WorklistItemSearchCriteria> where, string filterKey)
        {
            return CollectionUtils.Map<WorklistItemSearchCriteria, WorklistItemSearchCriteria>(where,
                delegate(WorklistItemSearchCriteria criteria)
                {
					return (WorklistItemSearchCriteria)criteria.Clone(
															delegate(SearchCriteria subCriteria)
															{
																return subCriteria.GetKey() != filterKey;
															}, false);
				}).ToArray();
        }

        public static WorklistItemSearchCriteria[] Select(IEnumerable<WorklistItemSearchCriteria> where, string key)
        {
            return CollectionUtils.Select(where,
                delegate(WorklistItemSearchCriteria criteria)
                {
                    return criteria.SubCriteria.ContainsKey(key);
                }).ToArray();
        }

        public static WorklistItemSearchCriteria[] Exclude(IEnumerable<WorklistItemSearchCriteria> where, string key)
        {
            return CollectionUtils.Select(where,
                delegate(WorklistItemSearchCriteria criteria)
                {
                    return !criteria.SubCriteria.ContainsKey(key);
                }).ToArray();
        }
    }
}
