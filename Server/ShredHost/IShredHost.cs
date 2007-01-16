using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Server.ShredHost
{
    [ServiceContract]
    public interface IShredHost
    {
        [OperationContract]
        void Start();

        [OperationContract]
        void Stop();

        [OperationContract]
        WcfDataShredCollection GetShreds();

        [OperationContract]
        void StartShred(WcfDataShred shred);

        [OperationContract]
        void StopShred(WcfDataShred shred);
    }
}
