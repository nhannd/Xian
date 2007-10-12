#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
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

        protected IList GetWorklist(EntityRef worklistRef)
        {
            IWorklist  worklist = this.PersistenceContext.Load<Worklist>(worklistRef);
            return worklist.GetWorklist(this.CurrentUserStaff, this.PersistenceContext);
        }

        protected int GetWorklistCount(string worklistClassName)
        {
            IWorklist worklist = (IWorklist)_worklistExtPoint.CreateExtension(new ClassNameExtensionFilter(worklistClassName));
            return worklist.GetWorklistCount(this.CurrentUserStaff, this.PersistenceContext);
        }

        protected int GetWorklistCount(EntityRef worklistRef)
        {
            IWorklist worklist = this.PersistenceContext.Load<Worklist>(worklistRef);
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
