using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

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
        /// Asks if the item can have a subtree.  Note that this method should return true to inidicate that it
        /// is possible that the item might have a subtree.  This allows the view to determine whether to display
        /// a "plus" sign next to the node, without having to actually call <see cref="GetSubTree"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CanHaveSubTree(object item);

        /// <summary>
        /// Gets the <see cref="ITree"/> that represents the subtree for the specified item,
        /// or null if the item does not have a subtree.  Note that <see cref="CanHaveSubTree"/> is called first,
        /// and this method will be called only if <see cref="CanHaveSubTree"/> returns true.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        ITree GetSubTree(object item);

        /// <summary>
        /// Asks the specified item if it can accept the specified drop data in a drag-drop operation.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dropData"></param>
        /// <param name="kind"></param>
        /// <returns>The drop kind that will be accepted</returns>
        DragDropKind CanAcceptDrop(object item, object dropData, DragDropKind kind);

        /// <summary>
        /// Informs the specified item that it should accept a drop of the specified data, completing a drag-drop operation.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dropData"></param>
        /// <param name="kind"></param>
        /// <returns>The drop kind that will be accepted</returns>
        DragDropKind AcceptDrop(object item, object dropData, DragDropKind kind);
    }
}
