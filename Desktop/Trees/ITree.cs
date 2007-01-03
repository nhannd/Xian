using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// Defines the interface to a tree, which provides a presentation model for viewing hierarchical data
    /// </summary>
    public interface ITree
    {
        /// <summary>
        /// Obtains the <see cref="ITreeItemBinding"/> that defines how items in this tree are mapped to the view.
        /// </summary>
        ITreeItemBinding Binding { get; }

        /// <summary>
        /// Obtains a reference to the collection of items in this tree.  Note that this collection contains
        /// only the immediate items.  Each item may provide a sub-tree, which can be obtained via the
        /// <see cref="ITreeItemBinding.GetSubTree"/> method.
        /// </summary>
        IItemCollection Items { get; }
    }
}
