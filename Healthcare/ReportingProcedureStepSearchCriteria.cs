using System;
using System.Collections.Generic;
using System.Text;

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
   }
}
