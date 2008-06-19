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
using System.Threading;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Abstract base class for workflow folder systems.  A workflow folder system is a folder system
	/// that consists of <see cref="WorkflowFolder"/>s. 
	/// </summary>
	public abstract class WorkflowFolderSystem : IFolderSystem
	{
		#region FolderList

		class FolderList : ObservableList<IFolder>
		{
			private readonly WorkflowFolderSystem _owner;

			public FolderList(WorkflowFolderSystem owner)
			{
				_owner = owner;
			}

			protected override void OnItemAdded(ListEventArgs<IFolder> e)
			{
				// this cast is safe in practice
				((WorkflowFolder)e.Item).SetFolderSystem(_owner);
				base.OnItemAdded(e);
			}

			protected override void OnItemRemoved(ListEventArgs<IFolder> e)
			{
				// this cast is safe in practice
				((WorkflowFolder)e.Item).SetFolderSystem(null);
				base.OnItemRemoved(e);
			}
		}

		#endregion

		#region WorkflowItemToolContext

		protected class WorkflowItemToolContext : IWorkflowItemToolContext
        {
            private readonly WorkflowFolderSystem _owner;

            public WorkflowItemToolContext(WorkflowFolderSystem owner)
            {
                _owner = owner;
            }

            public bool GetOperationEnablement(string operationClass)
            {
                return _owner.GetOperationEnablement(operationClass);
            }

			public bool GetOperationEnablement(Type serviceContract, string operationClass)
			{
				return _owner.GetOperationEnablement(serviceContract, operationClass);
			}

            public event EventHandler SelectionChanged
            {
                add { _owner.SelectionChanged += value; }
                remove { _owner.SelectionChanged -= value; }
            }

            public ISelection Selection
            {
                get { return _owner.Selection; }
            }

            public IFolder SelectedFolder
            {
                get { return _owner.SelectedFolder; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _owner.DesktopWindow; }
            }

            public void InvalidateSelectedFolder()
            {
                _owner.InvalidateSelectedFolder();
            }

            public void InvalidateFolder(Type folderClass)
            {
                _owner.InvalidateFolder(folderClass);
            }

            public void RegisterDoubleClickHandler(ClickHandlerDelegate handler)
            {
                _owner._doubleClickHandler = handler;
            }

            protected void RegisterDropHandler(Type folderClass, object dropHandler)
            {
                _owner.RegisterDropHandler(folderClass, dropHandler);
            }

			public void RegisterWorkflowService(Type serviceContract)
			{
				_owner.RegisterWorkflowService(serviceContract);
			}
		}

        #endregion

		private readonly IFolderExplorerToolContext _folderExplorer;
		private readonly FolderList _workflowFolders;

		private event EventHandler _selectedItemDoubleClicked;
		private event EventHandler _selectedItemsChanged;
		private event EventHandler _titleChanged;
		private event EventHandler _titleIconChanged;
		private event EventHandler _foldersChanged;


		private string _title;
		private IconSet _titleIcon;
		private IResourceResolver _resourceResolver;

		private IToolSet _itemTools;
		private IToolSet _folderTools;

		private readonly List<Type> _workflowServices = new List<Type>();
		private IDictionary<string, bool> _workflowEnablement;
        private readonly Dictionary<Type, List<object>> _mapFolderClassToDropHandlers = new Dictionary<Type, List<object>>();
        private ClickHandlerDelegate _doubleClickHandler;

        private SearchResultsFolder _searchFolder;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="title">Initial title of the folder system.</param>
		/// <param name="folderExplorer">The folder explorer that is hosting the folder system.</param>
		protected WorkflowFolderSystem(string title, IFolderExplorerToolContext folderExplorer)
		{
			_title = title;
			_folderExplorer = folderExplorer;

			// establish default resource resolver
			_resourceResolver = new ResourceResolver(this.GetType(), true);

			_workflowFolders = new FolderList(this);
		}

		/// <summary>
		/// Finalizer
		/// </summary>
		~WorkflowFolderSystem()
		{
			Dispose(false);
		}

		#region IFolderSystem implementation

		public IDesktopWindow DesktopWindow
		{
			get { return _folderExplorer.DesktopWindow; }
		}

		public string Id
		{
			get { return this.GetType().FullName; }
		}

		public string Title
		{
			get { return _title; }
			protected set
			{
				_title = value;
				EventsHelper.Fire(_titleChanged, this, EventArgs.Empty);
			}
		}

		public IconSet TitleIcon
		{
			get { return _titleIcon; }
			protected set
			{
				_titleIcon = value;
				EventsHelper.Fire(_titleIconChanged, this, EventArgs.Empty);
			}
		}

		public IResourceResolver ResourceResolver
		{
			get { return _resourceResolver; }
			protected set { _resourceResolver = value; }
		}

		public ObservableList<IFolder> Folders
		{
			get { return _workflowFolders; }
		}

		public IToolSet FolderTools
		{
			get
			{
				if (_folderTools == null)
					_folderTools = CreateFolderToolSet();
				return _folderTools;
			}
		}

		public IToolSet ItemTools
		{
			get
			{
				if (_itemTools == null)
					_itemTools = CreateItemToolSet();
				return _itemTools;
			}
		}

		public string PreviewUrl
		{
			get { return GetPreviewUrl(); }
		}

		public event EventHandler TitleChanged
		{
			add { _titleChanged += value; }
			remove { _titleChanged -= value; }
		}

		public event EventHandler TitleIconChanged
		{
			add { _titleIconChanged += value; }
			remove { _titleIconChanged -= value; }
		}

		public event EventHandler FoldersChanged
		{
			add { _foldersChanged += value; }
			remove { _foldersChanged -= value; }
		}

		public void OnSelectedItemsChanged()
		{
			try
			{
				BlockingOperation.Run(
					delegate
					{
						_workflowEnablement = this.Selection.Equals(Desktop.Selection.Empty) ? null :
							QueryOperationEnablement(this.Selection);
					});
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex);
			}

			EventsHelper.Fire(_selectedItemsChanged, this, EventArgs.Empty);
		}

		public virtual void OnSelectedItemDoubleClicked()
		{
			EventsHelper.Fire(_selectedItemDoubleClicked, this, EventArgs.Empty);

            if (_doubleClickHandler != null)
                _doubleClickHandler();
		}

        /// <summary>
        /// Gets a value indicating whether this folder system supports searching.
        /// </summary>
        public virtual bool SearchEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Performs a search, if supported.
        /// </summary>
        /// <param name="data"></param>
        public void ExecuteSearch(SearchData data)
        {
            if (this.SearchResultsFolder != null)
            {
                this.SearchResultsFolder.SearchData = data;
                this.SelectedFolder = this.SearchResultsFolder;
            }
        }

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
				if (folder.GetType().Equals(folderType))
				{
					if (folder.IsOpen)
						folder.Refresh();
					else
						folder.RefreshCount();
				}
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion


		#region Protected API

		/// <summary>
		/// Called to instantiate the tool set for tools that operate on items.
		/// </summary>
		/// <returns></returns>
		protected abstract IToolSet CreateItemToolSet();

		/// <summary>
		/// Called to instantiate the tool set for tools that operate on folders.
		/// </summary>
		/// <returns></returns>
		protected abstract IToolSet CreateFolderToolSet();

		/// <summary>
		/// Called to instantiate the search-results folder, if this folder system supports searches.
		/// </summary>
		/// <returns></returns>
        protected abstract SearchResultsFolder CreateSearchResultsFolder();

		/// <summary>
		/// Called to obtain the URL of the preview page.
		/// </summary>
		/// <returns></returns>
		protected abstract string GetPreviewUrl();

		/// <summary>
		/// Called whenever the selection changes, to obtain the operation enablement for a given selection.
		/// </summary>
		/// <param name="selection"></param>
		/// <returns></returns>
		protected abstract IDictionary<string, bool> QueryOperationEnablement(ISelection selection);

		/// <summary>
		/// Called to allow a subclass to select a drop handler from the list of registered drop handlers, for
		/// a given set of items.
		/// </summary>
		/// <param name="handlers"></param>
		/// <param name="items"></param>
		/// <returns></returns>
        protected abstract object SelectDropHandler(IList handlers, object[] items);

		/// <summary>
		/// Registers the specified drop handler for the specified folder class.
		/// </summary>
		/// <param name="folderClass"></param>
		/// <param name="dropHandler"></param>
        protected void RegisterDropHandler(Type folderClass, object dropHandler)
        {
            List<object> handlers;
            if (!_mapFolderClassToDropHandlers.TryGetValue(folderClass, out handlers))
                _mapFolderClassToDropHandlers.Add(folderClass, handlers = new List<object>());
            handlers.Add(dropHandler);
        }

		/// <summary>
		/// Dispose pattern.
		/// </summary>
		/// <param name="disposing"></param>
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

		/// <summary>
		/// Gets the current selection of items.
		/// </summary>
		protected internal ISelection Selection
		{
			get { return _folderExplorer.SelectedItems; }
		}

		/// <summary>
		/// Occurs when the items selection changes.
		/// </summary>
		protected internal event EventHandler SelectionChanged
		{
			add { _selectedItemsChanged += value; }
			remove { _selectedItemsChanged -= value; }
		}

		/// <summary>
		/// Gets or sets the currently selected folder.
		/// </summary>
		protected internal IFolder SelectedFolder
		{
			get { return _folderExplorer.SelectedFolder; }
			set { _folderExplorer.SelectedFolder = value; }
		}

		/// <summary>
		/// Occurs when the selected folder changes.
		/// </summary>
		protected internal event EventHandler SelectedFolderChanged
		{
			add { _folderExplorer.SelectedFolderChanged += value; }
			remove { _folderExplorer.SelectedFolderChanged -= value; }
		}

		/// <summary>
		/// Invalidates the currently selected folder, causing it to re-populate its contents.
		/// </summary>
		protected internal void InvalidateSelectedFolder()
		{
			// TODO: could implement more of an "invalidate", rather than an immediate refresh
			// the folder doesn't actually need to refresh unless the explorer workspace is visible
			if (_folderExplorer.SelectedFolder != null)
				_folderExplorer.SelectedFolder.Refresh();
		}

		/// <summary>
		/// Gets the search-results folder, or null if this system does not support searches.
		/// </summary>
		protected SearchResultsFolder SearchResultsFolder
        {
            get
            {
                if(_searchFolder == null)
                {
                    _searchFolder = CreateSearchResultsFolder();
                    _workflowFolders.Add(_searchFolder);
                }
                return _searchFolder;
            }
        }

		/// <summary>
		/// Gets the set of registered workflow services.
		/// </summary>
		/// <returns></returns>
		protected IEnumerable<Type> GetRegisteredWorkflowServices()
		{
			return _workflowServices;
		}

		/// <summary>
		/// Notifies that the entire <see cref="Folders"/> collection has changed.
		/// </summary>
		protected void NotifyFoldersChanged()
		{
			EventsHelper.Fire(_foldersChanged, this, EventArgs.Empty);
		}

		#endregion

        #region Helpers

		/// <summary>
		/// Called by <see cref="WorkflowFolder"/>s to obtain the drop handler for the specified items.
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		internal object GetDropHandler(WorkflowFolder folder, object[] items)
		{
			List<object> handlers;
			if(!_mapFolderClassToDropHandlers.TryGetValue(folder.GetType(), out handlers))
				handlers = new List<object>();

			return SelectDropHandler(handlers, items);
		}

		/// <summary>
		/// Registers the specified workflow service.
		/// </summary>
		/// <param name="workflowService"></param>
		private void RegisterWorkflowService(Type workflowService)
		{
			if(!_workflowServices.Contains(workflowService))
				_workflowServices.Add(workflowService);
		}

		/// <summary>
		/// Gets a value indicating whether the specified operation is enabled for the current selection.
		/// </summary>
		/// <param name="operationName"></param>
		/// <returns></returns>
		private bool GetOperationEnablement(string operationName)
		{
			// case where no item selected
			if (_workflowEnablement == null)
				return false;

			// the name may already be fully qualified
			bool result;
			if (_workflowEnablement.TryGetValue(operationName, out result))
				return result;

			// try to resolve the unqualified name
			string qualifiedKey = CollectionUtils.SelectFirst(_workflowEnablement.Keys,
				delegate(string key) { return key.EndsWith(operationName); });

			if (qualifiedKey != null && _workflowEnablement.TryGetValue(qualifiedKey, out result))
				return result;

			// couldn't resolve it
			Platform.Log(LogLevel.Error, string.Format(SR.ExceptionOperationEnablementUnknown, operationName));
			return false;
		}

		/// <summary>
		/// Gets a value indicating whether the specified operation is enabled for the current selection.
		/// </summary>
		/// <param name="serviceContract"></param>
		/// <param name="operationName"></param>
		/// <returns></returns>
		private bool GetOperationEnablement(Type serviceContract, string operationName)
		{
			return GetOperationEnablement(string.Format("{0}.{1}", serviceContract.FullName, operationName));
		}

		#endregion

	}

	/// <summary>
	/// Abstract base class for workflow folder systems.
	/// </summary>
	/// <typeparam name="TItem">The class of item that the folders contain.</typeparam>
	/// <typeparam name="TFolderToolExtensionPoint">Extension point that defines the set of folder tools for this folders system.</typeparam>
	/// <typeparam name="TItemToolExtensionPoint">Extension point that defines the set of item tools for this folders system.</typeparam>
	public abstract class WorkflowFolderSystem<TItem, TFolderToolExtensionPoint, TItemToolExtensionPoint> : WorkflowFolderSystem
		where TItem : DataContractBase
		where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
    {
		#region WorkflowItemToolContext class

        protected new class WorkflowItemToolContext : WorkflowFolderSystem.WorkflowItemToolContext, IWorkflowItemToolContext<TItem>
		{
			public WorkflowItemToolContext(WorkflowFolderSystem owner)
                :base(owner)
			{
			}

			public ICollection<TItem> SelectedItems
			{
				get
				{
					return CollectionUtils.Map<object, TItem>(this.Selection.Items,
						delegate(object item) { return (TItem)item; });
				}
			}

            public void RegisterDropHandler(Type folderClass, IDropHandler<TItem> dropHandler)
            {
                base.RegisterDropHandler(folderClass, dropHandler);
            }
		}

		#endregion

		#region WorkflowFolderToolContext class
		
		protected class WorkflowFolderToolContext : IWorkflowFolderToolContext
		{
			private readonly WorkflowFolderSystem _owner;

			public WorkflowFolderToolContext(WorkflowFolderSystem owner)
			{
				_owner = owner;
			}

			public IEnumerable<IFolder> Folders
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
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="folderExplorer"></param>
		protected WorkflowFolderSystem(string title, IFolderExplorerToolContext folderExplorer)
			:base(title, folderExplorer)
		{
		}

		#region Protected overrides

		/// <summary>
		/// Called to instantiate the tool set for tools that operate on items.
		/// </summary>
		/// <returns></returns>
		protected override IToolSet CreateItemToolSet()
		{
			return new ToolSet(new TItemToolExtensionPoint(), CreateItemToolContext());
		}

		/// <summary>
		/// Called to instantiate the tool set for tools that operate on folders.
		/// </summary>
		/// <returns></returns>
		protected override IToolSet CreateFolderToolSet()
		{
			return new ToolSet(new TFolderToolExtensionPoint(), CreateFolderToolContext());
		}

		/// <summary>
		/// Called to allow a subclass to select a drop handler from the list of registered drop handlers, for
		/// a given set of items.
		/// </summary>
		/// <param name="handlers"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		protected override object SelectDropHandler(IList handlers, object[] items)
        {
            // cast items to type safe collection, cannot accept drop if items contains a different item type 
            ICollection<TItem> dropItems = new List<TItem>();
            foreach (object item in items)
            {
                if (item is TItem)
                    dropItems.Add((TItem)item);
                else
                    return null;
            }

            // check for a handler that can accept
            return CollectionUtils.SelectFirst<IDropHandler<TItem>>(handlers,
                delegate(IDropHandler<TItem> handler)
                {
                    return handler.CanAcceptDrop(dropItems);
                });
        }

		/// <summary>
		/// Called whenever the selection changes, to obtain the operation enablement for a given selection.
		/// </summary>
		/// <param name="selection"></param>
		/// <returns></returns>
		protected override IDictionary<string, bool> QueryOperationEnablement(ISelection selection)
		{
			TItem item = (TItem)selection.Item;
			Dictionary<string, bool> enablement = new Dictionary<string, bool>();

			// query all registered workflow service for operation enablement
			foreach (Type serviceContract in GetRegisteredWorkflowServices())
			{
				if (typeof(IWorkflowService<TItem>).IsAssignableFrom(serviceContract))
				{
					IWorkflowService<TItem> service = (IWorkflowService<TItem>)Platform.GetService(serviceContract);
					GetOperationEnablementResponse response = service.GetOperationEnablement(
						new GetOperationEnablementRequest<TItem>(item));

					// add fully qualified operation name to dictionary
					CollectionUtils.ForEach(response.OperationEnablementDictionary,
						delegate(KeyValuePair<string, bool> kvp)
						{
							enablement.Add(string.Format("{0}.{1}", serviceContract.FullName, kvp.Key), kvp.Value);
						});

					// ensure to dispose of service
					if (service is IDisposable)
						(service as IDisposable).Dispose();
				}
				else
				{
					Platform.Log(LogLevel.Error, "Can not use service {0} to obtain operation enablement because its does not extend {1}",
						serviceContract.FullName, typeof(IWorkflowService<TItem>).FullName);
				}
			}

			return enablement;
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Called once to instantiate the item tool context.
		/// </summary>
		/// <returns></returns>
		protected abstract IWorkflowItemToolContext CreateItemToolContext();

		/// <summary>
		/// Called once to instantiate the folder tool context.
		/// </summary>
		/// <returns></returns>
		protected abstract IWorkflowFolderToolContext CreateFolderToolContext();

		#endregion
    }

	/// <summary>
	/// Abstract base class for folder systems that consist of <see cref="WorklistFolder{TItem,TWorklistService}"/>s.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="TFolderExtensionPoint"></typeparam>
	/// <typeparam name="TFolderToolExtensionPoint"></typeparam>
	/// <typeparam name="TItemToolExtensionPoint"></typeparam>
	/// <typeparam name="TWorklistService"></typeparam>
	public abstract class WorklistFolderSystem<TItem, TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint, TWorklistService>
		: WorkflowFolderSystem<TItem, TFolderToolExtensionPoint, TItemToolExtensionPoint>
		where TItem : DataContractBase
		where TFolderExtensionPoint : ExtensionPoint<IWorklistFolder>, new()
		where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TWorklistService : IWorklistService<TItem>, IWorkflowService<TItem>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="folderExplorer"></param>
		protected WorklistFolderSystem(string title, IFolderExplorerToolContext folderExplorer)
			: base(title, folderExplorer)
		{
			InitializeFolders(new TFolderExtensionPoint());
		}

		/// <summary>
		/// Called to obtain the set of worklists for the current user.  May be overridden, but typically not necessary.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		protected virtual ListWorklistsForUserResponse QueryWorklistSet(ListWorklistsForUserRequest request)
		{
			ListWorklistsForUserResponse response = null;
			Platform.GetService<TWorklistService>(
				delegate(TWorklistService service)
				{
					response = service.ListWorklistsForUser(request);
				});

			return response;
		}

		/// <summary>
		/// Creates the folder system based on the specified extension point.
		/// </summary>
		/// <param name="folderExtensionPoint"></param>
		private void InitializeFolders(ExtensionPoint<IWorklistFolder> folderExtensionPoint)
		{
			Dictionary<string, Type> mapWorklistClassToFolderClass = new Dictionary<string, Type>();

			// collect worklist class names, and add unfiltered folders if authorized
			foreach (IWorklistFolder folder in folderExtensionPoint.CreateExtensions())
			{
				if (!string.IsNullOrEmpty(folder.WorklistClassName))
					mapWorklistClassToFolderClass.Add(folder.WorklistClassName, folder.GetType());

				// if unfiltered folders are visible, add the root folder
				if (Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Development.ViewUnfilteredWorkflowFolders))
				{
					this.Folders.Add(folder);
				}

			}

			if (mapWorklistClassToFolderClass.Keys.Count > 0)
			{
				ListWorklistsForUserResponse response = QueryWorklistSet(new ListWorklistsForUserRequest(new List<string>(mapWorklistClassToFolderClass.Keys)));
				foreach (WorklistSummary summary in response.Worklists)
				{
					try
					{
						Type folderClass = mapWorklistClassToFolderClass[summary.ClassName];
						IWorklistFolder folder = (IWorklistFolder)folderExtensionPoint.CreateExtension(new ClassNameExtensionFilter(folderClass.FullName));
						if(folder is IInitializeWorklistFolder)
						{
							IInitializeWorklistFolder initFolder = folder as IInitializeWorklistFolder;

							// augment default base path with worklist name
							Path path = folder.FolderPath;
							if (!string.IsNullOrEmpty(summary.DisplayName))
							{
								path = new Path(string.Concat(path.ToString(), "/", summary.DisplayName), folder.ResourceResolver);
							}

							initFolder.Initialize(path, summary.WorklistRef, summary.Description, false);
						}

						this.Folders.Add(folder);
					}
					catch (KeyNotFoundException e)
					{
						Platform.Log(LogLevel.Error, e, string.Format("Worklist class {0} is not mapped to a folder class.", summary.ClassName));
					}
				}
			}
		}
	}
}
