using System.ServiceModel;
using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    [ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName = "IServerDirectory", Namespace = ServerDirectoryNamespace.Value)]
    public interface IServerDirectory
    {
        [OperationContract]
        GetServersResult GetServers(GetServersRequest request);

        [OperationContract]
        [FaultContract(typeof(ServerExistsFault))]
        AddServerResult AddServer(AddServerRequest request);

        [OperationContract]
        [FaultContract(typeof(ServerNotFoundFault))]
        UpdateServerResult UpdateServer(UpdateServerRequest request);

        [OperationContract]
        [FaultContract(typeof(ServerNotFoundFault))]
        DeleteServerResult DeleteServer(DeleteServerRequest request);

        [OperationContract]
        [FaultContract(typeof(ServerNotFoundFault))]
        DeleteAllServersResult DeleteAllServers(DeleteAllServersRequest request);
    }
}
