using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare
{
    public class ModalityProcedureStepSearchCriteria : ProcedureStepSearchCriteria
    {
        public ModalityProcedureStepSearchCriteria()
        {

        }

        public ModalityProcedureStepSearchCriteria(EntityRef<ModalityProcedureStep> mpsRef)
            :base(mpsRef.Cast<ProcedureStep>())
        {

        }
    }
}
