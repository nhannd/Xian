using System.Collections.Generic;
using System.Security.Permissions;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

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

            // TODO: 
            response.WorklistTypes = new List<string>();
            response.WorklistTypes.Add("Foo");
            response.WorklistTypes.Add("Bar");

            return response;
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.WorklistAdmin)]
        public ListWorklistsResponse ListWorklists(ListWorklistsRequest request)
        {
            ListWorklistsResponse response = new ListWorklistsResponse();
            WorklistAssembler assembler = new WorklistAssembler();

            response.WorklistSummaries = CollectionUtils.Map<Worklist, WorklistSummary, List<WorklistSummary>>(
                this.PersistenceContext.GetBroker<IWorklistBroker>().Find(
                    new WorklistSearchCriteria(),
                    new SearchResultPage(request.PageRequest.FirstRow, request.PageRequest.MaxRows)),
                delegate(Worklist worklist)
                {
                    return assembler.GetWorklistSummary(worklist, this.PersistenceContext);
                });

            return response;
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.WorklistAdmin)]
        public LoadWorklistForEditResponse LoadWorklistForEdit(LoadWorklistForEditRequest request)
        {
            Worklist worklist = PersistenceContext.Load<Worklist>(request.EntityRef);
            WorklistAssembler assembler = new WorklistAssembler();
            WorklistDetail detail = assembler.GetWorklistDetail(worklist, this.PersistenceContext);
            return new LoadWorklistForEditResponse(worklist.GetRef(), detail);
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.WorklistAdmin)]
        public AddWorklistResponse AddWorklist(AddWorklistRequest request)
        {
            if(string.IsNullOrEmpty(request.Detail.Name))
            {
                throw new RequestValidationException(SR.ExceptionWorklistNameRequired);
            }

            if(WorklistExists(request.Detail.Name))
            {
                throw new RequestValidationException(string.Format(SR.ExceptionWorklistNameAlreadyExists, request.Detail.Name));
            }

            WorklistAssembler assembler = new WorklistAssembler();
            Worklist worklist = assembler.WorklistFactory(request.Detail.WorklistType);
            assembler.UpdateWorklist(worklist, request.Detail, this.PersistenceContext);

            PersistenceContext.Lock(worklist, DirtyState.New);
            PersistenceContext.SynchState();

            return new AddWorklistResponse(assembler.GetWorklistSummary(worklist, this.PersistenceContext));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.WorklistAdmin)]
        public UpdateWorklistResponse UpdateWorklist(UpdateWorklistRequest request)
        {
            Worklist worklist = this.PersistenceContext.Load<Worklist>(request.EntityRef);
            WorklistAssembler assembler = new WorklistAssembler();
            assembler.UpdateWorklist(worklist, request.WorklistDetail, this.PersistenceContext);

            return new UpdateWorklistResponse(assembler.GetWorklistSummary(worklist, this.PersistenceContext));
        }

        #endregion

        private bool WorklistExists(string name)
        {
            WorklistSearchCriteria criteria = new WorklistSearchCriteria();
            criteria.Name.EqualTo(name);
            return this.PersistenceContext.GetBroker<IWorklistBroker>().Find(criteria).Count != 0;
        }
    }
}
