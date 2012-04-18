using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom.ServiceModel.Query;

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
}
