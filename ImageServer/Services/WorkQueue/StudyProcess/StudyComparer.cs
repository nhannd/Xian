using System;
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    
    class StudyCompareOptions
    {
        private bool _matchIssuerOfPatientId;
        private bool _matchPatientId;
        private bool _matchPatientsName;
        private bool _matchPatientsBirthDate;
        private bool _matchPatientsSex;
        private bool _matchAccessionNumber;

        public bool MatchIssuerOfPatientId
        {
            get { return _matchIssuerOfPatientId; }
            set { _matchIssuerOfPatientId = value; }
        }

        public bool MatchPatientId
        {
            get { return _matchPatientId; }
            set { _matchPatientId = value; }
        }

        public bool MatchPatientsName
        {
            get { return _matchPatientsName; }
            set { _matchPatientsName = value; }
        }

        public bool MatchPatientsBirthDate
        {
            get { return _matchPatientsBirthDate; }
            set { _matchPatientsBirthDate = value; }
        }

        public bool MatchPatientsSex
        {
            get { return _matchPatientsSex; }
            set { _matchPatientsSex = value; }
        }

        public bool MatchAccessionNumber
        {
            get { return _matchAccessionNumber; }
            set { _matchAccessionNumber = value; }
        }
    }

    class Difference
    {
        public string Description;
        public string ExpectValue;
        public string RealValue;

        public Difference(string description, string expected, string actual)
        {
            Description = description;
            ExpectValue = expected;
            RealValue = actual;
        }
    }

    class StudyComparer
    {
        void InternalCompare(string name, string expected, string actual, List<Difference> list)
        {
            if(!StringUtils.AreEqual(expected, actual))
                list.Add(new Difference(name, expected, actual));
        }

        public IList<Difference> Compare(DicomMessageBase message, Study study, StudyCompareOptions options)
        {
            List<Difference> list = new List<Difference>();

            if (options.MatchIssuerOfPatientId)
            {
                InternalCompare("Issuer Of Patient Id", study.IssuerOfPatientId,
                                message.DataSet[DicomTags.IssuerOfPatientId].ToString(), list);
            }

            if (options.MatchPatientId)
            {
                InternalCompare("Patient Id", study.PatientId,
                                message.DataSet[DicomTags.PatientId].ToString(), list);
            }


            if (options.MatchPatientsName)
            {
                InternalCompare("Patient's Name", study.PatientsName,
                                 message.DataSet[DicomTags.PatientsName].ToString(), list);
            }

            if (options.MatchPatientsBirthDate)
            {
                InternalCompare("Patient's BirthDate", study.PatientsBirthDate,
                                 message.DataSet[DicomTags.PatientsBirthDate].ToString(), list);
            }

            if (options.MatchPatientsSex)
            {
                InternalCompare("Patient's Sex", study.PatientsSex,
                                message.DataSet[DicomTags.PatientsSex].ToString(), list);
            }


            if (options.MatchAccessionNumber)
            {
                InternalCompare("Accession Number", study.AccessionNumber,
                                message.DataSet[DicomTags.AccessionNumber].ToString(), list);
            }

            return list;
        }
    }
}