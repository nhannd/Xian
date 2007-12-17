#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
                ToolStripItem toolstripItem;

                if (node.IsLeaf)
                {
                    // this is a leaf node (terminal menu item)
                    IAction action = (IAction)node.Action;
                    toolstripItem = CreateToolStripItemForAction(action, ToolStripKind.Menu);

                    toolstripItem.Tag = node;
                    parentItemCollection.Add(toolstripItem);

                    // Determine whether we should check the parent menu items too
                    IClickAction clickAction = node.Action as IClickAction;

                    if (clickAction != null && clickAction.CheckParents && clickAction.Checked)
                        CheckParentItems(toolstripItem);
                }
                else
                {
                    // this menu item has a sub menu
                    toolstripItem = new ToolStripMenuItem(node.PathSegment.LocalizedText);

                    toolstripItem.Tag = node;
                    parentItemCollection.Add(toolstripItem);

                    BuildMenu(((ToolStripMenuItem)toolstripItem).DropDownItems, node.ChildNodes);
                }
            }
        }

        private static void CheckParentItems(ToolStripItem menuItem)
        {
            ToolStripMenuItem parentItem = menuItem.OwnerItem as ToolStripMenuItem;

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
            if (action.GetType().GetCustomAttributes(typeof(AssociateViewAttribute), true).Length == 0)
            {
                if (action is IClickAction)
                {
                    if (kind == ToolStripKind.Menu)
                        return new ActiveMenuItem((IClickAction)action);
                    else
                        return new ActiveToolbarButton((IClickAction)action);
                }
            }

            IActionView view = (IActionView)ViewFactory.CreateAssociatedView(action.GetType());
            view.SetAction(action);
            return (ToolStripItem)view.GuiElement;
        }

    }
}
