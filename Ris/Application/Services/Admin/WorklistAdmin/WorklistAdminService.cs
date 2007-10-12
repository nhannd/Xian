#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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

            UserAssembler userAssembler = new UserAssembler();
            response.Users = CollectionUtils.Map<User, UserSummary, List<UserSummary>>(
                this.PersistenceContext.GetBroker<IUserBroker>().FindAll(),
                delegate(User user)
                {
                    return userAssembler.GetUserSummary(user);
                });

            // TODO: Need stronger typed representation of worklist type options.  See bug #886
            response.WorklistTypes = new List<string>();
            response.WorklistTypes.AddRange(WorklistFactory.Instance.WorklistTypes);

            return response;
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.WorklistAdmin)]
        public ListRequestedProcedureTypeGroupsForWorklistCategoryResponse ListRequestedProcedureTypeGroupsForWorklistCategory(ListRequestedProcedureTypeGroupsForWorklistCategoryRequest request)
        {
            ListRequestedProcedureTypeGroupsForWorklistCategoryResponse response = 
                new ListRequestedProcedureTypeGroupsForWorklistCategoryResponse();

            RequestedProcedureTypeGroupAssembler assembler = new RequestedProcedureTypeGroupAssembler();
            RequestedProcedureTypeGroupSearchCriteria criteria = new RequestedProcedureTypeGroupSearchCriteria();
            criteria.Category.EqualTo(GetGroupCategoryFromWorklistType(request.WorklistType));

            response.RequestedProcedureTypeGroups =
                CollectionUtils.Map<RequestedProcedureTypeGroup, RequestedProcedureTypeGroupSummary>(
                    this.PersistenceContext.GetBroker<IRequestedProcedureTypeGroupBroker>().Find(criteria),
                    delegate(RequestedProcedureTypeGroup group)
                    {
                        return assembler.GetRequestedProcedureTypeGroupSummary(group, this.PersistenceContext);
                    });

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

        private RequestedProcedureTypeGroupCategory GetGroupCategoryFromWorklistType(string type)
        {
            // TODO: Hard-coding needs to be removed. See bug #886
            if(string.Compare(type, "ReportingToBeReportedWorklist") == 0)
            {
                return RequestedProcedureTypeGroupCategory.READING;
            }
            else
            {
                return RequestedProcedureTypeGroupCategory.MODALITY;
            }
        }

        private bool WorklistExists(string name, string type)
        {
            return this.PersistenceContext.GetBroker<IWorklistBroker>().NameExistsForType(name, type);
        }
    }
}
