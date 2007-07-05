using System.Security.Permissions;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.RequestedProcedureTypeGroupAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IRequestedProcedureTypeGroupAdminService))]
    public class RequestedProcedureTypeGroupAdminService : ApplicationServiceBase, IRequestedProcedureTypeGroupAdminService
    {
        #region IRequestedProcedureTypeGroupAdminService Members

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.RequestedProcedureTypeGroupAdmin)]
        public GetRequestedProcedureTypeGroupEditFormDataResponse GetRequestedProcedureTypeGroupEditFormData(
            GetRequestedProcedureTypeGroupEditFormDataRequest request)
        {
            return new GetRequestedProcedureTypeGroupEditFormDataResponse();
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.RequestedProcedureTypeGroupAdmin)]
        public ListRequestedProcedureTypeGroupsResponse ListRequestedProcedureTypeGroups(
            ListRequestedProcedureTypeGroupsRequest request)
        {
            return new ListRequestedProcedureTypeGroupsResponse();
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.RequestedProcedureTypeGroupAdmin)]
        public LoadRequestedProcedureTypeGroupForEditResponse LoadRequestedProcedureTypeGroupForEdit(
            LoadRequestedProcedureTypeGroupForEditRequest request)
        {
            return new LoadRequestedProcedureTypeGroupForEditResponse(request.EntityRef, new RequestedProcedureTypeGroupDetail());
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.RequestedProcedureTypeGroupAdmin)]
        public AddRequestedProcedureTypeGroupResponse AddRequestedProcedureTypeGroup(
            AddRequestedProcedureTypeGroupRequest request)
        {
            return new AddRequestedProcedureTypeGroupResponse(new RequestedProcedureTypeGroupSummary());
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.RequestedProcedureTypeGroupAdmin)]
        public UpdateRequestedProcedureTypeGroupResponse UpdateRequestedProcedureTypeGroup(
            UpdateRequestedProcedureTypeGroupRequest request)
        {
            return new UpdateRequestedProcedureTypeGroupResponse(new RequestedProcedureTypeGroupSummary());
        }

        #endregion
    }
}
