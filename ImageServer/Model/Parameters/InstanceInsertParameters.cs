using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class InstanceInsertParameters : ProcedureParameters
    {
        public InstanceInsertParameters()
            : base("InstanceInsert")
        {
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { this.SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }

        public StatusEnum StatusEnum
        {
            set { this.SubCriteria["StatusEnum"] = new ProcedureParameter<StatusEnum>("StatusEnum", value); }
        }

        [DicomField(DicomTags.PatientID, DefaultValue = DicomFieldDefault.Null)]
        public string PatientId
        {
            set { this.SubCriteria["PatientId"] = new ProcedureParameter<string>("PatientId", value); }
        }

        [DicomField(DicomTags.PatientsName, DefaultValue = DicomFieldDefault.Null)]
        public string PatientName
        {
            set { this.SubCriteria["PatientName"] = new ProcedureParameter<string>("PatientName", value); }
        }

        [DicomField(DicomTags.IssuerofPatientID, DefaultValue = DicomFieldDefault.Null)]
        public string IssuerOfPatientId
        {
            set { this.SubCriteria["IssuerOfPatientId"] = new ProcedureParameter<string>("IssuerOfPatientId", value); }
        }

        [DicomField(DicomTags.StudyInstanceUID, DefaultValue = DicomFieldDefault.Null)]
        public string StudyInstanceUid
        {
            set { this.SubCriteria["StudyInstanceUid"] = new ProcedureParameter<string>("StudyInstanceUid", value); }
        }

        [DicomField(DicomTags.PatientsBirthDate, DefaultValue = DicomFieldDefault.Null)]
        public string PatientsBirthDate
        {
            set { this.SubCriteria["PatientsBirthDate"] = new ProcedureParameter<string>("PatientsBirthDate", value); }
        }

        [DicomField(DicomTags.PatientsSex, DefaultValue = DicomFieldDefault.Null)]
        public string PatientsSex
        {
            set { this.SubCriteria["PatientsSex"] = new ProcedureParameter<string>("PatientsSex", value); }
        }

        [DicomField(DicomTags.StudyDate, DefaultValue = DicomFieldDefault.Null)]
        public string StudyDate
        {
            set { this.SubCriteria["StudyDate"] = new ProcedureParameter<string>("StudyDate", value); }
        }

        [DicomField(DicomTags.StudyTime, DefaultValue = DicomFieldDefault.Null)]
        public string StudyTime
        {
            set { this.SubCriteria["StudyTime"] = new ProcedureParameter<string>("StudyTime", value); }
        }

        [DicomField(DicomTags.AccessionNumber, DefaultValue = DicomFieldDefault.Null)]
        public string AccessionNumber
        {
            set { this.SubCriteria["AccessionNumber"] = new ProcedureParameter<string>("AccessionNumber", value); }
        }

        [DicomField(DicomTags.StudyID, DefaultValue = DicomFieldDefault.Null)]
        public string StudyId
        {
            set { this.SubCriteria["StudyId"] = new ProcedureParameter<string>("StudyId", value); }
        }

        [DicomField(DicomTags.StudyDescription, DefaultValue = DicomFieldDefault.Null)]
        public string StudyDescription
        {
            set { this.SubCriteria["StudyDescription"] = new ProcedureParameter<string>("StudyDescription", value); }
        }

        [DicomField(DicomTags.ReferringPhysiciansName, DefaultValue = DicomFieldDefault.Null)]
        public string ReferringPhysiciansName
        {
            set { this.SubCriteria["ReferringPhysiciansName"] = new ProcedureParameter<string>("ReferringPhysiciansName", value); }
        }

        [DicomField(DicomTags.SeriesInstanceUID, DefaultValue = DicomFieldDefault.Null)]
        public string SeriesInstanceUid
        {
            set { this.SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<string>("SeriesInstanceUid", value); }
        }

        [DicomField(DicomTags.Modality, DefaultValue = DicomFieldDefault.Null)]
        public string Modality
        {
            set { this.SubCriteria["Modality"] = new ProcedureParameter<string>("Modality", value); }
        }

        [DicomField(DicomTags.SeriesNumber, DefaultValue = DicomFieldDefault.Null)]
        public string SeriesNumber
        {
            set { this.SubCriteria["SeriesNumber"] = new ProcedureParameter<string>("SeriesNumber", value); }
        }

        [DicomField(DicomTags.SeriesDescription, DefaultValue = DicomFieldDefault.Null)]
        public string SeriesDescription
        {
            set { this.SubCriteria["SeriesDescription"] = new ProcedureParameter<string>("SeriesDescription", value); }
        }

        [DicomField(DicomTags.PerformedProcedureStepStartDate, DefaultValue = DicomFieldDefault.Null)]
        public string PerformedProcedureStepStartDate
        {
            set { this.SubCriteria["PerformedProcedureStepStartDate"] = new ProcedureParameter<string>("PerformedProcedureStepStartDate", value); }
        }

        [DicomField(DicomTags.PerformedProcedureStepStartTime, DefaultValue = DicomFieldDefault.Null)]
        public string PerformedProcedureStepStartTime
        {
            set { this.SubCriteria["PerformedProcedureStepStartTime"] = new ProcedureParameter<string>("PerformedProcedureStepStartTime", value); }
        }
    }
}
