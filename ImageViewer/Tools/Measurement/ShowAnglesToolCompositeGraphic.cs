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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	partial class ShowAnglesTool
	{
		[Cloneable(true)]
		private partial class ShowAnglesToolCompositeGraphic : CompositeGraphic
		{
			private IPointsGraphic _selectedLine;
			private ShowAnglesTool _ownerTool;

			[CloneIgnore]
			private bool _isDirty = true;

			public ShowAnglesToolCompositeGraphic(ShowAnglesTool ownerTool) : this()
			{
				_ownerTool = ownerTool;
			}

			private ShowAnglesToolCompositeGraphic() : base()
			{
				_selectedLine = null;
			}

			protected override void Dispose(bool disposing)
			{
				_ownerTool = null;

				base.Dispose(disposing);
			}

			public override bool Visible
			{
				get
				{
					if (_ownerTool != null && !_ownerTool.ShowAngles)
						return false;
					return base.Visible;
				}
				set { base.Visible = value; }
			}

			protected override void OnParentPresentationImageChanged(IPresentationImage oldParentPresentationImage, IPresentationImage newParentPresentationImage)
			{
				if (oldParentPresentationImage != null)
				{
					IOverlayGraphicsProvider overlayGraphicsProvider = oldParentPresentationImage as IOverlayGraphicsProvider;
					if (overlayGraphicsProvider != null)
					{
						overlayGraphicsProvider.OverlayGraphics.ItemChanging -= OnOverlayGraphicsItemRemoved;
						overlayGraphicsProvider.OverlayGraphics.ItemRemoved -= OnOverlayGraphicsItemRemoved;
					}
				}

				base.OnParentPresentationImageChanged(oldParentPresentationImage, newParentPresentationImage);

				if (newParentPresentationImage != null)
				{
					IOverlayGraphicsProvider overlayGraphicsProvider = newParentPresentationImage as IOverlayGraphicsProvider;
					if (overlayGraphicsProvider != null)
					{
						overlayGraphicsProvider.OverlayGraphics.ItemRemoved += OnOverlayGraphicsItemRemoved;
						overlayGraphicsProvider.OverlayGraphics.ItemChanging += OnOverlayGraphicsItemRemoved;
					}
				}

				_isDirty = true;
			}

			private void OnOverlayGraphicsItemRemoved(object sender, ListEventArgs<IGraphic> e)
			{
				IPointsGraphic lineGraphic = GetLine(e.Item);
				if (ReferenceEquals(_selectedLine, lineGraphic))
				{
					this.Select(null);
				}
			}

			public void Select(IGraphic graphic)
			{
				if (graphic != null)
				{
					Platform.CheckFalse(ReferenceEquals(graphic.ParentPresentationImage, null), "Supplied graphic must be on the same presentation image.");
					Platform.CheckTrue(ReferenceEquals(graphic.ParentPresentationImage, this.ParentPresentationImage), "Supplied graphic must be on the same presentation image.");
				}

				IPointsGraphic value = GetLine(graphic);
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

					_isDirty = true;
				}
			}

			public override void OnDrawing()
			{
				base.OnDrawing();

				if (!_isDirty)
					return;

				IOverlayGraphicsProvider overlayGraphicsProvider = this.ParentPresentationImage as IOverlayGraphicsProvider;
				if (overlayGraphicsProvider == null)
					return;

				IList<ShowAnglesToolGraphic> freeAngleGraphics = CollectionUtils.Cast<ShowAnglesToolGraphic>(this.Graphics);

				if (this.Visible && _selectedLine != null && _selectedLine.Points.Count == 2)
				{
					_selectedLine.CoordinateSystem = CoordinateSystem.Source;
					try
					{
						foreach (IGraphic otherLineGraphic in overlayGraphicsProvider.OverlayGraphics)
						{
							IPointsGraphic otherLine = GetLine(otherLineGraphic);
							if (otherLine != null && !ReferenceEquals(otherLine, _selectedLine) && otherLine.Points.Count == 2)
							{
								ShowAnglesToolGraphic showAnglesToolGraphic;
								if (freeAngleGraphics.Count > 0)
									freeAngleGraphics.Remove(showAnglesToolGraphic = freeAngleGraphics[0]);
								else
									this.Graphics.Add(showAnglesToolGraphic = new ShowAnglesToolGraphic());

								showAnglesToolGraphic.CoordinateSystem = otherLine.CoordinateSystem = CoordinateSystem.Source;
								try
								{
									showAnglesToolGraphic.SetEndpoints(_selectedLine.Points[0], _selectedLine.Points[1], otherLine.Points[0], otherLine.Points[1]);
								}
								finally
								{
									showAnglesToolGraphic.ResetCoordinateSystem();
									otherLine.ResetCoordinateSystem();
								}
							}
						}
					}
					finally
					{
						_selectedLine.ResetCoordinateSystem();
					}
				}

				foreach (IGraphic freeAngleGraphic in freeAngleGraphics)
				{
					this.Graphics.Remove(freeAngleGraphic);
					freeAngleGraphic.Dispose();
				}
			}

			private void OnSelectedLinePointsCleared(object sender, EventArgs e)
			{
				_isDirty = true;
			}

			private void OnSelectedLinePointChanged(object sender, IndexEventArgs e)
			{
				_isDirty = true;
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
}