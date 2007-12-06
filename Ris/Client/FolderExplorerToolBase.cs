using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
    public interface IFolderExplorerToolContext : IToolContext
    {
        IDesktopWindow DesktopWindow { get; }
        IFolder SelectedFolder { get; set; }
        ISelection SelectedItems { get; }
        void RegisterSearchDataHandler(ISearchDataHandler handler);
    }

    public class FolderExplorerToolBase : Tool<IFolderExplorerToolContext>
    {
        protected IFolderSystem _folderSystem;

        public IFolderSystem FolderSystem
        {
            get { return _folderSystem; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_folderSystem != null) _folderSystem.Dispose();
            }
        }
    }
}
