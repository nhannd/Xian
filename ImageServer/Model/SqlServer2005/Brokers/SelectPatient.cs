using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Database.SqlServer2005;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Criteria;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.Brokers
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
