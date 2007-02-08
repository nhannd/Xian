using System;

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise
{
    public interface ITimeService
    {
        DateTime GetTime();
    }

    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    class TimeService : ServiceLayer, ITimeService
    {
        #region ITimeService Members

        [ReadOperation(PersistenceScopeOption=PersistenceScopeOption.RequiresNew)]
        public DateTime GetTime()
        {
            ITimeBroker broker = this.CurrentContext.GetBroker<ITimeBroker>();
            return broker.GetTime();
        }

        #endregion
    }

}
