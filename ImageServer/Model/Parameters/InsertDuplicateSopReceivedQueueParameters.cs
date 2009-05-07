using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class InsertDuplicateSopReceivedQueueParameters: ProcedureParameters
    {
        public InsertDuplicateSopReceivedQueueParameters()
            : base("InsertDuplicateSopReceivedQueueParameters")
        {

        }
        public String Description
        {
            set { SubCriteria["Description"] = new ProcedureParameter<String>("Description", value); }
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }
        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }

        public String StudyInstanceUid
        {
            set { SubCriteria["StudyInstanceUid"] = new ProcedureParameter<String>("StudyInstanceUid", value); }
        }

        public String SeriesInstanceUid
        {
            set { SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<String>("SeriesInstanceUid", value); }
        }

        public String SopInstanceUid
        {
            set { SubCriteria["SopInstanceUid"] = new ProcedureParameter<String>("SopInstanceUid", value); }
        }

        public String SeriesDescription
        {
            set { SubCriteria["SeriesDescription"] = new ProcedureParameter<String>("SeriesDescription", value); }
        }

        public String GroupID
        {
            set { SubCriteria["GroupID"] = new ProcedureParameter<String>("GroupID", value); }
        }

        public String UidRelativePath
        {
            set { SubCriteria["UidRelativePath"] = new ProcedureParameter<String>("UidRelativePath", value); }
        }
	
        public XmlDocument StudyData
        {
            set { SubCriteria["StudyData"] = new ProcedureParameter<XmlDocument>("StudyData", value); }
        }
        public XmlDocument QueueData
        {
            set { SubCriteria["QueueData"] = new ProcedureParameter<XmlDocument>("QueueData", value); }
        }
    }
}
