using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.ProtocolAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IProtocolAdminService))]
    public class ProtocolAdminService : ApplicationServiceBase, IProtocolAdminService
    {
        #region IProtocolAdminService Members

        [UpdateOperation]
        public AddProtocolCodeResponse AddProtocolCode(AddProtocolCodeRequest request)
        {
            ProtocolCode protocolCode = new ProtocolCode(request.Name, request.Description);

            this.PersistenceContext.Lock(protocolCode, DirtyState.New);
            this.PersistenceContext.SynchState();

            return new AddProtocolCodeResponse(new ProtocolCodeDetail(protocolCode.Name, protocolCode.Description));
        }

        #endregion
    }
}
