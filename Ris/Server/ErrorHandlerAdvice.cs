using System;
using System.Collections.Generic;
using System.Text;
using AopAlliance.Intercept;
using System.ServiceModel;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

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
            catch (Exception error)
            {
                // get the service contract
                ServiceImplementsContractAttribute serviceContractAttr = CollectionUtils.FirstElement<ServiceImplementsContractAttribute>(
                    invocation.This.GetType().GetCustomAttributes(typeof(ServiceImplementsContractAttribute), false));
                
                if (serviceContractAttr == null)
                    throw;  // this should never happen, but if it does, there's nothing we can do

                // get the operation contract method
                MethodInfo method = serviceContractAttr.ServiceContract.GetMethod(invocation.Method.Name);

                // find a fault contract for this exception type
                object[] faultContracts = method.GetCustomAttributes(typeof(FaultContractAttribute), true);
                FaultContractAttribute faultContract = CollectionUtils.SelectFirst<FaultContractAttribute>(
                    faultContracts,
                    delegate(FaultContractAttribute a)
                    {
                        return a.DetailType.Equals(error.GetType());
                    });

                if (faultContract != null)
                {
                    // from Juval Lowey
                    Type faultUnboundedType = typeof(FaultException<>);
                    Type faultBoundedType = faultUnboundedType.MakeGenericType(error.GetType());
                    //Exception newException = (Exception)Activator.CreateInstance(error.GetType(), error.Message);
                    FaultException faultException = (FaultException)Activator.CreateInstance(faultBoundedType, error, new FaultReason(error.Message));

                    throw faultException;
                }
                else
                {
                    // no fault contract for this type, so just pass the exception on
                    throw;
                }
            }
        }

        #endregion
    }
}
