using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Configuration.Brokers;
using ClearCanvas.Enterprise.Hibernate;
using System.Collections;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Configuration.Hibernate.Brokers
{
    public partial class ConfigSettingsInstanceBroker
    {
        public IList<ConfigSettingsInstance> Find(ConfigSettingsGroupSearchCriteria groupCriteria, ConfigSettingsInstanceSearchCriteria instanceCriteria)
        {
            HqlQuery query = new HqlQuery("from ConfigSettingsInstance i join fetch i.Group g");
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("i", instanceCriteria));
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("g", groupCriteria));

            return MakeTypeSafe<ConfigSettingsInstance>(this.ExecuteHql(query));
        }
    }
}
