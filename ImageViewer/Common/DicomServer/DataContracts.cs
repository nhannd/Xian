using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    public static class DicomServerNamespace
    {
        public const string Value = ImageViewerNamespace.Value + "/dicomServer";
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class GetDicomServerConfigurationResult
    {
        [DataMember(IsRequired = true)]
        public DicomServerConfiguration Configuration { get; set; }
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class GetDicomServerConfigurationRequest
    {}

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class UpdateDicomServerConfigurationResult
    {
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class UpdateDicomServerConfigurationRequest
    {
        [DataMember(IsRequired = true)]
        public DicomServerConfiguration Configuration { get; set; }
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class DicomServerConfiguration
    {
        [DataMember(IsRequired = true)]
        public string HostName { get; set; }

        [DataMember(IsRequired = true)]
        public string AETitle { get; set; }

        [DataMember(IsRequired = true)]
        public int Port { get; set; }

        // TODO (Marmot): Gonzo.
        [DataMember(IsRequired = true)]
        public string InterimStorageDirectory { get; set; }
    }
}
