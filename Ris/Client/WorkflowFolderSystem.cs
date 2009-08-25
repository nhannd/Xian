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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

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

			public void InvalidateFolders()
			{
				_owner.InvalidateFolders();
			}

			public void InvalidateSelectedFolder()
			{
				_owner.InvalidateSelectedFolder();
			}

			public void InvalidateFolders(Type folderClass)
			{
				_owner.InvalidateFolders(folderClass);
			}

			public void RegisterDoubleClickHandler(IClickAction clickAction)
			{
				Platform.CheckForNullReference(clickAction, "clickAction");
				_owner._doubleClickHandlers.Add(new DoubleClickHandlerRegistration(clickAction));
			}

			public void UnregisterDoubleClickHandler(IClickAction clickAction)
			{
				if (clickAction == null)
					return;

				foreach (DoubleClickHandlerRegistration handler in _owner._doubleClickHandlers)
				{
					if (handler.ClickAction.ActionID == clickAction.ActionID)
					{
						_owner._doubleClickHandlers.Remove(handler);
						return;
					}
				}
			}

			protected void RegisterDropHandler(Type folderClass, object dropHandler)
			{
				_owner.RegisterDropHandler(folderClass, dropHandler);
			}

			protected void UnregisterDropHandler(Type folderClass, object dropHandler)
			{
				_owner.UnregisterDropHandler(folderClass, dropHandler);
			}

			public void RegisterWorkflowService(Type serviceContract)
			{
				_owner.RegisterWorkflowService(serviceContract);
			}
		}

		#endregion

		#region DoubleClickHandlerRegistration

		class DoubleClickHandlerRegistration
		{
			private readonly IClickAction _clickAction;

			public DoubleClickHandlerRegistration(IClickAction clickAction)
			{
				_clickAction = clickAction;
			}

			public IClickAction ClickAction
			{
				get { return _clickAction; }
			}

			public bool Handle()
			{
				if (_clickAction.Permissible && _clickAction.Enabled)
				{
					_clickAction.Click();
					return true;
				}
				return false;
			}
		}

		#endregion

		private readonly FolderList _workflowFolders;
		private IFolderSystemContext _context;

		private event EventHandler _selectedItemsChanged;
		private event EventHandler _titleChanged;
		private event EventHandler _titleIconChanged;
		private event EventHandler _foldersChanged;
		private event EventHandler _foldersInvalidated;


		private string _title;
		private IconSet _titleIcon;
		private IResourceResolver _resourceResolver;

		private IToolSet _itemTools;
		private IToolSet _folderTools;

		private readonly List<Type> _workflowServices = new List<Type>();
		private IDictionary<string, bool> _workflowEnablement;
		private readonly Dictionary<Type, List<object>> _mapFolderClassToDropHandlers = new Dictionary<Type, List<object>>();
		private readonly List<DoubleClickHandlerRegistration> _doubleClickHandlers = new List<DoubleClickHandlerRegistration>();

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="title">Initial title of the folder system.</param>
		protected WorkflowFolderSystem(string title)
		{
			_title = title;

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

		public void SetContext(IFolderSystemContext context)
		{
			_context = context;
			_context.SelectedItemsChanged += SelectedItemsChangedEventHandler;
			_context.SelectedItemDoubleClicked += SelectedItemDoubleClickedEventHandler;
		}

		public virtual void Initialize()
		{
			// nothing to do
		}

		public virtual bool LazyInitialize
		{
			get
			{
				// lazy initialize by default
				return true;
			}
		}

		public IDesktopWindow DesktopWindow
		{
			get { return _context.DesktopWindow; }
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

		public abstract string GetPreviewUrl(IFolder folder, object[] items);


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

		public event EventHandler FoldersInvalidated
		{
			add { _foldersInvalidated += value; }
			remove { _foldersInvalidated -= value; }
		}

		/// <summary>
		/// Gets a value indicating whether this folder system supports searching.
		/// </summary>
		public virtual bool SearchEnabled
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this folder system supports advanced searching.
		/// </summary>
		public virtual bool AdvancedSearchEnabled
		{
			get { return true; }
		}

		public virtual string SearchMessage
		{
			get { return SR.MessageSearchMessageDefault; }
		}

		/// <summary>
		/// Performs a search, if supported.
		/// </summary>
		/// <param name="params"></param>
		public virtual void ExecuteSearch(SearchParams @params)
		{
		}

		public abstract SearchParams CreateSearchParams(string searchText);
		public abstract void LaunchSearchComponent();

		/// <summary>
		/// Invalidates all folders.
		/// </summary>
		public void InvalidateFolders()
		{
			InvalidateFolders(delegate { return true; });
		}

		/// <summary>
		/// Invalidates all folders of the specified class.
		/// </summary>
		public void InvalidateFolders(Type folderClass)
		{
			InvalidateFolders(delegate(IFolder f) { return folderClass.IsAssignableFrom(f.GetType()); });
		}

		/// <summary>
		/// Invalidates the currently selected folder.
		/// </summary>
		public void InvalidateSelectedFolder()
		{
			if (this.SelectedFolder != null)
				InvalidateFolders(delegate(IFolder f) { return f == this.SelectedFolder; });
		}

		/// <summary>
		/// Invalidates the specified folder.
		/// </summary>
		/// <param name="folder"></param>
		public void InvalidateFolder(IFolder folder)
		{
			InvalidateFolders(delegate(IFolder f) { return f == folder; });
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
		/// Unregisters the specified drop handler for the specified folder class.
		/// </summary>
		/// <param name="folderClass"></param>
		/// <param name="dropHandler"></param>
		protected void UnregisterDropHandler(Type folderClass, object dropHandler)
		{
			List<object> handlers;
			if (!_mapFolderClassToDropHandlers.TryGetValue(folderClass, out handlers))
				_mapFolderClassToDropHandlers.Add(folderClass, handlers = new List<object>());
			handlers.Remove(dropHandler);
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

				if (_itemTools != null)
				{
					_itemTools.Dispose();
					_itemTools = null;
				}
				if (_folderTools != null)
				{
					_folderTools.Dispose();
					_folderTools = null;
				}
			}
		}

		/// <summary>
		/// Gets the current selection of items.
		/// </summary>
		protected internal ISelection Selection
		{
			get { return _context.SelectedItems; }
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
			get { return _context.SelectedFolder; }
			set { _context.SelectedFolder = value; }
		}

		/// <summary>
		/// Occurs when the selected folder changes.
		/// </summary>
		protected internal event EventHandler SelectedFolderChanged
		{
			add { _context.SelectedFolderChanged += value; }
			remove { _context.SelectedFolderChanged -= value; }
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

		/// <summary>
		/// Notifies that one or more folders has been invalidated.
		/// </summary>
		protected void NotifyFoldersInvalidated()
		{
			EventsHelper.Fire(_foldersInvalidated, this, EventArgs.Empty);
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
			if (!_mapFolderClassToDropHandlers.TryGetValue(folder.GetType(), out handlers))
				handlers = new List<object>();

			return SelectDropHandler(handlers, items);
		}

		/// <summary>
		/// Handles the <see cref="IFolderSystemContext.SelectedItemsChanged"/> event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SelectedItemsChangedEventHandler(object sender, EventArgs args)
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

		/// <summary>
		/// Handles the <see cref="IFolderSystemContext.SelectedItemDoubleClicked"/> event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void SelectedItemDoubleClickedEventHandler(object sender, EventArgs args)
		{
			// find a double-click handler
			foreach (DoubleClickHandlerRegistration handler in _doubleClickHandlers)
			{
				if (handler.Handle())
					break;
			}
		}

		/// <summary>
		/// Registers the specified workflow service.
		/// </summary>
		/// <param name="workflowService"></param>
		private void RegisterWorkflowService(Type workflowService)
		{
			if (!_workflowServices.Contains(workflowService))
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

		/// <summary>
		/// Invalidates any folders matching the specified condition.
		/// </summary>
		/// <param name="condition"></param>
		private void InvalidateFolders(Predicate<IFolder> condition)
		{
			int count = 0;
			foreach (IFolder folder in _workflowFolders)
			{
				if (condition(folder))
				{
					folder.Invalidate();
					count++;
				}
			}

			if (count > 0)
				NotifyFoldersInvalidated();
		}

		#endregion

	}

	public abstract class WorkflowFolderSystem<TSearchParams> : WorkflowFolderSystem
		where TSearchParams : SearchParams
	{
		private SearchResultsFolder<TSearchParams> _searchFolder;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="title"></param>
		protected WorkflowFolderSystem(string title)
			: base(title)
		{
		}

		/// <summary>
		/// Performs a search, if supported.
		/// </summary>
		/// <param name="params"></param>
		public override void ExecuteSearch(SearchParams @params)
		{
			if (this.SearchResultsFolder != null)
			{
				this.SearchResultsFolder.SearchParams = @params as TSearchParams; 

				// ensure the results folder is selected, and force an immediate update
				this.SelectedFolder = this.SearchResultsFolder;
				this.SearchResultsFolder.Update();
			}
		}

		/// <summary>
		/// Gets the search-results folder, or null if this system does not support searches.
		/// </summary>
		protected SearchResultsFolder<TSearchParams> SearchResultsFolder
		{
			get
			{
				if (_searchFolder == null)
				{
					_searchFolder = (SearchResultsFolder<TSearchParams>)CreateSearchResultsFolder();
					this.Folders.Add(_searchFolder);
				}
				return _searchFolder;
			}
		}
	}

	/// <summary>
	/// Abstract base class for workflow folder systems.
	/// </summary>
	/// <typeparam name="TItem">The class of item that the folders contain.</typeparam>
	/// <typeparam name="TFolderToolExtensionPoint">Extension point that defines the set of folder tools for this folders system.</typeparam>
	/// <typeparam name="TItemToolExtensionPoint">Extension point that defines the set of item tools for this folders system.</typeparam>
	/// <typeparam name="TSearchParams"></typeparam>
	public abstract class WorkflowFolderSystem<TItem, TFolderToolExtensionPoint, TItemToolExtensionPoint, TSearchParams> : WorkflowFolderSystem<TSearchParams>
		where TItem : DataContractBase
		where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TSearchParams : SearchParams
	{
		#region WorkflowItemToolContext class

		protected new class WorkflowItemToolContext : WorkflowFolderSystem.WorkflowItemToolContext, IWorkflowItemToolContext<TItem>
		{
			public WorkflowItemToolContext(WorkflowFolderSystem owner)
				: base(owner)
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

			public void UnregisterDropHandler(Type folderClass, IDropHandler<TItem> dropHandler)
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

			public void InvalidateFolders()
			{
				_owner.InvalidateFolders();
			}

			public void InvalidateFolders(Type folderClass)
			{
				_owner.InvalidateFolders(folderClass);
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="title"></param>
		protected WorkflowFolderSystem(string title)
			: base(title)
		{
		}

		#region Overrides

		public override string GetPreviewUrl(IFolder folder, object[] items)
		{
			return GetPreviewUrl((WorkflowFolder)folder, CollectionUtils.Cast<TItem>(items));
		}

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
			Dictionary<string, bool> enablement = new Dictionary<string, bool>();

			// query all registered workflow service for operation enablement
			foreach (Type serviceContract in GetRegisteredWorkflowServices())
			{
				if (typeof(IWorkflowService).IsAssignableFrom(serviceContract))
				{
					IWorkflowService service = (IWorkflowService)Platform.GetService(serviceContract);
					GetOperationEnablementResponse response = service.GetOperationEnablement(
						new GetOperationEnablementRequest(selection.Item));

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
					Platform.Log(LogLevel.Error, "Can not use service {0} to obtain operation enablement because its does not implement {1}",
						serviceContract.FullName, typeof(IWorkflowService).FullName);
				}
			}

			return enablement;
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Called to obtain the preview URL for the specified folder and items.
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="items"></param>
		/// <returns></returns>
		protected abstract string GetPreviewUrl(WorkflowFolder folder, ICollection<TItem> items);

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

}
