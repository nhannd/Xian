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

using Gtk;
using System;

using ClearCanvas.Common;
//using ClearCanvas.ImageViewer.Tools;
//using ClearCanvas.Workstation.Layout.Basic;
//using ClearCanvas.Workstation.View.GTK;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Layout.Basic;
using ClearCanvas.ImageViewer.View.GTK;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.GTK
{
	//[ExtensionOf(typeof(ClearCanvas.Workstation.Layout.Basic.LayoutToolViewExtensionPoint))]
	[ExtensionOf(typeof(ClearCanvas.ImageViewer.Layout.Basic.LayoutToolViewExtensionPoint))]
	public class LayoutToolView : GtkView, IToolView
	{
		private LayoutPanel _layoutPanel;
		private LayoutTool _layoutTool;
		
		public LayoutToolView()
		{
			
		}
		
		public void SetTool(ITool tool)
		{
			_layoutTool = (LayoutTool)tool;
			_layoutTool.LayoutChanged += OnLayoutToolLayoutChanged;
		}
		
		
		private LayoutPanel LayoutPanel
		{
			get {
				if(_layoutPanel == null)
				{
					_layoutPanel = new LayoutPanel();
					_layoutPanel.ApplyImageBoxesButton.Clicked += ApplyImageBoxesButton_Clicked;
					_layoutPanel.ApplyTilesButton.Clicked += ApplyTilesButton_Clicked;
					UpdateDisplay();
				}
				return _layoutPanel;
			}
		}
		
		
		private void ApplyImageBoxesButton_Clicked(object sender, EventArgs e)
		{
			_layoutTool.LayoutImageBoxes(LayoutPanel.ImageBoxRows, LayoutPanel.ImageBoxCols,
										 LayoutPanel.TileRows, LayoutPanel.TileCols);
		}
		
		private void ApplyTilesButton_Clicked(object sender, EventArgs e)
		{
			_layoutTool.LayoutTiles(LayoutPanel.TileRows, LayoutPanel.TileCols);
		}
		
		///<summary>
		/// Returns the GTK widget that implements this view, allowing a parent view to insert
		/// the widget as one of its children.
		/// </summary>
		public override object GuiElement
		{
			get { return LayoutPanel; }
		}
		
		private void OnLayoutToolLayoutChanged(object sender, EventArgs args)
		{
			UpdateDisplay();
		}
		
		private void UpdateDisplay()
		{
			if(_layoutPanel != null)
			{
				this.LayoutPanel.ImageBoxRows = _layoutTool.ImageBoxRows;
				this.LayoutPanel.ImageBoxCols = _layoutTool.ImageBoxColumns;
				this.LayoutPanel.TileRows = _layoutTool.TileRows;
				this.LayoutPanel.TileCols = _layoutTool.TileColumns;
			}
		}
	}
}
