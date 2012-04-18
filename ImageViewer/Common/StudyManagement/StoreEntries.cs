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
        [DicomField(DicomTags.TransferSyntaxUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        public string[] TransferSyntaxesInStudy { get; set; }
        
        [DataMember(IsRequired = false)]
        [DicomField(DicomTags.SourceApplicationEntityTitle, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        public string[] SourceAETitlesInStudy { get; set; }

        [DataMember(IsRequired = false)]
        [DicomField(DicomTags.StationName, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        public string[] StationNamesInStudy { get; set; }
        
        [DataMember(IsRequired = false)]
        [DicomField(DicomTags.InstitutionName, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        public string[] InstitutionNamesInStudy { get; set; }

        [DataMember(IsRequired = false)]
        [DicomField(DicomTags.PhotometricInterpretation, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        public string[] PhotometricInterpretationsInStudy { get; set; }
        
        [DataMember(IsRequired = false)]
        //[DicomField(DicomTags.BitsStored, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        public int[] BitsStoredInStudy { get; set; }

        [DataMember(IsRequired = false)]
        //[DicomField(DicomTags.BitsAllocated, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
        public int[] BitsAllocatedInStudy { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class SeriesEntryData : DataContractBase, ISeriesEntryData
    {
        [DataMember(IsRequired = true)]
        public DateTime? DeleteTime { get; set; }

        [DataMember(IsRequired = false)]
        public string[] TransferSyntaxesInSeries { get; set; }

        [DataMember(IsRequired = false)]
        public string[] SourceAETitlesInSeries { get; set; }

        [DataMember(IsRequired = false)]
        public string[] PhotometricInterpretationsInSeries { get; set; }
        
        [DataMember(IsRequired = false)]        
        public int[] BitsStoredInSeries { get; set; }
        
        [DataMember(IsRequired = false)]
        public int[] BitsAllocatedInSeries { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class ImageEntryData : DataContractBase, IImageEntryData
    {
        [DataMember(IsRequired = false)]
        public string TransferSyntax { get; set; }

        [DataMember(IsRequired = false)]
        public string SourceAETitle { get; set; }
        
        [DataMember(IsRequired = false)]
        public string PhotometricInterpretation { get; set; }
        
        [DataMember(IsRequired = false)]
        public int? BitsStored { get; set; }
        
        [DataMember(IsRequired = false)]
        public int? BitsAllocated { get; set; }
    }

    public interface IStudyEntryData
    {
        [DataMember(IsRequired = true)]
        DateTime? StoreTime { get; set; }

        [DataMember(IsRequired = true)]
        DateTime? DeleteTime { get; set; }

        [DataMember(IsRequired = false)]
        string[] TransferSyntaxesInStudy { get; set; }

        [DataMember(IsRequired = false)]
        string[] SourceAETitlesInStudy { get; set; }

        [DataMember(IsRequired = false)]
        string[] StationNamesInStudy { get; set; }

        [DataMember(IsRequired = false)]
        string[] InstitutionNamesInStudy { get; set; }

        [DataMember(IsRequired = false)]
        string[] PhotometricInterpretationsInStudy { get; set; }

        [DataMember(IsRequired = false)]
        int[] BitsStoredInStudy { get; set; }

        [DataMember(IsRequired = false)]
        int[] BitsAllocatedInStudy { get; set; }
    }

    public interface ISeriesEntryData
    {
        [DataMember(IsRequired = true)]
        DateTime? DeleteTime { get; set; }

        [DataMember(IsRequired = false)]
        string[] TransferSyntaxesInSeries { get; set; }

        [DataMember(IsRequired = false)]
        string[] SourceAETitlesInSeries { get; set; }

        [DataMember(IsRequired = false)]
        string[] PhotometricInterpretationsInSeries { get; set; }

        [DataMember(IsRequired = false)]
        int[] BitsStoredInSeries { get; set; }

        [DataMember(IsRequired = false)]
        int[] BitsAllocatedInSeries { get; set; }
    }

    public interface IImageEntryData
    {
        [DataMember(IsRequired = false)]
        string TransferSyntax { get; set; }

        [DataMember(IsRequired = false)]
        string SourceAETitle { get; set; }

        [DataMember(IsRequired = false)]
        string PhotometricInterpretation { get; set; }

        [DataMember(IsRequired = false)]
        int? BitsStored { get; set; }

        [DataMember(IsRequired = false)]
        int? BitsAllocated { get; set; }
    }
}