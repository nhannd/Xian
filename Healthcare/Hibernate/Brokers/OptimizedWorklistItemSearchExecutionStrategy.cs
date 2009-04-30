using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;

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

            if (wisc.IncludeDegenerate)
            {
                // search for procedures
                List<TItem> procedures = new List<TItem>();
                if (whereSelectPatient.Length > 0)
                {
                    HqlProjectionQuery patientBasedProcedureQuery = wisc.BuildProcedureSearchQuery(whereSelectPatient, false);
                    results = MergeResults(results, wisc.DoQuery(patientBasedProcedureQuery),
                        delegate(TItem item) { return item.ProcedureRef; });
                }

                if (whereExcludePatient.Length > 0)
                {
                    HqlProjectionQuery patientExcludedProcedureQuery = wisc.BuildProcedureSearchQuery(whereExcludePatient, false);
                    results = MergeResults(results, wisc.DoQuery(patientExcludedProcedureQuery),
                        delegate(TItem item) { return item.ProcedureRef; });
                }

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

            // if includeDegenerate == true, we don't actually need to do a query for "active worklist items", e.g. ProcedureSteps,
            // because the degenerate set is by definition a superset of the active items
            if (wisc.IncludeDegenerate)
            {
                // Strategy:
                // Assume that only 4 search fields are really valid: Patient name, MRN, Healthcard, and Accession #.
                // The approach taken here is to perform a patient count query and a procedure count query.
                // The patient query will count all potential patient matches based on Patient name, MRN and Healthcard.
                // The procedure count query will count all potential procedure matches based on Patient name, MRN and Healthcard, and Accession #.
                // If either count exceeds the threshold, we can bail immediately.
                // Otherwise, the counts must be combined.  Note that each count represents a potentially overlapping
                // set of items, so there is no possible way to determine an 'exact' count (hence the word Estimate).
                // However, we know that the true count is a) greater than or equal to the maximum of either independent count, and
                // b) less than or equal to the sum of both counts.  Therefore, choose the midpoint of this number as a
                // 'good enough' estimate.

                // count number of patient matches
                HqlProjectionQuery patientCountQuery = wisc.BuildPatientSearchQuery(wisc.SearchCriteria, true);
                int numPatients = wisc.DoQueryCount(patientCountQuery);

                // if this number exceeds threshold, bail
                if (numPatients > wisc.Threshold)
                    return false;

                // count number of procedure matches with patient criteria only
                int numProcedures = 0;
                WorklistItemSearchCriteria[] whereSelectPatient = Select(wisc.SearchCriteria, "PatientProfile");
                if (whereSelectPatient.Length > 0)
                {
                    HqlProjectionQuery procedureCountQuery = wisc.BuildProcedureSearchQuery(whereSelectPatient, true);
                    numProcedures += wisc.DoQueryCount(procedureCountQuery);

                    // if this number exceeds threshold, bail
                    if (numProcedures > wisc.Threshold)
                        return false;
                }

                WorklistItemSearchCriteria[] whereExcludePatient = Exclude(wisc.SearchCriteria, "PatientProfile");
                if (whereExcludePatient.Length > 0)
                {
                    HqlProjectionQuery procedureCountQuery = wisc.BuildProcedureSearchQuery(whereExcludePatient, true);
                    numProcedures += wisc.DoQueryCount(procedureCountQuery);

                    // if this number exceeds threshold, bail
                    if (numProcedures > wisc.Threshold)
                        return false;
                }

                // combine the two numbers to produce a guess at the actual number of results
                count = (Math.Max(numPatients, numProcedures) + numPatients + numProcedures) / 2;
            }
            else
            {
                // search for worklist items, delegating the task of designing the query to the subclass
                HqlProjectionQuery worklistItemCountQuery = wisc.BuildWorklistItemSearchQuery(wisc.SearchCriteria, true);
                count = wisc.DoQueryCount(worklistItemCountQuery);
            }

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
            List<WorklistItemSearchCriteria> filteredCriteria = CollectionUtils.Map<WorklistItemSearchCriteria, WorklistItemSearchCriteria>(where,
                delegate(WorklistItemSearchCriteria criteria)
                {
                    WorklistItemSearchCriteria copy = new WorklistItemSearchCriteria();
                    // copy only the filter key
                    if (criteria.SubCriteria.ContainsKey(filterKey))
                        copy.SubCriteria[filterKey] = criteria.SubCriteria[filterKey];

                    return copy;
                });

            return filteredCriteria.ToArray();
        }

        /// <summary>
        /// Returns all criteria without the filter key
        /// </summary>
        /// <param name="where"></param>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public static WorklistItemSearchCriteria[] FilterOut(IEnumerable<WorklistItemSearchCriteria> where, string filterKey)
        {
            List<WorklistItemSearchCriteria> filteredCriteria = CollectionUtils.Map<WorklistItemSearchCriteria, WorklistItemSearchCriteria>(where,
                delegate(WorklistItemSearchCriteria criteria)
                {
                    WorklistItemSearchCriteria copy = new WorklistItemSearchCriteria();
                    // Copy every key except for the filter key
                    CollectionUtils.ForEach(criteria.SubCriteria.Keys,
                        delegate(string key)
                        {
                            if (!Equals(key, filterKey))
                                copy.SubCriteria[key] = criteria.SubCriteria[key];
                        });

                    return copy;
                });

            return filteredCriteria.ToArray();
        }

        public static WorklistItemSearchCriteria[] Select(IEnumerable<WorklistItemSearchCriteria> where, string key)
        {
            // create a copy of the criteria that exclude the filter criteria
            return CollectionUtils.Select(where,
                delegate(WorklistItemSearchCriteria criteria)
                {
                    return criteria.SubCriteria.ContainsKey(key);
                }).ToArray();
        }

        public static WorklistItemSearchCriteria[] Exclude(IEnumerable<WorklistItemSearchCriteria> where, string key)
        {
            // create a copy of the criteria that exclude the filter criteria
            return CollectionUtils.Select(where,
                delegate(WorklistItemSearchCriteria criteria)
                {
                    return !criteria.SubCriteria.ContainsKey(key);
                }).ToArray();
        }
    }
}
