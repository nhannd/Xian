using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM study.
	/// </summary>
	public class Patient
	{
		private ImageSop _imageSop;
		private StudyCollection _studies;

		internal Patient()
		{
		}

		/// <summary>
		/// Gets the collection of <see cref="Study"/> objects that belong
		/// to this <see cref="Patient"/>.
		/// </summary>
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

		/// <summary>
		/// Gets the patient ID.
		/// </summary>
		public string PatientId
		{
			get { return _imageSop.PatientId; }
		}

		/// <summary>
		/// Gets the patient's name.
		/// </summary>
		public PersonName PatientsName
		{
			get { return _imageSop.PatientsName; }
		}

		/// <summary>
		/// Gets the patient's birthdate.
		/// </summary>
		public string PatientsBirthDate
		{
			get { return _imageSop.PatientsBirthDate; }
		}

		/// <summary>
		/// Gets the patient's sex.
		/// </summary>
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
