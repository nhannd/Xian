using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    public abstract class WorkflowFolderSystem<TItem> : IDisposable
    {
        private IFolderExplorerToolContext _folderExplorer;
        private List<WorkflowFolder<TItem>> _folders;
        private event EventHandler _selectedItemsChanged;
        private event EventHandler _selectedFolderChanged;

        public WorkflowFolderSystem(IFolderExplorerToolContext folderExplorer)
        {
            _folders = new List<WorkflowFolder<TItem>>();

            _folderExplorer = folderExplorer;
            _folderExplorer.SelectedFolderChanged += new EventHandler(_folderExplorer_SelectedFolderChanged);
            _folderExplorer.SelectedItemsChanged += new EventHandler(_folderExplorer_SelectedItemsChanged);
        }

        ~WorkflowFolderSystem()
        {
            Dispose(false);
        }

        public IDesktopWindow DesktopWindow
        {
            get { return _folderExplorer.DesktopWindow; }
        }

        public event EventHandler SelectedItemsChanged
        {
            add { _selectedItemsChanged += value; }
            remove { _selectedItemsChanged -= value; }
        }

        public event EventHandler SelectedFolderChanged
        {
            add { _selectedFolderChanged += value; }
            remove { _selectedFolderChanged -= value; }
        }

        public IFolder SelectedFolder
        {
            get { return _folderExplorer.SelectedFolder; }
            set { _folderExplorer.SelectedFolder = value; }
        }

        public ICollection<TItem> SelectedItems
        {
            get
            {
                // in general we don't know what type the selected items are, since they may
                // have come from another folder system
                // therefore, need to check if they are of type TItem and only include them
                // in SelectedItems if they are of the correct type
                List<TItem> items = new List<TItem>();

                // map the selection to a collection of TItem
                foreach(object obj in _folderExplorer.SelectedItems.Items)
                {
                    if(obj is TItem)
                        items.Add((TItem)obj);
                }
                return items;
            }
        }

        private void _folderExplorer_SelectedFolderChanged(object sender, EventArgs e)
        {
            EventsHelper.Fire(_selectedFolderChanged, this, EventArgs.Empty);
        }

        private void _folderExplorer_SelectedItemsChanged(object sender, EventArgs e)
        {
            EventsHelper.Fire(_selectedItemsChanged, this, EventArgs.Empty);
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
            foreach (WorkflowFolder<TItem> folder in _folders)
            {
                folder.Dispose();
            }
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
