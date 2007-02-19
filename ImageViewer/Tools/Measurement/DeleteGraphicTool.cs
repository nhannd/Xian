using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

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
			IGraphic graphic = this.Context.Graphic;
			
			IOverlayGraphicsProvider image = graphic.ParentPresentationImage as IOverlayGraphicsProvider;

			if (image == null)
				return;

			PositionGraphicCommand command = new PositionGraphicCommand(graphic, PositionGraphicCommand.CreateOperation.Delete);
			command.Name = SR.NameDeleteGraphic;
			this.Context.Graphic.ImageViewer.CommandHistory.AddCommand(command);
			graphic.Visible = false;
			graphic.Draw();
		}
	}
}
