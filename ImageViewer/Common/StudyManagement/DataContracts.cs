using System.Runtime.Serialization;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public static class StudyManagementNamespace
    {
        public const string Value = ImageViewerNamespace.Value + "/studyManagement";
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStudyCountRequest
    {
        [DataMember(IsRequired = false)]
        public StudyRootStudyIdentifier QueryIdentifier { get; set; }
    }

    [DataContract(Namespace = StudyManagementNamespace.Value)]
    public class GetStudyCountResult
    {
        [DataMember(IsRequired = true)]
        public int StudyCount { get; set; }
    }
}