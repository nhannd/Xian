using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    // TODO (CR Jun 2012): Remove this? It's really not used anymore, although the "data contracts" remain useful.
    [ServiceContract(SessionMode = SessionMode.Allowed,
        ConfigurationName = "IStorageConfiguration",
        Namespace = StudyManagementNamespace.Value)]
    public interface IStorageConfiguration
    {
        [OperationContract]
        GetStorageConfigurationResult GetConfiguration(GetStorageConfigurationRequest request);

        [OperationContract]
        UpdateStorageConfigurationResult UpdateConfiguration(UpdateStorageConfigurationRequest request);
    }
}
