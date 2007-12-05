#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="FolderContentsComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class FolderContentsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistExplorerComponent class
    /// </summary>
    [AssociateView(typeof(FolderContentsComponentViewExtensionPoint))]
    public class FolderContentsComponent : ApplicationComponent
    {
        private bool _multiSelect;
        private ISelection _selectedItems = Selection.Empty;

        private event EventHandler _tableChanged;
        private event EventHandler _folderSystemChanged;
        private event EventHandler _selectedItemDoubleClicked;
        private event EventHandler _selectedItemsChanged;

        private IFolderSystem _folderSystem;
        private ITable _folderContentsTable;

        public IFolderSystem FolderSystem
        {
            get { return _folderSystem; }
            set
            {
                if (_folderSystem != null)
                {
                    this.SelectedItemsChanged -= _folderSystem.SelectedItemsChangedEventHandler;
                    this.SelectedItemDoubleClicked -= _folderSystem.SelectedItemDoubleClickedEventHandler;
                }

                _folderSystem = value;

                if (_folderSystem != null)
                {
                    this.SelectedItemsChanged += _folderSystem.SelectedItemsChangedEventHandler;
                    this.SelectedItemDoubleClicked += _folderSystem.SelectedItemDoubleClickedEventHandler;
                }

                EventsHelper.Fire(_folderSystemChanged, this, EventArgs.Empty);
            }
        }

        #region Application Component overrides

        public override IActionSet ExportedActions
        {
            get 
            {
                return _folderSystem == null || _folderSystem.ItemTools == null
                    ? new ActionSet() 
                    : _folderSystem.ItemTools.Actions; 
            }
        }

        #endregion

        #region Presentation Model

        public ITable FolderContentsTable
        {
            get { return _folderContentsTable; }
            set
            {
                _folderContentsTable = value;
                EventsHelper.Fire(_tableChanged, this, EventArgs.Empty);
            }
        }

        public bool MultiSelect
        {
            get { return _multiSelect; }
            set { _multiSelect = value; }
        }

        public ISelection SelectedItems
        {
            get { return _selectedItems; }
            set 
            {
                if (!_selectedItems.Equals(value))
                {
                    _selectedItems = value;
                    EventsHelper.Fire(_selectedItemsChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler TableChanged
        {
            add { _tableChanged += value; }
            remove { _tableChanged -= value; }
        }

        public event EventHandler FolderSystemChanged
        {
            add { _folderSystemChanged += value; }
            remove { _folderSystemChanged -= value; }
        }

        public event EventHandler SelectedItemDoubleClicked
        {
            add { _selectedItemDoubleClicked += value; }
            remove { _selectedItemDoubleClicked -= value; }
        }

        public event EventHandler SelectedItemsChanged
        {
            add { _selectedItemsChanged += value; }
            remove { _selectedItemsChanged -= value; }
        }

        public ActionModelRoot ItemsContextMenuModel
        {
            get
            {
                return _folderSystem == null || _folderSystem.ItemTools == null ? null
                    : ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-items-contextmenu", _folderSystem.ItemTools.Actions);
            }
        }

        public ActionModelNode ItemsToolbarModel
        {
            get
            {
                return _folderSystem == null || _folderSystem.ItemTools == null ? null
                    : ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-items-toolbar", _folderSystem.ItemTools.Actions);
            }
        }

        public void OnSelectedItemDoubleClicked()
        {
            EventsHelper.Fire(_selectedItemDoubleClicked, this, EventArgs.Empty);
        }

        #endregion
    }
}
