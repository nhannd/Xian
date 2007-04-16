using System;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(ITimeService))]
    public class TimeService : CoreServiceLayer, ITimeService
    {
        #region ITimeService Members

        //[ReadOperation(PersistenceScopeOption=PersistenceScopeOption.RequiresNew, Auditable=false)]
        public DateTime GetTime()
        {
            //ITimeBroker broker = PersistenceContext.GetBroker<ITimeBroker>();
            //return broker.GetTime();
            return DateTime.Now;
        }

        #endregion
    }

}
