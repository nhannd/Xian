using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Workflow;
using System.Collections;

namespace ClearCanvas.Ris.Application.Services
{
    public abstract class WorkflowServiceBase : ApplicationServiceBase
    {
        protected IExtensionPoint _worklistExtPoint;
        protected IExtensionPoint _operationExtPoint;

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

            public IPersistenceContext CurrentContext 
            {
                get { return _context; }
            }

            #endregion
        }

        protected IList GetWorklist(string worklistClassName)
        {
            IWorklist worklist = (IWorklist)_worklistExtPoint.CreateExtension(new ClassNameExtensionFilter(worklistClassName));
            return worklist.GetWorklist(this.PersistenceContext);
        }

        //protected void ExecuteOperation(IWorklistItem item, IList parameters, OperationBase operation)
        //{
        //    // TODO: just hack in the user staff for the time-being
        //    IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
        //    operation.CurrentUserStaff = staffBroker.FindOne(new StaffSearchCriteria());

        //    operation.Execute(item, parameters, new PersistentWorkflow(PersistenceContext));
        //}

        protected Dictionary<string, bool> GetOperationEnablement(IWorklistItem item)
        {
            //TODO: HACK WARNING for GetOperationEnablement!!!
            // Depends on the worklistClassName of the particular worklist item, the Dictionary returns 
            // whether a particular service operation should be enable for that item.  Ideally, we want 
            // to get rid of the switch-case statement.  But this will do for now.  The Operation extension 
            // point architecture still exist, we are not using it at this point.

            Dictionary<string, bool> results = new Dictionary<string, bool>();
            switch (item.WorklistClassName)
            {
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Scheduled":
                    results.Add("CheckInProcedure", true);
                    results.Add("CancelOrder", true);
                    break;
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+CheckIn":
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+InProgress":
                    results.Add("CheckInProcedure", false);
                    results.Add("CancelOrder", true);
                    break;
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Completed":
                case "ClearCanvas.Healthcare.Workflow.Registration.Worklists+Cancelled":
                default:
                    results.Add("CheckInProcedure", false);
                    results.Add("CancelOrder", false);
                    break;
            }

            //foreach (IOperation op in _operationExtPoint.CreateExtensions())
            //{
            //    op.CurrentUserStaff = userStaff;
            //    results.Add(op.GetType().FullName, op.InputSpecification.Test(item).Success);
            //}
            return results;
        }
    }
}
