using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("closemenu", "basicgraphic-menu/CloseMenu")]
	[Tooltip("closemenu", "CloseMenu")]

	[MenuAction("delete", "basicgraphic-menu/DeleteGraphicTool")]
	[Tooltip("delete", "DeleteGraphicTool")]
	[IconSet("delete", IconScheme.Colour, "Icons.DeleteGraphicSmall.png", "DeleteGraphicSmall.png", "DeleteGraphicSmall.png")]
	[ClickHandler("delete", "Delete")]

	[ExtensionOf(typeof(GraphicToolExtensionPoint))]

	public class DeleteGraphicTool : Tool<IGraphicToolContext>
	{
		public DeleteGraphicTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public void Delete()
		{
			Graphic graphic = this.Context.Graphic;
			if (graphic == null)
				return;

			if (graphic.ParentLayerManager == null)
				return;

			if (graphic.ParentLayerManager.SelectedGraphicLayer == null)
				return;

			graphic.ParentLayerManager.SelectedGraphicLayer.Graphics.Remove(graphic);
			graphic.ParentLayerManager.ParentPresentationImage.Draw();
		}
	}
}
