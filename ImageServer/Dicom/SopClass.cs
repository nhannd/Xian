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
        public static readonly String Sop12leadECGWaveformStorageUid = "1.2.840.10008.5.1.4.1.1.9.1.1";

        /// <summary>SopClass for
        /// <para>12-lead ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.1</para>
        /// </summary>
        public static readonly SopClass Sop12leadECGWaveformStorage =
                             new SopClass("12-lead ECG Waveform Storage", 
                                          SopClass.Sop12leadECGWaveformStorageUid, 
                                          false);

        /// <summary>
        /// <para>Ambulatory ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.3</para>
        /// </summary>
        public static readonly String AmbulatoryECGWaveformStorageUid = "1.2.840.10008.5.1.4.1.1.9.1.3";

        /// <summary>SopClass for
        /// <para>Ambulatory ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.3</para>
        /// </summary>
        public static readonly SopClass AmbulatoryECGWaveformStorage =
                             new SopClass("Ambulatory ECG Waveform Storage", 
                                          SopClass.AmbulatoryECGWaveformStorageUid, 
                                          false);

        /// <summary>
        /// <para>Basic Annotation Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.15</para>
        /// </summary>
        public static readonly String BasicAnnotationBoxSOPClassUid = "1.2.840.10008.5.1.1.15";

        /// <summary>SopClass for
        /// <para>Basic Annotation Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.15</para>
        /// </summary>
        public static readonly SopClass BasicAnnotationBoxSOPClass =
                             new SopClass("Basic Annotation Box SOP Class", 
                                          SopClass.BasicAnnotationBoxSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Basic Color Image Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4.1</para>
        /// </summary>
        public static readonly String BasicColorImageBoxSOPClassUid = "1.2.840.10008.5.1.1.4.1";

        /// <summary>SopClass for
        /// <para>Basic Color Image Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4.1</para>
        /// </summary>
        public static readonly SopClass BasicColorImageBoxSOPClass =
                             new SopClass("Basic Color Image Box SOP Class", 
                                          SopClass.BasicColorImageBoxSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Basic Film Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.2</para>
        /// </summary>
        public static readonly String BasicFilmBoxSOPClassUid = "1.2.840.10008.5.1.1.2";

        /// <summary>SopClass for
        /// <para>Basic Film Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.2</para>
        /// </summary>
        public static readonly SopClass BasicFilmBoxSOPClass =
                             new SopClass("Basic Film Box SOP Class", 
                                          SopClass.BasicFilmBoxSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Basic Film Session SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.1</para>
        /// </summary>
        public static readonly String BasicFilmSessionSOPClassUid = "1.2.840.10008.5.1.1.1";

        /// <summary>SopClass for
        /// <para>Basic Film Session SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.1</para>
        /// </summary>
        public static readonly SopClass BasicFilmSessionSOPClass =
                             new SopClass("Basic Film Session SOP Class", 
                                          SopClass.BasicFilmSessionSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Basic Grayscale Image Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4</para>
        /// </summary>
        public static readonly String BasicGrayscaleImageBoxSOPClassUid = "1.2.840.10008.5.1.1.4";

        /// <summary>SopClass for
        /// <para>Basic Grayscale Image Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4</para>
        /// </summary>
        public static readonly SopClass BasicGrayscaleImageBoxSOPClass =
                             new SopClass("Basic Grayscale Image Box SOP Class", 
                                          SopClass.BasicGrayscaleImageBoxSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Basic Print Image Overlay Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.24.1</para>
        /// </summary>
        public static readonly String BasicPrintImageOverlayBoxSOPClassRetiredUid = "1.2.840.10008.5.1.1.24.1";

        /// <summary>SopClass for
        /// <para>Basic Print Image Overlay Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.24.1</para>
        /// </summary>
        public static readonly SopClass BasicPrintImageOverlayBoxSOPClassRetired =
                             new SopClass("Basic Print Image Overlay Box SOP Class (Retired)", 
                                          SopClass.BasicPrintImageOverlayBoxSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Basic Study Content Notification SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.9</para>
        /// </summary>
        public static readonly String BasicStudyContentNotificationSOPClassRetiredUid = "1.2.840.10008.1.9";

        /// <summary>SopClass for
        /// <para>Basic Study Content Notification SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.9</para>
        /// </summary>
        public static readonly SopClass BasicStudyContentNotificationSOPClassRetired =
                             new SopClass("Basic Study Content Notification SOP Class (Retired)", 
                                          SopClass.BasicStudyContentNotificationSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Basic Text SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.11</para>
        /// </summary>
        public static readonly String BasicTextSRUid = "1.2.840.10008.5.1.4.1.1.88.11";

        /// <summary>SopClass for
        /// <para>Basic Text SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.11</para>
        /// </summary>
        public static readonly SopClass BasicTextSR =
                             new SopClass("Basic Text SR", 
                                          SopClass.BasicTextSRUid, 
                                          false);

        /// <summary>
        /// <para>Basic Voice Audio Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.4.1</para>
        /// </summary>
        public static readonly String BasicVoiceAudioWaveformStorageUid = "1.2.840.10008.5.1.4.1.1.9.4.1";

        /// <summary>SopClass for
        /// <para>Basic Voice Audio Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.4.1</para>
        /// </summary>
        public static readonly SopClass BasicVoiceAudioWaveformStorage =
                             new SopClass("Basic Voice Audio Waveform Storage", 
                                          SopClass.BasicVoiceAudioWaveformStorageUid, 
                                          false);

        /// <summary>
        /// <para>Blending Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.4</para>
        /// </summary>
        public static readonly String BlendingSoftcopyPresentationStateStorageSOPClassUid = "1.2.840.10008.5.1.4.1.1.11.4";

        /// <summary>SopClass for
        /// <para>Blending Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.4</para>
        /// </summary>
        public static readonly SopClass BlendingSoftcopyPresentationStateStorageSOPClass =
                             new SopClass("Blending Softcopy Presentation State Storage SOP Class", 
                                          SopClass.BlendingSoftcopyPresentationStateStorageSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Breast Imaging Relevant Patient Information Query</para>
        /// <para>UID: 1.2.840.10008.5.1.4.37.2</para>
        /// </summary>
        public static readonly String BreastImagingRelevantPatientInformationQueryUid = "1.2.840.10008.5.1.4.37.2";

        /// <summary>SopClass for
        /// <para>Breast Imaging Relevant Patient Information Query</para>
        /// <para>UID: 1.2.840.10008.5.1.4.37.2</para>
        /// </summary>
        public static readonly SopClass BreastImagingRelevantPatientInformationQuery =
                             new SopClass("Breast Imaging Relevant Patient Information Query", 
                                          SopClass.BreastImagingRelevantPatientInformationQueryUid, 
                                          false);

        /// <summary>
        /// <para>Cardiac Electrophysiology Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.3.1</para>
        /// </summary>
        public static readonly String CardiacElectrophysiologyWaveformStorageUid = "1.2.840.10008.5.1.4.1.1.9.3.1";

        /// <summary>SopClass for
        /// <para>Cardiac Electrophysiology Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.3.1</para>
        /// </summary>
        public static readonly SopClass CardiacElectrophysiologyWaveformStorage =
                             new SopClass("Cardiac Electrophysiology Waveform Storage", 
                                          SopClass.CardiacElectrophysiologyWaveformStorageUid, 
                                          false);

        /// <summary>
        /// <para>Cardiac Relevant Patient Information Query</para>
        /// <para>UID: 1.2.840.10008.5.1.4.37.3</para>
        /// </summary>
        public static readonly String CardiacRelevantPatientInformationQueryUid = "1.2.840.10008.5.1.4.37.3";

        /// <summary>SopClass for
        /// <para>Cardiac Relevant Patient Information Query</para>
        /// <para>UID: 1.2.840.10008.5.1.4.37.3</para>
        /// </summary>
        public static readonly SopClass CardiacRelevantPatientInformationQuery =
                             new SopClass("Cardiac Relevant Patient Information Query", 
                                          SopClass.CardiacRelevantPatientInformationQueryUid, 
                                          false);

        /// <summary>
        /// <para>Chest CAD SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.65</para>
        /// </summary>
        public static readonly String ChestCADSRUid = "1.2.840.10008.5.1.4.1.1.88.65";

        /// <summary>SopClass for
        /// <para>Chest CAD SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.65</para>
        /// </summary>
        public static readonly SopClass ChestCADSR =
                             new SopClass("Chest CAD SR", 
                                          SopClass.ChestCADSRUid, 
                                          false);

        /// <summary>
        /// <para>Color Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.2</para>
        /// </summary>
        public static readonly String ColorSoftcopyPresentationStateStorageSOPClassUid = "1.2.840.10008.5.1.4.1.1.11.2";

        /// <summary>SopClass for
        /// <para>Color Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.2</para>
        /// </summary>
        public static readonly SopClass ColorSoftcopyPresentationStateStorageSOPClass =
                             new SopClass("Color Softcopy Presentation State Storage SOP Class", 
                                          SopClass.ColorSoftcopyPresentationStateStorageSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Comprehensive SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.33</para>
        /// </summary>
        public static readonly String ComprehensiveSRUid = "1.2.840.10008.5.1.4.1.1.88.33";

        /// <summary>SopClass for
        /// <para>Comprehensive SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.33</para>
        /// </summary>
        public static readonly SopClass ComprehensiveSR =
                             new SopClass("Comprehensive SR", 
                                          SopClass.ComprehensiveSRUid, 
                                          false);

        /// <summary>
        /// <para>Computed Radiography Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1</para>
        /// </summary>
        public static readonly String ComputedRadiographyImageStorageUid = "1.2.840.10008.5.1.4.1.1.1";

        /// <summary>SopClass for
        /// <para>Computed Radiography Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1</para>
        /// </summary>
        public static readonly SopClass ComputedRadiographyImageStorage =
                             new SopClass("Computed Radiography Image Storage", 
                                          SopClass.ComputedRadiographyImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>CT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.2</para>
        /// </summary>
        public static readonly String CTImageStorageUid = "1.2.840.10008.5.1.4.1.1.2";

        /// <summary>SopClass for
        /// <para>CT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.2</para>
        /// </summary>
        public static readonly SopClass CTImageStorage =
                             new SopClass("CT Image Storage", 
                                          SopClass.CTImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Deformable Spatial Registration Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66.3</para>
        /// </summary>
        public static readonly String DeformableSpatialRegistrationStorageUid = "1.2.840.10008.5.1.4.1.1.66.3";

        /// <summary>SopClass for
        /// <para>Deformable Spatial Registration Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66.3</para>
        /// </summary>
        public static readonly SopClass DeformableSpatialRegistrationStorage =
                             new SopClass("Deformable Spatial Registration Storage", 
                                          SopClass.DeformableSpatialRegistrationStorageUid, 
                                          false);

        /// <summary>
        /// <para>Detached Interpretation Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.6.1</para>
        /// </summary>
        public static readonly String DetachedInterpretationManagementSOPClassRetiredUid = "1.2.840.10008.3.1.2.6.1";

        /// <summary>SopClass for
        /// <para>Detached Interpretation Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.6.1</para>
        /// </summary>
        public static readonly SopClass DetachedInterpretationManagementSOPClassRetired =
                             new SopClass("Detached Interpretation Management SOP Class (Retired)", 
                                          SopClass.DetachedInterpretationManagementSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Detached Patient Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.1.1</para>
        /// </summary>
        public static readonly String DetachedPatientManagementSOPClassRetiredUid = "1.2.840.10008.3.1.2.1.1";

        /// <summary>SopClass for
        /// <para>Detached Patient Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.1.1</para>
        /// </summary>
        public static readonly SopClass DetachedPatientManagementSOPClassRetired =
                             new SopClass("Detached Patient Management SOP Class (Retired)", 
                                          SopClass.DetachedPatientManagementSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Detached Results Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.1</para>
        /// </summary>
        public static readonly String DetachedResultsManagementSOPClassRetiredUid = "1.2.840.10008.3.1.2.5.1";

        /// <summary>SopClass for
        /// <para>Detached Results Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.1</para>
        /// </summary>
        public static readonly SopClass DetachedResultsManagementSOPClassRetired =
                             new SopClass("Detached Results Management SOP Class (Retired)", 
                                          SopClass.DetachedResultsManagementSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Detached Study Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.1</para>
        /// </summary>
        public static readonly String DetachedStudyManagementSOPClassRetiredUid = "1.2.840.10008.3.1.2.3.1";

        /// <summary>SopClass for
        /// <para>Detached Study Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.1</para>
        /// </summary>
        public static readonly SopClass DetachedStudyManagementSOPClassRetired =
                             new SopClass("Detached Study Management SOP Class (Retired)", 
                                          SopClass.DetachedStudyManagementSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Detached Visit Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.2.1</para>
        /// </summary>
        public static readonly String DetachedVisitManagementSOPClassRetiredUid = "1.2.840.10008.3.1.2.2.1";

        /// <summary>SopClass for
        /// <para>Detached Visit Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.2.1</para>
        /// </summary>
        public static readonly SopClass DetachedVisitManagementSOPClassRetired =
                             new SopClass("Detached Visit Management SOP Class (Retired)", 
                                          SopClass.DetachedVisitManagementSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Digital Intra-oral X-Ray Image Storage – For Presentation</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.3</para>
        /// </summary>
        public static readonly String DigitalIntraoralXRayImageStorageForPresentationUid = "1.2.840.10008.5.1.4.1.1.1.3";

        /// <summary>SopClass for
        /// <para>Digital Intra-oral X-Ray Image Storage – For Presentation</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.3</para>
        /// </summary>
        public static readonly SopClass DigitalIntraoralXRayImageStorageForPresentation =
                             new SopClass("Digital Intra-oral X-Ray Image Storage – For Presentation", 
                                          SopClass.DigitalIntraoralXRayImageStorageForPresentationUid, 
                                          false);

        /// <summary>
        /// <para>Digital Intra-oral X-Ray Image Storage – For Processing</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.3.1</para>
        /// </summary>
        public static readonly String DigitalIntraoralXRayImageStorageForProcessingUid = "1.2.840.10008.5.1.4.1.1.1.3.1";

        /// <summary>SopClass for
        /// <para>Digital Intra-oral X-Ray Image Storage – For Processing</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.3.1</para>
        /// </summary>
        public static readonly SopClass DigitalIntraoralXRayImageStorageForProcessing =
                             new SopClass("Digital Intra-oral X-Ray Image Storage – For Processing", 
                                          SopClass.DigitalIntraoralXRayImageStorageForProcessingUid, 
                                          false);

        /// <summary>
        /// <para>Digital Mammography X-Ray Image Storage – For Presentation</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.2</para>
        /// </summary>
        public static readonly String DigitalMammographyXRayImageStorageForPresentationUid = "1.2.840.10008.5.1.4.1.1.1.2";

        /// <summary>SopClass for
        /// <para>Digital Mammography X-Ray Image Storage – For Presentation</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.2</para>
        /// </summary>
        public static readonly SopClass DigitalMammographyXRayImageStorageForPresentation =
                             new SopClass("Digital Mammography X-Ray Image Storage – For Presentation", 
                                          SopClass.DigitalMammographyXRayImageStorageForPresentationUid, 
                                          false);

        /// <summary>
        /// <para>Digital Mammography X-Ray Image Storage – For Processing</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.2.1</para>
        /// </summary>
        public static readonly String DigitalMammographyXRayImageStorageForProcessingUid = "1.2.840.10008.5.1.4.1.1.1.2.1";

        /// <summary>SopClass for
        /// <para>Digital Mammography X-Ray Image Storage – For Processing</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.2.1</para>
        /// </summary>
        public static readonly SopClass DigitalMammographyXRayImageStorageForProcessing =
                             new SopClass("Digital Mammography X-Ray Image Storage – For Processing", 
                                          SopClass.DigitalMammographyXRayImageStorageForProcessingUid, 
                                          false);

        /// <summary>
        /// <para>Digital X-Ray Image Storage – For Presentation</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.1</para>
        /// </summary>
        public static readonly String DigitalXRayImageStorageForPresentationUid = "1.2.840.10008.5.1.4.1.1.1.1";

        /// <summary>SopClass for
        /// <para>Digital X-Ray Image Storage – For Presentation</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.1</para>
        /// </summary>
        public static readonly SopClass DigitalXRayImageStorageForPresentation =
                             new SopClass("Digital X-Ray Image Storage – For Presentation", 
                                          SopClass.DigitalXRayImageStorageForPresentationUid, 
                                          false);

        /// <summary>
        /// <para>Digital X-Ray Image Storage – For Processing</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.1.1</para>
        /// </summary>
        public static readonly String DigitalXRayImageStorageForProcessingUid = "1.2.840.10008.5.1.4.1.1.1.1.1";

        /// <summary>SopClass for
        /// <para>Digital X-Ray Image Storage – For Processing</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.1.1</para>
        /// </summary>
        public static readonly SopClass DigitalXRayImageStorageForProcessing =
                             new SopClass("Digital X-Ray Image Storage – For Processing", 
                                          SopClass.DigitalXRayImageStorageForProcessingUid, 
                                          false);

        /// <summary>
        /// <para>Encapsulated PDF Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.104.1</para>
        /// </summary>
        public static readonly String EncapsulatedPDFStorageUid = "1.2.840.10008.5.1.4.1.1.104.1";

        /// <summary>SopClass for
        /// <para>Encapsulated PDF Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.104.1</para>
        /// </summary>
        public static readonly SopClass EncapsulatedPDFStorage =
                             new SopClass("Encapsulated PDF Storage", 
                                          SopClass.EncapsulatedPDFStorageUid, 
                                          false);

        /// <summary>
        /// <para>Enhanced CT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.2.1</para>
        /// </summary>
        public static readonly String EnhancedCTImageStorageUid = "1.2.840.10008.5.1.4.1.1.2.1";

        /// <summary>SopClass for
        /// <para>Enhanced CT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.2.1</para>
        /// </summary>
        public static readonly SopClass EnhancedCTImageStorage =
                             new SopClass("Enhanced CT Image Storage", 
                                          SopClass.EnhancedCTImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Enhanced MR Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4.1</para>
        /// </summary>
        public static readonly String EnhancedMRImageStorageUid = "1.2.840.10008.5.1.4.1.1.4.1";

        /// <summary>SopClass for
        /// <para>Enhanced MR Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4.1</para>
        /// </summary>
        public static readonly SopClass EnhancedMRImageStorage =
                             new SopClass("Enhanced MR Image Storage", 
                                          SopClass.EnhancedMRImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Enhanced SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.22</para>
        /// </summary>
        public static readonly String EnhancedSRUid = "1.2.840.10008.5.1.4.1.1.88.22";

        /// <summary>SopClass for
        /// <para>Enhanced SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.22</para>
        /// </summary>
        public static readonly SopClass EnhancedSR =
                             new SopClass("Enhanced SR", 
                                          SopClass.EnhancedSRUid, 
                                          false);

        /// <summary>
        /// <para>Enhanced XA Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.1.1</para>
        /// </summary>
        public static readonly String EnhancedXAImageStorageUid = "1.2.840.10008.5.1.4.1.1.12.1.1";

        /// <summary>SopClass for
        /// <para>Enhanced XA Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.1.1</para>
        /// </summary>
        public static readonly SopClass EnhancedXAImageStorage =
                             new SopClass("Enhanced XA Image Storage", 
                                          SopClass.EnhancedXAImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Enhanced XRF Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.2.1</para>
        /// </summary>
        public static readonly String EnhancedXRFImageStorageUid = "1.2.840.10008.5.1.4.1.1.12.2.1";

        /// <summary>SopClass for
        /// <para>Enhanced XRF Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.2.1</para>
        /// </summary>
        public static readonly SopClass EnhancedXRFImageStorage =
                             new SopClass("Enhanced XRF Image Storage", 
                                          SopClass.EnhancedXRFImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>General ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.2</para>
        /// </summary>
        public static readonly String GeneralECGWaveformStorageUid = "1.2.840.10008.5.1.4.1.1.9.1.2";

        /// <summary>SopClass for
        /// <para>General ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.2</para>
        /// </summary>
        public static readonly SopClass GeneralECGWaveformStorage =
                             new SopClass("General ECG Waveform Storage", 
                                          SopClass.GeneralECGWaveformStorageUid, 
                                          false);

        /// <summary>
        /// <para>General Purpose Performed Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.3</para>
        /// </summary>
        public static readonly String GeneralPurposePerformedProcedureStepSOPClassUid = "1.2.840.10008.5.1.4.32.3";

        /// <summary>SopClass for
        /// <para>General Purpose Performed Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.3</para>
        /// </summary>
        public static readonly SopClass GeneralPurposePerformedProcedureStepSOPClass =
                             new SopClass("General Purpose Performed Procedure Step SOP Class", 
                                          SopClass.GeneralPurposePerformedProcedureStepSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>General Purpose Scheduled Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.2</para>
        /// </summary>
        public static readonly String GeneralPurposeScheduledProcedureStepSOPClassUid = "1.2.840.10008.5.1.4.32.2";

        /// <summary>SopClass for
        /// <para>General Purpose Scheduled Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.2</para>
        /// </summary>
        public static readonly SopClass GeneralPurposeScheduledProcedureStepSOPClass =
                             new SopClass("General Purpose Scheduled Procedure Step SOP Class", 
                                          SopClass.GeneralPurposeScheduledProcedureStepSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>General Purpose Worklist Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.1</para>
        /// </summary>
        public static readonly String GeneralPurposeWorklistInformationModelFINDUid = "1.2.840.10008.5.1.4.32.1";

        /// <summary>SopClass for
        /// <para>General Purpose Worklist Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.1</para>
        /// </summary>
        public static readonly SopClass GeneralPurposeWorklistInformationModelFIND =
                             new SopClass("General Purpose Worklist Information Model – FIND", 
                                          SopClass.GeneralPurposeWorklistInformationModelFINDUid, 
                                          false);

        /// <summary>
        /// <para>General Relevant Patient Information Query</para>
        /// <para>UID: 1.2.840.10008.5.1.4.37.1</para>
        /// </summary>
        public static readonly String GeneralRelevantPatientInformationQueryUid = "1.2.840.10008.5.1.4.37.1";

        /// <summary>SopClass for
        /// <para>General Relevant Patient Information Query</para>
        /// <para>UID: 1.2.840.10008.5.1.4.37.1</para>
        /// </summary>
        public static readonly SopClass GeneralRelevantPatientInformationQuery =
                             new SopClass("General Relevant Patient Information Query", 
                                          SopClass.GeneralRelevantPatientInformationQueryUid, 
                                          false);

        /// <summary>
        /// <para>Grayscale Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.1</para>
        /// </summary>
        public static readonly String GrayscaleSoftcopyPresentationStateStorageSOPClassUid = "1.2.840.10008.5.1.4.1.1.11.1";

        /// <summary>SopClass for
        /// <para>Grayscale Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.1</para>
        /// </summary>
        public static readonly SopClass GrayscaleSoftcopyPresentationStateStorageSOPClass =
                             new SopClass("Grayscale Softcopy Presentation State Storage SOP Class", 
                                          SopClass.GrayscaleSoftcopyPresentationStateStorageSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Hanging Protocol Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.2</para>
        /// </summary>
        public static readonly String HangingProtocolInformationModelFINDUid = "1.2.840.10008.5.1.4.38.2";

        /// <summary>SopClass for
        /// <para>Hanging Protocol Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.2</para>
        /// </summary>
        public static readonly SopClass HangingProtocolInformationModelFIND =
                             new SopClass("Hanging Protocol Information Model – FIND", 
                                          SopClass.HangingProtocolInformationModelFINDUid, 
                                          false);

        /// <summary>
        /// <para>Hanging Protocol Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.3</para>
        /// </summary>
        public static readonly String HangingProtocolInformationModelMOVEUid = "1.2.840.10008.5.1.4.38.3";

        /// <summary>SopClass for
        /// <para>Hanging Protocol Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.3</para>
        /// </summary>
        public static readonly SopClass HangingProtocolInformationModelMOVE =
                             new SopClass("Hanging Protocol Information Model – MOVE", 
                                          SopClass.HangingProtocolInformationModelMOVEUid, 
                                          false);

        /// <summary>
        /// <para>Hanging Protocol Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.1</para>
        /// </summary>
        public static readonly String HangingProtocolStorageUid = "1.2.840.10008.5.1.4.38.1";

        /// <summary>SopClass for
        /// <para>Hanging Protocol Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.1</para>
        /// </summary>
        public static readonly SopClass HangingProtocolStorage =
                             new SopClass("Hanging Protocol Storage", 
                                          SopClass.HangingProtocolStorageUid, 
                                          false);

        /// <summary>
        /// <para>Hardcopy  Grayscale Image Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.29</para>
        /// </summary>
        public static readonly String HardcopyGrayscaleImageStorageSOPClassRetiredUid = "1.2.840.10008.5.1.1.29";

        /// <summary>SopClass for
        /// <para>Hardcopy  Grayscale Image Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.29</para>
        /// </summary>
        public static readonly SopClass HardcopyGrayscaleImageStorageSOPClassRetired =
                             new SopClass("Hardcopy  Grayscale Image Storage SOP Class (Retired)", 
                                          SopClass.HardcopyGrayscaleImageStorageSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Hardcopy Color Image Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.30</para>
        /// </summary>
        public static readonly String HardcopyColorImageStorageSOPClassRetiredUid = "1.2.840.10008.5.1.1.30";

        /// <summary>SopClass for
        /// <para>Hardcopy Color Image Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.30</para>
        /// </summary>
        public static readonly SopClass HardcopyColorImageStorageSOPClassRetired =
                             new SopClass("Hardcopy Color Image Storage SOP Class (Retired)", 
                                          SopClass.HardcopyColorImageStorageSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Hemodynamic Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.2.1</para>
        /// </summary>
        public static readonly String HemodynamicWaveformStorageUid = "1.2.840.10008.5.1.4.1.1.9.2.1";

        /// <summary>SopClass for
        /// <para>Hemodynamic Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.2.1</para>
        /// </summary>
        public static readonly SopClass HemodynamicWaveformStorage =
                             new SopClass("Hemodynamic Waveform Storage", 
                                          SopClass.HemodynamicWaveformStorageUid, 
                                          false);

        /// <summary>
        /// <para>Image Overlay Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.24</para>
        /// </summary>
        public static readonly String ImageOverlayBoxSOPClassRetiredUid = "1.2.840.10008.5.1.1.24";

        /// <summary>SopClass for
        /// <para>Image Overlay Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.24</para>
        /// </summary>
        public static readonly SopClass ImageOverlayBoxSOPClassRetired =
                             new SopClass("Image Overlay Box SOP Class (Retired)", 
                                          SopClass.ImageOverlayBoxSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Instance Availability Notification SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.33</para>
        /// </summary>
        public static readonly String InstanceAvailabilityNotificationSOPClassUid = "1.2.840.10008.5.1.4.33";

        /// <summary>SopClass for
        /// <para>Instance Availability Notification SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.33</para>
        /// </summary>
        public static readonly SopClass InstanceAvailabilityNotificationSOPClass =
                             new SopClass("Instance Availability Notification SOP Class", 
                                          SopClass.InstanceAvailabilityNotificationSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Key Object Selection Document</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.59</para>
        /// </summary>
        public static readonly String KeyObjectSelectionDocumentUid = "1.2.840.10008.5.1.4.1.1.88.59";

        /// <summary>SopClass for
        /// <para>Key Object Selection Document</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.59</para>
        /// </summary>
        public static readonly SopClass KeyObjectSelectionDocument =
                             new SopClass("Key Object Selection Document", 
                                          SopClass.KeyObjectSelectionDocumentUid, 
                                          false);

        /// <summary>
        /// <para>Mammography CAD SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.50</para>
        /// </summary>
        public static readonly String MammographyCADSRUid = "1.2.840.10008.5.1.4.1.1.88.50";

        /// <summary>SopClass for
        /// <para>Mammography CAD SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.50</para>
        /// </summary>
        public static readonly SopClass MammographyCADSR =
                             new SopClass("Mammography CAD SR", 
                                          SopClass.MammographyCADSRUid, 
                                          false);

        /// <summary>
        /// <para>Media Creation Management SOP Class UID</para>
        /// <para>UID: 1.2.840.10008.5.1.1.33</para>
        /// </summary>
        public static readonly String MediaCreationManagementSOPClassUIDUid = "1.2.840.10008.5.1.1.33";

        /// <summary>SopClass for
        /// <para>Media Creation Management SOP Class UID</para>
        /// <para>UID: 1.2.840.10008.5.1.1.33</para>
        /// </summary>
        public static readonly SopClass MediaCreationManagementSOPClassUID =
                             new SopClass("Media Creation Management SOP Class UID", 
                                          SopClass.MediaCreationManagementSOPClassUIDUid, 
                                          false);

        /// <summary>
        /// <para>Media Storage Directory Storage</para>
        /// <para>UID: 1.2.840.10008.1.3.10</para>
        /// </summary>
        public static readonly String MediaStorageDirectoryStorageUid = "1.2.840.10008.1.3.10";

        /// <summary>SopClass for
        /// <para>Media Storage Directory Storage</para>
        /// <para>UID: 1.2.840.10008.1.3.10</para>
        /// </summary>
        public static readonly SopClass MediaStorageDirectoryStorage =
                             new SopClass("Media Storage Directory Storage", 
                                          SopClass.MediaStorageDirectoryStorageUid, 
                                          false);

        /// <summary>
        /// <para>Modality Performed Procedure Step Notification SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.5</para>
        /// </summary>
        public static readonly String ModalityPerformedProcedureStepNotificationSOPClassUid = "1.2.840.10008.3.1.2.3.5";

        /// <summary>SopClass for
        /// <para>Modality Performed Procedure Step Notification SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.5</para>
        /// </summary>
        public static readonly SopClass ModalityPerformedProcedureStepNotificationSOPClass =
                             new SopClass("Modality Performed Procedure Step Notification SOP Class", 
                                          SopClass.ModalityPerformedProcedureStepNotificationSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Modality Performed Procedure Step Retrieve SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.4</para>
        /// </summary>
        public static readonly String ModalityPerformedProcedureStepRetrieveSOPClassUid = "1.2.840.10008.3.1.2.3.4";

        /// <summary>SopClass for
        /// <para>Modality Performed Procedure Step Retrieve SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.4</para>
        /// </summary>
        public static readonly SopClass ModalityPerformedProcedureStepRetrieveSOPClass =
                             new SopClass("Modality Performed Procedure Step Retrieve SOP Class", 
                                          SopClass.ModalityPerformedProcedureStepRetrieveSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Modality Performed Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.3</para>
        /// </summary>
        public static readonly String ModalityPerformedProcedureStepSOPClassUid = "1.2.840.10008.3.1.2.3.3";

        /// <summary>SopClass for
        /// <para>Modality Performed Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.3</para>
        /// </summary>
        public static readonly SopClass ModalityPerformedProcedureStepSOPClass =
                             new SopClass("Modality Performed Procedure Step SOP Class", 
                                          SopClass.ModalityPerformedProcedureStepSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Modality Worklist Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.31</para>
        /// </summary>
        public static readonly String ModalityWorklistInformationModelFINDUid = "1.2.840.10008.5.1.4.31";

        /// <summary>SopClass for
        /// <para>Modality Worklist Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.31</para>
        /// </summary>
        public static readonly SopClass ModalityWorklistInformationModelFIND =
                             new SopClass("Modality Worklist Information Model – FIND", 
                                          SopClass.ModalityWorklistInformationModelFINDUid, 
                                          false);

        /// <summary>
        /// <para>MR Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4</para>
        /// </summary>
        public static readonly String MRImageStorageUid = "1.2.840.10008.5.1.4.1.1.4";

        /// <summary>SopClass for
        /// <para>MR Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4</para>
        /// </summary>
        public static readonly SopClass MRImageStorage =
                             new SopClass("MR Image Storage", 
                                          SopClass.MRImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>MR Spectroscopy Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4.2</para>
        /// </summary>
        public static readonly String MRSpectroscopyStorageUid = "1.2.840.10008.5.1.4.1.1.4.2";

        /// <summary>SopClass for
        /// <para>MR Spectroscopy Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4.2</para>
        /// </summary>
        public static readonly SopClass MRSpectroscopyStorage =
                             new SopClass("MR Spectroscopy Storage", 
                                          SopClass.MRSpectroscopyStorageUid, 
                                          false);

        /// <summary>
        /// <para>Multi-frame Grayscale Byte Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.2</para>
        /// </summary>
        public static readonly String MultiframeGrayscaleByteSecondaryCaptureImageStorageUid = "1.2.840.10008.5.1.4.1.1.7.2";

        /// <summary>SopClass for
        /// <para>Multi-frame Grayscale Byte Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.2</para>
        /// </summary>
        public static readonly SopClass MultiframeGrayscaleByteSecondaryCaptureImageStorage =
                             new SopClass("Multi-frame Grayscale Byte Secondary Capture Image Storage", 
                                          SopClass.MultiframeGrayscaleByteSecondaryCaptureImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Multi-frame Grayscale Word Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.3</para>
        /// </summary>
        public static readonly String MultiframeGrayscaleWordSecondaryCaptureImageStorageUid = "1.2.840.10008.5.1.4.1.1.7.3";

        /// <summary>SopClass for
        /// <para>Multi-frame Grayscale Word Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.3</para>
        /// </summary>
        public static readonly SopClass MultiframeGrayscaleWordSecondaryCaptureImageStorage =
                             new SopClass("Multi-frame Grayscale Word Secondary Capture Image Storage", 
                                          SopClass.MultiframeGrayscaleWordSecondaryCaptureImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Multi-frame Single Bit Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.1</para>
        /// </summary>
        public static readonly String MultiframeSingleBitSecondaryCaptureImageStorageUid = "1.2.840.10008.5.1.4.1.1.7.1";

        /// <summary>SopClass for
        /// <para>Multi-frame Single Bit Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.1</para>
        /// </summary>
        public static readonly SopClass MultiframeSingleBitSecondaryCaptureImageStorage =
                             new SopClass("Multi-frame Single Bit Secondary Capture Image Storage", 
                                          SopClass.MultiframeSingleBitSecondaryCaptureImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Multi-frame True Color Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.4</para>
        /// </summary>
        public static readonly String MultiframeTrueColorSecondaryCaptureImageStorageUid = "1.2.840.10008.5.1.4.1.1.7.4";

        /// <summary>SopClass for
        /// <para>Multi-frame True Color Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.4</para>
        /// </summary>
        public static readonly SopClass MultiframeTrueColorSecondaryCaptureImageStorage =
                             new SopClass("Multi-frame True Color Secondary Capture Image Storage", 
                                          SopClass.MultiframeTrueColorSecondaryCaptureImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Nuclear Medicine Image  Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.5</para>
        /// </summary>
        public static readonly String NuclearMedicineImageStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.5";

        /// <summary>SopClass for
        /// <para>Nuclear Medicine Image  Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.5</para>
        /// </summary>
        public static readonly SopClass NuclearMedicineImageStorageRetired =
                             new SopClass("Nuclear Medicine Image  Storage (Retired)", 
                                          SopClass.NuclearMedicineImageStorageRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Nuclear Medicine Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.20</para>
        /// </summary>
        public static readonly String NuclearMedicineImageStorageUid = "1.2.840.10008.5.1.4.1.1.20";

        /// <summary>SopClass for
        /// <para>Nuclear Medicine Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.20</para>
        /// </summary>
        public static readonly SopClass NuclearMedicineImageStorage =
                             new SopClass("Nuclear Medicine Image Storage", 
                                          SopClass.NuclearMedicineImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Ophthalmic Photography 16 Bit Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.5.2</para>
        /// </summary>
        public static readonly String OphthalmicPhotography16BitImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.5.2";

        /// <summary>SopClass for
        /// <para>Ophthalmic Photography 16 Bit Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.5.2</para>
        /// </summary>
        public static readonly SopClass OphthalmicPhotography16BitImageStorage =
                             new SopClass("Ophthalmic Photography 16 Bit Image Storage", 
                                          SopClass.OphthalmicPhotography16BitImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Ophthalmic Photography 8 Bit Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.5.1</para>
        /// </summary>
        public static readonly String OphthalmicPhotography8BitImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.5.1";

        /// <summary>SopClass for
        /// <para>Ophthalmic Photography 8 Bit Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.5.1</para>
        /// </summary>
        public static readonly SopClass OphthalmicPhotography8BitImageStorage =
                             new SopClass("Ophthalmic Photography 8 Bit Image Storage", 
                                          SopClass.OphthalmicPhotography8BitImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Patient Root Query/Retrieve Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.1</para>
        /// </summary>
        public static readonly String PatientRootQueryRetrieveInformationModelFINDUid = "1.2.840.10008.5.1.4.1.2.1.1";

        /// <summary>SopClass for
        /// <para>Patient Root Query/Retrieve Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.1</para>
        /// </summary>
        public static readonly SopClass PatientRootQueryRetrieveInformationModelFIND =
                             new SopClass("Patient Root Query/Retrieve Information Model – FIND", 
                                          SopClass.PatientRootQueryRetrieveInformationModelFINDUid, 
                                          false);

        /// <summary>
        /// <para>Patient Root Query/Retrieve Information Model – GET</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.3</para>
        /// </summary>
        public static readonly String PatientRootQueryRetrieveInformationModelGETUid = "1.2.840.10008.5.1.4.1.2.1.3";

        /// <summary>SopClass for
        /// <para>Patient Root Query/Retrieve Information Model – GET</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.3</para>
        /// </summary>
        public static readonly SopClass PatientRootQueryRetrieveInformationModelGET =
                             new SopClass("Patient Root Query/Retrieve Information Model – GET", 
                                          SopClass.PatientRootQueryRetrieveInformationModelGETUid, 
                                          false);

        /// <summary>
        /// <para>Patient Root Query/Retrieve Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.2</para>
        /// </summary>
        public static readonly String PatientRootQueryRetrieveInformationModelMOVEUid = "1.2.840.10008.5.1.4.1.2.1.2";

        /// <summary>SopClass for
        /// <para>Patient Root Query/Retrieve Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.2</para>
        /// </summary>
        public static readonly SopClass PatientRootQueryRetrieveInformationModelMOVE =
                             new SopClass("Patient Root Query/Retrieve Information Model – MOVE", 
                                          SopClass.PatientRootQueryRetrieveInformationModelMOVEUid, 
                                          false);

        /// <summary>
        /// <para>Patient/Study Only Query/Retrieve Information Model  - FIND (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.1</para>
        /// </summary>
        public static readonly String PatientStudyOnlyQueryRetrieveInformationModelFINDRetiredUid = "1.2.840.10008.5.1.4.1.2.3.1";

        /// <summary>SopClass for
        /// <para>Patient/Study Only Query/Retrieve Information Model  - FIND (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.1</para>
        /// </summary>
        public static readonly SopClass PatientStudyOnlyQueryRetrieveInformationModelFINDRetired =
                             new SopClass("Patient/Study Only Query/Retrieve Information Model  - FIND (Retired)", 
                                          SopClass.PatientStudyOnlyQueryRetrieveInformationModelFINDRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Patient/Study Only Query/Retrieve Information Model  - GET (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.3</para>
        /// </summary>
        public static readonly String PatientStudyOnlyQueryRetrieveInformationModelGETRetiredUid = "1.2.840.10008.5.1.4.1.2.3.3";

        /// <summary>SopClass for
        /// <para>Patient/Study Only Query/Retrieve Information Model  - GET (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.3</para>
        /// </summary>
        public static readonly SopClass PatientStudyOnlyQueryRetrieveInformationModelGETRetired =
                             new SopClass("Patient/Study Only Query/Retrieve Information Model  - GET (Retired)", 
                                          SopClass.PatientStudyOnlyQueryRetrieveInformationModelGETRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Patient/Study Only Query/Retrieve Information Model  - MOVE (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.2</para>
        /// </summary>
        public static readonly String PatientStudyOnlyQueryRetrieveInformationModelMOVERetiredUid = "1.2.840.10008.5.1.4.1.2.3.2";

        /// <summary>SopClass for
        /// <para>Patient/Study Only Query/Retrieve Information Model  - MOVE (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.2</para>
        /// </summary>
        public static readonly SopClass PatientStudyOnlyQueryRetrieveInformationModelMOVERetired =
                             new SopClass("Patient/Study Only Query/Retrieve Information Model  - MOVE (Retired)", 
                                          SopClass.PatientStudyOnlyQueryRetrieveInformationModelMOVERetiredUid, 
                                          false);

        /// <summary>
        /// <para>Positron Emission Tomography Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.128</para>
        /// </summary>
        public static readonly String PositronEmissionTomographyImageStorageUid = "1.2.840.10008.5.1.4.1.1.128";

        /// <summary>SopClass for
        /// <para>Positron Emission Tomography Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.128</para>
        /// </summary>
        public static readonly SopClass PositronEmissionTomographyImageStorage =
                             new SopClass("Positron Emission Tomography Image Storage", 
                                          SopClass.PositronEmissionTomographyImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Presentation LUT SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.23</para>
        /// </summary>
        public static readonly String PresentationLUTSOPClassUid = "1.2.840.10008.5.1.1.23";

        /// <summary>SopClass for
        /// <para>Presentation LUT SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.23</para>
        /// </summary>
        public static readonly SopClass PresentationLUTSOPClass =
                             new SopClass("Presentation LUT SOP Class", 
                                          SopClass.PresentationLUTSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Print Job SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.14</para>
        /// </summary>
        public static readonly String PrintJobSOPClassUid = "1.2.840.10008.5.1.1.14";

        /// <summary>SopClass for
        /// <para>Print Job SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.14</para>
        /// </summary>
        public static readonly SopClass PrintJobSOPClass =
                             new SopClass("Print Job SOP Class", 
                                          SopClass.PrintJobSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Print Queue Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.26</para>
        /// </summary>
        public static readonly String PrintQueueManagementSOPClassRetiredUid = "1.2.840.10008.5.1.1.26";

        /// <summary>SopClass for
        /// <para>Print Queue Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.26</para>
        /// </summary>
        public static readonly SopClass PrintQueueManagementSOPClassRetired =
                             new SopClass("Print Queue Management SOP Class (Retired)", 
                                          SopClass.PrintQueueManagementSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Printer Configuration Retrieval SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.16.376</para>
        /// </summary>
        public static readonly String PrinterConfigurationRetrievalSOPClassUid = "1.2.840.10008.5.1.1.16.376";

        /// <summary>SopClass for
        /// <para>Printer Configuration Retrieval SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.16.376</para>
        /// </summary>
        public static readonly SopClass PrinterConfigurationRetrievalSOPClass =
                             new SopClass("Printer Configuration Retrieval SOP Class", 
                                          SopClass.PrinterConfigurationRetrievalSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Printer SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.16</para>
        /// </summary>
        public static readonly String PrinterSOPClassUid = "1.2.840.10008.5.1.1.16";

        /// <summary>SopClass for
        /// <para>Printer SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.16</para>
        /// </summary>
        public static readonly SopClass PrinterSOPClass =
                             new SopClass("Printer SOP Class", 
                                          SopClass.PrinterSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Procedural Event Logging SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.40</para>
        /// </summary>
        public static readonly String ProceduralEventLoggingSOPClassUid = "1.2.840.10008.1.40";

        /// <summary>SopClass for
        /// <para>Procedural Event Logging SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.40</para>
        /// </summary>
        public static readonly SopClass ProceduralEventLoggingSOPClass =
                             new SopClass("Procedural Event Logging SOP Class", 
                                          SopClass.ProceduralEventLoggingSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Procedure Log Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.40</para>
        /// </summary>
        public static readonly String ProcedureLogStorageUid = "1.2.840.10008.5.1.4.1.1.88.40";

        /// <summary>SopClass for
        /// <para>Procedure Log Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.40</para>
        /// </summary>
        public static readonly SopClass ProcedureLogStorage =
                             new SopClass("Procedure Log Storage", 
                                          SopClass.ProcedureLogStorageUid, 
                                          false);

        /// <summary>
        /// <para>Pseudo-Color Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.3</para>
        /// </summary>
        public static readonly String PseudoColorSoftcopyPresentationStateStorageSOPClassUid = "1.2.840.10008.5.1.4.1.1.11.3";

        /// <summary>SopClass for
        /// <para>Pseudo-Color Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.3</para>
        /// </summary>
        public static readonly SopClass PseudoColorSoftcopyPresentationStateStorageSOPClass =
                             new SopClass("Pseudo-Color Softcopy Presentation State Storage SOP Class", 
                                          SopClass.PseudoColorSoftcopyPresentationStateStorageSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Pull Print Request SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.31</para>
        /// </summary>
        public static readonly String PullPrintRequestSOPClassRetiredUid = "1.2.840.10008.5.1.1.31";

        /// <summary>SopClass for
        /// <para>Pull Print Request SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.31</para>
        /// </summary>
        public static readonly SopClass PullPrintRequestSOPClassRetired =
                             new SopClass("Pull Print Request SOP Class (Retired)", 
                                          SopClass.PullPrintRequestSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Raw Data Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66</para>
        /// </summary>
        public static readonly String RawDataStorageUid = "1.2.840.10008.5.1.4.1.1.66";

        /// <summary>SopClass for
        /// <para>Raw Data Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66</para>
        /// </summary>
        public static readonly SopClass RawDataStorage =
                             new SopClass("Raw Data Storage", 
                                          SopClass.RawDataStorageUid, 
                                          false);

        /// <summary>
        /// <para>Real World Value Mapping Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.67</para>
        /// </summary>
        public static readonly String RealWorldValueMappingStorageUid = "1.2.840.10008.5.1.4.1.1.67";

        /// <summary>SopClass for
        /// <para>Real World Value Mapping Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.67</para>
        /// </summary>
        public static readonly SopClass RealWorldValueMappingStorage =
                             new SopClass("Real World Value Mapping Storage", 
                                          SopClass.RealWorldValueMappingStorageUid, 
                                          false);

        /// <summary>
        /// <para>Referenced Image Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4.2</para>
        /// </summary>
        public static readonly String ReferencedImageBoxSOPClassRetiredUid = "1.2.840.10008.5.1.1.4.2";

        /// <summary>SopClass for
        /// <para>Referenced Image Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4.2</para>
        /// </summary>
        public static readonly SopClass ReferencedImageBoxSOPClassRetired =
                             new SopClass("Referenced Image Box SOP Class (Retired)", 
                                          SopClass.ReferencedImageBoxSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>RT Beams Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.4</para>
        /// </summary>
        public static readonly String RTBeamsTreatmentRecordStorageUid = "1.2.840.10008.5.1.4.1.1.481.4";

        /// <summary>SopClass for
        /// <para>RT Beams Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.4</para>
        /// </summary>
        public static readonly SopClass RTBeamsTreatmentRecordStorage =
                             new SopClass("RT Beams Treatment Record Storage", 
                                          SopClass.RTBeamsTreatmentRecordStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Brachy Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.6</para>
        /// </summary>
        public static readonly String RTBrachyTreatmentRecordStorageUid = "1.2.840.10008.5.1.4.1.1.481.6";

        /// <summary>SopClass for
        /// <para>RT Brachy Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.6</para>
        /// </summary>
        public static readonly SopClass RTBrachyTreatmentRecordStorage =
                             new SopClass("RT Brachy Treatment Record Storage", 
                                          SopClass.RTBrachyTreatmentRecordStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Dose Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.2</para>
        /// </summary>
        public static readonly String RTDoseStorageUid = "1.2.840.10008.5.1.4.1.1.481.2";

        /// <summary>SopClass for
        /// <para>RT Dose Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.2</para>
        /// </summary>
        public static readonly SopClass RTDoseStorage =
                             new SopClass("RT Dose Storage", 
                                          SopClass.RTDoseStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.1</para>
        /// </summary>
        public static readonly String RTImageStorageUid = "1.2.840.10008.5.1.4.1.1.481.1";

        /// <summary>SopClass for
        /// <para>RT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.1</para>
        /// </summary>
        public static readonly SopClass RTImageStorage =
                             new SopClass("RT Image Storage", 
                                          SopClass.RTImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Ion Beams Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.9</para>
        /// </summary>
        public static readonly String RTIonBeamsTreatmentRecordStorageUid = "1.2.840.10008.5.1.4.1.1.481.9";

        /// <summary>SopClass for
        /// <para>RT Ion Beams Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.9</para>
        /// </summary>
        public static readonly SopClass RTIonBeamsTreatmentRecordStorage =
                             new SopClass("RT Ion Beams Treatment Record Storage", 
                                          SopClass.RTIonBeamsTreatmentRecordStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Ion Plan Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.8</para>
        /// </summary>
        public static readonly String RTIonPlanStorageUid = "1.2.840.10008.5.1.4.1.1.481.8";

        /// <summary>SopClass for
        /// <para>RT Ion Plan Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.8</para>
        /// </summary>
        public static readonly SopClass RTIonPlanStorage =
                             new SopClass("RT Ion Plan Storage", 
                                          SopClass.RTIonPlanStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Plan Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.5</para>
        /// </summary>
        public static readonly String RTPlanStorageUid = "1.2.840.10008.5.1.4.1.1.481.5";

        /// <summary>SopClass for
        /// <para>RT Plan Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.5</para>
        /// </summary>
        public static readonly SopClass RTPlanStorage =
                             new SopClass("RT Plan Storage", 
                                          SopClass.RTPlanStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Structure Set Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.3</para>
        /// </summary>
        public static readonly String RTStructureSetStorageUid = "1.2.840.10008.5.1.4.1.1.481.3";

        /// <summary>SopClass for
        /// <para>RT Structure Set Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.3</para>
        /// </summary>
        public static readonly SopClass RTStructureSetStorage =
                             new SopClass("RT Structure Set Storage", 
                                          SopClass.RTStructureSetStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Treatment Summary Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.7</para>
        /// </summary>
        public static readonly String RTTreatmentSummaryRecordStorageUid = "1.2.840.10008.5.1.4.1.1.481.7";

        /// <summary>SopClass for
        /// <para>RT Treatment Summary Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.7</para>
        /// </summary>
        public static readonly SopClass RTTreatmentSummaryRecordStorage =
                             new SopClass("RT Treatment Summary Record Storage", 
                                          SopClass.RTTreatmentSummaryRecordStorageUid, 
                                          false);

        /// <summary>
        /// <para>Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7</para>
        /// </summary>
        public static readonly String SecondaryCaptureImageStorageUid = "1.2.840.10008.5.1.4.1.1.7";

        /// <summary>SopClass for
        /// <para>Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7</para>
        /// </summary>
        public static readonly SopClass SecondaryCaptureImageStorage =
                             new SopClass("Secondary Capture Image Storage", 
                                          SopClass.SecondaryCaptureImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Segmentation Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66.4</para>
        /// </summary>
        public static readonly String SegmentationStorageUid = "1.2.840.10008.5.1.4.1.1.66.4";

        /// <summary>SopClass for
        /// <para>Segmentation Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66.4</para>
        /// </summary>
        public static readonly SopClass SegmentationStorage =
                             new SopClass("Segmentation Storage", 
                                          SopClass.SegmentationStorageUid, 
                                          false);

        /// <summary>
        /// <para>Spatial Fiducials Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66.2</para>
        /// </summary>
        public static readonly String SpatialFiducialsStorageUid = "1.2.840.10008.5.1.4.1.1.66.2";

        /// <summary>SopClass for
        /// <para>Spatial Fiducials Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66.2</para>
        /// </summary>
        public static readonly SopClass SpatialFiducialsStorage =
                             new SopClass("Spatial Fiducials Storage", 
                                          SopClass.SpatialFiducialsStorageUid, 
                                          false);

        /// <summary>
        /// <para>Spatial Registration Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66.1</para>
        /// </summary>
        public static readonly String SpatialRegistrationStorageUid = "1.2.840.10008.5.1.4.1.1.66.1";

        /// <summary>SopClass for
        /// <para>Spatial Registration Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.66.1</para>
        /// </summary>
        public static readonly SopClass SpatialRegistrationStorage =
                             new SopClass("Spatial Registration Storage", 
                                          SopClass.SpatialRegistrationStorageUid, 
                                          false);

        /// <summary>
        /// <para>Standalone Curve Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9</para>
        /// </summary>
        public static readonly String StandaloneCurveStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.9";

        /// <summary>SopClass for
        /// <para>Standalone Curve Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9</para>
        /// </summary>
        public static readonly SopClass StandaloneCurveStorageRetired =
                             new SopClass("Standalone Curve Storage (Retired)", 
                                          SopClass.StandaloneCurveStorageRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Standalone Modality LUT Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.10</para>
        /// </summary>
        public static readonly String StandaloneModalityLUTStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.10";

        /// <summary>SopClass for
        /// <para>Standalone Modality LUT Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.10</para>
        /// </summary>
        public static readonly SopClass StandaloneModalityLUTStorageRetired =
                             new SopClass("Standalone Modality LUT Storage (Retired)", 
                                          SopClass.StandaloneModalityLUTStorageRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Standalone Overlay Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.8</para>
        /// </summary>
        public static readonly String StandaloneOverlayStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.8";

        /// <summary>SopClass for
        /// <para>Standalone Overlay Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.8</para>
        /// </summary>
        public static readonly SopClass StandaloneOverlayStorageRetired =
                             new SopClass("Standalone Overlay Storage (Retired)", 
                                          SopClass.StandaloneOverlayStorageRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Standalone PET Curve Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.129</para>
        /// </summary>
        public static readonly String StandalonePETCurveStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.129";

        /// <summary>SopClass for
        /// <para>Standalone PET Curve Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.129</para>
        /// </summary>
        public static readonly SopClass StandalonePETCurveStorageRetired =
                             new SopClass("Standalone PET Curve Storage (Retired)", 
                                          SopClass.StandalonePETCurveStorageRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Standalone VOI LUT Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11</para>
        /// </summary>
        public static readonly String StandaloneVOILUTStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.11";

        /// <summary>SopClass for
        /// <para>Standalone VOI LUT Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11</para>
        /// </summary>
        public static readonly SopClass StandaloneVOILUTStorageRetired =
                             new SopClass("Standalone VOI LUT Storage (Retired)", 
                                          SopClass.StandaloneVOILUTStorageRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Stereometric Relationship Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.5.3</para>
        /// </summary>
        public static readonly String StereometricRelationshipStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.5.3";

        /// <summary>SopClass for
        /// <para>Stereometric Relationship Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.5.3</para>
        /// </summary>
        public static readonly SopClass StereometricRelationshipStorage =
                             new SopClass("Stereometric Relationship Storage", 
                                          SopClass.StereometricRelationshipStorageUid, 
                                          false);

        /// <summary>
        /// <para>Storage Commitment Pull Model SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.20.2</para>
        /// </summary>
        public static readonly String StorageCommitmentPullModelSOPClassRetiredUid = "1.2.840.10008.1.20.2";

        /// <summary>SopClass for
        /// <para>Storage Commitment Pull Model SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.20.2</para>
        /// </summary>
        public static readonly SopClass StorageCommitmentPullModelSOPClassRetired =
                             new SopClass("Storage Commitment Pull Model SOP Class (Retired)", 
                                          SopClass.StorageCommitmentPullModelSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Storage Commitment Push Model SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.20.1</para>
        /// </summary>
        public static readonly String StorageCommitmentPushModelSOPClassUid = "1.2.840.10008.1.20.1";

        /// <summary>SopClass for
        /// <para>Storage Commitment Push Model SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.20.1</para>
        /// </summary>
        public static readonly SopClass StorageCommitmentPushModelSOPClass =
                             new SopClass("Storage Commitment Push Model SOP Class", 
                                          SopClass.StorageCommitmentPushModelSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Stored Print Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.27</para>
        /// </summary>
        public static readonly String StoredPrintStorageSOPClassRetiredUid = "1.2.840.10008.5.1.1.27";

        /// <summary>SopClass for
        /// <para>Stored Print Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.27</para>
        /// </summary>
        public static readonly SopClass StoredPrintStorageSOPClassRetired =
                             new SopClass("Stored Print Storage SOP Class (Retired)", 
                                          SopClass.StoredPrintStorageSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Study Component Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.2</para>
        /// </summary>
        public static readonly String StudyComponentManagementSOPClassRetiredUid = "1.2.840.10008.3.1.2.3.2";

        /// <summary>SopClass for
        /// <para>Study Component Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.2</para>
        /// </summary>
        public static readonly SopClass StudyComponentManagementSOPClassRetired =
                             new SopClass("Study Component Management SOP Class (Retired)", 
                                          SopClass.StudyComponentManagementSOPClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Study Root Query/Retrieve Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.1</para>
        /// </summary>
        public static readonly String StudyRootQueryRetrieveInformationModelFINDUid = "1.2.840.10008.5.1.4.1.2.2.1";

        /// <summary>SopClass for
        /// <para>Study Root Query/Retrieve Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.1</para>
        /// </summary>
        public static readonly SopClass StudyRootQueryRetrieveInformationModelFIND =
                             new SopClass("Study Root Query/Retrieve Information Model – FIND", 
                                          SopClass.StudyRootQueryRetrieveInformationModelFINDUid, 
                                          false);

        /// <summary>
        /// <para>Study Root Query/Retrieve Information Model – GET</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.3</para>
        /// </summary>
        public static readonly String StudyRootQueryRetrieveInformationModelGETUid = "1.2.840.10008.5.1.4.1.2.2.3";

        /// <summary>SopClass for
        /// <para>Study Root Query/Retrieve Information Model – GET</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.3</para>
        /// </summary>
        public static readonly SopClass StudyRootQueryRetrieveInformationModelGET =
                             new SopClass("Study Root Query/Retrieve Information Model – GET", 
                                          SopClass.StudyRootQueryRetrieveInformationModelGETUid, 
                                          false);

        /// <summary>
        /// <para>Study Root Query/Retrieve Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.2</para>
        /// </summary>
        public static readonly String StudyRootQueryRetrieveInformationModelMOVEUid = "1.2.840.10008.5.1.4.1.2.2.2";

        /// <summary>SopClass for
        /// <para>Study Root Query/Retrieve Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.2</para>
        /// </summary>
        public static readonly SopClass StudyRootQueryRetrieveInformationModelMOVE =
                             new SopClass("Study Root Query/Retrieve Information Model – MOVE", 
                                          SopClass.StudyRootQueryRetrieveInformationModelMOVEUid, 
                                          false);

        /// <summary>
        /// <para>Ultrasound Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.6.1</para>
        /// </summary>
        public static readonly String UltrasoundImageStorageUid = "1.2.840.10008.5.1.4.1.1.6.1";

        /// <summary>SopClass for
        /// <para>Ultrasound Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.6.1</para>
        /// </summary>
        public static readonly SopClass UltrasoundImageStorage =
                             new SopClass("Ultrasound Image Storage", 
                                          SopClass.UltrasoundImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Ultrasound Image Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.6</para>
        /// </summary>
        public static readonly String UltrasoundImageStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.6";

        /// <summary>SopClass for
        /// <para>Ultrasound Image Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.6</para>
        /// </summary>
        public static readonly SopClass UltrasoundImageStorageRetired =
                             new SopClass("Ultrasound Image Storage (Retired)", 
                                          SopClass.UltrasoundImageStorageRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Ultrasound Multi-frame Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.3.1</para>
        /// </summary>
        public static readonly String UltrasoundMultiframeImageStorageUid = "1.2.840.10008.5.1.4.1.1.3.1";

        /// <summary>SopClass for
        /// <para>Ultrasound Multi-frame Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.3.1</para>
        /// </summary>
        public static readonly SopClass UltrasoundMultiframeImageStorage =
                             new SopClass("Ultrasound Multi-frame Image Storage", 
                                          SopClass.UltrasoundMultiframeImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Ultrasound Multi-frame Image Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.3</para>
        /// </summary>
        public static readonly String UltrasoundMultiframeImageStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.3";

        /// <summary>SopClass for
        /// <para>Ultrasound Multi-frame Image Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.3</para>
        /// </summary>
        public static readonly SopClass UltrasoundMultiframeImageStorageRetired =
                             new SopClass("Ultrasound Multi-frame Image Storage (Retired)", 
                                          SopClass.UltrasoundMultiframeImageStorageRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Verification SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.1</para>
        /// </summary>
        public static readonly String VerificationSOPClassUid = "1.2.840.10008.1.1";

        /// <summary>SopClass for
        /// <para>Verification SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.1</para>
        /// </summary>
        public static readonly SopClass VerificationSOPClass =
                             new SopClass("Verification SOP Class", 
                                          SopClass.VerificationSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>Video Endoscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.1.1</para>
        /// </summary>
        public static readonly String VideoEndoscopicImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.1.1";

        /// <summary>SopClass for
        /// <para>Video Endoscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.1.1</para>
        /// </summary>
        public static readonly SopClass VideoEndoscopicImageStorage =
                             new SopClass("Video Endoscopic Image Storage", 
                                          SopClass.VideoEndoscopicImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Video Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.2.1</para>
        /// </summary>
        public static readonly String VideoMicroscopicImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.2.1";

        /// <summary>SopClass for
        /// <para>Video Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.2.1</para>
        /// </summary>
        public static readonly SopClass VideoMicroscopicImageStorage =
                             new SopClass("Video Microscopic Image Storage", 
                                          SopClass.VideoMicroscopicImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Video Photographic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.4.1</para>
        /// </summary>
        public static readonly String VideoPhotographicImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.4.1";

        /// <summary>SopClass for
        /// <para>Video Photographic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.4.1</para>
        /// </summary>
        public static readonly SopClass VideoPhotographicImageStorage =
                             new SopClass("Video Photographic Image Storage", 
                                          SopClass.VideoPhotographicImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>VL Endoscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.1</para>
        /// </summary>
        public static readonly String VLEndoscopicImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.1";

        /// <summary>SopClass for
        /// <para>VL Endoscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.1</para>
        /// </summary>
        public static readonly SopClass VLEndoscopicImageStorage =
                             new SopClass("VL Endoscopic Image Storage", 
                                          SopClass.VLEndoscopicImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>VL Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.2</para>
        /// </summary>
        public static readonly String VLMicroscopicImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.2";

        /// <summary>SopClass for
        /// <para>VL Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.2</para>
        /// </summary>
        public static readonly SopClass VLMicroscopicImageStorage =
                             new SopClass("VL Microscopic Image Storage", 
                                          SopClass.VLMicroscopicImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>VL Photographic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.4</para>
        /// </summary>
        public static readonly String VLPhotographicImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.4";

        /// <summary>SopClass for
        /// <para>VL Photographic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.4</para>
        /// </summary>
        public static readonly SopClass VLPhotographicImageStorage =
                             new SopClass("VL Photographic Image Storage", 
                                          SopClass.VLPhotographicImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>VL Slide-Coordinates Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.3</para>
        /// </summary>
        public static readonly String VLSlideCoordinatesMicroscopicImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.3";

        /// <summary>SopClass for
        /// <para>VL Slide-Coordinates Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.3</para>
        /// </summary>
        public static readonly SopClass VLSlideCoordinatesMicroscopicImageStorage =
                             new SopClass("VL Slide-Coordinates Microscopic Image Storage", 
                                          SopClass.VLSlideCoordinatesMicroscopicImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>VOI LUT Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.22</para>
        /// </summary>
        public static readonly String VOILUTBoxSOPClassUid = "1.2.840.10008.5.1.1.22";

        /// <summary>SopClass for
        /// <para>VOI LUT Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.22</para>
        /// </summary>
        public static readonly SopClass VOILUTBoxSOPClass =
                             new SopClass("VOI LUT Box SOP Class", 
                                          SopClass.VOILUTBoxSOPClassUid, 
                                          false);

        /// <summary>
        /// <para>X-Ray Angiographic Bi-Plane Image Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.3</para>
        /// </summary>
        public static readonly String XRayAngiographicBiPlaneImageStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.12.3";

        /// <summary>SopClass for
        /// <para>X-Ray Angiographic Bi-Plane Image Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.3</para>
        /// </summary>
        public static readonly SopClass XRayAngiographicBiPlaneImageStorageRetired =
                             new SopClass("X-Ray Angiographic Bi-Plane Image Storage (Retired)", 
                                          SopClass.XRayAngiographicBiPlaneImageStorageRetiredUid, 
                                          false);

        /// <summary>
        /// <para>X-Ray Angiographic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.1</para>
        /// </summary>
        public static readonly String XRayAngiographicImageStorageUid = "1.2.840.10008.5.1.4.1.1.12.1";

        /// <summary>SopClass for
        /// <para>X-Ray Angiographic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.1</para>
        /// </summary>
        public static readonly SopClass XRayAngiographicImageStorage =
                             new SopClass("X-Ray Angiographic Image Storage", 
                                          SopClass.XRayAngiographicImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>X-Ray Radiation Dose SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.67</para>
        /// </summary>
        public static readonly String XRayRadiationDoseSRUid = "1.2.840.10008.5.1.4.1.1.88.67";

        /// <summary>SopClass for
        /// <para>X-Ray Radiation Dose SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.67</para>
        /// </summary>
        public static readonly SopClass XRayRadiationDoseSR =
                             new SopClass("X-Ray Radiation Dose SR", 
                                          SopClass.XRayRadiationDoseSRUid, 
                                          false);

        /// <summary>
        /// <para>X-Ray Radiofluoroscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.2</para>
        /// </summary>
        public static readonly String XRayRadiofluoroscopicImageStorageUid = "1.2.840.10008.5.1.4.1.1.12.2";

        /// <summary>SopClass for
        /// <para>X-Ray Radiofluoroscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.2</para>
        /// </summary>
        public static readonly SopClass XRayRadiofluoroscopicImageStorage =
                             new SopClass("X-Ray Radiofluoroscopic Image Storage", 
                                          SopClass.XRayRadiofluoroscopicImageStorageUid, 
                                          false);

        /// <summary>String UID for
        /// <para>Basic Color Print Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.18</para>
        /// </summary>
        public static readonly String BasicColorPrintManagementMetaSOPClassUid = "1.2.840.10008.5.1.1.18";

        /// <summary>SopClass for
        /// <para>Basic Color Print Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.18</para>
        /// </summary>
        public static readonly SopClass BasicColorPrintManagementMetaSOPClass =
                             new SopClass("Basic Color Print Management Meta SOP Class", 
                                          SopClass.BasicColorPrintManagementMetaSOPClassUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Basic Grayscale Print Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.9</para>
        /// </summary>
        public static readonly String BasicGrayscalePrintManagementMetaSOPClassUid = "1.2.840.10008.5.1.1.9";

        /// <summary>SopClass for
        /// <para>Basic Grayscale Print Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.9</para>
        /// </summary>
        public static readonly SopClass BasicGrayscalePrintManagementMetaSOPClass =
                             new SopClass("Basic Grayscale Print Management Meta SOP Class", 
                                          SopClass.BasicGrayscalePrintManagementMetaSOPClassUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Detached Patient Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.1.4</para>
        /// </summary>
        public static readonly String DetachedPatientManagementMetaSOPClassRetiredUid = "1.2.840.10008.3.1.2.1.4";

        /// <summary>SopClass for
        /// <para>Detached Patient Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.1.4</para>
        /// </summary>
        public static readonly SopClass DetachedPatientManagementMetaSOPClassRetired =
                             new SopClass("Detached Patient Management Meta SOP Class (Retired)", 
                                          SopClass.DetachedPatientManagementMetaSOPClassRetiredUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Detached Results Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.4</para>
        /// </summary>
        public static readonly String DetachedResultsManagementMetaSOPClassRetiredUid = "1.2.840.10008.3.1.2.5.4";

        /// <summary>SopClass for
        /// <para>Detached Results Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.4</para>
        /// </summary>
        public static readonly SopClass DetachedResultsManagementMetaSOPClassRetired =
                             new SopClass("Detached Results Management Meta SOP Class (Retired)", 
                                          SopClass.DetachedResultsManagementMetaSOPClassRetiredUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Detached Study Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.5</para>
        /// </summary>
        public static readonly String DetachedStudyManagementMetaSOPClassRetiredUid = "1.2.840.10008.3.1.2.5.5";

        /// <summary>SopClass for
        /// <para>Detached Study Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.5</para>
        /// </summary>
        public static readonly SopClass DetachedStudyManagementMetaSOPClassRetired =
                             new SopClass("Detached Study Management Meta SOP Class (Retired)", 
                                          SopClass.DetachedStudyManagementMetaSOPClassRetiredUid, 
                                          true);
        /// <summary>String UID for
        /// <para>General Purpose Worklist Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32</para>
        /// </summary>
        public static readonly String GeneralPurposeWorklistManagementMetaSOPClassUid = "1.2.840.10008.5.1.4.32";

        /// <summary>SopClass for
        /// <para>General Purpose Worklist Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32</para>
        /// </summary>
        public static readonly SopClass GeneralPurposeWorklistManagementMetaSOPClass =
                             new SopClass("General Purpose Worklist Management Meta SOP Class", 
                                          SopClass.GeneralPurposeWorklistManagementMetaSOPClassUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Pull Stored Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.32</para>
        /// </summary>
        public static readonly String PullStoredPrintManagementMetaSOPClassRetiredUid = "1.2.840.10008.5.1.1.32";

        /// <summary>SopClass for
        /// <para>Pull Stored Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.32</para>
        /// </summary>
        public static readonly SopClass PullStoredPrintManagementMetaSOPClassRetired =
                             new SopClass("Pull Stored Print Management Meta SOP Class (Retired)", 
                                          SopClass.PullStoredPrintManagementMetaSOPClassRetiredUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Referenced Color Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.18.1</para>
        /// </summary>
        public static readonly String ReferencedColorPrintManagementMetaSOPClassRetiredUid = "1.2.840.10008.5.1.1.18.1";

        /// <summary>SopClass for
        /// <para>Referenced Color Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.18.1</para>
        /// </summary>
        public static readonly SopClass ReferencedColorPrintManagementMetaSOPClassRetired =
                             new SopClass("Referenced Color Print Management Meta SOP Class (Retired)", 
                                          SopClass.ReferencedColorPrintManagementMetaSOPClassRetiredUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Referenced Grayscale Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.9.1</para>
        /// </summary>
        public static readonly String ReferencedGrayscalePrintManagementMetaSOPClassRetiredUid = "1.2.840.10008.5.1.1.9.1";

        /// <summary>SopClass for
        /// <para>Referenced Grayscale Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.9.1</para>
        /// </summary>
        public static readonly SopClass ReferencedGrayscalePrintManagementMetaSOPClassRetired =
                             new SopClass("Referenced Grayscale Print Management Meta SOP Class (Retired)", 
                                          SopClass.ReferencedGrayscalePrintManagementMetaSOPClassRetiredUid, 
                                          true);

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
        /// <summary> Property that returns a DicomUid that represents the SOP Class. </summary>
        public DicomUid DicomUid
        {
            get { return new DicomUid(_sopUid,_sopName,_bIsMeta ? UidType.MetaSOPClass : UidType.SOPClass); }
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
                _sopList.Add(SopClass.Sop12leadECGWaveformStorageUid, 
                             SopClass.Sop12leadECGWaveformStorage);

                _sopList.Add(SopClass.AmbulatoryECGWaveformStorageUid, 
                             SopClass.AmbulatoryECGWaveformStorage);

                _sopList.Add(SopClass.BasicAnnotationBoxSOPClassUid, 
                             SopClass.BasicAnnotationBoxSOPClass);

                _sopList.Add(SopClass.BasicColorImageBoxSOPClassUid, 
                             SopClass.BasicColorImageBoxSOPClass);

                _sopList.Add(SopClass.BasicFilmBoxSOPClassUid, 
                             SopClass.BasicFilmBoxSOPClass);

                _sopList.Add(SopClass.BasicFilmSessionSOPClassUid, 
                             SopClass.BasicFilmSessionSOPClass);

                _sopList.Add(SopClass.BasicGrayscaleImageBoxSOPClassUid, 
                             SopClass.BasicGrayscaleImageBoxSOPClass);

                _sopList.Add(SopClass.BasicPrintImageOverlayBoxSOPClassRetiredUid, 
                             SopClass.BasicPrintImageOverlayBoxSOPClassRetired);

                _sopList.Add(SopClass.BasicStudyContentNotificationSOPClassRetiredUid, 
                             SopClass.BasicStudyContentNotificationSOPClassRetired);

                _sopList.Add(SopClass.BasicTextSRUid, 
                             SopClass.BasicTextSR);

                _sopList.Add(SopClass.BasicVoiceAudioWaveformStorageUid, 
                             SopClass.BasicVoiceAudioWaveformStorage);

                _sopList.Add(SopClass.BlendingSoftcopyPresentationStateStorageSOPClassUid, 
                             SopClass.BlendingSoftcopyPresentationStateStorageSOPClass);

                _sopList.Add(SopClass.BreastImagingRelevantPatientInformationQueryUid, 
                             SopClass.BreastImagingRelevantPatientInformationQuery);

                _sopList.Add(SopClass.CardiacElectrophysiologyWaveformStorageUid, 
                             SopClass.CardiacElectrophysiologyWaveformStorage);

                _sopList.Add(SopClass.CardiacRelevantPatientInformationQueryUid, 
                             SopClass.CardiacRelevantPatientInformationQuery);

                _sopList.Add(SopClass.ChestCADSRUid, 
                             SopClass.ChestCADSR);

                _sopList.Add(SopClass.ColorSoftcopyPresentationStateStorageSOPClassUid, 
                             SopClass.ColorSoftcopyPresentationStateStorageSOPClass);

                _sopList.Add(SopClass.ComprehensiveSRUid, 
                             SopClass.ComprehensiveSR);

                _sopList.Add(SopClass.ComputedRadiographyImageStorageUid, 
                             SopClass.ComputedRadiographyImageStorage);

                _sopList.Add(SopClass.CTImageStorageUid, 
                             SopClass.CTImageStorage);

                _sopList.Add(SopClass.DeformableSpatialRegistrationStorageUid, 
                             SopClass.DeformableSpatialRegistrationStorage);

                _sopList.Add(SopClass.DetachedInterpretationManagementSOPClassRetiredUid, 
                             SopClass.DetachedInterpretationManagementSOPClassRetired);

                _sopList.Add(SopClass.DetachedPatientManagementSOPClassRetiredUid, 
                             SopClass.DetachedPatientManagementSOPClassRetired);

                _sopList.Add(SopClass.DetachedResultsManagementSOPClassRetiredUid, 
                             SopClass.DetachedResultsManagementSOPClassRetired);

                _sopList.Add(SopClass.DetachedStudyManagementSOPClassRetiredUid, 
                             SopClass.DetachedStudyManagementSOPClassRetired);

                _sopList.Add(SopClass.DetachedVisitManagementSOPClassRetiredUid, 
                             SopClass.DetachedVisitManagementSOPClassRetired);

                _sopList.Add(SopClass.DigitalIntraoralXRayImageStorageForPresentationUid, 
                             SopClass.DigitalIntraoralXRayImageStorageForPresentation);

                _sopList.Add(SopClass.DigitalIntraoralXRayImageStorageForProcessingUid, 
                             SopClass.DigitalIntraoralXRayImageStorageForProcessing);

                _sopList.Add(SopClass.DigitalMammographyXRayImageStorageForPresentationUid, 
                             SopClass.DigitalMammographyXRayImageStorageForPresentation);

                _sopList.Add(SopClass.DigitalMammographyXRayImageStorageForProcessingUid, 
                             SopClass.DigitalMammographyXRayImageStorageForProcessing);

                _sopList.Add(SopClass.DigitalXRayImageStorageForPresentationUid, 
                             SopClass.DigitalXRayImageStorageForPresentation);

                _sopList.Add(SopClass.DigitalXRayImageStorageForProcessingUid, 
                             SopClass.DigitalXRayImageStorageForProcessing);

                _sopList.Add(SopClass.EncapsulatedPDFStorageUid, 
                             SopClass.EncapsulatedPDFStorage);

                _sopList.Add(SopClass.EnhancedCTImageStorageUid, 
                             SopClass.EnhancedCTImageStorage);

                _sopList.Add(SopClass.EnhancedMRImageStorageUid, 
                             SopClass.EnhancedMRImageStorage);

                _sopList.Add(SopClass.EnhancedSRUid, 
                             SopClass.EnhancedSR);

                _sopList.Add(SopClass.EnhancedXAImageStorageUid, 
                             SopClass.EnhancedXAImageStorage);

                _sopList.Add(SopClass.EnhancedXRFImageStorageUid, 
                             SopClass.EnhancedXRFImageStorage);

                _sopList.Add(SopClass.GeneralECGWaveformStorageUid, 
                             SopClass.GeneralECGWaveformStorage);

                _sopList.Add(SopClass.GeneralPurposePerformedProcedureStepSOPClassUid, 
                             SopClass.GeneralPurposePerformedProcedureStepSOPClass);

                _sopList.Add(SopClass.GeneralPurposeScheduledProcedureStepSOPClassUid, 
                             SopClass.GeneralPurposeScheduledProcedureStepSOPClass);

                _sopList.Add(SopClass.GeneralPurposeWorklistInformationModelFINDUid, 
                             SopClass.GeneralPurposeWorklistInformationModelFIND);

                _sopList.Add(SopClass.GeneralRelevantPatientInformationQueryUid, 
                             SopClass.GeneralRelevantPatientInformationQuery);

                _sopList.Add(SopClass.GrayscaleSoftcopyPresentationStateStorageSOPClassUid, 
                             SopClass.GrayscaleSoftcopyPresentationStateStorageSOPClass);

                _sopList.Add(SopClass.HangingProtocolInformationModelFINDUid, 
                             SopClass.HangingProtocolInformationModelFIND);

                _sopList.Add(SopClass.HangingProtocolInformationModelMOVEUid, 
                             SopClass.HangingProtocolInformationModelMOVE);

                _sopList.Add(SopClass.HangingProtocolStorageUid, 
                             SopClass.HangingProtocolStorage);

                _sopList.Add(SopClass.HardcopyGrayscaleImageStorageSOPClassRetiredUid, 
                             SopClass.HardcopyGrayscaleImageStorageSOPClassRetired);

                _sopList.Add(SopClass.HardcopyColorImageStorageSOPClassRetiredUid, 
                             SopClass.HardcopyColorImageStorageSOPClassRetired);

                _sopList.Add(SopClass.HemodynamicWaveformStorageUid, 
                             SopClass.HemodynamicWaveformStorage);

                _sopList.Add(SopClass.ImageOverlayBoxSOPClassRetiredUid, 
                             SopClass.ImageOverlayBoxSOPClassRetired);

                _sopList.Add(SopClass.InstanceAvailabilityNotificationSOPClassUid, 
                             SopClass.InstanceAvailabilityNotificationSOPClass);

                _sopList.Add(SopClass.KeyObjectSelectionDocumentUid, 
                             SopClass.KeyObjectSelectionDocument);

                _sopList.Add(SopClass.MammographyCADSRUid, 
                             SopClass.MammographyCADSR);

                _sopList.Add(SopClass.MediaCreationManagementSOPClassUIDUid, 
                             SopClass.MediaCreationManagementSOPClassUID);

                _sopList.Add(SopClass.MediaStorageDirectoryStorageUid, 
                             SopClass.MediaStorageDirectoryStorage);

                _sopList.Add(SopClass.ModalityPerformedProcedureStepNotificationSOPClassUid, 
                             SopClass.ModalityPerformedProcedureStepNotificationSOPClass);

                _sopList.Add(SopClass.ModalityPerformedProcedureStepRetrieveSOPClassUid, 
                             SopClass.ModalityPerformedProcedureStepRetrieveSOPClass);

                _sopList.Add(SopClass.ModalityPerformedProcedureStepSOPClassUid, 
                             SopClass.ModalityPerformedProcedureStepSOPClass);

                _sopList.Add(SopClass.ModalityWorklistInformationModelFINDUid, 
                             SopClass.ModalityWorklistInformationModelFIND);

                _sopList.Add(SopClass.MRImageStorageUid, 
                             SopClass.MRImageStorage);

                _sopList.Add(SopClass.MRSpectroscopyStorageUid, 
                             SopClass.MRSpectroscopyStorage);

                _sopList.Add(SopClass.MultiframeGrayscaleByteSecondaryCaptureImageStorageUid, 
                             SopClass.MultiframeGrayscaleByteSecondaryCaptureImageStorage);

                _sopList.Add(SopClass.MultiframeGrayscaleWordSecondaryCaptureImageStorageUid, 
                             SopClass.MultiframeGrayscaleWordSecondaryCaptureImageStorage);

                _sopList.Add(SopClass.MultiframeSingleBitSecondaryCaptureImageStorageUid, 
                             SopClass.MultiframeSingleBitSecondaryCaptureImageStorage);

                _sopList.Add(SopClass.MultiframeTrueColorSecondaryCaptureImageStorageUid, 
                             SopClass.MultiframeTrueColorSecondaryCaptureImageStorage);

                _sopList.Add(SopClass.NuclearMedicineImageStorageRetiredUid, 
                             SopClass.NuclearMedicineImageStorageRetired);

                _sopList.Add(SopClass.NuclearMedicineImageStorageUid, 
                             SopClass.NuclearMedicineImageStorage);

                _sopList.Add(SopClass.OphthalmicPhotography16BitImageStorageUid, 
                             SopClass.OphthalmicPhotography16BitImageStorage);

                _sopList.Add(SopClass.OphthalmicPhotography8BitImageStorageUid, 
                             SopClass.OphthalmicPhotography8BitImageStorage);

                _sopList.Add(SopClass.PatientRootQueryRetrieveInformationModelFINDUid, 
                             SopClass.PatientRootQueryRetrieveInformationModelFIND);

                _sopList.Add(SopClass.PatientRootQueryRetrieveInformationModelGETUid, 
                             SopClass.PatientRootQueryRetrieveInformationModelGET);

                _sopList.Add(SopClass.PatientRootQueryRetrieveInformationModelMOVEUid, 
                             SopClass.PatientRootQueryRetrieveInformationModelMOVE);

                _sopList.Add(SopClass.PatientStudyOnlyQueryRetrieveInformationModelFINDRetiredUid, 
                             SopClass.PatientStudyOnlyQueryRetrieveInformationModelFINDRetired);

                _sopList.Add(SopClass.PatientStudyOnlyQueryRetrieveInformationModelGETRetiredUid, 
                             SopClass.PatientStudyOnlyQueryRetrieveInformationModelGETRetired);

                _sopList.Add(SopClass.PatientStudyOnlyQueryRetrieveInformationModelMOVERetiredUid, 
                             SopClass.PatientStudyOnlyQueryRetrieveInformationModelMOVERetired);

                _sopList.Add(SopClass.PositronEmissionTomographyImageStorageUid, 
                             SopClass.PositronEmissionTomographyImageStorage);

                _sopList.Add(SopClass.PresentationLUTSOPClassUid, 
                             SopClass.PresentationLUTSOPClass);

                _sopList.Add(SopClass.PrintJobSOPClassUid, 
                             SopClass.PrintJobSOPClass);

                _sopList.Add(SopClass.PrintQueueManagementSOPClassRetiredUid, 
                             SopClass.PrintQueueManagementSOPClassRetired);

                _sopList.Add(SopClass.PrinterConfigurationRetrievalSOPClassUid, 
                             SopClass.PrinterConfigurationRetrievalSOPClass);

                _sopList.Add(SopClass.PrinterSOPClassUid, 
                             SopClass.PrinterSOPClass);

                _sopList.Add(SopClass.ProceduralEventLoggingSOPClassUid, 
                             SopClass.ProceduralEventLoggingSOPClass);

                _sopList.Add(SopClass.ProcedureLogStorageUid, 
                             SopClass.ProcedureLogStorage);

                _sopList.Add(SopClass.PseudoColorSoftcopyPresentationStateStorageSOPClassUid, 
                             SopClass.PseudoColorSoftcopyPresentationStateStorageSOPClass);

                _sopList.Add(SopClass.PullPrintRequestSOPClassRetiredUid, 
                             SopClass.PullPrintRequestSOPClassRetired);

                _sopList.Add(SopClass.RawDataStorageUid, 
                             SopClass.RawDataStorage);

                _sopList.Add(SopClass.RealWorldValueMappingStorageUid, 
                             SopClass.RealWorldValueMappingStorage);

                _sopList.Add(SopClass.ReferencedImageBoxSOPClassRetiredUid, 
                             SopClass.ReferencedImageBoxSOPClassRetired);

                _sopList.Add(SopClass.RTBeamsTreatmentRecordStorageUid, 
                             SopClass.RTBeamsTreatmentRecordStorage);

                _sopList.Add(SopClass.RTBrachyTreatmentRecordStorageUid, 
                             SopClass.RTBrachyTreatmentRecordStorage);

                _sopList.Add(SopClass.RTDoseStorageUid, 
                             SopClass.RTDoseStorage);

                _sopList.Add(SopClass.RTImageStorageUid, 
                             SopClass.RTImageStorage);

                _sopList.Add(SopClass.RTIonBeamsTreatmentRecordStorageUid, 
                             SopClass.RTIonBeamsTreatmentRecordStorage);

                _sopList.Add(SopClass.RTIonPlanStorageUid, 
                             SopClass.RTIonPlanStorage);

                _sopList.Add(SopClass.RTPlanStorageUid, 
                             SopClass.RTPlanStorage);

                _sopList.Add(SopClass.RTStructureSetStorageUid, 
                             SopClass.RTStructureSetStorage);

                _sopList.Add(SopClass.RTTreatmentSummaryRecordStorageUid, 
                             SopClass.RTTreatmentSummaryRecordStorage);

                _sopList.Add(SopClass.SecondaryCaptureImageStorageUid, 
                             SopClass.SecondaryCaptureImageStorage);

                _sopList.Add(SopClass.SegmentationStorageUid, 
                             SopClass.SegmentationStorage);

                _sopList.Add(SopClass.SpatialFiducialsStorageUid, 
                             SopClass.SpatialFiducialsStorage);

                _sopList.Add(SopClass.SpatialRegistrationStorageUid, 
                             SopClass.SpatialRegistrationStorage);

                _sopList.Add(SopClass.StandaloneCurveStorageRetiredUid, 
                             SopClass.StandaloneCurveStorageRetired);

                _sopList.Add(SopClass.StandaloneModalityLUTStorageRetiredUid, 
                             SopClass.StandaloneModalityLUTStorageRetired);

                _sopList.Add(SopClass.StandaloneOverlayStorageRetiredUid, 
                             SopClass.StandaloneOverlayStorageRetired);

                _sopList.Add(SopClass.StandalonePETCurveStorageRetiredUid, 
                             SopClass.StandalonePETCurveStorageRetired);

                _sopList.Add(SopClass.StandaloneVOILUTStorageRetiredUid, 
                             SopClass.StandaloneVOILUTStorageRetired);

                _sopList.Add(SopClass.StereometricRelationshipStorageUid, 
                             SopClass.StereometricRelationshipStorage);

                _sopList.Add(SopClass.StorageCommitmentPullModelSOPClassRetiredUid, 
                             SopClass.StorageCommitmentPullModelSOPClassRetired);

                _sopList.Add(SopClass.StorageCommitmentPushModelSOPClassUid, 
                             SopClass.StorageCommitmentPushModelSOPClass);

                _sopList.Add(SopClass.StoredPrintStorageSOPClassRetiredUid, 
                             SopClass.StoredPrintStorageSOPClassRetired);

                _sopList.Add(SopClass.StudyComponentManagementSOPClassRetiredUid, 
                             SopClass.StudyComponentManagementSOPClassRetired);

                _sopList.Add(SopClass.StudyRootQueryRetrieveInformationModelFINDUid, 
                             SopClass.StudyRootQueryRetrieveInformationModelFIND);

                _sopList.Add(SopClass.StudyRootQueryRetrieveInformationModelGETUid, 
                             SopClass.StudyRootQueryRetrieveInformationModelGET);

                _sopList.Add(SopClass.StudyRootQueryRetrieveInformationModelMOVEUid, 
                             SopClass.StudyRootQueryRetrieveInformationModelMOVE);

                _sopList.Add(SopClass.UltrasoundImageStorageUid, 
                             SopClass.UltrasoundImageStorage);

                _sopList.Add(SopClass.UltrasoundImageStorageRetiredUid, 
                             SopClass.UltrasoundImageStorageRetired);

                _sopList.Add(SopClass.UltrasoundMultiframeImageStorageUid, 
                             SopClass.UltrasoundMultiframeImageStorage);

                _sopList.Add(SopClass.UltrasoundMultiframeImageStorageRetiredUid, 
                             SopClass.UltrasoundMultiframeImageStorageRetired);

                _sopList.Add(SopClass.VerificationSOPClassUid, 
                             SopClass.VerificationSOPClass);

                _sopList.Add(SopClass.VideoEndoscopicImageStorageUid, 
                             SopClass.VideoEndoscopicImageStorage);

                _sopList.Add(SopClass.VideoMicroscopicImageStorageUid, 
                             SopClass.VideoMicroscopicImageStorage);

                _sopList.Add(SopClass.VideoPhotographicImageStorageUid, 
                             SopClass.VideoPhotographicImageStorage);

                _sopList.Add(SopClass.VLEndoscopicImageStorageUid, 
                             SopClass.VLEndoscopicImageStorage);

                _sopList.Add(SopClass.VLMicroscopicImageStorageUid, 
                             SopClass.VLMicroscopicImageStorage);

                _sopList.Add(SopClass.VLPhotographicImageStorageUid, 
                             SopClass.VLPhotographicImageStorage);

                _sopList.Add(SopClass.VLSlideCoordinatesMicroscopicImageStorageUid, 
                             SopClass.VLSlideCoordinatesMicroscopicImageStorage);

                _sopList.Add(SopClass.VOILUTBoxSOPClassUid, 
                             SopClass.VOILUTBoxSOPClass);

                _sopList.Add(SopClass.XRayAngiographicBiPlaneImageStorageRetiredUid, 
                             SopClass.XRayAngiographicBiPlaneImageStorageRetired);

                _sopList.Add(SopClass.XRayAngiographicImageStorageUid, 
                             SopClass.XRayAngiographicImageStorage);

                _sopList.Add(SopClass.XRayRadiationDoseSRUid, 
                             SopClass.XRayRadiationDoseSR);

                _sopList.Add(SopClass.XRayRadiofluoroscopicImageStorageUid, 
                             SopClass.XRayRadiofluoroscopicImageStorage);

                _sopList.Add(SopClass.BasicColorPrintManagementMetaSOPClassUid, 
                             SopClass.BasicColorPrintManagementMetaSOPClass);

                _sopList.Add(SopClass.BasicGrayscalePrintManagementMetaSOPClassUid, 
                             SopClass.BasicGrayscalePrintManagementMetaSOPClass);

                _sopList.Add(SopClass.DetachedPatientManagementMetaSOPClassRetiredUid, 
                             SopClass.DetachedPatientManagementMetaSOPClassRetired);

                _sopList.Add(SopClass.DetachedResultsManagementMetaSOPClassRetiredUid, 
                             SopClass.DetachedResultsManagementMetaSOPClassRetired);

                _sopList.Add(SopClass.DetachedStudyManagementMetaSOPClassRetiredUid, 
                             SopClass.DetachedStudyManagementMetaSOPClassRetired);

                _sopList.Add(SopClass.GeneralPurposeWorklistManagementMetaSOPClassUid, 
                             SopClass.GeneralPurposeWorklistManagementMetaSOPClass);

                _sopList.Add(SopClass.PullStoredPrintManagementMetaSOPClassRetiredUid, 
                             SopClass.PullStoredPrintManagementMetaSOPClassRetired);

                _sopList.Add(SopClass.ReferencedColorPrintManagementMetaSOPClassRetiredUid, 
                             SopClass.ReferencedColorPrintManagementMetaSOPClassRetired);

                _sopList.Add(SopClass.ReferencedGrayscalePrintManagementMetaSOPClassRetiredUid, 
                             SopClass.ReferencedGrayscalePrintManagementMetaSOPClassRetired);

            }

            if (!_sopList.ContainsKey(uid))
                return null;

            return _sopList[uid];
        }
    }
}
