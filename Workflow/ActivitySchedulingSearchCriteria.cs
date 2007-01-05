using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// Search criteria for <see cref="ActivityScheduling"/> entity
    /// This file is machine generated - changes will be lost.
    /// </summary>
	public partial class ActivitySchedulingSearchCriteria : SearchCriteria
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ActivitySchedulingSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public ActivitySchedulingSearchCriteria(string key)
			:base(key)
		{
		}

		
	
		
	  	public ISearchCondition<DateTime?> StartTime
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("StartTime"))
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
	  			if(!this.SubCriteria.ContainsKey("EndTime"))
	  			{
	  				this.SubCriteria["EndTime"] = new SearchCondition<DateTime?>("EndTime");
	  			}
	  			return (ISearchCondition<DateTime?>)this.SubCriteria["EndTime"];
	  		}
	  	}
	}
}
