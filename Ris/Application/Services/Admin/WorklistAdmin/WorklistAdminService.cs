using System.Collections.Generic;
using System.Security.Permissions;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Ris.Application.Services.Admin.AuthenticationAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
    [ServiceImplementsContract(typeof(IWorklistAdminService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class WorklistAdminService : ApplicationServiceBase, IWorklistAdminService
    {
        #region IWorklistAdminService Members

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.WorklistAdmin)]
        public GetWorklistEditFormDataResponse GetWorklistEditFormData(GetWorklistEditFormDataRequest request)
        {
            GetWorklistEditFormDataResponse response = new GetWorklistEditFormDataResponse();

            RequestedProcedureTypeGroupAssembler assembler = new RequestedProcedureTypeGroupAssembler();
            response.RequestedProcedureTypeGroups = CollectionUtils.Map<RequestedProcedureTypeGroup, RequestedProcedureTypeGroupSummary, List<RequestedProcedureTypeGroupSummary>>(
                this.PersistenceContext.GetBroker<IRequestedProcedureTypeGroupBroker>().FindAll(),
                delegate(RequestedProcedureTypeGroup rptGroup)
                {
                    return assembler.GetRequestedProcedureTypeGroupSummary(rptGroup, this.PersistenceContext);
                });

            UserAssembler userAssembler = new UserAssembler();
            response.Users = CollectionUtils.Map<User, UserSummary, List<UserSummary>>(
                this.PersistenceContext.GetBroker<IUserBroker>().FindAll(),
                delegate(User user)
                {
                    return userAssembler.GetUserSummary(user);
                });

            response.WorklistTypes = new List<string>();
            response.WorklistTypes.AddRange(WorklistFactory.Instance.WorklistTypes);

            return response;
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.WorklistAdmin)]
        public ListWorklistsResponse ListWorklists(ListWorklistsRequest request)
        {
            ListWorklistsResponse response = new ListWorklistsResponse();
            WorklistAdminAssembler adminAssembler = new WorklistAdminAssembler();

            response.WorklistSummaries = CollectionUtils.Map<Worklist, WorklistAdminSummary, List<WorklistAdminSummary>>(
                this.PersistenceContext.GetBroker<IWorklistBroker>().Find(
                    new WorklistSearchCriteria(),
                    new SearchResultPage(request.PageRequest.FirstRow, request.PageRequest.MaxRows)),
                delegate(Worklist worklist)
                {
                    return adminAssembler.GetWorklistSummary(worklist, this.PersistenceContext);
                });

            return response;
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.WorklistAdmin)]
        public LoadWorklistForEditResponse LoadWorklistForEdit(LoadWorklistForEditRequest request)
        {
            Worklist worklist = PersistenceContext.Load<Worklist>(request.EntityRef);
            WorklistAdminAssembler adminAssembler = new WorklistAdminAssembler();
            WorklistAdminDetail adminDetail = adminAssembler.GetWorklistDetail(worklist, this.PersistenceContext);
            return new LoadWorklistForEditResponse(worklist.GetRef(), adminDetail);
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.WorklistAdmin)]
        public AddWorklistResponse AddWorklist(AddWorklistRequest request)
        {
            if(string.IsNullOrEmpty(request.Detail.Name))
            {
                throw new RequestValidationException(SR.ExceptionWorklistNameRequired);
            }

            if(WorklistExists(request.Detail.Name, request.Detail.WorklistType))
            {
                throw new RequestValidationException(string.Format(SR.ExceptionWorklistNameAlreadyExists, request.Detail.Name));
            }

            WorklistAdminAssembler adminAssembler = new WorklistAdminAssembler();
            Worklist worklist = WorklistFactory.Instance.GetWorklist(request.Detail.WorklistType);
            adminAssembler.UpdateWorklist(worklist, request.Detail, this.PersistenceContext);

            PersistenceContext.Lock(worklist, DirtyState.New);
            PersistenceContext.SynchState();

            return new AddWorklistResponse(adminAssembler.GetWorklistSummary(worklist, this.PersistenceContext));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.WorklistAdmin)]
        public UpdateWorklistResponse UpdateWorklist(UpdateWorklistRequest request)
        {
            Worklist worklist = this.PersistenceContext.Load<Worklist>(request.EntityRef);

            if (worklist.Name != request.Detail.Name && WorklistExists(request.Detail.Name, request.Detail.WorklistType))
            {
                throw new RequestValidationException(string.Format(SR.ExceptionWorklistNameAlreadyExists, request.Detail.Name));
            }

            WorklistAdminAssembler adminAssembler = new WorklistAdminAssembler();
            adminAssembler.UpdateWorklist(worklist, request.Detail, this.PersistenceContext);

            return new UpdateWorklistResponse(adminAssembler.GetWorklistSummary(worklist, this.PersistenceContext));
        }

        #endregion

        private bool WorklistExists(string name, string type)
        {
            return this.PersistenceContext.GetBroker<IWorklistBroker>().NameExistsForType(name, type);
        }
    }
}
