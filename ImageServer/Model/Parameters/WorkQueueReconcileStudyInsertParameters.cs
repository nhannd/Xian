using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class WorkQueueReconcileStudyInsertParameters: ProcedureParameters
    {
        public WorkQueueReconcileStudyInsertParameters()
            : base("InsertWorkQueueReconcileStudy")
        { }

        public string StudyInstanceUid
        {
            set { this.SubCriteria["StudyInstanceUid"] = new ProcedureParameter<string>("StudyInstanceUid", value); }
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { this.SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }

        public ServerEntityKey StudyStorageKey
        {
            set { this.SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public ServerEntityKey StudyHistoryKey
        {
            set { this.SubCriteria["StudyHistoryKey"] = new ProcedureParameter<ServerEntityKey>("StudyHistoryKey", value); }
        }

        public XmlDocument WorkQueueData
        {
            set
            {
                this.SubCriteria["Data"] = new ProcedureParameter<XmlDocument>("Data", value);
            }
        }

        public DateTime ExpirationTime
        {
            set { this.SubCriteria["ExpirationTime"] = new ProcedureParameter<DateTime>("ExpirationTime", value); }
        }

        public DateTime ScheduledTime
        {
            set { this.SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }

        public string SeriesInstanceUid
        {
            set { this.SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<string>("SeriesInstanceUid", value); }
        }

        public string SopInstanceUid
        {
            set { this.SubCriteria["SopInstanceUid"] = new ProcedureParameter<string>("SopInstanceUid", value); }
        }

        public ServerEnum Priority
        {
            set { SubCriteria["WorkQueuePriorityEnum"] = new ProcedureParameter<ServerEnum>("WorkQueuePriorityEnum", value); }
        }
        
    }
}
