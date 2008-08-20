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
    /// IOD for common Image Query Retrieve items.  
    /// </summary>
    public class ImageQueryIod : QueryIodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageQueryIod"/> class.
        /// </summary>
        public ImageQueryIod()
        {
            SetAttributeFromEnum(DicomAttributeCollection[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Image);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageQueryIod"/> class.
        /// </summary>
        /// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
        public ImageQueryIod(DicomAttributeCollection dicomAttributeCollection)
            :base(dicomAttributeCollection)
        {
            SetAttributeFromEnum(DicomAttributeCollection[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Image);
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
        /// Gets or sets the sop instance uid.
        /// </summary>
        /// <value>The sop instance uid.</value>
        public string SopInstanceUid
        {
            get { return DicomAttributeCollection[DicomTags.SopInstanceUid].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.SopInstanceUid].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the instance number.
        /// </summary>
        /// <value>The instance number.</value>
        public string InstanceNumber
        {
            get { return DicomAttributeCollection[DicomTags.InstanceNumber].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.InstanceNumber].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the sop class uid.
        /// </summary>
        /// <value>The sop class uid.</value>
        public string SopClassUid
        {
            get { return DicomAttributeCollection[DicomTags.SopClassUid].GetString(0, String.Empty); }
            set { DicomAttributeCollection[DicomTags.SopClassUid].SetString(0, value); }
        }

		/// <summary>
		///  Get the number of Rows
		/// </summary>
		public ushort Rows
		{
			get { return DicomAttributeCollection[DicomTags.Rows].GetUInt16(0, 0); }
		}

		/// <summary>
		/// Get the number of columns
		/// </summary>
		public ushort Columns
		{
			get { return DicomAttributeCollection[DicomTags.Columns].GetUInt16(0, 0); }
		}

		/// <summary>
		/// Get the Bits Allocated
		/// </summary>
		public ushort BitsAllocated
		{
			get { return DicomAttributeCollection[DicomTags.BitsAllocated].GetUInt16(0, 0); }
		}

		/// <summary>
		/// Get the number of frames
		/// </summary>
		public string NumberOfFrames
		{
			get { return DicomAttributeCollection[DicomTags.NumberOfFrames].GetString(0, String.Empty); }
		}

		/// <summary>
		/// Get the content label
		/// </summary>
		public string ContentLabel
		{
			get { return DicomAttributeCollection[DicomTags.ContentLabel].GetString(0, String.Empty); }
		}

		/// <summary>
		/// Get the content description
		/// </summary>
		public string ContentDescription
		{
			get { return DicomAttributeCollection[DicomTags.ContentDescription].GetString(0, String.Empty); }
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
			SetAttributeFromEnum(dicomAttributeCollection[DicomTags.QueryRetrieveLevel], QueryRetrieveLevel.Image);

			// Set image level..
			dicomAttributeCollection[DicomTags.SopInstanceUid].SetNullValue();
			dicomAttributeCollection[DicomTags.InstanceNumber].SetNullValue();
			dicomAttributeCollection[DicomTags.SopClassUid].SetNullValue();
			// IHE specified Image Query Keys
			dicomAttributeCollection[DicomTags.Rows].SetNullValue();
			dicomAttributeCollection[DicomTags.Columns].SetNullValue();
			dicomAttributeCollection[DicomTags.BitsAllocated].SetNullValue();
			dicomAttributeCollection[DicomTags.NumberOfFrames].SetNullValue();
			// IHE specified Presentation State Query Keys
			dicomAttributeCollection[DicomTags.ContentLabel].SetNullValue();
			dicomAttributeCollection[DicomTags.ContentDescription].SetNullValue();
			dicomAttributeCollection[DicomTags.PresentationCreationDate].SetNullValue();
			dicomAttributeCollection[DicomTags.PresentationCreationTime].SetNullValue();
			// IHE specified Report Query Keys
			dicomAttributeCollection[DicomTags.ReferencedRequestSequence].SetNullValue();
			dicomAttributeCollection[DicomTags.ContentDate].SetNullValue();
			dicomAttributeCollection[DicomTags.ContentTime].SetNullValue();
			dicomAttributeCollection[DicomTags.ConceptNameCodeSequence].SetNullValue();
		}

    	#endregion
    }

}
