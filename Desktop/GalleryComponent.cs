using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
	[ExtensionPoint]
	public class GalleryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof (GalleryComponentViewExtensionPoint))]
	public class GalleryComponent : ApplicationComponent
	{
		private const string _msgNullDataSource = "No data source is specified.";
		private const string _msgItemNotInDataSource = "The item is not in the data source.";
		private const string _msgItemsNotInDataSource = "One or more items are not in the data source.";

		private IBindingList _dataSource;
		private IToolSet _toolSet;
		private ActionModelNode _menuModel;
		private ActionModelNode _toolbarModel;

		private GalleryItemEventHandler _itemActivated;
		private EventHandler _selectionChanged;
		private EventHandler _dataSourceChanged;

		private ISelection _selection = new Selection();
		private ISelection _dataSelection = new Selection();
		private bool _multiSelect = true;
		private bool _showDescription = false;
		private bool _hideSelection = true;
		private Size _imageSize = new Size(100, 100);
		private int _maxDescriptionLines = 0;
		private bool _allowRenaming = false;

		public GalleryComponent() : this(null, null, null)
		{
		}

		public GalleryComponent(IBindingList dataSource) : this(dataSource, null, null)
		{
		}

		public GalleryComponent(string toolbarSite, string contextMenuSite) : this(null, toolbarSite, contextMenuSite)
		{
		}

		public GalleryComponent(IBindingList dataSource, string toolbarSite, string contextMenuSite)
		{
			_dataSource = dataSource;

			if (toolbarSite != null || contextMenuSite != null)
			{
				GalleryToolExtensionPoint xp = new GalleryToolExtensionPoint();
				ToolContext context = new ToolContext(this);
				ToolSet ts = new ToolSet(xp, context);

				if (contextMenuSite != null)
					_menuModel = ActionModelRoot.CreateModel(typeof (GalleryComponent).FullName, contextMenuSite, ts.Actions);
				if (toolbarSite != null)
					_toolbarModel = ActionModelRoot.CreateModel(typeof (GalleryComponent).FullName, toolbarSite, ts.Actions);

				_toolSet = ts;
			}
		}

		#region Tools, Actions, and Models

		#region Context Class

		private class ToolContext : IGalleryToolContext
		{
			private GalleryComponent _component;
			private event EventHandler _selectionChanged;
			private event GalleryItemEventHandler _itemActivated;

			public ToolContext(GalleryComponent component)
			{
				_component = component;
				_component.SelectionChanged += FireSelectionChanged;
				_component.ItemActivated += FireItemActivated;
			}

			~ToolContext()
			{
				_component.SelectionChanged -= FireSelectionChanged;
				_component.ItemActivated -= FireItemActivated;
				_component = null;
			}

			private void FireItemActivated(object sender, GalleryItemEventArgs e) {
				if (_itemActivated != null)
					_itemActivated(this, new GalleryItemEventArgs(e.Item));
			}

			private void FireSelectionChanged(object sender, EventArgs e) {
				if (_selectionChanged != null)
					_selectionChanged(this, new EventArgs());
			}

			public event EventHandler SelectionChanged
			{
				add { _selectionChanged += value; }
				remove { _selectionChanged -= value; }
			}

			public event GalleryItemEventHandler ItemActivated
			{
				add { _itemActivated += value; }
				remove { _itemActivated -= value; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public IBindingList DataSource
			{
				get { return _component.DataSource; }
			}

			public ISelection Selection
			{
				get { return _component.Selection; }
			}

			public ISelection SelectedData
			{
				get { return _component.SelectedData; }
			}

			public void Activate(IGalleryItem item)
			{
				_component.Activate(item);
			}

			public void Select(IEnumerable<IGalleryItem> selection)
			{
				_component.Select(selection);
			}

			public void Select(IGalleryItem item)
			{
				_component.Select(item);
			}
		}

		#endregion

		protected IToolSet ToolSet
		{
			get { return _toolSet; }
		}

		public ActionModelNode MenuModel
		{
			get { return _menuModel; }
			set { _menuModel = value; }
		}

		public ActionModelNode ToolbarModel
		{
			get { return _toolbarModel; }
			set { _toolbarModel = value; }
		}

		#endregion

		#region Data Source

		public IBindingList DataSource
		{
			get { return _dataSource; }
			set
			{
				if (_dataSource != value)
				{
					_dataSource = value;
					EventsHelper.Fire(_dataSourceChanged, this, new EventArgs());
					base.Modified = true;
					base.NotifyPropertyChanged("DataSource");
				}
			}
		}

		public event EventHandler DataSourceChanged
		{
			add { _dataSourceChanged += value; }
			remove { _dataSourceChanged -= value; }
		}

		#endregion

		#region Selection

		public virtual bool MultiSelect
		{
			get { return _multiSelect; }
			set { _multiSelect = value; }
		}

		public ISelection Selection
		{
			get { return _selection; }
		}

		public ISelection SelectedData
		{
			get { return _dataSelection; }
		}

		public event EventHandler SelectionChanged
		{
			add { _selectionChanged += value; }
			remove { _selectionChanged -= value; }
		}

		public event GalleryItemEventHandler ItemActivated
		{
			add { _itemActivated += value; }
			remove { _itemActivated -= value; }
		}

		public void Select(IEnumerable<IGalleryItem> selection)
		{
			Platform.CheckForNullReference(selection, "selection");
			if (_dataSource == null)
				throw new InvalidOperationException(_msgNullDataSource);

			List<IGalleryItem> list = new List<IGalleryItem>();
			List<object> data = new List<object>();
			foreach (IGalleryItem item in selection)
			{
				if (!_dataSource.Contains(item))
					throw new ArgumentException(_msgItemsNotInDataSource, "selection");
				list.Add(item);
				data.Add(item.Item);
			}
			_selection = new Selection(list);
			_dataSelection = new Selection(data);
			NotifySelectionChanged();
		}

		public void Select(IGalleryItem item)
		{
			if (_dataSource == null)
				throw new InvalidOperationException(_msgNullDataSource);
			if (!_dataSource.Contains(item))
				throw new ArgumentException(_msgItemNotInDataSource, "item");

			_selection = new Selection(item);
			_dataSelection = new Selection(item.Item);
			NotifySelectionChanged();
		}

		public void Activate(IGalleryItem item)
		{
			if (_dataSource == null)
				throw new InvalidOperationException(_msgNullDataSource);
			if (!_dataSource.Contains(item))
				throw new ArgumentException(_msgItemNotInDataSource, "item");

			NotifyItemActivated(item);
		}

		private void NotifySelectionChanged()
		{
			EventsHelper.Fire(_selectionChanged, this, new EventArgs());
		}

		private void NotifyItemActivated(IGalleryItem item)
		{
			EventsHelper.Fire(_itemActivated, this, new GalleryItemEventArgs(item));
		}

		#endregion

		#region Display Options

		public virtual bool HideSelection
		{
			get { return _hideSelection; }
			set { _hideSelection = value; }
		}

		public virtual bool ShowDescription
		{
			get { return _showDescription; }
			set { _showDescription = value; }
		}

		public virtual int MaxDescriptionLines
		{
			get { return _maxDescriptionLines; }
			set { _maxDescriptionLines = value; }
		}

		public virtual Size ImageSize
		{
			get { return _imageSize; }
			set { _imageSize = value; }
		}

		public virtual bool AllowRenaming
		{
			get { return _allowRenaming; }
			set { _allowRenaming = value; }
		}

		#endregion

		#region Drag Drop Functionality

		public virtual bool AllowsDropOnItem
		{
			get { return false; }
		}

		public virtual bool AllowsDropAtIndex
		{
			get { return false; }
		}

		/// <summary>
		/// Signals the component that a drag &amp; drop operation involving the specified
		/// <see cref="IGalleryItem"/>s has started on the associated view.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="draggedItems">The <see cref="IGalleryItem"/>s being dragged.</param>
		public virtual DragDropOption BeginDrag(IList<IGalleryItem> draggedItems)
		{
			return DragDropOption.Copy;
		}

		/// <summary>
		/// Signals the component that a drag &amp; drop operation involving the specified
		/// <see cref="IGalleryItem"/>s has ended with the given action being taken on the items by the drop target.
		/// </summary>
		/// <param name="draggedItems">The <see cref="IGalleryItem"/>s that were dragged.</param>
		/// <param name="action">The <see cref="DragDropOption"/> action that was taken on the items by the drop target.</param>
		public virtual void EndDrag(IList<IGalleryItem> draggedItems, DragDropOption action)
		{
		}

		/// <summary>
		/// Checks for allowed drag &amp; drop actions involving the specified foreign data and the given target on this component.
		/// </summary>
		/// <param name="droppingData">The <see cref="IDragDropObject"/> object that encapsulates all forms of the foreign data.</param>
		/// <param name="targetIndex">The target index that the user is trying to drop at.</param>
		/// <param name="actions"></param>
		/// <param name="modifiers">The modifier keys that are being held by the user.</param>
		/// <returns>The allowed <see cref="DragDropKind"/> actions for this attempted drag &amp; drop operation.</returns>
		public virtual DragDropOption CheckDrop(IDragDropObject droppingData, int targetIndex, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// Checks for allowed drag &amp; drop actions involving the specified foreign data and the given target on this component.
		/// </summary>
		/// <param name="droppingData">The <see cref="IDragDropObject"/> object that encapsulates all forms of the foreign data.</param>
		/// <param name="targetItem">The target item that the user is trying to drop on to.</param>
		/// <param name="actions"></param>
		/// <param name="modifiers">The modifier keys that are being held by the user.</param>
		/// <returns>The allowed <see cref="DragDropKind"/> action for this attempted drag &amp; drop operation.</returns>
		public virtual DragDropOption CheckDrop(IDragDropObject droppingData, IGalleryItem targetItem, DragDropOption actions, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// This method or <see cref="PerformDrop(IDragDropObject,IGalleryItem,DragDropOption,ModifierFlags)"/> may be called
		/// additional times if the returned action is <see cref="DragDropOption.None"/> in order to attempt other ways to drop the item in
		/// an acceptable manner. It is thus very important that the result be set properly if the drop was accepted and no further attempts
		/// should be made.
		/// </remarks>
		/// <param name="droppedData"></param>
		/// <param name="targetIndex"></param>
		/// <param name="action"></param>
		/// <param name="modifiers"></param>
		/// <returns></returns>
		public virtual DragDropOption PerformDrop(IDragDropObject droppedData, int targetIndex, DragDropOption action, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="droppedData"></param>
		/// <param name="targetItem"></param>
		/// <param name="action"></param>
		/// <param name="modifiers"></param>
		/// <returns></returns>
		public virtual DragDropOption PerformDrop(IDragDropObject droppedData, IGalleryItem targetItem, DragDropOption action, ModifierFlags modifiers)
		{
			return DragDropOption.None;
		}

		#endregion
	}
}