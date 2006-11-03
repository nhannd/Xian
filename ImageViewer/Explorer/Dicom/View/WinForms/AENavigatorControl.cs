using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Controls.WinForms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	public partial class AENavigatorControl : UserControl
	{
        public AENavigatorControl(AENavigatorComponent component)
		{
            Platform.CheckForNullReference(component, "component");
            InitializeComponent();

            _component = component;
            ServerTreeUpdated += new EventHandler(OnServerTreeUpdated);

            this.ToolStripItemDisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ToolbarModel = _component.ToolbarModel;
            this.MenuModel = _component.ContextMenuModel;
            this.titleBar1.Style = WinFormsView.VisualStyle;

            _aeTreeView.Click += new EventHandler(AETreeView_Click);
            BuildServerTreeView(_aeTreeView, _component.DicomServerTree);
        }

        void OnServerTreeUpdated(object sender, EventArgs e)
        {
            UpdateServerTree();
        }


        void AETreeView_Click(object sender, EventArgs e)
        {
            _lastClickedNode = _aeTreeView.GetNodeAt(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
            if (_lastClickedNode == null)
                return;
            _aeTreeView.SelectedNode = _lastClickedNode;
            IDicomServer dataNode = (IDicomServer)_lastClickedNode.Tag;
            _component.SelectChanged(dataNode);
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
        }

        /// <summary>
        /// Called to build subsequent levels of the tree as they are expanded
        /// </summary>
        /// <param name="treeNode"></param>
        private void BuildNextTreeLevel(TreeNode treeNode)
        {
            IDicomServer dataNode = (IDicomServer)treeNode.Tag;
            if (dataNode.IsServer)
                return;
            foreach (IDicomServer dataChild in ((DicomServerGroup)dataNode).ChildServers)
            {
                TreeNode treeChild = AddTreeNode(treeNode, dataChild);
                BuildNextTreeLevel(treeChild);
            }
        }

        private TreeNode AddTreeNode(TreeNode treeNode, IDicomServer dataChild)
        {
            TreeNode treeChild = new TreeNode(dataChild.ServerName);
            SetIcon(dataChild, treeChild);
            treeChild.Tag = dataChild;
            treeChild.ToolTipText = dataChild.ServerDetails;
            treeNode.Nodes.Add(treeChild);
            return treeChild;
        }

        private void RefreshToolTipText(TreeNode treeNode)
        {
            DicomServerGroup dataNode = (DicomServerGroup)treeNode.Tag;
            foreach (TreeNode tnChild in treeNode.Nodes)
            {
                foreach (IDicomServer dataChild in (dataNode.ChildServers))
                {
                    if (tnChild.Text.Equals(dataChild.ServerName))
                    {
                        if (!tnChild.ToolTipText.Equals(dataChild.ServerDetails))
                            tnChild.ToolTipText = dataChild.ServerDetails;
                        break;
                    }
                }
                if (!((IDicomServer)tnChild.Tag).IsServer)
                    RefreshToolTipText(tnChild);
            }
        }

        private void UpdateServerTree()
        {
            if (_lastClickedNode == null)
                return;
            if (_component.UpdateType == (int)ServerUpdateType.Add)
            {
                IDicomServer dataChild = _component.DicomServerTree.CurrentServer;
                AddTreeNode(_lastClickedNode, dataChild);
            }
            else if (_component.UpdateType == (int)ServerUpdateType.Delete)
            {
                _aeTreeView.SelectedNode = _lastClickedNode.Parent;
                _lastClickedNode.Remove();
                _lastClickedNode = _aeTreeView.SelectedNode;
                IDicomServer dataNode = (IDicomServer)_lastClickedNode.Tag;
                _component.SelectChanged(dataNode);
            }
            else if (_component.UpdateType == (int)ServerUpdateType.Edit)
            {
                IDicomServer dataNode = _component.DicomServerTree.CurrentServer;
                _lastClickedNode.Text = dataNode.ServerName;
                _lastClickedNode.Tag = dataNode;
                _lastClickedNode.ToolTipText = dataNode.ServerDetails;
                RefreshToolTipText(_aeTreeView.Nodes[1]);
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
        private ActionModelNode _toolbarModel;
        private ActionModelNode _menuModel;
        private ToolStripItemDisplayStyle _toolStripItemDisplayStyle = ToolStripItemDisplayStyle.Image;

        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
            set
            {
                _toolbarModel = value;
                ToolStripBuilder.Clear(_serverTools.Items);
                if (_toolbarModel != null)
                {
                    //_toolbarModel.ChildNodes.Sort(delegate(ActionModelNode n1, ActionModelNode n2) { return n1.Action.Label.CompareTo(n2.Action.Label); });
                    ToolStripBuilder.BuildToolbar(_serverTools.Items, _toolbarModel.ChildNodes, _toolStripItemDisplayStyle);

                    foreach (ToolStripItem item in _serverTools.Items)
                        item.DisplayStyle = _toolStripItemDisplayStyle;
                }
            }
        }

        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;
                ToolStripBuilder.Clear(_contextMenu.Items);
                if (_menuModel != null)
                {
                    ToolStripBuilder.BuildMenu(_contextMenu.Items, _menuModel.ChildNodes);
                }
            }
        }

        public ToolStripItemDisplayStyle ToolStripItemDisplayStyle
        {
            get { return _toolStripItemDisplayStyle; }
            set { _toolStripItemDisplayStyle = value; }
        }

        public event EventHandler ServerTreeUpdated
        {
            add { _component.DicomServerTree.ServerTreeUpdated += value; }
            remove { _component.DicomServerTree.ServerTreeUpdated -= value; }
        }

    }
}
