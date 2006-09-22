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

            _aenavigatorComponent.SelectedServerChanged += new EventHandler(ServerSettingChanged);

            _aeTreeView.Click += new EventHandler(AeserverTree_Click);
            BuildServerTreeView(_aeTreeView, _aenavigatorComponent.ServerTreeView);
        }

        void ServerSettingChanged(object sender, EventArgs e)
        {
            // to do
        }

        void OnAddClicked(object sender, EventArgs e)
        {
            // to do 
            _aenavigatorComponent.UpdateServer();
        }

        void OnDeleteClicked(object sender, EventArgs e)
        {
            // to do
        }

        void AeserverTree_Click(object sender, EventArgs e)
        {
            //if (!CheckServerSettingData())
            //{ return; }
            _lastClickedNode = _aeTreeView.GetNodeAt(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
            if (_lastClickedNode == null)
                return;
            //if (_lastClickedNode.Text.Equals(AENavigatorComponent.MyDatastoreTitle))
            //    _aenavigatorComponent.DataStoreEvent();
            //else
                _aenavigatorComponent.SelectChanged(_lastClickedNode.Text);
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
                BuildNextTreeLevel(treeChild);
            }
        }

    }
}
