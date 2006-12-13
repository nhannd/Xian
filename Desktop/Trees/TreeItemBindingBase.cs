using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// Base implementation of <see cref="ITreeItemBinding"/>.  Provides null default implementations of most methods.
    /// </summary>
    public abstract class TreeItemBindingBase : ITreeItemBinding
    {
        #region ITreeItemBinding members

        public abstract string GetNodeText(object item);

        public virtual string GetTooltipText(object item)
        {
            return null;
        }

        public virtual ITree GetSubTree(object item)
        {
            return null;
        }

        #endregion
    }
}
