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
        public string ExpectValue;
        public string RealValue;

        public ComparisionDifference(string description, string expected, string actual)
        {
            Description = description;
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
        static void InternalCompare(string name, string expected, string actual, List<ComparisionDifference> list)
        {
            if (!StringUtils.AreEqual(expected, actual))
                list.Add(new ComparisionDifference(name, expected, actual));
        }

        public DifferenceCollection Compare(DicomMessageBase message, Study study, StudyCompareOptions options)
        {
            DifferenceCollection list = new DifferenceCollection();

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
                                message.DataSet[DicomTags.PatientsBirthDate].ToString(),list);
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

            SimulateUnknownErrors(list);
            return list;
        }

        [Conditional("DEBUG_SIMULATE_ERRORS")]
        private void SimulateUnknownErrors(DifferenceCollection list)
        {
            ServerPlatform.SimulateError("Some unknown difference detected in the image",
                                         delegate()
                                             {
                                                 list.Add(new ComparisionDifference("Fake difference", "12345", "12283"));
                                             });
        }
    }
}