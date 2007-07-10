using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IPreviewBroker : IPersistenceBroker
    {
        IList<Order> QueryOrderData(Patient patient);
        IList<RequestedProcedure> QueryRequestedProcedureData(Patient patient);
        IList<ModalityProcedureStep> QueryModalityProcedureStepData(Patient patient);
    }
}
