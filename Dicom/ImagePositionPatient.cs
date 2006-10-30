using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
	public class ImagePositionPatient
	{
		#region Private Members
		
		private double _x;
		private double _y;
		private double _z;

		#endregion

		public ImagePositionPatient(double x, double y, double z)
		{
			_x = x;
			_y = y;
			_z = z;
		}

		protected ImagePositionPatient()
		{ 
		}

		public double X
		{
			get { return _x; }
			set { _x = value; }
		}
		
		public double Y
		{
			get { return _y; }
			set { _y = value; }
		}

		public double Z
		{
			get { return _z; }
			set { _z = value; }
		}
	}
}
