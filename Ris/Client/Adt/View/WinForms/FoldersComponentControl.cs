using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="FoldersComponent"/>
    /// </summary>
    public partial class FoldersComponentControl : CustomUserControl
    {
        private FoldersComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public FoldersComponentControl(FoldersComponent component)
        {
            InitializeComponent();

            _component = component;

            ToolStripBuilder.BuildToolbar(_toolbar.Items, _component.ToolbarModel.ChildNodes);

            foreach (IFolder folder in _component.Folders)
            {
                TreeNode node = new TreeNode(folder.DisplayName);
                node.Tag = folder;

                _folderTree.Nodes.Add(node);
            }

            _component.SelectedFolderChanged += new EventHandler(_component_SelectedFolderChanged);

            // TODO add .NET databindings to _component
        }

        private void _component_SelectedFolderChanged(object sender, EventArgs e)
        {
            if (_component.SelectedFolder != _folderTree.SelectedNode.Tag)
            {
                SelectTreeNodeForFolder(_component.SelectedFolder);
            }
        }

        private void _folderTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _component.SelectedFolder = (IFolder)_folderTree.SelectedNode.Tag;
        }

        private void SelectTreeNodeForFolder(IFolder folder)
        {
            foreach(TreeNode node in _folderTree.Nodes)
            {
                if (node.Tag == folder)
                {
                    _folderTree.SelectedNode = node;
                    break;
                }
            }
        }
    }
}
