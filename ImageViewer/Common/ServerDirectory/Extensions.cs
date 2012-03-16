using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public static class Extensions
    {
        public static IServiceNode ToServiceNode(this PersistedServerEntry entry)
        {
            Platform.CheckForNullReference(entry, "entry");
            if (entry.Server.IsStreaming)
                return new PersistedStreamingDicomServiceNode(entry);

            // TODO (CR Mar 2012): Currently, this would throw if it's not a DICOM service node.
            return new PersistedDicomServiceNode(entry);
        }
    }
}
