#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Castle.DynamicProxy;


namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Advice class responsible for honouring <see cref="ReadOperationAttribute"/> and <see cref="UpdateOperationAttribute"/>.
    /// </summary>
    public class PersistenceContextAdvice : ServiceOperationAdvice, IInterceptor
    {
		public PersistenceContextAdvice()
        {
        }

        public object Intercept(IInvocation invocation, params object[] args)
        {
            object retval;
            ServiceOperationAttribute a = GetServiceOperationAttribute(invocation);
            if (a != null)
            {
                // persistence context required
                using (PersistenceScope scope = a.CreatePersistenceScope())
                {
                    // configure change-set auditing
                    ConfigureAuditing(PersistenceScope.CurrentContext, a, invocation);

                    // proceed with invocation
                    retval = invocation.Proceed(args);

                    // auto-commit transaction
                    scope.Complete();
                }
            }
            else
            {
                // no persistence context required
                retval = invocation.Proceed(args);
            }

            return retval;
        }

        private void ConfigureAuditing(IPersistenceContext context, ServiceOperationAttribute attribute, IInvocation invocation)
        {
            // if this is a read-context, there is no change set to audit
            IUpdateContext uctx = context as IUpdateContext;
            if (uctx == null)
                return;

            // if this operation is marked as not auditable, then explicitly
            // disable the change set recorder
            if (attribute.ChangeSetAuditable == false)
            {
                uctx.ChangeSetRecorder = null;
                return;
            }

            // if the current context has a change-set recorder installed
            // ensure that the ChangeSetRecorder.OperationName property is set appropriately
            // if the name is already set (by an outer service layer), don't change it
            if (uctx.ChangeSetRecorder != null && string.IsNullOrEmpty(uctx.ChangeSetRecorder.OperationName))
            {
                uctx.ChangeSetRecorder.OperationName = string.Format("{0}.{1}",
                    invocation.InvocationTarget.GetType().FullName, invocation.Method.Name);
            }
        }
    }
}
