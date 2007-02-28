using System;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Data;

namespace ClearCanvas.Enterprise
{
    [ExtensionOf(typeof(ServiceLayerExtensionPoint))]
    class TimeService : ServiceLayer, ITimeService
    {
        #region ITimeService Members

        [ReadOperation(PersistenceScopeOption=PersistenceScopeOption.RequiresNew, Auditable=false)]
        public DateTime GetTime()
        {
            ITimeBroker broker = PersistenceScope.Current.GetBroker<ITimeBroker>();
            return broker.GetTime();
        }

        #endregion
    }

}
