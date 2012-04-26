using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
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
