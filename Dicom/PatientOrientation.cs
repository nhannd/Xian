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
			this.Row = row;
			this.Column = column;
		}

		protected PatientOrientation()
			: this("", "")
		{ 
		}

		public string Row
		{
			get { return _row; }
			protected set { _row = value ?? ""; }
		}

		public string Column
		{
			get { return _column; }
			protected set { _column = value ?? ""; }
		}
	}
}
