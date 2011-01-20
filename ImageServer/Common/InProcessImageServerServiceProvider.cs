#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Common
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    public class InProcessImageServerServiceProvider : IServiceProvider
    {
        private readonly IServiceFactory _serviceFactory;

        public InProcessImageServerServiceProvider()
        {
            _serviceFactory = new ServiceFactory(new ApplicationServiceExtensionPoint());
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (_serviceFactory.HasService(serviceType))
            {
                return _serviceFactory.GetService(serviceType);
            }
            else
            {
                return null;    // as per MSDN
            }
        }

        #endregion
    }
}