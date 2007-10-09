using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM study.
	/// </summary>
	public class Study
	{
		private ImageSop _imageSop;
		private Patient _parentPatient;
		private SeriesCollection _series;

		internal Study(Patient parentPatient)
		{
			_parentPatient = parentPatient;
		}

		/// <summary>
		/// Gets the parent <see cref="Patient"/>.
		/// </summary>
		public Patient ParentPatient
		{
			get { return _parentPatient; }
		}

		/// <summary>
		/// Gets the collection of <see cref="Series"/> objects that belong
		/// to this <see cref="Study"/>.
		/// </summary>
		public SeriesCollection Series
		{
			get 
			{
				if (_series == null)
					_series = new SeriesCollection();

				return _series; 
			}
		}

		#region General Study Module

		/// <summary>
		/// Gets the Study Instance UID.
		/// </summary>
		public string StudyInstanceUID
		{
			get { return _imageSop.StudyInstanceUID; }
		}

		/// <summary>
		/// Gets the study date.
		/// </summary>
		public string StudyDate 
		{
			get { return _imageSop.StudyDate; }
		}

		/// <summary>
		/// Gets the study time.
		/// </summary>
		public string StudyTime 
		{
			get { return _imageSop.StudyTime; }
		}

		/// <summary>
		/// Gets the referring physician's name.
		/// </summary>
		public PersonName ReferringPhysiciansName 
		{
			get { return _imageSop.ReferringPhysiciansName; } 
		}

		/// <summary>
		/// Gets the accession number.
		/// </summary>
		public string AccessionNumber 
		{
			get { return _imageSop.AccessionNumber; } 
		}

		/// <summary>
		/// Gets the study description.
		/// </summary>
		public string StudyDescription 
		{
			get { return _imageSop.StudyDescription; } 
		}

		/// <summary>
		/// Gets the names of physicians reading the study.
		/// </summary>
		public PersonName[] NameOfPhysiciansReadingStudy 
		{
			get { return _imageSop.NameOfPhysiciansReadingStudy; }
		}

		#endregion

		#region Patient Study Module

		/// <summary>
		/// Gets the admitting diagnoses descriptions.
		/// </summary>
		public string[] AdmittingDiagnosesDescription 
		{
			get { return _imageSop.AdmittingDiagnosesDescription; }
		}

		/// <summary>
		/// Gets the patient's age.
		/// </summary>
		public string PatientsAge 
		{
			get { return _imageSop.PatientsAge; }
		}

		/// <summary>
		/// Gets the additional patient's history.
		/// </summary>
		public string AdditionalPatientsHistory 
		{
			get { return _imageSop.AdditionalPatientsHistory; }
		}

		#endregion

		public override string ToString()
		{
			string str = String.Format("{0} | {1}", this.StudyDescription, this.StudyInstanceUID);
			return str;
		}

		internal void SetSop(ImageSop imageSop)
		{
			if (_imageSop == null)
			{
				_imageSop = imageSop;
				this.ParentPatient.SetSop(imageSop);
			}
		}
	}
}
