using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using System.Collections;

namespace ClearCanvas.Healthcare.Workflow.Registration
{
    [ExtensionPoint]
    public class WorkflowOperationExtensionPoint : ExtensionPoint<IOperation>
    {
    }

    public class Operations
    {
        [ExtensionOf(typeof(WorkflowOperationExtensionPoint))]
        public class CheckIn : OperationBase
        {
            public override void Execute(IWorklistItem item, IList parameters ,IWorkflow workflow)
            {
                Platform.ShowMessageBox("CheckIn Not Implemented");

                //if (parameters == null)
                //    return;

                //foreach (EntityRef rpRef in parameters)
                //{
                //    RequestedProcedure rp = workflow.CurrentContext.GetBroker<IRequestedProcedureBroker>().Load(rpRef);
                    
                //    CheckInProcedureStep cps = new CheckInProcedureStep(rp);
                //    cps.Start(this.CurrentUserStaff);
                //    cps.Complete(this.CurrentUserStaff);

                //    rp.CheckInProcedureSteps.Add(cps);
                //    workflow.CurrentContext.Lock(rp, DirtyState.Dirty);
                //}
            }

            protected override bool CanExecute(IWorklistItem item)
            {
                return item.WorklistClassName == "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Scheduled";
            }
        }

        [ExtensionOf(typeof(WorkflowOperationExtensionPoint))]
        public class Cancel : OperationBase
        {
            public override void Execute(IWorklistItem item, IList parameters, IWorkflow workflow)
            {
                Platform.ShowMessageBox("Cancel Not Implemented");
            }

            protected override bool CanExecute(IWorklistItem item)
            {
                return item.WorklistClassName == "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Scheduled"
                    || item.WorklistClassName == "ClearCanvas.Healthcare.Workflow.Registration.Worklists+CheckIn"
                    || item.WorklistClassName == "ClearCanvas.Healthcare.Workflow.Registration.Worklists+InProgress";
            }
        }
    
    }
}
