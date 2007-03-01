using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines the interface to a service factory, which instantiates a service based on a specified
    /// contract.
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// Obtains an instance of the service that implements the specified interface.
        /// </summary>
        /// <typeparam name="TServiceInterface"></typeparam>
        /// <returns></returns>
        TServiceInterface GetService<TServiceInterface>();

        object GetService(Type serviceContract);


        ICollection<Type> ListServices();

        bool HasService(Type serviceContract);
    }
}
