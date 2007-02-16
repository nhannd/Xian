using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;

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

        public virtual IconSet GetIconSet(object item)
        {
            return null;
        }

        public virtual IResourceResolver GetResourceResolver(object item)
        {
            return null;
        }
        
        public virtual bool CanHaveSubTree(object item)
        {
            return true;
        }

        public virtual ITree GetSubTree(object item)
        {
            return null;
        }

        public virtual DragDropKind CanAcceptDrop(object item, object dropData, DragDropKind kind)
        {
            return DragDropKind.None;
        }

        public virtual DragDropKind AcceptDrop(object item, object dropData, DragDropKind kind)
        {
            return DragDropKind.None;
        }

        #endregion
    }
}
