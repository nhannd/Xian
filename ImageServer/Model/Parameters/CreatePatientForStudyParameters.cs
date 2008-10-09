using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class CreatePatientForStudyParameters : ProcedureParameters
    {
        public CreatePatientForStudyParameters()
            : base("CreatePatientForStudy")
        {
        }

        public ServerEntityKey StudyKey
        {
            set { SubCriteria["StudyKey"] = new ProcedureParameter<ServerEntityKey>("StudyKey", value); }
        }

        [DicomField(DicomTags.PatientsName)]
        public string PatientsName
        {
            set { SubCriteria["PatientsName"] = new ProcedureParameter<string>("PatientsName", value); }
        }

        [DicomField(DicomTags.PatientId)]
        public string PatientId
        {
            set { SubCriteria["PatientId"] = new ProcedureParameter<string>("PatientId", value); }
        }

        [DicomField(DicomTags.IssuerOfPatientId)]
        public string IssuerOfPatientId
        {
            set { SubCriteria["IssuerOfPatientId"] = new ProcedureParameter<string>("IssuerOfPatientId", value); }
        }

        [DicomField(DicomTags.SpecificCharacterSet)]
        public string SpecificCharacterSet
        {
            set { SubCriteria["SpecificCharacterSet"] = new ProcedureParameter<string>("SpecificCharacterSet", value); }
        }

    }
}
