using System;
using System.Collections.Generic;

using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public partial class LocationBroker : EntityBroker<Location, LocationSearchCriteria>, ILocationBroker
    {
        /// <summary>
        /// Override to search Facilities as well
        /// </summary>
        public override IList<Location> Find(LocationSearchCriteria criteria, SearchResultPage page)
        {
            HqlQuery query = HqlQuery.FromSearchCriteria(
                "from Location l join fetch l.Facility",
                new string[] { "l" },
                new SearchCriteria[] { criteria },
                page);

            return MakeTypeSafe<Location>(ExecuteHql(query));
        }
    }
}
