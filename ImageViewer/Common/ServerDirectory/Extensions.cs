using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public static class Extensions
    {
        public static IServiceNode ToServiceNode(this ServerDirectoryEntry entry)
        {
            Platform.CheckForNullReference(entry, "entry");
            if (entry.Server.IsStreaming)
                return new StreamingDicomServiceNode(entry);

            // TODO (CR Mar 2012): Currently, this would throw if it's not a DICOM service node.
            return new DicomServiceNode(entry);
        }
    }
}
