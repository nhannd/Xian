using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common;
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
        public const double AutoMinimumFreeSpace = -1;

        private DriveInfo _fileStoreDrive;
        private string _fileStoreDirectory;
        private double _minimumFreeSpacePercent = AutoMinimumFreeSpace;

        [DataMember(IsRequired = true)]
        public string FileStoreDirectory
        {
            get { return _fileStoreDirectory; }
            set
            {
                _fileStoreDirectory = value;
                _fileStoreDrive = null;
            }
        }

        public bool AutoCalculateMinimumFreeSpacePercent { get { return MinimumFreeSpacePercent < 0; } }

        [DataMember(IsRequired = true)]
        public double MinimumFreeSpacePercent
        {
            get { return _minimumFreeSpacePercent; }
            set
            {
                if (value < 0)
                {
                    _minimumFreeSpacePercent = AutoMinimumFreeSpace;
                    return;
                }
                
                if (value > 100)
                    throw new ArgumentException("Value must be between 0 and 100.", "MinimumFreeSpacePercent");

                _minimumFreeSpacePercent = value;
            }
        }

        public long MinimumFreeSpaceBytes
        {
            get
            {
                Platform.CheckMemberIsSet(FileStoreDrive, "FileStoreDrive");
                if (MinimumFreeSpacePercent < 0)
                    throw new InvalidOperationException("MinimumFreeSpacePercent must be set.");

                return (long)(FileStoreDrive.TotalSize * MinimumFreeSpacePercent / 100);
            }
            set { MinimumFreeSpacePercent = (double)value / FileStoreDrive.TotalSize * 100; }
        }

        public double MaximumUsedSpacePercent
        {
            get
            {
                if (MinimumFreeSpacePercent < 0)
                    throw new InvalidOperationException("MinimumFreeSpacePercent must be set.");

                return 100F - MinimumFreeSpacePercent;
            }
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentException("Value must be between 0 and 100.", "MaximumUsedSpacePercent");

                MinimumFreeSpacePercent = 100 - value;
            }
        }

        public long MaximumUsedSpaceBytes
        {
            get
            {
                Platform.CheckMemberIsSet(FileStoreDrive, "FileStoreDrive");
                return FileStoreDrive.TotalSize - MinimumFreeSpaceBytes;
            }
            set { MinimumFreeSpaceBytes = FileStoreDrive.TotalSize - value; }
        }

        public bool IsMaximumUsedSpaceExceeded
        {
            get { return FileStoreDrive.TotalUsedSpacePercent > MaximumUsedSpacePercent; }
        }

        public string FileStoreDriveName
        {
            get
            {
                return !String.IsNullOrEmpty(FileStoreDirectory)
                    ? System.IO.Path.GetPathRoot(FileStoreDirectory)
                    : null;
            }
        }

        public DriveInfo FileStoreDrive
        {
            get
            {
                if (String.IsNullOrEmpty(FileStoreDirectory))
                    return null;

                return _fileStoreDrive ?? (_fileStoreDrive = new DriveInfo(FileStoreDriveName));
            }
            internal set { _fileStoreDrive = value; }
        }

        public override int GetHashCode()
        {
            int hash = 0x2453671;

            if (FileStoreDirectory != null)
                hash ^= FileStoreDirectory.GetHashCode();
            
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
                   MinimumFreeSpacePercent.Equals(other.MinimumFreeSpacePercent);
        }

        #endregion
    }
}
