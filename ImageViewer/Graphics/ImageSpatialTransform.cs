using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using System.Drawing.Drawing2D;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An image specific <see cref="SpatialTransform"/>.
	/// </summary>
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
			this.ScaleToFit = true;
			this.RecalculationRequired = true;
		}

		/// <summary>
		/// Gets or sets a value indicating whether images will be scaled to fit
		/// in a <see cref="Tile"/>.
		/// </summary>
		public bool ScaleToFit
		{
			get { return _scaleToFit; }
			set
			{
				_scaleToFit = value;
				this.RecalculationRequired = true;
			}
		}

		private int SourceWidth
		{
			get { return _columns; }
		}

		private int SourceHeight
		{
			get { return _rows; }
		}

		private int DestinationWidth
		{
			get { return this.ClientRectangle.Width; }
		}

		private int DestinationHeight
		{
			get { return this.ClientRectangle.Height; }
		}

		public override IMemento CreateMemento()
		{
			ImageSpatialTransformMemento memento = new ImageSpatialTransformMemento();

			memento.ScaleToFit = _scaleToFit;
			memento.SpatialTransformMemento = base.CreateMemento();

			return memento;
		}

		public override void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			ImageSpatialTransformMemento imageSpatialTransformMemento = memento as ImageSpatialTransformMemento;
			Platform.CheckForInvalidCast(imageSpatialTransformMemento, "memento", "ImageSpatialTransformMemento");

			this.ScaleToFit = _scaleToFit;

			base.SetMemento(imageSpatialTransformMemento.SpatialTransformMemento);
		}

		/// <summary>
		/// Moves the origin to center of Tile.
		/// </summary>
		protected override void CalculatePreTransform(Matrix cumulativeTransform)
		{
			// Move origin to center of tile before performing transform
			cumulativeTransform.Translate(this.DestinationWidth / 2.0f, this.DestinationHeight / 2.0f);
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
		/// Calculates the scale.
		/// </summary>
		/// <remarks>
		/// This scale calculation accounts for non-square pixels.
		/// </remarks>
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
			if (this.RotationXY == 90 || this.RotationXY == 270)
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
