using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    //public interface IDicomServerConfigurationCallback
    //{
    //    [OperationContract]
    //    void ConfigurationChanged();
    //}

    [ServiceContract(SessionMode = SessionMode.Allowed,
        ConfigurationName = "IDicomServerConfiguration",
        //CallbackContract = typeof(IDicomServerConfigurationCallback),
        Namespace = DicomServerNamespace.Value)]
    public interface IDicomServerConfiguration
    {
        [OperationContract]
        GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request);

        [OperationContract]
        UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request);
    }
}
