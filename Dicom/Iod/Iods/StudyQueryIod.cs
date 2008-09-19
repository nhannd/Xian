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

namespace ClearCanvas.Dicom.Iod.Iods
{
    /// <summary>
    /// IOD for common Query Retrieve items.  This is a replacement for the <see cref="ClearCanvas.Dicom.QueryResult"/>
    /// </summary>
    public class StudyQueryIod : QueryIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StudyQueryIod"/> class.
        /// </summary>
        public StudyQueryIod()
            :base()
        {
            SetAttributeFromEnum(DicomAttributeCollection[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Study);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StudyQueryIod"/> class.
        /// </summary>
        /// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
        public StudyQueryIod(DicomAttributeCollection dicomAttributeCollection)
            :base(dicomAttributeCollection)
        {
            SetAttributeFromEnum(DicomAttributeCollection[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Study);
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the study instance uid.
        /// </summary>
        /// <value>The study instance uid.</value>
        public string StudyInstanceUid
        {
            get { return DicomAttributeCollection[DicomTags.StudyInstanceUid].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.StudyInstanceUid].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the patient id.
        /// </summary>
        /// <value>The patient id.</value>
        public string PatientId
        {
            get { return DicomAttributeCollection[DicomTags.PatientId].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.PatientId].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the name of the patient.
        /// </summary>
        /// <value>The name of the patients.</value>
        public PersonName PatientsName
        {
            get { return new PersonName(DicomAttributeCollection[DicomTags.PatientsName].GetString(0, String.Empty)); }
            set { DicomAttributeCollection[DicomTags.PatientsName].SetString(0, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the patients birth date.
        /// </summary>
        /// <value>The patients birth date.</value>
        public DateTime PatientsBirthDate
        {
            get { return DicomAttributeCollection[DicomTags.PatientsBirthDate].GetDateTime(0, DateTime.MinValue); }
            set { DicomAttributeCollection[DicomTags.PatientsBirthDate].SetDateTime(0, value); }
        }

        /// <summary>
        /// Gets or sets the patients sex.
        /// </summary>
        /// <value>The patients sex.</value>
        public string PatientsSex
        {
            get { return DicomAttributeCollection[DicomTags.PatientsSex].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.PatientsSex].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the modalities in study.
        /// </summary>
        /// <value>The modalities in study.</value>
        public string ModalitiesInStudy
        {
            get { return DicomAttributeCollection[DicomTags.ModalitiesInStudy].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.ModalitiesInStudy].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the study description.
        /// </summary>
        /// <value>The study description.</value>
        public string StudyDescription
        {
            get { return DicomAttributeCollection[DicomTags.StudyDescription].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.StudyDescription].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the study id.
        /// </summary>
        /// <value>The study id.</value>
        public string StudyId
        {
            get { return DicomAttributeCollection[DicomTags.StudyId].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.StudyId].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the study date.
        /// </summary>
        /// <value>The study date.</value>
        public DateTime? StudyDate
        {
            get { return DateTimeParser.ParseDateAndTime(String.Empty, 
                    DicomAttributeCollection[DicomTags.StudyDate].GetString(0, String.Empty), 
                    DicomAttributeCollection[DicomTags.StudyTime].GetString(0, String.Empty)); }

            set { DateTimeParser.SetDateTimeAttributeValues(value, DicomAttributeCollection[DicomTags.StudyDate], DicomAttributeCollection[DicomTags.StudyTime]); }
        }

        /// <summary>
        /// Gets or sets the accession number.
        /// </summary>
        /// <value>The accession number.</value>
        public string AccessionNumber
        {
            get { return DicomAttributeCollection[DicomTags.AccessionNumber].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.AccessionNumber].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the number of study related instances.
        /// </summary>
        /// <value>The number of study related instances.</value>
        public uint NumberOfStudyRelatedInstances
        {
            get { return DicomAttributeCollection[DicomTags.NumberOfStudyRelatedInstances].GetUInt32(0, 0); }
            set { DicomAttributeCollection[DicomTags.NumberOfStudyRelatedInstances].SetUInt32(0, value); }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the common tags for a query retrieve request.
        /// </summary>
        public void SetCommonTags()
        {
            SetCommonTags(DicomAttributeCollection);
        }

        public static void SetCommonTags(DicomAttributeCollection dicomAttributeCollection)
        {
        	PatientQueryIod.SetCommonTags(dicomAttributeCollection);

			SetAttributeFromEnum(dicomAttributeCollection[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Study);

			dicomAttributeCollection[DicomTags.StudyInstanceUid].SetNullValue();
			dicomAttributeCollection[DicomTags.StudyId].SetNullValue();
			dicomAttributeCollection[DicomTags.StudyDate].SetNullValue();
			dicomAttributeCollection[DicomTags.StudyTime].SetNullValue();
			dicomAttributeCollection[DicomTags.StudyDescription].SetNullValue();
			dicomAttributeCollection[DicomTags.AccessionNumber].SetNullValue();
			dicomAttributeCollection[DicomTags.NumberOfStudyRelatedInstances].SetNullValue();
			dicomAttributeCollection[DicomTags.NumberOfStudyRelatedSeries].SetNullValue();
			dicomAttributeCollection[DicomTags.ModalitiesInStudy].SetNullValue();
			dicomAttributeCollection[DicomTags.RequestingPhysician].SetNullValue();
			dicomAttributeCollection[DicomTags.ReferringPhysiciansName].SetNullValue();
        }
        #endregion
    }

}
