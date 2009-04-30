#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    public static class ToolStripBuilder
	{
		#region ToolStripKind

		public enum ToolStripKind
        {
            Menu,
            Toolbar
		}

		#endregion

		#region ToolStripBuilderStyle

		/// <summary>
		/// Specifies style charateristics for a tool strip.
		/// </summary>
		public class ToolStripBuilderStyle
        {
			/// <summary>
			/// Gets an object representing the default style defined by <see cref="DesktopViewSettings"/>.
			/// </summary>
        	public static ToolStripBuilderStyle GetDefault()
        	{
				return new ToolStripBuilderStyle(ToolStripItemDisplayStyle.Image,
											  DesktopViewSettings.Default.LocalToolStripItemAlignment,
											  DesktopViewSettings.Default.LocalToolStripItemTextImageRelation);
        	}


            private readonly ToolStripItemDisplayStyle _toolStripItemDisplayStyle = ToolStripItemDisplayStyle.Image;
            private readonly ToolStripItemAlignment _toolStripItemAlignment = ToolStripItemAlignment.Left;
            private readonly TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;

            public ToolStripBuilderStyle(ToolStripItemDisplayStyle toolStripItemDisplayStyle, ToolStripItemAlignment toolStripItemAlignment, TextImageRelation textImageRelation)
            {
                _toolStripItemAlignment = toolStripItemAlignment;
                _toolStripItemDisplayStyle = toolStripItemDisplayStyle;
                _textImageRelation = textImageRelation;
            }

			[Obsolete("This overload will be removed in a future version of the framework.")]
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

		#endregion

		#region Public API

		/// <summary>
		/// Builds a toolstrip of the specified kind, from the specified action model nodes, using the default style.
		/// </summary>
		/// <param name="kind"></param>
		/// <param name="parentItemCollection"></param>
		/// <param name="nodes"></param>
		public static void BuildToolStrip(ToolStripKind kind, ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes)
        {
            BuildToolStrip(kind, parentItemCollection, nodes, ToolStripBuilderStyle.GetDefault());
        }

		/// <summary>
		/// Builds a toolstrip of the specified kind, from the specified action model nodes, using the specified style.
		/// </summary>
		/// <param name="kind"></param>
		/// <param name="parentItemCollection"></param>
		/// <param name="nodes"></param>
		/// <param name="builderStyle"></param>
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

		/// <summary>
		/// Builds a toolbar from the specified action model nodes, using the default style.
		/// </summary>
		/// <param name="parentItemCollection"></param>
		/// <param name="nodes"></param>
        public static void BuildToolbar(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes)
        {
        	BuildToolbar(parentItemCollection, nodes, ToolStripBuilderStyle.GetDefault());
        }

		/// <summary>
		/// Builds a toolbar from the specified action model nodes, using the specified style.
		/// </summary>
		/// <param name="parentItemCollection"></param>
		/// <param name="nodes"></param>
		/// <param name="builderStyle"></param>
        public static void BuildToolbar(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes, ToolStripBuilderStyle builderStyle)
        {
			List<ActionModelNode> nodeList = CombineAdjacentSeparators(new List<ActionModelNode>(nodes));
			
			// reverse nodes if alignment is right
			if (builderStyle.ToolStripAlignment == ToolStripItemAlignment.Right)
				nodeList.Reverse();

			foreach (ActionModelNode node in nodeList)
            {
                if (node is ActionNode)
                {
                    IAction action = ((ActionNode)node).Action;
                    ToolStripItem button = CreateToolStripItemForAction(action, ToolStripKind.Toolbar);
                    button.Tag = node;

                    // By default, only display the image on the button
                    button.DisplayStyle = builderStyle.ToolStripItemDisplayStyle;
                    button.Alignment = builderStyle.ToolStripAlignment;
                    button.TextImageRelation = builderStyle.TextImageRelation;

                    parentItemCollection.Add(button);
                }
				else if(node is SeparatorNode)
				{
					ToolStripSeparator separator = new ToolStripSeparator();
					separator.Tag = node;
					parentItemCollection.Add(separator);
				}
                else
                {
                    BuildToolbar(parentItemCollection, node.ChildNodes, builderStyle);
                }
            }
        }

		[Obsolete("This overload will be removed in a future version of the framework.")]
        public static void BuildToolbar(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes, ToolStripItemDisplayStyle toolStripItemDisplayStyle)
        {
            BuildToolbar(parentItemCollection, nodes, new ToolStripBuilderStyle(toolStripItemDisplayStyle));
        }

		/// <summary>
		/// Builds a menu from the specified action model nodes.
		/// </summary>
		/// <param name="parentItemCollection"></param>
		/// <param name="nodes"></param>
        public static void BuildMenu(ToolStripItemCollection parentItemCollection, IEnumerable<ActionModelNode> nodes)
        {
			List<ActionModelNode> nodeList = CombineAdjacentSeparators(new List<ActionModelNode>(nodes));
			foreach (ActionModelNode node in nodeList)
            {
                ToolStripItem toolstripItem;

                if (node is ActionNode)
                {
                    // this is a leaf node (terminal menu item)
                	ActionNode actionNode = (ActionNode) node;
					IAction action = actionNode.Action;
                    toolstripItem = CreateToolStripItemForAction(action, ToolStripKind.Menu);

                    toolstripItem.Tag = node;
                    parentItemCollection.Add(toolstripItem);

                    // Determine whether we should check the parent menu items too
					IClickAction clickAction = actionNode.Action as IClickAction;

                    if (clickAction != null && clickAction.CheckParents && clickAction.Checked)
                        CheckParentItems(toolstripItem);
                }
				else if (node is SeparatorNode)
				{
					toolstripItem = new ToolStripSeparator();
					toolstripItem.Tag = node;
					parentItemCollection.Add(toolstripItem);
				}
				else
                {
                    // this menu item has a sub menu
                    toolstripItem = new ToolStripMenuItem(node.PathSegment.LocalizedText);

                    toolstripItem.Tag = node;
                    parentItemCollection.Add(toolstripItem);

                    BuildMenu(((ToolStripMenuItem)toolstripItem).DropDownItems, node.ChildNodes);
                }

                // When you get Visible, it refers to whether the object is really visible, as opposed to whether it _can_ be visible. 
                // When you _set_ Visible, it affects whether it _can_ be visible.
                // For example, an item is really invisible but _can_ be visible before it is actually drawn.
                // This is why we use the Available property, which give us the information when we are interested in "_Could_ this be Visible?"
                ToolStripMenuItem parent = toolstripItem.OwnerItem as ToolStripMenuItem;
                if (parent != null)
                {
                    SetParentAvailability(parent);
                    toolstripItem.AvailableChanged += delegate { SetParentAvailability(parent); };
                }
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

		#endregion

		#region Private Helpers

		private static void SetParentAvailability(ToolStripMenuItem parent)
		{
			bool parentIsAvailable = false;
			foreach (ToolStripItem item in parent.DropDownItems)
			{
				if (item.Available)
					parentIsAvailable = true;
			}

			if (parent.Available != parentIsAvailable)
				parent.Available = parentIsAvailable;
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

		private static List<ActionModelNode> CombineAdjacentSeparators(List<ActionModelNode> nodes)
		{
			// nothing to do if less than 2 items
			if(nodes.Count < 2)
				return nodes;

			List<ActionModelNode> result = new List<ActionModelNode>();
			result.Add(nodes[0]);
			for(int i = 1; i < nodes.Count; i++)
			{
				// if both this node and the previous node are separators, do not add this node to the result
				if(nodes[i] is SeparatorNode && nodes[i-1] is SeparatorNode)
					continue;

				result.Add(nodes[i]);
			}
			return result;
		}

		private static ToolStripItem CreateToolStripItemForAction(IAction action, ToolStripKind kind)
        {
            // optimization: for framework-provided actions, we can just create the controls
            // directly rather than use the associated view, which is slower;
            // however, an AssociateViewAttribute should always take precedence.
            if (action.GetType().GetCustomAttributes(typeof(AssociateViewAttribute), true).Length == 0)
            {
				if (kind == ToolStripKind.Toolbar)
				{
					if (action is IDropDownAction)
					{
						if (action is IClickAction)
							return new DropDownButtonToolbarItem((IClickAction)action);
						
						return new DropDownToolbarItem((IDropDownAction) action);
					}
					else if (action is IClickAction)
					{
						return new ActiveToolbarButton((IClickAction)action);
					}
				}
				else
				{
					if (action is IClickAction)
						return new ActiveMenuItem((IClickAction)action);
				}
            }

            IActionView view = (IActionView)ViewFactory.CreateAssociatedView(action.GetType());
            view.SetAction(action);
            return (ToolStripItem)view.GuiElement;
		}

		#endregion
	}
}
