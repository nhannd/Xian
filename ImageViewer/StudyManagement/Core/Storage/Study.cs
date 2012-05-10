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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
    public partial class Study : IStudy
    {
        private List<ISeries> _series;
        private StudyLocation _studyLocation;
        private StudyXml _studyXml;

        #region IPatientData Members

        string IPatientData.PatientsName
        {
            get { return PatientsName; }
        }

        string IPatientData.PatientsBirthDate
        {
            get { return PatientsBirthDateRaw; }
        }

        string IPatientData.PatientsBirthTime
        {
            get { return PatientsBirthTimeRaw; }
        }

        string IPatientData.ResponsiblePerson
        {
            get { return ResponsiblePerson; }
        }

        #endregion

        #region IStudyData Members

        string IStudyData.ReferringPhysiciansName
        {
            get { return ReferringPhysiciansName; }
        }

        string IStudyData.StudyDate
        {
            get { return StudyDateRaw; }
        }

        string IStudyData.StudyTime
        {
            get { return StudyTimeRaw; }
        }

        string[] IStudyData.ModalitiesInStudy
        {
            get { return DicomStringHelper.GetStringArray(ModalitiesInStudy); }
        }

        string[] IStudyData.SopClassesInStudy
        {
            get { return DicomStringHelper.GetStringArray(SopClassesInStudy); }
        }

        int? IStudyData.NumberOfStudyRelatedSeries
        {
            get { return NumberOfStudyRelatedSeries; }
        }

        int? IStudyData.NumberOfStudyRelatedInstances
        {
            get { return NumberOfStudyRelatedInstances; }
        }

        #endregion
        #region IStudy Members

        public IEnumerable<ISeries> GetSeries()
        {
            return Series;
        }

        public IEnumerable<ISopInstance> GetSopInstances()
        {
            return Series.SelectMany(s => s.GetSopInstances());
        }

        public DateTime? GetStoreTime()
        {
            return StoreTime;
        }

        PersonName IStudy.PatientsName
        {
            get { return new PersonName(PatientsName); }
        }

        PersonName IStudy.ReferringPhysiciansName
        {
            get { return new PersonName(ReferringPhysiciansName); }
        }

        int IStudy.NumberOfStudyRelatedSeries
        {
            get { return NumberOfStudyRelatedSeries.HasValue ? NumberOfStudyRelatedSeries.Value : 0; }
        }

        int IStudy.NumberOfStudyRelatedInstances
        {
            get { return NumberOfStudyRelatedInstances.HasValue ? NumberOfStudyRelatedInstances.Value : 0; }
        }

        #endregion

        #region Public Methods

        public void Update(DicomMessageBase dicomMessage)
        {
            var dataSet = dicomMessage.DataSet;
            DicomAttribute attribute = dataSet[DicomTags.StudyInstanceUid];
            string datasetStudyUid = attribute.ToString();
            if (!String.IsNullOrEmpty(StudyInstanceUid) && StudyInstanceUid != datasetStudyUid)
            {
                string message = String.Format("The study uid in the data set does not match this study's uid ({0} != {1}).",
                                               datasetStudyUid, StudyInstanceUid);

                throw new InvalidOperationException(message);
            }

            StudyInstanceUid = attribute.ToString();

            Platform.CheckForEmptyString(StudyInstanceUid, "StudyInstanceUid");

            attribute = dataSet[DicomTags.PatientId];
            PatientId = attribute.ToString();

            attribute = dataSet[DicomTags.PatientsName];
            PatientsName = new PersonName(attribute.ToString());

            attribute = dataSet[DicomTags.ReferringPhysiciansName];
            ReferringPhysiciansName = new PersonName(attribute.ToString());

            attribute = dataSet[DicomTags.PatientsSex];
            PatientsSex = attribute.ToString();

            attribute = dataSet[DicomTags.PatientsBirthDate];
            PatientsBirthDateRaw = attribute.ToString();
            PatientsBirthDate = DateParser.Parse(PatientsBirthDateRaw);

            attribute = dataSet[DicomTags.PatientsBirthTime];
            PatientsBirthTimeRaw = attribute.ToString();
            var time = TimeParser.Parse(PatientsBirthTimeRaw);
            if (time.HasValue)
                PatientsBirthTimeTicks = time.Value.TimeOfDay.Ticks;
            else
                PatientsBirthTimeTicks = null;

            attribute = dataSet[DicomTags.StudyId];
            StudyId = attribute.ToString();

            attribute = dataSet[DicomTags.AccessionNumber];
            AccessionNumber = attribute.ToString();

            attribute = dataSet[DicomTags.StudyDescription];
            StudyDescription = attribute.ToString();

            attribute = dataSet[DicomTags.StudyDate];
            StudyDateRaw = attribute.ToString();
            StudyDate = DateParser.Parse(StudyDateRaw);

            attribute = dataSet[DicomTags.StudyTime];
            StudyTimeRaw = attribute.ToString();
            time = TimeParser.Parse(StudyTimeRaw);
            if (time.HasValue)
                StudyTimeTicks = time.Value.TimeOfDay.Ticks;
            else
                StudyTimeTicks = null;

            if (dataSet.Contains(DicomTags.ProcedureCodeSequence))
            {
                attribute = dataSet[DicomTags.ProcedureCodeSequence];
                if (!attribute.IsEmpty && !attribute.IsNull)
                {
                    DicomSequenceItem sequence = ((DicomSequenceItem[])attribute.Values)[0];
                    ProcedureCodeSequenceCodeValue = sequence[DicomTags.CodeValue].ToString();
                    ProcedureCodeSequenceCodingSchemeDesignator = sequence[DicomTags.CodingSchemeDesignator].ToString();
                }
            }

            attribute = dataSet[DicomTags.PatientSpeciesDescription];
            PatientSpeciesDescription = attribute.ToString();

            if (dataSet.Contains(DicomTags.PatientSpeciesCodeSequence))
            {
                attribute = dataSet[DicomTags.PatientSpeciesCodeSequence];
                if (!attribute.IsEmpty && !attribute.IsNull)
                {
                    DicomSequenceItem sequence = ((DicomSequenceItem[])attribute.Values)[0];
                    PatientSpeciesCodeSequenceCodingSchemeDesignator = sequence[DicomTags.CodingSchemeDesignator].ToString();
                    PatientSpeciesCodeSequenceCodeValue = sequence[DicomTags.CodeValue].ToString();
                    PatientSpeciesCodeSequenceCodeMeaning = sequence[DicomTags.CodeMeaning].ToString();
                }
            }

            attribute = dataSet[DicomTags.PatientBreedDescription];
            PatientBreedDescription = attribute.ToString();

            if (dataSet.Contains(DicomTags.PatientBreedCodeSequence))
            {
                attribute = dataSet[DicomTags.PatientBreedCodeSequence];
                if (!attribute.IsEmpty && !attribute.IsNull)
                {
                    DicomSequenceItem sequence = ((DicomSequenceItem[])attribute.Values)[0];
                    PatientBreedCodeSequenceCodingSchemeDesignator = sequence[DicomTags.CodingSchemeDesignator].ToString();
                    PatientBreedCodeSequenceCodeValue = sequence[DicomTags.CodeValue].ToString();
                    PatientBreedCodeSequenceCodeMeaning = sequence[DicomTags.CodeMeaning].ToString();
                }
            }

            attribute = dataSet[DicomTags.ResponsiblePerson];
            ResponsiblePerson = new PersonName(attribute.ToString());

            attribute = dataSet[DicomTags.ResponsiblePersonRole];
            ResponsiblePersonRole = attribute.ToString();

            attribute = dataSet[DicomTags.ResponsibleOrganization];
            ResponsibleOrganization = attribute.ToString();

            attribute = dataSet[DicomTags.SpecificCharacterSet];
            SpecificCharacterSet = attribute.ToString();

            string[] modalitiesInStudy = DicomStringHelper.GetStringArray(ModalitiesInStudy ?? "");
            ModalitiesInStudy = DicomStringHelper.GetDicomStringArray(
                AppendIfNotIn(modalitiesInStudy, dataSet[DicomTags.Modality].GetString(0, "")));

            string[] sopClassesInStudy = DicomStringHelper.GetStringArray(SopClassesInStudy ?? "");
            SopClassesInStudy = DicomStringHelper.GetDicomStringArray(
                AppendIfNotIn(sopClassesInStudy, dataSet[DicomTags.SopClassUid].GetString(0, "")));

            string[] stationNamesInStudy = DicomStringHelper.GetStringArray(StationNamesInStudy ?? "");
            StationNamesInStudy = DicomStringHelper.GetDicomStringArray(
                AppendIfNotIn(stationNamesInStudy, dataSet[DicomTags.StationName].GetString(0, "")));

            string[] institutionNamesInStudy = DicomStringHelper.GetStringArray(InstitutionNamesInStudy ?? "");
            InstitutionNamesInStudy = DicomStringHelper.GetDicomStringArray(
                AppendIfNotIn(institutionNamesInStudy, dataSet[DicomTags.InstitutionName].GetString(0, "")));

            #region Meta Info

            string[] sourceAETitlesInStudy = DicomStringHelper.GetStringArray(SourceAETitlesInStudy ?? "");
            SourceAETitlesInStudy = DicomStringHelper.GetDicomStringArray(
                AppendIfNotIn(sourceAETitlesInStudy, dicomMessage.MetaInfo[DicomTags.SourceApplicationEntityTitle].GetString(0, "")));

            #endregion
        }

        #endregion
        #region Private Properties

        internal StudyLocation StudyLocation
        {
            get
            {
                Platform.CheckMemberIsSet(StudyInstanceUid, "StudyInstanceUid");
                return _studyLocation ?? (_studyLocation = new StudyLocation(StudyInstanceUid));
            }
        }

        private StudyXml StudyXml
        {
            get { return _studyXml ?? (_studyXml = StudyLocation.LoadStudyXml()); }
        }

        private IList<ISeries> Series
        {
            get
            {
                if (_series == null)
                {
                    _series = new List<ISeries>();
                    foreach (SeriesXml seriesXml in StudyXml)
                        _series.Add(new Series(this, seriesXml));
                }

                return _series;
            }
        }

        #endregion

        #region Private/Internal Methods

        internal void Initialize(StudyXml studyXml)
        {
            Platform.CheckForNullReference(studyXml, "studyXml");
            _studyXml = studyXml;

            var file = new DicomFile(null, new DicomAttributeCollection(), _studyXml.First().First().Collection);
            Update(file);
            //these have to be here, rather than in Initialize b/c they are 
            // computed from the series, which are parsed from the xml.
            NumberOfStudyRelatedSeries = _studyXml.NumberOfStudyRelatedSeries;
            NumberOfStudyRelatedInstances = _studyXml.NumberOfStudyRelatedInstances;
        }

        private static IEnumerable<string> AppendIfNotIn(IEnumerable<string> values, string candidate)
        {
            foreach (string value in values)
            {
                if (value == candidate)
                    candidate = null;

                yield return value;
            }

            if (candidate != null)
                yield return candidate;
        }

        #endregion

        public StudyEntry ToStoreEntry()
        {
            var entry = new StudyEntry
            {
                Study = new StudyRootStudyIdentifier(this)
                {
                    InstanceAvailability = "ONLINE",
                    RetrieveAE = ServerDirectory.GetLocalServer(),
                    SpecificCharacterSet = SpecificCharacterSet
                },
                Data = new StudyEntryData
                {
                    DeleteTime = DeleteTime,
                    InstitutionNamesInStudy = DicomStringHelper.GetStringArray(InstitutionNamesInStudy),
                    SourceAETitlesInStudy = DicomStringHelper.GetStringArray(SourceAETitlesInStudy),
                    StationNamesInStudy = DicomStringHelper.GetStringArray(StationNamesInStudy),
                    StoreTime = StoreTime
                }
            };

            return entry;
        }
    }
}
