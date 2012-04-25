using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using StorageConfigurationContract = ClearCanvas.ImageViewer.Common.StudyManagement.StorageConfiguration;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
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
        private static readonly string _configurationKey = typeof (StorageConfigurationContract).FullName;

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
            var cachedValue = Cache<StorageConfigurationContract>.CachedValue;
            if (cachedValue != null)
                return new GetStorageConfigurationResult {Configuration = cachedValue.Clone() };

            StorageConfigurationContract configuration;
            using (var context = new DataAccessContext())
            {
                var broker = context.GetConfigurationBroker();
                configuration = broker.GetDataContractValue<StorageConfigurationContract>(_configurationKey)
                                ?? new StorageConfigurationContract();
            }

            if (String.IsNullOrEmpty(configuration.FileStoreDirectory))
                configuration.FileStoreDirectory = DefaultFileStoreLocation;

            if (!configuration.MinimumFreeSpaceBytes.HasValue)
                configuration.MinimumFreeSpaceBytes = ComputeMinimumFreeSpaceBytes(configuration.FileStoreDirectory);

            Cache<StorageConfigurationContract>.CachedValue = configuration.Clone();
            return new GetStorageConfigurationResult {Configuration = configuration.Clone()};
        }

        public UpdateStorageConfigurationResult UpdateConfiguration(UpdateStorageConfigurationRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckForNullReference(request.Configuration, "Configuration");
            Platform.CheckForEmptyString(request.Configuration.FileStoreDirectory, "FileStoreDirectory");
            //Platform.CheckTrue(request.Configuration.MinimumFreeSpaceBytes.HasValue, "MinimumFreeSpaceBytes");

            //Trim before saving.
            request.Configuration.FileStoreDirectory = request.Configuration.FileStoreDirectory.Trim();

            using (var context = new DataAccessContext())
            {
                context.GetConfigurationBroker().SetDataContractValue(_configurationKey, request.Configuration);
                context.Commit();

                //Make a copy because the one in the request is a reference object that the caller could change afterwards.
                Cache<StorageConfigurationContract>.CachedValue = request.Configuration.Clone();
            }

            return new UpdateStorageConfigurationResult();
        }

        #endregion

        private long ComputeMinimumFreeSpaceBytes(string fileStoreDirectory)
        {
            fileStoreDirectory = fileStoreDirectory ?? DefaultFileStoreLocation;
            var driveInfo = new DriveInfo(fileStoreDirectory[0].ToString(System.Globalization.CultureInfo.InvariantCulture));
            return (long)(driveInfo.TotalSize*0.05);
        }
    }

    internal static class StorageConfigurationExtensions
    {
        public static StorageConfigurationContract Clone(this StorageConfigurationContract storageConfiguration)
        {
            return new StorageConfigurationContract
                       {
                           FileStoreDirectory = storageConfiguration.FileStoreDirectory,
                           MinimumFreeSpaceBytes = storageConfiguration.MinimumFreeSpaceBytes
                       };
        }
    }
}
