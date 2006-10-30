using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    public class PixelSpacing
    {
		#region Private Members
		
		double _row;
		double _column;
		
		#endregion
		
		public PixelSpacing(double row, double column)
        {
            this.Row = row;
            this.Column = column;
        }

		private PixelSpacing()
		{
		}
		
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
    }
}
