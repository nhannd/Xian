using System;
using System.Collections.Generic;

using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public partial class LocationBroker : EntityBroker<Location, LocationSearchCriteria>, ILocationBroker
    {
        /// <summary>
        /// Override to search Facilities as well
        /// </summary>
        public override IList<Location> Find(LocationSearchCriteria criteria, SearchResultPage page)
        {
            HqlQuery query = new HqlQuery(
                "from Location l join fetch l.Facility",
                HqlCondition.FromSearchCriteria("l", criteria),
                HqlSort.FromSearchCriteria("l", criteria),
                page);

            return MakeTypeSafe<Location>(ExecuteHql(query));
        }
    }
}
