#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		public ImagingServiceRequestModule(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
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
            get { return base.DicomAttributeProvider[DicomTags.ImagingServiceRequestComments].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ImagingServiceRequestComments].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the requesting physician.
        /// </summary>
        /// <value>The requesting physician.</value>
        public PersonName RequestingPhysician
        {
            get { return new PersonName(base.DicomAttributeProvider[DicomTags.RequestingPhysician].GetString(0, String.Empty)); }
            set { base.DicomAttributeProvider[DicomTags.RequestingPhysician].SetString(0, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the name of the referring physicians.
        /// </summary>
        /// <value>The name of the referring physicians.</value>
        public PersonName ReferringPhysiciansName
        {
            get { return new PersonName(base.DicomAttributeProvider[DicomTags.ReferringPhysiciansName].GetString(0, String.Empty)); }
            set { base.DicomAttributeProvider[DicomTags.ReferringPhysiciansName].SetString(0, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the requesting service.
        /// </summary>
        /// <value>The requesting service.</value>
        public string RequestingService
        {
            get { return base.DicomAttributeProvider[DicomTags.RequestingService].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.RequestingService].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the accession number.
        /// </summary>
        /// <value>The accession number.</value>
        public string AccessionNumber
        {
            get { return base.DicomAttributeProvider[DicomTags.AccessionNumber].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.AccessionNumber].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the issue date of imaging service request.
        /// </summary>
        /// <value>The issue date of imaging service request.</value>
        public DateTime? IssueDateOfImagingServiceRequest
        {
        	get { return DateTimeParser.ParseDateAndTime(String.Empty, 
        					base.DicomAttributeProvider[DicomTags.IssueDateOfImagingServiceRequest].GetString(0, String.Empty), 
                  base.DicomAttributeProvider[DicomTags.IssueTimeOfImagingServiceRequest].GetString(0, String.Empty)); }

            set { DateTimeParser.SetDateTimeAttributeValues(value, base.DicomAttributeProvider[DicomTags.IssueDateOfImagingServiceRequest], base.DicomAttributeProvider[DicomTags.IssueTimeOfImagingServiceRequest]); }
        }

        /// <summary>
        /// Gets or sets the placer order number imaging service request.
        /// </summary>
        /// <value>The placer order number imaging service request.</value>
        public string PlacerOrderNumberImagingServiceRequest
        {
            get { return base.DicomAttributeProvider[DicomTags.PlacerOrderNumberImagingServiceRequest].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.PlacerOrderNumberImagingServiceRequest].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the filler order number imaging service request.
        /// </summary>
        /// <value>The filler order number imaging service request.</value>
        public string FillerOrderNumberImagingServiceRequest
        {
            get { return base.DicomAttributeProvider[DicomTags.FillerOrderNumberImagingServiceRequest].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.FillerOrderNumberImagingServiceRequest].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the admission id.
        /// </summary>
        /// <value>The admission id.</value>
        public string AdmissionId
        {
            get { return base.DicomAttributeProvider[DicomTags.AdmissionId].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.AdmissionId].SetString(0, value); }
        }
        
        #endregion

    }
}
