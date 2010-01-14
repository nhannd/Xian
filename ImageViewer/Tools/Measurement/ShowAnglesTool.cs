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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ButtonAction("activate", "global-toolbars/MenuShowAngles", "ToggleShowAngles")]
	[CheckedStateObserver("activate", "ShowAngles", "ShowAnglesChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.ShowAnglesToolSmall.png", "Icons.ShowAnglesToolMedium.png", "Icons.ShowAnglesToolLarge.png")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public partial class ShowAnglesTool : ImageViewerTool
	{
		private event EventHandler _showAnglesChanged;
		private DelayedEventPublisher _linesChangedEvent;
		private ShowAnglesToolGraphic _showAnglesToolGraphic;
		private IPointsGraphic _selectedLine;
		private IPointsGraphic _focusedLine;
		private bool _showAngles = false;

		public override void Initialize()
		{
			base.Initialize();

			_linesChangedEvent = new DelayedEventPublisher(OnLinesChanged);
			_showAnglesToolGraphic = new ShowAnglesToolGraphic();

			base.ImageViewer.EventBroker.GraphicFocusChanged += OnGraphicFocusChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.ImageViewer.EventBroker.GraphicFocusChanged -= OnGraphicFocusChanged;

				if (_showAnglesToolGraphic != null)
				{
					_showAnglesToolGraphic.Dispose();
					_showAnglesToolGraphic = null;
				}

				if (_linesChangedEvent != null)
				{
					_linesChangedEvent.Dispose();
					_linesChangedEvent = null;
				}
			}
			base.Dispose(disposing);
		}

		private void OnLinesChanged(object sender, EventArgs e)
		{
			GraphicCollection targetOverlayGraphics = null;
			if (_showAngles // only if tool is enabled
			    && !ReferenceEquals(_selectedLine, _focusedLine) // and we have to distinct lines
			    && !ReferenceEquals(_selectedLine, null) && !ReferenceEquals(_focusedLine, null) // and neither are null
			    && _selectedLine.Points.Count == 2 && _focusedLine.Points.Count == 2 // and both have two end points
			    && _selectedLine.ParentPresentationImage == _focusedLine.ParentPresentationImage) // and both are on the same image
			{
				// base.SelectedOverlayGraphicsProvider is NOT necessarily the same as _selectedLine.ParentPresentationImage
				IOverlayGraphicsProvider overlayGraphicsProvider = _selectedLine.ParentPresentationImage as IOverlayGraphicsProvider;
				if (overlayGraphicsProvider != null)
					targetOverlayGraphics = overlayGraphicsProvider.OverlayGraphics;
			}

			// the graphic must be moved before setting the data points, due to potential need for source/destination coordinate conversion
			TranslocateGraphic(_showAnglesToolGraphic, targetOverlayGraphics);

			if (targetOverlayGraphics != null)
			{
				_showAnglesToolGraphic.Set(_selectedLine.Points[0], _selectedLine.Points[1], _focusedLine.Points[0], _focusedLine.Points[1]);
				_showAnglesToolGraphic.Draw();
			}


			//IOverlayGraphicsProvider overlayGraphicsProvider;
			//if (_showAngles && _selectedLine != _focusedLine
			//    && _selectedLine != null && _selectedLine.Points.Count == 2
			//    && _focusedLine != null && _focusedLine.Points.Count == 2
			//    && _selectedLine.ParentPresentationImage == _focusedLine.ParentPresentationImage) {
			//    // base.SelectedOverlayGraphicsProvider is NOT necessarily the same as _selectedLine.ParentPresentationImage
			//    overlayGraphicsProvider = _selectedLine.ParentPresentationImage as IOverlayGraphicsProvider;
			//    if (overlayGraphicsProvider != null) {
			//        TranslocateGraphic(_showAnglesToolGraphic, overlayGraphicsProvider.OverlayGraphics);

			//        // the graphic must be moved before setting the data points, due to need for source/destination image coordinate conversion
			//        _showAnglesToolGraphic.Set(_selectedLine.Points[0], _selectedLine.Points[1], _focusedLine.Points[0], _focusedLine.Points[1]);
			//        _showAnglesToolGraphic.Draw();
			//    } else {
			//        TranslocateGraphic(_showAnglesToolGraphic, null);
			//    }
			//} else {
			//    TranslocateGraphic(_showAnglesToolGraphic, null);
			//}
		}

		public bool ShowAngles
		{
			get { return _showAngles; }
			set
			{
				if (_showAngles != value)
				{
					_showAngles = value;
					_linesChangedEvent.PublishNow(this, EventArgs.Empty);
					EventsHelper.Fire(_showAnglesChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler ShowAnglesChanged
		{
			add { _showAnglesChanged += value; }
			remove { _showAnglesChanged -= value; }
		}

		public void ToggleShowAngles()
		{
			this.ShowAngles = !this.ShowAngles;
		}

		private void SetLineGraphic(ref IPointsGraphic lineVariable, IGraphic value)
		{
			if (lineVariable != null)
				lineVariable.Points.PointChanged -= OnLineGraphicPointChanged;

			if (value is IControlGraphic)
				lineVariable = ((IControlGraphic) value).Subject as IPointsGraphic;
			else
				lineVariable = value as IPointsGraphic;

			if (lineVariable != null)
				lineVariable.Points.PointChanged += OnLineGraphicPointChanged;
		}

		private void OnLineGraphicPointChanged(object sender, IndexEventArgs e)
		{
			_linesChangedEvent.Publish(this, EventArgs.Empty);
		}

		private void OnGraphicFocusChanged(object sender, GraphicFocusChangedEventArgs e)
		{
			this.SetLineGraphic(ref _focusedLine, e.FocusedGraphic);

			if (e.FocusedGraphic == null)
			{
				// if we're not focusing on anything, hide the angle immediately
				_linesChangedEvent.PublishNow(this, EventArgs.Empty);
			}
			else
			{
				// always pick the selected graphic from the same presentation image as the current focused graphic
				// there's no advantage to observing selected graphic change events since such an event is invariably preceded by a focus change
				if (e.FocusedGraphic.ParentPresentationImage != null)
					this.SetLineGraphic(ref _selectedLine, e.FocusedGraphic.ParentPresentationImage.SelectedGraphic);

				_linesChangedEvent.Publish(this, EventArgs.Empty);
			}
		}

		private static void TranslocateGraphic(IGraphic graphic, GraphicCollection target)
		{
			try
			{
				if (target != null && target.Contains(graphic))
					return;

				if (graphic.ParentGraphic is CompositeGraphic)
				{
					IGraphic oldParent = graphic.ParentGraphic;
					((CompositeGraphic) graphic.ParentGraphic).Graphics.Remove(graphic);
					oldParent.Draw();
				}

				if (target != null)
				{
					target.Add(graphic);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Unexpected error");
				throw;
			}
		}
	}
}