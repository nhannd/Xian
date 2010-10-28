#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml.Serialization;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Core.Data
{
	/// <summary>
	/// Represents serializable series information.
	/// </summary>
	[XmlRoot("Series")]
	public class SeriesInformation
	{
		#region Private Members

		private int _numberOfInstances;
       // private string _seriesNumber;
		#endregion

		#region Constructors

		public SeriesInformation()
		{
		}

		public SeriesInformation(IDicomAttributeProvider attributeProvider)
		{
			if (attributeProvider[DicomTags.SeriesInstanceUid] != null)
				SeriesInstanceUid = attributeProvider[DicomTags.SeriesInstanceUid].ToString();
			if (attributeProvider[DicomTags.SeriesDescription] != null)
				SeriesDescription = attributeProvider[DicomTags.SeriesDescription].ToString();
			if (attributeProvider[DicomTags.Modality] != null)
				Modality = attributeProvider[DicomTags.Modality].ToString();
            if (attributeProvider[DicomTags.SeriesNumber] != null)
                SeriesNumber = attributeProvider[DicomTags.SeriesNumber].ToString();

            if (attributeProvider[DicomTags.NumberOfSeriesRelatedInstances] != null)
                Int32.TryParse(attributeProvider[DicomTags.NumberOfSeriesRelatedInstances].ToString(), out _numberOfInstances);
		}

		#endregion

		#region Public Properties

		[XmlAttribute]
		public string SeriesInstanceUid { get; set; }

		[XmlAttribute]
		public string Modality { get; set; }

		public string SeriesDescription { get; set; }

        [XmlAttribute]
        public string SeriesNumber { get; set; }

		public int NumberOfInstances
		{
			get { return _numberOfInstances; }
			set { _numberOfInstances = value; }
		}

	    #endregion
	}
}