using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
	public class PatientOrientation
	{
		#region Private Members

		private string _row;
		private string _column;

		#endregion

		public PatientOrientation(string row, string column)
		{
			_row = row;
			_column = column;
		}

		protected PatientOrientation()
		{ 
		}

		string Row
		{
			get { return _row; }
			set { _row = value; }
		}

		string Column
		{
			get { return _column; }
			set { _column = value; }
		}
	}
}
