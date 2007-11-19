using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow;

namespace ClearCanvas.Ris.Application.Services
{
    public class WorklistTextQueryHelper<TDomainItem, TSummary>
        : TextQueryHelper<TDomainItem, WorklistItemSearchCriteria, TSummary>
        where TDomainItem : WorklistItemBase
        where TSummary : DataContractBase
    {
        /// <summary>
        /// Public constructor allows direct use of this class without the need to create a subclass.
        /// </summary>
        /// <param name="summaryAssembler"></param>
        /// <param name="countCallback"></param>
        /// <param name="queryCallback"></param>
        public WorklistTextQueryHelper(
            AssembleSummaryDelegate summaryAssembler,
            DoCountDelegate countCallback, 
            DoQueryDelegate queryCallback)
            : base(null, summaryAssembler, countCallback, queryCallback)
        {
        }

        protected override WorklistItemSearchCriteria[] BuildCriteria(string query)
        {
            // this will hold all criteria
            List<WorklistItemSearchCriteria> criteria = new List<WorklistItemSearchCriteria>();

            // build criteria against names
            PersonName[] names = ParsePersonNames(query);
            criteria.AddRange(CollectionUtils.Map<PersonName, WorklistItemSearchCriteria>(names,
                delegate(PersonName n)
                {
                    WorklistItemSearchCriteria sc = new WorklistItemSearchCriteria();
                    sc.PatientProfile.Name.FamilyName.StartsWith(n.FamilyName);
                    if (n.GivenName != null)
                        sc.PatientProfile.Name.GivenName.StartsWith(n.GivenName);
                    return sc;
                }));

            // build criteria against Mrn identifiers
            string[] ids = ParseIdentifiers(query);
            criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
                delegate(string word)
                {
                    WorklistItemSearchCriteria c = new WorklistItemSearchCriteria();
                    c.PatientProfile.Mrn.Id.StartsWith(word);
                    return c;
                }));

            // build criteria against Healthcard identifiers
            criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
                delegate(string word)
                {
                    WorklistItemSearchCriteria c = new WorklistItemSearchCriteria();
                    c.PatientProfile.Healthcard.Id.StartsWith(word);
                    return c;
                }));

            // build criteria against Accession Number
            criteria.AddRange(CollectionUtils.Map<string, WorklistItemSearchCriteria>(ids,
                delegate(string word)
                {
                    WorklistItemSearchCriteria c = new WorklistItemSearchCriteria();
                    c.Order.AccessionNumber.StartsWith(word);
                    return c;
                }));

            return criteria.ToArray();
        }
    }
}