using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public abstract class StudyStore : IStudyStoreQuery
    {
        static StudyStore()
        {
            try
            {
                var service = Platform.GetService<IStudyStoreQuery>();
                IsSupported = service != null;
                var disposable = service as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            catch (NotSupportedException)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Study Store is not supported.");
            }
            catch (Exception e)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, e, "Study Store is not supported.");
            }
        }

        public static bool IsSupported { get; private set; }

        public abstract GetStudyCountResult GetStudyCount(GetStudyCountRequest request);
        public abstract GetStudyEntriesResult GetStudyEntries(GetStudyEntriesRequest request);
        public abstract GetSeriesEntriesResult GetSeriesEntries(GetSeriesEntriesRequest request);
        public abstract GetImageEntriesResult GetImageEntries(GetImageEntriesRequest request);

        public static StorageConfiguration GetConfiguration()
        {
            StorageConfiguration configuration = null;
                Platform.GetService<IStorageConfiguration>(
                    s => configuration = s.GetConfiguration(new GetStorageConfigurationRequest()).Configuration);
            return configuration;
        }

        public static void UpdateConfiguration(string fileStoreDirectory, long? minimumFreeDiskSpaceBytes)
        {
            Platform.GetService<IStorageConfiguration>(
                s => s.UpdateConfiguration(new UpdateStorageConfigurationRequest
                {
                    Configuration = new StorageConfiguration
                    {
                        FileStoreDirectory = fileStoreDirectory,
                        MinimumFreeSpaceBytes = minimumFreeDiskSpaceBytes 
                    }}));
        }

        public static string FileStoreDirectory
        {
            get { return GetConfiguration().FileStoreDirectory; }
        }

        public static long? MinimumFreeSpaceBytes
        {
            get { return GetConfiguration().MinimumFreeSpaceBytes; }
        }
    }
}
