using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Workflow
{
    public abstract class ActivitySearchCriteria : EntitySearchCriteria
    {
 		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ActivitySearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public ActivitySearchCriteria(string key)
			:base(key)
		{
		}

		
	  	public ActivitySchedulingSearchCriteria Scheduling
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Scheduling"))
	  			{
	  				this.SubCriteria["Scheduling"] = new ActivitySchedulingSearchCriteria("Scheduling");
	  			}
	  			return (ActivitySchedulingSearchCriteria)this.SubCriteria["Scheduling"];
	  		}
	  	}
	  	
	  	public ISearchCondition<ActivityStatus> Status
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Status"))
	  			{
	  				this.SubCriteria["Status"] = new SearchCondition<ActivityStatus>("Status");
	  			}
	  			return (ISearchCondition<ActivityStatus>)this.SubCriteria["Status"];
	  		}
	  	}
   }
}
