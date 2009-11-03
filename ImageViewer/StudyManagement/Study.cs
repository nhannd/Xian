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
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using System.Collections.ObjectModel;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	//TODO (cr Oct 2009): make sure old properties not deleted.
	/// <summary>
	/// A DICOM study.
	/// </summary>
	public class Study : IStudyData
	{
		private Sop _sop;
		private readonly Patient _parentPatient;
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
		/// Gets the collection of <see cref="StudyManagement.Series"/> objects that belong
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

		#region IStudyData Members

		public string StudyInstanceUid
		{
			get { return _sop.StudyInstanceUid; }
		}

		public string[] ModalitiesInStudy
		{
			get
			{
				List<string> modalities = new List<string>();
				foreach(Series series in this.Series)
				{
					if (!modalities.Contains(series.Modality))
						modalities.Add(series.Modality);
				}
				return modalities.ToArray();
			}	
		}

		public string StudyDescription
		{
			get { return _sop.StudyDescription; }
		}

		public string StudyId
		{
			get { return _sop.StudyId; }
		}

		public string StudyDate
		{
			get { return _sop.StudyDate; }
		}

		public string StudyTime
		{
			get { return _sop.StudyTime; }
		}

		public string AccessionNumber
		{
			get { return _sop.AccessionNumber; }
		}

		public PersonName ReferringPhysiciansName
		{
			get { return _sop.ReferringPhysiciansName; }
		}

		string IStudyData.ReferringPhysiciansName
		{
			get { return _sop.ReferringPhysiciansName; }
		}

		public int NumberOfStudyRelatedSeries
		{
			get { return Series.Count; }
		}

		int? IStudyData.NumberOfStudyRelatedSeries
		{
			get { return NumberOfStudyRelatedSeries; }
		}

		public int NumberOfStudyRelatedInstances
		{
			get
			{
				int count = 0;
				foreach (Series series in Series)
					count += series.NumberOfSeriesRelatedInstances;
				return count;
			}
		}

		int? IStudyData.NumberOfStudyRelatedInstances
		{
			get { return NumberOfStudyRelatedInstances; }	
		}

		#endregion

		public IStudyRootStudyIdentifier GetIdentifier()
		{
			StudyItem identifier = new StudyItem(_parentPatient, this, _sop.DataSource.Server, _sop.DataSource.StudyLoaderName);
			identifier.InstanceAvailability = "ONLINE";
			return identifier;
		}

		/// <summary>
		/// Returns the study description and study instance UID associated with
		/// the <see cref="Study"/> in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = String.Format("{0} | {1}", this.StudyDescription, this.StudyInstanceUid);
			return str;
		}

		internal void SetSop(Sop sop)
		{
			_sop = sop;
			this.ParentPatient.SetSop(sop);
		}
	}
}
