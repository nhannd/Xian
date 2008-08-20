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
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Sequences;

namespace ClearCanvas.Dicom.Iod.Modules
{
    /// <summary>
    /// As per Dicom DOC 3 C.4.15 (pg 256)
    /// </summary>
    public class ImageAcquisitionResultsModuleIod : IodBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageAcquisitionResultsModuleIod"/> class.
        /// </summary>
        public ImageAcquisitionResultsModuleIod()
            :base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageAcquisitionResultsModuleIod"/> class.
        /// </summary>
        /// <param name="dicomAttributeCollection">The dicom attribute collection.</param>
        public ImageAcquisitionResultsModuleIod(DicomAttributeCollection dicomAttributeCollection)
            :base(dicomAttributeCollection)
        {
        }
        #endregion

        #region Public Properties
        public Modality Modality
        {
            get { return IodBase.ParseEnum<Modality>(base.DicomAttributeCollection[DicomTags.Modality].GetString(0, String.Empty), Modality.None); }
            set { IodBase.SetAttributeFromEnum(base.DicomAttributeCollection[DicomTags.Modality], value); }
        }

        public string StudyId
        {
            get { return base.DicomAttributeCollection[DicomTags.StudyId].GetString(0, String.Empty); }
            set { base.DicomAttributeCollection[DicomTags.StudyId].SetString(0, value); }
        }

        /// <summary>
        /// Gets the performed protocol code sequence list.
        /// Sequence describing the Protocol performed for this Procedure Step. This sequence 
        /// may have zero or more Items.
        /// </summary>
        /// <value>The performed protocol code sequence list.</value>
        public SequenceIodList<CodeSequenceMacro> PerformedProtocolCodeSequenceList
        {
            get
            {
                return new SequenceIodList<CodeSequenceMacro>(base.DicomAttributeCollection[DicomTags.PerformedProtocolCodeSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Gets the protocol context sequence list.
        /// Sequence that specifies the context for the Performed Protocol Code Sequence Item. 
        /// One or more items may be included in this sequence. See Section C.4.10.1.
        /// </summary>
        /// <value>The protocol context sequence list.</value>
        public SequenceIodList<ContentItemMacro> ProtocolContextSequenceList
        {
            get
            {
                return new SequenceIodList<ContentItemMacro>(base.DicomAttributeCollection[DicomTags.ProtocolContextSequence] as DicomAttributeSQ);
            }
        }

        /// <summary>
        /// Sequence that specifies modifiers for a Protocol Context Content Item. One or 
        /// more items may be included in this sequence. See Section C.4.10.1.
        /// </summary>
        /// <value>The content item modifier sequence list.</value>
        public SequenceIodList<ContentItemMacro> ContentItemModifierSequenceList
        {
            get
            {
                return new SequenceIodList<ContentItemMacro>(base.DicomAttributeCollection[DicomTags.ContentItemModifierSequence] as DicomAttributeSQ);
            }
        }

        public SequenceIodList<PerformedSeriesSequenceIod> PerformedSeriesSequenceList
        {
            get
            {
                return new SequenceIodList<PerformedSeriesSequenceIod>(base.DicomAttributeCollection[DicomTags.PerformedSeriesSequence] as DicomAttributeSQ);
            }
        }

        #endregion

    }

    #region Modality Enum
    /// <summary>
    /// Modality Enum.  as per Part 3, C.7.3.1.1.1
    /// </summary>
    /// <remarks>The modality listed in the Modality Data Element (0008,0060) may not match the name of the 
    /// IOD in which it appears. For example, a SOP instance from XA IOD may list the RF modality 
    /// when an RF implementation produces an XA object.
    /// </remarks>
    public enum Modality
    {
        /// <summary>
        /// None or blank value
        /// </summary>
        None,
        /// <summary>
        /// Computed Radiography
        /// </summary>
        CR, 
        /// <summary>
        /// Computed Tomography
        /// </summary>
        CT,
        /// <summary>
        /// Magnetic Resonance (The MR modality incorporates the retired modalities MA and MS.)
        /// </summary>
        MR, 
        /// <summary>
        /// Nuclear Medicine
        /// </summary>
        NM,
        /// <summary>
        /// Ultrasound
        /// </summary>
        US, 
        /// <summary>
        /// Other
        /// </summary>
        OT,
        /// <summary>
        /// Biomagnetic imaging 
        /// </summary>
        BI,
        /// <summary>
        /// Color flow Doppler
        /// </summary>
        CD,
        /// <summary>
        /// Duplex Doppler 
        /// </summary>
        DD,
        /// <summary>
        /// = Diaphanography
        /// </summary>
        DG,
        /// <summary>
        /// Endoscopy 
        /// </summary>
        ES, 
        /// <summary>
        /// Laser surface scan
        /// </summary>
        LS,
        /// <summary>
        /// Positron emission tomography (PET) 
        /// </summary>
        PT,
        /// <summary>
        /// Radiographic imaging (conventional film/screen)
        /// </summary>
        RG,
        /// <summary>
        /// Single-photon emission computed tomography (SPECT)
        /// </summary>
        ST,
        /// <summary>
        /// Thermography
        /// </summary>
        TG,
        /// <summary>
        /// X-Ray Angiography (incorporates the retired modality DS)
        /// </summary>
        XA,
        /// <summary>
        /// Radio Fluoroscopy (The RF modality incorporates the retired modalities CF, DF, VF.)
        /// </summary>
        RF,
        /// <summary>
        /// Radiotherapy Image
        /// </summary>
        RTImage, 
        /// <summary>
        /// Radiotherapy Dose
        /// </summary>
        RTDose,
        /// <summary>
        /// Radiotherapy Structure Set 
        /// </summary>
        RTStruct,
        /// <summary>
        /// Radiotherapy Plan
        /// </summary>
        RTPlan,
        /// <summary>
        /// RT Treatment Record 
        /// </summary>
        RTRecord,
        /// <summary>
        /// Hard Copy
        /// </summary>
        HC,
        /// <summary>
        /// Digital Radiography 
        /// </summary>
        DX,
        /// <summary>
        /// Mammography
        /// </summary>
        MG,
        /// <summary>
        /// Intra-oral Radiography 
        /// </summary>
        IO,
        /// <summary>
        /// Panoramic X-Ray
        /// </summary>
        PX,
        /// <summary>
        /// General Microscopy
        /// </summary>
        GM, 
        /// <summary>
        /// Slide Microscopy
        /// </summary>
        SM,
        /// <summary>
        /// External-camera Photography 
        /// </summary>
        XC,
        /// <summary>
        /// Presentation State
        /// </summary>
        PR,
        /// <summary>
        /// Audio
        /// </summary>
        AU, 
        /// <summary>
        /// Electrocardiography
        /// </summary>
        Ecg,
        /// <summary>
        /// Cardiac Electrophysiology 
        /// </summary>
        Eps,
        /// <summary>
        /// Hemodynamic Waveform
        /// </summary>
        HD,
        /// <summary>
        /// SR Document 
        /// </summary>
        SR,
        /// <summary>
        /// Intravascular Ultrasound
        /// </summary>
        Ivus,
        /// <summary>
        /// Ophthalmic Photography 
        /// </summary>
        OP,
        /// <summary>
        /// Stereometric Relationship
        /// </summary>
        Smr,
        /// <summary>
        /// Optical Coherence Tomography 
        /// </summary>
        Oct,
        /// <summary>
        /// Ophthalmic Refraction
        /// </summary>
        Opr,
        /// <summary>
        /// Ophthalmic Visual Field
        /// </summary>
        Opv, 
        /// <summary>
        /// Ophthalmic Mapping
        /// </summary>
        Opm,
        /// <summary>
        /// Key Object Selection
        /// </summary>
        KO, 
        /// <summary>
        /// Segmentation
        /// </summary>
        Seg,
        /// <summary>
        /// Registration
        /// </summary>
        Reg,
        /// <summary>
        /// Digital Subtraction Angiography (retired)  The XA modality incorporates the retired modality DS.
        /// </summary>
        DS,
        /// <summary>
        /// Cinefluorography (retired) The RF modality incorporates the retired modalities CF, DF, VF.
        /// </summary>
        CF,
        /// <summary>
        /// Digital fluoroscopy (retired) The RF modality incorporates the retired modalities CF, DF, VF.
        /// </summary>
        DF, 
        /// <summary>
        ///  = Videofluorography (retired) The RF modality incorporates the retired modalities CF, DF, VF.
        /// </summary>
        VF,
        /// <summary>
        /// Angioscopy (retired)
        /// </summary>
        AS, 
        /// <summary>
        /// Cystoscopy (retired)
        /// </summary>
        CS,
        /// <summary>
        /// Echocardiography (retired)
        /// </summary>
        EC, 
        /// <summary>
        /// Laparoscopy (retired)
        /// </summary>
        LP,
        /// <summary>
        /// Fluorescein angiography (retired)
        /// </summary>
        FA, 
        /// <summary>
        /// Culposcopy (retired)
        /// </summary>
        CP,
        /// <summary>
        /// Digital microscopy (retired)
        /// </summary>
        DM, 
        /// <summary>
        /// Fundoscopy (retired)
        /// </summary>
        FS,
        /// <summary>
        /// Magnetic resonance angiography (retired) The MR modality incorporates the retired modalities MA and MS.
        /// </summary>
        MA,
        /// <summary>
        /// Magnetic resonance spectroscopy (retired) The MR modality incorporates the retired modalities MA and MS.
        /// </summary>
        MS
    }
    #endregion
    
}
