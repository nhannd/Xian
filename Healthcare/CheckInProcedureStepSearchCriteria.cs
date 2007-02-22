using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare 
{
	public partial class CheckInProcedureStepSearchCriteria : ProcedureStepSearchCriteria
	{
		public CheckInProcedureStepSearchCriteria()
		{

		}
	
        public CheckInProcedureStepSearchCriteria(EntityRef<CheckInProcedureStep> cpsRef)
            : base(cpsRef.Cast<ProcedureStep>())
        {

        }
	}
}
