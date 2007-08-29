using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Criteria
{
    public class SeriesSelectCriteria : SelectCriteria
    {
        public SeriesSelectCriteria()
            : base()
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
        public ISearchCondition<ServerEntityKey> StudyKey
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StudyKey"))
                {
                    this.SubCriteria["StudyKey"] = new SearchCondition<ServerEntityKey>("StudyKey");
                }
                return (ISearchCondition<ServerEntityKey>)this.SubCriteria["StudyKey"];
            }
        }

        [DicomField(DicomTags.SeriesInstanceUID, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> SeriesInstanceUid
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("SeriesInstanceUid"))
                {
                    this.SubCriteria["SeriesInstanceUid"] = new SearchCondition<string>("SeriesInstanceUid");
                }
                return (ISearchCondition<string>)this.SubCriteria["SeriesInstanceUid"];
            }
        }

        [DicomField(DicomTags.Modality, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> Modality
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Modality"))
                {
                    this.SubCriteria["Modality"] = new SearchCondition<string>("Modality");
                }
                return (ISearchCondition<string>)this.SubCriteria["Modality"];
            }
        }

        [DicomField(DicomTags.SeriesNumber, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> SeriesNumber
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("SeriesNumber"))
                {
                    this.SubCriteria["SeriesNumber"] = new SearchCondition<string>("SeriesNumber");
                }
                return (ISearchCondition<string>)this.SubCriteria["SeriesNumber"];
            }
        }

        [DicomField(DicomTags.SeriesDescription, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> SeriesDescription
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("SeriesDescription"))
                {
                    this.SubCriteria["SeriesDescription"] = new SearchCondition<string>("SeriesDescription");
                }
                return (ISearchCondition<string>)this.SubCriteria["SeriesDescription"];
            }
        }
        [DicomField(DicomTags.PerformedProcedureStepStartDate, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PerformedProcedureStepStartDate
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PerformedProcedureStepStartDate"))
                {
                    this.SubCriteria["PerformedProcedureStepStartDate"] = new SearchCondition<string>("PerformedProcedureStepStartDate");
                }
                return (ISearchCondition<string>)this.SubCriteria["PerformedProcedureStepStartDate"];
            }
        }
        [DicomField(DicomTags.PerformedProcedureStepStartTime, DefaultValue = DicomFieldDefault.Null)]
        public ISearchCondition<string> PerformedProcedureStepStartTime
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PerformedProcedureStepStartTime"))
                {
                    this.SubCriteria["PerformedProcedureStepStartTime"] = new SearchCondition<string>("PerformedProcedureStepStartTime");
                }
                return (ISearchCondition<string>)this.SubCriteria["PerformedProcedureStepStartTime"];
            }
        }
    }
}
