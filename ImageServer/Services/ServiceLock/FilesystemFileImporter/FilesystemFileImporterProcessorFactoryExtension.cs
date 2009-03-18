using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter
{
    /// <summary>
    /// Plugin for creating processors for 'FilesystemStudyProcess' <see cref="Model.ServiceLock"/> items.
    /// </summary>
    [ExtensionOf(typeof(ServiceLockFactoryExtensionPoint))]
    class FilesystemFileImporterProcessorFactoryExtension : IServiceLockProcessorFactory
    {
        #region IServiceLockProcessorFactory Members

        public ServiceLockTypeEnum GetServiceLockType()
        {
            return ServiceLockTypeEnum.ImportFiles;
        }

        public IServiceLockItemProcessor GetItemProcessor()
        {
            return new FilesystemFileImporterProcessor();
        }

        #endregion
    }
}
