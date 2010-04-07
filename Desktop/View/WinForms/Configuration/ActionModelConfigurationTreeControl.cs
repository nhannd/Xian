#region License

// Copyright (c) 2010, ClearCanvas Inc.
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