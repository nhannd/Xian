using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public interface IWorklistItemSearchContext<TItem>
        where TItem : WorklistItemBase
    {
        bool IncludeDegenerateProcedureItems { get; }
		bool IncludeDegeneratePatientItems { get; }
		WorklistItemSearchCriteria[] SearchCriteria { get; }
        int Threshold { get; }

        HqlProjectionQuery BuildWorklistItemSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery);
        HqlProjectionQuery BuildProcedureSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery);
        HqlProjectionQuery BuildPatientSearchQuery(WorklistItemSearchCriteria[] where, bool countQuery);
        List<TItem> DoQuery(HqlQuery query);
        int DoQueryCount(HqlQuery query);
    }


    public interface IWorklistItemSearchExecutionStrategy<TItem>
        where TItem : WorklistItemBase
    {
        IList<TItem> GetSearchResults(IWorklistItemSearchContext<TItem> wisc);
        bool EstimateSearchResultsCount(IWorklistItemSearchContext<TItem> wisc, out int count);
    }

    public abstract class WorklistItemSearchExecutionStrategy<TItem> : IWorklistItemSearchExecutionStrategy<TItem>
        where TItem : WorklistItemBase
    {
        #region IWorklistItemSearchExecutionStrategy<TItem> Members

        public abstract IList<TItem> GetSearchResults(IWorklistItemSearchContext<TItem> wisc);

        public abstract bool EstimateSearchResultsCount(IWorklistItemSearchContext<TItem> wisc, out int count);

        #endregion

        /// <summary>
        /// Returns a new list containing all items in primary, plus any items in secondary that were not
        /// already in primary according to the specified identity provider.  The arguments are not modified.
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <param name="identityProvider"></param>
        /// <returns></returns>
        protected static List<TItem> MergeResults(List<TItem> primary, List<TItem> secondary,
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
