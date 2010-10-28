#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// Patient Identification Module, as per Part 3, C.4.13
    /// </summary>
    public class PerformedProcedureStepRelationshipModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformedProcedureStepRelationshipModuleIod"/> class.
        /// </summary>
        public PerformedProcedureStepRelationshipModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformedProcedureStepRelationshipModuleIod"/> class.
        /// </summary>
		public PerformedProcedureStepRelationshipModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the patients.
        /// </summary>
        /// <value>The name of the patients.</value>
        public PersonName PatientsName
        {
            get { return new PersonName(base.DicomAttributeProvider[DicomTags.PatientsName].GetString(0, String.Empty)); }
            set { base.DicomAttributeProvider[DicomTags.PatientsName].SetString(0, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the patient id.
        /// </summary>
        /// <value>The patient id.</value>
        public string PatientId
        {
            get { return base.DicomAttributeProvider[DicomTags.PatientId].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.PatientId].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the issuer of patient id.
        /// </summary>
        /// <value>The issuer of patient id.</value>
        public string IssuerOfPatientId
        {
            get { return base.DicomAttributeProvider[DicomTags.IssuerOfPatientId].GetString(0, String.Empty); }
            set { base.DicomAttributeProvider[DicomTags.IssuerOfPatientId].SetString(0, value); }
        }

        /// <summary>
        /// Gets or sets the patients birth date (only, no time).
        /// </summary>
        /// <value>The patients birth date.</value>
        public DateTime? PatientsBirthDate
        {
        	get { return DateTimeParser.ParseDateAndTime(base.DicomAttributeProvider, 0, DicomTags.PatientsBirthDate, 0);  }
        
            set { DateTimeParser.SetDateTimeAttributeValues(value, base.DicomAttributeProvider, 0, DicomTags.PatientsBirthDate, 0); }
        }

        /// <summary>
        /// Gets or sets the patients sex.
        /// </summary>
        /// <value>The patients sex.</value>
        public PatientsSex PatientsSex
        {
            get { return IodBase.ParseEnum<PatientsSex>(base.DicomAttributeProvider[DicomTags.PatientsSex].GetString(0, String.Empty), PatientsSex.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.PatientsSex], value); }
        }

        /// <summary>
        /// Gets the referenced patient sequence list.
        /// </summary>
        /// <value>The referenced patient sequence list.</value>
        public SequenceIodList<ReferencedInstanceSequenceIod> ReferencedPatientSequenceList
        {
            get
            {
                return new SequenceIodList<ReferencedInstanceSequenceIod>(base.DicomAttributeProvider[DicomTags.ReferencedPatientSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Gets the scheduled step attributes sequence list.
        /// </summary>
        /// <value>The scheduled step attributes sequence list.</value>
        public SequenceIodList<ScheduledStepAttributesSequenceIod> ScheduledStepAttributesSequenceList
        {
            get
            {
                return new SequenceIodList<ScheduledStepAttributesSequenceIod>(base.DicomAttributeProvider[DicomTags.ScheduledStepAttributesSequence] as DicomAttributeSQ);
            }
        }

        #endregion

    }

    
}
