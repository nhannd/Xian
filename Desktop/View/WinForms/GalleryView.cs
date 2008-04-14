using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class GalleryView : UserControl
	{
		private IBindingList _gallery;
		private ActionModelNode _toolbarModel;
		private ActionModelNode _contextMenuModel;
		private event EventHandler _selectionChanged;
		private bool _suppressGalleryChangeEvents = false;
		private bool _dragOutside = false;
		private ListViewItem _draggedItem;

		public GalleryView()
		{
			InitializeComponent();

			_listView.LargeImageList = new ImageList();
			_listView.LargeImageList.ImageSize = new Size(100, 100);
			_listView.View = System.Windows.Forms.View.LargeIcon;
			_listView.BackColor = Color.Black;
			_listView.ForeColor = Color.WhiteSmoke;
			_listView.ListViewItemSorter = new ListViewIndexComparer();
			_listView.ItemSelectionChanged += OnSelectionChanged;
			_listView.ItemDrag += OnItemDrag;
			_listView.DragEnter += OnItemDragEnter;
			_listView.DragOver += OnItemDragOver;
			_listView.DragLeave += OnItemDragLeave;
			_listView.DragDrop += OnItemDragDrop;

			_toolStrip.Visible = false;
		}

		public object DataSource
		{
			get
			{
				return _gallery;
			}
			set
			{
				if (_gallery != value)
				{
					_gallery = value as IBindingList;

					if (_gallery == null)
						throw new Exception("DataSource must be an IBindingList of IGalleryItem objects.");

					InitializeBindings();
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ActionModelNode ToolbarModel
		{
			get { return _toolbarModel; }
			set
			{
				_toolbarModel = value;

				// Turn on the toolbar only if a toolbar model exists
				_toolStrip.Visible = true;

				InitializeToolStrip();
			}
		}


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ActionModelNode ContextMenuModel
		{
			get { return _contextMenuModel; }
			set
			{
				_contextMenuModel = value;
				ToolStripBuilder.Clear(_contextMenu.Items);
				if (_contextMenuModel != null)
				{
					ToolStripBuilder.BuildMenu(_contextMenu.Items, _contextMenuModel.ChildNodes);
				}
			}
		}

		public ISelection Selection
		{
			get
			{
				List<object> selectedItems = new List<object>();

				//when an item is removed from the list view, the 'selection changed' event fires
				//before the item is removed, so the indices are out of sync and we can't rely on them
				foreach (int index in _listView.SelectedIndices)
				{
					object item = _listView.Items[index].Tag;
					selectedItems.Add(CollectionUtils.SelectFirst(_gallery, 
										delegate(object test) { return ((IGalleryItem) test).Item == item;  }));
				}

				return new Selection(selectedItems);
			}
		}

		public event EventHandler SelectionChanged
		{
			add { _selectionChanged += value; }
			remove { _selectionChanged -= value; }
		}

		public bool DragReorder
		{
			get { return _listView.AllowDrop; }
			set { _listView.AllowDrop = value; }
		}

		public bool DragOutside
		{
			get { return _dragOutside; }
			set { _dragOutside = value; }
		}

		public bool MultiSelect
		{
			get { return _listView.MultiSelect; }
			set { _listView.MultiSelect = value; }
		}

		private void InitializeToolStrip()
		{
			ToolStripBuilder.Clear(_toolStrip.Items);

			if (_toolbarModel != null)
			{
				ToolStripBuilder.BuildToolbar(
					_toolStrip.Items,
					_toolbarModel.ChildNodes,
					ToolStripItemDisplayStyle.Image);
			}
		}

		private void InitializeBindings()
		{
			_gallery.ListChanged += OnListChanged;

			_listView.Items.Clear();

			foreach (object item in _gallery)
				AddItem(item);
		}

		private void OnListChanged(object sender, ListChangedEventArgs e)
		{
			if (_suppressGalleryChangeEvents)
				return;

			if (e.ListChangedType == ListChangedType.ItemAdded)
				AddItem(_gallery[e.NewIndex]);
			else if (e.ListChangedType == ListChangedType.ItemDeleted)
				RemoveItem(e.NewIndex);
		}

		private void AddItem(object item)
		{
			IGalleryItem galleryItem = CastToGalleryItem(item);

			string imageKey = Guid.NewGuid().ToString();
			_listView.LargeImageList.Images.Add(imageKey, galleryItem.Image);
			ListViewItem lvi = new ListViewItem(galleryItem.Description, imageKey);
			lvi.Tag = galleryItem.Item;
			_listView.Items.Add(lvi);
		}

		private void RemoveItem(int index)
		{
			ListViewItem lvi = _listView.Items[index];
			_listView.LargeImageList.Images.RemoveByKey(lvi.ImageKey);
			_listView.Items.RemoveAt(index);

			if (_listView.Items.Count > 0)
			{
				int i = Math.Min(index, _listView.Items.Count - 1);
				_listView.Items[i].Selected = true;
			}
		}

		private IGalleryItem CastToGalleryItem(object item)
		{
			IGalleryItem galleryItem = item as IGalleryItem;

			if (galleryItem == null)
				throw new InvalidCastException("DataSource must be an IBindingList of IGalleryItem objects.");

			return galleryItem;
		}

		private void OnSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
		}

		private void OnItemDrag(object sender, ItemDragEventArgs e)
		{
			// Only allow dragging of one item at a time, so deselect all other items
			foreach (ListViewItem lvi in _listView.Items)
			{
				if (lvi != e.Item)
					lvi.Selected = false;
			}

			_draggedItem = e.Item as ListViewItem;

			if (_dragOutside)
				_listView.DoDragDrop(_draggedItem.Tag, DragDropEffects.Move);
			else
				_listView.DoDragDrop(_draggedItem, DragDropEffects.Move);
		}

		private void OnItemDragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.AllowedEffect;
		}

		private void OnItemDragOver(object sender, DragEventArgs e)
		{
			// Retrieve the client coordinates of the mouse pointer.
			Point targetPoint =
				_listView.PointToClient(new Point(e.X, e.Y));

			// Retrieve the index of the item closest to the mouse pointer.
			int targetIndex = _listView.InsertionMark.NearestIndex(targetPoint);

			// Confirm that the mouse pointer is not over the dragged item.
			if (targetIndex > -1)
			{
				// Determine whether the mouse pointer is to the left or
				// the right of the midpoint of the closest item and set
				// the InsertionMark.AppearsAfterItem property accordingly.
				Rectangle itemBounds = _listView.GetItemRect(targetIndex);
				if (targetPoint.X > itemBounds.Left + (itemBounds.Width / 2))
				{
					_listView.InsertionMark.AppearsAfterItem = true;
				}
				else
				{
					_listView.InsertionMark.AppearsAfterItem = false;
				}
			}

			// Set the location of the insertion mark. If the mouse is
			// over the dragged item, the targetIndex value is -1 and
			// the insertion mark disappears.
			_listView.InsertionMark.Index = targetIndex;
		}

		private void OnItemDragLeave(object sender, EventArgs e)
		{
			_listView.InsertionMark.Index = -1;
		}

		private void OnItemDragDrop(object sender, DragEventArgs e)
		{
			// Retrieve the index of the insertion mark;
			int targetIndex = _listView.InsertionMark.Index;

			// If the insertion mark is not visible, exit the method.
			if (targetIndex == -1)
			{
				return;
			}

			// If the insertion mark is to the right of the item with
			// the corresponding index, increment the target index.
			if (_listView.InsertionMark.AppearsAfterItem)
			{
				targetIndex++;
			}

			// Retrieve the dragged item.
			//ListViewItem draggedItem =
			//	(ListViewItem)e.Data.GetData(typeof(ListViewItem));

			int draggedIndex = _draggedItem.Index;

			// Insert a copy of the dragged item at the target index.
			// A copy must be inserted before the original item is removed
			// to preserve item index values. 
			_listView.Items.Insert(
				targetIndex, (ListViewItem)_draggedItem.Clone());

			// Remove the original copy of the dragged item.
			_listView.Items.Remove(_draggedItem);

			_suppressGalleryChangeEvents = true;

			// Move the corresponding data object by inserting the dragged item
			_gallery.Insert(targetIndex, _gallery[draggedIndex]);

			// then remove the original dragged item.
			if (draggedIndex < targetIndex)
				_gallery.RemoveAt(draggedIndex);
			else 
				_gallery.RemoveAt(draggedIndex + 1);

			_suppressGalleryChangeEvents = false;
		}

		// Sorts ListViewItem objects by index.
		private class ListViewIndexComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				return ((ListViewItem)x).Index - ((ListViewItem)y).Index;
			}
		}

	}
}
