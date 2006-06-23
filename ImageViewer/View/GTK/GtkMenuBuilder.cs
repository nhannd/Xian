using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Workstation.Model;
using ClearCanvas.Workstation.Model.Actions;
using Gtk;

namespace ClearCanvas.Workstation.View.GTK
{
    public class GtkMenuBuilder
    {
        public static void BuildMenu(MenuShell menu, ActionModelNode node)
        {
            if (node.PathSegment != null)
            {
				
                MenuItem menuItem;
                if (node.Action != null)
                {
                    // this is a leaf node (terminal menu item)
                    menuItem = new ActiveMenuItem((IClickAction)node.Action);
                }
                else
                {
                    // this menu item has a sub menu
					string menuText = node.PathSegment.LocalizedText.Replace('&', '_');
					menuItem = new MenuItem(menuText);
                    menuItem.Submenu = new Menu();
                }

                menu.Append(menuItem);
                menu = (MenuShell)menuItem.Submenu;
            }

            foreach (ActionModelNode child in node.ChildNodes)
            {
                BuildMenu(menu, child);
            }
        }
    }
}
