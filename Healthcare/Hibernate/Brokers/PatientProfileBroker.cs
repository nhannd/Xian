using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Common;

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
            HqlQuery query = new HqlQuery(
                "from PatientProfile p join fetch p.Patient",
                HqlCondition.FromSearchCriteria("p", criteria),
                HqlSort.FromSearchCriteria("p", criteria),
                page);

            return MakeTypeSafe<PatientProfile>(ExecuteHql(query));
        }
    }
}
