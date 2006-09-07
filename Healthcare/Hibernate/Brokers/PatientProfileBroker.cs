using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public partial class PatientProfileBroker : EntityBroker<PatientProfile, PatientProfileSearchCriteria>, IPatientProfileBroker
    {
        /// <summary>
        /// Overridden to search patient identifiers as well
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public override IList<PatientProfile> Find(PatientProfileSearchCriteria criteria, SearchResultPage page)
        {
            // note use of "join fetch" - this is important to ensure that the patient identifiers are fetched
            // along with the patient in a single select statement, rather than having to execute a subsequent
            // select statement
            
            HqlQuery query = HqlQuery.FromSearchCriteria(
                "from PatientProfile p",
                new string[] { "p"},
                new SearchCriteria[] { criteria },
                page);

            IList<PatientProfile> results = MakeTypeSafe(ExecuteHql(query));

            // due to use of "join fetch", the result set may contain duplicates
            // (this is a fundamental limitation of SQL)
            // these should be removed
            List<PatientProfile> filtered = new List<PatientProfile>();
            foreach(PatientProfile p in results)
            {
                if(!filtered.Contains(p))
                    filtered.Add(p);
            }

            return filtered;
        }
    }
}
