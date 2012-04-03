using System.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    //TODO (Marmot):Name?

    /// <summary>
    /// Service interface for the local study store.
    /// </summary>
    /// <remarks>
    /// It is generally reasonable to assume that any DICOM viewer is going to have a local store of studies. This interface
    /// allows for typical DICOM queries, plus any other common query operations, like getting the total number of studies in the store.
    /// </remarks>
    [ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName="IStudyStore" , Namespace = StudyManagementNamespace.Value)]
    public interface IStudyStore : IStudyRootQuery
    {
        [OperationContract]
        GetStudyCountResult GetStudyCount(GetStudyCountRequest request);
    }
}
