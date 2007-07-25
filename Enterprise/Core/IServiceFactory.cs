using System;
using System.Collections.Generic;
using System.Text;
using AopAlliance.Intercept;

namespace ClearCanvas.Enterprise.Core
{
    public class ServiceCreationEventArgs : EventArgs
    {
        private IList<IMethodInterceptor> _interceptors;

        public ServiceCreationEventArgs(IList<IMethodInterceptor> interceptors)
        {
            _interceptors = interceptors;
        }

        public IList<IMethodInterceptor> ServiceOperationInterceptors
        {
            get { return _interceptors; }
        }
    }

    /// <summary>
    /// Defines the interface to a service factory, which instantiates a service based on a specified
    /// contract.
    /// </summary>
    public interface IServiceFactory
    {
        event EventHandler<ServiceCreationEventArgs> ServiceCreation;

        /// <summary>
        /// Obtains an instance of the service that implements the specified interface.
        /// </summary>
        /// <typeparam name="TServiceInterface"></typeparam>
        /// <returns></returns>
        TServiceInterface GetService<TServiceInterface>();

        object GetService(Type serviceContract);


        ICollection<Type> ListServiceClasses();
        ICollection<Type> ListServiceContracts();

        bool HasService(Type serviceContract);
    }
}
