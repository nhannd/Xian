using System;
using System.Collections.Generic;
using System.Text;
using AopAlliance.Intercept;
using System.ServiceModel;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Server
{
    class ErrorHandlerAdvice : IMethodInterceptor
    {
        #region IMethodInterceptor Members

        public object Invoke(IMethodInvocation invocation)
        {
            try
            {
                return invocation.Proceed();
            }
            catch (Exception e)
            {
                // translate exception if necessary
                Exception translated = TranslateException(e);

                // try promoting the exception to a fault
                // if successful, throw the fault exception, otherwise throw the normal exception
                FaultException fault = PromoteExceptionToFault(translated, invocation.This, invocation.Method.Name);
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
                return RequestValidationException.FromTestResultReasons(e.Message, (e as EntityValidationException).Reasons);
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
