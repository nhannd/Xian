using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    /// TODO (CR Jun 2012): Can leave the interface, but probably should make this use new settings provider, just so we don't have to figure out how to upgrade/migrate it.

    [ServiceContract(SessionMode = SessionMode.Allowed,
        ConfigurationName = "IStorageConfiguration",
        Namespace = StudyManagementNamespace.Value)]
    public interface IStorageConfiguration
    {
        [OperationContract]
        GetStorageConfigurationResult GetConfiguration(GetStorageConfigurationRequest request);

        [OperationContract]
        [FaultContract(typeof(ServiceStateFault))]
        UpdateStorageConfigurationResult UpdateConfiguration(UpdateStorageConfigurationRequest request);
    }
}
