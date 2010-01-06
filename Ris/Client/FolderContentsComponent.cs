#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Common;

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
        private bool _multiSelect = true;
        private ISelection _selectedItems = Selection.Empty;

        private event EventHandler _tableChanged;
        private event EventHandler _folderSystemChanged;
        private event EventHandler _selectedItemDoubleClicked;
        private event EventHandler _selectedItemsChanged;

        private IFolderSystem _folderSystem;
        private IFolder _selectedFolder;

    	private bool _suppressFolderContentSelectionChanges;

        public IFolderSystem FolderSystem
        {
            get { return _folderSystem; }
            set
            {
				if (_folderSystem != value)
				{
					// Must set the items and folders to null before chaning folder system, 
					// otherwise the tools that monitors folder and items selected will get out-of-sync with the folder system
					this.SelectedItems = Selection.Empty;
					this.SelectedFolder = null;

					_folderSystem = value;

					EventsHelper.Fire(_folderSystemChanged, this, EventArgs.Empty);
				}
            }
        }

        public IFolder SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                if (value != _selectedFolder)
                {
					if (_selectedFolder != null)
					{
						_selectedFolder.TotalItemCountChanged -= TotalItemCountChangedEventHandler;
						_selectedFolder.ItemsTableChanging -= ItemsTableChangingEventHandler;
						_selectedFolder.ItemsTableChanged -= ItemsTableChangedEventHandler;
						_selectedFolder.Updating -= UpdatingEventHandler;
						_selectedFolder.Updated -= UpdatedEventHandler;
					}

                	_selectedFolder = value;

                    if (_selectedFolder != null)
                    {
						_selectedFolder.TotalItemCountChanged += TotalItemCountChangedEventHandler;
						_selectedFolder.ItemsTableChanging += ItemsTableChangingEventHandler;
						_selectedFolder.ItemsTableChanged += ItemsTableChangedEventHandler;
						_selectedFolder.Updating += UpdatingEventHandler;
						_selectedFolder.Updated += UpdatedEventHandler;
					}

					// ensure that selection changes are not suppressed
					SuppressSelectionChanges(false);

                    // notify view
                    EventsHelper.Fire(_tableChanged, this, EventArgs.Empty);

					// notify that the selected items have changed (because the folder has changed)
					NotifySelectedItemsChanged();

					NotifyPropertyChanged("IsUpdating");
					NotifyPropertyChanged("StatusMessage");
                }
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

		public bool MultiSelect
		{
			get { return _multiSelect; }
		}

		public ITable FolderContentsTable
        {
            get { return _selectedFolder == null ? null : _selectedFolder.ItemsTable; }
        }

		// this is a bit of a hack to prevent the table view from sending selection changes during folder refreshes
		public bool SuppressFolderContentSelectionChanges
    	{
			get { return _suppressFolderContentSelectionChanges; }
    	}

        public string StatusMessage
        {
            get
            {
				if (_selectedFolder == null)
					return "";

				if (_selectedFolder.IsUpdating)
					return "Getting folder items...";

				// if no folder selected, or selected folder has 0 items or -1 items (i.e. unknown),
				// don't display a status message
                if (_selectedFolder.TotalItemCount < 1)
                    return "";

                if (_selectedFolder.TotalItemCount == _selectedFolder.ItemsTable.Items.Count)
                    return string.Format(SR.MessageShowAllItems, _selectedFolder.TotalItemCount);
                else 
                    return string.Format(SR.MessageShowPartialItems,
                                     _selectedFolder.ItemsTable.Items.Count, _selectedFolder.TotalItemCount);
            }
        }

    	public bool IsUpdating
    	{
			get { return _selectedFolder == null ? false : _selectedFolder.IsUpdating; }
    	}

        public ISelection SelectedItems
        {
            get { return _selectedItems; }
            set 
            {
                if (!_selectedItems.Equals(value))
                {
                    _selectedItems = value;
                	NotifySelectedItemsChanged();
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

		public ActionModelRoot ItemsToolbarModel
        {
            get
            {
                return _folderSystem == null || _folderSystem.ItemTools == null ? null
					: ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-items-toolbar", _folderSystem.ItemTools.Actions);
            }
        }

        public void DoubleClickSelectedItem()
        {
            EventsHelper.Fire(_selectedItemDoubleClicked, this, EventArgs.Empty);
        }

		#endregion

		private void NotifySelectedItemsChanged()
		{
			OnSelectedItemsChanged(this, EventArgs.Empty);
		}

		private void OnSelectedItemsChanged(object sender, EventArgs args)
		{
			EventsHelper.Fire(_selectedItemsChanged, sender, args);
		}

		private void TotalItemCountChangedEventHandler(object sender, EventArgs e)
        {
            NotifyPropertyChanged("StatusMessage");
        }

		private void ItemsTableChangingEventHandler(object sender, EventArgs e)
		{
			// suppress selection changes while the folder contents are being refreshed
			SuppressSelectionChanges(true);
		}

		private void ItemsTableChangedEventHandler(object sender, EventArgs e)
		{
			// update the selection appropriately - re-select the same items if possible
			// otherwise just select the first item by default

			// note: there are some subtleties here when attempting re-select the "same" items
			// if the items support IVersionedEquatable, then we need to compare them using a version-insensitive comparison,
			// but the new selection must consist of the instances that have the most current version
			_selectedItems = new Selection(CollectionUtils.Select(_selectedFolder.ItemsTable.Items,
					delegate(object item)
					{
						return CollectionUtils.Contains(_selectedItems.Items,
							delegate(object oldItem)
							{
								return (item is IVersionedEquatable)
									? (item as IVersionedEquatable).Equals(oldItem, true) // ignore version if IVersionedEquatable
									: Equals(item, oldItem);
							});
							
					}));

			// notify view about the updated selection table to the prior selection
			NotifySelectedItemsChanged();

			// revert back to normal mode where selection changes are not suppressed
			SuppressSelectionChanges(false);
		}

		private void SuppressSelectionChanges(bool suppress)
		{
			_suppressFolderContentSelectionChanges = suppress;
			NotifyPropertyChanged("SuppressFolderContentSelectionChanges");
		}

		private void UpdatingEventHandler(object sender, EventArgs e)
		{
			NotifyPropertyChanged("IsUpdating");
			NotifyPropertyChanged("StatusMessage");
		}

		private void UpdatedEventHandler(object sender, EventArgs e)
		{
			NotifyPropertyChanged("IsUpdating");
			NotifyPropertyChanged("StatusMessage");
		}

	}
}
