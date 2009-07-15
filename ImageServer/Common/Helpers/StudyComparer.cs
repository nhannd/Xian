#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.Generic;
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