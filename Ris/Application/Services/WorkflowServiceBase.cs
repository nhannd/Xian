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
using System.Reflection;

namespace ClearCanvas.Ris.Application.Services
{
    public abstract class WorkflowServiceBase : ApplicationServiceBase
    {
        protected IExtensionPoint _worklistExtPoint;
        protected IExtensionPoint _operationExtPoint;

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        protected class OperationEnablementAttribute : Attribute
        {
            public OperationEnablementAttribute()
            {
            }
        }

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

        protected int GetWorklistCount(string worklistClassName)
        {
            IWorklist worklist = (IWorklist)_worklistExtPoint.CreateExtension(new ClassNameExtensionFilter(worklistClassName));
            return worklist.GetWorklistCount(this.PersistenceContext);
        }

        protected Dictionary<string, bool> GetOperationEnablement(IWorklistItem item)
        {
            Dictionary<string, bool> results = new Dictionary<string, bool>();

            Type serviceContractType = this.GetType();
            foreach (MethodInfo info in serviceContractType.GetMethods())
            {
                object[] attribs = info.GetCustomAttributes(typeof(OperationEnablementAttribute), true);
                if (attribs.Length < 1)
                    continue;

                try
                {
                    // Find the CanXXX helper class to evaluate operation enablement
                    MethodInfo enablementHelper = serviceContractType.GetMethod(String.Format("Can{0}", info.Name));
                    object test = enablementHelper.Invoke(this, new object[] { item });
                    results.Add(info.Name, (bool)test);
                }
                catch (Exception e)
                {
                    // Helper method not found
                    results.Add(info.Name, false);
                }
            }

            return results;
        }
    }
}
