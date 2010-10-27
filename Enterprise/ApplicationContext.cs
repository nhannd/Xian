#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Provides convenience methods for the application to obtain references
    /// to needed resources without having to understand how those resources
    /// are acquired or how they are associated with the underlying session, etc.
    /// </summary>
    public static class ApplicationContext
    {
        /// <summary>
        /// Obtains the service that implements the specified interface.
        /// </summary>
        /// <typeparam name="TServiceInterface">The interface that the service must implement</typeparam>
        /// <returns>An object that implements the specified interface</returns>
        public static TServiceInterface GetService<TServiceInterface>()
        {
            return Core.ServiceManager.GetService<TServiceInterface>();
        }
    }
}
