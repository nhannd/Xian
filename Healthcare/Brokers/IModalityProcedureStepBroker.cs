using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IModalityProcedureStepBroker : IEntityBroker<ModalityProcedureStep, ModalityProcedureStepSearchCriteria>
    {
        void LoadTypeForModalityProcedureStep(ModalityProcedureStep mps);
    }
}
