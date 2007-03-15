using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Shreds
{
    [ServiceContract]
    public interface IDiskspaceManagementShredInterface
    {
        [OperationContract]
        string GetServerInfo();
    }
}
