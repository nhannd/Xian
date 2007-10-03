using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.Criteria;

namespace ClearCanvas.ImageServer.Model.SelectBrokers
{
    /// <summary>
    /// Broker for dynamic queries against the <see cref="Patient"/> table.
    /// </summary>
    public interface ISelectPatient : ISelectBroker<PatientSelectCriteria, Patient>
    {
    }
}