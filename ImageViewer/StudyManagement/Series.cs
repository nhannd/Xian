using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Collections;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class Series
	{
		private ImageSop _imageSop;
		private Study _parentStudy;
		private SopCollection _sops;

		internal Series(Study parentStudy)
		{
			_parentStudy = parentStudy;
		}

		public Study ParentStudy
		{
			get { return _parentStudy; }
		}

		public SopCollection Sops
		{
			get 
			{
				if (_sops == null)
					_sops = new SopCollection();

				return _sops; 
			}
		}

		#region General Series Module
		
		public string Modality 
		{ 
			get { return _imageSop.Modality; } 
		}

		public string SeriesInstanceUID 
		{ 
			get { return _imageSop.SeriesInstanceUID; } 
		}

		public int SeriesNumber 
		{ 
			get { return _imageSop.SeriesNumber; } 
		}

		public string SeriesDescription 
		{ 
			get { return _imageSop.SeriesDescription; } 
		}

		public string Laterality 
		{ 
			get { return _imageSop.Laterality; } 
		}

		public string SeriesDate 
		{ 
			get { return _imageSop.SeriesDate; } 
		}

		public string SeriesTime 
		{ 
			get { return _imageSop.SeriesTime; } 
		}

		public PersonName[] PerformingPhysiciansName 
		{ 
			get { return _imageSop.PerformingPhysiciansName; } 
		}

		public PersonName[] OperatorsName 
		{ 
			get { return _imageSop.OperatorsName; } 
		}

		public string BodyPartExamined 
		{ 
			get { return _imageSop.BodyPartExamined; } 
		}

		public string PatientPosition 
		{ 
			get { return _imageSop.PatientPosition; } 
		}

		#endregion

		#region General Equipment Module

		public string Manufacturer 
		{ 
			get { return _imageSop.Manufacturer; } 
		}

		public string InstitutionName 
		{ 
			get { return _imageSop.InstitutionName; } 
		}

		public string StationName 
		{ 
			get { return _imageSop.StationName; } 
		}

		public string InstitutionalDepartmentName 
		{ 
			get { return _imageSop.InstitutionalDepartmentName; } 
		}

		public string ManufacturersModelName 
		{ 
			get { return _imageSop.ManufacturersModelName; }
		} 

		#endregion


		internal void SetSop(ImageSop imageSop)
		{
			if (_imageSop == null)
			{
				_imageSop = imageSop;

				this.ParentStudy.SetSop(imageSop);
			}
		}

		public override string ToString()
		{
			string str = String.Format("{0} | {1}", this.SeriesDescription, this.SeriesInstanceUID);
			return str;
		}
	}
}
