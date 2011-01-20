#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	internal class ImageBoxMemento : IEquatable<ImageBoxMemento>
	{
		private IDisplaySet _displaySet;
		private bool _displaySetLocked;
		private object _displaySetMemento;
		private int _rows;
		private int _columns;
		private int _topLeftPresentationImageIndex;
		private RectangleF _normalizedRectangle;
		private int _indexOfSelectedTile;

		public ImageBoxMemento(
			IDisplaySet displaySet,
			bool displaySetLocked,
			object displaySetMemento,
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
			_displaySetLocked = displaySetLocked;
			_displaySetMemento = displaySetMemento;
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

		public bool DisplaySetLocked
		{
			get { return _displaySetLocked; }	
		}

		public object DisplaySetMemento
		{
			get { return _displaySetMemento; }	
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

		public override bool Equals(object obj)
		{
			if (obj == this)
				return false;

			if (obj is ImageBoxMemento)
				return this.Equals((ImageBoxMemento) obj);

			return false;
		}

		/// <summary>
		/// Gets a hash code for the object.
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region IEquatable<ImageBoxMemento> Members

		public bool Equals(ImageBoxMemento other)
		{
			if (other == null)
				return false;

			return DisplaySet == other.DisplaySet &&
				   DisplaySetLocked == other.DisplaySetLocked &&
				   Object.Equals(DisplaySetMemento, other.DisplaySetMemento) &&
			       Rows == other.Rows &&
			       Columns == other.Columns &&
			       TopLeftPresentationImageIndex == other.TopLeftPresentationImageIndex &&
			       IndexOfSelectedTile == other.IndexOfSelectedTile &&
			       NormalizedRectangle.Equals(other.NormalizedRectangle);
		}

		#endregion
	}
}
