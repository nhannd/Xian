using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Workflow
{
    public class PerformedStepSearchCriteria : EntitySearchCriteria
    {
 		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public PerformedStepSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
        public PerformedStepSearchCriteria(string key)
			:base(key)
		{
		}

        public ISearchCondition<PerformedStepStatus> Status
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Status"))
                {
                    this.SubCriteria["Status"] = new SearchCondition<PerformedStepStatus>("Status");
                }
                return (ISearchCondition<PerformedStepStatus>)this.SubCriteria["Status"];
            }
        }

        public ISearchCondition<DateTime?> StartTime
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("StartTime"))
                {
                    this.SubCriteria["StartTime"] = new SearchCondition<DateTime?>("StartTime");
                }
                return (ISearchCondition<DateTime?>)this.SubCriteria["StartTime"];
            }
        }

        public ISearchCondition<DateTime?> EndTime
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("EndTime"))
                {
                    this.SubCriteria["EndTime"] = new SearchCondition<DateTime?>("EndTime");
                }
                return (ISearchCondition<DateTime?>)this.SubCriteria["EndTime"];
            }
        }
    }
}
