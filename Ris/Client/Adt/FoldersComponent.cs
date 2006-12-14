using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="FoldersComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class FoldersComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ExtensionPoint]
    public class FolderToolExtensionPoint : ExtensionPoint<ITool>
    {
    }


    public interface IFolderToolContext : IToolContext
    {
        IDesktopWindow DesktopWindow { get; }
        IList<IFolder> Folders { get; }
        IFolder SelectedFolder { get; set; }
    }

    /// <summary>
    /// FoldersComponent class
    /// </summary>
    [AssociateView(typeof(FoldersComponentViewExtensionPoint))]
    public class FoldersComponent : ApplicationComponent
    {
        class FolderToolContext : ToolContext, IFolderToolContext
        {
            FoldersComponent _component;
            
            public FolderToolContext(FoldersComponent component)
            {
                _component = component;
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            public IFolder SelectedFolder
            {
                get { return _component.SelectedFolder; }
                set { _component.SelectedFolder = value; }
            }

            public IList<IFolder> Folders
            {
                get { return _component.Folders; }
            }
        }

        class FolderList : ObservableList<IFolder, CollectionEventArgs<IFolder>>
        {
        }

        private ToolSet _toolSet;
        private FolderList _folders;
        private IFolder _selectedFolder;
        private event EventHandler _selectedFolderChanged;
        private WorklistComponent _worklistComponent;

        /// <summary>
        /// Constructor
        /// </summary>
        public FoldersComponent(WorklistComponent worklistComponent)
        {
            _worklistComponent = worklistComponent;
        }

        public override void Start()
        {
            _folders = new FolderList();
            _folders.ItemAdded += delegate(object sender, CollectionEventArgs<IFolder> e)
            {
                e.Item.ItemsChanged += new EventHandler(FolderItemsChangedEventHandler);
            };
            _folders.ItemRemoved += delegate(object sender, CollectionEventArgs<IFolder> e)
            {
                e.Item.ItemsChanged -= new EventHandler(FolderItemsChangedEventHandler);
            };

            _folders.Add(new Folder(SR.TitleFolderScheduledToday));
            _folders.Add(new Folder(SR.TitleFolderRecentlyArrived));
            _folders.Add(new Folder(SR.TitleFolderCancelledToday));
            _folders.Add(new Folder(SR.TitleFolderRecentItems));

            _toolSet = new ToolSet(new FolderToolExtensionPoint(), new FolderToolContext(this));

            base.Start();
        }

        private void FolderItemsChangedEventHandler(object sender, EventArgs e)
        {
            if (sender == _selectedFolder)
            {
                _worklistComponent.SearchResults = _selectedFolder.Items;
            }
        }

        public override void Stop()
        {
            _toolSet.Dispose();
            base.Stop();
        }

        #region Presentation Model

        public ActionModelNode ToolbarModel
        {
            get { return ActionModelRoot.CreateModel(this.GetType().FullName, "folders-toolbar", _toolSet.Actions); }
        }

        public IList<IFolder> Folders
        {
            get { return _folders; }
        }

        public IFolder SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                if (value != _selectedFolder)
                {
                    _selectedFolder = value;
                    _worklistComponent.SearchResults = _selectedFolder.Items;
                    EventsHelper.Fire(_selectedFolderChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectedFolderChanged
        {
            add { _selectedFolderChanged += value; }
            remove { _selectedFolderChanged -= value; }
        }

        #endregion
    }
}
