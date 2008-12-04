// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication
{

    /// <summary>
    /// Search criteria for <see cref="User"/> class.
    /// </summary>
	public partial class UserSearchCriteria : EntitySearchCriteria<User>
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public UserSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public UserSearchCriteria(string key)
			:base(key)
		{
		}

		
	  	public ISearchCondition<string> UserName
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("UserName"))
	  			{
	  				this.SubCriteria["UserName"] = new SearchCondition<string>("UserName");
	  			}
	  			return (ISearchCondition<string>)this.SubCriteria["UserName"];
	  		}
	  	}
	  	
	  	public ClearCanvas.Enterprise.Authentication.PasswordSearchCriteria Password
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Password"))
	  			{
	  				this.SubCriteria["Password"] = new ClearCanvas.Enterprise.Authentication.PasswordSearchCriteria("Password");
	  			}
	  			return (ClearCanvas.Enterprise.Authentication.PasswordSearchCriteria)this.SubCriteria["Password"];
	  		}
	  	}
	  	
	  	public ISearchCondition<string> DisplayName
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("DisplayName"))
	  			{
	  				this.SubCriteria["DisplayName"] = new SearchCondition<string>("DisplayName");
	  			}
	  			return (ISearchCondition<string>)this.SubCriteria["DisplayName"];
	  		}
	  	}
	  	
	  	public ISearchCondition<DateTime?> ValidFrom
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("ValidFrom"))
	  			{
	  				this.SubCriteria["ValidFrom"] = new SearchCondition<DateTime?>("ValidFrom");
	  			}
	  			return (ISearchCondition<DateTime?>)this.SubCriteria["ValidFrom"];
	  		}
	  	}
	  	
	  	public ISearchCondition<DateTime?> ValidUntil
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("ValidUntil"))
	  			{
	  				this.SubCriteria["ValidUntil"] = new SearchCondition<DateTime?>("ValidUntil");
	  			}
	  			return (ISearchCondition<DateTime?>)this.SubCriteria["ValidUntil"];
	  		}
	  	}
	  	
	  	public ISearchCondition<bool> Enabled
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Enabled"))
	  			{
	  				this.SubCriteria["Enabled"] = new SearchCondition<bool>("Enabled");
	  			}
	  			return (ISearchCondition<bool>)this.SubCriteria["Enabled"];
	  		}
	  	}
	  	
	  	public ISearchCondition<DateTime> CreationTime
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("CreationTime"))
	  			{
	  				this.SubCriteria["CreationTime"] = new SearchCondition<DateTime>("CreationTime");
	  			}
	  			return (ISearchCondition<DateTime>)this.SubCriteria["CreationTime"];
	  		}
	  	}
	  	
	  	public ISearchCondition<DateTime?> LastLoginTime
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("LastLoginTime"))
	  			{
	  				this.SubCriteria["LastLoginTime"] = new SearchCondition<DateTime?>("LastLoginTime");
	  			}
	  			return (ISearchCondition<DateTime?>)this.SubCriteria["LastLoginTime"];
	  		}
	  	}
	  	
	  	public ClearCanvas.Enterprise.Authentication.UserSessionSearchCriteria CurrentSession
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("CurrentSession"))
	  			{
	  				this.SubCriteria["CurrentSession"] = new ClearCanvas.Enterprise.Authentication.UserSessionSearchCriteria("CurrentSession");
	  			}
	  			return (ClearCanvas.Enterprise.Authentication.UserSessionSearchCriteria)this.SubCriteria["CurrentSession"];
	  		}
	  	}
	  	
	}
}
