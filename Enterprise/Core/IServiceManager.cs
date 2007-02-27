using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Defines the interface to a service manager, which allows the application to obtain
    /// the service that implements a specified interface.
    /// </summary>
    public interface IServiceManager
    {
        /// <summary>
        /// Obtains an instance of the service that implements the specified interface.
        /// </summary>
        /// <typeparam name="TServiceInterface"></typeparam>
        /// <returns></returns>
        TServiceInterface GetService<TServiceInterface>();

        IServiceLayer GetService(Type serviceContract);


        ICollection<Type> ListServices();

        bool HasService(Type serviceContract);
    }
}
