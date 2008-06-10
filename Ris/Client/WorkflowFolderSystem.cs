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
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using System.Collections;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public abstract class WorkflowFolderSystem<TItem> : IFolderSystem
    {
		#region WorkflowItemToolContext class

		protected class WorkflowItemToolContext : IWorkflowItemToolContext<TItem>
		{
			private readonly WorkflowFolderSystem<TItem> _owner;

			public WorkflowItemToolContext(WorkflowFolderSystem<TItem> owner)
			{
				_owner = owner;
			}


			#region IWorkflowItemToolContext<TItem> Members

			public ICollection<TItem> SelectedItems
			{
				get
				{
					return CollectionUtils.Map<object, TItem>(_owner.SelectedItems.Items,
						delegate(object item) { return (TItem)item; });
				}
			}

			#endregion

			#region IWorkflowItemToolContext Members

			public bool GetWorkflowOperationEnablement(string operationClass)
			{
				return _owner.GetOperationEnablement(operationClass);
			}

			public event EventHandler SelectionChanged
			{
				add { _owner.SelectedItemsChanged += value; }
				remove { _owner.SelectedItemsChanged -= value; }
			}

			public ISelection Selection
			{
				get { return _owner.SelectedItems; }
			}

			public IEnumerable Folders
			{
				get { return _owner.Folders; }
			}

			public IFolder SelectedFolder
			{
				get { return _owner.SelectedFolder; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _owner.DesktopWindow; }
			}

			#endregion
		}

		#endregion

		#region WorkflowFolderToolContext class
		
		protected class WorkflowFolderToolContext : IWorkflowFolderToolContext
		{
			private readonly WorkflowFolderSystem<TItem> _owner;

			public WorkflowFolderToolContext(WorkflowFolderSystem<TItem> owner)
			{
				_owner = owner;
			}

			#region IWorkflowFolderToolContext Members

			public IEnumerable Folders
			{
				get { return _owner.Folders; }
			}

			public IFolder SelectedFolder
			{
				get { return _owner.SelectedFolder; }
			}

			public event EventHandler SelectedFolderChanged
			{
				add { _owner.SelectedFolderChanged += value; }
				remove { _owner.SelectedFolderChanged -= value; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _owner.DesktopWindow; }
			}

			#endregion
		}

		#endregion

        private readonly IFolderExplorerToolContext _folderExplorer;
        private readonly IDictionary<string, Type> _mapWorklistClassToFolderClass = new Dictionary<string, Type>();
        private readonly IList<IFolder> _workflowFolders;

        private event EventHandler _selectedItemDoubleClicked;
        private event EventHandler _selectedItemsChanged;
        private event EventHandler _selectedFolderChanged;
    	private event EventHandler _titleChanged;
    	private event EventHandler _titleIconChanged;

		private string _title;
    	private IconSet _titleIcon;
		protected IResourceResolver _resourceResolver;
		
		protected IToolSet _itemTools;
        protected IToolSet _folderTools;

		private IDictionary<string, bool> _workflowEnablement;

        public WorkflowFolderSystem(string title, IFolderExplorerToolContext folderExplorer)
            : this(title, folderExplorer, null)
        {
        }

		public WorkflowFolderSystem(string title, IFolderExplorerToolContext folderExplorer, ExtensionPoint<IFolder> folderExtensionPoint)
        {
			_title = title;
			_folderExplorer = folderExplorer;
            _workflowFolders = new List<IFolder>();

			// establish default resource resolver
			_resourceResolver = new ResourceResolver(this.GetType(), true);

			InitializeFolders(folderExtensionPoint);

            _itemTools = new ToolSet();
            _folderTools = new ToolSet();
        }

		private void InitializeFolders(ExtensionPoint<IFolder> folderExtensionPoint)
		{
			// Collect all worklist classes
			if (folderExtensionPoint != null)
			{
				foreach (IFolder folder in folderExtensionPoint.CreateExtensions())
				{
					if (folder is WorkflowFolder<TItem>)
					{
						WorkflowFolder<TItem> workflowFolder = (WorkflowFolder<TItem>)folder;
						if (!string.IsNullOrEmpty(workflowFolder.WorklistClassName))
							_mapWorklistClassToFolderClass.Add(workflowFolder.WorklistClassName, workflowFolder.GetType());
					}
				}
			}

			if (_mapWorklistClassToFolderClass.Keys.Count > 0)
			{
				ListWorklistsForUserResponse response = QueryWorklistSet(new ListWorklistsForUserRequest(this.WorklistClassNames));
				foreach (WorklistSummary summary in response.Worklists)
				{
					try
					{
						Type folderClass = _mapWorklistClassToFolderClass[summary.ClassName];
						IFolder folder = (IFolder)Activator.CreateInstance(folderClass, this, summary.DisplayName, summary.Description, summary.WorklistRef);
						if (folder != null)
						{
							folder.IsStatic = false;
							this.AddFolder(folder);
						}

					}
					catch (KeyNotFoundException e)
					{
						Platform.Log(LogLevel.Error, e, string.Format("Worklist class {0} is not mapped to a folder class.", summary.ClassName));
					}
				}
			}
		}

    	protected abstract ListWorklistsForUserResponse QueryWorklistSet(ListWorklistsForUserRequest request);


        ~WorkflowFolderSystem()
        {
            Dispose(false);
        }

        #region IFolderSystem implmentation

        public string Id
        {
            get { return this.GetType().FullName; }
        }

    	public string Title
    	{
			get { return _title; }
			set 
			{
				_title = value;
				EventsHelper.Fire(_titleChanged, this, EventArgs.Empty);
			}
    	}

		public IconSet TitleIcon
		{
			get { return _titleIcon; }
			set
			{
				_titleIcon = value;
				EventsHelper.Fire(_titleIconChanged, this, EventArgs.Empty);
			}
		}

		public IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
			set { _resourceResolver = value; }
		}

		public IList<IFolder> Folders
        {
            get { return _workflowFolders; }
        }

        public IToolSet FolderTools
        {
            get { return _folderTools; }
        }

        public IToolSet ItemTools
        {
            get { return _itemTools; }
        }

        public abstract string PreviewUrl { get; }

		public bool GetOperationEnablement(string operationName)
		{
			try
			{
				return _workflowEnablement == null ? false : _workflowEnablement[operationName];
			}
			catch (KeyNotFoundException)
			{
				Platform.Log(LogLevel.Error, string.Format(SR.ExceptionOperationEnablementUnknown, operationName));
				return false;
			}
		}

		#endregion

        /// <summary>
        /// Invalidates all folders of the specified type so that they
        /// will refresh their content or count.
        /// </summary>
        /// <param name="folderType"></param>
        public void InvalidateFolder(Type folderType)
        {
            // TODO: could implement more of an "invalidate", rather than an immediate refresh
            // the folder doesn't actually need to refresh unless the explorer workspace is visible
            foreach (IFolder folder in _workflowFolders)
            {
                if(folder.GetType().Equals(folderType))
                {
                    if(folder.IsOpen)
                        folder.Refresh();
                    else 
                        folder.RefreshCount();
                }
            }
        }

        /// <summary>
        /// Invalidates the currently selected folder, causing it to re-populate its contents.
        /// </summary>
        public void InvalidateSelectedFolder()
        {
            // TODO: could implement more of an "invalidate", rather than an immediate refresh
            // the folder doesn't actually need to refresh unless the explorer workspace is visible
			if(_folderExplorer.SelectedFolder != null)
				_folderExplorer.SelectedFolder.Refresh();
        }

        public Type GetFolderClassForWorklistClass(string worklistClassName)
        {
            return _mapWorklistClassToFolderClass.ContainsKey(worklistClassName) ? _mapWorklistClassToFolderClass[worklistClassName] : null;
        }

        public List<string> WorklistClassNames
        {
            get { return new List<string>(_mapWorklistClassToFolderClass.Keys); }
        }

        public IDesktopWindow DesktopWindow
        {
            get { return _folderExplorer.DesktopWindow; }
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

        public event EventHandler SelectedFolderChanged
        {
            add { _selectedFolderChanged += value; }
            remove { _selectedFolderChanged -= value; }
        }

		public event EventHandler TextChanged
		{
			add { _titleChanged += value; }
			remove { _titleChanged -= value; }
		}

		public event EventHandler IconChanged
		{
			add { _titleIconChanged += value; }
			remove { _titleIconChanged -= value; }
		}

		public IFolder SelectedFolder
        {
            get { return _folderExplorer.SelectedFolder; }
            set { _folderExplorer.SelectedFolder = value; }
        }

        public ISelection SelectedItems
        {
            get { return _folderExplorer.SelectedItems; }
        }

        public virtual void SelectedFolderChangedEventHandler(object sender, EventArgs e)
        {
            EventsHelper.Fire(_selectedFolderChanged, this, EventArgs.Empty);
        }

        public void SelectedItemsChangedEventHandler(object sender, EventArgs e)
        {
			try
			{
				BlockingOperation.Run(
					delegate
					{
						_workflowEnablement = this.SelectedItems.Equals(Selection.Empty) ? null :
							QueryOperationEnablement(this.SelectedItems);
					});
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex);
			}

			EventsHelper.Fire(_selectedItemsChanged, this, EventArgs.Empty);
        }

    	protected abstract IDictionary<string, bool> QueryOperationEnablement(ISelection selection);

        public virtual void SelectedItemDoubleClickedEventHandler(object sender, EventArgs e)
        {
            EventsHelper.Fire(_selectedItemDoubleClicked, this, EventArgs.Empty);
        }

        protected void AddFolder(IFolder folder)
        {
            _workflowFolders.Add(folder);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
				foreach (IFolder folder in _workflowFolders)
				{
					if (folder is IDisposable)
						((IDisposable)folder).Dispose();
				}

				if (_itemTools != null) _itemTools.Dispose();
                if (_folderTools != null) _folderTools.Dispose();
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
