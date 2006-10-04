using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	public partial class AENavigatorControl : UserControl
	{
        private AENavigatorComponent _aenavigatorComponent;
        private TreeNode _lastClickedNode;

        public event EventHandler AddClicked
        {
            add { _btnAdd.Click += value; }
            remove { _btnAdd.Click -= value; }
        }

        public event EventHandler AddGroupClicked
        {
            add { _btnAddGroup.Click += value; }
            remove { _btnAddGroup.Click -= value; }
        }

        public event EventHandler EditClicked
        {
            add { _btnEdit.Click += value; }
            remove { _btnEdit.Click -= value; }
        }

        public event EventHandler DeleteClicked
        {
            add { _btnDelete.Click += value; }
            remove { _btnDelete.Click -= value; }
        }

        public event EventHandler CEchoClicked
        {
            add { _btnCEcho.Click += value; }
            remove { _btnCEcho.Click -= value; }
        }

        public AENavigatorControl(AENavigatorComponent component)
		{
            Platform.CheckForNullReference(component, "component");
            InitializeComponent();

            _aenavigatorComponent = component;
            _btnAdd.ToolTipText = "Add New Server";
            _btnAddGroup.ToolTipText = "Add New Server Group";
            _btnEdit.ToolTipText = "Edit";
            _btnDelete.ToolTipText = "Delete";
            _btnCEcho.ToolTipText = "Verify (C-ECHO)";
            AddClicked += new EventHandler(OnAddClicked);
            AddGroupClicked += new EventHandler(OnAddGroupClicked);
            EditClicked += new EventHandler(OnEditClicked);
            DeleteClicked += new EventHandler(OnDeleteClicked);
            CEchoClicked += new EventHandler(OnCEchoClicked);

			this.addServerToolStripMenuItem.Click += OnAddClicked;
			this.addGroupToolStripMenuItem.Click += OnAddGroupClicked;
			this.editServerGroupToolStripMenuItem.Click += OnEditClicked;
			this.deleteServerGroupToolStripMenuItem.Click += OnDeleteClicked;
			this.verifyToolStripMenuItem.Click += OnCEchoClicked;

            _aeTreeView.Click += new EventHandler(AeserverTree_Click);
            BuildServerTreeView(_aeTreeView, _aenavigatorComponent.ServerTreeView);
        }

        void OnAddClicked(object sender, EventArgs e)
        {
            AddServerServerGroup(true);
        }

        void OnAddGroupClicked(object sender, EventArgs e)
        {
            AddServerServerGroup(false);
        }

        void OnEditClicked(object sender, EventArgs e)
        {
            if (_lastClickedNode == null)
                return;
            IBrowserNode dataNode = (IBrowserNode)_lastClickedNode.Tag;
            dataNode =_aenavigatorComponent.EditServer(dataNode);
            if (dataNode != null)
            {
                _lastClickedNode.Text = dataNode.ServerName;
                _lastClickedNode.Tag = dataNode;
                _lastClickedNode.ToolTipText = dataNode.Details;
            }
            _aeTreeView.SelectedNode = _lastClickedNode;
            SetButtonEnabled();
        }

        void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_lastClickedNode == null || _lastClickedNode.Text.Equals(AENavigatorComponent.MyDatastoreTitle)
                || _lastClickedNode.Text.Equals(AENavigatorComponent.MyServersTitle))
                return;
            IBrowserNode dataNode = (IBrowserNode)_lastClickedNode.Tag;
            if (_aenavigatorComponent.DeleteServer(dataNode) && _lastClickedNode != null)
            {
                _aeTreeView.SelectedNode = _lastClickedNode.Parent;
                _lastClickedNode.Remove();
                _lastClickedNode = _aeTreeView.SelectedNode;
            }
            SetButtonEnabled();
        }

        void OnCEchoClicked(object sender, EventArgs e)
        {
			using (new CursorManager(Cursors.WaitCursor))
			{
				_aenavigatorComponent.CEchoServer();
			}
        }

        void AddServerServerGroup(bool isservernode)
        {
            if (_lastClickedNode == null)
                return;
            IBrowserNode dataNode = (IBrowserNode)_lastClickedNode.Tag;
            if (isservernode)
                dataNode = _aenavigatorComponent.AddServer(dataNode);
            else
                dataNode = _aenavigatorComponent.AddServerGroup(dataNode);
            _aeTreeView.SelectedNode = _lastClickedNode;
            if (dataNode != null)
            {
                TreeNode newNode = new TreeNode(dataNode.ServerName);
                newNode.Tag = dataNode;
                newNode.ToolTipText = dataNode.Details;

				if (isservernode)
				{
					newNode.ImageIndex = 1;
					newNode.SelectedImageIndex = 1;
				}
				else
				{
					newNode.ImageIndex = 2;
					newNode.SelectedImageIndex = 2;
				}

                _lastClickedNode.Nodes.Add(newNode);
                _aeTreeView.SelectedNode = newNode;
                _lastClickedNode = _aeTreeView.SelectedNode;
            }
            SetButtonEnabled();
        }

        void AeserverTree_Click(object sender, EventArgs e)
        {
            _lastClickedNode = _aeTreeView.GetNodeAt(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
            if (_lastClickedNode == null)
                return;
            _aeTreeView.SelectedNode = _lastClickedNode;
			//_aeTreeView.SelectedImageIndex = _lastClickedNode.ImageIndex;
            IBrowserNode dataNode = (IBrowserNode)_lastClickedNode.Tag;
            _aenavigatorComponent.SelectChanged(dataNode);
            SetButtonEnabled();
        }

        void SetButtonEnabled()
        {
            if (_lastClickedNode == null)
                return;
            _btnCEcho.Enabled = true;
            if (_lastClickedNode.Text.Equals(AENavigatorComponent.MyDatastoreTitle))
            {
                _btnAdd.Enabled = false;
                _btnAddGroup.Enabled = false;
                _btnEdit.Enabled = true;
                _btnDelete.Enabled = false;
				_btnCEcho.Enabled = false;

				addServerToolStripMenuItem.Enabled = false;
				addGroupToolStripMenuItem.Enabled = false;
				editServerGroupToolStripMenuItem.Enabled = true;
				deleteServerGroupToolStripMenuItem.Enabled = false;
				verifyToolStripMenuItem.Enabled = false;

				return;
            }
            if (_lastClickedNode.Text.Equals(AENavigatorComponent.MyServersTitle))
            {
                _btnAdd.Enabled = true;
                _btnAddGroup.Enabled = true;
                _btnEdit.Enabled = false;
                _btnDelete.Enabled = false;
				_btnCEcho.Enabled = true;

				addServerToolStripMenuItem.Enabled = true;
				addGroupToolStripMenuItem.Enabled = true;
				editServerGroupToolStripMenuItem.Enabled = false;
				deleteServerGroupToolStripMenuItem.Enabled = false;
				verifyToolStripMenuItem.Enabled = true;

				return;
            }
            IBrowserNode dataNode = (IBrowserNode)_lastClickedNode.Tag;
            if (dataNode.IsServerNode)
            {
                _btnAdd.Enabled = false;
                _btnAddGroup.Enabled = false;
                _btnEdit.Enabled = true;
                _btnDelete.Enabled = true;
				_btnCEcho.Enabled = true;

				addServerToolStripMenuItem.Enabled = false;
				addGroupToolStripMenuItem.Enabled = false;
				editServerGroupToolStripMenuItem.Enabled = true;
				deleteServerGroupToolStripMenuItem.Enabled = true;
				verifyToolStripMenuItem.Enabled = true;

			}
            else
            {
                _btnAdd.Enabled = true;
                _btnAddGroup.Enabled = true;
                _btnEdit.Enabled = true;
                _btnDelete.Enabled = true;
				_btnCEcho.Enabled = true;

				addServerToolStripMenuItem.Enabled = true;
				addGroupToolStripMenuItem.Enabled = true;
				editServerGroupToolStripMenuItem.Enabled = true;
				deleteServerGroupToolStripMenuItem.Enabled = true;
				verifyToolStripMenuItem.Enabled = true;

			}
            return;
        }

        /// <summary>
        /// Builds the root and first-level of the tree
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="dataRoot"></param>
        private void BuildServerTreeView(TreeView treeView, IBrowserNode dataRoot)
        {
            treeView.Nodes.Clear();
            treeView.ShowNodeToolTips = true;

            foreach (IBrowserNode dataChild in dataRoot.ChildNodes)
            {
                TreeNode treeChild = new TreeNode(dataChild.ServerName);
                treeChild.Tag = dataChild;
                treeChild.ToolTipText = dataChild.Details;

				SetIcon(dataChild, treeChild);

                treeView.Nodes.Add(treeChild);
                if (dataChild.ServerName.Equals(AENavigatorComponent.MyDatastoreTitle))
                {
                    treeView.SelectedNode = treeChild;
                    _lastClickedNode = treeChild;
                }
                BuildNextTreeLevel(treeChild);
            }
            SetButtonEnabled();
        }

        /// <summary>
        /// Called to build subsequent levels of the tree as they are expanded
        /// </summary>
        /// <param name="treeNode"></param>
        private void BuildNextTreeLevel(TreeNode treeNode)
        {
            IBrowserNode dataNode = (IBrowserNode)treeNode.Tag;
            foreach (IBrowserNode dataChild in dataNode.ChildNodes)
            {
                TreeNode treeChild = new TreeNode(dataChild.ServerName);
				SetIcon(dataChild, treeChild);
                treeChild.Tag = dataChild;
                treeChild.ToolTipText = dataChild.Details;
                treeNode.Nodes.Add(treeChild);
                BuildNextTreeLevel(treeChild);
            }
        }

		private void SetIcon(IBrowserNode browserNode, TreeNode treeNode)
		{
			ServerViewServerNode serverNode = browserNode as ServerViewServerNode;

			if (serverNode == null)
				return;

			if (serverNode.IsServerNode)
			{
				if (serverNode.ServerName == AENavigatorComponent.MyDatastoreTitle)
				{
					treeNode.ImageIndex = 0;
					treeNode.SelectedImageIndex = 0;
				}
				else
				{
					treeNode.ImageIndex = 1;
					treeNode.SelectedImageIndex = 1;
				}
			}
			else
			{
				treeNode.ImageIndex = 2;
				treeNode.SelectedImageIndex = 2;
			}
		}
    }
}
