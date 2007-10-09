using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM series.
	/// </summary>
	public class Series
	{
		private ImageSop _imageSop;
		private Study _parentStudy;
		private SopCollection _sops;

		internal Series(Study parentStudy)
		{
			_parentStudy = parentStudy;
		}

		/// <summary>
		/// Gets the parent <see cref="Study"/>.
		/// </summary>
		public Study ParentStudy
		{
			get { return _parentStudy; }
		}

		/// <summary>
		/// Gets the collection of <see cref="Sop"/> objects that belong
		/// to this <see cref="Study"/>.
		/// </summary>
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

		/// <summary>
		/// Gets the modality.
		/// </summary>
		public string Modality 
		{ 
			get { return _imageSop.Modality; } 
		}

		/// <summary>
		/// Gets the Series Instance UID.
		/// </summary>
		public string SeriesInstanceUID 
		{ 
			get { return _imageSop.SeriesInstanceUID; } 
		}

		/// <summary>
		/// Gets the series number.
		/// </summary>
		public int SeriesNumber 
		{ 
			get { return _imageSop.SeriesNumber; } 
		}

		/// <summary>
		/// Gets the series description.
		/// </summary>
		public string SeriesDescription 
		{ 
			get { return _imageSop.SeriesDescription; } 
		}

		/// <summary>
		/// Gets the laterality.
		/// </summary>
		public string Laterality 
		{ 
			get { return _imageSop.Laterality; } 
		}

		/// <summary>
		/// Gets the series date.
		/// </summary>
		public string SeriesDate 
		{ 
			get { return _imageSop.SeriesDate; } 
		}

		/// <summary>
		/// Gets the series time.
		/// </summary>
		public string SeriesTime 
		{ 
			get { return _imageSop.SeriesTime; } 
		}

		/// <summary>
		/// Gets the names of performing physicians.
		/// </summary>
		public PersonName[] PerformingPhysiciansName 
		{ 
			get { return _imageSop.PerformingPhysiciansName; } 
		}

		/// <summary>
		/// Gets the names of operators.
		/// </summary>
		public PersonName[] OperatorsName 
		{ 
			get { return _imageSop.OperatorsName; } 
		}

		/// <summary>
		/// Gets the body part examined.
		/// </summary>
		public string BodyPartExamined 
		{ 
			get { return _imageSop.BodyPartExamined; } 
		}

		/// <summary>
		/// Gets the patient position.
		/// </summary>
		public string PatientPosition 
		{ 
			get { return _imageSop.PatientPosition; } 
		}

		#endregion

		#region General Equipment Module

		/// <summary>
		/// Gets the manufacturer.
		/// </summary>
		public string Manufacturer 
		{ 
			get { return _imageSop.Manufacturer; } 
		}

		/// <summary>
		/// Gets the institution name.
		/// </summary>
		public string InstitutionName 
		{ 
			get { return _imageSop.InstitutionName; } 
		}

		/// <summary>
		/// Gets the station name.
		/// </summary>
		public string StationName 
		{ 
			get { return _imageSop.StationName; } 
		}

		/// <summary>
		/// Gets the institutional department name.
		/// </summary>
		public string InstitutionalDepartmentName 
		{ 
			get { return _imageSop.InstitutionalDepartmentName; } 
		}

		/// <summary>
		/// Gets the manufacturer's model name.
		/// </summary>
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
