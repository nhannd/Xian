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

        protected void ExecuteOperation(IWorklistItem item, IList parameters, string operationClassName)
        {
            IOperation operation = (IOperation)_operationExtPoint.CreateExtension(new ClassNameExtensionFilter(operationClassName));

            // TODO: just hack in the user staff for the time-being
            IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
            operation.CurrentUserStaff = staffBroker.FindOne(new StaffSearchCriteria());

            operation.Execute(item, parameters, new PersistentWorkflow(PersistenceContext));
        }

        protected Dictionary<string, bool> GetOperationEnablement(IWorklistItem item)
        {
            // just hack in the user staff for the time-being
            IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
            Staff userStaff = staffBroker.FindOne(new StaffSearchCriteria());

            Dictionary<string, bool> results = new Dictionary<string, bool>();
            foreach (IOperation op in _operationExtPoint.CreateExtensions())
            {
                op.CurrentUserStaff = userStaff;
                results.Add(op.GetType().FullName, op.InputSpecification.Test(item).Success);
            }
            return results;
        }

        //protected void ExecuteOperation(IWorklistItem item, OperationBase operation)
        //{
        //    // TODO: just hack in the user staff for the time-being
        //    IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
        //    operation.CurrentUserStaff = staffBroker.FindOne(new StaffSearchCriteria());

        //    operation.Execute(item, null, new PersistentWorkflow(PersistenceContext));
        //}

        //protected void ExecuteOperation(ProcedureStep step, ClearCanvas.Healthcare.Workflow.Operation operation)
        //{
        //    // just hack in the user staff for the time-being
        //    IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
        //    operation.CurrentUserStaff = staffBroker.FindOne(new StaffSearchCriteria());

        //    operation.Execute(step, new PersistentWorkflow(PersistenceContext));
        //}

        //protected void ExecuteOperation(ProcedureStep step, IExtensionPoint operationExtPoint, string operationClassName)
        //{
        //    IOperation operation = (IOperation)operationExtPoint.CreateExtension(new ClassNameExtensionFilter(operationClassName));

        //    // just hack in the user staff for the time-being
        //    IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
        //    operation.CurrentUserStaff = staffBroker.FindOne(new StaffSearchCriteria());

        //    operation.Execute(step, new PersistentWorkflow(PersistenceContext));
        //}

        //protected Dictionary<string, bool> GetOperationEnablement(ProcedureStep step, IExtensionPoint operationExtPoint)
        //{
        //    // just hack in the user staff for the time-being
        //    IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
        //    Staff userStaff = staffBroker.FindOne(new StaffSearchCriteria());

        //    Dictionary<string, bool> results = new Dictionary<string, bool>();
        //    foreach (IOperation op in operationExtPoint.CreateExtensions())
        //    {
        //        op.CurrentUserStaff = userStaff;
        //        results.Add(op.GetType().FullName, op.InputSpecification.Test(step).Success);
        //    }
        //    return results;
        //}

    }
}
