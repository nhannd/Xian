using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IWorklistBroker : IPersistenceBroker
    {
        IList<WorklistItem> GetWorklist();
    }
}
