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
        private BindingSource _bindingSource;

        public event EventHandler AddClicked
        {
            add { _btnAdd.Click += value; }
            remove { _btnAdd.Click -= value; }
        }

        public event EventHandler DeleteClicked
        {
            add { _btnDelete.Click += value; }
            remove { _btnDelete.Click -= value; }
        }

        public AENavigatorControl(AENavigatorComponent component)
		{
            Platform.CheckForNullReference(component, "component");
            InitializeComponent();

            _aenavigatorComponent = component;
            AddClicked += new EventHandler(OnAddClicked);
            DeleteClicked += new EventHandler(OnDeleteClicked);

            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _aenavigatorComponent;

            _aenavigatorComponent.SelectedServerChanged += new EventHandler(ServerSettingChanged);

            _aeserverTreeForm1.ServerName.DataBindings.Add("Text", _bindingSource, "ServerName", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeserverTreeForm1.ServerDesc.DataBindings.Add("Text", _bindingSource, "ServerDesc", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeserverTreeForm1.ServerAE.DataBindings.Add("Text", _bindingSource, "ServerAE", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeserverTreeForm1.ServerHost.DataBindings.Add("Text", _bindingSource, "ServerHost", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeserverTreeForm1.ServerPort.DataBindings.Add("Text", _bindingSource, "ServerPort", true, DataSourceUpdateMode.OnPropertyChanged);

            //_aeserverTreeForm1.AeserverTree.BeforeExpand += new TreeViewCancelEventHandler(TreeView_BeforeExpand);
            _aeserverTreeForm1.AeserverTree.Click += new EventHandler(AeserverTree_Click);
            BuildServerTreeView(_aeserverTreeForm1.AeserverTree, _aenavigatorComponent.ServerTreeView);
        }

        void ServerSettingChanged(object sender, EventArgs e)
        {
            _bindingSource.ResetBindings(false);
        }

        void AeserverTree_Click(object sender, EventArgs e)
        {
            UpdateServerNodeName();

            _lastClickedNode = _aeserverTreeForm1.AeserverTree.GetNodeAt(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
            if (_lastClickedNode == null)
                return;
            if (_lastClickedNode.Text.Equals(AENavigatorComponent.MyDatastoreTitle))
                _aenavigatorComponent.DataStoreEvent();
            else
                _aenavigatorComponent.SelectChanged(_lastClickedNode.Text);
        }

        private void TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode expandingNode = e.Node;
            foreach (TreeNode subNode in expandingNode.Nodes)
            {
                // only build the next level if it hasn't been built previously
                if (subNode.Nodes.Count == 0)
                {
                    BuildNextTreeLevel(subNode);
                }
            }
        }

        /// <summary>
        /// Builds the root and first-level of the tree
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="dataRoot"></param>
        private void BuildServerTreeView(TreeView treeView, IBrowserNode dataRoot)
        {
            if (null == _aeserverTreeForm1.AeserverTree.TopNode)
            {
                treeView.Nodes.Add(new TreeNode(AENavigatorComponent.MyDatastoreTitle));
            }

            //treeView.Nodes[1].Nodes.Clear();
            if (dataRoot != null)
            {
                TreeNode treeRoot = new TreeNode(dataRoot.DisplayName);
                treeRoot.Tag = dataRoot;
                treeView.Nodes.Add(treeRoot);

                BuildNextTreeLevel(treeRoot);
            }
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
                TreeNode treeChild = new TreeNode(dataChild.DisplayName);
                treeChild.Tag = dataChild;
                treeNode.Nodes.Add(treeChild);
            }
        }

        void OnAddClicked(object sender, EventArgs e)
        {
            TreeNode newNode = new TreeNode(AENavigatorComponent.NewServerName);
            _aeserverTreeForm1.AeserverTree.Nodes[1].Nodes.Add(newNode);
            _aeserverTreeForm1.AeserverTree.SelectedNode = newNode;

            UpdateServerNodeName();

            _lastClickedNode = newNode;
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _aenavigatorComponent.Add();
            }
        }

        void OnDeleteClicked(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _aenavigatorComponent.Delete();
            }
        }

        private void UpdateServerNodeName()
        {
            if (_lastClickedNode != null && !_lastClickedNode.Text.Equals(AENavigatorComponent.MyDatastoreTitle)
                && !_lastClickedNode.Text.Equals(AENavigatorComponent.MyServersTitle)
                && !_aeserverTreeForm1.ServerName.Text.Equals("") && !_lastClickedNode.Text.Equals(_aeserverTreeForm1.ServerName.Text))
            {
                _lastClickedNode.Text = _aeserverTreeForm1.ServerName.Text;
            }
        }

    }
}
