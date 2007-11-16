using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
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

            return new AddProtocolCodeResponse(new ProtocolCodeDetail(protocolCode.GetRef(), protocolCode.Name, protocolCode.Description));
        }

        [UpdateOperation]
        public UpdateProtocolCodeResponse UpdateProtocolCode(UpdateProtocolCodeRequest request)
        {
            throw new System.NotImplementedException();
        }

        [UpdateOperation]
        public DeleteProtocolCodeResponse DeleteProtocolCode(DeleteProtocolCodeRequest request)
        {
            throw new System.NotImplementedException();
        }

        [ReadOperation]
        public ListProtocolGroupsResponse ListProtocolGroups(ListProtocolGroupsRequest request)
        {
            List<ProtocolGroupSummary> protocolGroups = CollectionUtils.Map<ProtocolGroup, ProtocolGroupSummary>(
                this.PersistenceContext.GetBroker<IProtocolGroupBroker>().FindAll(),
                delegate(ProtocolGroup pg) { return new ProtocolGroupSummary(pg.GetRef(), pg.Name, pg.Description); });

            return new ListProtocolGroupsResponse(protocolGroups);
        }

        [ReadOperation]
        public LoadProtocolGroupForEditResponse LoadProtocolGroupForEdit(LoadProtocolGroupForEditRequest request)
        {
            ProtocolGroupAssembler assembler = new ProtocolGroupAssembler();
            throw new System.NotImplementedException();
        }

        [ReadOperation]
        public GetProtocolGroupEditFormDataResponse GetProtocolGroupEditFormData(
            GetProtocolGroupEditFormDataRequest request)
        {
            throw new System.NotImplementedException();
        }

        [UpdateOperation]
        public AddProtocolGroupResponse AddProtocolGroup(AddProtocolGroupRequest request)
        {
            throw new System.NotImplementedException();
        }

        [UpdateOperation]
        public UpdateProtocolGroupResponse UpdateProtocolGroup(UpdateProtocolGroupRequest request)
        {
            throw new System.NotImplementedException();
        }

        [UpdateOperation]
        public DeleteProtocolGroupResonse DeleteProtocolGroup(DeleteProtocolGroupRequest request)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
