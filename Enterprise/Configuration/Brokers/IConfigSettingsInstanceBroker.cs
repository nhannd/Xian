using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Configuration.Brokers
{
    public partial interface IConfigSettingsInstanceBroker
    {
        IList<ConfigSettingsInstance> Find(ConfigSettingsGroupSearchCriteria groupCriteria, ConfigSettingsInstanceSearchCriteria instanceCriteria);
    }
}
