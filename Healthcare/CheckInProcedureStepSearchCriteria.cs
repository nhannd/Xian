using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare 
{
	public partial class CheckInProcedureStepSearchCriteria : ProcedureStepSearchCriteria
	{
		public CheckInProcedureStepSearchCriteria()
		{

		}
	
        public CheckInProcedureStepSearchCriteria(EntityRef cpsRef)
            : base(cpsRef)
        {

        }
	}
}
