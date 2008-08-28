using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class InsertReconcileQueueParameters : ProcedureParameters
    {
        public InsertReconcileQueueParameters()
            : base("InsertReconcileQueue")
        {
           
        }

        public ServerEntityKey FilesystemKey
        {
            set { SubCriteria["FilesystemKey"] = new ProcedureParameter<ServerEntityKey>("FilesystemKey", value); }
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public string StudyInstanceUid
        {
            set { SubCriteria["StudyInstanceUid"] = new ProcedureParameter<string>("StudyInstanceUid", value); }
        }

        public string SeriesInstanceUid
        {
            set { SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<string>("SeriesInstanceUid", value); }
        }

        public string SopInstanceUid
        {
            set { SubCriteria["SopInstanceUid"] = new ProcedureParameter<string>("SopInstanceUid", value); }
        }

        public string PatientId
        {
            set { SubCriteria["PatientId"] = new ProcedureParameter<string>("PatientId", value); }
        }

        public string PatientsName
        {
            set { SubCriteria["PatientsName"] = new ProcedureParameter<string>("PatientsName", value); }
        }

        public string IssuerOfPatientId
        {
            set { SubCriteria["IssuerOfPatientId"] = new ProcedureParameter<string>("IssuerOfPatientId", value); }
        }

        public string PatientsBirthDate
        {
            set { SubCriteria["PatientsBirthDate"] = new ProcedureParameter<string>("PatientsBirthDate", value); }
        }

        public string PatientsSex
        {
            set { SubCriteria["PatientsSex"] = new ProcedureParameter<string>("PatientsSex", value); }
        }

        public string AccessionNumber
        {
            set { SubCriteria["AccessionNumber"] = new ProcedureParameter<string>("AccessionNumber", value); }
        }

    }
}
