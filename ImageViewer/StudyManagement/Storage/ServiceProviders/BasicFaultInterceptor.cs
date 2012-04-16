using System;
using System.ServiceModel;
using Castle.Core.Interceptor;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
    internal class BasicFaultInterceptor : IInterceptor
    {
        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch(FaultException)
            {
                throw;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                throw new FaultException();
            }
        }

        #endregion
    }
}
