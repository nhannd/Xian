// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication
{

    /// <summary>
    /// Search criteria for <see cref="Password"/> class.
    /// </summary>
	public partial class PasswordSearchCriteria : SearchCriteria
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public PasswordSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public PasswordSearchCriteria(string key)
			:base(key)
		{
		}

		
	  	public ISearchCondition<string> Salt
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Salt"))
	  			{
	  				this.SubCriteria["Salt"] = new SearchCondition<string>("Salt");
	  			}
	  			return (ISearchCondition<string>)this.SubCriteria["Salt"];
	  		}
	  	}
	  	
	  	public ISearchCondition<string> SaltedHash
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("SaltedHash"))
	  			{
	  				this.SubCriteria["SaltedHash"] = new SearchCondition<string>("SaltedHash");
	  			}
	  			return (ISearchCondition<string>)this.SubCriteria["SaltedHash"];
	  		}
	  	}
	  	
	  	public ISearchCondition<DateTime?> ExpiryTime
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("ExpiryTime"))
	  			{
	  				this.SubCriteria["ExpiryTime"] = new SearchCondition<DateTime?>("ExpiryTime");
	  			}
	  			return (ISearchCondition<DateTime?>)this.SubCriteria["ExpiryTime"];
	  		}
	  	}
	  	
	}
}
