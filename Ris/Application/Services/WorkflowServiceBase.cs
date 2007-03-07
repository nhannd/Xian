using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Workflow;

namespace ClearCanvas.Ris.Application.Services
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

        protected void ExecuteOperation(ProcedureStep step, IExtensionPoint operationExtPoint, string operationClassName)
        {
            IOperation operation = (IOperation)operationExtPoint.CreateExtension(new ClassNameExtensionFilter(operationClassName));

            // just hack in the user staff for the time-being
            IStaffBroker staffBroker = CurrentContext.GetBroker<IStaffBroker>();
            operation.CurrentUserStaff = staffBroker.FindOne(new StaffSearchCriteria());

            operation.Execute(step, new PersistentWorkflow(CurrentContext));
        }

        protected IDictionary<string, bool> GetOperationEnablement(ProcedureStep step, IExtensionPoint operationExtPoint)
        {
            // just hack in the user staff for the time-being
            IStaffBroker staffBroker = CurrentContext.GetBroker<IStaffBroker>();
            Staff userStaff = staffBroker.FindOne(new StaffSearchCriteria());

            Dictionary<string, bool> results = new Dictionary<string, bool>();
            foreach (IOperation op in operationExtPoint.CreateExtensions())
            {
                op.CurrentUserStaff = userStaff;
                results.Add(op.GetType().FullName, op.InputSpecification.Test(step).Success);
            }
            return results;
        }

    }
}
