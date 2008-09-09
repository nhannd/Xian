using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class InsertReconcileQueueParameters: ProcedureParameters
    {
        public InsertReconcileQueueParameters()
            : base("InsertReconcileQueue")
        {

        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }
        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }

        public String SeriesInstanceUid
        {
            set { SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<String>("SeriesInstanceUid", value); }
        }

        public String SopInstanceUid 
        {
            set { SubCriteria["SopInstanceUid"] = new ProcedureParameter<String>("SopInstanceUid", value); }
        }

        
        public XmlDocument StudyData
        {
            set { SubCriteria["StudyData"] = new ProcedureParameter<XmlDocument>("StudyData", value); }
        }

        public XmlDocument QueueData
        {
            set { SubCriteria["QueueData"] = new ProcedureParameter<XmlDocument>("QueueData", value); }
        }


        public ServerEnum ReconcileReasonEnum
        {
            set { SubCriteria["ReconcileReasonEnum"] = new ProcedureParameter<ServerEnum>("ReconcileReasonEnum", value); }
        }
        
    }
}
