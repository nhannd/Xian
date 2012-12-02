#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
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
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ProcedureStepSearchCriteria(ProcedureStepSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new ProcedureStepSearchCriteria(this);
        }

        public ISearchCondition<string> ProcedureStepId
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ProcedureStepId"))
                {
                    this.SubCriteria["ProcedureStepId"] = new SearchCondition<string>("ProcedureStepId");
                }
                return (ISearchCondition<string>)this.SubCriteria["ProcedureStepId"];
            }
        }
		
        public new ProcedureStepSchedulingSearchCriteria Scheduling
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Scheduling"))
                {
                    this.SubCriteria["Scheduling"] = new ProcedureStepSchedulingSearchCriteria("Scheduling");
                }
                return (ProcedureStepSchedulingSearchCriteria)this.SubCriteria["Scheduling"];
            }
        }

        public ProcedureSearchCriteria Procedure
	  	{
	  		get
	  		{
	  			if(!this.SubCriteria.ContainsKey("Procedure"))
	  			{
                    this.SubCriteria["Procedure"] = new ProcedureSearchCriteria("Procedure");
	  			}
                return (ProcedureSearchCriteria)this.SubCriteria["Procedure"];
	  		}
	  	}

        public ProcedureStepPerformerSearchCriteria Performer
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Performer"))
                {
                    this.SubCriteria["Performer"] = new ProcedureStepPerformerSearchCriteria("Performer");
                }
                return (ProcedureStepPerformerSearchCriteria)this.SubCriteria["Performer"];
            }
        }

	}
}
