using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom.ServiceModel.Query;
using System;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public static class StudyManagementNamespace
    {
        public const string Value = ImageViewerNamespace.Value + "/studyManagement";
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStudyCountRequest : DataContractBase
    {
        [DataMember(IsRequired = false)]
        public StudyEntry Criteria { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStudyCountResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public int StudyCount { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStudyEntriesRequest : DataContractBase
    {
        [DataMember(IsRequired = false)]
        public StudyEntry Criteria { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStudyEntriesResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public IList<StudyEntry> StudyEntries  { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetSeriesEntriesRequest : DataContractBase
    {
        [DataMember(IsRequired = false)]
        public SeriesEntry Criteria { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetSeriesEntriesResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public IList<SeriesEntry> SeriesEntries { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetImageEntriesRequest : DataContractBase
    {
        [DataMember(IsRequired = false)]
        public ImageEntry Criteria { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetImageEntriesResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public IList<ImageEntry> ImageEntries { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public abstract class StoreEntry : DataContractBase
    {
    }

    public interface IStoreEntry
    {
        Identifier Identifier { get; }
    }

    public interface IStoreEntry<out T> where T : Identifier
    {
        T Identifier { get; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class StudyEntry : StoreEntry, IStoreEntry, IStoreEntry<StudyRootStudyIdentifier>
    {
        [DataMember(IsRequired = true)]
        public StudyRootStudyIdentifier Study { get; set; }

        [DataMember(IsRequired = false)]
        public StudyEntryData Data { get; set; }

        #region IStoreEntry Members

        Identifier IStoreEntry.Identifier
        {
            get { return Study; }
        }

        #endregion

        #region IStoreEntry<StudyRootStudyIdentifier> Members

        StudyRootStudyIdentifier IStoreEntry<StudyRootStudyIdentifier>.Identifier
        {
            get { return Study; }
        }

        #endregion
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class SeriesEntry : StoreEntry, IStoreEntry, IStoreEntry<SeriesIdentifier>
    {
        [DataMember(IsRequired = true)]
        public SeriesIdentifier Series { get; set; }

        [DataMember(IsRequired = false)]
        public SeriesEntryData Data { get; set; }

        #region IStoreEntry Members

        Identifier IStoreEntry.Identifier
        {
            get { return Series; }
        }

        #endregion

        #region IStoreEntry<SeriesIdentifier> Members

        SeriesIdentifier IStoreEntry<SeriesIdentifier>.Identifier
        {
            get { return Series; }
        }

        #endregion
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class ImageEntry : StoreEntry, IStoreEntry, IStoreEntry<ImageIdentifier>
    {
        [DataMember(IsRequired = true)]
        public ImageIdentifier Image { get; set; }

        [DataMember(IsRequired = false)]
        public ImageEntryData Data { get; set; }

        #region IStoreEntry Members

        Identifier IStoreEntry.Identifier
        {
            get { return Image; }
        }

        #endregion

        #region IStoreEntry<ImageIdentifier> Members

        ImageIdentifier IStoreEntry<ImageIdentifier>.Identifier
        {
            get { return Image; }
        }

        #endregion
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStorageConfigurationResult
    {
        [DataMember(IsRequired = true)]
        public StorageConfiguration Configuration { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStorageConfigurationRequest
    { }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class UpdateStorageConfigurationResult
    {
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class UpdateStorageConfigurationRequest
    {
        [DataMember(IsRequired = true)]
        public StorageConfiguration Configuration { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class StorageConfiguration : IEquatable<StorageConfiguration>
    {
        [DataMember(IsRequired = false)]
        public string FileStoreDirectory { get; set; }

        [DataMember(IsRequired = false)]
        public long? MinimumFreeSpaceBytes { get; set; }

        public float MinimumFreeSpacePercent
        {
            get
            {
                var minimumFreeSpaceBytes = MinimumFreeSpaceBytes;
                if (!minimumFreeSpaceBytes.HasValue)
                    return 0;

                double ratio = (double)minimumFreeSpaceBytes.Value / FileStoreDrive.TotalSize;
                return (float) ratio * 100;
            }
        }

        public float MaximumUsedSpacePercent
        {
            get
            {
                var maximumUsedSpaceBytes = MaximumUsedSpaceBytes;
                if (!maximumUsedSpaceBytes.HasValue)
                    return 100;

                double ratio = (double)maximumUsedSpaceBytes.Value / FileStoreDrive.TotalSize;
                return (float)ratio * 100;
            }
        }

        public long? MaximumUsedSpaceBytes
        {
            get 
            {
                var drive = FileStoreDrive;
                if (drive == null)
                    return null;

                if (!MinimumFreeSpaceBytes.HasValue)
                    return drive.TotalSize;

                return drive.TotalSize - MinimumFreeSpaceBytes.Value;
            }
        }

        public string FileStoreDriveName
        {
            get
            {
                return !String.IsNullOrEmpty(FileStoreDirectory)
                    ? Path.GetPathRoot(FileStoreDirectory)
                    : null;
            }
        }

        public DriveInfo FileStoreDrive
        {
            get
            {
                return !String.IsNullOrEmpty(FileStoreDirectory) 
                    ? new DriveInfo(FileStoreDriveName)
                    : null;
            }
        }

        public override int GetHashCode()
        {
            int hash = 0x2453671;

            if (FileStoreDirectory != null)
                hash ^= FileStoreDirectory.GetHashCode();
            if (MinimumFreeSpaceBytes != null)
                hash ^= MinimumFreeSpaceBytes.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            var storageConfiguration = obj as StorageConfiguration;
            return storageConfiguration != null && Equals(storageConfiguration);
        }

        #region IEquatable<StorageConfiguration> Members

        public bool Equals(StorageConfiguration other)
        {
            return FileStoreDirectory == other.FileStoreDirectory &&
                   MinimumFreeSpaceBytes == other.MinimumFreeSpaceBytes;
        }

        #endregion
    }
}
