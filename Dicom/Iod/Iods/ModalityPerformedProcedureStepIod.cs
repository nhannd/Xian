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
        /// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
        public ModalityPerformedProcedureStepIod(DicomAttributeCollection dicomAttributeCollection)
            :base(dicomAttributeCollection)
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
            SetCommonTags(base.DicomAttributeCollection);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Sets the common tags for a typical request.
        /// </summary>
        /// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
        public static void SetCommonTags(DicomAttributeCollection dicomAttributeCollection)
        {
            //dicomAttributeCollection[DicomTags.PatientsName].SetString(0, "*");
            //dicomAttributeCollection[DicomTags.PatientId].SetNullValue();
            //dicomAttributeCollection[DicomTags.PatientsBirthDate].SetNullValue();
            //dicomAttributeCollection[DicomTags.PatientsBirthTime].SetNullValue();
            //dicomAttributeCollection[DicomTags.PatientsWeight].SetNullValue();

            //dicomAttributeCollection[DicomTags.RequestedProcedureId].SetNullValue();
            //dicomAttributeCollection[DicomTags.RequestedProcedureDescription].SetNullValue();
            //dicomAttributeCollection[DicomTags.StudyInstanceUid].SetNullValue();
            //dicomAttributeCollection[DicomTags.ReasonForTheRequestedProcedure].SetNullValue();
            //dicomAttributeCollection[DicomTags.RequestedProcedureComments].SetNullValue();
            //dicomAttributeCollection[DicomTags.RequestedProcedurePriority].SetNullValue();
            //dicomAttributeCollection[DicomTags.ImagingServiceRequestComments].SetNullValue();
            //dicomAttributeCollection[DicomTags.RequestingPhysician].SetNullValue();
            //dicomAttributeCollection[DicomTags.ReferringPhysiciansName].SetNullValue();
            //dicomAttributeCollection[DicomTags.RequestedProcedureLocation].SetNullValue();
            //dicomAttributeCollection[DicomTags.AccessionNumber].SetNullValue();

            //// TODO: this better and easier...
            //DicomAttributeSQ dicomAttributeSQ = dicomAttributeCollection[DicomTags.ScheduledProcedureStepSequence] as DicomAttributeSQ;
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
