using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClearCanvas.Controls.WinForms
{
	public class FolderView : FolderControl
	{
		private event EventHandler _selectedItemsChanged;
		private event FolderViewItemEventHandler _itemDoubleClick;
		private readonly FolderListView _folderListView;
		private IList<FolderViewItem> _selectedItems;

		public FolderView()
		{
			_folderListView = new FolderListView();
			_folderListView.AfterBrowse += OnFolderListViewAfterBrowse;
			_folderListView.SelectedIndexChanged += OnFolderListViewSelectedIndexChanged;
			_folderListView.Dock = DockStyle.Fill;
			_folderListView.View = View.LargeIcon;

			_selectedItems = new List<FolderViewItem>(0).AsReadOnly();

			base.SuspendLayout();
			base.Controls.Add(_folderListView);
			base.ResumeLayout(false);
		}

		#region Designer Properties

		#region AutoArrange

		[DefaultValue(true)]
		public bool AutoArrange
		{
			get { return _folderListView.AutoArrange; }
			set
			{
				if (_folderListView.AutoArrange != value)
				{
					_folderListView.SuspendLayout();
					_folderListView.AutoArrange = value;
					_folderListView.ResumeLayout(true);
					this.OnPropertyChanged(new PropertyChangedEventArgs("AutoArrange"));
				}
			}
		}

		private void ResetAutoArrange()
		{
			this.AutoArrange = true;
		}

		#endregion

		#region AutoDrillDown

		[DefaultValue(true)]
		public bool AutoDrillDown
		{
			get { return _folderListView.AutoDrillDown; }
			set
			{
				if (_folderListView.AutoDrillDown != value)
				{
					_folderListView.AutoDrillDown = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("AutoDrillDown"));
				}
			}
		}

		private void ResetAutoDrillDown()
		{
			this.AutoDrillDown = true;
		}

		#endregion

		#region MultiSelect

		[DefaultValue(true)]
		public bool MultiSelect
		{
			get { return _folderListView.MultiSelect; }
			set
			{
				if (_folderListView.MultiSelect != value)
				{
					_folderListView.MultiSelect = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("MultiSelect"));
				}
			}
		}

		private void ResetMultiSelect()
		{
			this.MultiSelect = true;
		}

		#endregion

		#region View

		public View View
		{
			get { return _folderListView.View; }
			set
			{
				if (_folderListView.View != value)
				{
					_folderListView.SuspendLayout();
					_folderListView.View = value;
					_folderListView.ResumeLayout(true);
					this.OnPropertyChanged(new PropertyChangedEventArgs("View"));
				}
			}
		}

		private void ResetView()
		{
			this.View = View.LargeIcon;
		}

		private bool ShouldSerializeView()
		{
			return this.View != View.LargeIcon;
		}

		#endregion

		#endregion

		#region SelectedItems

		public IList<FolderViewItem> SelectedItems
		{
			get { return _selectedItems; }
			private set
			{
				if (_selectedItems != value)
				{
					_selectedItems = value;
					this.OnSelectedItemsChanged(EventArgs.Empty);
					this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
				}
			}
		}

		public event EventHandler SelectedItemsChanged
		{
			add { _selectedItemsChanged += value; }
			remove { _selectedItemsChanged -= value; }
		}

		protected virtual void OnSelectedItemsChanged(EventArgs e)
		{
			if (_selectedItemsChanged != null)
				_selectedItemsChanged.Invoke(this, e);
		}

		#endregion

		#region ItemDoubleClick

		public event FolderViewItemEventHandler ItemDoubleClick
		{
			add { _itemDoubleClick += value; }
			remove { _itemDoubleClick -= value; }
		}

		protected virtual void OnItemDoubleClick(FolderViewItemEventArgs e)
		{
			if (_itemDoubleClick != null)
				_itemDoubleClick.Invoke(this, e);
		}

		#endregion

		protected override Pidl CurrentPidlCore
		{
			get { return _folderListView.CurrentPidl; }
		}

		protected override void BrowseToCore(Pidl pidl)
		{
			_folderListView.BrowseToPidl(pidl);
		}

		public override void Reload()
		{
			try
			{
				_folderListView.BrowseToPidl(_folderListView.CurrentPidl);
			}
			catch (Exception ex)
			{
				HandleBrowseException(ex);
			}
		}

		private void OnFolderListViewAfterBrowse(object sender, EventArgs e)
		{
			this.OnCurrentPidlChanged(EventArgs.Empty);
			this.NotifyCoordinatorPidlChanged();
		}

		private void OnFolderListViewSelectedIndexChanged(object sender, EventArgs e)
		{
			List<FolderViewItem> selection = new List<FolderViewItem>();
			if (_folderListView.SelectedItems != null)
			{
				foreach (FolderListViewItem item in _folderListView.SelectedItems)
					selection.Add(new FolderViewItem(item.Pidl));
			}
			this.SelectedItems = selection.AsReadOnly();
		}

		#region FolderViewItem Class

		public class FolderViewItem : FolderObject
		{
			internal FolderViewItem(Pidl pidl) : base(pidl) {}
		}

		#endregion

		#region FolderListViewItem Class

		private class FolderListViewItem : ListViewItem, IDisposable
		{
			private ShellItem _shellItem;
			private DateTime _lastModified = DateTime.MinValue;
			private long _fileSize = -1;
			private byte _folderType;

			public FolderListViewItem(ShellItem shellItem) : base()
			{
				string lastModified = string.Empty;
				string fileSize = string.Empty;
				if (!string.IsNullOrEmpty(shellItem.Path) && File.Exists(shellItem.Path))
				{
					FileInfo info = new FileInfo(shellItem.Path);
					_lastModified = info.LastWriteTime;
					lastModified = _lastModified.ToString();
					_fileSize = info.Length;
					fileSize = FormatFileSize(_fileSize);
				}

				_shellItem = shellItem;

				using (Pidl myDocumentsPidl = new Pidl(Environment.SpecialFolder.MyDocuments))
				{
					if (shellItem.Pidl.Equals(myDocumentsPidl))
						_folderType = 0; // force My Documents to the top of the order
					else
						_folderType = DecodeFolderType(shellItem, _fileSize);
				}

				this.Text = _shellItem.DisplayName;
				this.ImageIndex = _shellItem.IconIndex;
				this.SubItems.Add(new ListViewSubItem(this, fileSize, SystemColors.GrayText, this.BackColor, this.Font));
				this.SubItems.Add(new ListViewSubItem(this, _shellItem.TypeName, SystemColors.GrayText, this.BackColor, this.Font));
				this.SubItems.Add(new ListViewSubItem(this, lastModified, SystemColors.GrayText, this.BackColor, this.Font));
			}

			public void Dispose()
			{
				if (_shellItem != null)
				{
					_shellItem.Dispose();
					_shellItem = null;
				}
				GC.SuppressFinalize(this);
			}

			public int CompareTo(FolderListViewItem other, string key)
			{
				switch (key)
				{
					case FolderListView.COLUMN_NAME:
						return string.Compare(_shellItem.DisplayName, other._shellItem.DisplayName, StringComparison.InvariantCultureIgnoreCase);
					case FolderListView.COLUMN_TYPE:
						return string.Compare(_shellItem.TypeName, other._shellItem.TypeName, StringComparison.InvariantCultureIgnoreCase);
					case FolderListView.COLUMN_DATEMODIFIED:
						return DateTime.Compare(_lastModified, other._lastModified);
					case FolderListView.COLUMN_SIZE:
						return _fileSize.CompareTo(other._fileSize);
					case FolderListView.SORT_FOLDERTYPE:
						return _folderType.CompareTo(other._folderType);
					default:
						return 0;
				}
			}

			public Pidl Pidl
			{
				get { return _shellItem.Pidl; }
			}

			public bool IsFolder
			{
				get { return _shellItem.IsFolder; }
			}

			public ShellItem GetShellItemCopy()
			{
				return _shellItem.Clone();
			}

			private static byte DecodeFolderType(ShellItem shellItem, long fileSize)
			{
				if (shellItem.IsVirtual && shellItem.IsFolder)
					return 32; // a special location folder
				else if (shellItem.IsVirtual)
					return 64; // a special item
				else if (shellItem.IsFolder && fileSize < 0)
					return 128; // a real folder, not just a folder-like object
				else
					return byte.MaxValue; // files and any folder-like objects
			}

			private static string FormatFileSize(long fileSize)
			{
				if (fileSize < 0) // file doesn't exist!
					return string.Empty;
				else if (fileSize < 896) // less than 896 bytes
					return string.Format(SR.FormatFileSizeBytes, fileSize);
				else if (fileSize < 917504) // between 896 bytes and 896 KiB
					return string.Format(SR.FormatFileSizeKB, fileSize/1024.0);
				else if (fileSize < 939524096) // between 896 KiB and 896 MiB
					return string.Format(SR.FormatFileSizeMB, fileSize/1048576.0);
				else if (fileSize < 841813590016) // between 896 MiB and 896 GiB
					return string.Format(SR.FormatFileSizeGB, fileSize/1073741824.0);

				// and finally, in the event of having a file greater than 896 GiB...
				return string.Format(SR.FormatFileSizeTB, fileSize/1099511627776.0);
			}
		}

		#endregion

		#region FolderListView Class

		private class FolderListView : ListView
		{
			public event EventHandler AfterBrowse;
			private bool _suppressAfterBrowse = false;

			public const string SORT_FOLDERTYPE = "FolderType";
			public const string COLUMN_NAME = "Name";
			public const string COLUMN_SIZE = "Size";
			public const string COLUMN_TYPE = "Type";
			public const string COLUMN_DATEMODIFIED = "DateModified";

			private const int ColumnLimitInTileView = 1;
			private readonly List<ColumnHeader> _unusedColumnsInTileView = new List<ColumnHeader>();

			private ShellItem _rootShellItem = new ShellItem();
			private ShellItem _currentShellItem = null;

			private bool _autoDrillDown = true;

			public FolderListView() : base()
			{
				_currentShellItem = _rootShellItem.Clone();

				FolderListViewItemComparer comparer = new FolderListViewItemComparer();
				comparer.SortOn(COLUMN_TYPE);
				comparer.SortOn(COLUMN_NAME);

				this.Alignment = ListViewAlignment.SnapToGrid;
				this.AllowColumnReorder = false;
				this.AutoArrange = true;
				this.CheckBoxes = false;
				this.HeaderStyle = ColumnHeaderStyle.Clickable;
				this.HideSelection = false;
				this.LabelEdit = false;
				this.LabelWrap = true;
				this.ListViewItemSorter = comparer;
				this.MultiSelect = true;
				this.Scrollable = true;
				this.Sorting = SortOrder.Ascending;
				this.TileSize = new Size(12*(this.FontHeight + 4), 3*this.FontHeight + 4);
				foreach (ColumnHeader column in CreateDefaultColumns(this.ClientSize.Width))
					this.Columns.Add(column);
				foreach (ShellItem shellItem in _currentShellItem.EnumerateChildren())
					this.Items.Add(new FolderListViewItem(shellItem));
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					DisposeEach(this.Items);

					if (_currentShellItem != null)
					{
						_currentShellItem.Dispose();
						_currentShellItem = null;
					}

					if (_rootShellItem != null)
					{
						_rootShellItem.Dispose();
						_rootShellItem = null;
					}
				}
				base.Dispose(disposing);
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				base.OnHandleCreated(e);

				try
				{
					int hRes = Native.User32.SendMessage(base.Handle, Native.ListView.LVM_SETIMAGELIST, Native.ListView.LVSIL_SMALL, SystemImageList.SmallIcons);
					if (hRes != 0)
						Marshal.ThrowExceptionForHR(hRes);

					hRes = Native.User32.SendMessage(base.Handle, Native.ListView.LVM_SETIMAGELIST, Native.ListView.LVSIL_NORMAL, SystemImageList.LargeIcons);
					if (hRes != 0)
						Marshal.ThrowExceptionForHR(hRes);
				}
				catch (Exception ex)
				{
					((FolderView) this.Parent).HandleInitializationException(ex);
				}
			}

			protected override CreateParams CreateParams
			{
				get
				{
					CreateParams cp = base.CreateParams;
					cp.Style = cp.Style | Native.ListView.LVS_SHAREIMAGELISTS;
					return cp;
				}
			}

			protected override void OnResize(EventArgs e)
			{
				base.OnResize(e);
				this.ResizeColumns(this.ClientSize.Width - 15);
			}

			protected override void OnSizeChanged(EventArgs e)
			{
				base.OnSizeChanged(e);
				this.ResizeColumns(this.ClientSize.Width - 15);
			}

			protected override void OnColumnClick(ColumnClickEventArgs e)
			{
				base.OnColumnClick(e);
				((FolderListViewItemComparer) this.ListViewItemSorter).SortOn(this.Columns[e.Column].Name);
				this.Sort();
			}

			protected override void OnDoubleClick(EventArgs e)
			{
				if (this.SelectedItems != null && this.SelectedItems.Count > 0)
				{
					try
					{
						Point point = this.PointToClient(Cursor.Position);
						FolderListViewItem item = (FolderListViewItem) this.GetItemAt(point.X, point.Y);
						if (item != null)
						{
							FolderView control = base.Parent as FolderView;
							if (control != null)
								control.OnItemDoubleClick(new FolderViewItemEventArgs(new FolderViewItem(item.Pidl)));

							if (_autoDrillDown && item.IsFolder)
							{
								// if the user double clicked on a folder item, perform a drill down
								this.Browse(item.GetShellItemCopy());
							}
						}
					}
					catch (Exception ex)
					{
						HandleBrowseException(ex);
					}
				}
				base.OnDoubleClick(e);
			}

			public Pidl CurrentPidl
			{
				get
				{
					if (_currentShellItem == null)
						return null;
					return _currentShellItem.Pidl;
				}
			}

			public bool AutoDrillDown
			{
				get { return _autoDrillDown; }
				set { _autoDrillDown = value; }
			}

			public new View View
			{
				get { return base.View; }
				set
				{
					if (base.View != value)
					{
						base.View = value;

						if (value != View.Details && base.Columns.Count > ColumnLimitInTileView)
						{
							while (base.Columns.Count > ColumnLimitInTileView)
							{
								_unusedColumnsInTileView.Add(base.Columns[ColumnLimitInTileView]);
								base.Columns.RemoveAt(ColumnLimitInTileView);
							}
						}
						else if (value == View.Details && _unusedColumnsInTileView.Count > 0)
						{
							foreach (ColumnHeader column in _unusedColumnsInTileView)
								base.Columns.Add(column);
							_unusedColumnsInTileView.Clear();
						}
					}
				}
			}

			public void BrowseToPidl(Pidl pidl)
			{
				_suppressAfterBrowse = true;
				try
				{
					this.Browse(new ShellItem(pidl, _rootShellItem, false));
				}
				catch (Exception ex)
				{
					HandleBrowseException(ex);
				}
				finally
				{
					_suppressAfterBrowse = false;
				}
			}

			private void Browse(ShellItem destination)
			{
				if (_currentShellItem != destination)
				{
					this.SuspendLayout();
					try
					{
						DisposeEach(this.Items);
						this.Items.Clear();

						if (_currentShellItem != null)
						{
							_currentShellItem.Dispose();
						}

						_currentShellItem = destination;

						if (_currentShellItem != null)
						{
							foreach (ShellItem shellItem in _currentShellItem.EnumerateChildren())
								this.Items.Add(new FolderListViewItem(shellItem));
						}
					}
					catch (PathNotFoundException ex)
					{
						HandleBrowseException(ex);
					}
					catch (Exception ex)
					{
						HandleBrowseException(new IOException("The specified path is inaccessible.", ex));
					}
					finally
					{
						this.ResumeLayout(true);
					}

					if (!_suppressAfterBrowse && this.AfterBrowse != null)
						this.AfterBrowse.Invoke(this, EventArgs.Empty);
				}
			}

			private void ResizeColumns(int width)
			{
				this.SuspendLayout();

				int totalWidth = 0;
				foreach (ColumnHeader column in this.Columns)
					totalWidth += column.Width;

				if (totalWidth < 200)
				{
					foreach (ColumnHeader column in this.Columns)
						column.Width = ((column.Name == COLUMN_NAME) ? 2 : 1)*width/5;
				}
				else
				{
					foreach (ColumnHeader column in this.Columns)
						column.Width = (int) (1f*width*column.Width/totalWidth);
				}

				this.ResumeLayout(true);
			}

			private void HandleBrowseException(Exception exception)
			{
				FolderView control = base.Parent as FolderView;
				if (control != null)
					control.HandleBrowseException(exception);
			}

			private class FolderListViewItemComparer : IComparer
			{
				private readonly List<string> _sortKeys = new List<string>(4);
				private bool _invertOrder = false;

				public void SortOn(string key)
				{
					if (_sortKeys.IndexOf(key) == 0)
					{
						_invertOrder = !_invertOrder;
						return;
					}
					_invertOrder = false;
					_sortKeys.Remove(key);
					_sortKeys.Insert(0, key);
				}

				public int Compare(object x, object y)
				{
					FolderListViewItem itemX = (FolderListViewItem) x;
					FolderListViewItem itemY = (FolderListViewItem) y;

					if (!_invertOrder)
						return this.CompareCore(itemX, itemY);
					return this.CompareCore(itemY, itemX);
				}

				private int CompareCore(FolderListViewItem itemX, FolderListViewItem itemY)
				{
					int result = itemX.CompareTo(itemY, SORT_FOLDERTYPE);
					if (result != 0)
						return result;

					foreach (string key in _sortKeys)
					{
						result = itemX.CompareTo(itemY, key);
						if (result != 0)
							return result;
					}
					return 0;
				}
			}

			private static IEnumerable<ColumnHeader> CreateDefaultColumns(int width)
			{
				ColumnHeader column;

				column = new ColumnHeader();
				column.Name = COLUMN_NAME;
				column.Text = SR.Name;
				column.Width = width*2/5;
				yield return column;

				column = new ColumnHeader();
				column.Name = COLUMN_SIZE;
				column.Text = SR.Size;
				column.Width = width*1/5;
				yield return column;

				column = new ColumnHeader();
				column.Name = COLUMN_TYPE;
				column.Text = SR.Type;
				column.Width = width*1/5;
				yield return column;

				column = new ColumnHeader();
				column.Name = COLUMN_DATEMODIFIED;
				column.Text = SR.DateModified;
				column.Width = width*1/5;
				yield return column;
			}
		}

		#endregion
	}

	public delegate void FolderViewItemEventHandler(object sender, FolderViewItemEventArgs e);

	public class FolderViewItemEventArgs : EventArgs
	{
		private readonly FolderView.FolderViewItem _item;

		public FolderViewItemEventArgs(FolderView.FolderViewItem item)
		{
			_item = item;
		}

		public FolderView.FolderViewItem Item
		{
			get { return _item; }
		}
	}
}