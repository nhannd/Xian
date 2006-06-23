using System;
using System.Collections.Generic;
using System.Text;

using Gtk;
using ClearCanvas.Workstation.Model.Actions;

namespace ClearCanvas.Workstation.View.GTK
{
    public class GtkToolbarBuilder
    {
        public static void BuildToolbar(Toolbar toolbar, Tooltips tooltips, ActionModelNode node)
        {
           BuildToolbar(toolbar, tooltips, node, 0);
        }

        public static void BuildToolbar(Toolbar toolbar, Tooltips tooltips, ActionModelNode node, int depth)
        {
            if (node.Action != null)
            {

                // create the tool button
				ToolButton button = new ActiveToolbarButton((IClickAction)node.Action, tooltips);
                toolbar.Insert(button, toolbar.NItems);
            }
            else
            {
                foreach (ActionModelNode child in node.ChildNodes)
                {
                    BuildToolbar(toolbar, tooltips, child, depth + 1);
                }
            }
        }
    }
}
