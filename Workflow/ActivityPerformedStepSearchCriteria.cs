using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Workflow
{
    public class ActivityPerformedStepSearchCriteria : EntitySearchCriteria
    {
 		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ActivityPerformedStepSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
        public ActivityPerformedStepSearchCriteria(string key)
			:base(key)
		{
		}

        public ISearchCondition<ActivityPerformedStepStatus> Status
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Status"))
                {
                    this.SubCriteria["Status"] = new SearchCondition<ActivityPerformedStepStatus>("Status");
                }
                return (ISearchCondition<ActivityPerformedStepStatus>)this.SubCriteria["Status"];
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
