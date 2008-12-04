// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Configuration
{

    /// <summary>
    /// Search criteria for <see cref="ConfigurationDocument"/> class.
    /// </summary>
	public partial class ConfigurationDocumentSearchCriteria : EntitySearchCriteria<ConfigurationDocument>
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ConfigurationDocumentSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public ConfigurationDocumentSearchCriteria(string key)
			:base(key)
		{
		}

		
	  	public ISearchCondition<string> DocumentName
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("DocumentName"))
	  			{
	  				this.SubCriteria["DocumentName"] = new SearchCondition<string>("DocumentName");
	  			}
	  			return (ISearchCondition<string>)this.SubCriteria["DocumentName"];
	  		}
	  	}
	  	
	  	public ISearchCondition<string> DocumentVersionString
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("DocumentVersionString"))
	  			{
	  				this.SubCriteria["DocumentVersionString"] = new SearchCondition<string>("DocumentVersionString");
	  			}
	  			return (ISearchCondition<string>)this.SubCriteria["DocumentVersionString"];
	  		}
	  	}
	  	
	  	public ISearchCondition<string> User
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("User"))
	  			{
	  				this.SubCriteria["User"] = new SearchCondition<string>("User");
	  			}
	  			return (ISearchCondition<string>)this.SubCriteria["User"];
	  		}
	  	}
	  	
	  	public ISearchCondition<string> InstanceKey
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("InstanceKey"))
	  			{
	  				this.SubCriteria["InstanceKey"] = new SearchCondition<string>("InstanceKey");
	  			}
	  			return (ISearchCondition<string>)this.SubCriteria["InstanceKey"];
	  		}
	  	}
	  	
	  	public ClearCanvas.Enterprise.Configuration.ConfigurationDocumentBodySearchCriteria Body
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Body"))
	  			{
	  				this.SubCriteria["Body"] = new ClearCanvas.Enterprise.Configuration.ConfigurationDocumentBodySearchCriteria("Body");
	  			}
	  			return (ClearCanvas.Enterprise.Configuration.ConfigurationDocumentBodySearchCriteria)this.SubCriteria["Body"];
	  		}
	  	}
	  	
	}
}
