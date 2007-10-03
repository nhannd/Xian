using ClearCanvas.Common;
using ClearCanvas.ImageServer.Database.SqlServer2005;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.SelectBrokers;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.SelectBrokers
{
    /// <summary>
    /// Broker implementation for <see cref="ISelectPatient"/>
    /// </summary>
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class SelectPatient : SelectBroker<PatientSelectCriteria, Patient>, ISelectPatient
    {
        public SelectPatient()
            : base("Patient")
        { }
    }
}