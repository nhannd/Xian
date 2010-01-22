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
using System.Collections.Generic;
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
		private IPointsGraphic _selectedLine;
		private bool _showAngles = false;

		public override void Initialize()
		{
			base.Initialize();

			_linesChangedEvent = new DelayedEventPublisher(OnLinesChanged, 15);

			base.ImageViewer.EventBroker.GraphicSelectionChanged += OnGraphicSelectionChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.ImageViewer.EventBroker.GraphicSelectionChanged -= OnGraphicSelectionChanged;

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
			if (_selectedLine != null
			    && _selectedLine.ParentPresentationImage is IOverlayGraphicsProvider)
			{
				IOverlayGraphicsProvider overlayGraphicsProvider = ((IOverlayGraphicsProvider) _selectedLine.ParentPresentationImage);
				IList<IGraphic> freeAngleGraphics = CollectionUtils.Select(overlayGraphicsProvider.OverlayGraphics, g => g is ShowAnglesToolGraphic);

				bool any = false;
				if (_showAngles)
				{
					IList<IGraphic> otherGraphics = new List<IGraphic>(overlayGraphicsProvider.OverlayGraphics);
					foreach (IGraphic line in otherGraphics)
					{
						IPointsGraphic otherLine = null;
						SetLineGraphic(ref otherLine, line);
						if (otherLine != null && !ReferenceEquals(otherLine, _selectedLine))
							any |= DrawAngles(_selectedLine, otherLine, freeAngleGraphics);
					}
				}

				any |= freeAngleGraphics.Count > 0;
				foreach (IGraphic freeAngleGraphic in freeAngleGraphics)
				{
					overlayGraphicsProvider.OverlayGraphics.Remove(freeAngleGraphic);
					freeAngleGraphic.Dispose();
				}

				if (any)
					_selectedLine.ParentPresentationImage.Draw();
			}
		}

		private static bool DrawAngles(IPointsGraphic _selectedLine, IPointsGraphic _focusedLine, IList<IGraphic> freeAngleGraphics)
		{
			if (_selectedLine.Points.Count == 2 && _focusedLine.Points.Count == 2)
			{
				// base.SelectedOverlayGraphicsProvider is NOT necessarily the same as _selectedLine.ParentPresentationImage
				IOverlayGraphicsProvider overlayGraphicsProvider = _selectedLine.ParentPresentationImage as IOverlayGraphicsProvider;
				if (overlayGraphicsProvider == null)
					return false;

				GraphicCollection targetOverlayGraphics = overlayGraphicsProvider.OverlayGraphics;

				ShowAnglesToolGraphic showAnglesToolGraphic = null;
				if (freeAngleGraphics.Count > 0)
				{
					showAnglesToolGraphic = (ShowAnglesToolGraphic) freeAngleGraphics[0];
					freeAngleGraphics.RemoveAt(0);
				}
				else
				{
					showAnglesToolGraphic = new ShowAnglesToolGraphic();
					targetOverlayGraphics.Add(showAnglesToolGraphic);
				}

				if (targetOverlayGraphics != null)
				{
					showAnglesToolGraphic.Set(_selectedLine.Points[0], _selectedLine.Points[1], _focusedLine.Points[0], _focusedLine.Points[1]);
					return true;
				}
			}
			return false;
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

		private void OnGraphicSelectionChanged(object sender, GraphicSelectionChangedEventArgs e)
		{
			this.SetLineGraphic(ref _selectedLine, e.SelectedGraphic);

			if (e.SelectedGraphic == null)
			{
				// if we're not focusing on anything, hide the angle immediately
				_linesChangedEvent.PublishNow(this, EventArgs.Empty);
			}
			else
			{
				_linesChangedEvent.Publish(this, EventArgs.Empty);
			}
		}
	}
}