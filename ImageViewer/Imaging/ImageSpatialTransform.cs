using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class ImageSpatialTransform : SpatialTransform
	{
		private int _sourceWidth;
		private int _sourceHeight;
		private int _destinationWidth;
		private int _destinationHeight;

		private double _pixelSpacingX;
		private double _pixelSpacingY;

		private double _pixelAspectRatioX;
		private double _pixelAspectRatioY;

		public ImageSpatialTransform(
			Graphic parentGraphic,
			int sourceWidth, 
			int sourceHeight,
			double pixelSpacingX,
			double pixelSpacingY,
			double pixelAspectRatioX,
			double pixelAspectRatioY) : base(parentGraphic)
		{
			_sourceWidth = sourceWidth;
			_sourceHeight = sourceHeight;
			_pixelSpacingX = pixelSpacingX;
			_pixelSpacingY = pixelSpacingY;
			_pixelAspectRatioX = pixelAspectRatioX;
			_pixelAspectRatioY = pixelAspectRatioY;
			this.RecalculationRequired = true;
		}

		public int SourceWidth
		{
			get { return _sourceWidth; }
		}

		public int SourceHeight
		{
			get { return _sourceHeight; }
		}

		private int DestinationWidth
		{
			get { return this.ClientRectangle.Width; }
		}

		private int DestinationHeight
		{
			get { return this.ClientRectangle.Height; }
		}

		protected override void CalculatePreTransform()
		{
			// Move origin to center of tile before performing transform
			this.CumulativeTransformInternal.Translate(this.DestinationWidth / 2.0f, this.DestinationHeight / 2.0f);
		}

		protected override void CalculatePostTransform()
		{
			// Move origin to the center of source image after performing transform.
			// This will center the image in the tile
			this.CumulativeTransformInternal.Translate(-this.SourceWidth / 2.0f, -this.SourceHeight / 2.0f);
		}

		protected override void CalculateScale()
		{
			float pixelAspectRatio;

			if (_pixelAspectRatioX == 0 || _pixelAspectRatioY == 0)
			{
				if (_pixelSpacingX == 0 || _pixelSpacingY == 0)
					pixelAspectRatio = 1;
				else
					pixelAspectRatio = (float)_pixelSpacingY / (float)_pixelSpacingX;
			}
			else
			{
				pixelAspectRatio = (float)_pixelAspectRatioY / (float)_pixelAspectRatioX;
			}

			if (this.ScaleToFit)
				CalculateScaleToFit();

			if (pixelAspectRatio >= 1)
			{
				this.ScaleX = this.Scale * pixelAspectRatio;
				this.ScaleY = this.Scale;
			}
			else
			{
				this.ScaleX = this.Scale;
				this.ScaleY = this.Scale / pixelAspectRatio;
			}
		}

		private void CalculateScaleToFit()
		{
			if (this.Rotation == 90 || this.Rotation == 270)
			{
				float imageAspectRatio = (float)this.SourceWidth / (float)this.SourceHeight;
				float clientAspectRatio = (float)this.DestinationHeight / (float)this.DestinationWidth;

				if (clientAspectRatio >= imageAspectRatio)
					this.Scale = (float)this.DestinationWidth / (float)this.SourceHeight;
				else
					this.Scale = (float)this.DestinationHeight / (float)this.SourceWidth;
			}
			else
			{
				float imageAspectRatio = (float)this.SourceHeight / (float)this.SourceWidth;
				float clientAspectRatio = (float)this.DestinationHeight / (float)this.DestinationWidth;

				if (clientAspectRatio >= imageAspectRatio)
					this.Scale = (float)this.DestinationWidth / (float)this.SourceWidth;
				else
					this.Scale = (float)this.DestinationHeight / (float)this.SourceHeight;
			}

			this.MinimumScale = this.Scale / 2;
		}
	}
}
