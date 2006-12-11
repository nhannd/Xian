using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// Event args used when a tree item changes
    /// </summary>
    public class TreeItemEventArgs : EventArgs
    {
        private int _itemIndex;
        private TreeItemChangeType _changeType;

        internal TreeItemEventArgs(TreeItemChangeType changeType, int itemIndex)
        {
            _changeType = changeType;
            _itemIndex = itemIndex;
        }

        /// <summary>
        /// The type of change that occured
        /// </summary>
        public TreeItemChangeType ChangeType
        {
            get { return _changeType; }
        }

        /// <summary>
        /// The index of the item that changed
        /// </summary>
        public int ItemIndex
        {
            get { return _itemIndex; }
        }
    }
}
