using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Services
{
    public abstract class WorkflowServiceBase : HealthcareServiceLayer
    {
        class PersistentWorkflow : IWorkflow
        {
            private IPersistenceContext _context;

            public PersistentWorkflow(IPersistenceContext context)
            {
                _context = context;
            }

            #region IWorkflow Members

            public void AddActivity(Activity activity)
            {
                _context.Lock(activity, DirtyState.New);
            }

            #endregion
        }

        protected void ExecuteOperation(ProcedureStep step, ClearCanvas.Healthcare.Workflow.Operation operation)
        {
            // just hack in the user staff for the time-being
            IStaffBroker staffBroker = CurrentContext.GetBroker<IStaffBroker>();
            operation.CurrentUserStaff = staffBroker.FindOne(new StaffSearchCriteria());

            operation.Execute(step, new PersistentWorkflow(CurrentContext));
        }
    }
}
