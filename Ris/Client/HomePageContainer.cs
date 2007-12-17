#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
    public interface IPreviewComponent : IApplicationComponent
    {
        void SetUrl(string url);
    }

    public class HomePageContainer : SplitComponentContainer, ISearchDataHandler
    {
        #region IFolderExplorerToolContext implementation

        class FolderExplorerToolContext : ToolContext, IFolderExplorerToolContext
        {
            private readonly HomePageContainer _component;

            public FolderExplorerToolContext(HomePageContainer component)
            {
                _component = component;
            }

            #region IFolderExplorerToolContext Members

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            public IFolder SelectedFolder
            {
                get { return (IFolder) _component.SelectedFolderExplorer.SelectedFolder.Item; }
                set { _component.SelectedFolderExplorer.SelectedFolder = new Selection(value); }
            }

            public ISelection SelectedItems
            {
                get { return _component._folderContentComponent.SelectedItems; }
            }

            public void RegisterSearchDataHandler(ISearchDataHandler handler)
            {
                _component.RegisterSearchDataHandler(handler);
            }

            #endregion
        }

        #endregion

        #region Search related

        private ISearchDataHandler _searchDataHandler;

        public void RegisterSearchDataHandler(ISearchDataHandler handler)
        {
            _searchDataHandler = handler;
        }

        public SearchData SearchData
        {
            set
            {
                if (_searchDataHandler != null)
                    _searchDataHandler.SearchData = value;
            }
        }

        #endregion

        private readonly Dictionary<IFolderSystem, FolderExplorerComponent> _folderExplorerComponents;
        private readonly FolderContentsComponent _folderContentComponent;
        private readonly IPreviewComponent _previewComponent;
        private readonly StackTabComponentContainer _stackContainers;

        private FolderExplorerComponent _selectedFolderExplorer;
        private readonly ToolSet _tools;

        public HomePageContainer(IExtensionPoint folderExplorerExtensionPoint, IPreviewComponent preview)
            : base(Desktop.SplitOrientation.Vertical)
        {
            _folderExplorerComponents = new Dictionary<IFolderSystem, FolderExplorerComponent>();
            _folderContentComponent = new FolderContentsComponent();
            _previewComponent = preview;

            _tools = new ToolSet(folderExplorerExtensionPoint, new FolderExplorerToolContext(this));

            // Construct the explorer component and place each into a stack tab
            _stackContainers = new StackTabComponentContainer(StackStyle.ShowOneOnly);
            _stackContainers.CurrentPageChanged += OnFolderSystemChanged;

            List<IFolderSystem> folderSystems = CollectionUtils.Map<ITool, IFolderSystem, List<IFolderSystem>>(_tools.Tools,
                delegate(ITool tool)
                {
                    FolderExplorerToolBase folderExplorerTool = (FolderExplorerToolBase) tool;
                    return folderExplorerTool.FolderSystem;
                });

            // Order the Folder Systems
            folderSystems = FolderExplorerComponentSettings.Default.OrderFolderSystems(folderSystems);

            CollectionUtils.ForEach(folderSystems,
                delegate(IFolderSystem folderSystem)
                    {
                        FolderExplorerComponent component = new FolderExplorerComponent(folderSystem);
                        component.SelectedFolderChanged += OnSelectedFolderChanged;
                        // TODO: what does this suppress??
                        //component.SuppressSelectionChanged += _folderContentComponent.OnSuppressSelectionChanged;

                        _folderExplorerComponents.Add(folderSystem, component);
                        _stackContainers.Pages.Add(new TabPage(folderSystem.DisplayName, component));
                    });

            // Construct the home page
            SplitComponentContainer contentAndPreview = new SplitComponentContainer(
                new SplitPane("Folder Contents", _folderContentComponent, 0.4f),
                new SplitPane("Content Preview", _previewComponent, 0.6f),
                SplitOrientation.Vertical);

            this.Pane1 = new SplitPane("Folders", _stackContainers, 0.2f);
            this.Pane2 = new SplitPane("Contents", contentAndPreview, 0.8f);
        }

        public override void Start()
        {
            base.Start();

            CollectionUtils.ForEach(_folderExplorerComponents.Keys,
                delegate(IFolderSystem folderSystem)
                    {
                        DocumentManager.RegisterFolderSystem(folderSystem);
                    });
        }

        public override void Stop()
        {
            CollectionUtils.ForEach(_folderExplorerComponents.Keys,
                delegate(IFolderSystem folderSystem)
                {
                    DocumentManager.UnregisterFolderSystem(folderSystem);
                });

            base.Stop();
        }

        public FolderExplorerComponent SelectedFolderExplorer
        {
            get { return _selectedFolderExplorer; }
            set
            {
                _selectedFolderExplorer = value;

                if (_selectedFolderExplorer == null)
                {
                    _folderContentComponent.FolderSystem = null;
                    _previewComponent.SetUrl(null);
                }
                else
                {
                    _folderContentComponent.FolderSystem = _selectedFolderExplorer.FolderSystem;
                    _previewComponent.SetUrl(_selectedFolderExplorer.FolderSystem.PreviewUrl);

                    IFolder selectedFolder = ((IFolder)_selectedFolderExplorer.SelectedFolder.Item);
                    _folderContentComponent.FolderContentsTable = selectedFolder == null ? null : selectedFolder.ItemsTable;
                }
            }
        }

        void OnFolderSystemChanged(object sender, EventArgs e)
        {
            this.SelectedFolderExplorer = (FolderExplorerComponent)_stackContainers.CurrentPage.Component;
        }

        void OnSelectedFolderChanged(object sender, EventArgs e)
        {
            IFolder selectedFolder = ((IFolder) _selectedFolderExplorer.SelectedFolder.Item);
            _folderContentComponent.FolderContentsTable = selectedFolder == null ? null : selectedFolder.ItemsTable;
        }

        public FolderContentsComponent ContentsComponent
        {
            get { return _folderContentComponent; }
        }
    }
}
