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
        public AENavigatorControl(AENavigatorComponent component)
		{
            Platform.CheckForNullReference(component, "component");
            InitializeComponent();

            _component = component;
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

            _aeTreeView.Click += new EventHandler(AETreeView_Click);
            BuildServerTreeView(_aeTreeView, _component.DicomServerTree);
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
            IDicomServer dataNode = (IDicomServer)_lastClickedNode.Tag;
            if (dataNode.IsServer)
                dataNode =_component.AddEditServer(dataNode);
            else
                dataNode = _component.AddEditServerGroup(dataNode, false);
            if (dataNode != null) 
            {
                _lastClickedNode.Text = dataNode.ServerName;
                _lastClickedNode.Tag = dataNode;
                _lastClickedNode.ToolTipText = dataNode.ServerDetails;
            }
            _aeTreeView.SelectedNode = _lastClickedNode;
            SetButtonEnabled();
        }

        void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_lastClickedNode == null || _lastClickedNode.Text.Equals(AENavigatorComponent.MyDatastoreTitle)
                || _lastClickedNode.Text.Equals(AENavigatorComponent.MyServersTitle))
                return;
            IDicomServer dataNode = (IDicomServer)_lastClickedNode.Tag;
            if (_component.DeleteServer(dataNode) && _lastClickedNode != null)
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
				_component.CEchoServer();
			}
        }

        void AddServerServerGroup(bool isservernode)
        {
            if (_lastClickedNode == null)
                return;
            IDicomServer dataNode = (IDicomServer)_lastClickedNode.Tag;
            if (isservernode)
                dataNode = _component.AddEditServer(dataNode);
            else
                dataNode = _component.AddEditServerGroup(dataNode, true);
            _aeTreeView.SelectedNode = _lastClickedNode;
            if (dataNode != null)
            {
                TreeNode newNode = new TreeNode(dataNode.ServerName);
                newNode.Tag = dataNode;
                newNode.ToolTipText = dataNode.ServerDetails;

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

        void AETreeView_Click(object sender, EventArgs e)
        {
            _lastClickedNode = _aeTreeView.GetNodeAt(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
            if (_lastClickedNode == null)
                return;
            _aeTreeView.SelectedNode = _lastClickedNode;
			//_aeTreeView.SelectedImageIndex = _lastClickedNode.ImageIndex;
            IDicomServer dataNode = (IDicomServer)_lastClickedNode.Tag;
            _component.SelectChanged(dataNode);
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
            IDicomServer dataNode = (IDicomServer)_lastClickedNode.Tag;
            if (dataNode.IsServer)
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
        private void BuildServerTreeView(TreeView treeView, DicomServerTree dicomServerTree)
        {
            treeView.Nodes.Clear();
            treeView.ShowNodeToolTips = true;

            dicomServerTree.MyServerGroup.ChildServers.Sort(delegate(IDicomServer s1, IDicomServer s2)
            {
                string s1param = s1.IsServer ? "aa" : "bb"; s1param += s1.ServerName;
                string s2param = s2.IsServer ? "aa" : "bb"; s2param += s2.ServerName;
                return s1param.CompareTo(s2param);
            });
            foreach (IDicomServer dataChild in dicomServerTree.MyServerGroup.ChildServers)
            {
                TreeNode treeChild = new TreeNode(dataChild.ServerName);
                treeChild.Tag = dataChild;
                treeChild.ToolTipText = dataChild.ServerDetails;

				SetIcon(dataChild, treeChild);

                treeView.Nodes.Add(treeChild);
                if (dataChild.ServerName.Equals(AENavigatorComponent.MyDatastoreTitle))
                {
                    treeView.SelectedNode = treeChild;
                    _lastClickedNode = treeChild;
                    //_component.SelectChanged(dataChild);
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
            IDicomServer dataNode = (IDicomServer)treeNode.Tag;
            dataNode.ChildServers.Sort(delegate(IDicomServer s1, IDicomServer s2)
            { string s1param = s1.IsServer ? "cc" : "bb"; s1param += s1.ServerName; 
                string s2param = s2.IsServer ? "cc" : "bb"; s2param += s2.ServerName; 
                return s1param.CompareTo(s2param); });
            foreach (IDicomServer dataChild in dataNode.ChildServers)
            {
                TreeNode treeChild = new TreeNode(dataChild.ServerName);
				SetIcon(dataChild, treeChild);
                treeChild.Tag = dataChild;
                treeChild.ToolTipText = dataChild.ServerDetails;
                treeNode.Nodes.Add(treeChild);
                BuildNextTreeLevel(treeChild);
            }
        }

        private void SetIcon(IDicomServer browserNode, TreeNode treeNode)
		{
            if (browserNode == null)
				return;

            if (browserNode.IsServer)
			{
                if (browserNode.ServerName == AENavigatorComponent.MyDatastoreTitle)
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

        private AENavigatorComponent _component;
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
    }
}
