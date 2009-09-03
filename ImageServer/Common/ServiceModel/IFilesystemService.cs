using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Common.ServiceModel
{
    [ImageServerService]
    [ServiceContract]
    [Authentication(false)]
    public interface IFilesystemService
    {
        [OperationContract]
        FilesystemInfo GetFilesystemInfo(string path);
    }
}