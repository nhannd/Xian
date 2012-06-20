using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class StudyEntryData : DataContractBase, IStudyEntryData
    {
        [DataMember(IsRequired = true)]
        public DateTime? StoreTime { get; set; }
        
        [DataMember(IsRequired = true)]
        public DateTime? DeleteTime { get; set; }

        [DataMember(IsRequired = false)]
        [DicomField(DicomTags.SourceApplicationEntityTitle, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        public string[] SourceAETitlesInStudy { get; set; }

        [DataMember(IsRequired = false)]
        [DicomField(DicomTags.StationName, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        public string[] StationNamesInStudy { get; set; }
        
        [DataMember(IsRequired = false)]
        [DicomField(DicomTags.InstitutionName, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        public string[] InstitutionNamesInStudy { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class SeriesEntryData : DataContractBase, ISeriesEntryData
    {
        [DataMember(IsRequired = true)]
        public DateTime? ScheduledDeleteTime { get; set; }

        [DataMember(IsRequired = false)]
        public string[] SourceAETitlesInSeries { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class ImageEntryData : DataContractBase, IImageEntryData
    {
        [DataMember(IsRequired = false)]
        public string SourceAETitle { get; set; }
    }

    public interface IStudyEntryData
    {
        [DataMember(IsRequired = true)]
        DateTime? StoreTime { get; set; }

        [DataMember(IsRequired = true)]
        DateTime? DeleteTime { get; set; }

        [DataMember(IsRequired = false)]
        string[] SourceAETitlesInStudy { get; set; }

        [DataMember(IsRequired = false)]
        string[] StationNamesInStudy { get; set; }

        [DataMember(IsRequired = false)]
        string[] InstitutionNamesInStudy { get; set; }
    }

    public interface ISeriesEntryData
    {
        [DataMember(IsRequired = true)]
        DateTime? ScheduledDeleteTime { get; set; }

        [DataMember(IsRequired = false)]
        string[] SourceAETitlesInSeries { get; set; }
    }

    public interface IImageEntryData
    {
        [DataMember(IsRequired = false)]
        string SourceAETitle { get; set; }
    }
}