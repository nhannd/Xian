using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Server.ShredHost
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class ShredHostServiceType : IShredHost
    {
        #region IShredHost Members

        public bool IsShredHostRunning()
        {
            return ShredHost.IsShredHostRunning;
        }

        public WcfDataShred[] GetShreds()
        {
            return ShredHost.ShredControllerList.WcfDataShredCollection;
        }

        public bool StartShred(WcfDataShred shred)
        {
            return ShredHost.StartShred(shred);
        }

        public bool StopShred(WcfDataShred shred)
        {
            return ShredHost.StopShred(shred);
        }

        #endregion
    }
}
