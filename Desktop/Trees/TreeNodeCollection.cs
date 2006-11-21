using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ClearCanvas.Desktop.Trees
{
    public class TreeNodeCollection<TItem> : ITreeNodeCollection
    {
        private ICollection _items;
        private TreeNodeProviderDelegate<TItem> _treeNodeProvider;

        public TreeNodeCollection(ICollection items, TreeNodeProviderDelegate<TItem> treeNodeProvider)
        {
            _items = items;
            _treeNodeProvider = treeNodeProvider;
        }

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            foreach (TItem item in _items)
            {
                yield return _treeNodeProvider(item);
            }
        }

        #endregion

        #region ITreeNodeCollection Members

        public int Count
        {
            get { return _items.Count; }
        }

        #endregion
    }
}
