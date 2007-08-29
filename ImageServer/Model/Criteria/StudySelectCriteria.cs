using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Criteria
{
    public class StudySelectCriteria : SelectCriteria
    {
        public StudySelectCriteria() : base()
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

        public ISearchCondition<ServerEntityKey> PatientKey
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientKey"))
                {
                    this.SubCriteria["PatientKey"] = new SearchCondition<ServerEntityKey>("PatientKey");
                }
                return (ISearchCondition<ServerEntityKey>)this.SubCriteria["PatientKey"];
            }
        }

        [DicomField(DicomTags.StudyInstanceUID, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> StudyInstanceUid
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyInstanceUid"))
                {
                    this.SubCriteria["StudyInstanceUid"] = new SearchCondition<string>("StudyInstanceUid");
                }
                return (ISearchCondition<string>)this.SubCriteria["StudyInstanceUid"];
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

        [DicomField(DicomTags.PatientID, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PatientId
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientId"))
                {
                    this.SubCriteria["PatientId"] = new SearchCondition<string>("PatientId");
                }
                return (ISearchCondition<string>) this.SubCriteria["PatientId"];
            }
        }

        [DicomField(DicomTags.PatientsBirthDate, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PatientsBirthDate
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientsBirthDate"))
                {
                    this.SubCriteria["PatientsBirthDate"] = new SearchCondition<string>("PatientsBirthDate");
                }
                return (ISearchCondition<string>) this.SubCriteria["PatientsBirthDate"];
            }
        }

        [DicomField(DicomTags.PatientsSex, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PatientsSex
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientsSex"))
                {
                    this.SubCriteria["PatientsSex"] = new SearchCondition<string>("PatientsSex");
                }
                return (ISearchCondition<string>)this.SubCriteria["PatientsSex"];
            }
        }

        [DicomField(DicomTags.StudyDate, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> StudyDate
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyDate"))
                {
                    this.SubCriteria["StudyDate"] = new SearchCondition<string>("StudyDate");
                }
                return (ISearchCondition<string>)this.SubCriteria["StudyDate"];
            }
        }

        [DicomField(DicomTags.StudyTime, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> StudyTime
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyTime"))
                {
                    this.SubCriteria["StudyTime"] = new SearchCondition<string>("StudyTime");
                }
                return (ISearchCondition<string>)this.SubCriteria["StudyTime"];
            }
        }

        [DicomField(DicomTags.AccessionNumber, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> AccessionNumber
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("AccessionNumber"))
                {
                    this.SubCriteria["AccessionNumber"] = new SearchCondition<string>("AccessionNumber");
                }
                return (ISearchCondition<string>)this.SubCriteria["AccessionNumber"];
            }
        }

        [DicomField(DicomTags.StudyID, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> StudyId
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyId"))
                {
                    this.SubCriteria["StudyId"] = new SearchCondition<string>("StudyId");
                }
                return (ISearchCondition<string>)this.SubCriteria["StudyId"];
            }
        }

        [DicomField(DicomTags.StudyDescription, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> StudyDescription
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyDescription"))
                {
                    this.SubCriteria["StudyDescription"] = new SearchCondition<string>("StudyDescription");
                }
                return (ISearchCondition<string>)this.SubCriteria["StudyDescription"];
            }
        }

        [DicomField(DicomTags.ReferringPhysiciansName, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> ReferringPhysiciansName
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ReferringPhysiciansName"))
                {
                    this.SubCriteria["ReferringPhysiciansName"] = new SearchCondition<string>("ReferringPhysiciansName");
                }
                return (ISearchCondition<string>)this.SubCriteria["ReferringPhysiciansName"];
            }
        }
    }
}
