using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Criteria
{
    /// <summary>
    /// Criteria for selects against the <see cref="Patient"/> table.
    /// </summary>
    public class PatientSelectCriteria: SelectCriteria
    {
        public PatientSelectCriteria()
            : base("Patient")
        {}

        public ISearchCondition<ServerEntityKey> ServerPartitionKey
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ServerPartitionKey"))
                {
                    this.SubCriteria["ServerPartitionKey"] = new SearchCondition<ServerEntityKey>("ServerPartitionKey");
                }
                return (ISearchCondition<ServerEntityKey>)this.SubCriteria["ServerPartitionKey"];
            }
        }

        [DicomField(DicomTags.PatientsName, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PatientName
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientName"))
                {
                    this.SubCriteria["PatientName"] = new SearchCondition<string>("PatientName");
                }
                return (ISearchCondition<string>)this.SubCriteria["PatientName"];
            }
        }

        [DicomField(DicomTags.PatientId, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PatientId
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientId"))
                {
                    this.SubCriteria["PatientId"] = new SearchCondition<string>("PatientId");
                }
                return (ISearchCondition<string>)this.SubCriteria["PatientId"];
            }
        }

        [DicomField(DicomTags.IssuerOfPatientId, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> IssuerOfPatientId
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("IssuerOfPatientId"))
                {
                    this.SubCriteria["IssuerOfPatientId"] = new SearchCondition<string>("IssuerOfPatientId");
                }
                return (ISearchCondition<string>)this.SubCriteria["IssuerOfPatientId"];
            }
        }
    }
}
