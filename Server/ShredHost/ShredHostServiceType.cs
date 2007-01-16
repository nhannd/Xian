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
        public void Start()
        {
            ShredHost.Start();
        }

        public void Stop()
        {
            ShredHost.Stop();
        }

        public WcfDataShredCollection GetShreds()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void StartShred(WcfDataShred shred)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void StopShred(WcfDataShred shred)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
