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

namespace ClearCanvas.Healthcare
{
    public class ReportingProcedureStepSearchCriteria : ProcedureStepSearchCriteria
    {
 		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ReportingProcedureStepSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
        public ReportingProcedureStepSearchCriteria(string key)
			:base(key)
		{
		}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ReportingProcedureStepSearchCriteria(ReportingProcedureStepSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new ReportingProcedureStepSearchCriteria(this);
        }

        public ReportPartSearchCriteria ReportPart
        {
	  		get
	  		{
                if (!this.SubCriteria.ContainsKey("ReportPart"))
	  			{
                    this.SubCriteria["ReportPart"] = new ReportPartSearchCriteria("ReportPart");
	  			}
                return (ReportPartSearchCriteria)this.SubCriteria["ReportPart"];
	  		}
        }
    }
}
