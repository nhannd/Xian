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
            ProtocolGroup group = this.PersistenceContext.Load<ProtocolGroup>(request.ProtocolGroupRef);

            ProtocolGroupAssembler assembler = new ProtocolGroupAssembler();
            return new LoadProtocolGroupForEditResponse(group.GetRef(), assembler.GetProtocolGroupDetail(group, this.PersistenceContext));
        }

        [ReadOperation]
        public GetProtocolGroupEditFormDataResponse GetProtocolGroupEditFormData(
            GetProtocolGroupEditFormDataRequest request)
        {
            List<ProtocolCodeDetail> codes = CollectionUtils.Map<ProtocolCode, ProtocolCodeDetail>(
                this.PersistenceContext.GetBroker<IProtocolCodeBroker>().FindAll(),
                delegate(ProtocolCode code) { return new ProtocolCodeDetail(code.GetRef(), code.Name, code.Description); });

            RequestedProcedureTypeGroupAssembler assembler = new RequestedProcedureTypeGroupAssembler();
            RequestedProcedureTypeGroupSearchCriteria criteria = new RequestedProcedureTypeGroupSearchCriteria();
            criteria.Category.EqualTo(RequestedProcedureTypeGroupCategory.READING);
            List<RequestedProcedureTypeGroupSummary> readingGroups = CollectionUtils.Map<RequestedProcedureTypeGroup, RequestedProcedureTypeGroupSummary>(
                this.PersistenceContext.GetBroker<IRequestedProcedureTypeGroupBroker>().Find(criteria),
                delegate(RequestedProcedureTypeGroup readingGroup) { return assembler.GetRequestedProcedureTypeGroupSummary(readingGroup, this.PersistenceContext); });

            return new GetProtocolGroupEditFormDataResponse(codes, readingGroups);
        }

        [UpdateOperation]
        public AddProtocolGroupResponse AddProtocolGroup(AddProtocolGroupRequest request)
        {
            ProtocolGroupAssembler assembler = new ProtocolGroupAssembler();

            ProtocolGroup group = new ProtocolGroup();
            assembler.UpdateProtocolGroup(group, request.Detail, this.PersistenceContext);

            this.PersistenceContext.Lock(group);
            this.PersistenceContext.SynchState();

            return new AddProtocolGroupResponse(assembler.GetProtocolGroupSummary(group));
        }

        [UpdateOperation]
        public UpdateProtocolGroupResponse UpdateProtocolGroup(UpdateProtocolGroupRequest request)
        {
            ProtocolGroup group = this.PersistenceContext.Load<ProtocolGroup>(request.ProtocolGroupRef);

            ProtocolGroupAssembler assembler = new ProtocolGroupAssembler();
            assembler.UpdateProtocolGroup(group, request.Detail, this.PersistenceContext);

            this.PersistenceContext.SynchState();

            return new UpdateProtocolGroupResponse(assembler.GetProtocolGroupSummary(group));
        }

        [UpdateOperation]
        public DeleteProtocolGroupResonse DeleteProtocolGroup(DeleteProtocolGroupRequest request)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
