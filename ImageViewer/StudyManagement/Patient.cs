using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.StudyManagement
{
	public class Patient
	{
		private string _patientID;

		private StudyCollection _studies = new StudyCollection();

		internal Patient(string patientID)
		{
			_patientID = patientID;
		}

		public string PatientId
		{
			get { return _patientID; }
		}

		public StudyCollection Studies
		{
			get { return _studies; }
		}

		public override string ToString()
		{
			return this.PatientId;
		}
	}
}
