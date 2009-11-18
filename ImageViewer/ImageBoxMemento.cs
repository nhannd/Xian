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
