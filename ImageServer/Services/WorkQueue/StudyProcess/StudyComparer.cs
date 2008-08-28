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

    class StudyComparer
    {
        public IList<DicomAttribute> Compare(DicomMessageBase message, Study study, StudyCompareOptions options)
        {
            
            List<DicomAttribute> list = new List<DicomAttribute>();


            if (options.MatchIssuerOfPatientId)
            {
                if (!StringUtils.AreEqual(study.IssuerOfPatientId,message.DataSet[DicomTags.IssuerOfPatientId].GetString(0, String.Empty)))
                    list.Add(message.DataSet[DicomTags.IssuerOfPatientId]);
            }

            if (options.MatchPatientId)
            {
                if (!StringUtils.AreEqual(study.PatientId, message.DataSet[DicomTags.PatientId].GetString(0, String.Empty)))
                    list.Add(message.DataSet[DicomTags.PatientId]);
            }


            if (options.MatchPatientsName)
            {
                if (!StringUtils.AreEqual(study.PatientsName, message.DataSet[DicomTags.PatientsName].GetString(0, String.Empty)))
                    list.Add(message.DataSet[DicomTags.PatientsName]);
            }

            if (options.MatchPatientsBirthDate)
            {
                if (!StringUtils.AreEqual(study.PatientsBirthDate, message.DataSet[DicomTags.PatientsBirthDate].GetString(0, String.Empty)))
                    list.Add(message.DataSet[DicomTags.PatientsBirthDate]);
            }

            if (options.MatchPatientsSex)
            {
                if (!StringUtils.AreEqual(study.PatientsSex, message.DataSet[DicomTags.PatientsSex].GetString(0, String.Empty)))
                    list.Add(message.DataSet[DicomTags.PatientsSex]);
            }


            if (options.MatchAccessionNumber)
            {
                if (!StringUtils.AreEqual(study.AccessionNumber, message.DataSet[DicomTags.AccessionNumber].GetString(0, String.Empty)))
                    list.Add(message.DataSet[DicomTags.AccessionNumber]);
            }

            return list;
        }
    }
}