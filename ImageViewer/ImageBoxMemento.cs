using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Drawing;

namespace ClearCanvas.ImageViewer
{
	internal class ImageBoxMemento : IMemento
	{
		private IDisplaySet _displaySet;
		private int _rows;
		private int _columns;
		private int _topLeftPresentationImageIndex;
		private RectangleF _normalizedRectangle;
		private int _indexOfSelectedTile;

		public ImageBoxMemento(
			IDisplaySet displaySet, 
			int rows,
			int columns,
			int topLeftPresentationImageIndex,
			RectangleF normalizedRectangle,
			int indexOfSelectedTile)
		{
			// displaySet can be null, as that would correspond to an
			// empty imageBox
			Platform.CheckNonNegative(rows, "rows");
			Platform.CheckNonNegative(columns, "columns");
			Platform.CheckNonNegative(_topLeftPresentationImageIndex, "_topLeftPresentationImageIndex");

			_displaySet = displaySet;
			_rows = rows;
			_columns = columns;
			_topLeftPresentationImageIndex = topLeftPresentationImageIndex;
			_normalizedRectangle = normalizedRectangle;
			_indexOfSelectedTile = indexOfSelectedTile;
		}

		public IDisplaySet DisplaySet
		{
			get { return _displaySet; }
		}

		public int Rows
		{
			get { return _rows; }
		}

		public int Columns
		{
			get { return _columns; }
		}

		public int TopLeftPresentationImageIndex
		{
			get { return _topLeftPresentationImageIndex; }
		}

		public RectangleF NormalizedRectangle
		{
			get { return _normalizedRectangle; }
		}

		public int IndexOfSelectedTile
		{
			get { return _indexOfSelectedTile; }
		}
	}
}
