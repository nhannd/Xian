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
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuShowAngles", "ToggleShowAngles")]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarShowAngles", "ToggleShowAngles")]
	[CheckedStateObserver("activate", "ShowAngles", "ShowAnglesChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.ShowAnglesToolSmall.png", "Icons.ShowAnglesToolMedium.png", "Icons.ShowAnglesToolLarge.png")]
	[Tooltip("activate", "TooltipShowAngles")]
	[GroupHint("activate", "Tools.Image.Measurement.Angle")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public partial class ShowAnglesTool : ImageViewerTool
	{
		private event EventHandler _showAnglesChanged;
		private DelayedEventPublisher _updateEventPublisher;
		private IPointsGraphic _selectedLine;
		private bool _showAngles = false;

		public override void Initialize()
		{
			base.Initialize();

			_updateEventPublisher = new DelayedEventPublisher(OnUpdate, 15);

			base.ImageViewer.EventBroker.GraphicSelectionChanged += OnGraphicSelectionChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.ImageViewer.EventBroker.GraphicSelectionChanged -= OnGraphicSelectionChanged;

				if (_updateEventPublisher != null)
				{
					_updateEventPublisher.Dispose();
					_updateEventPublisher = null;
				}
			}
			base.Dispose(disposing);
		}

		public IPointsGraphic SelectedLine
		{
			get { return _selectedLine; }
			set
			{
				if (_selectedLine != value)
				{
					if (_selectedLine != null)
					{
						_selectedLine.Points.PointAdded -= OnSelectedLinePointChanged;
						_selectedLine.Points.PointChanged -= OnSelectedLinePointChanged;
						_selectedLine.Points.PointRemoved -= OnSelectedLinePointChanged;
						_selectedLine.Points.PointsCleared -= OnSelectedLinePointsCleared;
					}

					_selectedLine = value;

					if (_selectedLine != null)
					{
						_selectedLine.Points.PointAdded += OnSelectedLinePointChanged;
						_selectedLine.Points.PointChanged += OnSelectedLinePointChanged;
						_selectedLine.Points.PointRemoved += OnSelectedLinePointChanged;
						_selectedLine.Points.PointsCleared += OnSelectedLinePointsCleared;
					}
				}
			}
		}

		private void OnSelectedLinePointsCleared(object sender, EventArgs e)
		{
			_updateEventPublisher.Publish(this, EventArgs.Empty);
		}

		private void OnSelectedLinePointChanged(object sender, IndexEventArgs e)
		{
			_updateEventPublisher.Publish(this, EventArgs.Empty);
		}

		private void OnUpdate(object sender, EventArgs e)
		{
			IPresentationImage ownerImage = null;
			if (this.SelectedLine != null)
				ownerImage = this.SelectedLine.ParentPresentationImage;
			else
				ownerImage = base.SelectedPresentationImage;

			if (ownerImage is IOverlayGraphicsProvider)
			{
				IOverlayGraphicsProvider overlayGraphicsProvider = (IOverlayGraphicsProvider) ownerImage;
				IList<IGraphic> freeAngleGraphics = CollectionUtils.Select(overlayGraphicsProvider.OverlayGraphics, g => g is ShowAnglesToolGraphic);

				bool drawRequired = false;
				if (_showAngles && this.SelectedLine != null)
				{
					IList<IGraphic> otherGraphics = new List<IGraphic>(overlayGraphicsProvider.OverlayGraphics);
					foreach (IGraphic line in otherGraphics)
					{
						IPointsGraphic otherLine = GetLine(line);
						if (otherLine != null && !ReferenceEquals(otherLine, this.SelectedLine))
							drawRequired |= DrawAngles(this.SelectedLine, otherLine, freeAngleGraphics);
					}
				}

				drawRequired |= freeAngleGraphics.Count > 0;
				foreach (IGraphic freeAngleGraphic in freeAngleGraphics)
				{
					overlayGraphicsProvider.OverlayGraphics.Remove(freeAngleGraphic);
					freeAngleGraphic.Dispose();
				}

				if (drawRequired)
					ownerImage.Draw();
			}
		}

		private static bool DrawAngles(IPointsGraphic baseLine, IPointsGraphic otherLine, IList<IGraphic> freeAngleGraphics)
		{
			if (baseLine == null || otherLine == null || baseLine.ParentPresentationImage != otherLine.ParentPresentationImage)
				return false;

			if (baseLine.Points.Count == 2 && otherLine.Points.Count == 2)
			{
				IOverlayGraphicsProvider overlayGraphicsProvider = baseLine.ParentPresentationImage as IOverlayGraphicsProvider;
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
					showAnglesToolGraphic.Set(baseLine.Points[0], baseLine.Points[1], otherLine.Points[0], otherLine.Points[1]);
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
					_updateEventPublisher.PublishNow(this, EventArgs.Empty);
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

		private void HandleGraphicSelectionChange(IGraphic graphic)
		{
			this.SelectedLine = GetLine(graphic);

			if (graphic == null)
			{
				// if we're not focusing on anything, hide the angle immediately
				_updateEventPublisher.PublishNow(this, EventArgs.Empty);
			}
			else
			{
				_updateEventPublisher.Publish(this, EventArgs.Empty);
			}
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.OnPresentationImageSelected(sender, e);

			if (e.SelectedPresentationImage != null)
			{
				this.HandleGraphicSelectionChange(e.SelectedPresentationImage.SelectedGraphic);
			}
		}

		private void OnGraphicSelectionChanged(object sender, GraphicSelectionChangedEventArgs e)
		{
			this.HandleGraphicSelectionChange(e.SelectedGraphic);
		}

		private static IPointsGraphic GetLine(IGraphic graphic)
		{
			if (graphic is IControlGraphic)
				return ((IControlGraphic) graphic).Subject as IPointsGraphic;
			else
				return graphic as IPointsGraphic;
		}
	}
}