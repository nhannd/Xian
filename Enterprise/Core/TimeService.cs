using System;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(ITimeService))]
    public class TimeService : ITimeService
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
