using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare
{
    public class ModalityProcedureStepSearchCriteria : ProcedureStepSearchCriteria
    {
        public ModalityProcedureStepSearchCriteria()
        {

        }

        public ModalityProcedureStepSearchCriteria(EntityRef mpsRef)
            :base(mpsRef)
        {

        }
    }
}
