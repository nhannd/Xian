using System;
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.ImageServer.Dicom
{
    /// <summary>
    /// This class contains defines for all DICOM SOP Classes.
    /// </summary>
    public class SopClass
    {
        /// <summary>
        /// <para>12-lead ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.1</para>
        /// </summary>
        public static readonly String Sop12leadECGWaveformStorage = "1.2.840.10008.5.1.4.1.1.9.1.1";

        /// <summary>
        /// <para>Ambulatory ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.3</para>
        /// </summary>
        public static readonly String AmbulatoryECGWaveformStorage = "1.2.840.10008.5.1.4.1.1.9.1.3";

        /// <summary>
        /// <para>Basic Annotation Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.15</para>
        /// </summary>
        public static readonly String BasicAnnotationBoxSOPClass = "1.2.840.10008.5.1.1.15";

        /// <summary>
        /// <para>Basic Color Image Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4.1</para>
        /// </summary>
        public static readonly String BasicColorImageBoxSOPClass = "1.2.840.10008.5.1.1.4.1";

        /// <summary>
        /// <para>Basic Film Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.2</para>
        /// </summary>
        public static readonly String BasicFilmBoxSOPClass = "1.2.840.10008.5.1.1.2";

        /// <summary>
        /// <para>Basic Film Session SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.1</para>
        /// </summary>
        public static readonly String BasicFilmSessionSOPClass = "1.2.840.10008.5.1.1.1";

        /// <summary>
        /// <para>Basic Grayscale Image Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4</para>
        /// </summary>
        public static readonly String BasicGrayscaleImageBoxSOPClass = "1.2.840.10008.5.1.1.4";

        /// <summary>
        /// <para>Basic Print Image Overlay Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.24.1</para>
        /// </summary>
        public static readonly String BasicPrintImageOverlayBoxSOPClassRetired = "1.2.840.10008.5.1.1.24.1";

        /// <summary>
        /// <para>Basic Study Content Notification SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.9</para>
        /// </summary>
        public static readonly String BasicStudyContentNotificationSOPClassRetired = "1.2.840.10008.1.9";

        /// <summary>
        /// <para>Basic Text SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.11</para>
        /// </summary>
        public static readonly String BasicTextSR = "1.2.840.10008.5.1.4.1.1.88.11";

        /// <summary>
        /// <para>Basic Voice Audio Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.4.1</para>
        /// </summary>
        public static readonly String BasicVoiceAudioWaveformStorage = "1.2.840.10008.5.1.4.1.1.9.4.1";

        /// <summary>
        /// <para>Blending Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.4</para>
        /// </summary>
        public static readonly String BlendingSoftcopyPresentationStateStorageSOPClass = "1.2.840.10008.5.1.4.1.1.11.4";

        /// <summary>
        /// <para>Breast Imaging Relevant Patient Information Query</para>
        /// <para>UID: 1.2.840.10008.5.1.4.37.2</para>
        /// </summary>
        public static readonly String BreastImagingRelevantPatientInformationQuery = "1.2.840.10008.5.1.4.37.2";

        /// <summary>
        /// <para>Cardiac Electrophysiology Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.3.1</para>
        /// </summary>
        public static readonly String CardiacElectrophysiologyWaveformStorage = "1.2.840.10008.5.1.4.1.1.9.3.1";

        /// <summary>
        /// <para>Cardiac Relevant Patient Information Query</para>
        /// <para>UID: 1.2.840.10008.5.1.4.37.3</para>
        /// </summary>
        public static readonly String CardiacRelevantPatientInformationQuery = "1.2.840.10008.5.1.4.37.3";

        /// <summary>
        /// <para>Chest CAD SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.65</para>
        /// </summary>
        public static readonly String ChestCADSR = "1.2.840.10008.5.1.4.1.1.88.65";

        /// <summary>
        /// <para>Color Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.2</para>
        /// </summary>
        public static readonly String ColorSoftcopyPresentationStateStorageSOPClass = "1.2.840.10008.5.1.4.1.1.11.2";

        /// <summary>
        /// <para>Comprehensive SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.33</para>
        /// </summary>
        public static readonly String ComprehensiveSR = "1.2.840.10008.5.1.4.1.1.88.33";

        /// <summary>
        /// <para>Computed Radiography Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1</para>
        /// </summary>
        public static readonly String ComputedRadiographyImageStorage = "1.2.840.10008.5.1.4.1.1.1";

        /// <summary>
        /// <para>CT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.2</para>
        /// </summary>
        public static readonly String CTImageStorage = "1.2.840.10008.5.1.4.1.1.2";

        /// <summary>
        /// <para>Deformable Spatial Registration Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66.3</para>
        /// </summary>
        public static readonly String DeformableSpatialRegistrationStorage = "1.2.840.10008.5.1.4.1.1.66.3";

        /// <summary>
        /// <para>Detached Interpretation Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.6.1</para>
        /// </summary>
        public static readonly String DetachedInterpretationManagementSOPClassRetired = "1.2.840.10008.3.1.2.6.1";

        /// <summary>
        /// <para>Detached Patient Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.1.1</para>
        /// </summary>
        public static readonly String DetachedPatientManagementSOPClassRetired = "1.2.840.10008.3.1.2.1.1";

        /// <summary>
        /// <para>Detached Results Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.1</para>
        /// </summary>
        public static readonly String DetachedResultsManagementSOPClassRetired = "1.2.840.10008.3.1.2.5.1";

        /// <summary>
        /// <para>Detached Study Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.1</para>
        /// </summary>
        public static readonly String DetachedStudyManagementSOPClassRetired = "1.2.840.10008.3.1.2.3.1";

        /// <summary>
        /// <para>Detached Visit Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.2.1</para>
        /// </summary>
        public static readonly String DetachedVisitManagementSOPClassRetired = "1.2.840.10008.3.1.2.2.1";

        /// <summary>
        /// <para>Digital Intra-oral X-Ray Image Storage – For Presentation</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.3</para>
        /// </summary>
        public static readonly String DigitalIntraoralXRayImageStorageForPresentation = "1.2.840.10008.5.1.4.1.1.1.3";

        /// <summary>
        /// <para>Digital Intra-oral X-Ray Image Storage – For Processing</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.3.1</para>
        /// </summary>
        public static readonly String DigitalIntraoralXRayImageStorageForProcessing = "1.2.840.10008.5.1.4.1.1.1.3.1";

        /// <summary>
        /// <para>Digital Mammography X-Ray Image Storage – For Presentation</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.2</para>
        /// </summary>
        public static readonly String DigitalMammographyXRayImageStorageForPresentation = "1.2.840.10008.5.1.4.1.1.1.2";

        /// <summary>
        /// <para>Digital Mammography X-Ray Image Storage – For Processing</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.2.1</para>
        /// </summary>
        public static readonly String DigitalMammographyXRayImageStorageForProcessing = "1.2.840.10008.5.1.4.1.1.1.2.1";

        /// <summary>
        /// <para>Digital X-Ray Image Storage – For Presentation</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.1</para>
        /// </summary>
        public static readonly String DigitalXRayImageStorageForPresentation = "1.2.840.10008.5.1.4.1.1.1.1";

        /// <summary>
        /// <para>Digital X-Ray Image Storage – For Processing</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.1.1</para>
        /// </summary>
        public static readonly String DigitalXRayImageStorageForProcessing = "1.2.840.10008.5.1.4.1.1.1.1.1";

        /// <summary>
        /// <para>Encapsulated PDF Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.104.1</para>
        /// </summary>
        public static readonly String EncapsulatedPDFStorage = "1.2.840.10008.5.1.4.1.1.104.1";

        /// <summary>
        /// <para>Enhanced CT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.2.1</para>
        /// </summary>
        public static readonly String EnhancedCTImageStorage = "1.2.840.10008.5.1.4.1.1.2.1";

        /// <summary>
        /// <para>Enhanced MR Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4.1</para>
        /// </summary>
        public static readonly String EnhancedMRImageStorage = "1.2.840.10008.5.1.4.1.1.4.1";

        /// <summary>
        /// <para>Enhanced SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.22</para>
        /// </summary>
        public static readonly String EnhancedSR = "1.2.840.10008.5.1.4.1.1.88.22";

        /// <summary>
        /// <para>Enhanced XA Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.1.1</para>
        /// </summary>
        public static readonly String EnhancedXAImageStorage = "1.2.840.10008.5.1.4.1.1.12.1.1";

        /// <summary>
        /// <para>Enhanced XRF Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.2.1</para>
        /// </summary>
        public static readonly String EnhancedXRFImageStorage = "1.2.840.10008.5.1.4.1.1.12.2.1";

        /// <summary>
        /// <para>General ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.2</para>
        /// </summary>
        public static readonly String GeneralECGWaveformStorage = "1.2.840.10008.5.1.4.1.1.9.1.2";

        /// <summary>
        /// <para>General Purpose Performed Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.3</para>
        /// </summary>
        public static readonly String GeneralPurposePerformedProcedureStepSOPClass = "1.2.840.10008.5.1.4.32.3";

        /// <summary>
        /// <para>General Purpose Scheduled Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.2</para>
        /// </summary>
        public static readonly String GeneralPurposeScheduledProcedureStepSOPClass = "1.2.840.10008.5.1.4.32.2";

        /// <summary>
        /// <para>General Purpose Worklist Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.1</para>
        /// </summary>
        public static readonly String GeneralPurposeWorklistInformationModelFIND = "1.2.840.10008.5.1.4.32.1";

        /// <summary>
        /// <para>General Relevant Patient Information Query</para>
        /// <para>UID: 1.2.840.10008.5.1.4.37.1</para>
        /// </summary>
        public static readonly String GeneralRelevantPatientInformationQuery = "1.2.840.10008.5.1.4.37.1";

        /// <summary>
        /// <para>Grayscale Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.1</para>
        /// </summary>
        public static readonly String GrayscaleSoftcopyPresentationStateStorageSOPClass = "1.2.840.10008.5.1.4.1.1.11.1";

        /// <summary>
        /// <para>Hanging Protocol Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.2</para>
        /// </summary>
        public static readonly String HangingProtocolInformationModelFIND = "1.2.840.10008.5.1.4.38.2";

        /// <summary>
        /// <para>Hanging Protocol Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.3</para>
        /// </summary>
        public static readonly String HangingProtocolInformationModelMOVE = "1.2.840.10008.5.1.4.38.3";

        /// <summary>
        /// <para>Hanging Protocol Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.1</para>
        /// </summary>
        public static readonly String HangingProtocolStorage = "1.2.840.10008.5.1.4.38.1";

        /// <summary>
        /// <para>Hardcopy  Grayscale Image Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.29</para>
        /// </summary>
        public static readonly String HardcopyGrayscaleImageStorageSOPClassRetired = "1.2.840.10008.5.1.1.29";

        /// <summary>
        /// <para>Hardcopy Color Image Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.30</para>
        /// </summary>
        public static readonly String HardcopyColorImageStorageSOPClassRetired = "1.2.840.10008.5.1.1.30";

        /// <summary>
        /// <para>Hemodynamic Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.2.1</para>
        /// </summary>
        public static readonly String HemodynamicWaveformStorage = "1.2.840.10008.5.1.4.1.1.9.2.1";

        /// <summary>
        /// <para>Image Overlay Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.24</para>
        /// </summary>
        public static readonly String ImageOverlayBoxSOPClassRetired = "1.2.840.10008.5.1.1.24";

        /// <summary>
        /// <para>Instance Availability Notification SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.33</para>
        /// </summary>
        public static readonly String InstanceAvailabilityNotificationSOPClass = "1.2.840.10008.5.1.4.33";

        /// <summary>
        /// <para>Key Object Selection Document</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.59</para>
        /// </summary>
        public static readonly String KeyObjectSelectionDocument = "1.2.840.10008.5.1.4.1.1.88.59";

        /// <summary>
        /// <para>Mammography CAD SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.50</para>
        /// </summary>
        public static readonly String MammographyCADSR = "1.2.840.10008.5.1.4.1.1.88.50";

        /// <summary>
        /// <para>Media Creation Management SOP Class UID</para>
        /// <para>UID: 1.2.840.10008.5.1.1.33</para>
        /// </summary>
        public static readonly String MediaCreationManagementSOPClassUID = "1.2.840.10008.5.1.1.33";

        /// <summary>
        /// <para>Media Storage Directory Storage</para>
        /// <para>UID: 1.2.840.10008.1.3.10</para>
        /// </summary>
        public static readonly String MediaStorageDirectoryStorage = "1.2.840.10008.1.3.10";

        /// <summary>
        /// <para>Modality Performed Procedure Step Notification SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.5</para>
        /// </summary>
        public static readonly String ModalityPerformedProcedureStepNotificationSOPClass = "1.2.840.10008.3.1.2.3.5";

        /// <summary>
        /// <para>Modality Performed Procedure Step Retrieve SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.4</para>
        /// </summary>
        public static readonly String ModalityPerformedProcedureStepRetrieveSOPClass = "1.2.840.10008.3.1.2.3.4";

        /// <summary>
        /// <para>Modality Performed Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.3</para>
        /// </summary>
        public static readonly String ModalityPerformedProcedureStepSOPClass = "1.2.840.10008.3.1.2.3.3";

        /// <summary>
        /// <para>Modality Worklist Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.31</para>
        /// </summary>
        public static readonly String ModalityWorklistInformationModelFIND = "1.2.840.10008.5.1.4.31";

        /// <summary>
        /// <para>MR Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4</para>
        /// </summary>
        public static readonly String MRImageStorage = "1.2.840.10008.5.1.4.1.1.4";

        /// <summary>
        /// <para>MR Spectroscopy Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4.2</para>
        /// </summary>
        public static readonly String MRSpectroscopyStorage = "1.2.840.10008.5.1.4.1.1.4.2";

        /// <summary>
        /// <para>Multi-frame Grayscale Byte Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.2</para>
        /// </summary>
        public static readonly String MultiframeGrayscaleByteSecondaryCaptureImageStorage = "1.2.840.10008.5.1.4.1.1.7.2";

        /// <summary>
        /// <para>Multi-frame Grayscale Word Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.3</para>
        /// </summary>
        public static readonly String MultiframeGrayscaleWordSecondaryCaptureImageStorage = "1.2.840.10008.5.1.4.1.1.7.3";

        /// <summary>
        /// <para>Multi-frame Single Bit Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.1</para>
        /// </summary>
        public static readonly String MultiframeSingleBitSecondaryCaptureImageStorage = "1.2.840.10008.5.1.4.1.1.7.1";

        /// <summary>
        /// <para>Multi-frame True Color Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.4</para>
        /// </summary>
        public static readonly String MultiframeTrueColorSecondaryCaptureImageStorage = "1.2.840.10008.5.1.4.1.1.7.4";

        /// <summary>
        /// <para>Nuclear Medicine Image  Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.5</para>
        /// </summary>
        public static readonly String NuclearMedicineImageStorageRetired = "1.2.840.10008.5.1.4.1.1.5";

        /// <summary>
        /// <para>Nuclear Medicine Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.20</para>
        /// </summary>
        public static readonly String NuclearMedicineImageStorage = "1.2.840.10008.5.1.4.1.1.20";

        /// <summary>
        /// <para>Ophthalmic Photography 16 Bit Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.5.2</para>
        /// </summary>
        public static readonly String OphthalmicPhotography16BitImageStorage = "1.2.840.10008.5.1.4.1.1.77.1.5.2";

        /// <summary>
        /// <para>Ophthalmic Photography 8 Bit Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.5.1</para>
        /// </summary>
        public static readonly String OphthalmicPhotography8BitImageStorage = "1.2.840.10008.5.1.4.1.1.77.1.5.1";

        /// <summary>
        /// <para>Patient Root Query/Retrieve Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.1</para>
        /// </summary>
        public static readonly String PatientRootQueryRetrieveInformationModelFIND = "1.2.840.10008.5.1.4.1.2.1.1";

        /// <summary>
        /// <para>Patient Root Query/Retrieve Information Model – GET</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.3</para>
        /// </summary>
        public static readonly String PatientRootQueryRetrieveInformationModelGET = "1.2.840.10008.5.1.4.1.2.1.3";

        /// <summary>
        /// <para>Patient Root Query/Retrieve Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.2</para>
        /// </summary>
        public static readonly String PatientRootQueryRetrieveInformationModelMOVE = "1.2.840.10008.5.1.4.1.2.1.2";

        /// <summary>
        /// <para>Patient/Study Only Query/Retrieve Information Model  - FIND (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.1</para>
        /// </summary>
        public static readonly String PatientStudyOnlyQueryRetrieveInformationModelFINDRetired = "1.2.840.10008.5.1.4.1.2.3.1";

        /// <summary>
        /// <para>Patient/Study Only Query/Retrieve Information Model  - GET (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.3</para>
        /// </summary>
        public static readonly String PatientStudyOnlyQueryRetrieveInformationModelGETRetired = "1.2.840.10008.5.1.4.1.2.3.3";

        /// <summary>
        /// <para>Patient/Study Only Query/Retrieve Information Model  - MOVE (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.2</para>
        /// </summary>
        public static readonly String PatientStudyOnlyQueryRetrieveInformationModelMOVERetired = "1.2.840.10008.5.1.4.1.2.3.2";

        /// <summary>
        /// <para>Positron Emission Tomography Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.128</para>
        /// </summary>
        public static readonly String PositronEmissionTomographyImageStorage = "1.2.840.10008.5.1.4.1.1.128";

        /// <summary>
        /// <para>Presentation LUT SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.23</para>
        /// </summary>
        public static readonly String PresentationLUTSOPClass = "1.2.840.10008.5.1.1.23";

        /// <summary>
        /// <para>Print Job SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.14</para>
        /// </summary>
        public static readonly String PrintJobSOPClass = "1.2.840.10008.5.1.1.14";

        /// <summary>
        /// <para>Print Queue Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.26</para>
        /// </summary>
        public static readonly String PrintQueueManagementSOPClassRetired = "1.2.840.10008.5.1.1.26";

        /// <summary>
        /// <para>Printer Configuration Retrieval SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.16.376</para>
        /// </summary>
        public static readonly String PrinterConfigurationRetrievalSOPClass = "1.2.840.10008.5.1.1.16.376";

        /// <summary>
        /// <para>Printer SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.16</para>
        /// </summary>
        public static readonly String PrinterSOPClass = "1.2.840.10008.5.1.1.16";

        /// <summary>
        /// <para>Procedural Event Logging SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.40</para>
        /// </summary>
        public static readonly String ProceduralEventLoggingSOPClass = "1.2.840.10008.1.40";

        /// <summary>
        /// <para>Procedure Log Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.40</para>
        /// </summary>
        public static readonly String ProcedureLogStorage = "1.2.840.10008.5.1.4.1.1.88.40";

        /// <summary>
        /// <para>Pseudo-Color Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.3</para>
        /// </summary>
        public static readonly String PseudoColorSoftcopyPresentationStateStorageSOPClass = "1.2.840.10008.5.1.4.1.1.11.3";

        /// <summary>
        /// <para>Pull Print Request SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.31</para>
        /// </summary>
        public static readonly String PullPrintRequestSOPClassRetired = "1.2.840.10008.5.1.1.31";

        /// <summary>
        /// <para>Raw Data Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66</para>
        /// </summary>
        public static readonly String RawDataStorage = "1.2.840.10008.5.1.4.1.1.66";

        /// <summary>
        /// <para>Real World Value Mapping Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.67</para>
        /// </summary>
        public static readonly String RealWorldValueMappingStorage = "1.2.840.10008.5.1.4.1.1.67";

        /// <summary>
        /// <para>Referenced Image Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4.2</para>
        /// </summary>
        public static readonly String ReferencedImageBoxSOPClassRetired = "1.2.840.10008.5.1.1.4.2";

        /// <summary>
        /// <para>RT Beams Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.4</para>
        /// </summary>
        public static readonly String RTBeamsTreatmentRecordStorage = "1.2.840.10008.5.1.4.1.1.481.4";

        /// <summary>
        /// <para>RT Brachy Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.6</para>
        /// </summary>
        public static readonly String RTBrachyTreatmentRecordStorage = "1.2.840.10008.5.1.4.1.1.481.6";

        /// <summary>
        /// <para>RT Dose Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.2</para>
        /// </summary>
        public static readonly String RTDoseStorage = "1.2.840.10008.5.1.4.1.1.481.2";

        /// <summary>
        /// <para>RT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.1</para>
        /// </summary>
        public static readonly String RTImageStorage = "1.2.840.10008.5.1.4.1.1.481.1";

        /// <summary>
        /// <para>RT Ion Beams Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.9</para>
        /// </summary>
        public static readonly String RTIonBeamsTreatmentRecordStorage = "1.2.840.10008.5.1.4.1.1.481.9";

        /// <summary>
        /// <para>RT Ion Plan Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.8</para>
        /// </summary>
        public static readonly String RTIonPlanStorage = "1.2.840.10008.5.1.4.1.1.481.8";

        /// <summary>
        /// <para>RT Plan Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.5</para>
        /// </summary>
        public static readonly String RTPlanStorage = "1.2.840.10008.5.1.4.1.1.481.5";

        /// <summary>
        /// <para>RT Structure Set Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.3</para>
        /// </summary>
        public static readonly String RTStructureSetStorage = "1.2.840.10008.5.1.4.1.1.481.3";

        /// <summary>
        /// <para>RT Treatment Summary Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.7</para>
        /// </summary>
        public static readonly String RTTreatmentSummaryRecordStorage = "1.2.840.10008.5.1.4.1.1.481.7";

        /// <summary>
        /// <para>Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7</para>
        /// </summary>
        public static readonly String SecondaryCaptureImageStorage = "1.2.840.10008.5.1.4.1.1.7";

        /// <summary>
        /// <para>Segmentation Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66.4</para>
        /// </summary>
        public static readonly String SegmentationStorage = "1.2.840.10008.5.1.4.1.1.66.4";

        /// <summary>
        /// <para>Spatial Fiducials Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66.2</para>
        /// </summary>
        public static readonly String SpatialFiducialsStorage = "1.2.840.10008.5.1.4.1.1.66.2";

        /// <summary>
        /// <para>Spatial Registration Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66.1</para>
        /// </summary>
        public static readonly String SpatialRegistrationStorage = "1.2.840.10008.5.1.4.1.1.66.1";

        /// <summary>
        /// <para>Standalone Curve Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9</para>
        /// </summary>
        public static readonly String StandaloneCurveStorageRetired = "1.2.840.10008.5.1.4.1.1.9";

        /// <summary>
        /// <para>Standalone Modality LUT Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.10</para>
        /// </summary>
        public static readonly String StandaloneModalityLUTStorageRetired = "1.2.840.10008.5.1.4.1.1.10";

        /// <summary>
        /// <para>Standalone Overlay Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.8</para>
        /// </summary>
        public static readonly String StandaloneOverlayStorageRetired = "1.2.840.10008.5.1.4.1.1.8";

        /// <summary>
        /// <para>Standalone PET Curve Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.129</para>
        /// </summary>
        public static readonly String StandalonePETCurveStorageRetired = "1.2.840.10008.5.1.4.1.1.129";

        /// <summary>
        /// <para>Standalone VOI LUT Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11</para>
        /// </summary>
        public static readonly String StandaloneVOILUTStorageRetired = "1.2.840.10008.5.1.4.1.1.11";

        /// <summary>
        /// <para>Stereometric Relationship Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.5.3</para>
        /// </summary>
        public static readonly String StereometricRelationshipStorage = "1.2.840.10008.5.1.4.1.1.77.1.5.3";

        /// <summary>
        /// <para>Storage Commitment Pull Model SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.20.2</para>
        /// </summary>
        public static readonly String StorageCommitmentPullModelSOPClassRetired = "1.2.840.10008.1.20.2";

        /// <summary>
        /// <para>Storage Commitment Push Model SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.20.1</para>
        /// </summary>
        public static readonly String StorageCommitmentPushModelSOPClass = "1.2.840.10008.1.20.1";

        /// <summary>
        /// <para>Stored Print Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.27</para>
        /// </summary>
        public static readonly String StoredPrintStorageSOPClassRetired = "1.2.840.10008.5.1.1.27";

        /// <summary>
        /// <para>Study Component Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.2</para>
        /// </summary>
        public static readonly String StudyComponentManagementSOPClassRetired = "1.2.840.10008.3.1.2.3.2";

        /// <summary>
        /// <para>Study Root Query/Retrieve Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.1</para>
        /// </summary>
        public static readonly String StudyRootQueryRetrieveInformationModelFIND = "1.2.840.10008.5.1.4.1.2.2.1";

        /// <summary>
        /// <para>Study Root Query/Retrieve Information Model – GET</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.3</para>
        /// </summary>
        public static readonly String StudyRootQueryRetrieveInformationModelGET = "1.2.840.10008.5.1.4.1.2.2.3";

        /// <summary>
        /// <para>Study Root Query/Retrieve Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.2</para>
        /// </summary>
        public static readonly String StudyRootQueryRetrieveInformationModelMOVE = "1.2.840.10008.5.1.4.1.2.2.2";

        /// <summary>
        /// <para>Ultrasound Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.6.1</para>
        /// </summary>
        public static readonly String UltrasoundImageStorage = "1.2.840.10008.5.1.4.1.1.6.1";

        /// <summary>
        /// <para>Ultrasound Image Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.6</para>
        /// </summary>
        public static readonly String UltrasoundImageStorageRetired = "1.2.840.10008.5.1.4.1.1.6";

        /// <summary>
        /// <para>Ultrasound Multi-frame Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.3.1</para>
        /// </summary>
        public static readonly String UltrasoundMultiframeImageStorage = "1.2.840.10008.5.1.4.1.1.3.1";

        /// <summary>
        /// <para>Ultrasound Multi-frame Image Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.3</para>
        /// </summary>
        public static readonly String UltrasoundMultiframeImageStorageRetired = "1.2.840.10008.5.1.4.1.1.3";

        /// <summary>
        /// <para>Verification SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.1</para>
        /// </summary>
        public static readonly String VerificationSOPClass = "1.2.840.10008.1.1";

        /// <summary>
        /// <para>Video Endoscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.1.1</para>
        /// </summary>
        public static readonly String VideoEndoscopicImageStorage = "1.2.840.10008.5.1.4.1.1.77.1.1.1";

        /// <summary>
        /// <para>Video Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.2.1</para>
        /// </summary>
        public static readonly String VideoMicroscopicImageStorage = "1.2.840.10008.5.1.4.1.1.77.1.2.1";

        /// <summary>
        /// <para>Video Photographic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.4.1</para>
        /// </summary>
        public static readonly String VideoPhotographicImageStorage = "1.2.840.10008.5.1.4.1.1.77.1.4.1";

        /// <summary>
        /// <para>VL Endoscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.1</para>
        /// </summary>
        public static readonly String VLEndoscopicImageStorage = "1.2.840.10008.5.1.4.1.1.77.1.1";

        /// <summary>
        /// <para>VL Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.2</para>
        /// </summary>
        public static readonly String VLMicroscopicImageStorage = "1.2.840.10008.5.1.4.1.1.77.1.2";

        /// <summary>
        /// <para>VL Photographic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.4</para>
        /// </summary>
        public static readonly String VLPhotographicImageStorage = "1.2.840.10008.5.1.4.1.1.77.1.4";

        /// <summary>
        /// <para>VL Slide-Coordinates Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.3</para>
        /// </summary>
        public static readonly String VLSlideCoordinatesMicroscopicImageStorage = "1.2.840.10008.5.1.4.1.1.77.1.3";

        /// <summary>
        /// <para>VOI LUT Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.22</para>
        /// </summary>
        public static readonly String VOILUTBoxSOPClass = "1.2.840.10008.5.1.1.22";

        /// <summary>
        /// <para>X-Ray Angiographic Bi-Plane Image Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.3</para>
        /// </summary>
        public static readonly String XRayAngiographicBiPlaneImageStorageRetired = "1.2.840.10008.5.1.4.1.1.12.3";

        /// <summary>
        /// <para>X-Ray Angiographic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.1</para>
        /// </summary>
        public static readonly String XRayAngiographicImageStorage = "1.2.840.10008.5.1.4.1.1.12.1";

        /// <summary>
        /// <para>X-Ray Radiation Dose SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.67</para>
        /// </summary>
        public static readonly String XRayRadiationDoseSR = "1.2.840.10008.5.1.4.1.1.88.67";

        /// <summary>
        /// <para>X-Ray Radiofluoroscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.2</para>
        /// </summary>
        public static readonly String XRayRadiofluoroscopicImageStorage = "1.2.840.10008.5.1.4.1.1.12.2";

        /// <summary>
        /// <para>Basic Color Print Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.18</para>
        /// </summary>
        public static readonly String BasicColorPrintManagementMetaSOPClass = "1.2.840.10008.5.1.1.18";

        /// <summary>
        /// <para>Basic Grayscale Print Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.9</para>
        /// </summary>
        public static readonly String BasicGrayscalePrintManagementMetaSOPClass = "1.2.840.10008.5.1.1.9";

        /// <summary>
        /// <para>Detached Patient Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.1.4</para>
        /// </summary>
        public static readonly String DetachedPatientManagementMetaSOPClassRetired = "1.2.840.10008.3.1.2.1.4";

        /// <summary>
        /// <para>Detached Results Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.4</para>
        /// </summary>
        public static readonly String DetachedResultsManagementMetaSOPClassRetired = "1.2.840.10008.3.1.2.5.4";

        /// <summary>
        /// <para>Detached Study Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.5</para>
        /// </summary>
        public static readonly String DetachedStudyManagementMetaSOPClassRetired = "1.2.840.10008.3.1.2.5.5";

        /// <summary>
        /// <para>General Purpose Worklist Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32</para>
        /// </summary>
        public static readonly String GeneralPurposeWorklistManagementMetaSOPClass = "1.2.840.10008.5.1.4.32";

        /// <summary>
        /// <para>Pull Stored Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.32</para>
        /// </summary>
        public static readonly String PullStoredPrintManagementMetaSOPClassRetired = "1.2.840.10008.5.1.1.32";

        /// <summary>
        /// <para>Referenced Color Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.18.1</para>
        /// </summary>
        public static readonly String ReferencedColorPrintManagementMetaSOPClassRetired = "1.2.840.10008.5.1.1.18.1";

        /// <summary>
        /// <para>Referenced Grayscale Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.9.1</para>
        /// </summary>
        public static readonly String ReferencedGrayscalePrintManagementMetaSOPClassRetired = "1.2.840.10008.5.1.1.9.1";


        private String _sopName;
        private String _sopUid;
        private bool _bIsMeta;

        /// <summary> Property that represents the Name of the SOP Class. </summary>
        public String Name
        {
            get { return _sopName; }
        }
        /// <summary> Property that represents the Uid for the SOP Class. </summary>
        public String Uid
        {
            get { return _sopUid; }
        }
        /// <summary> Property that represents the Uid for the SOP Class. </summary>
        public bool Meta
        {
            get { return _bIsMeta; }
        }
        /// <summary> Constructor to create SopClass object. </summary>
        public SopClass(String name,
                           String uid,
                           bool isMeta)
        {
            _sopName = name;
            _sopUid = uid;
            _bIsMeta = isMeta;
        }

        private static Dictionary<String,SopClass> _sopList = new Dictionary<String,SopClass>();
        private static bool _bIsFirst = true;

        /// <summary>Retrieve a SopClass object associated with the Uid.</summary>
        public static SopClass GetSopClass(String uid)
        {
            if (_bIsFirst)
            {
                _bIsFirst = false;
                _sopList.Add(SopClass.Sop12leadECGWaveformStorage, 
                             new SopClass("12-lead ECG Waveform Storage", 
                                          SopClass.Sop12leadECGWaveformStorage, 
                                          false));

                _sopList.Add(SopClass.AmbulatoryECGWaveformStorage, 
                             new SopClass("Ambulatory ECG Waveform Storage", 
                                          SopClass.AmbulatoryECGWaveformStorage, 
                                          false));

                _sopList.Add(SopClass.BasicAnnotationBoxSOPClass, 
                             new SopClass("Basic Annotation Box SOP Class", 
                                          SopClass.BasicAnnotationBoxSOPClass, 
                                          false));

                _sopList.Add(SopClass.BasicColorImageBoxSOPClass, 
                             new SopClass("Basic Color Image Box SOP Class", 
                                          SopClass.BasicColorImageBoxSOPClass, 
                                          false));

                _sopList.Add(SopClass.BasicFilmBoxSOPClass, 
                             new SopClass("Basic Film Box SOP Class", 
                                          SopClass.BasicFilmBoxSOPClass, 
                                          false));

                _sopList.Add(SopClass.BasicFilmSessionSOPClass, 
                             new SopClass("Basic Film Session SOP Class", 
                                          SopClass.BasicFilmSessionSOPClass, 
                                          false));

                _sopList.Add(SopClass.BasicGrayscaleImageBoxSOPClass, 
                             new SopClass("Basic Grayscale Image Box SOP Class", 
                                          SopClass.BasicGrayscaleImageBoxSOPClass, 
                                          false));

                _sopList.Add(SopClass.BasicPrintImageOverlayBoxSOPClassRetired, 
                             new SopClass("Basic Print Image Overlay Box SOP Class (Retired)", 
                                          SopClass.BasicPrintImageOverlayBoxSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.BasicStudyContentNotificationSOPClassRetired, 
                             new SopClass("Basic Study Content Notification SOP Class (Retired)", 
                                          SopClass.BasicStudyContentNotificationSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.BasicTextSR, 
                             new SopClass("Basic Text SR", 
                                          SopClass.BasicTextSR, 
                                          false));

                _sopList.Add(SopClass.BasicVoiceAudioWaveformStorage, 
                             new SopClass("Basic Voice Audio Waveform Storage", 
                                          SopClass.BasicVoiceAudioWaveformStorage, 
                                          false));

                _sopList.Add(SopClass.BlendingSoftcopyPresentationStateStorageSOPClass, 
                             new SopClass("Blending Softcopy Presentation State Storage SOP Class", 
                                          SopClass.BlendingSoftcopyPresentationStateStorageSOPClass, 
                                          false));

                _sopList.Add(SopClass.BreastImagingRelevantPatientInformationQuery, 
                             new SopClass("Breast Imaging Relevant Patient Information Query", 
                                          SopClass.BreastImagingRelevantPatientInformationQuery, 
                                          false));

                _sopList.Add(SopClass.CardiacElectrophysiologyWaveformStorage, 
                             new SopClass("Cardiac Electrophysiology Waveform Storage", 
                                          SopClass.CardiacElectrophysiologyWaveformStorage, 
                                          false));

                _sopList.Add(SopClass.CardiacRelevantPatientInformationQuery, 
                             new SopClass("Cardiac Relevant Patient Information Query", 
                                          SopClass.CardiacRelevantPatientInformationQuery, 
                                          false));

                _sopList.Add(SopClass.ChestCADSR, 
                             new SopClass("Chest CAD SR", 
                                          SopClass.ChestCADSR, 
                                          false));

                _sopList.Add(SopClass.ColorSoftcopyPresentationStateStorageSOPClass, 
                             new SopClass("Color Softcopy Presentation State Storage SOP Class", 
                                          SopClass.ColorSoftcopyPresentationStateStorageSOPClass, 
                                          false));

                _sopList.Add(SopClass.ComprehensiveSR, 
                             new SopClass("Comprehensive SR", 
                                          SopClass.ComprehensiveSR, 
                                          false));

                _sopList.Add(SopClass.ComputedRadiographyImageStorage, 
                             new SopClass("Computed Radiography Image Storage", 
                                          SopClass.ComputedRadiographyImageStorage, 
                                          false));

                _sopList.Add(SopClass.CTImageStorage, 
                             new SopClass("CT Image Storage", 
                                          SopClass.CTImageStorage, 
                                          false));

                _sopList.Add(SopClass.DeformableSpatialRegistrationStorage, 
                             new SopClass("Deformable Spatial Registration Storage", 
                                          SopClass.DeformableSpatialRegistrationStorage, 
                                          false));

                _sopList.Add(SopClass.DetachedInterpretationManagementSOPClassRetired, 
                             new SopClass("Detached Interpretation Management SOP Class (Retired)", 
                                          SopClass.DetachedInterpretationManagementSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.DetachedPatientManagementSOPClassRetired, 
                             new SopClass("Detached Patient Management SOP Class (Retired)", 
                                          SopClass.DetachedPatientManagementSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.DetachedResultsManagementSOPClassRetired, 
                             new SopClass("Detached Results Management SOP Class (Retired)", 
                                          SopClass.DetachedResultsManagementSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.DetachedStudyManagementSOPClassRetired, 
                             new SopClass("Detached Study Management SOP Class (Retired)", 
                                          SopClass.DetachedStudyManagementSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.DetachedVisitManagementSOPClassRetired, 
                             new SopClass("Detached Visit Management SOP Class (Retired)", 
                                          SopClass.DetachedVisitManagementSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.DigitalIntraoralXRayImageStorageForPresentation, 
                             new SopClass("Digital Intra-oral X-Ray Image Storage – For Presentation", 
                                          SopClass.DigitalIntraoralXRayImageStorageForPresentation, 
                                          false));

                _sopList.Add(SopClass.DigitalIntraoralXRayImageStorageForProcessing, 
                             new SopClass("Digital Intra-oral X-Ray Image Storage – For Processing", 
                                          SopClass.DigitalIntraoralXRayImageStorageForProcessing, 
                                          false));

                _sopList.Add(SopClass.DigitalMammographyXRayImageStorageForPresentation, 
                             new SopClass("Digital Mammography X-Ray Image Storage – For Presentation", 
                                          SopClass.DigitalMammographyXRayImageStorageForPresentation, 
                                          false));

                _sopList.Add(SopClass.DigitalMammographyXRayImageStorageForProcessing, 
                             new SopClass("Digital Mammography X-Ray Image Storage – For Processing", 
                                          SopClass.DigitalMammographyXRayImageStorageForProcessing, 
                                          false));

                _sopList.Add(SopClass.DigitalXRayImageStorageForPresentation, 
                             new SopClass("Digital X-Ray Image Storage – For Presentation", 
                                          SopClass.DigitalXRayImageStorageForPresentation, 
                                          false));

                _sopList.Add(SopClass.DigitalXRayImageStorageForProcessing, 
                             new SopClass("Digital X-Ray Image Storage – For Processing", 
                                          SopClass.DigitalXRayImageStorageForProcessing, 
                                          false));

                _sopList.Add(SopClass.EncapsulatedPDFStorage, 
                             new SopClass("Encapsulated PDF Storage", 
                                          SopClass.EncapsulatedPDFStorage, 
                                          false));

                _sopList.Add(SopClass.EnhancedCTImageStorage, 
                             new SopClass("Enhanced CT Image Storage", 
                                          SopClass.EnhancedCTImageStorage, 
                                          false));

                _sopList.Add(SopClass.EnhancedMRImageStorage, 
                             new SopClass("Enhanced MR Image Storage", 
                                          SopClass.EnhancedMRImageStorage, 
                                          false));

                _sopList.Add(SopClass.EnhancedSR, 
                             new SopClass("Enhanced SR", 
                                          SopClass.EnhancedSR, 
                                          false));

                _sopList.Add(SopClass.EnhancedXAImageStorage, 
                             new SopClass("Enhanced XA Image Storage", 
                                          SopClass.EnhancedXAImageStorage, 
                                          false));

                _sopList.Add(SopClass.EnhancedXRFImageStorage, 
                             new SopClass("Enhanced XRF Image Storage", 
                                          SopClass.EnhancedXRFImageStorage, 
                                          false));

                _sopList.Add(SopClass.GeneralECGWaveformStorage, 
                             new SopClass("General ECG Waveform Storage", 
                                          SopClass.GeneralECGWaveformStorage, 
                                          false));

                _sopList.Add(SopClass.GeneralPurposePerformedProcedureStepSOPClass, 
                             new SopClass("General Purpose Performed Procedure Step SOP Class", 
                                          SopClass.GeneralPurposePerformedProcedureStepSOPClass, 
                                          false));

                _sopList.Add(SopClass.GeneralPurposeScheduledProcedureStepSOPClass, 
                             new SopClass("General Purpose Scheduled Procedure Step SOP Class", 
                                          SopClass.GeneralPurposeScheduledProcedureStepSOPClass, 
                                          false));

                _sopList.Add(SopClass.GeneralPurposeWorklistInformationModelFIND, 
                             new SopClass("General Purpose Worklist Information Model – FIND", 
                                          SopClass.GeneralPurposeWorklistInformationModelFIND, 
                                          false));

                _sopList.Add(SopClass.GeneralRelevantPatientInformationQuery, 
                             new SopClass("General Relevant Patient Information Query", 
                                          SopClass.GeneralRelevantPatientInformationQuery, 
                                          false));

                _sopList.Add(SopClass.GrayscaleSoftcopyPresentationStateStorageSOPClass, 
                             new SopClass("Grayscale Softcopy Presentation State Storage SOP Class", 
                                          SopClass.GrayscaleSoftcopyPresentationStateStorageSOPClass, 
                                          false));

                _sopList.Add(SopClass.HangingProtocolInformationModelFIND, 
                             new SopClass("Hanging Protocol Information Model – FIND", 
                                          SopClass.HangingProtocolInformationModelFIND, 
                                          false));

                _sopList.Add(SopClass.HangingProtocolInformationModelMOVE, 
                             new SopClass("Hanging Protocol Information Model – MOVE", 
                                          SopClass.HangingProtocolInformationModelMOVE, 
                                          false));

                _sopList.Add(SopClass.HangingProtocolStorage, 
                             new SopClass("Hanging Protocol Storage", 
                                          SopClass.HangingProtocolStorage, 
                                          false));

                _sopList.Add(SopClass.HardcopyGrayscaleImageStorageSOPClassRetired, 
                             new SopClass("Hardcopy  Grayscale Image Storage SOP Class (Retired)", 
                                          SopClass.HardcopyGrayscaleImageStorageSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.HardcopyColorImageStorageSOPClassRetired, 
                             new SopClass("Hardcopy Color Image Storage SOP Class (Retired)", 
                                          SopClass.HardcopyColorImageStorageSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.HemodynamicWaveformStorage, 
                             new SopClass("Hemodynamic Waveform Storage", 
                                          SopClass.HemodynamicWaveformStorage, 
                                          false));

                _sopList.Add(SopClass.ImageOverlayBoxSOPClassRetired, 
                             new SopClass("Image Overlay Box SOP Class (Retired)", 
                                          SopClass.ImageOverlayBoxSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.InstanceAvailabilityNotificationSOPClass, 
                             new SopClass("Instance Availability Notification SOP Class", 
                                          SopClass.InstanceAvailabilityNotificationSOPClass, 
                                          false));

                _sopList.Add(SopClass.KeyObjectSelectionDocument, 
                             new SopClass("Key Object Selection Document", 
                                          SopClass.KeyObjectSelectionDocument, 
                                          false));

                _sopList.Add(SopClass.MammographyCADSR, 
                             new SopClass("Mammography CAD SR", 
                                          SopClass.MammographyCADSR, 
                                          false));

                _sopList.Add(SopClass.MediaCreationManagementSOPClassUID, 
                             new SopClass("Media Creation Management SOP Class UID", 
                                          SopClass.MediaCreationManagementSOPClassUID, 
                                          false));

                _sopList.Add(SopClass.MediaStorageDirectoryStorage, 
                             new SopClass("Media Storage Directory Storage", 
                                          SopClass.MediaStorageDirectoryStorage, 
                                          false));

                _sopList.Add(SopClass.ModalityPerformedProcedureStepNotificationSOPClass, 
                             new SopClass("Modality Performed Procedure Step Notification SOP Class", 
                                          SopClass.ModalityPerformedProcedureStepNotificationSOPClass, 
                                          false));

                _sopList.Add(SopClass.ModalityPerformedProcedureStepRetrieveSOPClass, 
                             new SopClass("Modality Performed Procedure Step Retrieve SOP Class", 
                                          SopClass.ModalityPerformedProcedureStepRetrieveSOPClass, 
                                          false));

                _sopList.Add(SopClass.ModalityPerformedProcedureStepSOPClass, 
                             new SopClass("Modality Performed Procedure Step SOP Class", 
                                          SopClass.ModalityPerformedProcedureStepSOPClass, 
                                          false));

                _sopList.Add(SopClass.ModalityWorklistInformationModelFIND, 
                             new SopClass("Modality Worklist Information Model – FIND", 
                                          SopClass.ModalityWorklistInformationModelFIND, 
                                          false));

                _sopList.Add(SopClass.MRImageStorage, 
                             new SopClass("MR Image Storage", 
                                          SopClass.MRImageStorage, 
                                          false));

                _sopList.Add(SopClass.MRSpectroscopyStorage, 
                             new SopClass("MR Spectroscopy Storage", 
                                          SopClass.MRSpectroscopyStorage, 
                                          false));

                _sopList.Add(SopClass.MultiframeGrayscaleByteSecondaryCaptureImageStorage, 
                             new SopClass("Multi-frame Grayscale Byte Secondary Capture Image Storage", 
                                          SopClass.MultiframeGrayscaleByteSecondaryCaptureImageStorage, 
                                          false));

                _sopList.Add(SopClass.MultiframeGrayscaleWordSecondaryCaptureImageStorage, 
                             new SopClass("Multi-frame Grayscale Word Secondary Capture Image Storage", 
                                          SopClass.MultiframeGrayscaleWordSecondaryCaptureImageStorage, 
                                          false));

                _sopList.Add(SopClass.MultiframeSingleBitSecondaryCaptureImageStorage, 
                             new SopClass("Multi-frame Single Bit Secondary Capture Image Storage", 
                                          SopClass.MultiframeSingleBitSecondaryCaptureImageStorage, 
                                          false));

                _sopList.Add(SopClass.MultiframeTrueColorSecondaryCaptureImageStorage, 
                             new SopClass("Multi-frame True Color Secondary Capture Image Storage", 
                                          SopClass.MultiframeTrueColorSecondaryCaptureImageStorage, 
                                          false));

                _sopList.Add(SopClass.NuclearMedicineImageStorageRetired, 
                             new SopClass("Nuclear Medicine Image  Storage (Retired)", 
                                          SopClass.NuclearMedicineImageStorageRetired, 
                                          false));

                _sopList.Add(SopClass.NuclearMedicineImageStorage, 
                             new SopClass("Nuclear Medicine Image Storage", 
                                          SopClass.NuclearMedicineImageStorage, 
                                          false));

                _sopList.Add(SopClass.OphthalmicPhotography16BitImageStorage, 
                             new SopClass("Ophthalmic Photography 16 Bit Image Storage", 
                                          SopClass.OphthalmicPhotography16BitImageStorage, 
                                          false));

                _sopList.Add(SopClass.OphthalmicPhotography8BitImageStorage, 
                             new SopClass("Ophthalmic Photography 8 Bit Image Storage", 
                                          SopClass.OphthalmicPhotography8BitImageStorage, 
                                          false));

                _sopList.Add(SopClass.PatientRootQueryRetrieveInformationModelFIND, 
                             new SopClass("Patient Root Query/Retrieve Information Model – FIND", 
                                          SopClass.PatientRootQueryRetrieveInformationModelFIND, 
                                          false));

                _sopList.Add(SopClass.PatientRootQueryRetrieveInformationModelGET, 
                             new SopClass("Patient Root Query/Retrieve Information Model – GET", 
                                          SopClass.PatientRootQueryRetrieveInformationModelGET, 
                                          false));

                _sopList.Add(SopClass.PatientRootQueryRetrieveInformationModelMOVE, 
                             new SopClass("Patient Root Query/Retrieve Information Model – MOVE", 
                                          SopClass.PatientRootQueryRetrieveInformationModelMOVE, 
                                          false));

                _sopList.Add(SopClass.PatientStudyOnlyQueryRetrieveInformationModelFINDRetired, 
                             new SopClass("Patient/Study Only Query/Retrieve Information Model  - FIND (Retired)", 
                                          SopClass.PatientStudyOnlyQueryRetrieveInformationModelFINDRetired, 
                                          false));

                _sopList.Add(SopClass.PatientStudyOnlyQueryRetrieveInformationModelGETRetired, 
                             new SopClass("Patient/Study Only Query/Retrieve Information Model  - GET (Retired)", 
                                          SopClass.PatientStudyOnlyQueryRetrieveInformationModelGETRetired, 
                                          false));

                _sopList.Add(SopClass.PatientStudyOnlyQueryRetrieveInformationModelMOVERetired, 
                             new SopClass("Patient/Study Only Query/Retrieve Information Model  - MOVE (Retired)", 
                                          SopClass.PatientStudyOnlyQueryRetrieveInformationModelMOVERetired, 
                                          false));

                _sopList.Add(SopClass.PositronEmissionTomographyImageStorage, 
                             new SopClass("Positron Emission Tomography Image Storage", 
                                          SopClass.PositronEmissionTomographyImageStorage, 
                                          false));

                _sopList.Add(SopClass.PresentationLUTSOPClass, 
                             new SopClass("Presentation LUT SOP Class", 
                                          SopClass.PresentationLUTSOPClass, 
                                          false));

                _sopList.Add(SopClass.PrintJobSOPClass, 
                             new SopClass("Print Job SOP Class", 
                                          SopClass.PrintJobSOPClass, 
                                          false));

                _sopList.Add(SopClass.PrintQueueManagementSOPClassRetired, 
                             new SopClass("Print Queue Management SOP Class (Retired)", 
                                          SopClass.PrintQueueManagementSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.PrinterConfigurationRetrievalSOPClass, 
                             new SopClass("Printer Configuration Retrieval SOP Class", 
                                          SopClass.PrinterConfigurationRetrievalSOPClass, 
                                          false));

                _sopList.Add(SopClass.PrinterSOPClass, 
                             new SopClass("Printer SOP Class", 
                                          SopClass.PrinterSOPClass, 
                                          false));

                _sopList.Add(SopClass.ProceduralEventLoggingSOPClass, 
                             new SopClass("Procedural Event Logging SOP Class", 
                                          SopClass.ProceduralEventLoggingSOPClass, 
                                          false));

                _sopList.Add(SopClass.ProcedureLogStorage, 
                             new SopClass("Procedure Log Storage", 
                                          SopClass.ProcedureLogStorage, 
                                          false));

                _sopList.Add(SopClass.PseudoColorSoftcopyPresentationStateStorageSOPClass, 
                             new SopClass("Pseudo-Color Softcopy Presentation State Storage SOP Class", 
                                          SopClass.PseudoColorSoftcopyPresentationStateStorageSOPClass, 
                                          false));

                _sopList.Add(SopClass.PullPrintRequestSOPClassRetired, 
                             new SopClass("Pull Print Request SOP Class (Retired)", 
                                          SopClass.PullPrintRequestSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.RawDataStorage, 
                             new SopClass("Raw Data Storage", 
                                          SopClass.RawDataStorage, 
                                          false));

                _sopList.Add(SopClass.RealWorldValueMappingStorage, 
                             new SopClass("Real World Value Mapping Storage", 
                                          SopClass.RealWorldValueMappingStorage, 
                                          false));

                _sopList.Add(SopClass.ReferencedImageBoxSOPClassRetired, 
                             new SopClass("Referenced Image Box SOP Class (Retired)", 
                                          SopClass.ReferencedImageBoxSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.RTBeamsTreatmentRecordStorage, 
                             new SopClass("RT Beams Treatment Record Storage", 
                                          SopClass.RTBeamsTreatmentRecordStorage, 
                                          false));

                _sopList.Add(SopClass.RTBrachyTreatmentRecordStorage, 
                             new SopClass("RT Brachy Treatment Record Storage", 
                                          SopClass.RTBrachyTreatmentRecordStorage, 
                                          false));

                _sopList.Add(SopClass.RTDoseStorage, 
                             new SopClass("RT Dose Storage", 
                                          SopClass.RTDoseStorage, 
                                          false));

                _sopList.Add(SopClass.RTImageStorage, 
                             new SopClass("RT Image Storage", 
                                          SopClass.RTImageStorage, 
                                          false));

                _sopList.Add(SopClass.RTIonBeamsTreatmentRecordStorage, 
                             new SopClass("RT Ion Beams Treatment Record Storage", 
                                          SopClass.RTIonBeamsTreatmentRecordStorage, 
                                          false));

                _sopList.Add(SopClass.RTIonPlanStorage, 
                             new SopClass("RT Ion Plan Storage", 
                                          SopClass.RTIonPlanStorage, 
                                          false));

                _sopList.Add(SopClass.RTPlanStorage, 
                             new SopClass("RT Plan Storage", 
                                          SopClass.RTPlanStorage, 
                                          false));

                _sopList.Add(SopClass.RTStructureSetStorage, 
                             new SopClass("RT Structure Set Storage", 
                                          SopClass.RTStructureSetStorage, 
                                          false));

                _sopList.Add(SopClass.RTTreatmentSummaryRecordStorage, 
                             new SopClass("RT Treatment Summary Record Storage", 
                                          SopClass.RTTreatmentSummaryRecordStorage, 
                                          false));

                _sopList.Add(SopClass.SecondaryCaptureImageStorage, 
                             new SopClass("Secondary Capture Image Storage", 
                                          SopClass.SecondaryCaptureImageStorage, 
                                          false));

                _sopList.Add(SopClass.SegmentationStorage, 
                             new SopClass("Segmentation Storage", 
                                          SopClass.SegmentationStorage, 
                                          false));

                _sopList.Add(SopClass.SpatialFiducialsStorage, 
                             new SopClass("Spatial Fiducials Storage", 
                                          SopClass.SpatialFiducialsStorage, 
                                          false));

                _sopList.Add(SopClass.SpatialRegistrationStorage, 
                             new SopClass("Spatial Registration Storage", 
                                          SopClass.SpatialRegistrationStorage, 
                                          false));

                _sopList.Add(SopClass.StandaloneCurveStorageRetired, 
                             new SopClass("Standalone Curve Storage (Retired)", 
                                          SopClass.StandaloneCurveStorageRetired, 
                                          false));

                _sopList.Add(SopClass.StandaloneModalityLUTStorageRetired, 
                             new SopClass("Standalone Modality LUT Storage (Retired)", 
                                          SopClass.StandaloneModalityLUTStorageRetired, 
                                          false));

                _sopList.Add(SopClass.StandaloneOverlayStorageRetired, 
                             new SopClass("Standalone Overlay Storage (Retired)", 
                                          SopClass.StandaloneOverlayStorageRetired, 
                                          false));

                _sopList.Add(SopClass.StandalonePETCurveStorageRetired, 
                             new SopClass("Standalone PET Curve Storage (Retired)", 
                                          SopClass.StandalonePETCurveStorageRetired, 
                                          false));

                _sopList.Add(SopClass.StandaloneVOILUTStorageRetired, 
                             new SopClass("Standalone VOI LUT Storage (Retired)", 
                                          SopClass.StandaloneVOILUTStorageRetired, 
                                          false));

                _sopList.Add(SopClass.StereometricRelationshipStorage, 
                             new SopClass("Stereometric Relationship Storage", 
                                          SopClass.StereometricRelationshipStorage, 
                                          false));

                _sopList.Add(SopClass.StorageCommitmentPullModelSOPClassRetired, 
                             new SopClass("Storage Commitment Pull Model SOP Class (Retired)", 
                                          SopClass.StorageCommitmentPullModelSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.StorageCommitmentPushModelSOPClass, 
                             new SopClass("Storage Commitment Push Model SOP Class", 
                                          SopClass.StorageCommitmentPushModelSOPClass, 
                                          false));

                _sopList.Add(SopClass.StoredPrintStorageSOPClassRetired, 
                             new SopClass("Stored Print Storage SOP Class (Retired)", 
                                          SopClass.StoredPrintStorageSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.StudyComponentManagementSOPClassRetired, 
                             new SopClass("Study Component Management SOP Class (Retired)", 
                                          SopClass.StudyComponentManagementSOPClassRetired, 
                                          false));

                _sopList.Add(SopClass.StudyRootQueryRetrieveInformationModelFIND, 
                             new SopClass("Study Root Query/Retrieve Information Model – FIND", 
                                          SopClass.StudyRootQueryRetrieveInformationModelFIND, 
                                          false));

                _sopList.Add(SopClass.StudyRootQueryRetrieveInformationModelGET, 
                             new SopClass("Study Root Query/Retrieve Information Model – GET", 
                                          SopClass.StudyRootQueryRetrieveInformationModelGET, 
                                          false));

                _sopList.Add(SopClass.StudyRootQueryRetrieveInformationModelMOVE, 
                             new SopClass("Study Root Query/Retrieve Information Model – MOVE", 
                                          SopClass.StudyRootQueryRetrieveInformationModelMOVE, 
                                          false));

                _sopList.Add(SopClass.UltrasoundImageStorage, 
                             new SopClass("Ultrasound Image Storage", 
                                          SopClass.UltrasoundImageStorage, 
                                          false));

                _sopList.Add(SopClass.UltrasoundImageStorageRetired, 
                             new SopClass("Ultrasound Image Storage (Retired)", 
                                          SopClass.UltrasoundImageStorageRetired, 
                                          false));

                _sopList.Add(SopClass.UltrasoundMultiframeImageStorage, 
                             new SopClass("Ultrasound Multi-frame Image Storage", 
                                          SopClass.UltrasoundMultiframeImageStorage, 
                                          false));

                _sopList.Add(SopClass.UltrasoundMultiframeImageStorageRetired, 
                             new SopClass("Ultrasound Multi-frame Image Storage (Retired)", 
                                          SopClass.UltrasoundMultiframeImageStorageRetired, 
                                          false));

                _sopList.Add(SopClass.VerificationSOPClass, 
                             new SopClass("Verification SOP Class", 
                                          SopClass.VerificationSOPClass, 
                                          false));

                _sopList.Add(SopClass.VideoEndoscopicImageStorage, 
                             new SopClass("Video Endoscopic Image Storage", 
                                          SopClass.VideoEndoscopicImageStorage, 
                                          false));

                _sopList.Add(SopClass.VideoMicroscopicImageStorage, 
                             new SopClass("Video Microscopic Image Storage", 
                                          SopClass.VideoMicroscopicImageStorage, 
                                          false));

                _sopList.Add(SopClass.VideoPhotographicImageStorage, 
                             new SopClass("Video Photographic Image Storage", 
                                          SopClass.VideoPhotographicImageStorage, 
                                          false));

                _sopList.Add(SopClass.VLEndoscopicImageStorage, 
                             new SopClass("VL Endoscopic Image Storage", 
                                          SopClass.VLEndoscopicImageStorage, 
                                          false));

                _sopList.Add(SopClass.VLMicroscopicImageStorage, 
                             new SopClass("VL Microscopic Image Storage", 
                                          SopClass.VLMicroscopicImageStorage, 
                                          false));

                _sopList.Add(SopClass.VLPhotographicImageStorage, 
                             new SopClass("VL Photographic Image Storage", 
                                          SopClass.VLPhotographicImageStorage, 
                                          false));

                _sopList.Add(SopClass.VLSlideCoordinatesMicroscopicImageStorage, 
                             new SopClass("VL Slide-Coordinates Microscopic Image Storage", 
                                          SopClass.VLSlideCoordinatesMicroscopicImageStorage, 
                                          false));

                _sopList.Add(SopClass.VOILUTBoxSOPClass, 
                             new SopClass("VOI LUT Box SOP Class", 
                                          SopClass.VOILUTBoxSOPClass, 
                                          false));

                _sopList.Add(SopClass.XRayAngiographicBiPlaneImageStorageRetired, 
                             new SopClass("X-Ray Angiographic Bi-Plane Image Storage (Retired)", 
                                          SopClass.XRayAngiographicBiPlaneImageStorageRetired, 
                                          false));

                _sopList.Add(SopClass.XRayAngiographicImageStorage, 
                             new SopClass("X-Ray Angiographic Image Storage", 
                                          SopClass.XRayAngiographicImageStorage, 
                                          false));

                _sopList.Add(SopClass.XRayRadiationDoseSR, 
                             new SopClass("X-Ray Radiation Dose SR", 
                                          SopClass.XRayRadiationDoseSR, 
                                          false));

                _sopList.Add(SopClass.XRayRadiofluoroscopicImageStorage, 
                             new SopClass("X-Ray Radiofluoroscopic Image Storage", 
                                          SopClass.XRayRadiofluoroscopicImageStorage, 
                                          false));

                _sopList.Add(SopClass.BasicColorPrintManagementMetaSOPClass, 
                             new SopClass("Basic Color Print Management Meta SOP Class", 
                                          SopClass.BasicColorPrintManagementMetaSOPClass, 
                                          true));

                _sopList.Add(SopClass.BasicGrayscalePrintManagementMetaSOPClass, 
                             new SopClass("Basic Grayscale Print Management Meta SOP Class", 
                                          SopClass.BasicGrayscalePrintManagementMetaSOPClass, 
                                          true));

                _sopList.Add(SopClass.DetachedPatientManagementMetaSOPClassRetired, 
                             new SopClass("Detached Patient Management Meta SOP Class (Retired)", 
                                          SopClass.DetachedPatientManagementMetaSOPClassRetired, 
                                          true));

                _sopList.Add(SopClass.DetachedResultsManagementMetaSOPClassRetired, 
                             new SopClass("Detached Results Management Meta SOP Class (Retired)", 
                                          SopClass.DetachedResultsManagementMetaSOPClassRetired, 
                                          true));

                _sopList.Add(SopClass.DetachedStudyManagementMetaSOPClassRetired, 
                             new SopClass("Detached Study Management Meta SOP Class (Retired)", 
                                          SopClass.DetachedStudyManagementMetaSOPClassRetired, 
                                          true));

                _sopList.Add(SopClass.GeneralPurposeWorklistManagementMetaSOPClass, 
                             new SopClass("General Purpose Worklist Management Meta SOP Class", 
                                          SopClass.GeneralPurposeWorklistManagementMetaSOPClass, 
                                          true));

                _sopList.Add(SopClass.PullStoredPrintManagementMetaSOPClassRetired, 
                             new SopClass("Pull Stored Print Management Meta SOP Class (Retired)", 
                                          SopClass.PullStoredPrintManagementMetaSOPClassRetired, 
                                          true));

                _sopList.Add(SopClass.ReferencedColorPrintManagementMetaSOPClassRetired, 
                             new SopClass("Referenced Color Print Management Meta SOP Class (Retired)", 
                                          SopClass.ReferencedColorPrintManagementMetaSOPClassRetired, 
                                          true));

                _sopList.Add(SopClass.ReferencedGrayscalePrintManagementMetaSOPClassRetired, 
                             new SopClass("Referenced Grayscale Print Management Meta SOP Class (Retired)", 
                                          SopClass.ReferencedGrayscalePrintManagementMetaSOPClassRetired, 
                                          true));

            }

            return _sopList[uid];
        }
    }
}
