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
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Iods
{
    /// <summary>
    /// IOD for common Series Query Retrieve items.
    /// </summary>
    public class SeriesQueryIod : QueryIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesQueryIod"/> class.
        /// </summary>
        public SeriesQueryIod()
        {
            SetAttributeFromEnum(DicomAttributeCollection[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Series);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesQueryIod"/> class.
        /// </summary>
        /// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
        public SeriesQueryIod(DicomAttributeCollection dicomAttributeCollection)
            :base(dicomAttributeCollection)
        {
            SetAttributeFromEnum(DicomAttributeCollection[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Series);
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
        /// Gets or sets the series instance uid.
        /// </summary>
        /// <value>The series instance uid.</value>
        public string SeriesInstanceUid
        {
            get { return DicomAttributeCollection[DicomTags.SeriesInstanceUid].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.SeriesInstanceUid].SetString(0, value); }
        }

		/// <summary>
		/// Gets or sets the modality.
		/// </summary>
		/// <value>The modality.</value>
		public string Modality
		{
			get { return DicomAttributeCollection[DicomTags.Modality].GetString(0, String.Empty); }
			set { DicomAttributeCollection[DicomTags.Modality].SetString(0, value); }
		}

		/// <summary>
        /// Gets or sets the series description.
        /// </summary>
        /// <value>The series description.</value>
        public string SeriesDescription
        {
            get { return DicomAttributeCollection[DicomTags.SeriesDescription].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.SeriesDescription].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the series number.
        /// </summary>
        /// <value>The series number.</value>
        public string SeriesNumber
        {
            get { return DicomAttributeCollection[DicomTags.SeriesNumber].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.SeriesNumber].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the number of series related instances.
        /// </summary>
        /// <value>The number of series related instances.</value>
        public uint NumberOfSeriesRelatedInstances
        {
            get { return DicomAttributeCollection[DicomTags.NumberOfSeriesRelatedInstances].GetUInt32(0, 0); }
            set { DicomAttributeCollection[DicomTags.NumberOfSeriesRelatedInstances].SetUInt32(0, value); }
        }

        /// <summary>
        /// Gets or sets the series date.
        /// </summary>
        /// <value>The series date.</value>
        public DateTime? SeriesDate
        {
            get { return DateTimeParser.ParseDateAndTime(String.Empty, 
                    DicomAttributeCollection[DicomTags.SeriesDate].GetString(0, String.Empty), 
                    DicomAttributeCollection[DicomTags.SeriesTime].GetString(0, String.Empty)); }

            set { DateTimeParser.SetDateTimeAttributeValues(value, DicomAttributeCollection[DicomTags.SeriesDate], DicomAttributeCollection[DicomTags.SeriesTime]); }
        }

        /// <summary>
        /// Gets or sets the performed procedure step start date.
        /// </summary>
        /// <value>The performed procedure step start date.</value>
        public DateTime? PerformedProcedureStepStartDate
        {
            get { return DateTimeParser.ParseDateAndTime(String.Empty, 
                    DicomAttributeCollection[DicomTags.PerformedProcedureStepStartDate].GetString(0, String.Empty), 
                    DicomAttributeCollection[DicomTags.PerformedProcedureStepStartTime].GetString(0, String.Empty)); }

            set { DateTimeParser.SetDateTimeAttributeValues(value, DicomAttributeCollection[DicomTags.PerformedProcedureStepStartDate], DicomAttributeCollection[DicomTags.PerformedProcedureStepStartTime]); }
        }

		/// <summary>
		/// Gets the request attributes sequence list.
		/// </summary>
		/// <value>The request attributes sequence list.</value>
		public SequenceIodList<RequestAttributesSequenceIod> RequestAttributesSequence
		{
			get
			{
				return new SequenceIodList<RequestAttributesSequenceIod>(DicomAttributeCollection[DicomTags.RequestAttributesSequence] as DicomAttributeSQ);
			}
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
			SetAttributeFromEnum(dicomAttributeCollection[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Series);

			dicomAttributeCollection[DicomTags.SeriesInstanceUid].SetNullValue();
			dicomAttributeCollection[DicomTags.Modality].SetNullValue();
			dicomAttributeCollection[DicomTags.SeriesDescription].SetNullValue();
			dicomAttributeCollection[DicomTags.NumberOfSeriesRelatedInstances].SetNullValue();
			dicomAttributeCollection[DicomTags.SeriesNumber].SetNullValue();
			dicomAttributeCollection[DicomTags.SeriesDate].SetNullValue();
			dicomAttributeCollection[DicomTags.SeriesTime].SetNullValue();
			dicomAttributeCollection[DicomTags.RequestAttributesSequence].SetNullValue();
			dicomAttributeCollection[DicomTags.PerformedProcedureStepStartDate].SetNullValue();
			dicomAttributeCollection[DicomTags.PerformedProcedureStepStartTime].SetNullValue();
        }
        #endregion
    }

}
