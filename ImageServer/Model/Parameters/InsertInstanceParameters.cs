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

using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class InsertInstanceParameters : ProcedureParameters
    {
        public InsertInstanceParameters()
            : base("InsertInstance")
        {
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }

        [DicomField(DicomTags.PatientId, DefaultValue = DicomFieldDefault.Null)]
        public string PatientId
        {
            set { SubCriteria["PatientId"] = new ProcedureParameter<string>("PatientId", value); }
        }

        [DicomField(DicomTags.PatientsName, DefaultValue = DicomFieldDefault.Null)]
        public string PatientsName
        {
            set { SubCriteria["PatientsName"] = new ProcedureParameter<string>("PatientsName", value); }
        }

        [DicomField(DicomTags.IssuerOfPatientId, DefaultValue = DicomFieldDefault.Null)]
        public string IssuerOfPatientId
        {
            set { SubCriteria["IssuerOfPatientId"] = new ProcedureParameter<string>("IssuerOfPatientId", value); }
        }

        [DicomField(DicomTags.StudyInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        public string StudyInstanceUid
        {
            set { SubCriteria["StudyInstanceUid"] = new ProcedureParameter<string>("StudyInstanceUid", value); }
        }

        [DicomField(DicomTags.PatientsBirthDate, DefaultValue = DicomFieldDefault.Null)]
        public string PatientsBirthDate
        {
            set { SubCriteria["PatientsBirthDate"] = new ProcedureParameter<string>("PatientsBirthDate", value); }
        }

        [DicomField(DicomTags.PatientsSex, DefaultValue = DicomFieldDefault.Null)]
        public string PatientsSex
        {
            set { SubCriteria["PatientsSex"] = new ProcedureParameter<string>("PatientsSex", value); }
        }

        [DicomField(DicomTags.StudyDate, DefaultValue = DicomFieldDefault.Null)]
        public string StudyDate
        {
            set { SubCriteria["StudyDate"] = new ProcedureParameter<string>("StudyDate", value); }
        }

        [DicomField(DicomTags.StudyTime, DefaultValue = DicomFieldDefault.Null)]
        public string StudyTime
        {
            set { SubCriteria["StudyTime"] = new ProcedureParameter<string>("StudyTime", value); }
        }

        [DicomField(DicomTags.AccessionNumber, DefaultValue = DicomFieldDefault.Null)]
        public string AccessionNumber
        {
            set { SubCriteria["AccessionNumber"] = new ProcedureParameter<string>("AccessionNumber", value); }
        }

        [DicomField(DicomTags.StudyId, DefaultValue = DicomFieldDefault.Null)]
        public string StudyId
        {
            set { SubCriteria["StudyId"] = new ProcedureParameter<string>("StudyId", value); }
        }

        [DicomField(DicomTags.StudyDescription, DefaultValue = DicomFieldDefault.Null)]
        public string StudyDescription
        {
            set { SubCriteria["StudyDescription"] = new ProcedureParameter<string>("StudyDescription", value); }
        }

        [DicomField(DicomTags.ReferringPhysiciansName, DefaultValue = DicomFieldDefault.Null)]
        public string ReferringPhysiciansName
        {
            set { SubCriteria["ReferringPhysiciansName"] = new ProcedureParameter<string>("ReferringPhysiciansName", value); }
        }

        [DicomField(DicomTags.SeriesInstanceUid, DefaultValue = DicomFieldDefault.Null)]
        public string SeriesInstanceUid
        {
            set { SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<string>("SeriesInstanceUid", value); }
        }

        [DicomField(DicomTags.Modality, DefaultValue = DicomFieldDefault.Null)]
        public string Modality
        {
            set { SubCriteria["Modality"] = new ProcedureParameter<string>("Modality", value); }
        }

        [DicomField(DicomTags.SeriesNumber, DefaultValue = DicomFieldDefault.Null)]
        public string SeriesNumber
        {
            set { SubCriteria["SeriesNumber"] = new ProcedureParameter<string>("SeriesNumber", value); }
        }

        [DicomField(DicomTags.SeriesDescription, DefaultValue = DicomFieldDefault.Null)]
        public string SeriesDescription
        {
            set { SubCriteria["SeriesDescription"] = new ProcedureParameter<string>("SeriesDescription", value); }
        }

        [DicomField(DicomTags.PerformedProcedureStepStartDate, DefaultValue = DicomFieldDefault.Null)]
        public string PerformedProcedureStepStartDate
        {
            set { SubCriteria["PerformedProcedureStepStartDate"] = new ProcedureParameter<string>("PerformedProcedureStepStartDate", value); }
        }

        [DicomField(DicomTags.PerformedProcedureStepStartTime, DefaultValue = DicomFieldDefault.Null)]
        public string PerformedProcedureStepStartTime
        {
            set { SubCriteria["PerformedProcedureStepStartTime"] = new ProcedureParameter<string>("PerformedProcedureStepStartTime", value); }
        }

        [DicomField(DicomTags.SourceApplicationEntityTitle, DefaultValue = DicomFieldDefault.Null)]
        public string SourceApplicationEntityTitle
        {
            set { SubCriteria["SourceApplicationEntityTitle"] = new ProcedureParameter<string>("SourceApplicationEntityTitle", value); }
        }

        [DicomField(DicomTags.SpecificCharacterSet, DefaultValue = DicomFieldDefault.Null)]
        public string SpecificCharacterSet
        {
            set { SubCriteria["SpecificCharacterSet"] = new ProcedureParameter<string>("SpecificCharacterSet", value); }
        }
    }
}
