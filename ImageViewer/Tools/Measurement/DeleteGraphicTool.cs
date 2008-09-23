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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("closemenu", "basicgraphic-menu/MenuClose")]
	[GroupHint("closemenu", "Tools.Image.Measurement.MenuClose")]
	[Tooltip("closemenu", "CloseMenu")]

	[MenuAction("delete", "basicgraphic-menu/DeleteGraphic", "Delete")]
	[IconSet("delete", IconScheme.Colour, "DeleteMeasurementToolSmall.png", "DeleteMeasurementToolMedium.png", "DeleteMeasurementToolLarge.png")]
	[GroupHint("delete", "Tools.Image.Measurement.Delete")]
	[Tooltip("delete", "DeleteGraphic")]

	[MenuAction("deleteall", "basicgraphic-menu/DeleteAllGraphics", "DeleteAll")]
	[IconSet("deleteall", IconScheme.Colour, "DeleteAllMeasurementsToolSmall.png", "DeleteAllMeasurementsToolMedium.png", "DeleteAllMeasurementsToolLarge.png")]
	[GroupHint("deleteall", "Tools.Image.Measurement.Delete.All")]
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
				if (IsMeasurementGraphic(graphic)) {
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

		/// <summary>
		/// This method defines what graphics are considered measurement graphics and are hence removed by this tool
		/// </summary>
		/// <param name="graphic">The graphic in question</param>
		/// <returns>True if graphic is a measurement graphic; False otherwise.</returns>
		private static bool IsMeasurementGraphic(IGraphic graphic)
		{
			return (graphic is InteractiveGraphics.RoiGraphic); // assume all measurements derive from roigraphic
		}

		#region Delete All (ImageViewerTool)

		/// <summary>
		/// Delete all measurements tool for the general imageviewer tile context menu
		/// </summary>
		[MenuAction("deleteall", "imageviewer-contextmenu/DeleteAllMeasurementGraphics", "DeleteAll")]
		[VisibleStateObserver("deleteall", "DeleteAllVisible", "DeleteAllVisibleChanged")]
		[IconSet("deleteall", IconScheme.Colour, "DeleteAllMeasurementsToolSmall.png", "DeleteAllMeasurementsToolMedium.png", "DeleteAllMeasurementsToolLarge.png")]
		[GroupHint("deleteall", "Tools.Image.Measurement.DeleteAll")]
		[Tooltip("deleteall", "DeleteAllMeasurementGraphics")]
		[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
		public class DeleteAllMeasurementGraphicsTool : ImageViewerTool
		{
			public event EventHandler DeleteAllVisibleChanged;

			private IOverlayGraphicsProvider _currentOverlayProvider;
			private bool _deleteAllVisible;

			public void DeleteAll()
			{
				IPresentationImage image = base.SelectedPresentationImage;
				if (image == null || !(image is IOverlayGraphicsProvider))
					return;

				IImageViewer viewer = base.ImageViewer;

				UndoableCommand command = DeleteAllGraphics(image);

				viewer.CommandHistory.AddCommand(command);
			}

			public bool DeleteAllVisible
			{
				get { return _deleteAllVisible; }
				private set
				{
					if (_deleteAllVisible != value)
					{
						_deleteAllVisible = value;
						EventsHelper.Fire(DeleteAllVisibleChanged, this, new EventArgs());
					}
				}
			}

			/// <summary>
			/// Updates the <see cref="DeleteAllVisible"/> property
			/// </summary>
			private void UpdateDeleteAllVisible()
			{
				IOverlayGraphicsProvider overlayProvider = base.SelectedOverlayGraphicsProvider;
				if (overlayProvider != null) {
					foreach (IGraphic graphic in overlayProvider.OverlayGraphics) {
						if (IsMeasurementGraphic(graphic))
						{
							this.DeleteAllVisible = true;
							return;
						}
					}
				}
				this.DeleteAllVisible = false;
			}

			private IOverlayGraphicsProvider CurrentOverlayGraphicsProvider
			{
				set
				{
					if (_currentOverlayProvider != value)
					{
						if (_currentOverlayProvider != null) {
							_currentOverlayProvider.OverlayGraphics.ItemAdded -= OverlayGraphics_ListChanged;
							_currentOverlayProvider.OverlayGraphics.ItemChanged -= OverlayGraphics_ListChanged;
							_currentOverlayProvider.OverlayGraphics.ItemRemoved -= OverlayGraphics_ListChanged;
						}

						_currentOverlayProvider = value;

						if (_currentOverlayProvider != null)
						{
							_currentOverlayProvider.OverlayGraphics.ItemAdded += OverlayGraphics_ListChanged;
							_currentOverlayProvider.OverlayGraphics.ItemChanged += OverlayGraphics_ListChanged;
							_currentOverlayProvider.OverlayGraphics.ItemRemoved += OverlayGraphics_ListChanged;
						}
					}
				}
			}

			private void OverlayGraphics_ListChanged(object sender, ListEventArgs<IGraphic> e) {
				UpdateDeleteAllVisible();
			}

			protected override void Dispose(bool disposing) {
				// force an unsub when tool is being disposed
				this.CurrentOverlayGraphicsProvider = null; 

				base.Dispose(disposing);
			}

			protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e) {
				base.OnPresentationImageSelected(sender, e);

				// keep track of this so we can sub/unsub to list events
				this.CurrentOverlayGraphicsProvider = e.SelectedPresentationImage as IOverlayGraphicsProvider; 
			}

			protected override void OnTileSelected(object sender, TileSelectedEventArgs e) {
				base.OnTileSelected(sender, e);

				// OnPresentationImageSelected does not fire when selected tile has no image
				UpdateDeleteAllVisible(); 
			}
		}

		#endregion
	}
}
