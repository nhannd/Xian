using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// Search criteria for <see cref="ProcedureStepPerformer"/> entity
    /// This file is machine generated - changes will be lost.
    /// </summary>
	public partial class ProcedureStepPerformerSearchCriteria : SearchCriteria
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ProcedureStepPerformerSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public ProcedureStepPerformerSearchCriteria(string key)
			:base(key)
		{
		}

		
	
		
	  	public ISearchCondition<Staff> Staff
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Staff"))
	  			{
	  				this.SubCriteria["Staff"] = new SearchCondition<Staff>("Staff");
	  			}
	  			return (ISearchCondition<Staff>)this.SubCriteria["Staff"];
	  		}
	  	}
	  	
	}
}
