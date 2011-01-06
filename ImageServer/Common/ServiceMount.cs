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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Creates or stop web services.
    /// </summary>
    public class ServiceMount : ClearCanvas.Enterprise.Core.ServiceModel.ServiceMount
    {
        public ServiceMount(Uri baseAddress, IServiceHostConfiguration configuration) 
            : base(baseAddress, configuration)
        {
        }

        public ServiceMount(Uri baseAddress, string serviceHostConfigurationClass) 
            : base(baseAddress, serviceHostConfigurationClass)
        {
        }

        protected override void ApplyInterceptors(IList<Castle.Core.Interceptor.IInterceptor> interceptors)
        {
            // NO-OP
        }
    }
}