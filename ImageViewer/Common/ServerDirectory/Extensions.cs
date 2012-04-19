using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public static class Extensions
    {
        public static IDicomServiceNode ToServiceNode(this DicomServerConfiguration serverConfiguration)
        {
            Platform.CheckForNullReference(serverConfiguration, "serverConfiguration");
            return new DicomServiceNode(serverConfiguration);
        }

        public static IDicomServiceNode ToServiceNode(this IApplicationEntity server)
        {
            Platform.CheckForNullReference(server, "server");
            return new DicomServiceNode(server);
        }
    }
}
