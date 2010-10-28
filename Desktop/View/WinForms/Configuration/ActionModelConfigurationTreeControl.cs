#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	[Obsolete("See JY", true)]
	public class ActionModelConfigurationTreeControl : BindingTreeView
	{
		public ActionModelConfigurationTreeControl() : base()
		{
			base.TreeView.DrawMode = TreeViewDrawMode.OwnerDrawText;
			base.TreeView.DrawNode += OnTreeViewDrawNode;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.TreeView.DrawNode -= OnTreeViewDrawNode;
			}
			base.Dispose(disposing);
		}

		private void OnTreeViewDrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			if (e.Node.IsEditing)
				return;

			Font font = e.Node.NodeFont ?? base.TreeView.Font;
			Color fontColor = !e.Node.ForeColor.IsEmpty ? e.Node.ForeColor : base.TreeView.ForeColor;
			if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected && (e.State & TreeNodeStates.Focused) == TreeNodeStates.Focused)
				fontColor = SystemColors.HighlightText;

			using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
			{
				sf.HotkeyPrefix = HotkeyPrefix.Show;
				sf.Alignment = StringAlignment.Near;
				sf.LineAlignment = StringAlignment.Center;

				Rectangle newNodeBounds = new Rectangle(e.Bounds.X + 3, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
				using (Brush b = new SolidBrush(fontColor))
				{
					e.Graphics.DrawString(e.Node.Text, font, b, newNodeBounds, sf);
				}
			}
		}
	}
}