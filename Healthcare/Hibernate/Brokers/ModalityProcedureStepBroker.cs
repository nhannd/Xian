using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class ModalityProcedureStepBroker : EntityBroker<ModalityProcedureStep, ModalityProcedureStepSearchCriteria>, IModalityProcedureStepBroker
    {
        #region IModalityProcedureStepBroker Members

        public void LoadTypeForModalityProcedureStep(ModalityProcedureStep mps)
        {
            this.LoadAssociation(mps, mps.Type);
        }

        #endregion
    }
}
