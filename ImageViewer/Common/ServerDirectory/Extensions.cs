using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public static class Extensions
    {
        public static IDicomServiceNode ToServiceNode(this IApplicationEntity server)
        {
            return ToServiceNode((IDicomServerApplicationEntity) server);
        }

        public static IDicomServiceNode ToServiceNode(this IDicomServerApplicationEntity server)
        {
            Platform.CheckForNullReference(server, "server");
            if (server.IsStreaming)
                return new StreamingDicomServiceNode((IStreamingServerApplicationEntity)server);

            // TODO (CR Mar 2012): Currently, this would throw if it's not a DICOM service node.
            return new DicomServiceNode(server);
        }
    }
}
