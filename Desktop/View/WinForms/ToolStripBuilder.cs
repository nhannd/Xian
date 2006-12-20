using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    public static class ToolStripBuilder
    {
        public static void BuildToolbar(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes, ToolStripItemDisplayStyle displayStyle)
        {
            foreach (ActionModelNode node in nodes)
            {
                if (node.IsLeaf)
                {
                    IAction action = (IAction)node.Action;
                    ToolStripItem button = GetToolStripItemForAction(action);
                    button.Tag = node;

                    // By default, only display the image on the button
                    button.DisplayStyle = displayStyle;
                    parentItemCollection.Add(button);
                }
                else
                {
                    BuildToolbar(parentItemCollection, node.ChildNodes, displayStyle);
                }
            }
        }

        public static void BuildMenu(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes)
        {
            foreach (ActionModelNode node in nodes)
            {
                ToolStripMenuItem menuItem;

                if (node.IsLeaf)
                {
                    // this is a leaf node (terminal menu item)
                    IAction action = (IAction)node.Action;
                    menuItem = (ToolStripMenuItem)GetToolStripItemForAction(action);

                }
                else
                {
                    // this menu item has a sub menu
                    menuItem = new ToolStripMenuItem(node.PathSegment.LocalizedText);
                    BuildMenu(menuItem.DropDownItems, node.ChildNodes);
                }

				menuItem.Tag = node;
                parentItemCollection.Add(menuItem);
            }
        }
        
        public static void Clear(ToolStripItemCollection parentItemCollection)
        {
            // this is kinda dumb, but we can't just Dispose() of the items directly
            // because calling Dispose() alters the parent collection causing the
            // enumeration to fail - hence the temp array
            ToolStripItem[] temp = new ToolStripItem[parentItemCollection.Count];
            for (int i = 0; i < parentItemCollection.Count; i++)
            {
                temp[i] = parentItemCollection[i];
            }

            // item seems that calling Dispose() on the item will automatically recurse
            // to all it's children, so no need to recurse here
            foreach (ToolStripItem item in temp)
            {
                // the system may have added other items to the toolstrip,
                // so make sure we only delete our own
                if (item.Tag is ActionModelNode)
                {
                    item.Dispose();
                }
            }
        }

        private static ToolStripItem GetToolStripItemForAction(IAction action)
        {
            IActionView view = (IActionView)ViewFactory.CreateAssociatedView(action.GetType());
            view.SetAction(action);
            return (ToolStripItem)view.GuiElement;
        }

    }
}
