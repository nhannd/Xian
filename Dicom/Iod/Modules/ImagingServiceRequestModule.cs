#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// As per Dicom Doc 3, Table C.4-12 (pg 248)
    /// </summary>
    public class ImagingServiceRequestModule : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImagingServiceRequestModule"/> class.
        /// </summary>
        public ImagingServiceRequestModule()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagingServiceRequestModule"/> class.
        /// </summary>
        /// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
        public ImagingServiceRequestModule(DicomAttributeCollection dicomAttributeCollection)
            : base(dicomAttributeCollection)
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the imaging service request comments.
        /// </summary>
        /// <value>The imaging service request comments.</value>
        public string ImagingServiceRequestComments
        {
            get { return base.DicomAttributeCollection[DicomTags.ImagingServiceRequestComments].GetString(0, String.Empty); }
            set { base.DicomAttributeCollection[DicomTags.ImagingServiceRequestComments].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the requesting physician.
        /// </summary>
        /// <value>The requesting physician.</value>
        public PersonName RequestingPhysician
        {
            get { return new PersonName(base.DicomAttributeCollection[DicomTags.RequestingPhysician].GetString(0, String.Empty)); }
            set { base.DicomAttributeCollection[DicomTags.RequestingPhysician].SetString(0, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the name of the referring physicians.
        /// </summary>
        /// <value>The name of the referring physicians.</value>
        public PersonName ReferringPhysiciansName
        {
            get { return new PersonName(base.DicomAttributeCollection[DicomTags.ReferringPhysiciansName].GetString(0, String.Empty)); }
            set { base.DicomAttributeCollection[DicomTags.ReferringPhysiciansName].SetString(0, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the requesting service.
        /// </summary>
        /// <value>The requesting service.</value>
        public string RequestingService
        {
            get { return base.DicomAttributeCollection[DicomTags.RequestingService].GetString(0, String.Empty); }
            set { base.DicomAttributeCollection[DicomTags.RequestingService].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the accession number.
        /// </summary>
        /// <value>The accession number.</value>
        public string AccessionNumber
        {
            get { return base.DicomAttributeCollection[DicomTags.AccessionNumber].GetString(0, String.Empty); }
            set { base.DicomAttributeCollection[DicomTags.AccessionNumber].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the issue date of imaging service request.
        /// </summary>
        /// <value>The issue date of imaging service request.</value>
        public DateTime? IssueDateOfImagingServiceRequest
        {
        	get { return DateTimeParser.ParseDateAndTime(String.Empty, 
        					base.DicomAttributeCollection[DicomTags.IssueDateOfImagingServiceRequest].GetString(0, String.Empty), 
                  base.DicomAttributeCollection[DicomTags.IssueTimeOfImagingServiceRequest].GetString(0, String.Empty)); }

            set { DateTimeParser.SetDateTimeAttributeValues(value, base.DicomAttributeCollection[DicomTags.IssueDateOfImagingServiceRequest], base.DicomAttributeCollection[DicomTags.IssueTimeOfImagingServiceRequest]); }
        }

        /// <summary>
        /// Gets or sets the placer order number imaging service request.
        /// </summary>
        /// <value>The placer order number imaging service request.</value>
        public string PlacerOrderNumberImagingServiceRequest
        {
            get { return base.DicomAttributeCollection[DicomTags.PlacerOrderNumberImagingServiceRequest].GetString(0, String.Empty); }
            set { base.DicomAttributeCollection[DicomTags.PlacerOrderNumberImagingServiceRequest].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the filler order number imaging service request.
        /// </summary>
        /// <value>The filler order number imaging service request.</value>
        public string FillerOrderNumberImagingServiceRequest
        {
            get { return base.DicomAttributeCollection[DicomTags.FillerOrderNumberImagingServiceRequest].GetString(0, String.Empty); }
            set { base.DicomAttributeCollection[DicomTags.FillerOrderNumberImagingServiceRequest].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the admission id.
        /// </summary>
        /// <value>The admission id.</value>
        public string AdmissionId
        {
            get { return base.DicomAttributeCollection[DicomTags.AdmissionId].GetString(0, String.Empty); }
            set { base.DicomAttributeCollection[DicomTags.AdmissionId].SetString(0, value); }
        }
        
        #endregion

    }
}
