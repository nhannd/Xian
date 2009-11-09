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
        private string _seriesNumber;
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