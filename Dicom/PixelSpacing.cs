using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    public class PixelSpacing : IEquatable<PixelSpacing>
    {
		#region Private Members
		
		double _row;
		double _column;
		
		#endregion
		
		/// <summary>
		/// Constructor for NHibernate.
		/// </summary>
		private PixelSpacing()
		{
		}

		/// <summary>
		/// Mandatory Constructor.
		/// </summary>
		public PixelSpacing(double row, double column)
		{
			_row = row;
			_column = column;
		}

		#region NHibernate Persistent Properties

		public virtual double Row
        {
            get { return _row; }
            set { _row = value; }
        }

        public virtual double Column
        {
            get { return _column; }
            set { _column = value; }
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is PixelSpacing)
				return this.Equals((PixelSpacing) obj);

			return false;
		}

		#region IEquatable<PixelSpacing> Members

		public bool Equals(PixelSpacing other)
		{
			return this.Row == other.Row && this.Column == other.Column;
		}

		#endregion
	}
}
