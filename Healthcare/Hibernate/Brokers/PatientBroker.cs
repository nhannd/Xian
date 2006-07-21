using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public partial class PatientBroker : EntityBroker<Patient, PatientSearchCriteria>, IPatientBroker
    {
        /// <summary>
        /// Overridden to search patient identifiers as well
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public override IList<Patient> Find(PatientSearchCriteria criteria, SearchResultPage page)
        {
            // note use of "join fetch" - this is important to ensure that the patient identifiers are fetched
            // along with the patient in a single select statement, rather than having to execute a subsequent
            // select statement
            HqlQuery query = HqlQuery.FromSearchCriteria(
                "from Patient p left join fetch p.Identifiers i",
                new string[] { "p", "i" },
                new SearchCriteria[] { criteria, criteria.Identifiers },
                page);
            
            return MakeTypeSafe(ExecuteHql(query));
        }
    }
}
