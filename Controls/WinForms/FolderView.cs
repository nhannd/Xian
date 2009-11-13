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
		private event FolderViewItemEventHandler _itemKeyEnterPressed;
		private readonly FolderListView _folderListView;
		private IList<FolderViewItem> _selectedItems;
		private bool _suppressFolderListViewSelectedIndexChanged = false;

		public FolderView()
		{
			_folderListView = new FolderListView();
			_folderListView.AfterBrowse += OnFolderListViewAfterBrowse;
			_folderListView.SelectedIndexChanged += OnFolderListViewSelectedIndexChanged;
			_folderListView.KeyDown += OnFolderListViewKeyDown;
			_folderListView.KeyPress += OnFolderListViewKeyPress;
			_folderListView.KeyUp += OnFolderListViewKeyUp;
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

		#region Sorting

		public void SortBy(SortKey sortKey)
		{
			_folderListView.Sort(sortKey);
		}

		#endregion

		#region SelectedItems

		public IList<FolderViewItem> SelectedItems
		{
			get
			{
				if (_selectedItems == null)
				{
					List<FolderViewItem> selection = new List<FolderViewItem>();
					if (_folderListView.SelectedItems != null)
					{
						foreach (FolderListViewItem item in _folderListView.SelectedItems)
							selection.Add(new FolderViewItem(item.Pidl));
					}
					_selectedItems = selection.AsReadOnly();
				}
				return _selectedItems;
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

		public void SelectAll()
		{
			_suppressFolderListViewSelectedIndexChanged = true;
			foreach (ListViewItem item in _folderListView.Items)
				item.Selected = true;
			_suppressFolderListViewSelectedIndexChanged = false;
			this.OnFolderListViewSelectedIndexChanged(_folderListView, EventArgs.Empty);
		}

		public void SelectNone()
		{
			_suppressFolderListViewSelectedIndexChanged = true;
			foreach (ListViewItem item in _folderListView.Items)
				item.Selected = false;
			_suppressFolderListViewSelectedIndexChanged = false;
			this.OnFolderListViewSelectedIndexChanged(_folderListView, EventArgs.Empty);
		}

		public void SelectInverted()
		{
			_suppressFolderListViewSelectedIndexChanged = true;
			foreach (ListViewItem item in _folderListView.Items)
				item.Selected = !item.Selected;
			_suppressFolderListViewSelectedIndexChanged = false;
			this.OnFolderListViewSelectedIndexChanged(_folderListView, EventArgs.Empty);
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

		#region ItemKeyEnterPressed

		public event FolderViewItemEventHandler ItemKeyEnterPressed
		{
			add { _itemKeyEnterPressed += value; }
			remove { _itemKeyEnterPressed -= value; }
		}

		protected virtual void OnItemKeyEnterPressed(FolderViewItemEventArgs e)
		{
			if (_itemKeyEnterPressed != null)
				_itemKeyEnterPressed.Invoke(this, e);
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
			if (!_suppressFolderListViewSelectedIndexChanged)
			{
				// reset the selected items list and notify listeners of change
				_selectedItems = null;
				this.OnSelectedItemsChanged(EventArgs.Empty);
				this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
			}
		}

		private void OnFolderListViewKeyDown(object sender, KeyEventArgs e)
		{
			this.OnKeyDown(e);
		}

		private void OnFolderListViewKeyPress(object sender, KeyPressEventArgs e)
		{
			this.OnKeyPress(e);
		}

		private void OnFolderListViewKeyUp(object sender, KeyEventArgs e)
		{
			this.OnKeyUp(e);
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
			private Pidl _pidl;
			private readonly string _fullPath;
			private readonly int _iconIndex;
			private readonly bool _lastModifiedValid;

			public readonly string DisplayName;
			public readonly string TypeName;
			public readonly byte ItemClass;
			public readonly long FileSize;
			public readonly bool IsFolder;
			public readonly DateTime LastModified;

			public FolderListViewItem(Pidl absolutePidl, Pidl myDocumentsReferencePidl) : base()
			{
				_pidl = absolutePidl;
				_fullPath = absolutePidl.Path;

				// request shell item info now - it's faster than binding to ShellItem directly, and we need it for sorting purposes anyway
				const Native.SFGAO REQUEST_ATTRIBUTES = Native.SFGAO.SFGAO_FILESYSTEM | Native.SFGAO.SFGAO_FOLDER | Native.SFGAO.SFGAO_HASSUBFOLDER;
				IntPtr pidl = (IntPtr) _pidl;
				Native.SHFILEINFO shInfo = new Native.SHFILEINFO();
				Native.Shell32.SHGetFileInfo(pidl, (uint) REQUEST_ATTRIBUTES, out shInfo, (uint) Marshal.SizeOf(shInfo),
				                             Native.SHGFI.SHGFI_PIDL | Native.SHGFI.SHGFI_ATTRIBUTES | Native.SHGFI.SHGFI_TYPENAME | Native.SHGFI.SHGFI_DISPLAYNAME | Native.SHGFI.SHGFI_SYSICONINDEX);

				bool isPureVirtual = !(((Native.SFGAO) shInfo.dwAttributes & Native.SFGAO.SFGAO_FILESYSTEM) != 0);
				bool isFolder = (((Native.SFGAO) shInfo.dwAttributes & (Native.SFGAO.SFGAO_FOLDER | Native.SFGAO.SFGAO_HASSUBFOLDER)) != 0) && !File.Exists(_fullPath);

				byte itemClass;
				if (_pidl == myDocumentsReferencePidl)
					itemClass = 0;
				else if (isPureVirtual && isFolder)
					itemClass = 64;
				else if (isFolder)
					itemClass = (byte) (128 + GetRootVolumeIndex(_fullPath));
				else
					itemClass = byte.MaxValue;

				this._iconIndex = shInfo.iIcon;
				this._lastModifiedValid = false;
				this.DisplayName = shInfo.szDisplayName;
				this.TypeName = shInfo.szTypeName;
				this.ItemClass = itemClass;
				this.LastModified = DateTime.MinValue;
				this.IsFolder = isFolder;
				this.FileSize = -1;

				if (File.Exists(_fullPath))
				{
					FileInfo fileInfo = new FileInfo(this._fullPath);
					this._lastModifiedValid = true;
					this.LastModified = fileInfo.LastWriteTime;
					this.FileSize = fileInfo.Length;
				}
				else if (Directory.Exists(_fullPath))
				{
					this._lastModifiedValid = true;
					this.LastModified = Directory.GetLastWriteTime(_fullPath);
				}

				this.Text = this.DisplayName;
				this.ImageIndex = this._iconIndex;
				this.SubItems.Add(CreateSubItem(this, FormatFileSize(this.FileSize)));
				this.SubItems.Add(CreateSubItem(this, this.TypeName));
				this.SubItems.Add(CreateSubItem(this, FormatLastModified(this.LastModified, this._lastModifiedValid)));
			}

			public void Dispose()
			{
				if (_pidl != null)
				{
					_pidl.Dispose();
					_pidl = null;
				}
			}

			public Pidl Pidl
			{
				get { return _pidl; }
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

			private static byte GetRootVolumeIndex(string fullPath)
			{
				if (string.IsNullOrEmpty(fullPath))
					return 26;
				string rootVolume = Directory.GetDirectoryRoot(fullPath);
				if (string.IsNullOrEmpty(rootVolume) || !string.Equals(rootVolume, fullPath, StringComparison.InvariantCultureIgnoreCase))
					return 26;
				char driveLetter = rootVolume.ToLowerInvariant()[0];
				if (driveLetter < 'a' || driveLetter > 'z')
					return 26;
				return (byte) (driveLetter - 'a');
			}

			private static string FormatLastModified(DateTime lastModified, bool valid)
			{
				if (!valid)
					return string.Empty;
				return lastModified.ToString();
			}

			private static ListViewSubItem CreateSubItem(ListViewItem parent, string value)
			{
				return new ListViewSubItem(parent, value, SystemColors.GrayText, parent.BackColor, parent.Font);
			}
		}

		#endregion

		#region FolderListView Class

		private class FolderListView : ListView
		{
			public event EventHandler AfterBrowse;
			private bool _suppressAfterBrowse = false;

			private const string COLUMN_NAME = "Name";
			private const string COLUMN_SIZE = "Size";
			private const string COLUMN_TYPE = "Type";
			private const string COLUMN_DATEMODIFIED = "DateModified";

			private Pidl _myDocumentsReferencePidl;
			private ShellItem _rootShellItem = new ShellItem();
			private ShellItem _currentShellItem = null;

			private bool _autoDrillDown = true;

			public FolderListView() : base()
			{
				_currentShellItem = _rootShellItem.Clone();
				_myDocumentsReferencePidl = new Pidl(Environment.SpecialFolder.MyDocuments);

				this.Alignment = ListViewAlignment.SnapToGrid;
				this.AllowColumnReorder = false;
				this.AutoArrange = true;
				this.CheckBoxes = false;
				this.HeaderStyle = ColumnHeaderStyle.Clickable;
				this.HideSelection = false;
				this.LabelEdit = false;
				this.LabelWrap = true;
				this.ListViewItemSorter = new FolderListViewItemComparer();
				this.MultiSelect = true;
				this.Scrollable = true;
				this.Sorting = SortOrder.Ascending;
				this.TileSize = new Size(12*(this.FontHeight + 4), 3*this.FontHeight + 4);
				this.View = View.LargeIcon;
				this.PopulateItems();
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

					if (_myDocumentsReferencePidl != null)
					{
						_myDocumentsReferencePidl.Dispose();
						_myDocumentsReferencePidl = null;
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

			protected override void OnColumnClick(ColumnClickEventArgs e)
			{
				base.OnColumnClick(e);
				switch (this.Columns[e.Column].Name)
				{
					case COLUMN_NAME:
						this.Sort(SortKey.Name);
						break;
					case COLUMN_SIZE:
						this.Sort(SortKey.Size);
						break;
					case COLUMN_TYPE:
						this.Sort(SortKey.Type);
						break;
					case COLUMN_DATEMODIFIED:
						this.Sort(SortKey.Date);
						break;
				}
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
								this.Browse(new ShellItem(item.Pidl, _rootShellItem, false));
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

			protected override void OnKeyDown(KeyEventArgs e)
			{
				if (e.KeyCode == Keys.Enter)
				{
					if (this.SelectedItems != null && this.SelectedItems.Count > 0)
					{
						try
						{
							FolderListViewItem item = (FolderListViewItem) this.FocusedItem;
							if (item != null)
							{
								FolderView control = base.Parent as FolderView;
								if (control != null)
									control.OnItemKeyEnterPressed(new FolderViewItemEventArgs(new FolderViewItem(item.Pidl)));

								if (_autoDrillDown && item.IsFolder)
								{
									// if the user pressed ENTER on a folder item, perform a drill down
									this.Browse(new ShellItem(item.Pidl, _rootShellItem, false));
								}
							}
						}
						catch (Exception ex)
						{
							HandleBrowseException(ex);
						}
					}

					e.Handled = true;
					e.SuppressKeyPress = true;
				}
				base.OnKeyDown(e);
			}

			protected override bool IsInputKey(Keys keyData)
			{
				if (keyData == Keys.Enter)
					return true;
				return base.IsInputKey(keyData);
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
						this.SuspendLayout();
						try
						{
							if (base.View == View.Details)
								base.Columns.Clear();

							base.View = value;

							if (value == View.Details)
								base.Columns.AddRange(CreateDetailsViewColumns());
						}
						finally
						{
							this.ResumeLayout(true);
						}
					}
				}
			}

			public void Sort(SortKey sortKey)
			{
				((FolderListViewItemComparer) this.ListViewItemSorter).SortOn(sortKey);
				this.Sort();
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
							_currentShellItem.Dispose();

						_currentShellItem = destination;

						if (_currentShellItem != null)
							this.PopulateItems();
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

			private void PopulateItems()
			{
				List<FolderListViewItem> items = new List<FolderListViewItem>();
				foreach (Pidl pidl in _currentShellItem.EnumerateChildPidls())
				{
					items.Add(new FolderListViewItem(new Pidl(_currentShellItem.Pidl, pidl), _myDocumentsReferencePidl));
					pidl.Dispose(); // the enumerator makes relative PIDLs, but the FolderListViewItem needs an absolute one
				}
				this.Items.AddRange(items.ToArray());
			}

			private void HandleBrowseException(Exception exception)
			{
				FolderView control = base.Parent as FolderView;
				if (control != null)
					control.HandleBrowseException(exception);
			}

			private static ColumnHeader[] CreateDetailsViewColumns()
			{
				ColumnHeader[] columns = new ColumnHeader[4];

				columns[0] = new ColumnHeader();
				columns[0].Name = COLUMN_NAME;
				columns[0].Text = SR.Name;
				columns[0].Width = -1;

				columns[1] = new ColumnHeader();
				columns[1].Name = COLUMN_SIZE;
				columns[1].Text = SR.Size;
				columns[1].Width = 100;

				columns[2] = new ColumnHeader();
				columns[2].Name = COLUMN_TYPE;
				columns[2].Text = SR.Type;
				columns[2].Width = 100;

				columns[3] = new ColumnHeader();
				columns[3].Name = COLUMN_DATEMODIFIED;
				columns[3].Text = SR.DateModified;
				columns[3].Width = 100;

				return columns;
			}

			private class FolderListViewItemComparer : IComparer
			{
				private readonly List<SortKey> _sortKeys = new List<SortKey>(4);
				private readonly Dictionary<SortKey, bool> _invertOrder = new Dictionary<SortKey, bool>();
				private bool _invertClassOrder = false;

				public FolderListViewItemComparer()
				{
					_sortKeys.Add(SortKey.Name);
					_invertOrder[SortKey.Name] = false;
					_invertOrder[SortKey.Date] = false;
					_invertOrder[SortKey.Size] = false;
					_invertOrder[SortKey.Type] = false;
				}

				public void SortOn(SortKey key)
				{
					if (_sortKeys.Remove(key))
						_invertOrder[key] = _invertClassOrder = !_invertOrder[key];
					_sortKeys.Insert(0, key);
				}

				public int Compare(object x, object y)
				{
					FolderListViewItem itemX = (FolderListViewItem) x;
					FolderListViewItem itemY = (FolderListViewItem) y;

					int result = itemX.ItemClass.CompareTo(itemY.ItemClass);
					if (result != 0)
						return result*(_invertClassOrder ? -1 : 1);

					foreach (SortKey key in _sortKeys)
					{
						result = key.Compare(itemX, itemY);
						if (result != 0)
							return result*(_invertOrder[key] ? -1 : 1);
					}
					return 0;
				}
			}
		}

		#endregion

		#region SortKey

		public sealed class SortKey
		{
			public static readonly SortKey Name = new SortKey((x, y) => string.Compare(x.DisplayName, y.DisplayName, StringComparison.InvariantCultureIgnoreCase));
			public static readonly SortKey Type = new SortKey((x, y) => string.Compare(x.TypeName, y.TypeName, StringComparison.InvariantCultureIgnoreCase));
			public static readonly SortKey Size = new SortKey((x, y) => DateTime.Compare(x.LastModified, y.LastModified));
			public static readonly SortKey Date = new SortKey((x, y) => x.FileSize.CompareTo(y.FileSize));

			private readonly Comparison<FolderListViewItem> _comparison;

			private SortKey(Comparison<FolderListViewItem> comparison)
			{
				_comparison = comparison;
			}

			internal int Compare(ListViewItem x, ListViewItem y)
			{
				return _comparison((FolderListViewItem) x, (FolderListViewItem) y);
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