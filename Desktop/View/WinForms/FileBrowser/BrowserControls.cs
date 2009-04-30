#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms.FileBrowser.ShellDll;

namespace ClearCanvas.Desktop.View.WinForms.FileBrowser
{
    #region TreeView

    /// <summary>
    /// This is the TreeView used in the Browser control
    /// </summary>
    internal class BrowserTreeView : TreeView
    {
        private BrowserTreeSorter sorter;

        public BrowserTreeView()
        {
            HandleCreated += new EventHandler(BrowserTreeView_HandleCreated);

            sorter = new BrowserTreeSorter();
        }

        #region Override

        #endregion

        #region Events

        /// <summary>
        /// Once the handle is created we can assign the image list to the TreeView
        /// </summary>
        void BrowserTreeView_HandleCreated(object sender, EventArgs e)
        {
            ShellImageList.SetSmallImageList(this);
        }

        #endregion

        #region Public

        public bool GetTreeNode(ShellItem shellItem, out TreeNode treeNode)
        {
            ArrayList pathList = new ArrayList();
            
            while (shellItem.ParentItem != null)
            {
                pathList.Add(shellItem);
                shellItem = shellItem.ParentItem;
            }
            pathList.Add(shellItem);

            pathList.Reverse();

            treeNode = Nodes[0];
            for (int i = 1; i < pathList.Count; i++)
            {
                bool found = false;
                foreach (TreeNode node in treeNode.Nodes)
                {
                    if (node.Tag != null && node.Tag.Equals(pathList[i]))
                    {
                        treeNode = node;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    treeNode = null;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This method will check whether a TreeNode is a parent of another TreeNode
        /// </summary>
        /// <param name="parent">The parent TreeNode</param>
        /// <param name="child">The child TreeNode</param>
        /// <returns>true if the parent is a parent of the child, false otherwise</returns>
        public bool IsParentNode(TreeNode parent, TreeNode child)
        {
            TreeNode current = child;
            while (current.Parent != null)
            {
                if (current.Parent.Equals(parent))
                    return true;

                current = current.Parent;
            }
            return false;
        }

        /// <summary>
        /// This method will check whether a TreeNode is a parent of another TreeNode
        /// </summary>
        /// <param name="parent">The parent TreeNode</param>
        /// <param name="child">The child TreeNode</param>
        /// <param name="path">If the parent is indeed a parent of the child, this will be a path of
        /// TreeNodes from the parent to the child including both parent and child</param>
        /// <returns>true if the parent is a parent of the child, false otherwise</returns>
        public bool IsParentNode(TreeNode parent, TreeNode child, out TreeNode[] path)
        {
            ArrayList pathList = new ArrayList();

            TreeNode current = child;
            while (current.Parent != null)
            {
                pathList.Add(current);
                if (current.Parent.Equals(parent))
                {
                    pathList.Add(parent);
                    pathList.Reverse();
                    path = (TreeNode[])pathList.ToArray(typeof(TreeNode));
                    return true;
                }

                current = current.Parent;
            }

            path = null;
            return false;
        }

        public void SetSorting(bool sorting)
        {
            if (sorting)
                this.TreeViewNodeSorter = sorter;
            else
                this.TreeViewNodeSorter = null;
        }

        #endregion
    }

    /// <summary>
    /// This class is used to sort the TreeNodes in the BrowserTreeView
    /// </summary>
    internal class BrowserTreeSorter : IComparer
    {
        #region IComparer Members

        /// <summary>
        /// This method will compare the ShellItems of the TreeNodes to determine the return value for
        /// comparing the TreeNodes.
        /// </summary>
        public int Compare(object x, object y)
        {
            TreeNode nodeX = x as TreeNode;
            TreeNode nodeY = y as TreeNode;

            if (nodeX.Tag != null && nodeY.Tag != null)
                return ((ShellItem)nodeX.Tag).CompareTo(nodeY.Tag);
            else if (nodeX.Tag != null)
                return 1;
            else if (nodeY.Tag != null)
                return -1;
            else
                return 0;
        }

        #endregion
    }

    #endregion

    #region ListView

    /// <summary>
    /// This is the ListView used in the Browser control
    /// </summary>
    internal class BrowserListView : ListView
    {
        #region Fields

        // The arraylist to store the order by which ListViewItems has been selected
        private ArrayList selectedOrder;

        private ContextMenu columnHeaderContextMenu;
        private bool suspendHeaderContextMenu;
        private int columnHeight = 0;

        private BrowserListSorter sorter;

        #endregion

        public BrowserListView()
        {
            OwnerDraw = true;

            HandleCreated += new EventHandler(BrowserListView_HandleCreated);
            selectedOrder = new ArrayList();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            DrawItem += new DrawListViewItemEventHandler(BrowserListView_DrawItem);
            DrawSubItem += new DrawListViewSubItemEventHandler(BrowserListView_DrawSubItem);
            DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(BrowserListView_DrawColumnHeader);

            this.Alignment = ListViewAlignment.Left;
            sorter = new BrowserListSorter();
        }

        #region Owner Draw

        void BrowserListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        void BrowserListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        void BrowserListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            columnHeight = e.Bounds.Height;
        }

        #endregion

        #region Override

		public new System.Windows.Forms.View View
        {
            get
            {
                return base.View;
            }
            set
            {
                base.View = value;

				if (value == System.Windows.Forms.View.Details)
                {
                    foreach (ColumnHeader col in Columns)
                        if (col.Width == 0)
                            col.Width = 120;
                }
            }
        }

        protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
                selectedOrder.Insert(0, e.Item);
            else
                selectedOrder.Remove(e.Item);

            base.OnItemSelectionChanged(e);
        }

        protected override void WndProc(ref Message m)
        {
			if (this.View == System.Windows.Forms.View.Details && columnHeaderContextMenu != null &&
                (int)m.Msg == (int)ShellAPI.WM.CONTEXTMENU)
            {
                if (suspendHeaderContextMenu)
                    suspendHeaderContextMenu = false;
                else
                {
                    int x = (int)ShellHelper.LoWord(m.LParam);
                    int y = (int)ShellHelper.HiWord(m.LParam);
                    Point clientPoint = PointToClient(new Point(x, y));
                    
                    if (clientPoint.Y <= columnHeight)
                        columnHeaderContextMenu.Show(this, clientPoint);
                }

                return;
            }

            base.WndProc(ref m);
        }

        #endregion

        #region Events

        /// <summary>
        /// Once the handle is created we can assign the image lists to the ListView
        /// </summary>
        void BrowserListView_HandleCreated(object sender, EventArgs e)
        {
            ShellImageList.SetSmallImageList(this);
            ShellImageList.SetLargeImageList(this);
        }

        #endregion

        #region Public

        [Browsable(false)]
        public ArrayList SelectedOrder
        {
            get { return selectedOrder; }
        }

        [Browsable(false)]
        public bool SuspendHeaderContextMenu
        {
            get { return suspendHeaderContextMenu; }
            set { suspendHeaderContextMenu = value; }
        }

        [Browsable(true)]
        public ContextMenu ColumnHeaderContextMenu
        {
            get { return columnHeaderContextMenu; }
            set { columnHeaderContextMenu = value; }
        }

        public void SetSorting(bool sorting)
        {
            if (sorting)
                this.ListViewItemSorter = sorter;
            else
                this.ListViewItemSorter = null;
        }

        public void ClearSelections()
        {
            selectedOrder.Clear();
            selectedOrder.Capacity = 0;
        }

        public bool GetListItem(ShellItem shellItem, out ListViewItem listItem)
        {
            listItem = null;

            foreach (ListViewItem item in Items)
            {
                if (shellItem.Equals(item.Tag))
                {
                    listItem = item;
                    return true;
                }
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    /// This class is used to sort the ListViewItems in the BrowserListView
    /// </summary>
    internal class BrowserListSorter : IComparer
    {
        #region IComparer Members

        /// <summary>
        /// This method will compare the ShellItems of the ListViewItems to determine the return value for
        /// comparing the ListViewItems.
        /// </summary>
        public int Compare(object x, object y)
        {
            ListViewItem itemX = x as ListViewItem;
            ListViewItem itemY = y as ListViewItem;

            if (itemX.Tag != null && itemY.Tag != null)
                return ((ShellItem)itemX.Tag).CompareTo(itemY.Tag);
            else if (itemX.Tag != null)
                return 1;
            else if (itemY.Tag != null)
                return -1;
            else
                return 0;
        }

        #endregion
    }

    #endregion
}