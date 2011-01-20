#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemStudyProcess
{
    /// <summary>
    /// Plugin for creating processors for 'FilesystemStudyProcess' <see cref="Model.ServiceLock"/> items.
    /// </summary>
    [ExtensionOf(typeof(ServiceLockFactoryExtensionPoint))]
    class FilesystemStudyProcessFactoryExtension : IServiceLockProcessorFactory
    {
        #region IServiceLockProcessorFactory Members

        public ServiceLockTypeEnum GetServiceLockType()
        {
            return ServiceLockTypeEnum.FilesystemStudyProcess;
        }

        public IServiceLockItemProcessor GetItemProcessor()
        {
            return new FilesystemStudyProcessItemProcessor();
        }

        #endregion
    }
}
