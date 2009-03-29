using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common.Helpers
{
    public class ComparisionDifference
    {
        public string Description;
        public DicomTag   DicomTag;
        public string ExpectValue;
        public string RealValue;

        public ComparisionDifference(uint tag, string expected, string actual)
        {
            DicomTag = DicomTagDictionary.GetDicomTag(tag);
            Description = DicomTag.Name;
            ExpectValue = expected;
            RealValue = actual;
        }
    }

    public class DifferenceCollection : List<ComparisionDifference>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (ComparisionDifference diff in this)
            {
                sb.AppendLine(String.Format("\t{0}: Expected='{1}'    Actual='{2}'", diff.Description, diff.ExpectValue, diff.RealValue));
            }
            return sb.ToString();
        }
    }

    public class StudyComparer
    {
        static void InternalCompare(uint tag, string expected, string actual, List<ComparisionDifference> list)
        {
            if (!StringUtils.AreEqual(expected, actual, StringComparison.InvariantCultureIgnoreCase))
                list.Add(new ComparisionDifference(tag, expected, actual));
        }

        public DifferenceCollection Compare(DicomMessageBase message, Study study, StudyCompareOptions options)
        {
            DifferenceCollection list = new DifferenceCollection();

            if (options.MatchIssuerOfPatientId)
            {
                InternalCompare(DicomTags.IssuerOfPatientId, study.IssuerOfPatientId,
                                message.DataSet[DicomTags.IssuerOfPatientId].ToString(), list);
            }

            if (options.MatchPatientId)
            {
                InternalCompare(DicomTags.PatientId, study.PatientId,
                                message.DataSet[DicomTags.PatientId].ToString(), list);
            }


            if (options.MatchPatientsName)
            {
                InternalCompare(DicomTags.PatientsName, study.PatientsName,
                                message.DataSet[DicomTags.PatientsName].ToString(), list);
            }

            if (options.MatchPatientsBirthDate)
            {
                InternalCompare(DicomTags.PatientsBirthDate, study.PatientsBirthDate,
                                message.DataSet[DicomTags.PatientsBirthDate].ToString(),list);
            }

            if (options.MatchPatientsSex)
            {
                InternalCompare(DicomTags.PatientsSex, study.PatientsSex,
                                message.DataSet[DicomTags.PatientsSex].ToString(), list);
            }


            if (options.MatchAccessionNumber)
            {
                InternalCompare(DicomTags.AccessionNumber, study.AccessionNumber,
                                message.DataSet[DicomTags.AccessionNumber].ToString(), list);
            }

            return list;
        }

    }
}