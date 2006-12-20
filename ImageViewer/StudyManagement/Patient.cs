using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class Patient
	{
		private ImageSop _imageSop;
		private StudyCollection _studies;

		internal Patient()
		{
		}

		public StudyCollection Studies
		{
			get
			{
				if (_studies == null)
					_studies = new StudyCollection();

				return _studies;
			}
		}

		#region Patient Module

		public string PatientId
		{
			get { return _imageSop.PatientId; }
		}

		public string PatientsName
		{
			get { return _imageSop.PatientsName; }
		}

		public string PatientsBirthDate
		{
			get { return _imageSop.PatientsBirthDate; }
		}

		public string PatientsSex
		{
			get { return _imageSop.PatientsSex; }
		}

		#endregion

		public override string ToString()
		{
			string str = String.Format("{0} | {1}", this.PatientsName, this.PatientId);
			return str;
		}

		internal void SetSop(ImageSop imageSop)
		{
			if (_imageSop == null)
			{
				_imageSop = imageSop;
			}
		}
	}
}
