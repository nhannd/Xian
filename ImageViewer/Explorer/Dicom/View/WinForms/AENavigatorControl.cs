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
            //AddClicked += new EventHandler(OnAddClicked);
            //DeleteClicked += new EventHandler(OnDeleteClicked);

            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _aenavigatorComponent;

            _aenavigatorComponent.SelectedServerChanged += new EventHandler(ServerSettingChanged);
            //_aeserverTreeForm1.LostFocus += new EventHandler(_aeserverTreeForm1_LostFocus);
            //_aeserverTreeForm1.ServerName.LostFocus += new EventHandler(ServerName_LostFocus);
            //_aeserverTreeForm1.ServerAE.LostFocus += new EventHandler(ServerAE_LostFocus);
            //_aeserverTreeForm1.ServerHost.LostFocus += new EventHandler(ServerHost_LostFocus);
            //_aeserverTreeForm1.ServerPort.LostFocus += new EventHandler(ServerPort_LostFocus);

            _aeserverTreeForm1.ServerName.DataBindings.Add("Text", _bindingSource, "ServerName", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeserverTreeForm1.ServerDesc.DataBindings.Add("Text", _bindingSource, "ServerDesc", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeserverTreeForm1.ServerAE.DataBindings.Add("Text", _bindingSource, "ServerAE", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeserverTreeForm1.ServerHost.DataBindings.Add("Text", _bindingSource, "ServerHost", true, DataSourceUpdateMode.OnPropertyChanged);
            _aeserverTreeForm1.ServerPort.DataBindings.Add("Text", _bindingSource, "ServerPort", true, DataSourceUpdateMode.OnPropertyChanged);

            //_aeserverTreeForm1.AeserverTree.BeforeExpand += new TreeViewCancelEventHandler(TreeView_BeforeExpand);
            _aeserverTreeForm1.AeserverTree.Click += new EventHandler(AeserverTree_Click);
            //_aeserverTreeForm1.AeserverTree.BeforeSelect += new TreeViewCancelEventHandler(AeserverTree_BeforeSelect);
            BuildServerTreeView(_aeserverTreeForm1.AeserverTree, _aenavigatorComponent.ServerTreeView);
        }

        void ServerSettingChanged(object sender, EventArgs e)
        {
//            _bindingSource.ResetBindings(false);
        }

        void _aeserverTreeForm1_LostFocus(object sender, EventArgs e)
        {
            //to do
        }

        void ServerName_LostFocus(object sender, EventArgs e)
        {
            int chknum = _aenavigatorComponent.IsServerSettingValid(ServerSettingItem.ServerName, _aeserverTreeForm1.ServerName.Text, true);
            if (chknum < 0)
            {
                _bindingSource.ResetBindings(false);
                _aenavigatorComponent.ServerSettingError(ServerSettingItem.ServerName, chknum);
            }
            else
            {
                UpdateServerNodeName();
            }
        }

        void ServerAE_LostFocus(object sender, EventArgs e)
        {
            int chknum = _aenavigatorComponent.IsServerSettingValid(ServerSettingItem.AE, _aeserverTreeForm1.ServerAE.Text, true);
            if (chknum < 0)
            {
                _bindingSource.ResetBindings(false);
                _aenavigatorComponent.ServerSettingError(ServerSettingItem.AE, chknum);
            }
        }

        void ServerHost_LostFocus(object sender, EventArgs e)
        {
            int chknum = _aenavigatorComponent.IsServerSettingValid(ServerSettingItem.Host, _aeserverTreeForm1.ServerHost.Text, true);
            if (chknum < 0)
            {
                _bindingSource.ResetBindings(false);
                _aenavigatorComponent.ServerSettingError(ServerSettingItem.Host, chknum);
            }
        }

        void ServerPort_LostFocus(object sender, EventArgs e)
        {
            int chknum = _aenavigatorComponent.IsServerSettingValid(ServerSettingItem.Port, _aeserverTreeForm1.ServerPort.Text, true);
            if (chknum < 0)
            {
                _bindingSource.ResetBindings(false);
                _aenavigatorComponent.ServerSettingError(ServerSettingItem.Port, chknum);
            }
        }

        void OnAddClicked(object sender, EventArgs e)
        {
            string svrname = _aenavigatorComponent.AddServer();
            if (svrname == null || svrname.Equals(""))
                return;
            TreeNode newNode = new TreeNode(svrname);
            _aeserverTreeForm1.AeserverTree.Nodes[1].Nodes.Add(newNode);
            _aeserverTreeForm1.AeserverTree.SelectedNode = newNode;

            _lastClickedNode = newNode;
            //UpdateServerNodeName();

        }

        void OnDeleteClicked(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                if (_lastClickedNode == null || _lastClickedNode.Text.Equals(AENavigatorComponent.MyDatastoreTitle)
                    || _lastClickedNode.Text.Equals(AENavigatorComponent.MyServersTitle))
                    return;
                if (_aenavigatorComponent.ServerDeleteConfirm())
                {
                    if (_aenavigatorComponent.DeleteServer() && _lastClickedNode != null)
                    {
                        _aeserverTreeForm1.AeserverTree.SelectedNode = _lastClickedNode.Parent;
                        _lastClickedNode.Remove();
                    }
                }
            }
        }

        void AeserverTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (!CheckServerSettingData())
                e.Cancel = true; ;
        }

        void AeserverTree_Click(object sender, EventArgs e)
        {
            //if (!CheckServerSettingData())
            //{ return; }
            _lastClickedNode = _aeserverTreeForm1.AeserverTree.GetNodeAt(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
            if (_lastClickedNode == null)
                return;
            //if (_lastClickedNode.Text.Equals(AENavigatorComponent.MyDatastoreTitle))
            //    _aenavigatorComponent.DataStoreEvent();
            //else
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

        public bool CheckServerSettingData()
        {
            int chknum = _aenavigatorComponent.IsServerSettingValid(ServerSettingItem.ServerName, _aeserverTreeForm1.ServerName.Text, false);
            if (chknum < 0)
            {
                _bindingSource.ResetBindings(false);
                if (_lastClickedNode != null && _aeserverTreeForm1.AeserverTree.SelectedNode != null
                    && _aeserverTreeForm1.AeserverTree.SelectedNode.Text.Equals(_lastClickedNode.Text))
                    _aeserverTreeForm1.AeserverTree.SelectedNode = _lastClickedNode;
                _aenavigatorComponent.ServerSettingError(ServerSettingItem.ServerName, chknum);
                return false;
            }
            chknum = _aenavigatorComponent.IsServerSettingValid(ServerSettingItem.AE, _aeserverTreeForm1.ServerAE.Text, false);
            if (chknum < 0)
            {
                _bindingSource.ResetBindings(false);
                if (_lastClickedNode != null && _aeserverTreeForm1.AeserverTree.SelectedNode != null
                    && _aeserverTreeForm1.AeserverTree.SelectedNode.Text.Equals(_lastClickedNode.Text))
                    _aeserverTreeForm1.AeserverTree.SelectedNode = _lastClickedNode;
                _aenavigatorComponent.ServerSettingError(ServerSettingItem.AE, chknum);
                return false;
            }
            chknum = _aenavigatorComponent.IsServerSettingValid(ServerSettingItem.Host, _aeserverTreeForm1.ServerHost.Text, false);
            if (chknum < 0)
            {
                _bindingSource.ResetBindings(false);
                if (_lastClickedNode != null && _aeserverTreeForm1.AeserverTree.SelectedNode != null
                    && _aeserverTreeForm1.AeserverTree.SelectedNode.Text.Equals(_lastClickedNode.Text))
                    _aeserverTreeForm1.AeserverTree.SelectedNode = _lastClickedNode;
                _aenavigatorComponent.ServerSettingError(ServerSettingItem.Host, chknum);
                return false;
            }
            chknum = _aenavigatorComponent.IsServerSettingValid(ServerSettingItem.Port, _aeserverTreeForm1.ServerPort.Text, false);
            if (chknum < 0)
            {
                _bindingSource.ResetBindings(false);
                if (_lastClickedNode != null && _aeserverTreeForm1.AeserverTree.SelectedNode != null
                    && _aeserverTreeForm1.AeserverTree.SelectedNode.Text.Equals(_lastClickedNode.Text))
                    _aeserverTreeForm1.AeserverTree.SelectedNode = _lastClickedNode;
                _aenavigatorComponent.ServerSettingError(ServerSettingItem.Port, chknum);
                return false;
            }
            return true;

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
                treeView.Nodes.Add(treeChild);
                BuildNextTreeLevel(treeChild);
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
                TreeNode treeChild = new TreeNode(dataChild.ServerName);
                treeChild.Tag = dataChild;
                treeChild.ToolTipText = dataChild.Details;
                treeNode.Nodes.Add(treeChild);
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
