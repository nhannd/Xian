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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
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
            response.Categories = EnumUtils.GetEnumValueList<RequestedProcedureTypeGroupCategoryEnum>(PersistenceContext);

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
    }
}
