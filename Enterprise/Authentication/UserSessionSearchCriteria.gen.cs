// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication
{

    /// <summary>
    /// Search criteria for <see cref="UserSession"/> class.
    /// </summary>
	public partial class UserSessionSearchCriteria : EntitySearchCriteria<UserSession>
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public UserSessionSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public UserSessionSearchCriteria(string key)
			:base(key)
		{
		}

		
	  	public ISearchCondition<string> SessionId
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("SessionId"))
	  			{
	  				this.SubCriteria["SessionId"] = new SearchCondition<string>("SessionId");
	  			}
	  			return (ISearchCondition<string>)this.SubCriteria["SessionId"];
	  		}
	  	}
	  	
	  	public ISearchCondition<DateTime> ExpiryTime
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("ExpiryTime"))
	  			{
	  				this.SubCriteria["ExpiryTime"] = new SearchCondition<DateTime>("ExpiryTime");
	  			}
	  			return (ISearchCondition<DateTime>)this.SubCriteria["ExpiryTime"];
	  		}
	  	}
	  	
	}
}
