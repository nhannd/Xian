using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
    internal class WorklistAssembler
    {
        public WorklistDetail GetWorklistDetail(Worklist worklist, IPersistenceContext context)
        {
            WorklistDetail detail = new WorklistDetail(worklist.GetRef(), worklist.Name, worklist.Description, "Foo");

            RequestedProcedureTypeGroupAssembler assembler = new RequestedProcedureTypeGroupAssembler();
            detail.RequestedProcedureTypeGroups = CollectionUtils.Map<RequestedProcedureTypeGroup, RequestedProcedureTypeGroupSummary, List<RequestedProcedureTypeGroupSummary>>(
                worklist.RequestedProcedureTypeGroups,
                delegate(RequestedProcedureTypeGroup rptGroup)
                {
                    return assembler.GetRequestedProcedureTypeGroupSummary(rptGroup, context);
                });

            return detail;
        }

        public WorklistSummary GetWorklistSummary(Worklist worklist, IPersistenceContext context)
        {
            return new WorklistSummary(worklist.GetRef(), worklist.Name, worklist.Description, "Foo");
        }

        public void UpdateWorklist(Worklist worklist, WorklistDetail detail, IPersistenceContext context)
        {
            worklist.Name = detail.Name;
            worklist.Description = detail.Description;
            
            worklist.RequestedProcedureTypeGroups.Clear();
            detail.RequestedProcedureTypeGroups.ForEach(delegate(RequestedProcedureTypeGroupSummary summary)
            {
                worklist.RequestedProcedureTypeGroups.Add(context.Load<RequestedProcedureTypeGroup>(summary.EntityRef));
            });
        }


        // TODO: put this somewhere more appropriate
        public Worklist WorklistFactory(object type)
        {
            return new RegistrationCheckedInWorklist();
        }
    }
}