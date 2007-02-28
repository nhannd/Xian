using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Server.DicomServerShred
{
    [ServiceContract]
    public interface IDicomServerShredInterface
    {
        [OperationContract]
        string GetServerInfo();
    }
}
