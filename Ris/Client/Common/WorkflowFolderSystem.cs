using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Common
{
    public abstract class WorkflowFolderSystem<TItem> : IDisposable
    {
        private IFolderExplorerToolContext _folderExplorer;
        private List<WorkflowFolder<TItem>> _folders;

        public WorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
        {
            _folders = new List<WorkflowFolder<TItem>>();

            _folderExplorer = folderExplorer;
            _folderExplorer.SelectedFolderChanged += new EventHandler(_folderExplorer_SelectedFolderChanged);
        }

        ~WorkflowFolderSystem()
        {
            Dispose(false);
        }

        public IDesktopWindow DesktopWindow
        {
            get { return _folderExplorer.DesktopWindow; }
        }

        private void _folderExplorer_SelectedFolderChanged(object sender, EventArgs e)
        {
        }

        protected void AddFolder(WorkflowFolder<TItem> folder)
        {
            _folders.Add(folder);
            _folderExplorer.AddFolder(folder);
        }

        protected IList<WorkflowFolder<TItem>> Folders
        {
            get { return _folders; }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
