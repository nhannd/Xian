using Gtk;
using System;

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Tools;
using ClearCanvas.Workstation.Layout.Basic;
using ClearCanvas.Workstation.View.GTK;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.GTK
{
	[ExtensionOf(typeof(ClearCanvas.Workstation.Layout.Basic.LayoutToolViewExtensionPoint))]
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
