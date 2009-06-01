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
using Castle.DynamicProxy;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;


namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Advice class responsible for honouring <see cref="AuditAttribute"/>s applied to service operation methods.
    /// </summary>
    public class AuditAdvice : ServiceOperationAdvice, IInterceptor
    {
		public AuditAdvice()
        {
        }

        #region IInterceptor Members

        public object Intercept(IInvocation invocation, params object[] args)
        {
            object retval = null;
            Exception exception = null;
            try
            {
                retval = invocation.Proceed(args);
                return retval;
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                List<AuditAttribute> auditAttrs = AttributeUtils.GetAttributes<AuditAttribute>(invocation.MethodInvocationTarget, true);
                if (auditAttrs.Count > 0)
                {
                    // inherit the current persistence scope, which should still be valid, or optionally create a new one
                    using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.Required))
                    {
                        string operationName =
                            string.Format("{0}.{1}", invocation.InvocationTarget.GetType().FullName, invocation.Method.Name);

                        ServiceOperationInvocationInfo info = new ServiceOperationInvocationInfo(
                            operationName,
                            invocation.InvocationTarget.GetType(),
                            invocation.MethodInvocationTarget,
                            args,
                            retval,
                            exception);

                        // multiple audit recorders may be specified for a given service operation
                        foreach (AuditAttribute attr in auditAttrs)
                        {
                            try
                            {
                                Audit(attr, info);
                            }
                            catch (Exception e)
                            {
                                // audit operation failed - this is low-level, so we log directly to log file
                                Platform.Log(LogLevel.Error, e);
                            }
                        }

                        scope.Complete();
                    }
                }
            }
        }

        private void Audit(AuditAttribute attr, ServiceOperationInvocationInfo info)
        {
            // create an instance of the specified recorder class
            IServiceOperationRecorder recorder = (IServiceOperationRecorder) Activator.CreateInstance(attr.RecorderClass);

            // write to the audit log
			AuditLog log = new AuditLog(null, recorder.Category);
			recorder.WriteLogEntry(info, log);
        }

        #endregion
    }
}
