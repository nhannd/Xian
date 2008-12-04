// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication
{

    /// <summary>
    /// Search criteria for <see cref="AuthorityGroup"/> class.
    /// </summary>
	public partial class AuthorityGroupSearchCriteria : EntitySearchCriteria<AuthorityGroup>
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public AuthorityGroupSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public AuthorityGroupSearchCriteria(string key)
			:base(key)
		{
		}

		
	  	public ISearchCondition<string> Name
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Name"))
	  			{
	  				this.SubCriteria["Name"] = new SearchCondition<string>("Name");
	  			}
	  			return (ISearchCondition<string>)this.SubCriteria["Name"];
	  		}
	  	}
	  	
	}
}
