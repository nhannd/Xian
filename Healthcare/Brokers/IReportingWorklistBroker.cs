using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IReportingWorklistBroker : IPersistenceBroker
    {
        IList<ReportingWorklistQueryResult> GetWorklist(Type stepClass, ReportingProcedureStepSearchCriteria criteria);
    }
}
