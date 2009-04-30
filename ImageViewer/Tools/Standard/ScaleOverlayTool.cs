#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("showHide", "global-menus/MenuTools/MenuStandard/MenuShowHideScaleOverlay", "ShowHide")]
	[Tooltip("showHide", "TooltipShowHideScaleOverlay")]
	[GroupHint("showHide", "Tools.Image.Overlays.Scale.ShowHide")]
	[IconSet("showHide", IconScheme.Colour, "Icons.ScaleOverlayToolSmall.png", "Icons.ScaleOverlayToolMedium.png", "Icons.ScaleOverlayToolLarge.png")]
	//
	[ButtonAction("toggle", "overlays-dropdown/ToolbarScaleOverlay", "ShowHide")]
	[CheckedStateObserver("toggle", "Checked", "CheckedChanged")]
	[Tooltip("toggle", "TooltipScaleOverlay")]
	[GroupHint("toggle", "Tools.Image.Overlays.Scale.ShowHide")]
	[IconSet("toggle", IconScheme.Colour, "Icons.ScaleOverlayToolSmall.png", "Icons.ScaleOverlayToolMedium.png", "Icons.ScaleOverlayToolLarge.png")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ScaleOverlayTool : OverlayToolBase
	{
		public ScaleOverlayTool()
		{
		}

		protected override void UpdateVisibility(IPresentationImage image, bool visible)
		{
			CompositeScaleGraphic scale = GetCompositeScaleGraphic(image, visible);
			if (scale != null)
				scale.Visible = visible;
		}

		private static CompositeScaleGraphic GetCompositeScaleGraphic(IPresentationImage image, bool createIfNull)
		{
			if (image is IApplicationGraphicsProvider)
			{
				GraphicCollection overlayGraphics = ((IApplicationGraphicsProvider)image).ApplicationGraphics;
				CompositeScaleGraphic scale = CollectionUtils.SelectFirst(overlayGraphics,
				                                                          delegate(IGraphic graphic) { return graphic is CompositeScaleGraphic; }
				                              	) as CompositeScaleGraphic;

				if (scale == null && createIfNull)
					overlayGraphics.Insert(0, scale = new CompositeScaleGraphic());

				return scale;
			}

			return null;
		}

		private class CompositeScaleGraphic : CompositeGraphic
		{
			private event EventHandler _changed;
			private readonly ScaleGraphic _horizontalScale;
			private readonly ScaleGraphic _verticalScale;

			public CompositeScaleGraphic()
			{
				base.Graphics.Add(_horizontalScale = new ScaleGraphic());
				base.Graphics.Add(_verticalScale = new ScaleGraphic());

				_horizontalScale.Visible = false;
				_horizontalScale.IsMirrored = true;

				_verticalScale.Visible = false;
				_verticalScale.IsMirrored = true;
			}

			/// <summary>
			/// Indicates that the ScaleGraphic have changed.
			/// </summary>
			public event EventHandler Changed
			{
				add { _changed += value; }
				remove { _changed -= value; }
			}

			/// <summary>
			/// Gets or sets a value indicating the visibility of the <see cref="IGraphic"/>.
			/// </summary>
			public override bool Visible
			{
				get { return base.Visible; }
				set
				{
					if (base.Visible != value)
					{
						base.Visible = value;
						OnChanged();
					}
				}
			}

			/// <summary>
			/// Fires the <see cref="Graphic.Drawing"/> event.  Should be called by an <see cref="IRenderer"/>
			/// for each object just before it is drawn/rendered, hence the reason it is public.
			/// </summary>
			public override void OnDrawing()
			{
				base.CoordinateSystem = CoordinateSystem.Destination;
				_horizontalScale.CoordinateSystem = CoordinateSystem.Destination;
				_verticalScale.CoordinateSystem = CoordinateSystem.Destination;

				try
				{
					Rectangle hScaleBounds = ComputeScaleBounds(base.ParentPresentationImage, 0.10f, 0.05f);
					_horizontalScale.SetEndPoints(new PointF(hScaleBounds.Left, hScaleBounds.Bottom), new SizeF(hScaleBounds.Width, 0));
					_horizontalScale.Visible = true;

					Rectangle vScaleBounds = ComputeScaleBounds(base.ParentPresentationImage, 0.05f, 0.10f);
					_verticalScale.SetEndPoints(new PointF(vScaleBounds.Right, vScaleBounds.Top), new SizeF(0, vScaleBounds.Height));
					_verticalScale.Visible = true;
				}
				catch (UncalibratedImageException)
				{
					_horizontalScale.Visible = false;
					_verticalScale.Visible = false;
				}
				finally
				{
					base.ResetCoordinateSystem();
					_horizontalScale.ResetCoordinateSystem();
					_verticalScale.ResetCoordinateSystem();
				}

				base.OnDrawing();
			}

			protected virtual void OnChanged()
			{
				EventsHelper.Fire(_changed, this, EventArgs.Empty);
				base.Draw();
			}

			/// <summary>
			/// Computes the maximum bounds for scales on a given <see cref="IPresentationImage"/>.
			/// </summary>
			/// <param name="presentationImage">The image to compute bounds for.</param>
			/// <param name="horizontalReduction">The percentage of width to subtract from both the client bounds and the source image bounds.</param>
			/// <param name="verticalReduction">The percentage of height to subtract from both the client bounds and the source image bounds.</param>
			/// <returns>The maximum scale bounds.</returns>
			private static Rectangle ComputeScaleBounds(IPresentationImage presentationImage, float horizontalReduction, float verticalReduction)
			{
				RectangleF clientBounds = presentationImage.ClientRectangle;
				float hReduction = horizontalReduction*Math.Min(1000f, clientBounds.Width);
				float vReduction = verticalReduction*Math.Min(1000f, clientBounds.Height);

				clientBounds = new RectangleF(clientBounds.X + hReduction, clientBounds.Y + vReduction, clientBounds.Width - 2*hReduction, clientBounds.Height - 2*vReduction);

				if (presentationImage is IImageGraphicProvider)
				{
					ImageGraphic imageGraphic = ((IImageGraphicProvider) presentationImage).ImageGraphic;
					Rectangle srcRectangle = new Rectangle(0, 0, imageGraphic.Columns, imageGraphic.Rows);

					RectangleF imageBounds = imageGraphic.SpatialTransform.ConvertToDestination(srcRectangle);
					imageBounds = RectangleUtilities.ConvertToPositiveRectangle(imageBounds);
					hReduction = horizontalReduction*imageBounds.Width;
					vReduction = verticalReduction*imageBounds.Height;

					imageBounds = new RectangleF(imageBounds.X + hReduction, imageBounds.Y + vReduction, imageBounds.Width - 2*hReduction, imageBounds.Height - 2*vReduction);
					return Rectangle.Round(RectangleUtilities.Intersect(imageBounds, clientBounds));
				}
				else
				{
					return Rectangle.Round(clientBounds);
				}
			}
		}
	}
}