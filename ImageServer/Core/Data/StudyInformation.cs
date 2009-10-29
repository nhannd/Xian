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

using System.Collections.Generic;
using System.Xml.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Data
{
	/// <summary>
	/// Represents serializable study information.
	/// </summary>
	[XmlRoot("StudyInformation")]
	public class StudyInformation 
	{
		#region Private Fields

		private PatientInformation _patientInfo;
		private List<SeriesInformation> _series;

		#endregion

		#region Constructors
		public StudyInformation()
		{
		}

		public StudyInformation(IDicomAttributeProvider attributeProvider)
		{
			if (attributeProvider[DicomTags.StudyId]!=null)
				StudyId = attributeProvider[DicomTags.StudyId].ToString();
            
			if (attributeProvider[DicomTags.AccessionNumber]!=null)
				AccessionNumber = attributeProvider[DicomTags.AccessionNumber].ToString();

			if (attributeProvider[DicomTags.StudyDate] != null )
				StudyDate = attributeProvider[DicomTags.StudyDate].ToString();

			if (attributeProvider[DicomTags.ModalitiesInStudy] != null)
				Modalities = attributeProvider[DicomTags.ModalitiesInStudy].ToString();

			if (attributeProvider[DicomTags.StudyInstanceUid] != null)
				StudyInstanceUid = attributeProvider[DicomTags.StudyInstanceUid].ToString();

			if (attributeProvider[DicomTags.StudyDescription] != null)
				StudyDescription = attributeProvider[DicomTags.StudyDescription].ToString();


			if (attributeProvider[DicomTags.ReferringPhysiciansName] != null)
				ReferringPhysician = attributeProvider[DicomTags.ReferringPhysiciansName].ToString();

			PatientInfo = new PatientInformation(attributeProvider);

		    DicomAttribute seriesUidAttr;
		    if (attributeProvider.TryGetAttribute(DicomTags.SeriesInstanceUid, out seriesUidAttr))
            {
                SeriesInformation series = new SeriesInformation(attributeProvider);
                Add(series);
            }
			
		}

		#endregion

		#region Public Properties

		public string StudyId { get; set; }

		public string AccessionNumber { get; set; }

		public string StudyDate { get; set; }

		public string StudyTime { get; set; }

		public string Modalities { get; set; }

		public string StudyInstanceUid { get; set; }

		public string StudyDescription { get; set; }


		public string ReferringPhysician { get; set; }

		public PatientInformation PatientInfo
		{
			get { return _patientInfo; }
			set { _patientInfo = value; }
		}

		[XmlArray("Series")]
		public List<SeriesInformation> Series
		{
			get { return _series;}
			set { _series = value; }
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Adds a <see cref="SeriesInformation"/> data
		/// </summary>
		/// <param name="message"></param>
		public void Add(DicomMessageBase message)
		{
            if (PatientInfo==null)
            {
                PatientInfo = new PatientInformation(message.DataSet);
            }

            if (Series == null)
                Series = new List<SeriesInformation>();

			string seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].ToString();
			SeriesInformation theSeries = Series.Find(ser => ser.SeriesInstanceUid == seriesInstanceUid);
			if (theSeries==null)
			{
				SeriesInformation newSeries = new SeriesInformation(message.DataSet) {NumberOfInstances = 1};
				Series.Add(newSeries);
			}
			else
			{
				theSeries.NumberOfInstances++;
			}
		}

		public void Add(SeriesInformation series)
		{
            if (Series == null)
                Series = new List<SeriesInformation>();

			Series.Add(series);
		}

		#endregion

		#region Public Static Methods
		public static StudyInformation CreateFrom(Study study)
		{
			ServerEntityAttributeProvider studyWrapper = new ServerEntityAttributeProvider(study);
			StudyInformation studyInfo = new StudyInformation(studyWrapper);

			foreach(Series series in study.Series)
			{
				ServerEntityAttributeProvider seriesWrapper = new ServerEntityAttributeProvider(series);
				SeriesInformation seriesInfo = new SeriesInformation(seriesWrapper);
				studyInfo.Add(seriesInfo);
			}

			return studyInfo;
		}
		#endregion
	}
}