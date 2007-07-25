using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin;
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