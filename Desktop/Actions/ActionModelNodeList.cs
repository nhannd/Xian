using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Used by <see cref="ActionModelNode"/> to hold the list of child nodes.
    /// </summary>
    public class ActionModelNodeList : List<ActionModelNode>
    {
        /// <summary>
        /// Returns the child node whose <see cref="ActionModelNode.PathSegment"/> <see cref="ClearCanvas.Desktop.PathSegment.LocalizedText"/> property
        /// is equal to the specified value.
        /// </summary>
        /// <param name="name">The name of the node to retrieve</param>
        /// <returns>The corresponding child node, or null if no such node exists</returns>
        public ActionModelNode this[string name]
        {
            get
            {
                foreach (ActionModelNode node in this)
                {
                    // define node equality in terms of the localized text
                    // (eg two menu items with the same name should be the same menu item, 
                    // even if a different resource key was used)
                    if (node.PathSegment.LocalizedText == name)
                        return node;
                }
                return null;
            }
        }
    }
}
