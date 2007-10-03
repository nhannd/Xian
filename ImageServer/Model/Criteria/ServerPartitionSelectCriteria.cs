using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Criteria
{
    public class ServerPartitionSelectCriteria : SelectCriteria
    {
        public ServerPartitionSelectCriteria()
            : base("ServerPartition")
        {}


        public ISearchCondition<ServerEntityKey> Key
        {
            get
            {
                if (!SubCriteria.ContainsKey("Key"))
                {
                    SubCriteria["Key"] = new SearchCondition<ServerEntityKey>("Key");
                }
                return (ISearchCondition<ServerEntityKey>)SubCriteria["Key"];
            }
        }
    }
}
