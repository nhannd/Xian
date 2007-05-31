using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services
{
    public abstract class WorkflowServiceBase : ApplicationServiceBase
    {
        protected IExtensionPoint _worklistExtPoint;
        protected IExtensionPoint _operationExtPoint;

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
        protected class OperationEnablementAttribute : Attribute
        {
            private string _enablementMethodName;

            public OperationEnablementAttribute(string enablementMethodName)
            {
                _enablementMethodName = enablementMethodName;
            }

            public string EnablementMethodName
            {
                get { return _enablementMethodName; }
            }
        }

        protected class PersistentWorkflow : IWorkflow
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
            return worklist.GetWorklist(this.CurrentUserStaff, this.PersistenceContext);
        }

        protected int GetWorklistCount(string worklistClassName)
        {
            IWorklist worklist = (IWorklist)_worklistExtPoint.CreateExtension(new ClassNameExtensionFilter(worklistClassName));
            return worklist.GetWorklistCount(this.CurrentUserStaff, this.PersistenceContext);
        }

        protected Dictionary<string, bool> GetOperationEnablement(IWorklistItemKey itemKey)
        {
            Dictionary<string, bool> results = new Dictionary<string, bool>();

            Type serviceContractType = this.GetType();
            foreach (MethodInfo info in serviceContractType.GetMethods())
            {
                object[] attribs = info.GetCustomAttributes(typeof(OperationEnablementAttribute), true);
                if (attribs.Length < 1)
                    continue;

                // Evaluate the list of enablement method in the OperationEnablementAttribute

                bool enablement = true;
                foreach (object obj in attribs)
                {
                    OperationEnablementAttribute attrib = obj as OperationEnablementAttribute;

                    MethodInfo enablementHelper = serviceContractType.GetMethod(attrib.EnablementMethodName);
                    if (enablementHelper == null)
                        throw new EnablementMethodNotFoundException(attrib.EnablementMethodName, info.Name);

                    bool test = (bool) enablementHelper.Invoke(this, new object[] { itemKey });
                    if (test == false)
                    {
                        // No need to continue after any evaluation failed
                        enablement = false;
                        break;
                    }
                }

                results.Add(info.Name, enablement);
            }

            return results;
        }
    }
}
