// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Authentication
{

    /// <summary>
    /// Search criteria for <see cref="AuthorityToken"/> class.
    /// </summary>
	public partial class AuthorityTokenSearchCriteria : EntitySearchCriteria<AuthorityToken>
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public AuthorityTokenSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public AuthorityTokenSearchCriteria(string key)
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
	  	
	  	public ISearchCondition<string> Description
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Description"))
	  			{
	  				this.SubCriteria["Description"] = new SearchCondition<string>("Description");
	  			}
	  			return (ISearchCondition<string>)this.SubCriteria["Description"];
	  		}
	  	}
	  	
	}
}
