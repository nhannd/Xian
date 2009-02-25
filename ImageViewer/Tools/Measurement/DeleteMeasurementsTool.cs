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
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	#region Delete Measurements Graphic Tool

	[MenuAction("closemenu", "basicgraphic-menu/MenuClose")]
	[GroupHint("closemenu", "Tools.Image.Measurement.MenuClose")]
	[Tooltip("closemenu", "MenuClose")]

	[MenuAction("delete", "basicgraphic-menu/MenuDeleteMeasurement", "Delete")]
	[IconSet("delete", IconScheme.Colour, "DeleteMeasurementToolSmall.png", "DeleteMeasurementToolMedium.png", "DeleteMeasurementToolLarge.png")]
	[GroupHint("delete", "Tools.Image.Measurement.Delete")]
	[Tooltip("delete", "MenuDeleteMeasurement")]

	[MenuAction("deleteall", "basicgraphic-menu/MenuDeleteAllMeasurements", "DeleteAll")]
	[IconSet("deleteall", IconScheme.Colour, "DeleteAllMeasurementsToolSmall.png", "DeleteAllMeasurementsToolMedium.png", "DeleteAllMeasurementsToolLarge.png")]
	[GroupHint("deleteall", "Tools.Image.Measurement.Delete.All")]
	[Tooltip("deleteall", "MenuDeleteAllMeasurements")]

	[ExtensionOf(typeof(GraphicToolExtensionPoint))]
	public class DeleteMeasurementsTool : GraphicTool
	{
		public DeleteMeasurementsTool()
		{
		}

		public void Delete()
		{
			DeleteRoiGraphicsHelper.Delete(Context.OwnerGraphic as RoiGraphic);
		}

		public void DeleteAll()
		{
			DeleteRoiGraphicsHelper.DeleteAll(Context.OwnerGraphic.ParentPresentationImage);
		}
	}

	#endregion

	#region Delete All Measurement Graphics Tool (ImageViewerTool)

	[MenuAction("deleteall", "imageviewer-contextmenu/MenuDeleteAllMeasurements", "DeleteAll")]
	[VisibleStateObserver("deleteall", "DeleteAllVisible", "DeleteAllVisibleChanged")]
	[IconSet("deleteall", IconScheme.Colour, "DeleteAllMeasurementsToolSmall.png", "DeleteAllMeasurementsToolMedium.png", "DeleteAllMeasurementsToolLarge.png")]
	[GroupHint("deleteall", "Tools.Image.Measurement.DeleteAll")]
	[Tooltip("deleteall", "MenuDeleteAllMeasurements")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class DeleteAllMeasurementsTool : ImageViewerTool
	{
		private bool _deleteAllVisible;
		private IOverlayGraphicsProvider _currentOverlayProvider;

		public DeleteAllMeasurementsTool()
		{
		}

		public event EventHandler DeleteAllVisibleChanged;

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

		private void OnOverlayGraphicsChanged(object sender, EventArgs e)
		{
			UpdateDeleteAllVisible();
		}

		private void SetCurrentOverlayGraphicsProvider(IOverlayGraphicsProvider provider)
		{
			if (_currentOverlayProvider != provider)
			{
				if (_currentOverlayProvider != null)
				{
					_currentOverlayProvider.OverlayGraphics.ItemAdded -= OnOverlayGraphicsChanged;
					_currentOverlayProvider.OverlayGraphics.ItemChanged -= OnOverlayGraphicsChanged;
					_currentOverlayProvider.OverlayGraphics.ItemRemoved -= OnOverlayGraphicsChanged;
				}

				_currentOverlayProvider = provider;

				if (_currentOverlayProvider != null)
				{
					_currentOverlayProvider.OverlayGraphics.ItemAdded += OnOverlayGraphicsChanged;
					_currentOverlayProvider.OverlayGraphics.ItemChanged += OnOverlayGraphicsChanged;
					_currentOverlayProvider.OverlayGraphics.ItemRemoved += OnOverlayGraphicsChanged;
				}
			}
		}

		private void UpdateDeleteAllVisible()
		{
			DeleteAllVisible = DeleteRoiGraphicsHelper.HasRoiGraphics(base.Context.Viewer.SelectedPresentationImage);
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			SetCurrentOverlayGraphicsProvider(e.SelectedPresentationImage as IOverlayGraphicsProvider);
			UpdateDeleteAllVisible();

			base.OnPresentationImageSelected(sender, e);
		}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			SetCurrentOverlayGraphicsProvider(e.SelectedTile.PresentationImage as IOverlayGraphicsProvider);
			UpdateDeleteAllVisible();

			base.OnTileSelected(sender, e);
		}

		public override void Initialize()
		{
			base.Initialize();
			SetCurrentOverlayGraphicsProvider(null);
			UpdateDeleteAllVisible();
		}

		protected override void Dispose(bool disposing)
		{
			SetCurrentOverlayGraphicsProvider(null);
			base.Dispose(disposing);
		}

		public void DeleteAll()
		{
			DeleteRoiGraphicsHelper.DeleteAll(base.SelectedPresentationImage);
		}
	}

	#endregion

	#region Delete Graphics Helper

	internal static class DeleteRoiGraphicsHelper
	{
		public static void DeleteAll(IPresentationImage image)
		{
			if (image == null || !(image is IOverlayGraphicsProvider))
				return;

			IOverlayGraphicsProvider provider = (IOverlayGraphicsProvider) image;

			List<RoiGraphic> roiGraphics = GetRoiGraphics(provider.OverlayGraphics);
			if (roiGraphics.Count == 0)
				return;

			DrawableUndoableCommand command = new DrawableUndoableCommand(image);
			command.Name = SR.CommandDeleteAllMeasurements;

			foreach (RoiGraphic graphic in roiGraphics)
				command.Enqueue(new RemoveGraphicUndoableCommand(graphic));
			
			command.Execute();

			image.ImageViewer.CommandHistory.AddCommand(command);
		}

		public static void Delete(RoiGraphic roiGraphic)
		{
			if (roiGraphic == null || roiGraphic.ParentPresentationImage == null)
				return;

			IPresentationImage image = roiGraphic.ParentPresentationImage;

			DrawableUndoableCommand command = new DrawableUndoableCommand(image);
			command.Enqueue(new RemoveGraphicUndoableCommand(roiGraphic));
			command.Name = SR.CommandDeleteMeasurement;
			command.Execute();

			image.ImageViewer.CommandHistory.AddCommand(command);
		}

		public static bool HasRoiGraphics(IPresentationImage image)
		{
			if (image != null && image is IOverlayGraphicsProvider)
				return HasRoiGraphics(((IOverlayGraphicsProvider) image).OverlayGraphics);

			return false;
		}

		private static bool HasRoiGraphics(IEnumerable<IGraphic> graphics)
		{
			return GetRoiGraphics(graphics).Count > 0;
		}

		private static List<RoiGraphic> GetRoiGraphics(IEnumerable<IGraphic> graphics)
		{
			List<RoiGraphic> roiGraphics = new List<RoiGraphic>();

			foreach (Graphic graphic in graphics)
			{
				if (graphic is RoiGraphic)
					roiGraphics.Add((RoiGraphic)graphic);
			}

			return roiGraphics;
		}
	}

	#endregion
}
