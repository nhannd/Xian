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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Ris.Application.Services.Admin.AuthenticationAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
    internal class WorklistAdminAssembler
    {
        public WorklistAdminDetail GetWorklistDetail(Worklist worklist, IPersistenceContext context)
        {
            WorklistAdminDetail detail = new WorklistAdminDetail(worklist.GetRef(), worklist.Name, worklist.Description, WorklistFactory.Instance.GetWorklistType(worklist));

            RequestedProcedureTypeGroupAssembler assembler = new RequestedProcedureTypeGroupAssembler();
            detail.RequestedProcedureTypeGroups = CollectionUtils.Map<RequestedProcedureTypeGroup, RequestedProcedureTypeGroupSummary, List<RequestedProcedureTypeGroupSummary>>(
                worklist.RequestedProcedureTypeGroups,
                delegate(RequestedProcedureTypeGroup rptGroup)
                    {
                        return assembler.GetRequestedProcedureTypeGroupSummary(rptGroup, context);
                    });

            UserAssembler userAssembler = new UserAssembler();
            detail.Users = CollectionUtils.Map<User, UserSummary, List<UserSummary>>(
                worklist.Users,
                delegate(User user)
                    {
                        return userAssembler.GetUserSummary(user);
                    });

            return detail;
        }

        public WorklistAdminSummary GetWorklistSummary(Worklist worklist, IPersistenceContext context)
        {
            return new WorklistAdminSummary(worklist.GetRef(), worklist.Name, worklist.Description, WorklistFactory.Instance.GetWorklistType(worklist));
        }

        public void UpdateWorklist(Worklist worklist, WorklistAdminDetail detail, IPersistenceContext context)
        {
            worklist.Name = detail.Name;
            worklist.Description = detail.Description;
            
            worklist.RequestedProcedureTypeGroups.Clear();
            detail.RequestedProcedureTypeGroups.ForEach(delegate(RequestedProcedureTypeGroupSummary summary)
            {
                worklist.RequestedProcedureTypeGroups.Add(context.Load<RequestedProcedureTypeGroup>(summary.EntityRef));
            });

            worklist.Users.Clear();
            detail.Users.ForEach(delegate (UserSummary summary)
            {
                worklist.Users.Add(context.Load<User>(summary.EntityRef));
            });
        }
    }
}