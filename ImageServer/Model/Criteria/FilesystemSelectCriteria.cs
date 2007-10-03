using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Criteria
{
    public class FilesystemSelectCriteria : SelectCriteria
    {
        public FilesystemSelectCriteria()
            : base("Filesystem")
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

        public ISearchCondition<bool> Enabled
        {
            get
            {
                if (!SubCriteria.ContainsKey("Enabled"))
                {
                    SubCriteria["Enabled"] = new SearchCondition<bool>("Enabled");
                }
                return (ISearchCondition<bool>)SubCriteria["Enabled"];
            }
        }
    }
}
