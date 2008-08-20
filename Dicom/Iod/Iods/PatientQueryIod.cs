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

namespace ClearCanvas.Dicom.Iod.Iods
{
    /// <summary>
    /// IOD for common Patient Query Retrieve items.
    /// </summary>
    public class PatientQueryIod : QueryIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PatientQueryIod"/> class.
        /// </summary>
        public PatientQueryIod()
        {
            SetAttributeFromEnum(DicomAttributeCollection[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Patient);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientQueryIod"/> class.
        /// </summary>
        /// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
        public PatientQueryIod(DicomAttributeCollection dicomAttributeCollection)
            :base(dicomAttributeCollection)
        {
            SetAttributeFromEnum(DicomAttributeCollection[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Patient);
        }
        #endregion

        #region Public Properties
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
		/// Gets or sets the number of patient related instances.
		/// </summary>
		/// <value>The number of patient related instances.</value>
		public uint NumberOfPatientRelatedInstances
		{
			get { return DicomAttributeCollection[DicomTags.NumberOfPatientRelatedInstances].GetUInt32(0, 0); }
			set { DicomAttributeCollection[DicomTags.NumberOfPatientRelatedInstances].SetUInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the number of patient related series.
		/// </summary>
		/// <value>The number of patient related series.</value>
		public uint NumberOfPatientRelatedSeries
		{
			get { return DicomAttributeCollection[DicomTags.NumberOfPatientRelatedSeries].GetUInt32(0, 0); }
			set { DicomAttributeCollection[DicomTags.NumberOfPatientRelatedSeries].SetUInt32(0, value); }
		}

		/// <summary>
		/// Gets or sets the number of patient related studies.
		/// </summary>
		/// <value>The number of patient related studies.</value>
		public uint NumberOfPatientRelatedStudies
		{
			get { return DicomAttributeCollection[DicomTags.NumberOfPatientRelatedStudies].GetUInt32(0, 0); }
			set { DicomAttributeCollection[DicomTags.NumberOfPatientRelatedStudies].SetUInt32(0, value); }
		}

        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the common tags for a patient query retrieve request.
        /// </summary>
         public void SetCommonTags()
        {
            SetCommonTags(DicomAttributeCollection);
        }

        /// <summary>
        /// Sets the common tags for a patient query retrieve request.
        /// </summary>
        /// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
        public static void SetCommonTags(DicomAttributeCollection dicomAttributeCollection)
        {
			SetAttributeFromEnum(dicomAttributeCollection[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Patient);

			// Always set the Patient 
			dicomAttributeCollection[DicomTags.PatientsName].SetString(0, "*");
			dicomAttributeCollection[DicomTags.PatientId].SetNullValue();
			dicomAttributeCollection[DicomTags.PatientsBirthDate].SetNullValue();
			dicomAttributeCollection[DicomTags.PatientsBirthTime].SetNullValue();
			dicomAttributeCollection[DicomTags.PatientsSex].SetNullValue();
			dicomAttributeCollection[DicomTags.NumberOfPatientRelatedStudies].SetNullValue();
			dicomAttributeCollection[DicomTags.NumberOfPatientRelatedSeries].SetNullValue();
			dicomAttributeCollection[DicomTags.NumberOfPatientRelatedInstances].SetNullValue();
		}
        #endregion
    }

}
