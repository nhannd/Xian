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
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	public partial class SpatialLocatorTool
	{
		// The crosshair is a container for a graphic that is tied to an imagebox;
		// the container moves the graphic between different images in the imagebox.
		private class CrossHair : IDisposable
		{
			#region Private Fields

			private readonly SpatialLocatorTool _spatialLocatorTool;

			private IPresentationImage _presentationImage;
			private PointF _imagePoint;
			private CrosshairGraphic _crosshairGraphic;
			private bool _dirty;

			#endregion

			public readonly IImageBox ImageBox;

			public CrossHair(IImageBox imageBox, SpatialLocatorTool spatialLocatorTool)
			{
				ImageBox = imageBox;
				_spatialLocatorTool = spatialLocatorTool;

				_crosshairGraphic = new CrosshairGraphic();
				_crosshairGraphic.Drawing += OnGraphicDrawing;
			}

			#region Private Methods

			private void OnGraphicDrawing(object sender, EventArgs e)
			{
				_dirty = false;
			}

			private void RemoveGraphicFromCurrentImage()
			{
				CompositeGraphic parentGraphic = _crosshairGraphic.ParentGraphic as CompositeGraphic;
				if (parentGraphic != null)
					parentGraphic.Graphics.Remove(_crosshairGraphic);
			}

			private void AddGraphicToCurrentImage()
			{
				CompositeGraphic container = _spatialLocatorTool._coordinator.GetSpatialLocatorCompositeGraphic(_presentationImage);
				if (container != null)
				{
					container.Graphics.Add(_crosshairGraphic);
					SetGraphicAnchorPoint();
				}
			}

			private void SetGraphicAnchorPoint()
			{
				if (_presentationImage != null)
				{
					_crosshairGraphic.CoordinateSystem = CoordinateSystem.Source;
					_crosshairGraphic.Anchor = ImagePoint;
					_crosshairGraphic.ResetCoordinateSystem();
				}
			}

			#endregion

			#region Public Properties

			public IPresentationImage Image
			{
				get { return _presentationImage; }
				set
				{
					if (_presentationImage == value)
						return;

					RemoveGraphicFromCurrentImage();

					_presentationImage = value;
					_dirty = true;

					if (_presentationImage == null)
						return;

					ImageBox.TopLeftPresentationImage = _presentationImage;
					AddGraphicToCurrentImage();
				}
			}

			public PointF ImagePoint
			{
				get { return _imagePoint; }
				set
				{
					if (_imagePoint == value)
						return;

					_imagePoint = value;
					_dirty = true;

					SetGraphicAnchorPoint();
				}
			}

			public bool Dirty
			{
				get { return _dirty; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				if (_crosshairGraphic != null)
				{
					_crosshairGraphic.Drawing -= OnGraphicDrawing;

					RemoveGraphicFromCurrentImage();

					_crosshairGraphic.Dispose();
					_crosshairGraphic = null;
				}
			}

			#endregion
		}
	}
}