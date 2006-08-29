using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare.Brokers
{
    public partial interface IPatientProfileBroker : IEntityBroker<PatientProfile, PatientProfileSearchCriteria>
    {
    }
}
