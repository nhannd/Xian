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
using System.Drawing.Drawing2D;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An image specific <see cref="SpatialTransform"/>.
	/// </summary>
	/// <remarks>
	/// This <see cref="SpatialTransform"/> centers an image in a <see cref="Tile"/>
	/// and provides <see cref="ScaleToFit"/> functionality.
	/// </remarks>
	[Cloneable]
	public class ImageSpatialTransform : SpatialTransform, IImageSpatialTransform
	{
		#region Private Fields

		private bool _scaleToFit;

		private int _columns;
		private int _rows;

		private double _pixelSpacingX;
		private double _pixelSpacingY;

		private double _pixelAspectRatioX;
		private double _pixelAspectRatioY;

		private Rectangle _clientRectangle;

		private float _pixelAspectRatio = 0.0f;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="ImageSpatialTransform"/> with
		/// the specified owner <see cref="IGraphic"/>, width, height, pixel spacing
		/// and pixel aspect ratio.
		/// </summary>
		/// <param name="ownerGraphic"></param>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelSpacingX"></param>
		/// <param name="pixelSpacingY"></param>
		/// <param name="pixelAspectRatioX"></param>
		/// <param name="pixelAspectRatioY"></param>
		public ImageSpatialTransform(
			IGraphic ownerGraphic,
			int rows, 
			int columns,
			double pixelSpacingX,
			double pixelSpacingY,
			double pixelAspectRatioX,
			double pixelAspectRatioY) : base(ownerGraphic)
		{
			_rows = rows;
			_columns = columns;
			_pixelSpacingX = pixelSpacingX;
			_pixelSpacingY = pixelSpacingY;
			_pixelAspectRatioX = pixelAspectRatioX;
			_pixelAspectRatioY = pixelAspectRatioY;
			_scaleToFit = true;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected ImageSpatialTransform(ImageSpatialTransform source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets or sets a value indicating whether images will be scaled to fit
		/// in a <see cref="Tile"/>.
		/// </summary>
		/// <remarks>
		/// If set to <b>true</b>, the <see cref="SpatialTransform.Scale"/> property will be auto-calculated.
		/// </remarks>
		public bool ScaleToFit
		{
			get { return _scaleToFit; }
			set
			{
				if (_scaleToFit == value)
					return;

				_scaleToFit = value;
				base.ForceRecalculation();
			}
		}

#pragma warning disable 1591
#if UNIT_TESTS
		public Rectangle ClientRectangle
#else
		internal Rectangle ClientRectangle
#endif
		{
			get
			{
				// NOTE: we maintain a member variable for the
				// client rectangle for 2 reasons:
				// 1. so we can unit test the class without a need 
				//    for a parent presentation image.
				// 2. So we don't have to unnecessarily recalculate
				//    the scale when the client rectangle has 
				//    not changed.

				return _clientRectangle;
			}
			set 
			{
				if (_clientRectangle == value)
					return;

				_clientRectangle = value;
				base.ForceRecalculation();
			}
		}
#pragma warning restore 1591
		private int SourceWidth
		{
			get { return _columns; }
		}

		private int SourceHeight
		{
			get { return _rows; }
		}

		private float AdjustedSourceHeight
		{
			get { return this.SourceHeight * this.PixelAspectRatio; }
		}

		private float PixelAspectRatio
		{
			get
			{
				if (_pixelAspectRatio == 0)
				{
					if (_pixelAspectRatioX == 0 || _pixelAspectRatioY == 0)
					{
						if (_pixelSpacingX == 0 || _pixelSpacingY == 0)
							_pixelAspectRatio = 1;
						else
							_pixelAspectRatio = (float)_pixelSpacingY / (float)_pixelSpacingX;
					}
					else
					{
						_pixelAspectRatio = (float)_pixelAspectRatioY / (float)_pixelAspectRatioX;
					}
				}

				return _pixelAspectRatio;
			}
		}

		/// <summary>
		/// This methods overrides <see cref="SpatialTransform.CreateMemento"/>.
		/// </summary>
		/// <returns></returns>
		public override object CreateMemento()
		{
			return new ImageSpatialTransformMemento(ScaleToFit, base.CreateMemento());
		}

		/// <summary>
		/// This method overrides <see cref="SpatialTransform.SetMemento"/>.
		/// </summary>
		/// <param name="memento"></param>
		public override void SetMemento(object memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			ImageSpatialTransformMemento imageSpatialTransformMemento = (ImageSpatialTransformMemento) memento;

			this.ScaleToFit = imageSpatialTransformMemento.ScaleToFit;

			base.SetMemento(imageSpatialTransformMemento.SpatialTransformMemento);
		}

		/// <summary>
		/// Moves the origin to center of Tile.
		/// </summary>
		protected override void CalculatePreTransform(Matrix cumulativeTransform)
		{
			// Move origin to center of tile before performing transform
			cumulativeTransform.Translate(this.ClientRectangle.Width / 2.0f, this.ClientRectangle.Height/ 2.0f);
		}

		/// <summary>
		/// Moves the origin to the center of the image.
		/// </summary>
		protected override void CalculatePostTransform(Matrix cumulativeTransform)
		{
			// Move origin to the center of source image after performing transform.
			// This will center the image in the tile
			cumulativeTransform.Translate(-this.SourceWidth / 2.0f, -this.SourceHeight / 2.0f);
		}

		/// <summary>
		/// Updates the scale parameters based on the values of <see cref="ClientRectangle"/> and <see cref="ScaleToFit"/>.
		/// </summary>
		protected override void UpdateScaleParameters()
		{
			if (base.OwnerGraphic != null && base.OwnerGraphic.ParentPresentationImage != null)
				ClientRectangle = base.OwnerGraphic.ParentPresentationImage.ClientRectangle;

			if (!base.RecalculationRequired)
				return;

			if (ScaleToFit)
				CalculateScaleToFit();
			else
				CalculateScaleXY();
		}

		private void CalculateScaleXY()
		{
			float scaleX, scaleY;

			if (this.PixelAspectRatio >= 1)
			{
				scaleX = this.Scale;
				scaleY = this.Scale * this.PixelAspectRatio;
			}
			else
			{
				scaleX = this.Scale / this.PixelAspectRatio;
				scaleY = this.Scale;
			}

			this.ScaleX = scaleX;
			this.ScaleY = scaleY;
		}

		private void CalculateScaleToFit()
		{
			//don't ever calculate the 'scale to fit' for an empty rectangle.
			int destinationWidth = Math.Max(10, this.ClientRectangle.Width);
			int destinationHeight = Math.Max(10, this.ClientRectangle.Height);

			float scaleX, scaleY;

			if (this.RotationXY == 90 || this.RotationXY == 270)
			{
				float imageAspectRatio = (float)this.SourceWidth / this.AdjustedSourceHeight;
				float clientAspectRatio = (float)destinationHeight / (float)destinationWidth;

				if (clientAspectRatio >= imageAspectRatio)
				{
					scaleX = (float)destinationWidth / this.AdjustedSourceHeight;
					scaleY = (float)destinationWidth / this.SourceHeight;
				}
				else
				{
					scaleX = (float)destinationHeight / (float)this.SourceWidth;
					scaleY = (float)destinationHeight / (float)this.SourceWidth * this.PixelAspectRatio;
				}
			}
			else
			{
				float imageAspectRatio = this.AdjustedSourceHeight / (float)this.SourceWidth;
				float clientAspectRatio = (float)destinationHeight / (float)destinationWidth;

				if (clientAspectRatio >= imageAspectRatio)
				{
					scaleX = (float)destinationWidth / (float)this.SourceWidth;
					scaleY = (float)destinationWidth / (float)this.SourceWidth * this.PixelAspectRatio;
				}
				else
				{
					scaleX = (float)destinationHeight / this.AdjustedSourceHeight;
					scaleY = (float)destinationHeight / this.SourceHeight;
				}
			}

			this.Scale = scaleX;
			this.ScaleX = scaleX;
			this.ScaleY = scaleY;
		}
	}
}
