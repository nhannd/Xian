using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
	public class PixelAspectRatio : IEquatable<PixelAspectRatio>
	{
		#region Private Members
		
		double _row;
		double _column;
		
		#endregion

		/// <summary>
		/// Constructor for NHibernate.
		/// </summary>
		private PixelAspectRatio()
		{
		}

		/// <summary>
		/// Mandatory Constructor.
		/// </summary>
		public PixelAspectRatio(double row, double column)
		{
			_row = row;
			_column = column;
		}

		#region NHibernate Persistent Properties

		public virtual double Row
        {
            get { return _row; }
            protected set { _row = value; }
        }

        public virtual double Column
        {
            get { return _column; }
			protected set { _column = value; }
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is PixelAspectRatio)
				return this.Equals((PixelAspectRatio) obj);

			return false;
		}

		#region IEquatable<PixelAspectRatio> Members

		public bool Equals(PixelAspectRatio other)
		{
			return Row == other.Row && Column == other.Column;
		}

		#endregion
	}
}
