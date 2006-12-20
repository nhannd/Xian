using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class Study
	{
		private ImageSop _imageSop;
		private Patient _parentPatient;
		private SeriesCollection _series;

		internal Study(Patient parentPatient)
		{
			_parentPatient = parentPatient;
		}

		public Patient ParentPatient
		{
			get { return _parentPatient; }
		}

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

		public string StudyInstanceUID
		{
			get { return _imageSop.StudyInstanceUID; }
		}

		public string StudyDate 
		{
			get { return _imageSop.StudyDate; }
		}

		public string StudyTime 
		{
			get { return _imageSop.StudyTime; }
		}

		public PersonName ReferringPhysiciansName 
		{
			get { return _imageSop.ReferringPhysiciansName; } 
		}

		public string AccessionNumber 
		{
			get { return _imageSop.AccessionNumber; } 
		}

		public string StudyDescription 
		{
			get { return _imageSop.StudyDescription; } 
		}

		public PersonName[] NameOfPhysiciansReadingStudy 
		{
			get { return _imageSop.NameOfPhysiciansReadingStudy; }
		}

		#endregion

		#region Patient Study Module
		
		public string[] AdmittingDiagnosesDescription 
		{
			get { return _imageSop.AdmittingDiagnosesDescription; }
		}
		
		public string PatientsAge 
		{
			get { return _imageSop.PatientsAge; }
		}
		
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
