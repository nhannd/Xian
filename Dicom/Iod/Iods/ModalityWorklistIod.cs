#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Iods
{
    /// <summary>
    /// Modality worklist IOD
    /// </summary>
    /// <remarks>As per Dicom Doc 4, K6.1.2 (pg 193-194)</remarks>
    public class ModalityWorklistIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ModalityWorklistIod"/> class.
        /// </summary>
        public ModalityWorklistIod()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModalityWorklistIod"/> class.
        /// </summary>
        public ModalityWorklistIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }

		#endregion

        #region Public Properties

        /// <summary>
        /// Gets the patient module.
        /// </summary>
        /// <value>The patient module.</value>
        public PatientIdentificationModuleIod PatientIdentificationModule
        {
            get { return GetModuleIod<PatientIdentificationModuleIod>(); }
        }

        /// <summary>
        /// Gets the requested procedure module.
        /// </summary>
        /// <value>The requested procedure module.</value>
        public RequestedProcedureModuleIod RequestedProcedureModule
        {
            get { return GetModuleIod<RequestedProcedureModuleIod>(); }
        }

        /// <summary>
        /// Gets the scheduled procedure step module.
        /// </summary>
        /// <value>The scheduled procedure step module.</value>
        public ScheduledProcedureStepModuleIod ScheduledProcedureStepModule
        {
            get { return GetModuleIod<ScheduledProcedureStepModuleIod>(); }
        }

        /// <summary>
        /// Gets the imaging service request module.
        /// </summary>
        /// <value>The imaging service request module.</value>
        public ImagingServiceRequestModule ImagingServiceRequestModule
        {
            get { return GetModuleIod<ImagingServiceRequestModule>(); }
        }

        /// <summary>
        /// Gets the patient medical module module.
        /// </summary>
        /// <value>The patient medical module.</value>
        public PatientMedicalModule PatientMedicalModule
        {
            get { return GetModuleIod<PatientMedicalModule>(); }
        }
       #endregion

        #region Public Methods
        /// <summary>
        /// Sets the common tags for a typical Modality Worklist Request.
        /// </summary>
        public void SetCommonTags()
        {
            SetCommonTags(DicomAttributeProvider);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Sets the common tags for a typical Modality Worklist Request.
        /// </summary>
        public static void SetCommonTags(IDicomAttributeProvider dicomAttributeProvider)
        {
            ModalityWorklistIod iod = new ModalityWorklistIod(dicomAttributeProvider);
            //iod.PatientIdentificationModule.PatientsName.FirstName = "*";
            iod.DicomAttributeProvider[DicomTags.PatientsName].SetStringValue("*");
            iod.SetAttributeNull(DicomTags.PatientId);
            iod.SetAttributeNull(DicomTags.PatientsBirthDate);
            iod.SetAttributeNull(DicomTags.PatientsBirthTime);
            iod.SetAttributeNull(DicomTags.PatientsWeight);
            iod.SetAttributeNull(DicomTags.RequestedProcedureId);
            iod.SetAttributeNull(DicomTags.RequestedProcedureDescription);
            iod.SetAttributeNull(DicomTags.StudyInstanceUid);
            iod.SetAttributeNull(DicomTags.ReasonForTheRequestedProcedure);
            iod.SetAttributeNull(DicomTags.RequestedProcedureComments);
            iod.SetAttributeNull(DicomTags.RequestedProcedurePriority);
            iod.SetAttributeNull(DicomTags.ImagingServiceRequestComments);
            iod.SetAttributeNull(DicomTags.RequestingPhysician);
            iod.SetAttributeNull(DicomTags.ReferringPhysiciansName);
            iod.SetAttributeNull(DicomTags.RequestedProcedureLocation);
            iod.SetAttributeNull(DicomTags.AccessionNumber);
            iod.SetAttributeNull(DicomTags.PatientsSex);

            ScheduledProcedureStepSequenceIod scheduledProcedureStepSequenceIod = new ScheduledProcedureStepSequenceIod();
            scheduledProcedureStepSequenceIod.SetCommonTags();
            iod.ScheduledProcedureStepModule.ScheduledProcedureStepSequenceList.Add(scheduledProcedureStepSequenceIod);

            //// TODO: this better and easier...
            //DicomAttributeSQ dicomAttributeSQ = dicomAttributeProvider[DicomTags.ScheduledProcedureStepSequence] as DicomAttributeSQ;
            //DicomSequenceItem dicomSequenceItem = new DicomSequenceItem();
            //dicomAttributeSQ.Values = dicomSequenceItem;

            //dicomSequenceItem[DicomTags.Modality].SetNullValue();
            //dicomSequenceItem[DicomTags.ScheduledProcedureStepId].SetNullValue();
            //dicomSequenceItem[DicomTags.ScheduledProcedureStepDescription].SetNullValue();
            //dicomSequenceItem[DicomTags.ScheduledStationAeTitle].SetNullValue();
            //dicomSequenceItem[DicomTags.ScheduledProcedureStepStartDate].SetNullValue();
            //dicomSequenceItem[DicomTags.ScheduledProcedureStepStartTime].SetNullValue();
            //dicomSequenceItem[DicomTags.ScheduledPerformingPhysiciansName].SetNullValue();
            //dicomSequenceItem[DicomTags.ScheduledProcedureStepLocation].SetNullValue();
            //dicomSequenceItem[DicomTags.ScheduledProcedureStepStatus].SetNullValue();
            //dicomSequenceItem[DicomTags.CommentsOnTheScheduledProcedureStep].SetNullValue();

        }
        #endregion
    }
}
