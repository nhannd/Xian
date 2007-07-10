using System.Collections.Generic;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.RequestedProcedureTypeGroupAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IRequestedProcedureTypeGroupAdminService))]
    public class RequestedProcedureTypeGroupAdminService : ApplicationServiceBase, IRequestedProcedureTypeGroupAdminService
    {
        #region IRequestedProcedureTypeGroupAdminService Members

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.RequestedProcedureTypeGroupAdmin)]
        public GetRequestedProcedureTypeGroupEditFormDataResponse GetRequestedProcedureTypeGroupEditFormData(
            GetRequestedProcedureTypeGroupEditFormDataRequest request)
        {
            GetRequestedProcedureTypeGroupEditFormDataResponse response = new GetRequestedProcedureTypeGroupEditFormDataResponse();

            // Category choices
            response.Categories = CollectionUtils.Map<RequestedProcedureTypeGroupCategoryEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IRequestedProcedureTypeGroupCategoryEnumBroker>().Load().Items,
                delegate(RequestedProcedureTypeGroupCategoryEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            // RequestedProcedureType choices
            RequestedProcedureTypeAssembler assembler = new RequestedProcedureTypeAssembler();
            response.RequestedProcedureTypes = CollectionUtils.Map<RequestedProcedureType, RequestedProcedureTypeSummary, List<RequestedProcedureTypeSummary>>(
                PersistenceContext.GetBroker<IRequestedProcedureTypeBroker>().FindAll(),
                delegate(RequestedProcedureType rpt)
                {
                    return assembler.GetRequestedProcedureTypeSummary(rpt);
                });

            return response;
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.RequestedProcedureTypeGroupAdmin)]
        public ListRequestedProcedureTypeGroupsResponse ListRequestedProcedureTypeGroups(
            ListRequestedProcedureTypeGroupsRequest request)
        {
            ListRequestedProcedureTypeGroupsResponse response = new ListRequestedProcedureTypeGroupsResponse();
            RequestedProcedureTypeGroupAssembler assembler = new RequestedProcedureTypeGroupAssembler();

            response.Items = CollectionUtils.Map<RequestedProcedureTypeGroup, RequestedProcedureTypeGroupSummary, List<RequestedProcedureTypeGroupSummary>>(
                PersistenceContext.GetBroker<IRequestedProcedureTypeGroupBroker>().Find(
                    new RequestedProcedureTypeGroupSearchCriteria(), 
                    new SearchResultPage(request.PageRequest.FirstRow, request.PageRequest.MaxRows)),
                delegate(RequestedProcedureTypeGroup rptGroup)
                {
                    return assembler.GetRequestedProcedureTypeGroupSummary(rptGroup, this.PersistenceContext);
                }); 

            return response;
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.RequestedProcedureTypeGroupAdmin)]
        public LoadRequestedProcedureTypeGroupForEditResponse LoadRequestedProcedureTypeGroupForEdit(
            LoadRequestedProcedureTypeGroupForEditRequest request)
        {
            RequestedProcedureTypeGroup rptGroup = PersistenceContext.GetBroker<IRequestedProcedureTypeGroupBroker>().Load(request.EntityRef);
            RequestedProcedureTypeGroupAssembler assembler = new RequestedProcedureTypeGroupAssembler();
            RequestedProcedureTypeGroupDetail detail = assembler.GetRequestedProcedureTypeGroupDetail(rptGroup, this.PersistenceContext);
            return new LoadRequestedProcedureTypeGroupForEditResponse(rptGroup.GetRef(), detail);
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.RequestedProcedureTypeGroupAdmin)]
        public AddRequestedProcedureTypeGroupResponse AddRequestedProcedureTypeGroup(
            AddRequestedProcedureTypeGroupRequest request)
        {
            if (string.IsNullOrEmpty(request.Detail.Name))
            {
                throw new RequestValidationException(SR.ExceptionRequestedProcedureTypeGroupNameRequired);
            }

            if (GroupExists(request.Detail.Name))
            {
                throw new RequestValidationException(string.Format(SR.ExceptionRequestedProcedureTypeGroupNameAlreadyExists, request.Detail.Name));
            }

            RequestedProcedureTypeGroup group = new RequestedProcedureTypeGroup();
            RequestedProcedureTypeGroupAssembler assembler = new RequestedProcedureTypeGroupAssembler();
            assembler.UpdateRequestedProcedureTypeGroup(group, request.Detail, this.PersistenceContext);

            PersistenceContext.Lock(group, DirtyState.New);
            PersistenceContext.SynchState();

            return new AddRequestedProcedureTypeGroupResponse(
                assembler.GetRequestedProcedureTypeGroupSummary(group, this.PersistenceContext));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.RequestedProcedureTypeGroupAdmin)]
        public UpdateRequestedProcedureTypeGroupResponse UpdateRequestedProcedureTypeGroup(
            UpdateRequestedProcedureTypeGroupRequest request)
        {
            RequestedProcedureTypeGroup group = PersistenceContext.Load<RequestedProcedureTypeGroup>(request.EntityRef);
            RequestedProcedureTypeGroupAssembler assembler = new RequestedProcedureTypeGroupAssembler();
            assembler.UpdateRequestedProcedureTypeGroup(group, request.Detail, this.PersistenceContext);

            return new UpdateRequestedProcedureTypeGroupResponse(
                assembler.GetRequestedProcedureTypeGroupSummary(group, this.PersistenceContext));
        }

        #endregion

        private bool GroupExists(string name)
        {
            RequestedProcedureTypeGroupSearchCriteria criteria = new RequestedProcedureTypeGroupSearchCriteria();
            criteria.Name.EqualTo(name);
            return PersistenceContext.GetBroker<IRequestedProcedureTypeGroupBroker>().Find(criteria).Count != 0;
        }
    }
}
