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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("closemenu", "basicgraphic-menu/MenuClose")]
	[Tooltip("closemenu", "CloseMenu")]

	[MenuAction("delete", "basicgraphic-menu/DeleteGraphic", "Delete")]
	[Tooltip("delete", "DeleteGraphic")]

	[MenuAction("deleteall", "basicgraphic-menu/DeleteAllGraphics", "DeleteAll")]
	[Tooltip("deleteall", "DeleteAllGraphics")]

	[ExtensionOf(typeof(GraphicToolExtensionPoint))]

	public class DeleteGraphicTool : GraphicTool
	{
		public DeleteGraphicTool()
		{
		}

		public void Delete()
		{
			IGraphic graphic = this.Context.Graphic;
			if (graphic == null)
				return;

			IPresentationImage image = graphic.ParentPresentationImage;
			if (image == null || !(image is IOverlayGraphicsProvider))
				return;

			IOverlayGraphicsProvider overlayProvider = (IOverlayGraphicsProvider) image;
			int restoreIndex = overlayProvider.OverlayGraphics.IndexOf(graphic);
			if (restoreIndex < 0)
				return;

			//the parent image will be null once the graphic is removed, so we store the history
			// and the presentation image first.
			IImageViewer viewer = this.Context.Graphic.ImageViewer;

			overlayProvider.OverlayGraphics.Remove(graphic);
			image.Draw();

			InsertRemoveOverlayGraphicUndoableCommand command =
				InsertRemoveOverlayGraphicUndoableCommand.GetInsertCommand(image, graphic, restoreIndex);
			command.Name = SR.NameDeleteGraphic;

			viewer.CommandHistory.AddCommand(command);
		}

		/// <summary>
		/// Delete all measurements tool for the graphic context menu
		/// </summary>
		public void DeleteAll()
		{
			IGraphic graphic = this.Context.Graphic;
			if (graphic == null)
				return;

			IPresentationImage image = graphic.ParentPresentationImage;
			if (image == null || !(image is IOverlayGraphicsProvider))
				return;

			//the parent image will be null once the graphic is removed, so we store the history
			// and the presentation image first.
			IImageViewer viewer = this.Context.Graphic.ImageViewer;

			UndoableCommand command = DeleteAllGraphics(image);

			viewer.CommandHistory.AddCommand(command);
		}

		/// <summary>
		/// Delete all measurements tool for the general imageviewer tile context menu
		/// </summary>
		[MenuAction("deleteall", "imageviewer-contextmenu/DeleteAllMeasurementGraphics", "DeleteAll")]
		[Tooltip("deleteall", "DeleteAllMeasurementGraphics")]
		[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
		public class DeleteAllMeasurementGraphicsTool : ImageViewerTool
		{
			public void DeleteAll()
			{
				IPresentationImage image = base.SelectedPresentationImage;
				if (image == null || !(image is IOverlayGraphicsProvider))
					return;

				IImageViewer viewer = base.ImageViewer;

				UndoableCommand command = DeleteAllGraphics(image);

				viewer.CommandHistory.AddCommand(command);
			}
		}

		/// <summary>
		/// Deletes all the the measurements from a particular image and generates the corresponding undo command
		/// </summary>
		/// <param name="image">The <see cref="IPresentationImage"/>; must also be an <see cref="IOverlayGraphicsProvider"/></param>
		/// <returns>The undo command.</returns>
		private static UndoableCommand DeleteAllGraphics(IPresentationImage image) 
		{
			List<IGraphic> graphics = new List<IGraphic>();
			List<int> indices = new List<int>();
			IOverlayGraphicsProvider overlayProvider = (IOverlayGraphicsProvider)image;

			for (int n=0; n< overlayProvider.OverlayGraphics.Count; n++)
			{
				IGraphic graphic = overlayProvider.OverlayGraphics[n];
				if (graphic is InteractiveGraphics.RoiGraphic) { // assume all measurements derive from roigraphic
					graphics.Add(graphic);
					indices.Add(n);
				}
			}

			foreach (IGraphic graphic in graphics) {
				overlayProvider.OverlayGraphics.Remove(graphic);
			}
			image.Draw();

			return InsertRemoveOverlayGraphicUndoableCommand.GetInsertCommand(image, graphics.AsReadOnly(), indices.AsReadOnly());
		}
	}
}
