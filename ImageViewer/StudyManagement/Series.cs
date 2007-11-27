#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
			_imageSop = imageSop;
			this.ParentStudy.SetSop(imageSop);
		}

		/// <summary>
		/// Returns the series description and series instance UID in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = String.Format("{0} | {1}", this.SeriesDescription, this.SeriesInstanceUID);
			return str;
		}
	}
}
