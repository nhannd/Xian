using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.WorklistTest;
using ClearCanvas.Ris.Application.Services.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Services.WorklistTest
{
    [ServiceImplementsContract(typeof(IWorklistTestService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class WorklistTestService : ApplicationServiceBase, IWorklistTestService
    {
        #region IWorklistTestService Members

        [ReadOperation]
        public GetAWorklistResponse GetAWorklist(GetAWorklistRequest request)
        {
            Worklist worklist = this.PersistenceContext.GetBroker<IWorklistBroker>().FindOne(new WorklistSearchCriteria());

            WorklistAssembler assembler = new WorklistAssembler();
            return new GetAWorklistResponse(assembler.GetWorklistSummary(worklist, this.PersistenceContext));
        }

        [ReadOperation]
        public DoWorklistTestResponse DoWorklistTest(DoWorklistTestRequest request)
        {
            Worklist aWorklist = this.PersistenceContext.Load<Worklist>(request.WorklistRef);

            int count = aWorklist.GetWorklistCount(this.CurrentUserStaff, this.PersistenceContext);

            return new DoWorklistTestResponse(string.Format("Name: {0}\r\nCount: {1}", aWorklist.Name, count));
        }

        [ReadOperation]
        public GetPersistentWorklistResponse GetPersistentWorklist(GetPersistentWorklistRequest request)
        {
            Worklist worklist = this.PersistenceContext.Load<Worklist>(request.WorklistRef);

            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();
            List<RegistrationWorklistItem> items = CollectionUtils.Map<WorklistItem, RegistrationWorklistItem, List<Common.RegistrationWorkflow.RegistrationWorklistItem>>(
                worklist.GetWorklist(this.CurrentUserStaff, this.PersistenceContext),
                delegate(WorklistItem worklistItem)
                {
                    return assembler.CreateRegistrationWorklistItem(worklistItem, this.PersistenceContext);
                });

            return new GetPersistentWorklistResponse(items);
        }

        [ReadOperation]
        public GetPersistentWorklistCountResponse GetPersistentWorklistCount(GetPersistentWorklistCountRequest request)
        {
            Worklist worklist = this.PersistenceContext.Load<Worklist>(request.WorklistRef);
            return new GetPersistentWorklistCountResponse(worklist.GetWorklistCount(this.CurrentUserStaff, this.PersistenceContext));
        }

        #endregion
    }
}
