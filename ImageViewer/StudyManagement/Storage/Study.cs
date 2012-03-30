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
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
    public partial class Study : IStudy
    {
        private List<ISeries> _series;
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

        public void Initialize(DicomMessageBase message)
        {
            Initialize(message.DataSet);
        }

        public void Initialize(DicomAttributeCollection dataSet)
        {
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
            ResponsiblePersonRaw = DicomImplementation.CharacterParser.EncodeAsIsomorphicString(ResponsiblePerson, dataSet.SpecificCharacterSet);

            attribute = dataSet[DicomTags.ResponsiblePersonRole];
            ResponsiblePersonRole = attribute.ToString();

            attribute = dataSet[DicomTags.ResponsibleOrganization];
            ResponsibleOrganization = attribute.ToString();

            attribute = dataSet[DicomTags.SpecificCharacterSet];
            SpecificCharacterSet = attribute.ToString();

            string[] modalitiesInStudy = DicomStringHelper.GetStringArray(ModalitiesInStudy ?? "");
            ModalitiesInStudy = DicomStringHelper.GetDicomStringArray(
                ComputeModalitiesInStudy(modalitiesInStudy, dataSet[DicomTags.Modality].GetString(0, "")));
        }

        #endregion
        #region Private Properties

        private StudyXml StudyXml
        {
            get
            {
                LoadStudyXml(true);
                return _studyXml;
            }
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

        internal void Update(DicomFile file)
        {
            Initialize(file);

            LoadStudyXml(false);
            _studyXml.AddFile(file);

            //these have to be here, rather than in Initialize b/c they are 
            // computed from the series, which are parsed from the xml.
            NumberOfStudyRelatedSeries = _studyXml.NumberOfStudyRelatedSeries;
            NumberOfStudyRelatedInstances = _studyXml.NumberOfStudyRelatedInstances;
        }

        private void LoadStudyXml(bool throwIfNotExists)
        {
            if (_studyXml != null)
                return;

            if (StudyXmlUri == null)
                throw new Exception("The study xml location must be set.");

            var doc = new XmlDocument();
            _studyXml = new StudyXml(StudyInstanceUid);

            if (File.Exists(StudyXmlUri))
            {
                using (FileStream stream = FileStreamOpener.OpenForRead(StudyXmlUri, FileMode.Open, 5000))
                {
                    StudyXmlIo.Read(doc, stream);
                    _studyXml.SetMemento(doc);
                }
            }
            else if (throwIfNotExists)
            {
                throw new FileNotFoundException("The study xml file could not be found", StudyXmlUri);
            }
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

        #endregion
    }
}
