#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A DICOM series.
	/// </summary>
	public class Series : ISeriesData
	{
		private Sop _sop;
		private readonly Study _parentStudy;
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

		#region ISeriesData Members

		/// <summary>
		/// Gets the Study Instance UID of the identified series.
		/// </summary>
		public string StudyInstanceUid
		{
			get { return _sop.StudyInstanceUid; }
		}

		/// <summary>
		/// Gets the Series Instance UID of the identified series.
		/// </summary>
		public string SeriesInstanceUid
		{
			get { return _sop.SeriesInstanceUid; }
		}

		/// <summary>
		/// Gets the modality of the identified series.
		/// </summary>
		public string Modality
		{
			get { return _sop.Modality; }
		}

		/// <summary>
		/// Gets the series description of the identified series.
		/// </summary>
		public string SeriesDescription
		{
			get { return _sop.SeriesDescription; }
		}

		/// <summary>
		/// Gets the series number of the identified series.
		/// </summary>
		public int SeriesNumber
		{
			get { return _sop.SeriesNumber; }
		}

		/// <summary>
		/// Gets the number of composite object instances belonging to the identified series.
		/// </summary>
		public int NumberOfSeriesRelatedInstances
		{
			get { return Sops.Count; }
		}

		int? ISeriesData.NumberOfSeriesRelatedInstances
		{
			get { return NumberOfSeriesRelatedInstances; }	
		}

		#endregion

		/// <summary>
		/// Gets the laterality.
		/// </summary>
		public string Laterality 
		{ 
			get { return _sop.Laterality; } 
		}

		/// <summary>
		/// Gets the series date.
		/// </summary>
		public string SeriesDate 
		{ 
			get { return _sop.SeriesDate; } 
		}

		/// <summary>
		/// Gets the series time.
		/// </summary>
		public string SeriesTime 
		{ 
			get { return _sop.SeriesTime; } 
		}

		/// <summary>
		/// Gets the names of performing physicians.
		/// </summary>
		public PersonName[] PerformingPhysiciansName 
		{ 
			get { return _sop.PerformingPhysiciansName; } 
		}

		/// <summary>
		/// Gets the names of operators.
		/// </summary>
		public PersonName[] OperatorsName 
		{ 
			get { return _sop.OperatorsName; } 
		}

		/// <summary>
		/// Gets the body part examined.
		/// </summary>
		public string BodyPartExamined 
		{ 
			get { return _sop.BodyPartExamined; } 
		}

		/// <summary>
		/// Gets the patient position.
		/// </summary>
		public string PatientPosition 
		{ 
			get { return _sop.PatientPosition; } 
		}

		/// <summary>
		/// Gets the manufacturer.
		/// </summary>
		public string Manufacturer 
		{ 
			get { return _sop.Manufacturer; } 
		}

		/// <summary>
		/// Gets the institution name.
		/// </summary>
		public string InstitutionName 
		{ 
			get { return _sop.InstitutionName; } 
		}

		/// <summary>
		/// Gets the station name.
		/// </summary>
		public string StationName 
		{ 
			get { return _sop.StationName; } 
		}

		/// <summary>
		/// Gets the institutional department name.
		/// </summary>
		public string InstitutionalDepartmentName 
		{ 
			get { return _sop.InstitutionalDepartmentName; } 
		}

		/// <summary>
		/// Gets the manufacturer's model name.
		/// </summary>
		public string ManufacturersModelName 
		{ 
			get { return _sop.ManufacturersModelName; }
		}

		/// <summary>
		/// Gets an <see cref="ISeriesIdentifier"/> for this <see cref="Series"/>.
		/// </summary>
		/// <remarks>An <see cref="ISeriesIdentifier"/> can be used in situations where you only
		/// need some data about the <see cref="Series"/>, but not the <see cref="Series"/> itself.  It can be problematic
		/// to hold references to <see cref="Series"/> objects outside the context of an <see cref="IImageViewer"/>
		/// because they are no longer valid when the viewer is closed; in these situations, it may be appropriate to
		/// use an identifier.
		/// </remarks>
		public ISeriesIdentifier GetIdentifier()
		{
			StudyItem studyIdentifier = new StudyItem(StudyInstanceUid, _sop.DataSource.Server, _sop.DataSource.StudyLoaderName);
			studyIdentifier.InstanceAvailability = "ONLINE";
			return new SeriesIdentifier(this, studyIdentifier);
		}

		internal void SetSop(Sop sop)
		{
			_sop = sop;
			this.ParentStudy.SetSop(sop);
		}

		/// <summary>
		/// Returns the series description and series instance UID in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = String.Format("{0} | {1} | {2}", this.SeriesNumber, this.SeriesDescription, this.SeriesInstanceUid);
			return str;
		}
	}
}
