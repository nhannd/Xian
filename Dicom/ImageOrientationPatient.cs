using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
	public class ImageOrientationPatient
	{
		#region Private Members

		private double _rowX;
		private double _rowY;
		private double _rowZ;
		private double _columnX;
		private double _columnY;
		private double _columnZ;

		#endregion

		public ImageOrientationPatient(double rowX, double rowY, double rowZ, double columnX, double columnY, double columnZ)
		{
			_rowX = rowX;
			_rowY = rowY;
			_rowZ = rowZ;
			_columnX = columnX;
			_columnY = columnY;
			_columnZ = columnZ;
		}

		protected ImageOrientationPatient()
		{ 
		}

		public double RowX
		{
			get { return _rowX; }
			set { _rowX = value; }
		}

		public double RowY
		{
			get { return _rowY; }
			set { _rowY = value; }
		}

		public double RowZ
		{
			get { return _rowZ; }
			set { _rowZ = value; }
		}

		public double ColumnX
		{
			get { return _columnX; }
			set { _columnX = value; }
		}

		public double ColumnY
		{
			get { return _columnY; }
			set { _columnY = value; }
		}

		public double ColumnZ
		{
			get { return _columnZ; }
			set { _columnZ = value; }
		}
	}
}
