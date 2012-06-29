#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.Configuration;
using ClearCanvas.ImageViewer.StudyManagement.Core.Configuration;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.ServiceProviders
{
    [ExtensionOf(typeof (ServiceProviderExtensionPoint))]
    internal class SystemConfigurationServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof (ISystemConfigurationSettingsStore))
                return null;

            return new SystemConfigurationSettingsStore();
        }

        #endregion
    }
}
