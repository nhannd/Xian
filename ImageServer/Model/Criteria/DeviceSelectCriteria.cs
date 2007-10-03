using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Criteria
{
    public class DeviceSelectCriteria: SelectCriteria
    {
        public DeviceSelectCriteria()
            : base("Device")
        {}

        public ISearchCondition<ServerEntityKey> ServerPartitionKey
        {
            get
            {
                if (!SubCriteria.ContainsKey("ServerPartitionKey"))
                {
                    SubCriteria["ServerPartitionKey"] = new SearchCondition<ServerEntityKey>("ServerPartitionKey");
                }
                return (ISearchCondition<ServerEntityKey>)SubCriteria["ServerPartitionKey"];
            } 
        }

        public  ISearchCondition<string> AeTitle
        {
            get
            {
                if (!SubCriteria.ContainsKey("AeTitle"))
                {
                    SubCriteria["AeTitle"] = new SearchCondition<string>("AeTitle");
                }
                return (ISearchCondition<string>)SubCriteria["AeTitle"];
            } 
        }

        public ISearchCondition<bool> Dhcp
        {
            get
            {
                if (!SubCriteria.ContainsKey("Dhcp"))
                {
                    SubCriteria["Dhcp"] = new SearchCondition<bool>("Dhcp");
                }
                return (ISearchCondition<bool>)SubCriteria["Dhcp"];
            }
        }

        public ISearchCondition<bool> Active
        {
            get
            {
                if (!SubCriteria.ContainsKey("Active"))
                {
                    SubCriteria["Active"] = new SearchCondition<bool>("Active");
                }
                return (ISearchCondition<bool>)SubCriteria["Active"];
            }
        }
    
    }
}
