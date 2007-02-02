using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// Search criteria for <see cref="ProcedureStep"/> entity
    /// This file is machine generated - changes will be lost.
    /// </summary>
	public partial class ProcedureStepSearchCriteria : ActivitySearchCriteria
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ProcedureStepSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public ProcedureStepSearchCriteria(string key)
			:base(key)
		{
		}

		
		/// <summary>
		/// Constructor to search by EntityRef
		/// </summary>
		public ProcedureStepSearchCriteria(EntityRef<ProcedureStep> entityRef)
		{
			this.SubCriteria["OID"] = new SearchCondition<object>("OID");
            ((ISearchCondition<object>)this.SubCriteria["OID"]).EqualTo(EntityUtils.GetOID(entityRef));
		}
		
	
		
	  	public ISearchCondition<RequestedProcedure> Procedure
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Procedure"))
	  			{
	  				this.SubCriteria["Procedure"] = new SearchCondition<RequestedProcedure>("Procedure");
	  			}
	  			return (ISearchCondition<RequestedProcedure>)this.SubCriteria["Procedure"];
	  		}
	  	}
	}
}
