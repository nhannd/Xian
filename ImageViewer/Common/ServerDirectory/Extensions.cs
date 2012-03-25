using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public static class Extensions
    {
        public static IDicomServiceNode ToServiceNode(this IApplicationEntity server)
        {
            Platform.CheckForNullReference(server, "server");
            return new DicomServiceNode(server);
        }
    }
}
