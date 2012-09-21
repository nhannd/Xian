#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;


namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines the interface to a service factory, which instantiates a service based on a specified
    /// contract.
    /// </summary>
    public interface IServiceFactory
    {
		/// <summary>
		/// Gets the list of interceptors that will be applied to service instances created by this factory.
		/// </summary>
		/// <remarks>
		/// Interceptors must be thread-safe, since the same interceptor instance is applied to every service object
		/// that is created.
		/// </remarks>
		IList<IInterceptor> Interceptors { get; }

        /// <summary>
        /// Obtains an instance of the service that implements the specified contract.
        /// </summary>
        /// <typeparam name="TServiceContract"></typeparam>
        /// <returns></returns>
        TServiceContract GetService<TServiceContract>();

        /// <summary>
        /// Obtains an instance of the service that implements the specified contract.
        /// </summary>
        /// <returns></returns>
        object GetService(Type serviceContract);

        /// <summary>
        /// Lists the service contracts supported by this factory.
        /// </summary>
        /// <returns></returns>
        ICollection<Type> ListServiceContracts();

        /// <summary>
        /// Lists the service classes that provide implementations of the contracts supported by this factory.
        /// </summary>
        /// <returns></returns>
        ICollection<Type> ListServiceClasses();

        /// <summary>
        /// Tests if this factory supports a service with the specified contract.
        /// </summary>
        /// <param name="serviceContract"></param>
        /// <returns></returns>
        bool HasService(Type serviceContract);
    }
}
