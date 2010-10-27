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
    /// As per Dicom Doc 3, Table C.4-11 (pg 248)
    /// </summary>
    public class RequestedProcedureModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PatientModule class.
        /// </summary>
        public RequestedProcedureModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Iod class.
        /// </summary>
        /// <param name="_dicomAttributeCollection"></param>
		public RequestedProcedureModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties
        public string RequestedProcedureId
        {
            get { return base.DicomAttributeProvider[DicomTags.RequestedProcedureId].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.RequestedProcedureId].SetString(0, value); }
        }
        public string ReasonForTheRequestedProcedure
        {
            get { return base.DicomAttributeProvider[DicomTags.ReasonForTheRequestedProcedure].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.ReasonForTheRequestedProcedure].SetString(0, value); }
        }

        public string RequestedProcedureComments
        {
            get { return base.DicomAttributeProvider[DicomTags.RequestedProcedureComments].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.RequestedProcedureComments].SetString(0, value); }
        }

        public string StudyInstanceUid
        {
            get { return base.DicomAttributeProvider[DicomTags.StudyInstanceUid].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.StudyInstanceUid].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the study date.
        /// </summary>
        /// <value>The study date.</value>
        public DateTime? StudyDate
        {
            get
            {
                return DateTimeParser.ParseDateAndTime(String.Empty,
                  base.DicomAttributeProvider[DicomTags.StudyDate].GetString(0, String.Empty),
                  base.DicomAttributeProvider[DicomTags.StudyTime].GetString(0, String.Empty));
            }

            set { DateTimeParser.SetDateTimeAttributeValues(value, base.DicomAttributeProvider[DicomTags.StudyDate], base.DicomAttributeProvider[DicomTags.StudyTime]); }
        }

        /// <summary>
        /// Gets or sets the requested procedure description.
        /// </summary>
        /// <value>The requested procedure description.</value>
        public string RequestedProcedureDescription
        {
            get { return base.DicomAttributeProvider[DicomTags.RequestedProcedureDescription].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.RequestedProcedureDescription].SetString(0, value); }
        }

        // TODO: make one with the RequestedProcedurePriority enum
        public RequestedProcedurePriority RequestedProcedurePriority
        {
            get { return IodBase.ParseEnum<RequestedProcedurePriority>(base.DicomAttributeProvider[DicomTags.RequestedProcedurePriority].GetString(0, String.Empty), RequestedProcedurePriority.None); }
            set 
            {
                string stringValue = value == RequestedProcedurePriority.None ? String.Empty : value.ToString().ToUpperInvariant();
                base.DicomAttributeProvider[DicomTags.RequestedProcedurePriority].SetString(0, stringValue); 
            }
        }
        
        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public enum RequestedProcedurePriority
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Stat,
        /// <summary>
        /// 
        /// </summary>
        High,
        /// <summary>
        /// 
        /// </summary>
        Routine,
        /// <summary>
        /// 
        /// </summary>
        Medium,
        /// <summary>
        /// 
        /// </summary>
        Low
    }
}
