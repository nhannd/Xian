#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer
{
	internal class PhysicalWorkspaceMemento
	{
		private ImageBoxCollection _imageBoxes;
		private List<object> _imageBoxMementos;
		private int _rows;
		private int _columns;

		public PhysicalWorkspaceMemento(
			ImageBoxCollection imageBoxes,
			List<object> imageBoxMementos,
			int rows,
			int columns)
		{
			Platform.CheckForNullReference(imageBoxes, "imageBoxes");
			Platform.CheckForNullReference(imageBoxMementos, "imageBoxMementos");

			_imageBoxes = imageBoxes;
			_imageBoxMementos = imageBoxMementos;
			_rows = rows;
			_columns = columns;
		}

		public ImageBoxCollection ImageBoxes
		{
			get { return _imageBoxes; }
		}

		public List<object> ImageBoxMementos
		{
			get { return _imageBoxMementos; }
		}

		public int Rows
		{
			get { return _rows; }
		}

		public int Columns
		{
			get { return _columns; }
		}
	}
}
