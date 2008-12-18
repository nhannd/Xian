#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.ServiceModel;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using Castle.DynamicProxy;

namespace ClearCanvas.Ris.Server
{
    class ExceptionPromotionAdvice : IInterceptor
    {
        #region IMethodInterceptor Members

        public object Intercept(IInvocation invocation, params object[] args)
        {
            try
            {
                return invocation.Proceed(args);
            }
            catch (Exception e)
            {
                // translate exception if necessary
                Exception translated = TranslateException(e);

                // try promoting the exception to a fault
                // if successful, throw the fault exception, otherwise throw the normal exception
                FaultException fault = PromoteExceptionToFault(translated, invocation.InvocationTarget, invocation.Method.Name);
                if (fault != null)
                    throw fault;
                else
                    throw translated;
            }
        }

        #endregion

        /// <summary>
        /// Translate certain "special" classes of exception to classes that can be passed through the service boundary.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private Exception TranslateException(Exception e)
        {
            // special handling of EntityVersionException
            // assume all such exceptions occured because of concurrent modifications
            // wrap in ConcurrentModificationException will be used in the fault contract
            if (e is EntityVersionException)
            {
                return new ConcurrentModificationException(e.Message);
            }

            // special handling of EntityValidationException
            // convert to RequestValidationException
            if (e is EntityValidationException)
            {
                return new RequestValidationException(e.Message);
            }

            // no translation
            return e;
        }

        /// <summary>
        /// Promotes the exception to a fault, if the exception type has a corresponding fault contract.
        /// Otherwise returns null.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="serviceInstance"></param>
        /// <param name="operationMethodName"></param>
        /// <returns></returns>
        private FaultException PromoteExceptionToFault(Exception e, object serviceInstance, string operationMethodName)
        {
            // get the service contract
            ServiceImplementsContractAttribute serviceContractAttr = CollectionUtils.FirstElement<ServiceImplementsContractAttribute>(
                serviceInstance.GetType().GetCustomAttributes(typeof(ServiceImplementsContractAttribute), false));

            if (serviceContractAttr == null)
                return null;  // this should never happen, but if it does, there's nothing we can do

            // get the operation contract method
            MethodInfo method = serviceContractAttr.ServiceContract.GetMethod(operationMethodName);

            // find a fault contract for this exception type
            object[] faultContracts = method.GetCustomAttributes(typeof(FaultContractAttribute), true);
            FaultContractAttribute faultContract = CollectionUtils.SelectFirst<FaultContractAttribute>(
                faultContracts,
                delegate(FaultContractAttribute a)
                {
                    return a.DetailType.Equals(e.GetType());
                });

            if (faultContract != null)
            {
                // from Juval Lowey
                Type faultUnboundedType = typeof(FaultException<>);
                Type faultBoundedType = faultUnboundedType.MakeGenericType(e.GetType());
                //Exception newException = (Exception)Activator.CreateInstance(error.GetType(), error.Message);
                FaultException faultException = (FaultException)Activator.CreateInstance(faultBoundedType, e, new FaultReason(e.Message));

                return faultException;
            }

            return null;    // no fault contract for this exception
        }
    }
}
