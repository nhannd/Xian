#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
    public partial class Study
    {
        public void Initialize(DicomMessageBase file)
        {
            DicomAttributeCollection sopInstanceDataset = file.DataSet;

            DicomAttribute attribute = sopInstanceDataset[DicomTags.StudyInstanceUid];
            string datasetStudyUid = attribute.ToString();
            if (!String.IsNullOrEmpty(StudyInstanceUid) && StudyInstanceUid != datasetStudyUid)
            {
                string message = String.Format("The study uid in the data set does not match this study's uid ({0} != {1}).",
                                               datasetStudyUid, StudyInstanceUid);

                throw new InvalidOperationException(message);
            }

            StudyInstanceUid = attribute.ToString();

            Platform.CheckForEmptyString(StudyInstanceUid, "StudyInstanceUid");

            attribute = sopInstanceDataset[DicomTags.PatientId];
            PatientId = attribute.ToString();

            attribute = sopInstanceDataset[DicomTags.PatientsName];
            PatientsName = new PersonName(attribute.ToString());
            PatientsNameRaw = DicomImplementation.CharacterParser.EncodeAsIsomorphicString(PatientsName, sopInstanceDataset.SpecificCharacterSet);

            attribute = sopInstanceDataset[DicomTags.ReferringPhysiciansName];
            ReferringPhysiciansName = new PersonName(attribute.ToString());
            ReferringPhysiciansNameRaw = DicomImplementation.CharacterParser.EncodeAsIsomorphicString(ReferringPhysiciansName, sopInstanceDataset.SpecificCharacterSet);

            attribute = sopInstanceDataset[DicomTags.PatientsSex];
            PatientsSex = attribute.ToString();

            attribute = sopInstanceDataset[DicomTags.PatientsBirthDate];
            PatientsBirthDateRaw = attribute.ToString();

            attribute = sopInstanceDataset[DicomTags.StudyId];
            StudyId = attribute.ToString();

            attribute = sopInstanceDataset[DicomTags.AccessionNumber];
            AccessionNumber = attribute.ToString();

            attribute = sopInstanceDataset[DicomTags.StudyDescription];
            StudyDescription = attribute.ToString();

            attribute = sopInstanceDataset[DicomTags.StudyDate];
            StudyDateRaw = attribute.ToString();
            StudyDate = DateParser.Parse(StudyDateRaw);

            attribute = sopInstanceDataset[DicomTags.StudyTime];
            StudyTimeRaw = attribute.ToString();

            if (sopInstanceDataset.Contains(DicomTags.ProcedureCodeSequence))
            {
                attribute = sopInstanceDataset[DicomTags.ProcedureCodeSequence];
                if (!attribute.IsEmpty && !attribute.IsNull)
                {
                    DicomSequenceItem sequence = ((DicomSequenceItem[])attribute.Values)[0];
                    ProcedureCodeSequenceCodeValue = sequence[DicomTags.CodeValue].ToString();
                    ProcedureCodeSequenceCodingSchemeDesignator = sequence[DicomTags.CodingSchemeDesignator].ToString();
                }
            }

            attribute = sopInstanceDataset[DicomTags.PatientSpeciesDescription];
            PatientSpeciesDescription = attribute.ToString();

            if (sopInstanceDataset.Contains(DicomTags.PatientSpeciesCodeSequence))
            {
                attribute = sopInstanceDataset[DicomTags.PatientSpeciesCodeSequence];
                if (!attribute.IsEmpty && !attribute.IsNull)
                {
                    DicomSequenceItem sequence = ((DicomSequenceItem[])attribute.Values)[0];
                    PatientSpeciesCodeSequenceCodingSchemeDesignator = sequence[DicomTags.CodingSchemeDesignator].ToString();
                    PatientSpeciesCodeSequenceCodeValue = sequence[DicomTags.CodeValue].ToString();
                    PatientSpeciesCodeSequenceCodeMeaning = sequence[DicomTags.CodeMeaning].ToString();
                }
            }

            attribute = sopInstanceDataset[DicomTags.PatientBreedDescription];
            PatientBreedDescription = attribute.ToString();

            if (sopInstanceDataset.Contains(DicomTags.PatientBreedCodeSequence))
            {
                attribute = sopInstanceDataset[DicomTags.PatientBreedCodeSequence];
                if (!attribute.IsEmpty && !attribute.IsNull)
                {
                    DicomSequenceItem sequence = ((DicomSequenceItem[])attribute.Values)[0];
                    PatientBreedCodeSequenceCodingSchemeDesignator = sequence[DicomTags.CodingSchemeDesignator].ToString();
                    PatientBreedCodeSequenceCodeValue = sequence[DicomTags.CodeValue].ToString();
                    PatientBreedCodeSequenceCodeMeaning = sequence[DicomTags.CodeMeaning].ToString();
                }
            }

            attribute = sopInstanceDataset[DicomTags.ResponsiblePerson];
            ResponsiblePerson = new PersonName(attribute.ToString());
            ResponsiblePersonRaw = DicomImplementation.CharacterParser.EncodeAsIsomorphicString(ResponsiblePerson, sopInstanceDataset.SpecificCharacterSet);

            attribute = sopInstanceDataset[DicomTags.ResponsiblePersonRole];
            ResponsiblePersonRole = attribute.ToString();

            attribute = sopInstanceDataset[DicomTags.ResponsibleOrganization];
            ResponsibleOrganization = attribute.ToString();

            attribute = sopInstanceDataset[DicomTags.SpecificCharacterSet];
            SpecificCharacterSet = attribute.ToString();

            string[] modalitiesInStudy = DicomStringHelper.GetStringArray(ModalitiesInStudy ?? "");
            ModalitiesInStudy = DicomStringHelper.GetDicomStringArray(
                ComputeModalitiesInStudy(modalitiesInStudy, sopInstanceDataset[DicomTags.Modality].GetString(0, "")));
        }

        private static IEnumerable<string> ComputeModalitiesInStudy(IEnumerable<string> existingModalities, string candidate)
        {
            foreach (string existingModality in existingModalities)
            {
                if (existingModality == candidate)
                    candidate = null;

                yield return existingModality;
            }

            if (candidate != null)
                yield return candidate;
        }
    }
}
