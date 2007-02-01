using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class CompositeImageGraphic : CompositeGraphic
	{
		private int _rows;
		private int _columns;
		private double _pixelSpacingX;
		private double _pixelSpacingY;
		private double _pixelAspectRatioX;
		private double _pixelAspectRatioY;

		public CompositeImageGraphic(
			int rows, 
			int columns) : this(rows, columns, 0, 0)
		{
		}

		public CompositeImageGraphic(
			int rows,
			int columns,
			double pixelSpacingX,
			double pixelSpacingY) : this(rows, columns, pixelSpacingX, pixelSpacingY, 0, 0)
		{
		}

		public CompositeImageGraphic(
				int rows,
				int columns,
				double pixelSpacingX,
				double pixelSpacingY,
				double pixelAspectRatioX,
				double pixelAspectRatioY)
		{
			_rows = rows;
			_columns = columns;
			_pixelSpacingX = pixelSpacingX;
			_pixelSpacingY = pixelSpacingY;
			_pixelAspectRatioX = pixelAspectRatioX;
			_pixelAspectRatioY = pixelAspectRatioY;
		}
		
		protected override SpatialTransform CreateSpatialTransform()
		{
			return new ImageSpatialTransform(
				this,
				_columns,
				_rows,
				_pixelSpacingX,
				_pixelSpacingY,
				_pixelAspectRatioX,
				_pixelAspectRatioY);
		}
	}
}
