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

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Common;
using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("clear", "imageviewer-contextmenu/MenuClearCustomShutters", "Clear")]
	[IconSet("clear", IconScheme.Colour, "Icons.ClearCustomShuttersToolSmall.png", "Icons.ClearCustomShuttersToolMedium.png", "Icons.ClearCustomShuttersToolLarge.png")]
	[VisibleStateObserver("clear", "Visible", "VisibleChanged")]

	[ButtonAction("clearToolbar", "global-toolbars/ToolbarStandard/ToolbarClearCustomShutters", "Clear")]
	[Tooltip("clearToolbar", "TooltipClearCustomShutters")]
	[IconSet("clearToolbar", IconScheme.Colour, "Icons.ClearCustomShuttersToolSmall.png", "Icons.ClearCustomShuttersToolMedium.png", "Icons.ClearCustomShuttersToolLarge.png")]
	[EnabledStateObserver("clearToolbar", "Visible", "VisibleChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ClearCustomShuttersTool : ImageViewerTool
	{
		private bool _visible;

		public ClearCustomShuttersTool()
		{
		}

		public bool Visible
		{
			get { return _visible; }	
			set
			{
				if (_visible == value)
					return;

				_visible = value;
				EventsHelper.Fire(VisibleChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler VisibleChanged;

		public override void Initialize()
		{
			base.Initialize();
			base.Context.Viewer.EventBroker.ImageDrawing += OnImageDrawing;
		}

		protected override void Dispose(bool disposing)
		{
			base.Context.Viewer.EventBroker.ImageDrawing -= OnImageDrawing;
			base.Dispose(disposing);
		}

		public void Clear()
		{
			if (base.SelectedPresentationImage == null)
				return;

			if (base.SelectedPresentationImage is IDicomPresentationImage)
			{
				IDicomPresentationImage dicomImage = (IDicomPresentationImage)base.SelectedPresentationImage;
				GeometricShuttersGraphic shuttersGraphic = DrawShutterTool.GetGeometricShuttersGraphic(dicomImage);
				DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(shuttersGraphic);
				foreach (GeometricShutter shutter in shuttersGraphic.CustomShutters)
					historyCommand.Enqueue(new RemoveGeometricShutterUndoableCommand(shuttersGraphic, shutter));

				historyCommand.Execute();

				historyCommand.Name = SR.CommandClearCustomShutters;
				base.Context.Viewer.CommandHistory.AddCommand(historyCommand);
				Visible = false;
			}
		}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			base.OnTileSelected(sender, e);
			UpdateVisible();
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.OnPresentationImageSelected(sender, e);
			UpdateVisible();
		}

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			if (e.PresentationImage == base.SelectedPresentationImage)
				UpdateVisible();
		}

		private void UpdateVisible()
		{
			Visible = HasCustomShutters(base.SelectedPresentationImage);
		}

		private static bool HasCustomShutters(IPresentationImage image)
		{
			if (image != null && image is IDicomPresentationImage)
			{
				IDicomPresentationImage dicomImage = (IDicomPresentationImage)image;
				GeometricShuttersGraphic shuttersGraphic = DrawShutterTool.GetGeometricShuttersGraphic(dicomImage);
				if (shuttersGraphic != null)
					return shuttersGraphic.CustomShutters.Count > 0;
			}

			return false;
		}
	}
}
