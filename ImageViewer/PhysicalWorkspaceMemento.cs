using System;
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	internal class PhysicalWorkspaceMemento : IMemento
	{
		private ImageBoxCollection _imageBoxes;
		private MementoList _imageBoxMementos;
		private bool _isRectangularImageBoxGrid;
		private int _rows;
		private int _columns;

		public PhysicalWorkspaceMemento(
			ImageBoxCollection imageBoxes,
			MementoList imageBoxMementos,
			bool isRectangularImageBoxGrid,
			int rows,
			int columns)
		{
			Platform.CheckForNullReference(imageBoxes, "imageBoxes");
			Platform.CheckForNullReference(imageBoxMementos, "imageBoxMementos");

			_imageBoxes = imageBoxes;
			_imageBoxMementos = imageBoxMementos;
			_isRectangularImageBoxGrid = IsRectangularImageBoxGrid;
			_rows = rows;
			_columns = columns;
		}

		public ImageBoxCollection ImageBoxes
		{
			get { return _imageBoxes; }
		}

		public MementoList ImageBoxMementos
		{
			get { return _imageBoxMementos; }
		}

		public bool IsRectangularImageBoxGrid
		{
			get { return _isRectangularImageBoxGrid; }
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
