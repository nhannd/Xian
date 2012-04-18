using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    /// <summary>
    /// Service interface for querying the (local) study store.
    /// </summary>
    /// <remarks>
    /// It is generally reasonable to assume that any DICOM viewer is going to have a local store of studies. This interface
    /// allows for typical DICOM queries, plus any other common query operations, like getting the total number of studies in the store.
    /// </remarks>
    [ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName = "IStudyStoreQuery", Namespace = StudyManagementNamespace.Value)]
    public interface IStudyStoreQuery
    {
        [OperationContract]
        GetStudyCountResult GetStudyCount(GetStudyCountRequest request);

        [OperationContract]
        GetStudyEntriesResult GetStudyEntries(GetStudyEntriesRequest request);

        [OperationContract]
        GetSeriesEntriesResult GetSeriesEntries(GetSeriesEntriesRequest request);

        [OperationContract]
        GetImageEntriesResult GetImageEntries(GetImageEntriesRequest request);
    }
}
