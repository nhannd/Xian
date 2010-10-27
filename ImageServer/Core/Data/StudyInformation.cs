#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

			foreach(Series series in study.Series.Values)
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