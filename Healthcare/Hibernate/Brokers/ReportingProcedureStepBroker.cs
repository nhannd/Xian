using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class ReportingProcedureStepBroker : EntityBroker<ReportingProcedureStep, ReportingProcedureStepSearchCriteria>, IReportingProcedureStepBroker
    {
    }
}
