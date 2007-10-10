using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
    public static class ToolStripBuilder
    {
        public enum ToolStripKind
        {
            Menu,
            Toolbar
        }

        public class ToolStripBuilderStyle
        {
            private ToolStripItemDisplayStyle _toolStripItemDisplayStyle = ToolStripItemDisplayStyle.Image;
            private ToolStripItemAlignment _toolStripItemAlignment = ToolStripItemAlignment.Left;
            private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;

            public ToolStripBuilderStyle(ToolStripItemDisplayStyle toolStripItemDisplayStyle, ToolStripItemAlignment toolStripItemAlignment, TextImageRelation textImageRelation)
            {
                _toolStripItemAlignment = toolStripItemAlignment;
                _toolStripItemDisplayStyle = toolStripItemDisplayStyle;
                _textImageRelation = textImageRelation;
            }

            public ToolStripBuilderStyle(ToolStripItemDisplayStyle toolStripItemDisplayStyle)
            {
                _toolStripItemDisplayStyle = toolStripItemDisplayStyle;
            }

            public ToolStripBuilderStyle()
            {
            }

            public ToolStripItemAlignment ToolStripAlignment
            {
                get { return _toolStripItemAlignment; }
            }

            public ToolStripItemDisplayStyle ToolStripItemDisplayStyle
            {
                get { return _toolStripItemDisplayStyle; }
            }

            public TextImageRelation TextImageRelation
            {
                get { return _textImageRelation; }
            }
        }

        public static void BuildToolStrip(ToolStripKind kind, ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes)
        {
            BuildToolStrip(kind, parentItemCollection, nodes, new ToolStripBuilderStyle());
        }
        
        public static void BuildToolStrip(ToolStripKind kind, ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes, ToolStripBuilderStyle builderStyle)
        {
            switch (kind)
            {
                case ToolStripKind.Menu:
                    BuildMenu(parentItemCollection, nodes);
                    break;
                case ToolStripKind.Toolbar:
                    BuildToolbar(parentItemCollection, nodes, builderStyle);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void BuildToolbar(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes, ToolStripBuilderStyle builderStyle)
        {
            foreach (ActionModelNode node in nodes)
            {
                if (node.IsLeaf)
                {
                    IAction action = (IAction)node.Action;
                    ToolStripItem button = CreateToolStripItemForAction(action, ToolStripKind.Toolbar);
                    button.Tag = node;

                    // By default, only display the image on the button
                    button.DisplayStyle = builderStyle.ToolStripItemDisplayStyle;
                    button.Alignment = builderStyle.ToolStripAlignment;
                    button.TextImageRelation = builderStyle.TextImageRelation;

                    parentItemCollection.Add(button);
                }
                else
                {
                    BuildToolbar(parentItemCollection, node.ChildNodes, builderStyle);
                }
            }
        }

        public static void BuildToolbar(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes, ToolStripItemDisplayStyle toolStripItemDisplayStyle)
        {
            BuildToolbar(parentItemCollection, nodes, new ToolStripBuilderStyle(toolStripItemDisplayStyle));
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
                    menuItem = (ToolStripMenuItem)CreateToolStripItemForAction(action, ToolStripKind.Menu);

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

        private static ToolStripItem CreateToolStripItemForAction(IAction action, ToolStripKind kind)
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
