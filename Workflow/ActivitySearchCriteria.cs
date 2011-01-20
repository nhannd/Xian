#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Workflow
{
    public abstract class ActivitySearchCriteria : EntitySearchCriteria<Activity>
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

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ActivitySearchCriteria(ActivitySearchCriteria other)
            : base(other)
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
	  	
	  	public ISearchCondition<ActivityStatus> State
	  	{
	  		get
	  		{
                if (!this.SubCriteria.ContainsKey("State"))
	  			{
                    this.SubCriteria["State"] = new SearchCondition<ActivityStatus>("State");
	  			}
                return (ISearchCondition<ActivityStatus>)this.SubCriteria["State"];
	  		}
	  	}

        public ISearchCondition<DateTime> CreationTime
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("CreationTime"))
                {
                    this.SubCriteria["CreationTime"] = new SearchCondition<DateTime>("CreationTime");
                }
                return (ISearchCondition<DateTime>)this.SubCriteria["CreationTime"];
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
