using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    [ServiceContract]
    public interface IProtocolAdminService
    {
        [OperationContract]
        AddProtocolCodeResponse AddProtocolCode(AddProtocolCodeRequest request);
    }
}
