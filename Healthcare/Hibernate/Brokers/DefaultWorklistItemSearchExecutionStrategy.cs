using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Hql;

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

            if (wisc.IncludeDegenerate)
            {
                // search for procedures
                List<TItem> procedures = new List<TItem>();
                HqlProjectionQuery procedureQuery = wisc.BuildProcedureSearchQuery(where, false);
                results = MergeResults(results, wisc.DoQuery(procedureQuery),
                    delegate(TItem item) { return item.ProcedureRef; });

                // search for patients
                HqlProjectionQuery patientQuery = wisc.BuildPatientSearchQuery(where, false);
                List<TItem> patients = wisc.DoQuery(patientQuery);

                // add any patients for which there is no result
                results = MergeResults(results, patients, delegate(TItem item) { return item.PatientRef; });
            }

            return results;
        }

        public override bool EstimateSearchResultsCount(IWorklistItemSearchContext<TItem> wisc, out int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
