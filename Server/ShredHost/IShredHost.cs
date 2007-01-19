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
        bool Start();

        [OperationContract]
        bool Stop();

        [OperationContract]
        bool IsShredHostRunning();

        [OperationContract]
        WcfDataShred[] GetShreds();

        [OperationContract]
        bool StartShred(WcfDataShred shred);

        [OperationContract]
        bool StopShred(WcfDataShred shred);
    }
}
