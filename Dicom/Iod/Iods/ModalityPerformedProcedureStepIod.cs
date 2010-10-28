#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod.Iods
{
    /// <summary>
    /// Modality Performed Procedure Step Iod
    /// </summary>
    /// <remarks>As per Dicom Doc 3, B.17.2-1 (pg 237)</remarks>
    public class ModalityPerformedProcedureStepIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ModalityPerformedProcedureStepIod"/> class.
        /// </summary>
        public ModalityPerformedProcedureStepIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModalityPerformedProcedureStepIod"/> class.
        /// </summary>
		public ModalityPerformedProcedureStepIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Contains SOP common information.
        /// </summary>
        /// <value>The sop common.</value>
        public SopCommonModuleIod SopCommon
        {
            get { return base.GetModuleIod<SopCommonModuleIod>(); }
        }

        /// <summary>
        /// References the related SOPs and IEs.
        /// </summary>
        /// <value>The performed procedure step relationship.</value>
        public PerformedProcedureStepRelationshipModuleIod PerformedProcedureStepRelationship
        {
            get { return base.GetModuleIod<PerformedProcedureStepRelationshipModuleIod>(); }
        }

        /// <summary>
        /// Includes identifying and status information as well as place and time
        /// </summary>
        /// <value>The performed procedure step information.</value>
        public PerformedProcedureStepInformationModuleIod PerformedProcedureStepInformation
        {
            get { return base.GetModuleIod<PerformedProcedureStepInformationModuleIod>(); }
        }

        /// <summary>
        /// Identifies Series and Images related to this PPS and specific image acquisition conditions.
        /// </summary>
        /// <value>The image acquisition results.</value>
        public ImageAcquisitionResultsModuleIod ImageAcquisitionResults
        {
            get { return base.GetModuleIod<ImageAcquisitionResultsModuleIod>(); }
        }

        /// <summary>
        /// Contains radiation dose information related to this Performed Procedure Step.
        /// </summary>
        /// <value>The radiation dose.</value>
        public RadiationDoseModuleIod RadiationDose
        {
            get { return base.GetModuleIod<RadiationDoseModuleIod>(); }
        }

        /// <summary>
        /// Contains codes for billing and material management.
        /// </summary>
        /// <value>The billing and material management codes.</value>
        public BillingAndMaterialManagementCodesModuleIod BillingAndMaterialManagementCodes
        {
            get { return base.GetModuleIod<BillingAndMaterialManagementCodesModuleIod>(); }
        }
        
       #endregion

        #region Public Methods
        /// <summary>
        /// Sets the common tags for a typical request.
        /// </summary>
        public void SetCommonTags()
        {
            SetCommonTags(base.DicomAttributeProvider);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Sets the common tags for a typical request.
        /// </summary>
        public static void SetCommonTags(IDicomAttributeProvider dicomAttributeProvider)
        {
            //dicomAttributeProvider[DicomTags.PatientsName].SetString(0, "*");
            //dicomAttributeProvider[DicomTags.PatientId].SetNullValue();
            //dicomAttributeProvider[DicomTags.PatientsBirthDate].SetNullValue();
            //dicomAttributeProvider[DicomTags.PatientsBirthTime].SetNullValue();
            //dicomAttributeProvider[DicomTags.PatientsWeight].SetNullValue();

            //dicomAttributeProvider[DicomTags.RequestedProcedureId].SetNullValue();
            //dicomAttributeProvider[DicomTags.RequestedProcedureDescription].SetNullValue();
            //dicomAttributeProvider[DicomTags.StudyInstanceUid].SetNullValue();
            //dicomAttributeProvider[DicomTags.ReasonForTheRequestedProcedure].SetNullValue();
            //dicomAttributeProvider[DicomTags.RequestedProcedureComments].SetNullValue();
            //dicomAttributeProvider[DicomTags.RequestedProcedurePriority].SetNullValue();
            //dicomAttributeProvider[DicomTags.ImagingServiceRequestComments].SetNullValue();
            //dicomAttributeProvider[DicomTags.RequestingPhysician].SetNullValue();
            //dicomAttributeProvider[DicomTags.ReferringPhysiciansName].SetNullValue();
            //dicomAttributeProvider[DicomTags.RequestedProcedureLocation].SetNullValue();
            //dicomAttributeProvider[DicomTags.AccessionNumber].SetNullValue();

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
