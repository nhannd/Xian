using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// Defines the interface to a tree-item binding, which describes how attributes of the visible tree are obtained
    /// from the underlying item.
    /// </summary>
    public interface ITreeItemBinding
    {
        /// <summary>
        /// Gets the text to display for the node representing the specified item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        string GetNodeText(object item);

        /// <summary>
        /// Gets the tooltip to display for the specified item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        string GetTooltipText(object item);
        

        /// <summary>
        /// Gets the <see cref="ITree"/> that represents the subtree for the specified item, or null the item does not have a subtree.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        ITree GetSubTree(object item);
    }
}
