using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClearCanvas.Controls.WinForms
{
	public class FolderTree : FolderControl
	{
		private event EventHandler _selectedItemChanged;
		private readonly FolderTreeView _folderTreeView;
		private FolderTreeItem _selectedItem = null;

		public FolderTree()
		{
			_folderTreeView = new FolderTreeView();
			_folderTreeView.AfterBrowse += OnFolderTreeViewAfterBrowse;
			_folderTreeView.AfterSelect += OnFolderTreeViewAfterSelect;
			_folderTreeView.KeyDown += OnFolderTreeViewKeyDown;
			_folderTreeView.KeyPress += OnFolderTreeViewKeyPress;
			_folderTreeView.KeyUp += OnFolderTreeViewKeyUp;
			_folderTreeView.Dock = DockStyle.Fill;

			base.SuspendLayout();
			base.Controls.Add(_folderTreeView);
			base.ResumeLayout(false);
		}

		#region SelectedItem

		public FolderTreeItem SelectedItem
		{
			get { return _selectedItem; }
			private set
			{
				if (_selectedItem != value)
				{
					_selectedItem = value;
					this.OnSelectedItemChanged(EventArgs.Empty);
					this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItem"));
				}
			}
		}

		public event EventHandler SelectedItemsChanged
		{
			add { _selectedItemChanged += value; }
			remove { _selectedItemChanged -= value; }
		}

		protected virtual void OnSelectedItemChanged(EventArgs e)
		{
			if (_selectedItemChanged != null)
				_selectedItemChanged.Invoke(this, e);
		}

		#endregion

		protected override Pidl CurrentPidlCore
		{
			get { return _folderTreeView.CurrentPidl; }
		}

		protected override void BrowseToCore(Pidl pidl)
		{
			_folderTreeView.BrowseTo(pidl);
		}

		public override void Reload()
		{
			Pidl selectedPidl = _folderTreeView.CurrentPidl.Clone();
			try
			{
				foreach (FolderTreeNode node in _folderTreeView.Nodes)
					node.Reload();

				_folderTreeView.BrowseTo(selectedPidl);
			}
			catch (Exception ex)
			{
				this.HandleBrowseException(ex);
			}
			finally
			{
				selectedPidl.Dispose();
			}
		}

		private void OnFolderTreeViewAfterBrowse(object sender, EventArgs e)
		{
			this.OnCurrentPidlChanged(EventArgs.Empty);
			this.NotifyCoordinatorPidlChanged();
		}

		private void OnFolderTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			FolderTreeItem item = null;
			if (_folderTreeView.SelectedNode != null)
			{
				item = new FolderTreeItem(((FolderTreeNode) _folderTreeView.SelectedNode).Pidl);
			}
			this.SelectedItem = item;
		}

		private void OnFolderTreeViewKeyDown(object sender, KeyEventArgs e)
		{
			this.OnKeyDown(e);
		}

		private void OnFolderTreeViewKeyPress(object sender, KeyPressEventArgs e)
		{
			this.OnKeyPress(e);
		}

		private void OnFolderTreeViewKeyUp(object sender, KeyEventArgs e)
		{
			this.OnKeyUp(e);
		}

		#region FolderTreeItem Class

		public class FolderTreeItem : FolderObject
		{
			internal FolderTreeItem(Pidl pidl) : base(pidl) {}
		}

		#endregion

		#region FolderTreeNode Class

		private class FolderTreeNode : TreeNode, IDisposable
		{
			private ShellItem _shellItem;

			private FolderTreeNode() : base() {}

			public FolderTreeNode(ShellItem shellItem) : base()
			{
				_shellItem = shellItem;
				this.Text = _shellItem.DisplayName;
				this.ImageIndex = _shellItem.IconIndex;
				this.SelectedImageIndex = _shellItem.IconIndex;

				// if this folder item has children then add a place holder node.
				if (_shellItem.IsFolder && _shellItem.HasSubFolders)
					this.Nodes.Add(new FolderTreeNode());
			}

			public void Dispose()
			{
				DisposeEach(base.Nodes);
				if (_shellItem != null)
				{
					_shellItem.Dispose();
					_shellItem = null;
				}
			}

			public Pidl Pidl
			{
				get { return _shellItem.Pidl; }
			}

			public void Reload()
			{
				this.TreeView.SuspendLayout();
				try
				{
					DisposeEach(base.Nodes);
					base.Nodes.Clear();
					foreach (ShellItem shellItem in _shellItem.EnumerateChildren(ShellItem.ChildType.Folders))
					{
						base.Nodes.Add(new FolderTreeNode(shellItem));
					}
				}
				catch (PathNotFoundException)
				{
					throw;
				}
				catch (Exception ex)
				{
					throw new IOException("The specified path is inaccessible.", ex);
				}
				finally
				{
					this.TreeView.ResumeLayout(true);
				}
			}
		}

		#endregion

		#region FolderTreeView

		private class FolderTreeView : TreeView
		{
			public event EventHandler AfterBrowse;
			private bool _suppressAfterBrowse = false;

			private ShellItem _rootShellItem = new ShellItem();

			public FolderTreeView()
			{
				this.FullRowSelect = false;
				this.HideSelection = false;
				this.LabelEdit = false;
				this.PathSeparator = Path.PathSeparator.ToString();
				this.Scrollable = true;
				this.ShowLines = true;
				this.ShowNodeToolTips = false;
				this.ShowPlusMinus = true;
				this.ShowRootLines = true;

				FolderTreeNode rootNode = new FolderTreeNode(_rootShellItem);
				this.Nodes.Add(rootNode);
				rootNode.Expand();
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					DisposeEach(base.Nodes);

					// disposing our top level nodes will also dispose the root shell item for us
					_rootShellItem = null;
				}
				base.Dispose(disposing);
			}

			protected override void OnHandleCreated(EventArgs e)
			{
				base.OnHandleCreated(e);

				try
				{
					int hRes = Native.User32.SendMessage(base.Handle, Native.TreeView.TVM_SETIMAGELIST, Native.TreeView.TVSIL_NORMAL, SystemImageList.SmallIcons);
					if (hRes != 0)
						Marshal.ThrowExceptionForHR(hRes);
				}
				catch (Exception ex)
				{
					((FolderTree) this.Parent).HandleInitializationException(ex);
				}
			}

			protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
			{
				// the TreeView cancels the select action if the right mouse button was clicked and a context menu is available
				// so we do this to allow right click select and drop down all in one
				if (e.Node != null && e.Button == MouseButtons.Right)
				{
					this.SelectedNode = e.Node;
				}
				base.OnNodeMouseClick(e);
			}

			protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
			{
				if (e.Node != null)
				{
					try
					{
						((FolderTreeNode) e.Node).Reload();
					}
					catch (Exception ex)
					{
						HandleBrowseException(ex);
					}
				}
				base.OnBeforeExpand(e);
			}

			protected override void OnAfterSelect(TreeViewEventArgs e)
			{
				if (this.SelectedNode != null)
				{
					try
					{
						if (!_suppressAfterBrowse && this.AfterBrowse != null)
							this.AfterBrowse.Invoke(this, EventArgs.Empty);
					}
					catch (Exception ex)
					{
						HandleBrowseException(ex);
					}
				}
				base.OnAfterSelect(e);
			}

			public Pidl CurrentPidl
			{
				get
				{
					if (this.SelectedNode == null)
						return null;
					return ((FolderTreeNode) this.SelectedNode).Pidl;
				}
			}

			public void BrowseTo(Pidl absolutePidl)
			{
				_suppressAfterBrowse = true;
				try
				{
					if (absolutePidl != null)
					{
						TreeNodeCollection nextSearchDomain = this.Nodes;
						do
						{
							TreeNodeCollection currentSearchDomain = nextSearchDomain;
							nextSearchDomain = null;
							foreach (FolderTreeNode node in currentSearchDomain)
							{
								if (node.Pidl == absolutePidl)
								{
									this.SelectedNode = node;
									break;
								}
								else if (node.Pidl.IsAncestorOf(absolutePidl))
								{
									node.Expand();
									nextSearchDomain = node.Nodes;
									break;
								}
							}
						} while (nextSearchDomain != null);
					}
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

			private void HandleBrowseException(Exception exception)
			{
				FolderTree control = base.Parent as FolderTree;
				if (control != null)
					control.HandleBrowseException(exception);
			}
		}

		#endregion
	}
}