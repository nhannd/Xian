using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    [ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName = "IServerDirectory", Namespace = ServerDirectoryNamespace.Value)]
    public interface IServerDirectory
    {
        [OperationContract]
        GetServersResult GetServers(GetServersRequest request);

        [OperationContract]
        AddServerResult AddServer(AddServerRequest request);

        [OperationContract]
        UpdateServerResult UpdateServer(UpdateServerRequest request);

        [OperationContract]
        DeleteServerResult DeleteServer(DeleteServerRequest request);
    }

    /// <summary>
    /// Validation service for operations on the servers.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName = "IServerDirectoryValidation", Namespace = ServerDirectoryNamespace.Value)]
    public interface IServerDirectoryValidation
    {
    }
}
