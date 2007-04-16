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

        public DateTime GetTime()
        {
            // TODO: obtain the time from some central source.  Sql server may be used for this purpose, however:
            // do not use NHibernate to communicate with Sql server to obtain the time!!!
            // this is because initializing NHibernate may itself cause this method to be invoked -> infinite recursion
            return DateTime.Now;
        }

        #endregion
    }

}
