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
        enum ToolStripKind
        {
            Menu,
            Toolbar
        }

        public static void BuildToolbar(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes, ToolStripItemDisplayStyle displayStyle)
        {
            foreach (ActionModelNode node in nodes)
            {
                if (node.IsLeaf)
                {
                    IAction action = (IAction)node.Action;
                    ToolStripItem button = GetToolStripItemForAction(action, ToolStripKind.Toolbar);
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
                    menuItem = (ToolStripMenuItem)GetToolStripItemForAction(action, ToolStripKind.Menu);

					menuItem.Tag = node;
					parentItemCollection.Add(menuItem);

					// Determine whether we should check the parent menu items too
					IClickAction clickAction = node.Action as IClickAction;

					if (clickAction != null && clickAction.CheckParents && clickAction.Checked)
						CheckParentItems(menuItem);
				}
                else
                {
                    // this menu item has a sub menu
                    menuItem = new ToolStripMenuItem(node.PathSegment.LocalizedText);
					
					menuItem.Tag = node;
					parentItemCollection.Add(menuItem);
					
					BuildMenu(menuItem.DropDownItems, node.ChildNodes);
                }
            }
        }

		private static void CheckParentItems(ToolStripMenuItem menuItem)
		{
			ToolStripMenuItem parentItem = (ToolStripMenuItem) menuItem.OwnerItem;

			if (parentItem != null)
			{
				parentItem.Checked = true;
				CheckParentItems(parentItem);
			}

			return;
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

        private static ToolStripItem GetToolStripItemForAction(IAction action, ToolStripKind kind)
        {
            // optimization: since most actions will be IClickAction, we can just create the controls
            // directly rather than use the associated view, which is slower
            // however, an AssociateViewAttribute should always take precedence
            if (action is IClickAction && action.GetType().GetCustomAttributes(typeof(AssociateViewAttribute), true).Length == 0)
            {
                if (kind == ToolStripKind.Menu)
                    return new ActiveMenuItem((IClickAction)action);
                else
                    return new ActiveToolbarButton((IClickAction)action);
            }
            else
            {
                IActionView view = (IActionView)ViewFactory.CreateAssociatedView(action.GetType());
                view.SetAction(action);
                return (ToolStripItem)view.GuiElement;
            }
        }

    }
}
