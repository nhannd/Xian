using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using StorageConfigurationContract = ClearCanvas.ImageViewer.Common.StudyManagement.StorageConfiguration;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.ServiceProviders
{
    [ExtensionOf(typeof (ServiceProviderExtensionPoint))]
    internal class StorageConfigurationServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof (IStorageConfiguration))
                return null;

            return new StorageConfigurationProxy();
        }

        #endregion
    }

    internal class StorageConfigurationProxy : IStorageConfiguration
    {
        private readonly IStorageConfiguration _real;

        public StorageConfigurationProxy()
        {
            _real = new StorageConfiguration();
        }

        public GetStorageConfigurationResult GetConfiguration(GetStorageConfigurationRequest request)
        {
            return ServiceProxyHelper.Call(_real.GetConfiguration, request);
        }

        public UpdateStorageConfigurationResult UpdateConfiguration(UpdateStorageConfigurationRequest request)
        {
            return ServiceProxyHelper.Call(_real.UpdateConfiguration, request);
        }
    }

    internal class StorageConfiguration : IStorageConfiguration
    {
        //Note: the installer is supposed to set these defaults. These are the bottom of the barrel, last-ditch defaults.

        #region Defaults

        private string DefaultFileStoreLocation
        {
            get { return Path.Combine(Platform.ApplicationDataDirectory, "filestore"); }
        }

        #endregion

        #region Implementation of IStorageConfiguration

        public GetStorageConfigurationResult GetConfiguration(GetStorageConfigurationRequest request)
        {
            StorageConfigurationContract configuration;
            using (var context = new DataAccessContext())
            {
                var broker = context.GetConfigurationBroker();
                configuration = broker.GetDataContractValue<StorageConfigurationContract>()
                                ?? new StorageConfigurationContract();
            }

            Complete(configuration);
            return new GetStorageConfigurationResult {Configuration = configuration.Clone()};
        }

        public UpdateStorageConfigurationResult UpdateConfiguration(UpdateStorageConfigurationRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckForNullReference(request.Configuration, "Configuration");
            Platform.CheckForEmptyString(request.Configuration.FileStoreDirectory, "FileStoreDirectory");
            //Platform.CheckTrue(request.Configuration.MinimumFreeSpaceBytes.HasValue, "MinimumFreeSpaceBytes");

            var newConfiguration = request.Configuration.Clone();

            //Trim before saving.
            newConfiguration.FileStoreDirectory = request.Configuration.FileStoreDirectory.Trim();
            Complete(newConfiguration);

            using (var context = new DataAccessContext())
            {
                context.GetConfigurationBroker().SetDataContractValue(newConfiguration);
                context.Commit();
            }

            return new UpdateStorageConfigurationResult();
        }

        #endregion

        private void Complete(StorageConfigurationContract configuration)
        {
            if (String.IsNullOrEmpty(configuration.FileStoreDirectory))
                configuration.FileStoreDirectory = DefaultFileStoreLocation;

            if (configuration.AutoCalculateMinimumFreeSpacePercent)
                configuration.MinimumFreeSpaceBytes = ComputeMinimumFreeSpaceBytes(configuration.FileStoreDirectory);

            if (configuration.DefaultDeletionRule == null)
                configuration.DefaultDeletionRule = new StorageConfigurationContract.DeletionRule
                                                        {
                                                            Enabled = false,
                                                            TimeUnit = TimeUnit.Weeks,
                                                            TimeValue = 1
                                                        };
        }

        private long ComputeMinimumFreeSpaceBytes(string fileStoreDirectory)
        {
            fileStoreDirectory = fileStoreDirectory ?? DefaultFileStoreLocation;
            var driveInfo = new DriveInfo(fileStoreDirectory[0].ToString(System.Globalization.CultureInfo.InvariantCulture));
            return (long)(driveInfo.TotalSize*0.05);
        }
    }
}
