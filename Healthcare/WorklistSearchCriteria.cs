using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public class WorklistSearchCriteria : EntitySearchCriteria
    {
        /// <summary>
        /// Constructor for top-level search criteria (no key required)
        /// </summary>
        public WorklistSearchCriteria()
        {
        }

        /// <summary>
        /// Constructor for sub-criteria (key required)
        /// </summary>
        public WorklistSearchCriteria(string key)
            : base(key)
        {
        }

        public ISearchCondition<string> Name
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Name"))
                {
                    this.SubCriteria["Name"] = new SearchCondition<string>("Name");
                }
                return (ISearchCondition<string>)this.SubCriteria["Name"];
            }
        }
    }
}
