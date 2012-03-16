using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    [ServiceContract(SessionMode = SessionMode.Allowed, ConfigurationName = "IDicomServerConfiguration", Namespace = DicomServerNamespace.Value)]
    interface IDicomServerConfiguration
    {
        [OperationContract]
        GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request);

        [OperationContract]
        UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request);
    }
}
