#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
