#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Dicom
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
        public static readonly String Sop12LeadEcgWaveformStorageUid = "1.2.840.10008.5.1.4.1.1.9.1.1";

        /// <summary>SopClass for
        /// <para>12-lead ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.1</para>
        /// </summary>
        public static readonly SopClass Sop12LeadEcgWaveformStorage =
                             new SopClass("12-lead ECG Waveform Storage", 
                                          SopClass.Sop12LeadEcgWaveformStorageUid, 
                                          false);

        /// <summary>
        /// <para>Ambulatory ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.3</para>
        /// </summary>
        public static readonly String AmbulatoryEcgWaveformStorageUid = "1.2.840.10008.5.1.4.1.1.9.1.3";

        /// <summary>SopClass for
        /// <para>Ambulatory ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.3</para>
        /// </summary>
        public static readonly SopClass AmbulatoryEcgWaveformStorage =
                             new SopClass("Ambulatory ECG Waveform Storage", 
                                          SopClass.AmbulatoryEcgWaveformStorageUid, 
                                          false);

        /// <summary>
        /// <para>Basic Annotation Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.15</para>
        /// </summary>
        public static readonly String BasicAnnotationBoxSopClassUid = "1.2.840.10008.5.1.1.15";

        /// <summary>SopClass for
        /// <para>Basic Annotation Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.15</para>
        /// </summary>
        public static readonly SopClass BasicAnnotationBoxSopClass =
                             new SopClass("Basic Annotation Box SOP Class", 
                                          SopClass.BasicAnnotationBoxSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Basic Color Image Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4.1</para>
        /// </summary>
        public static readonly String BasicColorImageBoxSopClassUid = "1.2.840.10008.5.1.1.4.1";

        /// <summary>SopClass for
        /// <para>Basic Color Image Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4.1</para>
        /// </summary>
        public static readonly SopClass BasicColorImageBoxSopClass =
                             new SopClass("Basic Color Image Box SOP Class", 
                                          SopClass.BasicColorImageBoxSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Basic Film Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.2</para>
        /// </summary>
        public static readonly String BasicFilmBoxSopClassUid = "1.2.840.10008.5.1.1.2";

        /// <summary>SopClass for
        /// <para>Basic Film Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.2</para>
        /// </summary>
        public static readonly SopClass BasicFilmBoxSopClass =
                             new SopClass("Basic Film Box SOP Class", 
                                          SopClass.BasicFilmBoxSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Basic Film Session SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.1</para>
        /// </summary>
        public static readonly String BasicFilmSessionSopClassUid = "1.2.840.10008.5.1.1.1";

        /// <summary>SopClass for
        /// <para>Basic Film Session SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.1</para>
        /// </summary>
        public static readonly SopClass BasicFilmSessionSopClass =
                             new SopClass("Basic Film Session SOP Class", 
                                          SopClass.BasicFilmSessionSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Basic Grayscale Image Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4</para>
        /// </summary>
        public static readonly String BasicGrayscaleImageBoxSopClassUid = "1.2.840.10008.5.1.1.4";

        /// <summary>SopClass for
        /// <para>Basic Grayscale Image Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4</para>
        /// </summary>
        public static readonly SopClass BasicGrayscaleImageBoxSopClass =
                             new SopClass("Basic Grayscale Image Box SOP Class", 
                                          SopClass.BasicGrayscaleImageBoxSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Basic Print Image Overlay Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.24.1</para>
        /// </summary>
        public static readonly String BasicPrintImageOverlayBoxSopClassRetiredUid = "1.2.840.10008.5.1.1.24.1";

        /// <summary>SopClass for
        /// <para>Basic Print Image Overlay Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.24.1</para>
        /// </summary>
        public static readonly SopClass BasicPrintImageOverlayBoxSopClassRetired =
                             new SopClass("Basic Print Image Overlay Box SOP Class (Retired)", 
                                          SopClass.BasicPrintImageOverlayBoxSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Basic Study Content Notification SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.9</para>
        /// </summary>
        public static readonly String BasicStudyContentNotificationSopClassRetiredUid = "1.2.840.10008.1.9";

        /// <summary>SopClass for
        /// <para>Basic Study Content Notification SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.9</para>
        /// </summary>
        public static readonly SopClass BasicStudyContentNotificationSopClassRetired =
                             new SopClass("Basic Study Content Notification SOP Class (Retired)", 
                                          SopClass.BasicStudyContentNotificationSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Basic Text SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.11</para>
        /// </summary>
        public static readonly String BasicTextSrUid = "1.2.840.10008.5.1.4.1.1.88.11";

        /// <summary>SopClass for
        /// <para>Basic Text SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.11</para>
        /// </summary>
        public static readonly SopClass BasicTextSr =
                             new SopClass("Basic Text SR", 
                                          SopClass.BasicTextSrUid, 
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
        public static readonly String BlendingSoftcopyPresentationStateStorageSopClassUid = "1.2.840.10008.5.1.4.1.1.11.4";

        /// <summary>SopClass for
        /// <para>Blending Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.4</para>
        /// </summary>
        public static readonly SopClass BlendingSoftcopyPresentationStateStorageSopClass =
                             new SopClass("Blending Softcopy Presentation State Storage SOP Class", 
                                          SopClass.BlendingSoftcopyPresentationStateStorageSopClassUid, 
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
        public static readonly String ChestCadSrUid = "1.2.840.10008.5.1.4.1.1.88.65";

        /// <summary>SopClass for
        /// <para>Chest CAD SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.65</para>
        /// </summary>
        public static readonly SopClass ChestCadSr =
                             new SopClass("Chest CAD SR", 
                                          SopClass.ChestCadSrUid, 
                                          false);

        /// <summary>
        /// <para>Color Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.2</para>
        /// </summary>
        public static readonly String ColorSoftcopyPresentationStateStorageSopClassUid = "1.2.840.10008.5.1.4.1.1.11.2";

        /// <summary>SopClass for
        /// <para>Color Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.2</para>
        /// </summary>
        public static readonly SopClass ColorSoftcopyPresentationStateStorageSopClass =
                             new SopClass("Color Softcopy Presentation State Storage SOP Class", 
                                          SopClass.ColorSoftcopyPresentationStateStorageSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Comprehensive SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.33</para>
        /// </summary>
        public static readonly String ComprehensiveSrUid = "1.2.840.10008.5.1.4.1.1.88.33";

        /// <summary>SopClass for
        /// <para>Comprehensive SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.33</para>
        /// </summary>
        public static readonly SopClass ComprehensiveSr =
                             new SopClass("Comprehensive SR", 
                                          SopClass.ComprehensiveSrUid, 
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
        public static readonly String CtImageStorageUid = "1.2.840.10008.5.1.4.1.1.2";

        /// <summary>SopClass for
        /// <para>CT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.2</para>
        /// </summary>
        public static readonly SopClass CtImageStorage =
                             new SopClass("CT Image Storage", 
                                          SopClass.CtImageStorageUid, 
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
        public static readonly String DetachedInterpretationManagementSopClassRetiredUid = "1.2.840.10008.3.1.2.6.1";

        /// <summary>SopClass for
        /// <para>Detached Interpretation Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.6.1</para>
        /// </summary>
        public static readonly SopClass DetachedInterpretationManagementSopClassRetired =
                             new SopClass("Detached Interpretation Management SOP Class (Retired)", 
                                          SopClass.DetachedInterpretationManagementSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Detached Patient Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.1.1</para>
        /// </summary>
        public static readonly String DetachedPatientManagementSopClassRetiredUid = "1.2.840.10008.3.1.2.1.1";

        /// <summary>SopClass for
        /// <para>Detached Patient Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.1.1</para>
        /// </summary>
        public static readonly SopClass DetachedPatientManagementSopClassRetired =
                             new SopClass("Detached Patient Management SOP Class (Retired)", 
                                          SopClass.DetachedPatientManagementSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Detached Results Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.1</para>
        /// </summary>
        public static readonly String DetachedResultsManagementSopClassRetiredUid = "1.2.840.10008.3.1.2.5.1";

        /// <summary>SopClass for
        /// <para>Detached Results Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.1</para>
        /// </summary>
        public static readonly SopClass DetachedResultsManagementSopClassRetired =
                             new SopClass("Detached Results Management SOP Class (Retired)", 
                                          SopClass.DetachedResultsManagementSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Detached Study Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.1</para>
        /// </summary>
        public static readonly String DetachedStudyManagementSopClassRetiredUid = "1.2.840.10008.3.1.2.3.1";

        /// <summary>SopClass for
        /// <para>Detached Study Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.1</para>
        /// </summary>
        public static readonly SopClass DetachedStudyManagementSopClassRetired =
                             new SopClass("Detached Study Management SOP Class (Retired)", 
                                          SopClass.DetachedStudyManagementSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Detached Visit Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.2.1</para>
        /// </summary>
        public static readonly String DetachedVisitManagementSopClassRetiredUid = "1.2.840.10008.3.1.2.2.1";

        /// <summary>SopClass for
        /// <para>Detached Visit Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.2.1</para>
        /// </summary>
        public static readonly SopClass DetachedVisitManagementSopClassRetired =
                             new SopClass("Detached Visit Management SOP Class (Retired)", 
                                          SopClass.DetachedVisitManagementSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Digital Intra-oral X-Ray Image Storage – For Presentation</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.3</para>
        /// </summary>
        public static readonly String DigitalIntraOralXRayImageStorageForPresentationUid = "1.2.840.10008.5.1.4.1.1.1.3";

        /// <summary>SopClass for
        /// <para>Digital Intra-oral X-Ray Image Storage – For Presentation</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.3</para>
        /// </summary>
        public static readonly SopClass DigitalIntraOralXRayImageStorageForPresentation =
                             new SopClass("Digital Intra-oral X-Ray Image Storage – For Presentation", 
                                          SopClass.DigitalIntraOralXRayImageStorageForPresentationUid, 
                                          false);

        /// <summary>
        /// <para>Digital Intra-oral X-Ray Image Storage – For Processing</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.3.1</para>
        /// </summary>
        public static readonly String DigitalIntraOralXRayImageStorageForProcessingUid = "1.2.840.10008.5.1.4.1.1.1.3.1";

        /// <summary>SopClass for
        /// <para>Digital Intra-oral X-Ray Image Storage – For Processing</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.1.3.1</para>
        /// </summary>
        public static readonly SopClass DigitalIntraOralXRayImageStorageForProcessing =
                             new SopClass("Digital Intra-oral X-Ray Image Storage – For Processing", 
                                          SopClass.DigitalIntraOralXRayImageStorageForProcessingUid, 
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
        public static readonly String EncapsulatedPdfStorageUid = "1.2.840.10008.5.1.4.1.1.104.1";

        /// <summary>SopClass for
        /// <para>Encapsulated PDF Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.104.1</para>
        /// </summary>
        public static readonly SopClass EncapsulatedPdfStorage =
                             new SopClass("Encapsulated PDF Storage", 
                                          SopClass.EncapsulatedPdfStorageUid, 
                                          false);

        /// <summary>
        /// <para>Enhanced CT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.2.1</para>
        /// </summary>
        public static readonly String EnhancedCtImageStorageUid = "1.2.840.10008.5.1.4.1.1.2.1";

        /// <summary>SopClass for
        /// <para>Enhanced CT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.2.1</para>
        /// </summary>
        public static readonly SopClass EnhancedCtImageStorage =
                             new SopClass("Enhanced CT Image Storage", 
                                          SopClass.EnhancedCtImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Enhanced MR Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4.1</para>
        /// </summary>
        public static readonly String EnhancedMrImageStorageUid = "1.2.840.10008.5.1.4.1.1.4.1";

        /// <summary>SopClass for
        /// <para>Enhanced MR Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4.1</para>
        /// </summary>
        public static readonly SopClass EnhancedMrImageStorage =
                             new SopClass("Enhanced MR Image Storage", 
                                          SopClass.EnhancedMrImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Enhanced SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.22</para>
        /// </summary>
        public static readonly String EnhancedSrUid = "1.2.840.10008.5.1.4.1.1.88.22";

        /// <summary>SopClass for
        /// <para>Enhanced SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.22</para>
        /// </summary>
        public static readonly SopClass EnhancedSr =
                             new SopClass("Enhanced SR", 
                                          SopClass.EnhancedSrUid, 
                                          false);

        /// <summary>
        /// <para>Enhanced XA Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.1.1</para>
        /// </summary>
        public static readonly String EnhancedXaImageStorageUid = "1.2.840.10008.5.1.4.1.1.12.1.1";

        /// <summary>SopClass for
        /// <para>Enhanced XA Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.1.1</para>
        /// </summary>
        public static readonly SopClass EnhancedXaImageStorage =
                             new SopClass("Enhanced XA Image Storage", 
                                          SopClass.EnhancedXaImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Enhanced XRF Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.2.1</para>
        /// </summary>
        public static readonly String EnhancedXrfImageStorageUid = "1.2.840.10008.5.1.4.1.1.12.2.1";

        /// <summary>SopClass for
        /// <para>Enhanced XRF Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.12.2.1</para>
        /// </summary>
        public static readonly SopClass EnhancedXrfImageStorage =
                             new SopClass("Enhanced XRF Image Storage", 
                                          SopClass.EnhancedXrfImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>General ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.2</para>
        /// </summary>
        public static readonly String GeneralEcgWaveformStorageUid = "1.2.840.10008.5.1.4.1.1.9.1.2";

        /// <summary>SopClass for
        /// <para>General ECG Waveform Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.9.1.2</para>
        /// </summary>
        public static readonly SopClass GeneralEcgWaveformStorage =
                             new SopClass("General ECG Waveform Storage", 
                                          SopClass.GeneralEcgWaveformStorageUid, 
                                          false);

        /// <summary>
        /// <para>General Purpose Performed Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.3</para>
        /// </summary>
        public static readonly String GeneralPurposePerformedProcedureStepSopClassUid = "1.2.840.10008.5.1.4.32.3";

        /// <summary>SopClass for
        /// <para>General Purpose Performed Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.3</para>
        /// </summary>
        public static readonly SopClass GeneralPurposePerformedProcedureStepSopClass =
                             new SopClass("General Purpose Performed Procedure Step SOP Class", 
                                          SopClass.GeneralPurposePerformedProcedureStepSopClassUid, 
                                          false);

        /// <summary>
        /// <para>General Purpose Scheduled Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.2</para>
        /// </summary>
        public static readonly String GeneralPurposeScheduledProcedureStepSopClassUid = "1.2.840.10008.5.1.4.32.2";

        /// <summary>SopClass for
        /// <para>General Purpose Scheduled Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.2</para>
        /// </summary>
        public static readonly SopClass GeneralPurposeScheduledProcedureStepSopClass =
                             new SopClass("General Purpose Scheduled Procedure Step SOP Class", 
                                          SopClass.GeneralPurposeScheduledProcedureStepSopClassUid, 
                                          false);

        /// <summary>
        /// <para>General Purpose Worklist Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.1</para>
        /// </summary>
        public static readonly String GeneralPurposeWorklistInformationModelFindUid = "1.2.840.10008.5.1.4.32.1";

        /// <summary>SopClass for
        /// <para>General Purpose Worklist Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32.1</para>
        /// </summary>
        public static readonly SopClass GeneralPurposeWorklistInformationModelFind =
                             new SopClass("General Purpose Worklist Information Model – FIND", 
                                          SopClass.GeneralPurposeWorklistInformationModelFindUid, 
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
        public static readonly String GrayscaleSoftcopyPresentationStateStorageSopClassUid = "1.2.840.10008.5.1.4.1.1.11.1";

        /// <summary>SopClass for
        /// <para>Grayscale Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.1</para>
        /// </summary>
        public static readonly SopClass GrayscaleSoftcopyPresentationStateStorageSopClass =
                             new SopClass("Grayscale Softcopy Presentation State Storage SOP Class", 
                                          SopClass.GrayscaleSoftcopyPresentationStateStorageSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Hanging Protocol Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.2</para>
        /// </summary>
        public static readonly String HangingProtocolInformationModelFindUid = "1.2.840.10008.5.1.4.38.2";

        /// <summary>SopClass for
        /// <para>Hanging Protocol Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.2</para>
        /// </summary>
        public static readonly SopClass HangingProtocolInformationModelFind =
                             new SopClass("Hanging Protocol Information Model – FIND", 
                                          SopClass.HangingProtocolInformationModelFindUid, 
                                          false);

        /// <summary>
        /// <para>Hanging Protocol Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.3</para>
        /// </summary>
        public static readonly String HangingProtocolInformationModelMoveUid = "1.2.840.10008.5.1.4.38.3";

        /// <summary>SopClass for
        /// <para>Hanging Protocol Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.38.3</para>
        /// </summary>
        public static readonly SopClass HangingProtocolInformationModelMove =
                             new SopClass("Hanging Protocol Information Model – MOVE", 
                                          SopClass.HangingProtocolInformationModelMoveUid, 
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
        public static readonly String HardcopyGrayscaleImageStorageSopClassRetiredUid = "1.2.840.10008.5.1.1.29";

        /// <summary>SopClass for
        /// <para>Hardcopy  Grayscale Image Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.29</para>
        /// </summary>
        public static readonly SopClass HardcopyGrayscaleImageStorageSopClassRetired =
                             new SopClass("Hardcopy  Grayscale Image Storage SOP Class (Retired)", 
                                          SopClass.HardcopyGrayscaleImageStorageSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Hardcopy Color Image Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.30</para>
        /// </summary>
        public static readonly String HardcopyColorImageStorageSopClassRetiredUid = "1.2.840.10008.5.1.1.30";

        /// <summary>SopClass for
        /// <para>Hardcopy Color Image Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.30</para>
        /// </summary>
        public static readonly SopClass HardcopyColorImageStorageSopClassRetired =
                             new SopClass("Hardcopy Color Image Storage SOP Class (Retired)", 
                                          SopClass.HardcopyColorImageStorageSopClassRetiredUid, 
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
        public static readonly String ImageOverlayBoxSopClassRetiredUid = "1.2.840.10008.5.1.1.24";

        /// <summary>SopClass for
        /// <para>Image Overlay Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.24</para>
        /// </summary>
        public static readonly SopClass ImageOverlayBoxSopClassRetired =
                             new SopClass("Image Overlay Box SOP Class (Retired)", 
                                          SopClass.ImageOverlayBoxSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Instance Availability Notification SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.33</para>
        /// </summary>
        public static readonly String InstanceAvailabilityNotificationSopClassUid = "1.2.840.10008.5.1.4.33";

        /// <summary>SopClass for
        /// <para>Instance Availability Notification SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.33</para>
        /// </summary>
        public static readonly SopClass InstanceAvailabilityNotificationSopClass =
                             new SopClass("Instance Availability Notification SOP Class", 
                                          SopClass.InstanceAvailabilityNotificationSopClassUid, 
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
        public static readonly String MammographyCadSrUid = "1.2.840.10008.5.1.4.1.1.88.50";

        /// <summary>SopClass for
        /// <para>Mammography CAD SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.50</para>
        /// </summary>
        public static readonly SopClass MammographyCadSr =
                             new SopClass("Mammography CAD SR", 
                                          SopClass.MammographyCadSrUid, 
                                          false);

        /// <summary>
        /// <para>Media Creation Management SOP Class UID</para>
        /// <para>UID: 1.2.840.10008.5.1.1.33</para>
        /// </summary>
        public static readonly String MediaCreationManagementSopClassUidUid = "1.2.840.10008.5.1.1.33";

        /// <summary>SopClass for
        /// <para>Media Creation Management SOP Class UID</para>
        /// <para>UID: 1.2.840.10008.5.1.1.33</para>
        /// </summary>
        public static readonly SopClass MediaCreationManagementSopClassUid =
                             new SopClass("Media Creation Management SOP Class UID", 
                                          SopClass.MediaCreationManagementSopClassUidUid, 
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
        public static readonly String ModalityPerformedProcedureStepNotificationSopClassUid = "1.2.840.10008.3.1.2.3.5";

        /// <summary>SopClass for
        /// <para>Modality Performed Procedure Step Notification SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.5</para>
        /// </summary>
        public static readonly SopClass ModalityPerformedProcedureStepNotificationSopClass =
                             new SopClass("Modality Performed Procedure Step Notification SOP Class", 
                                          SopClass.ModalityPerformedProcedureStepNotificationSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Modality Performed Procedure Step Retrieve SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.4</para>
        /// </summary>
        public static readonly String ModalityPerformedProcedureStepRetrieveSopClassUid = "1.2.840.10008.3.1.2.3.4";

        /// <summary>SopClass for
        /// <para>Modality Performed Procedure Step Retrieve SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.4</para>
        /// </summary>
        public static readonly SopClass ModalityPerformedProcedureStepRetrieveSopClass =
                             new SopClass("Modality Performed Procedure Step Retrieve SOP Class", 
                                          SopClass.ModalityPerformedProcedureStepRetrieveSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Modality Performed Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.3</para>
        /// </summary>
        public static readonly String ModalityPerformedProcedureStepSopClassUid = "1.2.840.10008.3.1.2.3.3";

        /// <summary>SopClass for
        /// <para>Modality Performed Procedure Step SOP Class</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.3</para>
        /// </summary>
        public static readonly SopClass ModalityPerformedProcedureStepSopClass =
                             new SopClass("Modality Performed Procedure Step SOP Class", 
                                          SopClass.ModalityPerformedProcedureStepSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Modality Worklist Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.31</para>
        /// </summary>
        public static readonly String ModalityWorklistInformationModelFindUid = "1.2.840.10008.5.1.4.31";

        /// <summary>SopClass for
        /// <para>Modality Worklist Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.31</para>
        /// </summary>
        public static readonly SopClass ModalityWorklistInformationModelFind =
                             new SopClass("Modality Worklist Information Model – FIND", 
                                          SopClass.ModalityWorklistInformationModelFindUid, 
                                          false);

        /// <summary>
        /// <para>MR Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4</para>
        /// </summary>
        public static readonly String MrImageStorageUid = "1.2.840.10008.5.1.4.1.1.4";

        /// <summary>SopClass for
        /// <para>MR Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4</para>
        /// </summary>
        public static readonly SopClass MrImageStorage =
                             new SopClass("MR Image Storage", 
                                          SopClass.MrImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>MR Spectroscopy Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4.2</para>
        /// </summary>
        public static readonly String MrSpectroscopyStorageUid = "1.2.840.10008.5.1.4.1.1.4.2";

        /// <summary>SopClass for
        /// <para>MR Spectroscopy Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.4.2</para>
        /// </summary>
        public static readonly SopClass MrSpectroscopyStorage =
                             new SopClass("MR Spectroscopy Storage", 
                                          SopClass.MrSpectroscopyStorageUid, 
                                          false);

        /// <summary>
        /// <para>Multi-frame Grayscale Byte Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.2</para>
        /// </summary>
        public static readonly String MultiFrameGrayscaleByteSecondaryCaptureImageStorageUid = "1.2.840.10008.5.1.4.1.1.7.2";

        /// <summary>SopClass for
        /// <para>Multi-frame Grayscale Byte Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.2</para>
        /// </summary>
        public static readonly SopClass MultiFrameGrayscaleByteSecondaryCaptureImageStorage =
                             new SopClass("Multi-frame Grayscale Byte Secondary Capture Image Storage", 
                                          SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Multi-frame Grayscale Word Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.3</para>
        /// </summary>
        public static readonly String MultiFrameGrayscaleWordSecondaryCaptureImageStorageUid = "1.2.840.10008.5.1.4.1.1.7.3";

        /// <summary>SopClass for
        /// <para>Multi-frame Grayscale Word Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.3</para>
        /// </summary>
        public static readonly SopClass MultiFrameGrayscaleWordSecondaryCaptureImageStorage =
                             new SopClass("Multi-frame Grayscale Word Secondary Capture Image Storage", 
                                          SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Multi-frame Single Bit Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.1</para>
        /// </summary>
        public static readonly String MultiFrameSingleBitSecondaryCaptureImageStorageUid = "1.2.840.10008.5.1.4.1.1.7.1";

        /// <summary>SopClass for
        /// <para>Multi-frame Single Bit Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.1</para>
        /// </summary>
        public static readonly SopClass MultiFrameSingleBitSecondaryCaptureImageStorage =
                             new SopClass("Multi-frame Single Bit Secondary Capture Image Storage", 
                                          SopClass.MultiFrameSingleBitSecondaryCaptureImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Multi-frame True Color Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.4</para>
        /// </summary>
        public static readonly String MultiFrameTrueColorSecondaryCaptureImageStorageUid = "1.2.840.10008.5.1.4.1.1.7.4";

        /// <summary>SopClass for
        /// <para>Multi-frame True Color Secondary Capture Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.7.4</para>
        /// </summary>
        public static readonly SopClass MultiFrameTrueColorSecondaryCaptureImageStorage =
                             new SopClass("Multi-frame True Color Secondary Capture Image Storage", 
                                          SopClass.MultiFrameTrueColorSecondaryCaptureImageStorageUid, 
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
        public static readonly String PatientRootQueryRetrieveInformationModelFindUid = "1.2.840.10008.5.1.4.1.2.1.1";

        /// <summary>SopClass for
        /// <para>Patient Root Query/Retrieve Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.1</para>
        /// </summary>
        public static readonly SopClass PatientRootQueryRetrieveInformationModelFind =
                             new SopClass("Patient Root Query/Retrieve Information Model – FIND", 
                                          SopClass.PatientRootQueryRetrieveInformationModelFindUid, 
                                          false);

        /// <summary>
        /// <para>Patient Root Query/Retrieve Information Model – GET</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.3</para>
        /// </summary>
        public static readonly String PatientRootQueryRetrieveInformationModelGetUid = "1.2.840.10008.5.1.4.1.2.1.3";

        /// <summary>SopClass for
        /// <para>Patient Root Query/Retrieve Information Model – GET</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.3</para>
        /// </summary>
        public static readonly SopClass PatientRootQueryRetrieveInformationModelGet =
                             new SopClass("Patient Root Query/Retrieve Information Model – GET", 
                                          SopClass.PatientRootQueryRetrieveInformationModelGetUid, 
                                          false);

        /// <summary>
        /// <para>Patient Root Query/Retrieve Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.2</para>
        /// </summary>
        public static readonly String PatientRootQueryRetrieveInformationModelMoveUid = "1.2.840.10008.5.1.4.1.2.1.2";

        /// <summary>SopClass for
        /// <para>Patient Root Query/Retrieve Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.1.2</para>
        /// </summary>
        public static readonly SopClass PatientRootQueryRetrieveInformationModelMove =
                             new SopClass("Patient Root Query/Retrieve Information Model – MOVE", 
                                          SopClass.PatientRootQueryRetrieveInformationModelMoveUid, 
                                          false);

        /// <summary>
        /// <para>Patient/Study Only Query/Retrieve Information Model  - FIND (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.1</para>
        /// </summary>
        public static readonly String PatientStudyOnlyQueryRetrieveInformationModelFindRetiredUid = "1.2.840.10008.5.1.4.1.2.3.1";

        /// <summary>SopClass for
        /// <para>Patient/Study Only Query/Retrieve Information Model  - FIND (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.1</para>
        /// </summary>
        public static readonly SopClass PatientStudyOnlyQueryRetrieveInformationModelFindRetired =
                             new SopClass("Patient/Study Only Query/Retrieve Information Model  - FIND (Retired)", 
                                          SopClass.PatientStudyOnlyQueryRetrieveInformationModelFindRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Patient/Study Only Query/Retrieve Information Model  - GET (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.3</para>
        /// </summary>
        public static readonly String PatientStudyOnlyQueryRetrieveInformationModelGetRetiredUid = "1.2.840.10008.5.1.4.1.2.3.3";

        /// <summary>SopClass for
        /// <para>Patient/Study Only Query/Retrieve Information Model  - GET (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.3</para>
        /// </summary>
        public static readonly SopClass PatientStudyOnlyQueryRetrieveInformationModelGetRetired =
                             new SopClass("Patient/Study Only Query/Retrieve Information Model  - GET (Retired)", 
                                          SopClass.PatientStudyOnlyQueryRetrieveInformationModelGetRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Patient/Study Only Query/Retrieve Information Model  - MOVE (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.2</para>
        /// </summary>
        public static readonly String PatientStudyOnlyQueryRetrieveInformationModelMoveRetiredUid = "1.2.840.10008.5.1.4.1.2.3.2";

        /// <summary>SopClass for
        /// <para>Patient/Study Only Query/Retrieve Information Model  - MOVE (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.3.2</para>
        /// </summary>
        public static readonly SopClass PatientStudyOnlyQueryRetrieveInformationModelMoveRetired =
                             new SopClass("Patient/Study Only Query/Retrieve Information Model  - MOVE (Retired)", 
                                          SopClass.PatientStudyOnlyQueryRetrieveInformationModelMoveRetiredUid, 
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
        public static readonly String PresentationLutSopClassUid = "1.2.840.10008.5.1.1.23";

        /// <summary>SopClass for
        /// <para>Presentation LUT SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.23</para>
        /// </summary>
        public static readonly SopClass PresentationLutSopClass =
                             new SopClass("Presentation LUT SOP Class", 
                                          SopClass.PresentationLutSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Print Job SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.14</para>
        /// </summary>
        public static readonly String PrintJobSopClassUid = "1.2.840.10008.5.1.1.14";

        /// <summary>SopClass for
        /// <para>Print Job SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.14</para>
        /// </summary>
        public static readonly SopClass PrintJobSopClass =
                             new SopClass("Print Job SOP Class", 
                                          SopClass.PrintJobSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Print Queue Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.26</para>
        /// </summary>
        public static readonly String PrintQueueManagementSopClassRetiredUid = "1.2.840.10008.5.1.1.26";

        /// <summary>SopClass for
        /// <para>Print Queue Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.26</para>
        /// </summary>
        public static readonly SopClass PrintQueueManagementSopClassRetired =
                             new SopClass("Print Queue Management SOP Class (Retired)", 
                                          SopClass.PrintQueueManagementSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Printer Configuration Retrieval SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.16.376</para>
        /// </summary>
        public static readonly String PrinterConfigurationRetrievalSopClassUid = "1.2.840.10008.5.1.1.16.376";

        /// <summary>SopClass for
        /// <para>Printer Configuration Retrieval SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.16.376</para>
        /// </summary>
        public static readonly SopClass PrinterConfigurationRetrievalSopClass =
                             new SopClass("Printer Configuration Retrieval SOP Class", 
                                          SopClass.PrinterConfigurationRetrievalSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Printer SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.16</para>
        /// </summary>
        public static readonly String PrinterSopClassUid = "1.2.840.10008.5.1.1.16";

        /// <summary>SopClass for
        /// <para>Printer SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.16</para>
        /// </summary>
        public static readonly SopClass PrinterSopClass =
                             new SopClass("Printer SOP Class", 
                                          SopClass.PrinterSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Procedural Event Logging SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.40</para>
        /// </summary>
        public static readonly String ProceduralEventLoggingSopClassUid = "1.2.840.10008.1.40";

        /// <summary>SopClass for
        /// <para>Procedural Event Logging SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.40</para>
        /// </summary>
        public static readonly SopClass ProceduralEventLoggingSopClass =
                             new SopClass("Procedural Event Logging SOP Class", 
                                          SopClass.ProceduralEventLoggingSopClassUid, 
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
        public static readonly String PseudoColorSoftcopyPresentationStateStorageSopClassUid = "1.2.840.10008.5.1.4.1.1.11.3";

        /// <summary>SopClass for
        /// <para>Pseudo-Color Softcopy Presentation State Storage SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11.3</para>
        /// </summary>
        public static readonly SopClass PseudoColorSoftcopyPresentationStateStorageSopClass =
                             new SopClass("Pseudo-Color Softcopy Presentation State Storage SOP Class", 
                                          SopClass.PseudoColorSoftcopyPresentationStateStorageSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Pull Print Request SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.31</para>
        /// </summary>
        public static readonly String PullPrintRequestSopClassRetiredUid = "1.2.840.10008.5.1.1.31";

        /// <summary>SopClass for
        /// <para>Pull Print Request SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.31</para>
        /// </summary>
        public static readonly SopClass PullPrintRequestSopClassRetired =
                             new SopClass("Pull Print Request SOP Class (Retired)", 
                                          SopClass.PullPrintRequestSopClassRetiredUid, 
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
        public static readonly String ReferencedImageBoxSopClassRetiredUid = "1.2.840.10008.5.1.1.4.2";

        /// <summary>SopClass for
        /// <para>Referenced Image Box SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.4.2</para>
        /// </summary>
        public static readonly SopClass ReferencedImageBoxSopClassRetired =
                             new SopClass("Referenced Image Box SOP Class (Retired)", 
                                          SopClass.ReferencedImageBoxSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>RT Beams Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.4</para>
        /// </summary>
        public static readonly String RtBeamsTreatmentRecordStorageUid = "1.2.840.10008.5.1.4.1.1.481.4";

        /// <summary>SopClass for
        /// <para>RT Beams Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.4</para>
        /// </summary>
        public static readonly SopClass RtBeamsTreatmentRecordStorage =
                             new SopClass("RT Beams Treatment Record Storage", 
                                          SopClass.RtBeamsTreatmentRecordStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Brachy Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.6</para>
        /// </summary>
        public static readonly String RtBrachyTreatmentRecordStorageUid = "1.2.840.10008.5.1.4.1.1.481.6";

        /// <summary>SopClass for
        /// <para>RT Brachy Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.6</para>
        /// </summary>
        public static readonly SopClass RtBrachyTreatmentRecordStorage =
                             new SopClass("RT Brachy Treatment Record Storage", 
                                          SopClass.RtBrachyTreatmentRecordStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Dose Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.2</para>
        /// </summary>
        public static readonly String RtDoseStorageUid = "1.2.840.10008.5.1.4.1.1.481.2";

        /// <summary>SopClass for
        /// <para>RT Dose Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.2</para>
        /// </summary>
        public static readonly SopClass RtDoseStorage =
                             new SopClass("RT Dose Storage", 
                                          SopClass.RtDoseStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.1</para>
        /// </summary>
        public static readonly String RtImageStorageUid = "1.2.840.10008.5.1.4.1.1.481.1";

        /// <summary>SopClass for
        /// <para>RT Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.1</para>
        /// </summary>
        public static readonly SopClass RtImageStorage =
                             new SopClass("RT Image Storage", 
                                          SopClass.RtImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Ion Beams Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.9</para>
        /// </summary>
        public static readonly String RtIonBeamsTreatmentRecordStorageUid = "1.2.840.10008.5.1.4.1.1.481.9";

        /// <summary>SopClass for
        /// <para>RT Ion Beams Treatment Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.9</para>
        /// </summary>
        public static readonly SopClass RtIonBeamsTreatmentRecordStorage =
                             new SopClass("RT Ion Beams Treatment Record Storage", 
                                          SopClass.RtIonBeamsTreatmentRecordStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Ion Plan Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.8</para>
        /// </summary>
        public static readonly String RtIonPlanStorageUid = "1.2.840.10008.5.1.4.1.1.481.8";

        /// <summary>SopClass for
        /// <para>RT Ion Plan Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.8</para>
        /// </summary>
        public static readonly SopClass RtIonPlanStorage =
                             new SopClass("RT Ion Plan Storage", 
                                          SopClass.RtIonPlanStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Plan Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.5</para>
        /// </summary>
        public static readonly String RtPlanStorageUid = "1.2.840.10008.5.1.4.1.1.481.5";

        /// <summary>SopClass for
        /// <para>RT Plan Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.5</para>
        /// </summary>
        public static readonly SopClass RtPlanStorage =
                             new SopClass("RT Plan Storage", 
                                          SopClass.RtPlanStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Structure Set Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.3</para>
        /// </summary>
        public static readonly String RtStructureSetStorageUid = "1.2.840.10008.5.1.4.1.1.481.3";

        /// <summary>SopClass for
        /// <para>RT Structure Set Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.3</para>
        /// </summary>
        public static readonly SopClass RtStructureSetStorage =
                             new SopClass("RT Structure Set Storage", 
                                          SopClass.RtStructureSetStorageUid, 
                                          false);

        /// <summary>
        /// <para>RT Treatment Summary Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.7</para>
        /// </summary>
        public static readonly String RtTreatmentSummaryRecordStorageUid = "1.2.840.10008.5.1.4.1.1.481.7";

        /// <summary>SopClass for
        /// <para>RT Treatment Summary Record Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.481.7</para>
        /// </summary>
        public static readonly SopClass RtTreatmentSummaryRecordStorage =
                             new SopClass("RT Treatment Summary Record Storage", 
                                          SopClass.RtTreatmentSummaryRecordStorageUid, 
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
        public static readonly String StandaloneModalityLutStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.10";

        /// <summary>SopClass for
        /// <para>Standalone Modality LUT Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.10</para>
        /// </summary>
        public static readonly SopClass StandaloneModalityLutStorageRetired =
                             new SopClass("Standalone Modality LUT Storage (Retired)", 
                                          SopClass.StandaloneModalityLutStorageRetiredUid, 
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
        public static readonly String StandalonePetCurveStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.129";

        /// <summary>SopClass for
        /// <para>Standalone PET Curve Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.129</para>
        /// </summary>
        public static readonly SopClass StandalonePetCurveStorageRetired =
                             new SopClass("Standalone PET Curve Storage (Retired)", 
                                          SopClass.StandalonePetCurveStorageRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Standalone VOI LUT Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11</para>
        /// </summary>
        public static readonly String StandaloneVoiLutStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.11";

        /// <summary>SopClass for
        /// <para>Standalone VOI LUT Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.11</para>
        /// </summary>
        public static readonly SopClass StandaloneVoiLutStorageRetired =
                             new SopClass("Standalone VOI LUT Storage (Retired)", 
                                          SopClass.StandaloneVoiLutStorageRetiredUid, 
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
        public static readonly String StorageCommitmentPullModelSopClassRetiredUid = "1.2.840.10008.1.20.2";

        /// <summary>SopClass for
        /// <para>Storage Commitment Pull Model SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.1.20.2</para>
        /// </summary>
        public static readonly SopClass StorageCommitmentPullModelSopClassRetired =
                             new SopClass("Storage Commitment Pull Model SOP Class (Retired)", 
                                          SopClass.StorageCommitmentPullModelSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Storage Commitment Push Model SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.20.1</para>
        /// </summary>
        public static readonly String StorageCommitmentPushModelSopClassUid = "1.2.840.10008.1.20.1";

        /// <summary>SopClass for
        /// <para>Storage Commitment Push Model SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.20.1</para>
        /// </summary>
        public static readonly SopClass StorageCommitmentPushModelSopClass =
                             new SopClass("Storage Commitment Push Model SOP Class", 
                                          SopClass.StorageCommitmentPushModelSopClassUid, 
                                          false);

        /// <summary>
        /// <para>Stored Print Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.27</para>
        /// </summary>
        public static readonly String StoredPrintStorageSopClassRetiredUid = "1.2.840.10008.5.1.1.27";

        /// <summary>SopClass for
        /// <para>Stored Print Storage SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.27</para>
        /// </summary>
        public static readonly SopClass StoredPrintStorageSopClassRetired =
                             new SopClass("Stored Print Storage SOP Class (Retired)", 
                                          SopClass.StoredPrintStorageSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Study Component Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.2</para>
        /// </summary>
        public static readonly String StudyComponentManagementSopClassRetiredUid = "1.2.840.10008.3.1.2.3.2";

        /// <summary>SopClass for
        /// <para>Study Component Management SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.3.2</para>
        /// </summary>
        public static readonly SopClass StudyComponentManagementSopClassRetired =
                             new SopClass("Study Component Management SOP Class (Retired)", 
                                          SopClass.StudyComponentManagementSopClassRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Study Root Query/Retrieve Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.1</para>
        /// </summary>
        public static readonly String StudyRootQueryRetrieveInformationModelFindUid = "1.2.840.10008.5.1.4.1.2.2.1";

        /// <summary>SopClass for
        /// <para>Study Root Query/Retrieve Information Model – FIND</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.1</para>
        /// </summary>
        public static readonly SopClass StudyRootQueryRetrieveInformationModelFind =
                             new SopClass("Study Root Query/Retrieve Information Model – FIND", 
                                          SopClass.StudyRootQueryRetrieveInformationModelFindUid, 
                                          false);

        /// <summary>
        /// <para>Study Root Query/Retrieve Information Model – GET</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.3</para>
        /// </summary>
        public static readonly String StudyRootQueryRetrieveInformationModelGetUid = "1.2.840.10008.5.1.4.1.2.2.3";

        /// <summary>SopClass for
        /// <para>Study Root Query/Retrieve Information Model – GET</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.3</para>
        /// </summary>
        public static readonly SopClass StudyRootQueryRetrieveInformationModelGet =
                             new SopClass("Study Root Query/Retrieve Information Model – GET", 
                                          SopClass.StudyRootQueryRetrieveInformationModelGetUid, 
                                          false);

        /// <summary>
        /// <para>Study Root Query/Retrieve Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.2</para>
        /// </summary>
        public static readonly String StudyRootQueryRetrieveInformationModelMoveUid = "1.2.840.10008.5.1.4.1.2.2.2";

        /// <summary>SopClass for
        /// <para>Study Root Query/Retrieve Information Model – MOVE</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.2.2.2</para>
        /// </summary>
        public static readonly SopClass StudyRootQueryRetrieveInformationModelMove =
                             new SopClass("Study Root Query/Retrieve Information Model – MOVE", 
                                          SopClass.StudyRootQueryRetrieveInformationModelMoveUid, 
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
        public static readonly String UltrasoundMultiFrameImageStorageUid = "1.2.840.10008.5.1.4.1.1.3.1";

        /// <summary>SopClass for
        /// <para>Ultrasound Multi-frame Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.3.1</para>
        /// </summary>
        public static readonly SopClass UltrasoundMultiFrameImageStorage =
                             new SopClass("Ultrasound Multi-frame Image Storage", 
                                          SopClass.UltrasoundMultiFrameImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>Ultrasound Multi-frame Image Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.3</para>
        /// </summary>
        public static readonly String UltrasoundMultiFrameImageStorageRetiredUid = "1.2.840.10008.5.1.4.1.1.3";

        /// <summary>SopClass for
        /// <para>Ultrasound Multi-frame Image Storage (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.3</para>
        /// </summary>
        public static readonly SopClass UltrasoundMultiFrameImageStorageRetired =
                             new SopClass("Ultrasound Multi-frame Image Storage (Retired)", 
                                          SopClass.UltrasoundMultiFrameImageStorageRetiredUid, 
                                          false);

        /// <summary>
        /// <para>Verification SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.1</para>
        /// </summary>
        public static readonly String VerificationSopClassUid = "1.2.840.10008.1.1";

        /// <summary>SopClass for
        /// <para>Verification SOP Class</para>
        /// <para>UID: 1.2.840.10008.1.1</para>
        /// </summary>
        public static readonly SopClass VerificationSopClass =
                             new SopClass("Verification SOP Class", 
                                          SopClass.VerificationSopClassUid, 
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
        public static readonly String VlEndoscopicImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.1";

        /// <summary>SopClass for
        /// <para>VL Endoscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.1</para>
        /// </summary>
        public static readonly SopClass VlEndoscopicImageStorage =
                             new SopClass("VL Endoscopic Image Storage", 
                                          SopClass.VlEndoscopicImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>VL Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.2</para>
        /// </summary>
        public static readonly String VlMicroscopicImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.2";

        /// <summary>SopClass for
        /// <para>VL Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.2</para>
        /// </summary>
        public static readonly SopClass VlMicroscopicImageStorage =
                             new SopClass("VL Microscopic Image Storage", 
                                          SopClass.VlMicroscopicImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>VL Photographic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.4</para>
        /// </summary>
        public static readonly String VlPhotographicImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.4";

        /// <summary>SopClass for
        /// <para>VL Photographic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.4</para>
        /// </summary>
        public static readonly SopClass VlPhotographicImageStorage =
                             new SopClass("VL Photographic Image Storage", 
                                          SopClass.VlPhotographicImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>VL Slide-Coordinates Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.3</para>
        /// </summary>
        public static readonly String VlSlideCoordinatesMicroscopicImageStorageUid = "1.2.840.10008.5.1.4.1.1.77.1.3";

        /// <summary>SopClass for
        /// <para>VL Slide-Coordinates Microscopic Image Storage</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.77.1.3</para>
        /// </summary>
        public static readonly SopClass VlSlideCoordinatesMicroscopicImageStorage =
                             new SopClass("VL Slide-Coordinates Microscopic Image Storage", 
                                          SopClass.VlSlideCoordinatesMicroscopicImageStorageUid, 
                                          false);

        /// <summary>
        /// <para>VOI LUT Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.22</para>
        /// </summary>
        public static readonly String VoiLutBoxSopClassUid = "1.2.840.10008.5.1.1.22";

        /// <summary>SopClass for
        /// <para>VOI LUT Box SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.22</para>
        /// </summary>
        public static readonly SopClass VoiLutBoxSopClass =
                             new SopClass("VOI LUT Box SOP Class", 
                                          SopClass.VoiLutBoxSopClassUid, 
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
        public static readonly String XRayRadiationDoseSrUid = "1.2.840.10008.5.1.4.1.1.88.67";

        /// <summary>SopClass for
        /// <para>X-Ray Radiation Dose SR</para>
        /// <para>UID: 1.2.840.10008.5.1.4.1.1.88.67</para>
        /// </summary>
        public static readonly SopClass XRayRadiationDoseSr =
                             new SopClass("X-Ray Radiation Dose SR", 
                                          SopClass.XRayRadiationDoseSrUid, 
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
        public static readonly String BasicColorPrintManagementMetaSopClassUid = "1.2.840.10008.5.1.1.18";

        /// <summary>SopClass for
        /// <para>Basic Color Print Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.18</para>
        /// </summary>
        public static readonly SopClass BasicColorPrintManagementMetaSopClass =
                             new SopClass("Basic Color Print Management Meta SOP Class", 
                                          SopClass.BasicColorPrintManagementMetaSopClassUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Basic Grayscale Print Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.9</para>
        /// </summary>
        public static readonly String BasicGrayscalePrintManagementMetaSopClassUid = "1.2.840.10008.5.1.1.9";

        /// <summary>SopClass for
        /// <para>Basic Grayscale Print Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.1.9</para>
        /// </summary>
        public static readonly SopClass BasicGrayscalePrintManagementMetaSopClass =
                             new SopClass("Basic Grayscale Print Management Meta SOP Class", 
                                          SopClass.BasicGrayscalePrintManagementMetaSopClassUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Detached Patient Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.1.4</para>
        /// </summary>
        public static readonly String DetachedPatientManagementMetaSopClassRetiredUid = "1.2.840.10008.3.1.2.1.4";

        /// <summary>SopClass for
        /// <para>Detached Patient Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.1.4</para>
        /// </summary>
        public static readonly SopClass DetachedPatientManagementMetaSopClassRetired =
                             new SopClass("Detached Patient Management Meta SOP Class (Retired)", 
                                          SopClass.DetachedPatientManagementMetaSopClassRetiredUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Detached Results Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.4</para>
        /// </summary>
        public static readonly String DetachedResultsManagementMetaSopClassRetiredUid = "1.2.840.10008.3.1.2.5.4";

        /// <summary>SopClass for
        /// <para>Detached Results Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.4</para>
        /// </summary>
        public static readonly SopClass DetachedResultsManagementMetaSopClassRetired =
                             new SopClass("Detached Results Management Meta SOP Class (Retired)", 
                                          SopClass.DetachedResultsManagementMetaSopClassRetiredUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Detached Study Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.5</para>
        /// </summary>
        public static readonly String DetachedStudyManagementMetaSopClassRetiredUid = "1.2.840.10008.3.1.2.5.5";

        /// <summary>SopClass for
        /// <para>Detached Study Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.3.1.2.5.5</para>
        /// </summary>
        public static readonly SopClass DetachedStudyManagementMetaSopClassRetired =
                             new SopClass("Detached Study Management Meta SOP Class (Retired)", 
                                          SopClass.DetachedStudyManagementMetaSopClassRetiredUid, 
                                          true);
        /// <summary>String UID for
        /// <para>General Purpose Worklist Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32</para>
        /// </summary>
        public static readonly String GeneralPurposeWorklistManagementMetaSopClassUid = "1.2.840.10008.5.1.4.32";

        /// <summary>SopClass for
        /// <para>General Purpose Worklist Management Meta SOP Class</para>
        /// <para>UID: 1.2.840.10008.5.1.4.32</para>
        /// </summary>
        public static readonly SopClass GeneralPurposeWorklistManagementMetaSopClass =
                             new SopClass("General Purpose Worklist Management Meta SOP Class", 
                                          SopClass.GeneralPurposeWorklistManagementMetaSopClassUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Pull Stored Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.32</para>
        /// </summary>
        public static readonly String PullStoredPrintManagementMetaSopClassRetiredUid = "1.2.840.10008.5.1.1.32";

        /// <summary>SopClass for
        /// <para>Pull Stored Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.32</para>
        /// </summary>
        public static readonly SopClass PullStoredPrintManagementMetaSopClassRetired =
                             new SopClass("Pull Stored Print Management Meta SOP Class (Retired)", 
                                          SopClass.PullStoredPrintManagementMetaSopClassRetiredUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Referenced Color Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.18.1</para>
        /// </summary>
        public static readonly String ReferencedColorPrintManagementMetaSopClassRetiredUid = "1.2.840.10008.5.1.1.18.1";

        /// <summary>SopClass for
        /// <para>Referenced Color Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.18.1</para>
        /// </summary>
        public static readonly SopClass ReferencedColorPrintManagementMetaSopClassRetired =
                             new SopClass("Referenced Color Print Management Meta SOP Class (Retired)", 
                                          SopClass.ReferencedColorPrintManagementMetaSopClassRetiredUid, 
                                          true);
        /// <summary>String UID for
        /// <para>Referenced Grayscale Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.9.1</para>
        /// </summary>
        public static readonly String ReferencedGrayscalePrintManagementMetaSopClassRetiredUid = "1.2.840.10008.5.1.1.9.1";

        /// <summary>SopClass for
        /// <para>Referenced Grayscale Print Management Meta SOP Class (Retired)</para>
        /// <para>UID: 1.2.840.10008.5.1.1.9.1</para>
        /// </summary>
        public static readonly SopClass ReferencedGrayscalePrintManagementMetaSopClassRetired =
                             new SopClass("Referenced Grayscale Print Management Meta SOP Class (Retired)", 
                                          SopClass.ReferencedGrayscalePrintManagementMetaSopClassRetiredUid, 
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

        /// <summary>Override that displays the name of the SOP Class.</summary>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>Retrieve a SopClass object associated with the Uid.</summary>
        public static SopClass GetSopClass(String uid)
        {
            if (_bIsFirst)
            {
                _bIsFirst = false;
                _sopList.Add(SopClass.Sop12LeadEcgWaveformStorageUid, 
                             SopClass.Sop12LeadEcgWaveformStorage);

                _sopList.Add(SopClass.AmbulatoryEcgWaveformStorageUid, 
                             SopClass.AmbulatoryEcgWaveformStorage);

                _sopList.Add(SopClass.BasicAnnotationBoxSopClassUid, 
                             SopClass.BasicAnnotationBoxSopClass);

                _sopList.Add(SopClass.BasicColorImageBoxSopClassUid, 
                             SopClass.BasicColorImageBoxSopClass);

                _sopList.Add(SopClass.BasicFilmBoxSopClassUid, 
                             SopClass.BasicFilmBoxSopClass);

                _sopList.Add(SopClass.BasicFilmSessionSopClassUid, 
                             SopClass.BasicFilmSessionSopClass);

                _sopList.Add(SopClass.BasicGrayscaleImageBoxSopClassUid, 
                             SopClass.BasicGrayscaleImageBoxSopClass);

                _sopList.Add(SopClass.BasicPrintImageOverlayBoxSopClassRetiredUid, 
                             SopClass.BasicPrintImageOverlayBoxSopClassRetired);

                _sopList.Add(SopClass.BasicStudyContentNotificationSopClassRetiredUid, 
                             SopClass.BasicStudyContentNotificationSopClassRetired);

                _sopList.Add(SopClass.BasicTextSrUid, 
                             SopClass.BasicTextSr);

                _sopList.Add(SopClass.BasicVoiceAudioWaveformStorageUid, 
                             SopClass.BasicVoiceAudioWaveformStorage);

                _sopList.Add(SopClass.BlendingSoftcopyPresentationStateStorageSopClassUid, 
                             SopClass.BlendingSoftcopyPresentationStateStorageSopClass);

                _sopList.Add(SopClass.BreastImagingRelevantPatientInformationQueryUid, 
                             SopClass.BreastImagingRelevantPatientInformationQuery);

                _sopList.Add(SopClass.CardiacElectrophysiologyWaveformStorageUid, 
                             SopClass.CardiacElectrophysiologyWaveformStorage);

                _sopList.Add(SopClass.CardiacRelevantPatientInformationQueryUid, 
                             SopClass.CardiacRelevantPatientInformationQuery);

                _sopList.Add(SopClass.ChestCadSrUid, 
                             SopClass.ChestCadSr);

                _sopList.Add(SopClass.ColorSoftcopyPresentationStateStorageSopClassUid, 
                             SopClass.ColorSoftcopyPresentationStateStorageSopClass);

                _sopList.Add(SopClass.ComprehensiveSrUid, 
                             SopClass.ComprehensiveSr);

                _sopList.Add(SopClass.ComputedRadiographyImageStorageUid, 
                             SopClass.ComputedRadiographyImageStorage);

                _sopList.Add(SopClass.CtImageStorageUid, 
                             SopClass.CtImageStorage);

                _sopList.Add(SopClass.DeformableSpatialRegistrationStorageUid, 
                             SopClass.DeformableSpatialRegistrationStorage);

                _sopList.Add(SopClass.DetachedInterpretationManagementSopClassRetiredUid, 
                             SopClass.DetachedInterpretationManagementSopClassRetired);

                _sopList.Add(SopClass.DetachedPatientManagementSopClassRetiredUid, 
                             SopClass.DetachedPatientManagementSopClassRetired);

                _sopList.Add(SopClass.DetachedResultsManagementSopClassRetiredUid, 
                             SopClass.DetachedResultsManagementSopClassRetired);

                _sopList.Add(SopClass.DetachedStudyManagementSopClassRetiredUid, 
                             SopClass.DetachedStudyManagementSopClassRetired);

                _sopList.Add(SopClass.DetachedVisitManagementSopClassRetiredUid, 
                             SopClass.DetachedVisitManagementSopClassRetired);

                _sopList.Add(SopClass.DigitalIntraOralXRayImageStorageForPresentationUid, 
                             SopClass.DigitalIntraOralXRayImageStorageForPresentation);

                _sopList.Add(SopClass.DigitalIntraOralXRayImageStorageForProcessingUid, 
                             SopClass.DigitalIntraOralXRayImageStorageForProcessing);

                _sopList.Add(SopClass.DigitalMammographyXRayImageStorageForPresentationUid, 
                             SopClass.DigitalMammographyXRayImageStorageForPresentation);

                _sopList.Add(SopClass.DigitalMammographyXRayImageStorageForProcessingUid, 
                             SopClass.DigitalMammographyXRayImageStorageForProcessing);

                _sopList.Add(SopClass.DigitalXRayImageStorageForPresentationUid, 
                             SopClass.DigitalXRayImageStorageForPresentation);

                _sopList.Add(SopClass.DigitalXRayImageStorageForProcessingUid, 
                             SopClass.DigitalXRayImageStorageForProcessing);

                _sopList.Add(SopClass.EncapsulatedPdfStorageUid, 
                             SopClass.EncapsulatedPdfStorage);

                _sopList.Add(SopClass.EnhancedCtImageStorageUid, 
                             SopClass.EnhancedCtImageStorage);

                _sopList.Add(SopClass.EnhancedMrImageStorageUid, 
                             SopClass.EnhancedMrImageStorage);

                _sopList.Add(SopClass.EnhancedSrUid, 
                             SopClass.EnhancedSr);

                _sopList.Add(SopClass.EnhancedXaImageStorageUid, 
                             SopClass.EnhancedXaImageStorage);

                _sopList.Add(SopClass.EnhancedXrfImageStorageUid, 
                             SopClass.EnhancedXrfImageStorage);

                _sopList.Add(SopClass.GeneralEcgWaveformStorageUid, 
                             SopClass.GeneralEcgWaveformStorage);

                _sopList.Add(SopClass.GeneralPurposePerformedProcedureStepSopClassUid, 
                             SopClass.GeneralPurposePerformedProcedureStepSopClass);

                _sopList.Add(SopClass.GeneralPurposeScheduledProcedureStepSopClassUid, 
                             SopClass.GeneralPurposeScheduledProcedureStepSopClass);

                _sopList.Add(SopClass.GeneralPurposeWorklistInformationModelFindUid, 
                             SopClass.GeneralPurposeWorklistInformationModelFind);

                _sopList.Add(SopClass.GeneralRelevantPatientInformationQueryUid, 
                             SopClass.GeneralRelevantPatientInformationQuery);

                _sopList.Add(SopClass.GrayscaleSoftcopyPresentationStateStorageSopClassUid, 
                             SopClass.GrayscaleSoftcopyPresentationStateStorageSopClass);

                _sopList.Add(SopClass.HangingProtocolInformationModelFindUid, 
                             SopClass.HangingProtocolInformationModelFind);

                _sopList.Add(SopClass.HangingProtocolInformationModelMoveUid, 
                             SopClass.HangingProtocolInformationModelMove);

                _sopList.Add(SopClass.HangingProtocolStorageUid, 
                             SopClass.HangingProtocolStorage);

                _sopList.Add(SopClass.HardcopyGrayscaleImageStorageSopClassRetiredUid, 
                             SopClass.HardcopyGrayscaleImageStorageSopClassRetired);

                _sopList.Add(SopClass.HardcopyColorImageStorageSopClassRetiredUid, 
                             SopClass.HardcopyColorImageStorageSopClassRetired);

                _sopList.Add(SopClass.HemodynamicWaveformStorageUid, 
                             SopClass.HemodynamicWaveformStorage);

                _sopList.Add(SopClass.ImageOverlayBoxSopClassRetiredUid, 
                             SopClass.ImageOverlayBoxSopClassRetired);

                _sopList.Add(SopClass.InstanceAvailabilityNotificationSopClassUid, 
                             SopClass.InstanceAvailabilityNotificationSopClass);

                _sopList.Add(SopClass.KeyObjectSelectionDocumentUid, 
                             SopClass.KeyObjectSelectionDocument);

                _sopList.Add(SopClass.MammographyCadSrUid, 
                             SopClass.MammographyCadSr);

                _sopList.Add(SopClass.MediaCreationManagementSopClassUidUid, 
                             SopClass.MediaCreationManagementSopClassUid);

                _sopList.Add(SopClass.MediaStorageDirectoryStorageUid, 
                             SopClass.MediaStorageDirectoryStorage);

                _sopList.Add(SopClass.ModalityPerformedProcedureStepNotificationSopClassUid, 
                             SopClass.ModalityPerformedProcedureStepNotificationSopClass);

                _sopList.Add(SopClass.ModalityPerformedProcedureStepRetrieveSopClassUid, 
                             SopClass.ModalityPerformedProcedureStepRetrieveSopClass);

                _sopList.Add(SopClass.ModalityPerformedProcedureStepSopClassUid, 
                             SopClass.ModalityPerformedProcedureStepSopClass);

                _sopList.Add(SopClass.ModalityWorklistInformationModelFindUid, 
                             SopClass.ModalityWorklistInformationModelFind);

                _sopList.Add(SopClass.MrImageStorageUid, 
                             SopClass.MrImageStorage);

                _sopList.Add(SopClass.MrSpectroscopyStorageUid, 
                             SopClass.MrSpectroscopyStorage);

                _sopList.Add(SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorageUid, 
                             SopClass.MultiFrameGrayscaleByteSecondaryCaptureImageStorage);

                _sopList.Add(SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorageUid, 
                             SopClass.MultiFrameGrayscaleWordSecondaryCaptureImageStorage);

                _sopList.Add(SopClass.MultiFrameSingleBitSecondaryCaptureImageStorageUid, 
                             SopClass.MultiFrameSingleBitSecondaryCaptureImageStorage);

                _sopList.Add(SopClass.MultiFrameTrueColorSecondaryCaptureImageStorageUid, 
                             SopClass.MultiFrameTrueColorSecondaryCaptureImageStorage);

                _sopList.Add(SopClass.NuclearMedicineImageStorageRetiredUid, 
                             SopClass.NuclearMedicineImageStorageRetired);

                _sopList.Add(SopClass.NuclearMedicineImageStorageUid, 
                             SopClass.NuclearMedicineImageStorage);

                _sopList.Add(SopClass.OphthalmicPhotography16BitImageStorageUid, 
                             SopClass.OphthalmicPhotography16BitImageStorage);

                _sopList.Add(SopClass.OphthalmicPhotography8BitImageStorageUid, 
                             SopClass.OphthalmicPhotography8BitImageStorage);

                _sopList.Add(SopClass.PatientRootQueryRetrieveInformationModelFindUid, 
                             SopClass.PatientRootQueryRetrieveInformationModelFind);

                _sopList.Add(SopClass.PatientRootQueryRetrieveInformationModelGetUid, 
                             SopClass.PatientRootQueryRetrieveInformationModelGet);

                _sopList.Add(SopClass.PatientRootQueryRetrieveInformationModelMoveUid, 
                             SopClass.PatientRootQueryRetrieveInformationModelMove);

                _sopList.Add(SopClass.PatientStudyOnlyQueryRetrieveInformationModelFindRetiredUid, 
                             SopClass.PatientStudyOnlyQueryRetrieveInformationModelFindRetired);

                _sopList.Add(SopClass.PatientStudyOnlyQueryRetrieveInformationModelGetRetiredUid, 
                             SopClass.PatientStudyOnlyQueryRetrieveInformationModelGetRetired);

                _sopList.Add(SopClass.PatientStudyOnlyQueryRetrieveInformationModelMoveRetiredUid, 
                             SopClass.PatientStudyOnlyQueryRetrieveInformationModelMoveRetired);

                _sopList.Add(SopClass.PositronEmissionTomographyImageStorageUid, 
                             SopClass.PositronEmissionTomographyImageStorage);

                _sopList.Add(SopClass.PresentationLutSopClassUid, 
                             SopClass.PresentationLutSopClass);

                _sopList.Add(SopClass.PrintJobSopClassUid, 
                             SopClass.PrintJobSopClass);

                _sopList.Add(SopClass.PrintQueueManagementSopClassRetiredUid, 
                             SopClass.PrintQueueManagementSopClassRetired);

                _sopList.Add(SopClass.PrinterConfigurationRetrievalSopClassUid, 
                             SopClass.PrinterConfigurationRetrievalSopClass);

                _sopList.Add(SopClass.PrinterSopClassUid, 
                             SopClass.PrinterSopClass);

                _sopList.Add(SopClass.ProceduralEventLoggingSopClassUid, 
                             SopClass.ProceduralEventLoggingSopClass);

                _sopList.Add(SopClass.ProcedureLogStorageUid, 
                             SopClass.ProcedureLogStorage);

                _sopList.Add(SopClass.PseudoColorSoftcopyPresentationStateStorageSopClassUid, 
                             SopClass.PseudoColorSoftcopyPresentationStateStorageSopClass);

                _sopList.Add(SopClass.PullPrintRequestSopClassRetiredUid, 
                             SopClass.PullPrintRequestSopClassRetired);

                _sopList.Add(SopClass.RawDataStorageUid, 
                             SopClass.RawDataStorage);

                _sopList.Add(SopClass.RealWorldValueMappingStorageUid, 
                             SopClass.RealWorldValueMappingStorage);

                _sopList.Add(SopClass.ReferencedImageBoxSopClassRetiredUid, 
                             SopClass.ReferencedImageBoxSopClassRetired);

                _sopList.Add(SopClass.RtBeamsTreatmentRecordStorageUid, 
                             SopClass.RtBeamsTreatmentRecordStorage);

                _sopList.Add(SopClass.RtBrachyTreatmentRecordStorageUid, 
                             SopClass.RtBrachyTreatmentRecordStorage);

                _sopList.Add(SopClass.RtDoseStorageUid, 
                             SopClass.RtDoseStorage);

                _sopList.Add(SopClass.RtImageStorageUid, 
                             SopClass.RtImageStorage);

                _sopList.Add(SopClass.RtIonBeamsTreatmentRecordStorageUid, 
                             SopClass.RtIonBeamsTreatmentRecordStorage);

                _sopList.Add(SopClass.RtIonPlanStorageUid, 
                             SopClass.RtIonPlanStorage);

                _sopList.Add(SopClass.RtPlanStorageUid, 
                             SopClass.RtPlanStorage);

                _sopList.Add(SopClass.RtStructureSetStorageUid, 
                             SopClass.RtStructureSetStorage);

                _sopList.Add(SopClass.RtTreatmentSummaryRecordStorageUid, 
                             SopClass.RtTreatmentSummaryRecordStorage);

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

                _sopList.Add(SopClass.StandaloneModalityLutStorageRetiredUid, 
                             SopClass.StandaloneModalityLutStorageRetired);

                _sopList.Add(SopClass.StandaloneOverlayStorageRetiredUid, 
                             SopClass.StandaloneOverlayStorageRetired);

                _sopList.Add(SopClass.StandalonePetCurveStorageRetiredUid, 
                             SopClass.StandalonePetCurveStorageRetired);

                _sopList.Add(SopClass.StandaloneVoiLutStorageRetiredUid, 
                             SopClass.StandaloneVoiLutStorageRetired);

                _sopList.Add(SopClass.StereometricRelationshipStorageUid, 
                             SopClass.StereometricRelationshipStorage);

                _sopList.Add(SopClass.StorageCommitmentPullModelSopClassRetiredUid, 
                             SopClass.StorageCommitmentPullModelSopClassRetired);

                _sopList.Add(SopClass.StorageCommitmentPushModelSopClassUid, 
                             SopClass.StorageCommitmentPushModelSopClass);

                _sopList.Add(SopClass.StoredPrintStorageSopClassRetiredUid, 
                             SopClass.StoredPrintStorageSopClassRetired);

                _sopList.Add(SopClass.StudyComponentManagementSopClassRetiredUid, 
                             SopClass.StudyComponentManagementSopClassRetired);

                _sopList.Add(SopClass.StudyRootQueryRetrieveInformationModelFindUid, 
                             SopClass.StudyRootQueryRetrieveInformationModelFind);

                _sopList.Add(SopClass.StudyRootQueryRetrieveInformationModelGetUid, 
                             SopClass.StudyRootQueryRetrieveInformationModelGet);

                _sopList.Add(SopClass.StudyRootQueryRetrieveInformationModelMoveUid, 
                             SopClass.StudyRootQueryRetrieveInformationModelMove);

                _sopList.Add(SopClass.UltrasoundImageStorageUid, 
                             SopClass.UltrasoundImageStorage);

                _sopList.Add(SopClass.UltrasoundImageStorageRetiredUid, 
                             SopClass.UltrasoundImageStorageRetired);

                _sopList.Add(SopClass.UltrasoundMultiFrameImageStorageUid, 
                             SopClass.UltrasoundMultiFrameImageStorage);

                _sopList.Add(SopClass.UltrasoundMultiFrameImageStorageRetiredUid, 
                             SopClass.UltrasoundMultiFrameImageStorageRetired);

                _sopList.Add(SopClass.VerificationSopClassUid, 
                             SopClass.VerificationSopClass);

                _sopList.Add(SopClass.VideoEndoscopicImageStorageUid, 
                             SopClass.VideoEndoscopicImageStorage);

                _sopList.Add(SopClass.VideoMicroscopicImageStorageUid, 
                             SopClass.VideoMicroscopicImageStorage);

                _sopList.Add(SopClass.VideoPhotographicImageStorageUid, 
                             SopClass.VideoPhotographicImageStorage);

                _sopList.Add(SopClass.VlEndoscopicImageStorageUid, 
                             SopClass.VlEndoscopicImageStorage);

                _sopList.Add(SopClass.VlMicroscopicImageStorageUid, 
                             SopClass.VlMicroscopicImageStorage);

                _sopList.Add(SopClass.VlPhotographicImageStorageUid, 
                             SopClass.VlPhotographicImageStorage);

                _sopList.Add(SopClass.VlSlideCoordinatesMicroscopicImageStorageUid, 
                             SopClass.VlSlideCoordinatesMicroscopicImageStorage);

                _sopList.Add(SopClass.VoiLutBoxSopClassUid, 
                             SopClass.VoiLutBoxSopClass);

                _sopList.Add(SopClass.XRayAngiographicBiPlaneImageStorageRetiredUid, 
                             SopClass.XRayAngiographicBiPlaneImageStorageRetired);

                _sopList.Add(SopClass.XRayAngiographicImageStorageUid, 
                             SopClass.XRayAngiographicImageStorage);

                _sopList.Add(SopClass.XRayRadiationDoseSrUid, 
                             SopClass.XRayRadiationDoseSr);

                _sopList.Add(SopClass.XRayRadiofluoroscopicImageStorageUid, 
                             SopClass.XRayRadiofluoroscopicImageStorage);

                _sopList.Add(SopClass.BasicColorPrintManagementMetaSopClassUid, 
                             SopClass.BasicColorPrintManagementMetaSopClass);

                _sopList.Add(SopClass.BasicGrayscalePrintManagementMetaSopClassUid, 
                             SopClass.BasicGrayscalePrintManagementMetaSopClass);

                _sopList.Add(SopClass.DetachedPatientManagementMetaSopClassRetiredUid, 
                             SopClass.DetachedPatientManagementMetaSopClassRetired);

                _sopList.Add(SopClass.DetachedResultsManagementMetaSopClassRetiredUid, 
                             SopClass.DetachedResultsManagementMetaSopClassRetired);

                _sopList.Add(SopClass.DetachedStudyManagementMetaSopClassRetiredUid, 
                             SopClass.DetachedStudyManagementMetaSopClassRetired);

                _sopList.Add(SopClass.GeneralPurposeWorklistManagementMetaSopClassUid, 
                             SopClass.GeneralPurposeWorklistManagementMetaSopClass);

                _sopList.Add(SopClass.PullStoredPrintManagementMetaSopClassRetiredUid, 
                             SopClass.PullStoredPrintManagementMetaSopClassRetired);

                _sopList.Add(SopClass.ReferencedColorPrintManagementMetaSopClassRetiredUid, 
                             SopClass.ReferencedColorPrintManagementMetaSopClassRetired);

                _sopList.Add(SopClass.ReferencedGrayscalePrintManagementMetaSopClassRetiredUid, 
                             SopClass.ReferencedGrayscalePrintManagementMetaSopClassRetired);

            }

            if (!_sopList.ContainsKey(uid))
                return null;

            return _sopList[uid];
        }
    }
}
